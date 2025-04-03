IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RIDeliveryTransfersSP]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[RIDeliveryTransfersSP]
GO

SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER OFF
GO


CREATE PROCEDURE [dbo].[RIDeliveryTransfersSP]
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : RIDeliveryTransfersSP.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : RI Interface - Delivery Transfers
-- Author       : John Croft
-- Date         : 08 March 2010
--
-- This procedure will create the interface file for Delivery Transfers.
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 22/06/11  IP  CR1212 - RI - #3987 - If the Interface method in Country Parameters is set to 'Parts' then we do not want to interface parts 
--				as part of the RI Export. If Interface method is 'RI' then we interface all items including parts.
-- 19/09/11  IP  #8205 - Delivery Transfer file was empty when processing Goods Return Collection. Added check on return stock location being different 
--				 to the branch the account was created.
-- 26/09/11  jec #8205 select return location if not a delivery
-- 20/10/11  IP  #8407 - LW74093 - For an item of quantity 2, that had 2 warranties attached, the warranty was displayed twice for an account. Now summing the quantity and taking max delivery note number
--				 and grouping by distinct columns.
-- 04/11/11 jec  #8480 - LW74096 -/#8575 LW74196  Spare Parts were incorrectly being exported in the DTF file.
-- ================================================
	
	@fileName varchar(40) OUT,
	@path varchar(500) OUT,
	@runno INT,
	@Repo BIT,
	@rerun BIT,
	@return INT output	
	
