IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RIRepossessionsSP]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[RIRepossessionsSP]
GO

SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[RIRepossessionsSP]
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : RIRepossessionsSP.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : RI Interface - Repossessions
-- Author       : John Croft
-- Date         : 08 March 2010
--
-- This procedure will create the interface file for Repossessions.
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 26/09/11 jec  #8204 Use Return Item Id when retrieving Cost price.
-- 18/11/11 IP   #8647 LW74285 - Repossession was not being interfaced due to the repossession item not existing at stocklocation.
--				 should use return stock location.
-- 18/04/12 jec #9891 CR9510 returned warranty RI. If a warranty is returned (repossession, collection, exchange or immediate replacement), No information will be interfaced to RI.
-- ================================================
	
	@fileName varchar(40) OUT,
	@path varchar(500) OUT,
	@runno INT,
	@rerun BIT,
	@return INT output		
	
As	
	set @return = 0
	truncate TABLE RIRepossessions
	
	declare @fileRunNo CHAR(2),@selectRunNo int
	declare @interfacedate DATETIME,@filedate varCHAR(8)
	select @interfacedate = (select top 1 DateStart from InterfaceControl where runno=@runno and interface='Cosacs2RI')
	set @filedate= CONVERT(VARCHAR,@interfacedate,12)	--yymmdd
	set @fileRunNo=RIGHT('000'+CONVERT(varchar(4),@runno),2)
	
	if @rerun=0
		set @selectRunNo=0
	else 
		set @selectRunNo=@runno
	
	insert into RIRepossessions 
	Select ISOCountryCode +				
			CONVERT(CHAR(3),d.RetStockLocn)+CONVERT(CHAR(18),ISNULL(s.IUPC,'IUPC missing'))+			
			' ' +  -- No negative sign
			RIGHT('00000' + CONVERT(varCHAR(8),CONVERT(Int,ABS(d.Quantity))),5)+
			' ' +	-- sign for Cost price always space  -- jec 28/04/11
			RIGHT('00000000' + CONVERT(varCHAR(9),CONVERT(DECIMAL(9,2),isnull(s.CostPrice,0))),10)+
			' ' + -- No negative sign
			RIGHT('00000000' + CONVERT(varCHAR(9),CONVERT(DECIMAL(9,2),case when d.Quantity!=0 then ABS(d.transvalue/ d.Quantity) else 0 end)),10)+
			CONVERT(CHAR(22),AcctNo+RIGHT('000000000' +CONVERT(VARCHAR(9),AgrmtNo),10)) +
			CONVERT(CHAR(10),CONVERT(VARCHAR(3),buffbranchno)+CONVERT(VARCHAR(7),buffno))
			
	
	--From Delivery d INNER JOIN StockItem s on d.ItemID= s.ID and d.Stocklocn=s.Stocklocn	-- jec 28/04/11 
	--From Delivery d INNER JOIN StockItem s on d.RetItemID= s.ID and d.Stocklocn=s.Stocklocn		-- jec 26/09/11
	From Delivery d INNER JOIN StockItem s on d.RetItemID= s.ID and d.Retstocklocn=s.Stocklocn		--IP - 18/11/11 - #8647 - LW74285 -- jec 26/09/11
					, Country
	Where RunNo=@selectRunNo
		and DelorColl='R'		-- Repossession
		and (Itemtype='S')						-- stock item
			-- #9891 or (ItemType ='N' and S.category in (select code from Code c where c.category ='WAR')))	-- warranty
	
	-- Now Update RunNo
	
	UPDATE delivery
		set Runno=@Runno
	Where Runno=0
		and @rerun=0	-- only if not a rerun 
		and delorcoll='R'
		
	
	set @fileName = 'RPO'+@filedate +@fileRunNo+'.FTE'

GO

-- end end end end end end end end end end end end end end end end end end end end end end end

