SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_LineItemGetDetailsSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_LineItemGetDetailsSP]
GO

CREATE PROCEDURE 	dbo.DN_LineItemGetDetailsSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_LineItemGetDetailsSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Get Lineitem details  
-- Author       : ??
-- Date         : ??
--
-- This procedure will retrieve the Lineitem Details
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 02/03/10 jec CR1072 Malaysia v4 merge
-- 24/10/10 ip  UAT(207) UAT5.2.1.0 Log - Unable to remove items from Revise Sales Order that have been rejectected by DHL
-- 04/06/10 ip  UAT(262) UAT5.2.1.0 Log - Flag an item as rejected if rejected by DHL and not scheduled for delivery.
-- 09/03/11 ip  Removed hasstring as this column is dropped in migration(20110308141100 - lineitemupdate.sql)
-- 16/05/11 ip  RI Integration changes - CR1212 - #3627 - Changed to use ParentItemID rather than ParentItemNo
-- 23/05/11 IP  RI Integration changes - CR1212 - #3651 - Return SalesBrnNo (Branch sale was processed)
-- 16/06/11 jec CR1212 - #3923 - Return RepossessedItem flag
-- 06/06/12 jec #10229 - Add Express delivery
-- 09/07/13 IP  #13716 - CR12949 - Return LineItem.ID
-- 16/08/13 jec #14603 - Lock items sent to Warehouse
-- 18/09/13 jec #14972 - Sum LineitemBookingSchedule qtys
-- ================================================
	-- Add the parameters for the stored procedure here
			@origbr smallint OUT,
			@acctNo varchar(12),
			@agreementNo int,
			@itemID int,		-- RI 
			@itemSuppText varchar(76) OUT,
			@quantity float OUT,
			@delQty float OUT,
			@stockLocn smallint,
			@price money OUT,
			@orderValue money OUT,
			@dateReqDel datetime OUT,
			@timeReqDel varchar(12) OUT,
			@datePlanDel datetime OUT,
			@delNoteBranch smallint OUT,
			@qtyDiff char(1) OUT,
			@itemType varchar(1) OUT,
			@hasString smallint OUT,
			@notes varchar(200) OUT,
			@taxAmount float OUT,
			@parentItemId int,
			@parentStockLocn smallint OUT,
			@isKit smallint OUT,
			@lastDelivery datetime OUT,
			@deliveryAddress char(2) OUT,
			@scheduled float OUT,
			@contractNo varchar(10),
			@expectedreturndate datetime OUT,
			@deliveryarea varchar(8) OUT,
			@deliveryprocess char(1) OUT,
			@damaged char(1) OUT,
			@assembly char(1) OUT,
			@spiff smallint OUT,
			@vanNo VARCHAR(8) OUT,
			@DhlInterfaceDate DATETIME out, 
			@DhlPickingDate DATETIME out, 
			@DHLDNNo VARCHAR(10) out,			
			--@OrigQty INT out,
			@ShipQty INT out,
			@ItemRejected BIT OUT,			--IP - 03/06/10 - UAT(262) UAT5.2.1.0 Log
			@isComponent BIT OUT,			--AA - 07/07/10 - UAT(267) UAT5.2.1.0 Log
			@itemno VARCHAR(18) OUT,
			@parentItemNo VARCHAR(18) OUT,
			@salesBrnNo smallint OUT,		--IP - 23/05/11 - CR1212 - RI - #3651
			@repoItem BIT out,					-- jec 16/06/11 
			@express CHAR(1) out,				-- #10229
			@lineItemId int out,				--#13716 - CR12949 
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code
	SET	@delQty = 0
	SET	@scheduled = 0;
	
	SET @itemno = (select top 1 IUPC from StockInfo where ID = @itemId)
	SET @parentItemNo = (select top 1 IUPC from StockInfo where ID = @parentItemId)
	SET @repoItem = (select top 1 RepossessedItem from StockInfo where ID = @itemId)		-- jec 16/06/11 

	--IP - 28/05/09 - (69962) 
	IF(@contractNo != '')
	BEGIN
		SELECT	@delQty	=	SUM(quantity)
		FROM	delivery
		WHERE	acctno 		=	@acctno
		AND		ItemID		=	@itemId
		AND		stocklocn	=	@stockLocn
		AND		agrmtno		=	@agreementNo
		AND		contractno	=	@contractNo
	END
	ELSE
	BEGIN
		SELECT	@delQty	=	SUM(quantity)
		FROM	delivery
		WHERE	acctno 		=	@acctno
		AND		ItemID		=	@itemId
		AND		stocklocn	=	@stockLocn
		AND		agrmtno		=	@agreementNo
		AND		contractno	=	@contractNo
		--AND		parentitemNo = @parentITemNo
		AND     ParentItemID = @parentItemId			--IP - 16/05/11 - CR1212 - #3627
	END		
	

	--SELECT	@scheduled	=	SUM(quantity)
	--FROM	schedule
	--WHERE	acctno 		=	@acctno
	--AND		ItemID		=	@itemId
	--AND		stocklocn	=	@stockLocn
	--AND		agrmtno		=	@agreementNo
	
	-- #14603 - get schedule quantity from lineitemschedule
	SELECT	@scheduled	=  sum(lb.quantity)					-- #14972 		 
	from LineItemBookingSchedule lb inner join lineitem l on lb.LineItemID=l.ID
	where l.acctno=@acctNo and l.itemid=@itemId and l.stocklocn=@stockLocn and l.agrmtno=@agreementNo
	--order by bookingid									-- #14972 	

	
	-- cr Malaysia DHL rdb 08/08/08 -- JC/IP 4.3 merge 02/03/10
	SELECT	@vanno = VanNo, 
			@DhlInterfaceDate = datefinish 
	FROM schedule
	INNER JOIN interfaceControl ON schedule.runno = interfacecontrol.runno AND
			interfacecontrol.interface = 'LOGEXPORT'
	WHERE	acctno 		=	@acctno
	AND		ItemID		=	@itemID
	AND		stocklocn	=	@stockLocn
	AND		agrmtno		=	@agreementNo

	-- cr Malaysia DHL rdb 08/08/08 
	IF @vanno IS NULL
	SELECT	@vanno = VanNo, 
			@DhlInterfaceDate = datefinish 
	FROM scheduleaudit
	INNER JOIN interfaceControl ON scheduleaudit.runno = interfacecontrol.runno AND
				interfacecontrol.interface = 'LOGEXPORT'
	WHERE	acctno 		=	@acctno
	AND		itemId		=	@itemID
	AND		stocklocn	=	@stockLocn
	AND		agrmtno		=	@agreementNo
	AND NOT EXISTS(SELECT * FROM ScheduleRemoval sr				--IP - 24/05/10 - UAT(207) UAT5.2.1.0 Log
					WHERE sr.ItemId = ScheduleAudit.itemId
					AND sr.StockLocn = ScheduleAudit.stocklocn
					AND sr.AgrmtNo = ScheduleAudit.agrmtno
					AND sr.BuffNo = ScheduleAudit.buffno)
	
	-- Malaysia 3PL jec 05/03/10
	SELECT @DhlPickingDate = DhlPickingDate, 
		   @DHLDNNo = DHLDNNo			
	FROM schedule
	INNER JOIN interfaceControl ON schedule.runnoImport = interfacecontrol.runno AND
				interfacecontrol.interface = 'LOGIMPORT'
	WHERE	acctno 		=	@acctno
	AND		ItemID		=	@itemID
	AND		stocklocn	=	@stockLocn
	AND		agrmtno		=	@agreementNo

	-- Malaysia 3PL jec 05/03/10
	IF @DHLDNNo IS NULL
	SELECT	@DhlPickingDate = DhlPickingDate, @DHLDNNo = DHLDNNo		
	FROM scheduleaudit
	INNER JOIN interfaceControl ON scheduleaudit.runnoImport = interfacecontrol.runno AND
				interfacecontrol.interface = 'LOGIMPORT'
	WHERE	acctno 		=	@acctno
	AND		itemId		=	@itemID
	AND		stocklocn	=	@stockLocn
	AND		agrmtno		=	@agreementNo
	AND NOT EXISTS(SELECT * FROM ScheduleRemoval sr				--IP - 24/05/10 - UAT(207) UAT5.2.1.0 Log
					WHERE sr.ItemId = ScheduleAudit.itemId
					AND sr.StockLocn = ScheduleAudit.stocklocn
					AND sr.AgrmtNo = ScheduleAudit.agrmtno
					AND sr.BuffNo = ScheduleAudit.buffno)
					
	-- get quantity in transit
	SELECT @ShipQty = ISNULL(SUM(ISNULL(quantity,0)),0)  			
	FROM schedule		
	WHERE	acctno 		=	@acctno
	AND		ItemID		=	@itemID
	AND		stocklocn	=	@stockLocn
	AND		agrmtno		=	@agreementNo
	and		RunNoImport is not null			

	--IP - 04/06/10 - UAT(262) UAT5.2.1.0 Log
	IF EXISTS(SELECT * FROM ScheduleRemoval r
			  INNER JOIN lineitem l ON r.AcctNo = l.acctno
				AND r.AgrmtNo = l.agrmtno
				AND r.ItemId = l.ItemID
				AND r.StockLocn = l.stocklocn
				AND r.AcctNo = @acctno
				AND r.AgrmtNo = @agreementNo
				AND r.itemId = @itemID
				AND r.StockLocn = @stockLocn
				AND r.DateRemoved = (SELECT MAX(r1.dateremoved) FROM ScheduleRemoval r1
										WHERE r1.AcctNo = r.AcctNo
										AND r1.AgrmtNo = r.AgrmtNo
										AND r1.itemId = r.ItemId
										AND r1.StockLocn = r.StockLocn)
				AND l.itemtype = 'S'
				AND l.quantity > 0
				AND l.ordval > 0
			 INNER JOIN ScheduleAudit sa ON sa.acctno = r.AcctNo
				AND sa.agrmtno = r.AgrmtNo
				AND sa.itemId = r.ItemId
				AND sa.stocklocn = r.StockLocn
				AND sa.buffno = r.BuffNo
				AND sa.RunnoImport IS NOT NULL
				AND sa.vanno = 'DHL'
				AND sa.DHLDNNo IS NOT NULL
				WHERE NOT EXISTS(SELECT SUM(d.quantity) FROM delivery d
								 WHERE d.acctno = l.acctno
								AND d.agrmtno = l.agrmtno
								AND d.ItemID = l.ItemID
								AND d.stocklocn = l.stocklocn
								AND d.delorcoll IN('C', 'D')
								HAVING SUM(d.quantity) >= l.quantity)
				AND NOT EXISTS(SELECT * FROM schedule s
								WHERE s.acctno = l.acctno
								AND s.agrmtno = l.agrmtno
								AND s.ItemID = l.ItemID
								AND s.stocklocn = l.stocklocn))
				BEGIN
						SET @ItemRejected = 1
				END
				ELSE
				BEGIN
						SET @ItemRejected = 0
				END
				
	IF EXISTS (SELECT * FROM kitproduct 
			  WHERE ItemID = @parentItemId AND ComponentID = @itemID )
	    SET @isComponent = 1
	ELSE 
		SET @isComponent = 0 
	
	
	--IP - 29/07/08 - UAT5.1 - UAT(494) - After processing an Identical Replacement
	--and adding a new warranty onto the item if the warranty is no longer valid, the warranty once delivered was being delivered 
	--with the incorrect 'datedel' in delivery, as the original date of when the item was delivered was being used
	--to set the 'datedel' of the warranty. The 'datedel' will now be set from the most recent delivery date of the 
    --item in this scenario.
	DECLARE @newwarrantychosen SMALLINT,@warrantyItemId int
	SELECT @warrantyItemId = isnull(ItemID ,0)
	FROM lineitemaudit 
	WHERE acctno = @acctno AND 
		  ParentItemID = @itemID AND 
		  parentlocation = @stockLocn
	
	if @warrantyItemId != 0 
	BEGIN -- checking if new warranty purchased - if so will go off latest delivery date of stockitem
	  if exists (select * from lineitem where acctno = @acctno and itemId = @warrantyItemId
	            and parentlocation = 0 and ParentItemID = 0 and quantity = 1)
  		SET @newwarrantychosen = 1  -- if new warranty purchased then old warranty becomes orphaned go off max date
    END

    IF @newwarrantychosen =1 
    BEGIN
		SELECT	@lastDelivery 	=	max(datedel)
		FROM	delivery
		WHERE	acctno 		=	@acctno
		AND		ItemID		=	@itemID
		AND		stocklocn	=	@stockLocn
		AND		agrmtno		=	@agreementNo
		AND		contractno	=	@contractNo
		AND		delorcoll 	=	N'D'
	END
	ELSE
	BEGIN -- get earliest delivery date
		SELECT	@lastDelivery 	=	min(datedel)
		FROM	delivery
		WHERE	acctno 		=	@acctno
		AND		ItemID		=	@itemID
		AND		stocklocn	=	@stockLocn
		AND		agrmtno		=	@agreementNo
		AND		contractno	=	@contractNo
		AND		delorcoll 	=	N'D'
    END	
	
	SELECT	@spiff = COUNT(*)
	FROM	SalesCommissionExtraSpiffs
	WHERE	acctno 		=	@acctno
	AND		itemId		=	@itemID
	AND		stocklocn	=	@stockLocn
	AND		agrmtno		=	@agreementNo

	SELECT	@origbr			=	origbr,
			@acctNo 		=	acctno,
			@agreementNo	=	agrmtno,
			@itemID			=	ItemID,
			@itemSuppText 	=	itemsupptext,
			@quantity 		=	quantity,
			@delQty			=	@delQty,
			@stockLocn 		=	stocklocn,
			@price 			=	price,
			@orderValue 	=	ordval,
			@dateReqDel 	=	datereqdel,
			@timeReqDel 	=	timereqdel,
			@datePlanDel 	=	dateplandel,
			@delNoteBranch 	=	delnotebranch,
			@qtyDiff 		=	qtydiff,
			@itemType 		=	itemtype,
			--@hasString 		=	hasstring, --IP - 09/03/11 - Removed hasstring
			@notes 			=	notes,
			@taxAmount 		=	taxamt,
			@parentItemId 	=	ParentItemID,
			@parentStockLocn =	parentLocation,
			@isKit			=	isKit,
			@lastDelivery	=	@lastDelivery,
			@deliveryAddress =	deliveryAddress,
			@scheduled 		=	@scheduled,
			@expectedreturndate	=	expectedreturndate,
			@deliveryarea	=	deliveryarea,
			@deliveryprocess =	deliveryprocess,
			@damaged		= damaged,
			@assembly		= isnull(assemblyrequired,'N'),
			@salesBrnNo     = SalesBrnNo,					--IP - 23/05/11 - CR1212 - RI - #3651
			@express        = Express,							-- #10229
			@lineItemId		= ID							--#13716 - CR12949
	FROM	lineitem
	WHERE	acctno = @acctNo
	AND		ItemID = @itemID
	AND		stocklocn = @stockLocn
	AND		agrmtno	= @agreementNo
	AND		contractno = @contractNo
	AND (@parentItemId IS NULL OR ParentItemID = @parentItemId)
	

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End 