As	
	
	set @return = 0
	
	declare @serviceItemLabour varchar(8),	--IP - 22/06/11 - CR1212 - RI - #3987
    @serviceItemPartsCourts varchar(8),		--IP - 22/06/11 - CR1212 - RI - #3987
    @serviceItemPartsOther varchar(8),		--IP - 22/06/11 - CR1212 - RI - #3987
    @interfaceMethod varchar(5)				--IP - 22/06/11 - CR1212 - RI - #3987	
    
    select @serviceItemLabour = value from countrymaintenance where codename = 'ServiceItemLabour'						--IP - 22/06/11 - CR1212 - RI - #3987
    select @serviceItemPartsCourts = value from countrymaintenance where codename = 'ServiceItemPartsCourts'			--IP - 22/06/11 - CR1212 - RI - #3987
    select @serviceItemPartsOther = value from countrymaintenance where codename = 'ServiceItemPartsOther'				--IP - 22/06/11 - CR1212 - RI - #3987										
	select @interfaceMethod = value from countrymaintenance where codename = 'RIInterfaceOptions'						--IP - 22/06/11 - CR1212 - RI - #3987
	
	if @Repo=0
		truncate TABLE RIDeliveryTransfers
	else
		truncate TABLE RIDeliveryTransfersRepo
	
	declare @fileRunNo CHAR(2),@selectRunNo int
	declare @interfacedate DATETIME,@filedate varCHAR(8)
	declare @DTF TABLE  (record VARCHAR(100))
	select @interfacedate = (select top 1 DateStart from InterfaceControl where runno=@runno and interface='Cosacs2RI')
	set @filedate= CONVERT(VARCHAR,@interfacedate,12)	--yymmdd
	
	set @fileRunNo=RIGHT('000'+CONVERT(varchar(4),@runno),2)
	
	if @rerun=0
		set @selectRunNo=0
	else 
		set @selectRunNo=@runno
		
	--insert into RIDeliveryTransfers 
	insert into @DTF
	Select Distinct ISOCountryCode +
			case when isnull(d.retstocklocn,0)!=0 and isnull(d.retstocklocn,'000')!=LEFT(d.AcctNo,3) and d.DelorColl='C' then CONVERT(CHAR(3),d.retstocklocn)  -- jec 26/09/11
				else CONVERT(CHAR(3),d.StockLocn)
			end +
			LEFT(d.AcctNo,3)+CONVERT(CHAR(11),ISNULL(s.SKU,''))+	-- jec 28/04/11 CONVERT(CHAR(11),d.ItemNo)+
			CONVERT(CHAR(18),ISNULL(s.IUPC,'IUPC missing'))+ 
			--case when d.Quantity<0 then '-' else ' ' end +
			case when sum(d.Quantity)<0 then '-' else ' ' end +													--IP - 20/10/11 - #8407 - LW74093
			--RIGHT('00000000' + CONVERT(varCHAR(8),CONVERT(int,ABS(d.Quantity))),5)+
			RIGHT('00000000' + CONVERT(varCHAR(8),CONVERT(int,ABS(sum(d.Quantity)))),5)+						--IP - 20/10/11 - #8407 - LW74093
			--CONVERT(CHAR(3),d.BuffBranchNo)+RIGHT('000000' + CONVERT(VARCHAR(6),d.BuffNo),6) +
			CONVERT(CHAR(3),d.BuffBranchNo)+RIGHT('000000' + CONVERT(VARCHAR(6),max(d.BuffNo)),6) +				--IP - 20/10/11 - #8407 - LW74093
			CONVERT(CHAR(22),AcctNo+RIGHT('000000000' +CONVERT(VARCHAR(9),AgrmtNo),10))+
			CONVERT(VARCHAR,@interfacedate,23)+
			--case when d.DelorColl='R' and d.Quantity<0 then 'C'		-- repossession
			case when d.DelorColl='R' and sum(d.Quantity)<0 then 'C'	-- repossession							--IP - 20/10/11	- #8407 - LW74093
				 --when d.DelorColl='R' and d.Quantity>0 then 'D'		-- redelivery after repossession
				  when d.DelorColl='R' and sum(d.Quantity)>0 then 'D'	-- redelivery after repossession		--IP - 20/10/11	- #8407 - LW74093
				 else d.DelorColl end		-- jec 28/04/11
	
	From Delivery d INNER JOIN StockInfo s on d.ItemID= s.ID and (s.RepossessedItem=@repo or s.SparePart = 1)--IP - 30/06/11 - CR1212 - RI - #3987 -- jec 28/04/11 
								, Country
	Where RunNo=@selectRunNo
		and delorcoll!='R'		-- 16/05/11 jec
		--and d.StockLocn!=LEFT(d.AcctNo,3)
		and ((d.StockLocn!=LEFT(d.AcctNo,3) and d.delorcoll = 'D') or (d.delorcoll = 'C' and (isnull(d.retstocklocn,0)!=0 and isnull(d.retstocklocn,'000')!=LEFT(d.AcctNo,3)))) --IP - 19/09/11 - RI - #8205
		and (Itemtype='S'						-- stock item
			or (ItemType ='N' and S.category in (select code from Code c where c.category ='WAR'))
			or(ItemType = 'N' and s.SparePart = 1 and @repo = 1)			--IP - 30/06/11 - CR1212 - RI - #3987
			)	-- warranty
		and (((s.SparePart!= 1 and s.itemno not in (@serviceItemLabour, @serviceItemPartsCourts, @serviceItemPartsOther)) and @interfaceMethod = 'Parts') or @interfaceMethod = 'RI') -- #8480/#8575 	
		 
	group by ISOCountryCode, d.retstocklocn, d.AcctNo, d.DelorColl, d.StockLocn, s.SKU, s.IUPC, d.BuffBranchNo, AgrmtNo,DateDel		--IP - 20/10/11 - #8407 - LW74093
	
	if @Repo=0
	Begin
		Insert into RIDeliveryTransfers select * from @DTF
		--set @fileName = 'DTF'+@filedate +@fileRunNo+'.FTE'		-- test only
	End
	else
	Begin
		Insert into RIDeliveryTransfersRepo select * from @DTF
		--set @fileName = 'DTFR'+@filedate +@fileRunNo+'.FTE'		-- test only
	End
	
	set @fileName = 'DTF'+@filedate +@fileRunNo+'.FTE'
	

GO
-- end end end end end end end end end end end end end end end end end end end end end end end

