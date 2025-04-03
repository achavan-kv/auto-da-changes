IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RIDeliveriesReturnsSP]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[RIDeliveriesReturnsSP]
GO

SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER OFF
GO


CREATE PROCEDURE [dbo].[RIDeliveriesReturnsSP]
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : RIDeliveriesReturnsSP.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : RI Interface - Deliveries & Returns
-- Date         : 08 March 2010
--
-- This procedure will create the interface file for Repossessions.
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 07/06/11 jec #3819  Total quantity should only be for Stock items
-- 22/06/11 ip  CR1212 - RI - #3987 - If the Interface method in Country Parameters is set to 'Parts' then we do not want to interface parts 
--				as part of the RI Export. If Interface method is 'RI' then we interface all items including parts.
-- 18/10/11 ip  #8453 - LW74120 - An item with two discounts attached displayed two rows for the item. Should only display one row for the item displaying the sum
--				of the item - sum of the discounts.
-- 19/10/11 IP  #8480 - LW74096 - Spare Parts were incorrectly being exported in the SAR file.
-- 14/11/11 IP  #8625 - LW74243 - Discounts were not being included for repo items.
-- 16/11/11 IP  #8625 - LW74243 - Positive discounts were not being linked to deliveries
-- 21/11/11 IP  #8625 - LW74243 - Repo discount incorrectly appearing in regular file.
-- 01/12/11 IP  #8784 - LW74363 - RI received 1 warranty instead of 2.
-- 14/12/11 IP  #8873 - LW74243 - Error for sales fixes.
-- 03/04/11 IP  #8613 - LW74242 - Header Total and Item detail line total did not balance if value of discount attached to an item was greater than the value of the item.
-- 18/04/12 jec #9891 CR9510 returned warranty RI. If a warranty is returned (repossession, collection, exchange or immediate replacement), No information will be interfaced to RI.
-- ================================================
	@fileName varchar(40) OUT,
	@path varchar(500) OUT,
	@runno INT,
	@Repo BIT,
	@rerun BIT,
	@return INT output		
