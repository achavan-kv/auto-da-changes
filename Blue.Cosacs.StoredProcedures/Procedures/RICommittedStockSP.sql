IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RICommittedStockSP]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[RICommittedStockSP]
GO

SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[RICommittedStockSP]
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : RICommittedStockSP.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : RI Interface - CommittedStock
-- Author       : John Croft
-- Date         : 08 March 2011
--
-- This procedure will create the interface file for Committed Stock.
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 16/06/11 jec  Cater for repossessed committed stock
-- 17/10/11 jec  #8409 LW74095 use Return location for collections
-- 16/12/11 ip   #8929 CR8928 RI Warranties
-- 16/12/11 ip   #8925 - Spare Parts incorrectly appear in regular file
-- 24/01/12 jec  #9499 LW74569 -  BR787 Transactions, Spare parts 
-- 16/01/13 ip   #11059 LW75330 - Merged from CoSACS 6.5
-- ================================================
	
	@fileName varchar(40) OUT,
	@path varchar(500) OUT,
	@runno INT,
	@rerun BIT,
	@repo BIT,
	@return INT output
		
As	
	set @return = 0
	
	if @Repo=0
		truncate TABLE RICommittedStock
	else
		truncate TABLE RICommittedStockRepo
	
	declare @fileRunNo CHAR(2),@selectRunNo int
	declare @interfacedate DATETIME,@filedate varCHAR(8)
	declare @QTY TABLE  (record VARCHAR(100))
	select @interfacedate = (select top 1 DateStart from InterfaceControl where runno=@runno and interface='Cosacs2RI')
	set @filedate= CONVERT(VARCHAR,@interfacedate,12)		--yymmdd
	set @fileRunNo=RIGHT('000'+CONVERT(varchar(4),@runno),2)
	
	declare @serviceItemLabour varchar(8),	--IP - 16/12/11 - #8925
    @serviceItemPartsCourts varchar(8),		--IP - 16/12/11 - #8925	
    @serviceItemPartsOther varchar(8),		--IP - 16/12/11 - #8925	
    @interfaceMethod varchar(5)				--IP - 16/12/11 - #8925		
    
    select @serviceItemLabour = value from countrymaintenance where codename = 'ServiceItemLabour'						--IP - 16/12/11 - #8925
    select @serviceItemPartsCourts = value from countrymaintenance where codename = 'ServiceItemPartsCourts'			--IP - 16/12/11 - #8925
    select @serviceItemPartsOther = value from countrymaintenance where codename = 'ServiceItemPartsOther'				--IP - 16/12/11 - #8925										
	select @interfaceMethod = value from countrymaintenance where codename = 'RIInterfaceOptions'						--IP - 16/12/11 - #8925
	
	if @rerun=0
		set @selectRunNo=0
	else 
		set @selectRunNo=@runno	
	
	insert into @QTY 
	Select ISOCountryCode +	
			--case when S.category in (select code from Code c where c.category ='WAR') then left(l.acctno, 3)							--IP - 16/12/11 - #8929  
			case when convert(varchar, S.category) in (select code from Code c where c.category ='WAR') then left(l.acctno, 3)	--IP - 16/01/13		--IP - 16/12/11 - #8929  
				 when StockLocn !=DelNoteBranch and (source ='GRTCancel' or source ='GRTExchange')		-- #9499 jec 24/01/12
										and DelNoteBranch!=0 then CONVERT(CHAR(3),DelNoteBranch)		-- #8409 jec Delnotebranch set to RetStocklocn
				 else CONVERT(CHAR(3),StockLocn) end
			+CONVERT(CHAR(18),ISNULL(s.IUPC,'IUPC missing'))+
			case when QuantityAfter-QuantityBefore<0 then '-' else ' ' end +
			RIGHT('000000' + CONVERT(varCHAR(5),CONVERT(Int,ABS(QuantityAfter-QuantityBefore))),5)+
			CONVERT(CHAR(22),AcctNo+RIGHT('000000000' +CONVERT(VARCHAR(9),AgrmtNo),10))+CONVERT(VARCHAR,DateChange,23)
	
	--From LineitemAudit l INNER JOIN StockInfo s on l.ItemID = s.ID	and s.RepossessedItem=@repo		-- jec 16/06/11 
	From LineitemAudit l INNER JOIN StockInfo s on l.ItemID = s.ID	and (s.RepossessedItem=@repo or s.SparePart = 1)					--IP - 16/12/11 - #8925 -- jec 16/06/11 
					, Country
	Where RunNo=@selectRunNo
		and QuantityBefore!=QuantityAfter		-- Quantity change
		and (Itemtype='S'						-- stock item
			or (ItemType ='N' and S.category in (select code from Code c where c.category ='WAR')))	-- warranty
		--IP - 16/12/11 - #8925
		and (
				(
					(s.SparePart!= 1 and s.itemno not in (@serviceItemLabour, @serviceItemPartsCourts, @serviceItemPartsOther) and (@interfaceMethod = 'Parts' or @interfaceMethod = 'RI'))	-- not spare				
					
					or ((s.SparePart= 1 or s.itemno in (@serviceItemLabour, @serviceItemPartsCourts, @serviceItemPartsOther)) and @interfaceMethod = 'RI' and @Repo = 1) -- spare - repo process
				) 
			
			
			) --IP - 19/10/11 - #8480 - lw74096	--IP - 22/06/11 - CR1212 - RI - Check country parameter
	order by datechange
	
	-- Now Update RunNo
	
	UPDATE LineitemAudit
		set Runno=@Runno
	--From LineitemAudit l INNER JOIN StockInfo s on l.ItemID=s.id and s.RepossessedItem=@repo		-- 16/06/11 jec
	From LineitemAudit l INNER JOIN StockInfo s on l.ItemID=s.id and (s.RepossessedItem=@repo or s.SparePart = 1)		--IP - 16/12/11 - #8925 -- 16/06/11 jec
	Where Runno=0
	and @rerun=0	-- only if not a rerun 
	--IP - 16/12/11 - #8925
	and (
				(
					(s.SparePart!= 1 and s.itemno not in (@serviceItemLabour, @serviceItemPartsCourts, @serviceItemPartsOther) and (@interfaceMethod = 'Parts' or @interfaceMethod = 'RI'))	-- not spare				
					
					or ((s.SparePart= 1 or s.itemno in (@serviceItemLabour, @serviceItemPartsCourts, @serviceItemPartsOther)) and @interfaceMethod = 'RI' and @Repo = 1) -- spare - repo process
				) 
			
			
		)
		
	if @Repo=0
	Begin
		Insert into RICommittedStock select * from @QTY
	End
	else
	Begin
		Insert into RICommittedStockRepo select * from @QTY
	End
	
	set @fileName = 'QTY'+@filedate +@fileRunNo+'.FTE'			

GO
-- end end end end end end end end end end end end end end end end end end end end end end end
