SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_LineItemsGetForLocationChangeSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_LineItemsGetForLocationChangeSP]
GO

CREATE PROCEDURE 	dbo.DN_LineItemsGetForLocationChangeSP 
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_LineItemsGetForLocationChangeSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : LineItems Get For LocationChange 
-- Author       : ??
-- Date         : ??
--
-- This procedure will load items for change of location
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 29/05/12 jec #10230 Cosacs - create consumer for Booking Cancellation
-- 07/06/12 ip  #10229 - Warehouse & Deliveries - Return Express
-- 07/06/12 ip  #10229 - Only select the last booking for an item from LineItemBooking#
-- 11/06/12 jec #10342 Error in change order details when an address is selected which is not saved for the customer.
-- 13/05/13 ip  #13490 (#12842) Confirmed deliveries not being registered - Return Price
-- 15/05/13 ip  #13464 - Change Order Details Error (Barbados)
-- ================================================
	-- Add the parameters for the stored procedure here
			@acctno varchar(12),
			@loadBeforeDA bit, --IP - 28/04/09 - CR929 & 974 - Deliveries
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code
	

	IF(@loadBeforeDA = 1)
	BEGIN
		SELECT	l.acctno,
				s.IUPC AS ItemNo,
				l.stocklocn,
				ISNULL(l.delnotebranch, 0) as delnotebranch,
				l.deliveryaddress,
				isnull(sc.buffno,0) as buffno,
				l.datereqdel,
				ISNULL(l.timereqdel, '') as timereqdel,
				CASE WHEN l.deliveryprocess = 'I' THEN 1 ELSE 0 END as deliveryprocess,
				l.notes,
				ISNULL(l.damaged, 'N') as damaged,
				ISNULL(l.deliveryarea, '') as deliveryarea,
				l.qtydiff as 'qtydiff',
				s.itemtype,
				l.contractno,
				l.agrmtno,
				l.quantity,
				CASE WHEN l.assemblyrequired = 'Y' THEN 1 ELSE 0 END as assemblyrequired,
				l.ItemID,
				l.ParentItemID,
				l.id,				-- #10230 LineItemId
				l.deliveryprocess as origdeliveryprocess,		-- #10230
				ISNULL(l.Express, 'N') as Express,				-- IP - 07/06/12 - #10229
				l.Price											-- #13490 #13464
		FROM    lineitem l 
		INNER JOIN stockinfo s ON l.ItemID  = s.ID
	    LEFT join schedule sc ON   l.acctno    = sc.acctno
									AND    	l.agrmtno   = sc.agrmtno
									AND    	l.ItemID    = sc.ItemID
									AND    	l.stockLocn = sc.stocklocn		
		WHERE	l.acctNo    = @acctno
		AND    	l.agrmtNo   = 1
		AND		l.quantity  > 0
		AND	l.iskit     <> 1 
		AND NOT  EXISTS(	SELECT	1
							FROM	delivery d
							WHERE	d.acctno = l.acctno
							AND	d.agrmtno = l.agrmtno
							AND	d.stocklocn = l.stocklocn
							AND	d.ItemID = l.ItemID
							AND	d.contractno = l.contractno)
		
		
	END
	ELSE	
	BEGIN
		SELECT	l.acctno,
				s.IUPC  AS ItemNo,
				l.stocklocn,
				ISNULL(l.delnotebranch, 0) as delnotebranch,
				l.deliveryaddress,
				sc.buffno,
				l.datereqdel,
				ISNULL(l.timereqdel, '') as timereqdel,
				CASE WHEN l.deliveryprocess = 'I' THEN 1 ELSE 0 END as deliveryprocess,
				l.notes,
				ISNULL(l.damaged, 'N') as damaged,
				ISNULL(l.deliveryarea, '') as deliveryarea,
				l.qtydiff as 'qtydiff',
				s.itemtype,
				l.contractno,
				l.agrmtno,
				l.quantity,
				CASE WHEN l.assemblyrequired = 'Y' THEN 1 ELSE 0 END as assemblyrequired,
				l.ItemID,
				l.ParentItemID,
				l.id,				-- #10230 LineItemId
				l.deliveryprocess as origdeliveryprocess,		-- #10230
				ISNULL(l.Express, 'N') as Express,				-- IP - 07/06/12 - #10229
				l.Price											-- #13490 #13464
		FROM    lineitem l, schedule sc, stockitem s
		WHERE	l.acctNo    = @acctno
		AND    	l.agrmtNo   = 1
		AND		l.quantity  > 0
		AND		l.acctno    = sc.acctno
		AND    	l.agrmtno   = sc.agrmtno
		AND    	l.ItemID    = sc.ItemID
		AND    	l.stockLocn = sc.stocklocn
		AND    	l.ItemID    = s.ItemID
		AND    	l.stockLocn = s.stocklocn
		AND	l.iskit     <> 1 
		AND NOT  EXISTS(	SELECT	1
						FROM	delivery d
						WHERE	d.acctno = l.acctno
						AND	d.agrmtno = l.agrmtno
						AND	d.stocklocn = l.stocklocn
						AND	d.ItemID = l.ItemID
						AND	d.contractno = l.contractno)
						
		union		-- #10230 scheduled to Warehouse		
		SELECT	l.acctno,
				s.IUPC  AS ItemNo,
				l.stocklocn,
				ISNULL(l.delnotebranch, 0) as delnotebranch,
				l.deliveryaddress,
				lb.ID as buffno,
				--sc.buffno,
				l.datereqdel,
				ISNULL(l.timereqdel, '') as timereqdel,
				CASE WHEN l.deliveryprocess = 'I' THEN 1 ELSE 0 END as deliveryprocess,
				l.notes,
				ISNULL(l.damaged, 'N') as damaged,
				ISNULL(l.deliveryarea, '') as deliveryarea,
				l.qtydiff as 'qtydiff',
				s.itemtype,
				l.contractno,
				l.agrmtno,
				l.quantity,
				CASE WHEN l.assemblyrequired = 'Y' THEN 1 ELSE 0 END as assemblyrequired,
				l.ItemID,
				l.ParentItemID,
				l.id,				-- #10230 LineItemId
				l.deliveryprocess as origdeliveryprocess,		-- #10230
				ISNULL(l.Express, 'N') as Express,				-- IP - 07/06/12 - #10229
				l.Price											-- #13490 #13464
		FROM    lineitem l INNER JOIN lineItemBooking lb on l.Id = lb.LineItemId
							INNER JOIN stockitem s on 	l.ItemID    = s.ItemID		
		WHERE	l.acctNo    = @acctno
		AND    	l.agrmtNo   = 1
		AND		l.quantity  > 0
		--AND		l.acctno    = sc.acctno
		--AND    	l.agrmtno   = sc.agrmtno
		--AND    	l.ItemID    = sc.ItemID
		--AND    	l.stockLocn = sc.stocklocn
		--AND    	l.ItemID    = s.ItemID
		AND    	l.stockLocn = s.stocklocn
		AND	l.iskit     <> 1 
		AND		lb.ID = (select max(ID) from lineItemBooking lb2	--IP - 07/06/12 - #10229 
							where lb2.LineItemID = lb.LineItemID)
		AND NOT  EXISTS(	SELECT	1
						FROM	delivery d
						WHERE	d.acctno = l.acctno
						AND	d.agrmtno = l.agrmtno
						AND	d.stocklocn = l.stocklocn
						AND	d.ItemID = l.ItemID
						AND	d.contractno = l.contractno)
		
	END
	
	SELECT DISTINCT l.ItemID, s.stocklocn
	FROM    lineitem l 
			INNER JOIN stockitem s ON l.ItemID  = s.ID
	WHERE l.acctno = @acctno
	-- #10342 - get address types for customer
	select AddType
	From Custaddress cadd INNER JOIN custacct ca on ca.custid=cadd.custid
	where ca.hldorjnt='H'
		and cadd.datemoved is null
		and ca.acctno=@acctno

	SET	@return = @@error

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End