As	

	--Begin try 	
	set @return = 0	
	
	declare @serviceItemLabour varchar(8),	--IP - 22/06/11 - CR1212 - RI - #3987
    @serviceItemPartsCourts varchar(8),		--IP - 22/06/11 - CR1212 - RI - #3987
    @serviceItemPartsOther varchar(8),		--IP - 22/06/11 - CR1212 - RI - #3987
    @interfaceMethod varchar(5)				--IP - 22/06/11 - CR1212 - RI - #3987	
    
    select @serviceItemLabour = value from countrymaintenance where codename = 'ServiceItemLabour'						--IP - 22/06/11 - CR1212 - RI - #3987
    select @serviceItemPartsCourts = value from countrymaintenance where codename = 'ServiceItemPartsCourts'			--IP - 22/06/11 - CR1212 - RI - #3987
    select @serviceItemPartsOther = value from countrymaintenance where codename = 'ServiceItemPartsOther'				--IP - 22/06/11 - CR1212 - RI - #3987										
	select @interfaceMethod = value from countrymaintenance where codename = 'RIInterfaceOptions'						--IP - 22/06/11 - CR1212 - RI - #3987
	
	declare @fileRunNo CHAR(2),@selectRunNo INT,@Company int
	declare @interfacedate DATETIME,@filedate varCHAR(8), @NumRows INT, @ProcessRow INT,@LineNumber INT, @detail int
	select @interfacedate = (select top 1 DateStart from InterfaceControl where runno=@runno and interface='Cosacs2RI')
	set @filedate= CONVERT(VARCHAR,@interfacedate,12)	--yymmdd
	
	set @fileRunNo=RIGHT('000'+CONVERT(varchar(4),@runno),2)
	
	if @Repo=0		-- Regular run
		Begin		
			truncate TABLE RIDeliveriesReturns
			select @Company=value from CountryMaintenance where codename='RICompanyNo'
		End
	else
		-- Repossession run
		Begin
			truncate TABLE RIDeliveriesReturnsRepo	
			select @Company=value from CountryMaintenance where codename='RICompanyNoRepo'
		End
		
	if @rerun=0
		set @selectRunNo=0
	else 
		set @selectRunNo=@runno
		 
	declare @SAR TABLE  (record VARCHAR(500))
	
	--drop TABLE #DeliveriesReturns	--testonly !!!!!
	--drop TABLE #DeliveryItems	--testonly !!!!!
	
	create TABLE #DeliveriesReturns (
	ID INT IDENTITY,
	AcctNo CHAR(12),
	AgrmtNo INT,
	Quantity FLOAT,
	TransValue MONEY,
	DateTrans SMALLDATETIME,
	CashierID INT,		--jec 09/05/11
	EmpeeNo INT,
	ItemCount int
	)
	
	create TABLE #DeliveryItems (
	Ranking INT,
	LineNumber INT,
	AcctNo CHAR(12),
	AgrmtNo INT,
	ItemNo  CHAR(18),
	Quantity FLOAT,
	TransValue MONEY,
	DiscountValue MONEY,
	DiscountItem CHAR(8),
	DelOrColl CHAR(1),
	ParentItemNo CHAR(18),
	ReturnLocn CHAR(3),
	DeliveryNote CHAR(6),
	Category INT,
	AuthorisedBy int,
	ItemID INT,
	DiscountItemID INT,
	ItemIUPC VARCHAR(18),
	BuffBranch CHAR(3)		
	)
	
	-- Header Summary
	insert into #DeliveriesReturns (acctno,agrmtno,quantity,transvalue,datetrans,CashierID,EmpeeNo, ItemCount)	--jec 09/05/11
	select d.acctno,d.agrmtno, 
			SUM(case 
					when DiscCode.Code IS NULL then quantity  -- only non-discount items counted
					else 0 
				end),	
			SUM(transvalue),MIN(datetrans),ISNULL(RICashierCode,0),u.id,
			SUM(case   
					when DiscCode.Code IS NULL then 1	  -- only non-discount items counted  
					else 0   
				end) AS ItemCount 
	--from delivery d INNER JOIN stockinfo s on d.itemID = s.ID and (s.RepossessedItem=@repo or s.SparePart = 1)--IP - 30/06/11 - CR1212 - RI - #3987	-- 16/05/11 jec
	from delivery d INNER JOIN stockinfo s on d.itemID = s.ID and (s.RepossessedItem=@repo or s.SparePart = 1 or (s.category in (select code from code where category = 'PCDIS')))--IP - 14/11/11 - #8625 - LW74243 --IP - 30/06/11 - CR1212 - RI - #3987	-- 16/05/11 jec
					-- Agreement exists ?? Missing for some Service accounts
					INNER JOIN agreement ag on d.acctno=ag.acctno and d.agrmtno=ag.agrmtno
					LEFT outer JOIN Admin.[User] u on u.id = case	-- jec 09/05/11 
									when ag.createdby=0 then ag.empeenochange else ag.createdby END
					INNER JOIN dbo.courtsperson c ON c.userid = u.id 
					LEFT JOIN dbo.code DiscCode ON DiscCode.Category = 'PCDIS' AND S.ItemType ='N' AND CONVERT(VARCHAR, S.Category) = DiscCode.Code		
					LEFT Outer JOIN stockinfo s2 on d.parentitemid = s2.ID and s2.RepossessedItem=@repo and d.parentitemid!=0	-- #8784 IP - 22/11/11 - #8625 - LW74243	
	where RunNo = @selectRunNo
	and (s.Itemtype='S'						-- stock item
			--or (s.ItemType ='N' and S.category in (select code from Code c where c.category in('WAR','PCDIS')))		--IP - 13/12/11 - #8873 - replaced with the below
			or (s.ItemType ='N' and S.category in (select code from Code c where c.category in('WAR')) and d.delorcoll!='C')					-- #9891 
			or (s.ItemType ='N' and S.category in (select code from Code c where c.category in('PCDIS'))
				and (not exists(select * from delivery d1 INNER JOIN stockinfo s1 on d1.ItemID=s1.ID		
					where d.acctno=d1.acctno and d.agrmtno=d1.agrmtno 
					and d.ParentItemID=d1.ItemID and d1.RunNo=@selectRunNo and d1.contractno='' and s1.ItemType='S')
					
					--IP - 13/12/11 - #8873 - select the discount that has a linked stockitem that is based on the @repo
					or exists(select * from delivery d2 INNER JOIN stockinfo s2 on d2.ItemID=s2.ID											
					where d.acctno=d2.acctno and d.agrmtno=d2.agrmtno 
					and d.ParentItemID=d2.ItemID and d2.RunNo=@selectRunNo and d2.contractno='' and s2.ItemType='S' and s2.RepossessedItem = @repo))
				)			
			or (s.ItemType = 'N' and S.SparePart = 1 and @repo = 1))		--IP - 30/06/11 - CR1212 - RI - #3987
	--and ((d.delorcoll!='R' and @repo=0)		-- regular
	--	or (d.delorcoll='R' and @repo=1))	-- repo
	and d.delorcoll!='R'
	and (((s.SparePart!= 1 and s.itemno not in (@serviceItemLabour, @serviceItemPartsCourts, @serviceItemPartsOther)) and @interfaceMethod = 'Parts') or @interfaceMethod = 'RI') --IP - 19/10/11 - #8480 - lw74096	--IP - 22/06/11 - CR1212 - RI - Check country parameter
		 
	group by d.acctno,d.agrmtno,RICashierCode,u.id  -- jec 09/05/11 
	order by d.acctno
	
	set @NumRows=@@ROWCOUNT
	
	-- Update Courtsperson with Cashier ID		-- jec 09/05/11
	create TABLE #empeeno (id INT IDENTITY,empeeno INT)
	insert into #empeeno
	select distinct Empeeno from #DeliveriesReturns
	where CashierID=0

	declare @maxCashier INT
	select @maxCashier= MAX(RICashierCode) from courtsperson
	
	UPDATE courtsperson
		set RICashierCode=@maxCashier +id	
	from courtsperson c INNER JOIN #empeeno t on c.UserID=t.empeeno 
	-- Update temp table with Cashier ID
	UPDATE #DeliveriesReturns
		set CashierID=RICashierCode	
	from courtsperson c INNER JOIN #DeliveriesReturns dr on c.UserID=dr.empeeno 
	Where dr.CashierID=0
	
	--Item Detail
	insert into #DeliveryItems (Ranking,LineNumber,acctno,agrmtno,ItemNo,quantity,transvalue,
				DiscountValue,DiscountItem,DelorColl,ParentItemNo,ReturnLocn,DeliveryNote,
				Category,AuthorisedBy,ItemID,DiscountItemID,ItemIUPC,BuffBranch)
	select DENSE_RANK() OVER (ORDER BY d.acctno,d.agrmtno),ROW_NUMBER() OVER (PARTITION BY d.acctno,d.agrmtno ORDER BY d.acctno,d.agrmtno,d.parentitemid),
		d.acctno,d.agrmtno,s.IUPC, d.quantity,d.transvalue,case when sd.itemno is null then 0 else d2.Transvalue end,	-- 16/05/11 jec
		case when sd.IUPC is null then ISNULL(sd.Itemno,'') else sd.IUPC end,d.Delorcoll,ISNULL(sp.IUPC,''),d.RetStockLocn,d.buffno,	-- 17/05/11 jec
		case when s.itemtype='S' then s.category else s.category end,'',d.ItemID,d2.ItemID,s.IUPC,d.BuffBranchNo
			
	--from delivery d INNER JOIN stockinfo s on d.itemID = s.ID and (s.RepossessedItem=@repo or s.SparePart = 1)--IP-30/06/11 - CR1212 - RI - #3987-- 16/05/11 jec
	from delivery d INNER JOIN stockinfo s on d.itemID = s.ID and (s.RepossessedItem=@repo or s.SparePart = 1 or (s.category in (select code from code where category = 'PCDIS'))) --IP - 14/11/11 - #8625 - LW74243 --IP-30/06/11 - CR1212 - RI - #3987-- 16/05/11 jec
			LEFT outer JOIN delivery d2 on d.acctno=d2.acctno and d.agrmtno=d2.agrmtno 
					and d.itemID=d2.ParentItemID and d2.RunNo=@selectRunNo and d2.contractno=''	-- 16/05/11 jec
					-- match correct discount to item where there may have been Del and Coll -- jec 28/04/11
					--and ((d.transvalue>0 and d2.transvalue<0) -- Delivery 
					--	or (d.transvalue<0 and d2.transvalue>0))	-- Collection
						and ((d.transvalue>0 and  d2.delorcoll = 'D') -- Delivery			-- IP - 16/11/11 - #8625 - LW74243 - Collection of discounts now marked as 'C'
						or (d.transvalue<0 and d2.delorcoll = 'C'))	-- Collection
			LEFT outer JOIN stockinfo sd on ISNULL(d2.itemID,'') = sd.ID		-- 16/05/11 jec
					and ISNULL(sd.itemtype,'')='N' and ISNULL(Sd.category,'') in (select code from Code c1 where c1.category in('PCDIS'))
			-- Agreement exists ?? Missing for some Service accounts
			INNER JOIN agreement ag on d.acctno=ag.acctno and d.agrmtno=ag.agrmtno
			-- Parent Item
			--LEFT Outer JOIN stockinfo sp on d.ParentItemID = sp.ID and sp.RepossessedItem=@repo	-- 16/05/11 jec
			--LEFT Outer JOIN stockinfo sp on d2.ParentItemID = sp.ID and sp.RepossessedItem=@repo and d2.ParentItemID!=0			-- #8784 IP - 22/11/11 - #8625 - LW74243
			LEFT Outer JOIN stockinfo sp on d.ParentItemID = sp.ID and sp.RepossessedItem=@repo and d.ParentItemID!=0				--IP - 13/12/11 - #8873 -- #8784 IP - 22/11/11 - #8625 - LW74243 
	where d.RunNo = @selectRunNo	
	and (s.Itemtype='S'						-- stock item
			or (s.ItemType ='N' and S.category in (select code from Code c where c.category in('WAR')) and d.delorcoll!='C')					-- #9891
			-- Required for Discounts where Stock item not in current run
			--or (s.ItemType ='N' and S.category in (select code from Code c where c.category in('PCDIS')							--IP - 13/12/11 - #8873 - Replaced with the below
			--		and not exists(select * from delivery d3 INNER JOIN stockinfo s2 on d3.ItemID=s2.ID		-- 16/05/11 jec
			--		where d.acctno=d3.acctno and d.agrmtno=d3.agrmtno 
			--		and d.itemID=d3.ItemID and d3.RunNo=@selectRunNo and d3.contractno='' and s2.ItemType='S')))	-- 16/05/11 jec
			
			--IP - 13/12/11 - #8873 - select the discount that has a linked stockitem that is based on the @repo
			or (s.ItemType ='N' and S.category in (select code from Code c where c.category in('PCDIS'))
					and (not exists(select * from delivery d3 INNER JOIN stockinfo s2 on d3.ItemID=s2.ID		
					where d.acctno=d3.acctno and d.agrmtno=d3.agrmtno 
					and d.ParentItemID=d3.ItemID and d3.RunNo=@selectRunNo and d3.contractno='' and s2.ItemType='S')
					
					or exists(select * from delivery d4 INNER JOIN stockinfo s3 on d4.ItemID=s3.ID						
					where d.acctno=d4.acctno and d.agrmtno=d4.agrmtno 
					and d.ParentItemID=d4.ItemID and d4.RunNo=@selectRunNo and d4.contractno='' and s3.ItemType='S' and s3.RepossessedItem = @repo))
				)	
					
			or(s.ItemType = 'N' and s.SparePart = 1 and @repo = 1)		--IP - 30/06/11 - CR1212 - RI - #3987
			)
	--and ((d.delorcoll!='R' and @repo=0)		-- regular
	--	or (d.delorcoll='R' and @repo=1))	-- repo
	and d.delorcoll!='R'		
	and (((s.SparePart!= 1 and s.itemno not in (@serviceItemLabour, @serviceItemPartsCourts, @serviceItemPartsOther)) and @interfaceMethod = 'Parts') or @interfaceMethod = 'RI') --IP - 19/10/11 - #8480 - LW74096	--IP - 22/06/11 - CR1212 - RI - Check country parameter
		 	
	order by d.acctno
	
	-- Update with employee who first authorised discount
	UPDATE #DeliveryItems
	set AuthorisedBy = da.AuthorisedBy
	from #DeliveryItems i INNER JOIN  DiscountsAuthorised da on i.AcctNo = da.AcctNo 
							and i.AgrmtNo = da.AgrmtNo and	i.DiscountItemID=da.ItemID	--	16/05/11 jec i.DiscountItem=da.DiscountItemNo
							--and i.itemno=da.ParentItemNo
							and i.itemID=da.ParentItemID		-- 16/05/11 jec

	set @ProcessRow=0
	
	-- Create Interface data
	While @ProcessRow<@NumRows
	BEGIN
		set @ProcessRow=@ProcessRow+1
		set @LineNumber=1
		--insert into RIDeliveriesReturns
		insert into @SAR
		--Header
		select distinct '1'+ CONVERT(CHAR(22),d.AcctNo+RIGHT('000000000' +CONVERT(VARCHAR(9),d.AgrmtNo),10)) + SUBSTRING(d.acctno,1,3) +
			Right('00'+CONVERT(varCHAR(2),@Company),2) + ISOCountryCode + '000' +	-- cashier CONVERT(CHAR(6),ag.CreatedBy) + RIGHT('00'+ CONVERT(VARCHAR(3),CashierID),3) + -- jec 09/05/11 
			case when a.accttype in('C') then '04' when a.accttype in('S') then '05'  else '00' end + '1' +	-- trans type CONVERT(CHAR(1),ag.PaymentMethod) + 
			Right('00'+CONVERT(varCHAR(2),DATEPART(hh,dr.DateTrans)),2)+Right('00'+CONVERT(varCHAR(2),DATEPART(mi,dr.DateTrans)),2) +
			CONVERT(CHAR(8),@interfacedate,112) + '00001' +'02' + case when ISNULL(ac.Code,'')='TE' then '1' else '0' end +
			case when dr.Quantity<0 then '-' else ' ' end +
			RIGHT('0000000' + CONVERT(varCHAR(8),CONVERT(int,ABS(dr.Quantity))),5)+
			case when dr.TransValue<0 then '-' else ' ' end +
			RIGHT('0000000000' + CONVERT(varCHAR(10),CONVERT(DECIMAL(11,2),ABS(dr.TransValue))),10)+
			CONVERT(CHAR(25),ag.SOA) + CONVERT(CHAR(25),ISNULL(c.CodeDescript,'NoDescription'))
			+ SPACE(239) -- padding to 362 + SPACE(231) -- padding		
		
		From #DeliveriesReturns dr 
				INNER JOIN Delivery d on dr.AcctNo=d.AcctNo and dr.AgrmtNo=d.AgrmtNo
				--INNER JOIN StockItem s on d.itemno = s.itemno and d.Stocklocn=s.Stocklocn
				INNER JOIN StockInfo s on d.ItemID=s.id		-- 16/05/11 jec
				INNER JOIN Agreement ag on d.AcctNo=ag.AcctNo and d.AgrmtNo=ag.AgrmtNo 
				INNER JOIN Acct a on dr.AcctNo=a.AcctNo 
				LEFT OUTER JOIN AcctCode ac on d.AcctNo=ac.AcctNo and ac.code ='TE'		-- jec 27/04/11
				LEFT OUTER JOIN code c on ag.soa=c.code and c.category='SOA' , Country
		Where RunNo=@selectRunNo		
		and (Itemtype='S'						-- stock item
			or (ItemType ='N' and S.category in (select code from Code c where c.category in('WAR','PCDIS')))
			or (ItemType = 'N' and s.SparePart = 1 and @repo = 1)		--IP - 30/06/11 - CR1212 - RI - #3987
			)	-- warranty
		and dr.id=@ProcessRow AND ItemCount > 0
		
		--Set @LineNumber=(select MAX(linenumber) from #DeliveryItems	di INNER JOIN #DeliveriesReturns dr on dr.id=di.Ranking
		--			where dr.id=@ProcessRow)
		--set @detail=-1
		
		--Detail
		
		--while @detail=@LineNumber
		--Begin
		--insert into RIDeliveriesReturns
		insert into @SAR
		Select --CONVERT(CHAR(5),@ProcessRow) +
		'3'+SPACE(122)+		-- length of Header details (excl Row type) change if Header changes
		--+RIGHT('00'+CONVERT(VARCHAR(2),LineNumber),2)+CONVERT(CHAR(18),ISNULL(i.ItemIUPC,'IUPC missing'))+
		+RIGHT('00'+CONVERT(VARCHAR(2),max(LineNumber)),2)+CONVERT(CHAR(18),ISNULL(i.ItemIUPC,'IUPC missing'))+										--IP - 18/10/11 - #8453 - LW74120
		case when i.Quantity<0 then '-' else ' ' end +
		RIGHT('000000' + CONVERT(varCHAR(8),CONVERT(int,ABS(i.Quantity))),5) +
		--case when max(i.TransValue)<0 then '-' else ' ' end +																						--IP - 18/10/11 - #8453 - LW74120
		case when (max(i.TransValue) + sum(i.DiscountValue)) <0 then '-' else ' ' end +																--IP - 03/04/11 - #8613 - LW74242 	
		--RIGHT('0000000000' + CONVERT(varCHAR(10),CONVERT(DECIMAL(9,2),ABS(i.TransValue+i.DiscountValue))),10)+	-- Add -ve discount
		RIGHT('0000000000' + CONVERT(varCHAR(10),CONVERT(DECIMAL(9,2),ABS(max(i.TransValue)+sum(i.DiscountValue)))),10)+	-- Add -ve discount		--IP - 18/10/11 - #8453 - LW74120
		--case when i.DelOrColl ='D' then '1' else '2' end +
		i.DelOrColl+
		--case when i.DiscountValue<0 then '-' else ' ' end +
		case when sum(i.DiscountValue)<0 then '-' else ' ' end +																					--IP - 18/10/11 - #8453 - LW74120
		--RIGHT('0000000000' + CONVERT(varCHAR(10),CONVERT(DECIMAL(9,2),ABS(i.DiscountValue))),10)+
		RIGHT('0000000000' + CONVERT(varCHAR(10),CONVERT(DECIMAL(9,2),ABS(sum(i.DiscountValue)))),10)+												--IP - 18/10/11 - #8453 - LW74120
		CONVERT(CHAR(5),ISNULL(i.AuthorisedBy,''))+ CONVERT(CHAR(25),ISNULL(cda.FullName,'')) +
		--CONVERT(CHAR(8),i.DiscountItem) + CONVERT(CHAR(25),ISNULL(sd.itemdescr1,'')) + 
		CONVERT(CHAR(8),max(i.DiscountItem)) + CONVERT(CHAR(25),ISNULL(max(sd.itemdescr1),'')) +													--IP - 18/10/11 - #8453 - LW74120
		CONVERT(CHAR(5),ag.empeenosale) + --CONVERT(CHAR(8),ag.createdby) +
		case when ac.code ='STAF' then '1' else '0' end +
		CONVERT(CHAR(18),i.ParentItemNo) + 
		CONVERT(CHAR(22),i.AcctNo+RIGHT('000000000' +CONVERT(VARCHAR(9),i.AgrmtNo),10)) +
		CONVERT(CHAR(3),case when ISNULL(i.ReturnLocn,0)=0 then '  ' else i.ReturnLocn end) +	-- jec 27/04/11
		--CONVERT(CHAR(3),i.BuffBranch)+CONVERT(CHAR(6),i.DeliveryNote)
		CONVERT(CHAR(3),i.BuffBranch)+CONVERT(CHAR(6),max(i.DeliveryNote))																			--IP - 18/10/11 - #8453 - LW74120
		+SPACE(69)	-- padding to 362 +SPACE(61)	-- padding
		
		From #DeliveryItems i --LEFT OUTER JOIN DiscountsAuthorised da on i.AcctNo = da.AcctNo 
							--and i.AgrmtNo = da.AgrmtNo and i.itemno=da.ParentItemNo
				LEFT OUTER JOIN Admin.[User] cda on ISNULL(i.AuthorisedBy,'')=cda.Id
				--LEFT OUTER JOIN stockinfo sd on i.DiscountItem=sd.itemno 
				LEFT OUTER JOIN stockinfo sd on i.DiscountItemID=sd.ID	-- 16/05/11 jec
				LEFT OUTER JOIN agreement ag on ag.acctno=i.acctno and ag.agrmtno = i.AgrmtNo
				LEFT OUTER JOIN acctCode ac on i.acctno = ac.acctno and ac.code='STAF'
				
		where Ranking=@ProcessRow 
			and not exists -- exclude discount items where No stock items exists for current run
				(select * from code c where i.category=CAST(c.code as INT) and c.category='PCDIS')
		group by i.Quantity,i.DelOrColl, i.AuthorisedBy, cda.empeename, ag.empeenosale, ac.code,i.ParentItemNo, i.AcctNo, i.Agrmtno, i.ReturnLocn, i.BuffBranch, i.ItemIUPC, i.DeliveryNote		-- #8784 --IP - 18/10/11 - #8453 - LW74120
		
		-- Discount detail
		--insert into RIDeliveriesReturns
		insert into @SAR
		Select --CONVERT(CHAR(5),@ProcessRow) +
		'5'+SPACE(122)+SPACE(170)		-- length of Header & Detail details (excl Row type) change if Header/Detail changes
		+CONVERT(CHAR(18),ISNULL(sp.IUPC,'IUPC missing'))+RIGHT('00'+CONVERT(VARCHAR(2),LineNumber),2)+
		case when di.DiscountValue<0 then '-' else ' ' end +
		RIGHT('0000000000' + CONVERT(varCHAR(10),CONVERT(DECIMAL(9,2),ABS(di.DiscountValue))),10)+
		CONVERT(CHAR(5),ISNULL(di.AuthorisedBy,'')) + CONVERT(CHAR(8),di.DiscountItem) +
		CONVERT(CHAR(25),ISNULL(s.itemdescr1,'No Description'))
		
		From #DeliveryItems di --LEFT OUTER JOIN DiscountsAuthorised da on di.AcctNo = da.AcctNo 		
					--LEFT OUTER JOIN Stockinfo s on s.itemno=di.itemno and s.id=di.DiscountItemID
					LEFT OUTER JOIN Stockinfo s on s.id=di.DiscountItemID
					INNER JOIN stockinfo sp on sp.ID=di.ItemID		-- 16/05/11 jec
		
		where Ranking=@ProcessRow and ISNULL(di.DiscountItem,'')!=''	-- 17/05/11 jec
		
		--End
		
	END	
	
	-- Now Update RunNo
	
	--IP - 14/12/11 - #8873
	UPDATE delivery
		set Runno=@Runno
	From Delivery d INNER JOIN StockInfo s on d.ItemID=s.id    -- 16/05/11 jec
	Where Runno=0
		and @rerun=0	-- only if not a rerun
		and d.delorcoll!='R'
		and ((@repo=0	-- Regular pass
				--and RepossessedItem=0	-- Regular products 
				and 
				(s.ItemType = 'S' and s.RepossessedItem=0	-- Regular products 
						or (s.ItemType ='N' and s.category in (select code from Code c where c.category in('PCDIS'))
								and exists(select * from delivery d1 INNER JOIN stockinfo s1 on d1.ItemID=s1.ID						
								where d.acctno=d1.acctno and d.agrmtno=d1.agrmtno 
								and d.ParentItemID=d1.ItemID and d1.contractno='' and s1.ItemType='S' and s1.RepossessedItem = @repo)
							)
						or(s.ItemType = 'N' and s.category not in (select code from Code c where c.category in('PCDIS')))	
					)
				-- not Spare Part 
				and (s.SparePart!= 1 and s.itemno not in (@serviceItemLabour, @serviceItemPartsCourts, @serviceItemPartsOther)))  -- not Spare part --IP - 19/10/11 - #8480 - LW74096
			or (@repo=1		-- Repo pass
				and ((s.ItemType = 'S' and s.RepossessedItem=1		-- Repo products
						or (s.ItemType ='N' and s.category in (select code from Code c where c.category in('PCDIS'))
								and (exists(select * from delivery d1 INNER JOIN stockinfo s1 on d1.ItemID=s1.ID						
										where d.acctno=d1.acctno and d.agrmtno=d1.agrmtno and d.ParentItemID=d1.ItemID and d1.contractno='' 
										and s1.ItemType='S' and s1.RepossessedItem = @repo))
							)	
						)
	
					or ((SparePart=1 OR s.itemno in (@serviceItemLabour, @serviceItemPartsCourts, @serviceItemPartsOther))		-- Spare part
													and @interfaceMethod != 'Parts')
													)))			-- Parts to RI		
		
		--and ((@repo=0	-- Regular pass
		--		and RepossessedItem=0	-- Regular products 
		--		-- not Spare Part 
		--		and (s.SparePart!= 1 and s.itemno not in (@serviceItemLabour, @serviceItemPartsCourts, @serviceItemPartsOther)))  -- not Spare part --IP - 19/10/11 - #8480 - LW74096
		--	or (@repo=1		-- Repo pass
		--		and (RepossessedItem=1		-- Repo products
		--		or ((SparePart=1 OR s.itemno in (@serviceItemLabour, @serviceItemPartsCourts, @serviceItemPartsOther))		-- Spare part
		--											and @interfaceMethod != 'Parts'))))			-- Parts to RI			
		
	if @Repo=0
	Begin
		Insert into RIDeliveriesReturns select * from @SAR
	End
	else
	Begin
		Insert into RIDeliveriesReturnsRepo select * from @SAR
	End
	
	set @fileName = 'SAR'+@filedate +@fileRunNo+'.FTE'
			
	--end try
	
	--Begin catch
	--		set @fileName = 'Error '
	--End catch
	

GO

-- end end end end end end end end end end end end end end end end end end end end end end end

