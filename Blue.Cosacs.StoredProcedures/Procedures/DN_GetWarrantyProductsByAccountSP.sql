

/****** Object:  StoredProcedure [dbo].[DN_GetWarrantyProductsByAccountSP]    Script Date: 09/28/2006 15:45:32 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[DN_GetWarrantyProductsByAccountSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DN_GetWarrantyProductsByAccountSP]

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Peter Chong / Alex A
-- Create date: 28-Sep-2006
-- Description:	Return a list of warranties for a given account Created for CR 822
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 08/06/11  IP  CR1212 - RI changes. Changed joins to use ItemID and ParentItemID. WarrantyBand.WarrantyLength now holds months. Previously held years, therefore
--				 removed *12 on WarrantyBand.WarrantyLength when comparing to WarrantyReturnCodes.MonthsSinceDelivery
-- =============================================
CREATE PROCEDURE DN_GetWarrantyProductsByAccountSP 
(	
	@acctno varchar(12)	,
	@return int output
)
AS

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT 
		wr.ReturnCode
		, s.stocklocn
		, s.itemdescr1
		, s.itemdescr2
		--, s.itemno as stockitemno 
		,s.IUPC as stockitemno						--IP - 08/06/11 - CR1212 - RI
		, wl.contractno
		--, ws.itemno as waritemno
		,ws.IUPC as waritemno						--IP - 08/06/11 - CR1212 - RI
		, sd.agrmtno
		, wd.buffno
		, s.ID as ItemID							--IP - 08/06/11 - CR1212 - RI
		,ws.ID as WarrantyID						--IP - 08/06/11 - CR1212 - RI
	FROM
		Delivery	sd,			-- item delivery
		lineitem	sl,			-- lineitem for delivery of stock
		lineitem	wl,			-- lineitem for delivery of warranty
		stockitem	s,			-- stockitem table containing descriptions
		stockitem	ws,			-- stockitem table for warranty to check for category
		delivery	wd,			-- warranty delivery
		warrantyband wb,		-- warrantyband to get length of warranty
		WarrantyReturnCodes wr,	-- warranty return codes for autopopulation of grid
		Code			 c
	WHERE	sd.acctno = @acctno 
		AND sd.acctno	= sl.acctno and sd.stocklocn = sl.stocklocn 
		--AND sd.itemno	= sl.itemno and sd.agrmtno =sl.agrmtno and sd.contractno = sl.contractno -- stockdel to stock order
		AND sd.ItemID = sl.ItemID and sd.agrmtno =sl.agrmtno and sd.contractno = sl.contractno -- stockdel to stock order			--IP - 08/06/11 - CR1212 - RI
		--AND ws.itemno	= wl.itemno and ws.Stocklocn =wl.stocklocn  --warranty stockitem to lineitem stockitem
		AND ws.ID = wl.ItemID and ws.Stocklocn =wl.stocklocn  --warranty stockitem to lineitem stockitem							--IP - 08/06/11 - CR1212 - RI
		AND ws.category in (select distinct code from code where category = 'WAR') -- warranty categories
		--AND wd.acctno	= wl.acctno and wd.Itemno =wl.Itemno  
		AND wd.acctno	= wl.acctno and wd.ItemID =wl.ItemID																		--IP - 08/06/11 - CR1212 - RI
		AND wd.stocklocn = wl.stocklocn and wd.contractno = wl.contractno-- warranty delivery to warranty order
		--AND s.itemno	= sd.itemno and s.stocklocn =sd.stocklocn -- stock delivery to stockitem
		AND s.ID = sd.ItemID and s.stocklocn =sd.stocklocn -- stock delivery to stockitem											--IP - 08/06/11 - CR1212 - RI
		--AND wl.acctno	= sl.acctno and wl.Parentitemno = sl.Itemno and wl.parentlocation = sl.stocklocn -- warranty to lineitem
		AND wl.acctno	= sl.acctno and wl.ParentItemID = sl.ItemID and wl.parentlocation = sl.stocklocn -- warranty to lineitem	--IP - 08/06/11 - CR1212 - RI
		AND sd.quantity > 0 and sd.delorcoll ='D' -- delivery only 
		--AND wb.waritemno = wl.itemno and wr.ProductType = case	when s.refcode = 'ZZ'	then 'I' 
		AND wb.ItemID = wl.ItemID and wr.ProductType = case	when s.refcode = 'ZZ'	then 'I'										--IP - 08/06/11 - CR1212 - RI
																when c.category = 'PCF' then 'F' 
																when c.category = 'PCE' then 'E' else c.category end  
		AND cast(s.category as varchar(12)) = c.code  
		AND	sd.datedel > dateadd(year, -wb.warrantylength, getdate())	-- valid warranty hasn't expired yet.
		--AND wb.warrantylength*12 = wr.warrantymonths  AND wr.MonthSinceDelivery =  floor(datediff(d, sd.datedel, getdate()) / 30.4) + 1 -- return codes to be autopopulated
		AND wb.warrantylength = wr.warrantymonths  AND wr.MonthSinceDelivery =  floor(datediff(d, sd.datedel, getdate()) / 30.4) + 1 -- return codes to be autopopulated --IP - 08/06/11 - CR1212 - RI - Removed * 12 as previously warranylength held years, now holds months for RI
		AND NOT EXISTS 
		(	SELECT * 
			FROM delivery wc 
			WHERE wc.acctno = wd.acctno 
				--AND wc.itemno = wd.itemno 
				AND wc.ItemID = wd.ItemID					--IP - 08/06/11 - CR1212 - RI
				AND wc.stocklocn = wd.stocklocn 
				AND wc.contractno = wd.contractno
				AND wc.quantity < 1 AND wc.delorcoll IN ('C', 'R')
		) -- exclude warranties which have already been collected or repossessed
		

		UNION
		-- Query to pick up renewals
	SELECT 
		wr.ReturnCode
		, s.stocklocn
		, s.itemdescr1
		, s.itemdescr2
		--, s.itemno as stockitem 
		, s.IUPC as stockitem								--IP - 08/06/11 - CR1212 - RI
		, wp.contractno
		--, ws.itemno
		, ws.IUPC as itemno									--IP - 08/06/11 - CR1212 - RI
		, sd.agrmtno
		, wd.buffno
		,s.ID as ItemID										--IP - 08/06/11 - CR1212 - RI
		,ws.ID as WarrantyID								--IP - 08/06/11 - CR1212 - RI
	FROM
		Delivery	sd,			-- item delivery
		lineitem	sl,			-- lineitem for delivery of stock
		lineitem	wl,			-- lineitem for delivery of warranty
		stockitem	s,			-- stockitem table containing descriptions
		stockitem	ws,			-- stockitem table for warranty to check for category
		delivery	wd,			-- warranty delivery
		warrantyband wb,		-- warrantyband to get length of warranty
		WarrantyReturnCodes wr,	-- warranty return codes for autopopulation of grid
		Code			 c,
		warrantyrenewalpurchase wp
	WHERE	sd.acctno = @acctno 
		AND sd.acctno	= sl.acctno and sd.stocklocn = sl.stocklocn 
		--AND sd.itemno	= sl.itemno and sd.agrmtno =sl.agrmtno and sd.contractno = sl.contractno -- stockdel to stock order
		AND sd.ItemID	= sl.ItemID and sd.agrmtno =sl.agrmtno and sd.contractno = sl.contractno -- stockdel to stock order					--IP - 08/06/11 - CR1212 - RI
		--AND ws.itemno	= wl.itemno and ws.Stocklocn =wl.stocklocn  --warranty stockitem to lineitem stockitem
		AND ws.ID	= wl.ItemID and ws.Stocklocn =wl.stocklocn  --warranty stockitem to lineitem stockitem									--IP - 08/06/11 - CR1212 - RI
		AND ws.category in (select distinct code from code where category = 'WAR') -- warranty categories
		--AND wd.acctno	= wl.acctno and wd.Itemno =wl.Itemno  
		AND wd.acctno	= wl.acctno and wd.ItemID =wl.ItemID																				--IP - 08/06/11 - CR1212 - RI																					  
		AND wd.stocklocn = wl.stocklocn and wd.contractno = wl.contractno-- warranty delivery to warranty order
		--AND s.itemno	= sd.itemno and s.stocklocn =sd.stocklocn -- stock delivery to stockitem
		AND s.ID	= sd.ItemID and s.stocklocn =sd.stocklocn -- stock delivery to stockitem												--IP - 08/06/11 - CR1212 - RI
		--AND wl.acctno	= sl.acctno and wl.Parentitemno = sl.Itemno and wl.parentlocation = sl.stocklocn -- warranty to lineitem
		AND wl.acctno	= sl.acctno and wl.ParentItemID = sl.ItemID and wl.parentlocation = sl.stocklocn -- warranty to lineitem			--IP - 08/06/11 - CR1212 - RI
		AND sd.quantity > 0 and sd.delorcoll ='D' -- delivery only 
		--AND wb.waritemno = wl.itemno and wr.ProductType = case  when s.refcode = 'ZZ'	then 'I' 
		AND wb.ItemID = wl.ItemID and wr.ProductType = case  when s.refcode = 'ZZ'	then 'I'												--IP - 08/06/11 - CR1212 - RI
																when c.category = 'PCF' then 'F' 
																when c.category = 'PCE' then 'E' 
															else c.category end  
		AND cast(s.category as varchar(12)) = c.code  
		AND	wp.datedelivered > dateadd(year, -wb.warrantylength, getdate())	-- valid warranty hasn't expired yet.
		AND wp.datedelivered IS NOT NULL AND wd.contractno = wp.originalcontractno AND sd.acctno = wp.stockItemacctno
		--AND wb.warrantylength*12 = wr.warrantymonths   and wr.MonthSinceDelivery =  floor(datediff(d, sd.datedel, getdate()) / 30.4) + 1 -- return codes to be autopopulated
		AND wb.warrantylength = wr.warrantymonths  and wr.MonthSinceDelivery =  floor(datediff(d, sd.datedel, getdate()) / 30.4) + 1 -- return codes to be autopopulated --IP - 08/06/11 - CR1212 - RI - Removed * 12 as previously warranylength held years, now holds months for RI
		AND NOT EXISTS 
		(	SELECT * 
			FROM delivery wc 
			WHERE wc.acctno = wd.acctno 
				--AND wc.itemno = wd.itemno 
				AND wc.ItemID = wd.ItemID								--IP - 08/06/11 - CR1212 - RI
				AND wc.stocklocn = wd.stocklocn 
				AND wc.contractno = wd.contractno 
				AND wc.quantity < 1 AND wc.delorcoll IN ('C', 'R')
		) -- exclude warranties which have already been collected
		
		SET @Return = @@Error



		

GO

 
