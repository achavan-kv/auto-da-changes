if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_LineItemGetDetailsSPForTaxInvoice]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DN_LineItemGetDetailsSPForTaxInvoice]
GO

GO
/****** Object:  StoredProcedure [dbo].[DN_LineItemGetDetailsSPForTaxInvoice]    Script Date: 20-04-2019 19:34:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[DN_LineItemGetDetailsSPForTaxInvoice]
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
			@lineItemId int out,
			@invoiceVersion int,
           -- @DeliveryorCollection varchar(12),				--#13716 - CR12949 
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code
	SET	@delQty = 0
	SET	@scheduled = 0;


	  update InvoiceDetails set RetItemNo=itemno where quantity<0 and RetItemNo=''  and acctno=@acctNo and agrmtno=@agreementNo and invoiceversion=@invoiceversion
	  
	IF EXISTS(Select acctno from InvoiceDetails where acctno=@acctNo and agrmtno=@agreementNo and invoiceversion=@invoiceversion and ItemID = @itemId and returnquantity IS NOT NULL )
	BEGIN
	
	IF EXISTS((Select acctno	from Invoicedetails where ItemID = @itemId and acctno=@acctNo and agrmtno=@agreementNo and invoiceversion=@invoiceversion and RetItemNo ='' OR RetItemNo = NULL ))
	BEGIN 
	SET @itemno = (select top 1 itemno from Invoicedetails where ItemID = @itemId and acctno=@acctNo and agrmtno=@agreementNo and invoiceversion=@invoiceversion and quantity>0)
	END	
	ELSE
	SET @itemno = (select top 1 itemno from Invoicedetails where ItemID = @itemId and acctno=@acctNo and agrmtno=@agreementNo and invoiceversion=@invoiceversion)
	
	END
	ELSE
	BEGIN
	SET @itemno =(select top 1  itemno from Invoicedetails  where acctno=@acctNo and agrmtno=@agreementNo and invoiceversion=@invoiceversion and ItemID = @itemId  and stocklocn=@stockLocn and quantity>0 )
	END
	--SET @itemno = (Select 
	--CASE
	-- WHEN Retitemno IS  NULL or Retitemno= ' '  then itemno
	-- WHEN Retitemno IS NOT NULL then Retitemno 
	-- END as Retitemno
	-- From Invoicedetails where  acctno=@acctNo and agrmtno=@agreementNo and invoiceversion=@invoiceversion)
	--END
	--ELSE
	--BEGIN
	--SET @itemno = (select top 1 IUPC from StockInfo where ID = @itemId)
	--END
	
	SET @parentItemNo = (select top 1 IUPC from StockInfo where ID = @parentItemId)
	SET @repoItem = (select top 1 RepossessedItem from StockInfo where ID = @itemId)		-- jec 16/06/11 

	----IP - 28/05/09 - (69962) 
	--IF(@contractNo != '')
	--BEGIN
	--	SELECT	@delQty	=	SUM(quantity)
	--	FROM	delivery
	--	WHERE	acctno 		=	@acctno
	--	AND		ItemID		=	@itemId
	--	AND		stocklocn	=	@stockLocn
	--	AND		agrmtno		=	@agreementNo
	--	AND		contractno	=	@contractNo
	--END
	--ELSE
	--BEGIN
	--	SELECT	@delQty	=	SUM(quantity)
	--	FROM	delivery
	--	WHERE	acctno 		=	@acctno
	--	AND		ItemID		=	@itemId
	--	AND		stocklocn	=	@stockLocn
	--	AND		agrmtno		=	@agreementNo
	--	AND		contractno	=	@contractNo
	--	--AND		parentitemNo = @parentITemNo
	--	AND     ParentItemID = @parentItemId			--IP - 16/05/11 - CR1212 - #3627
	--END		
	

	SELECT	@scheduled	=	SUM(quantity)
	FROM	schedule
	WHERE	acctno 		=	@acctno
	AND		ItemID		=	@itemId
	AND		stocklocn	=	@stockLocn
	AND		agrmtno		=	@agreementNo
	
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



	IF(@invoiceversion != 0)
	BEGIN
	--If Record is for GRT then display  returnquantity else display quantity	
	IF EXISTS(Select * from InvoiceDetails where acctno=@acctNo and agrmtno=@agreementNo and invoiceversion=@invoiceversion and returnquantity IS NOT NULL)
	BEGIN	
			
		SELECT	@quantity = returnquantity, @price = i.retval, @orderValue = i.RetVal  from invoicedetails i
		WHERE	i.acctno = @acctNo
		AND		i.agrmtno = @agreementNo
		AND i.invoiceversion = @invoiceversion
		AND i.returnquantity IS NOT NULL
		AND i.ItemId=@ItemId
				--AND		(i.ParentItemID = 0 OR					--IP - 16/05/11 - CR1212 - #3627
				-- EXISTS(SELECT	* 
				--		FROM	warrantyrenewalpurchase
				--		WHERE	acctno = @acctNo))

				SELECT  	
			@acctNo 		=	i.acctno,
			@agreementNo	=	i.agrmtno,
			@itemID			=	i.ItemID,			
			@quantity 		=	@quantity,
			@delQty			=	@delQty,
			@stockLocn 		=	i.stocklocn,
			@price 			=	@price,
			@orderValue 	=	(@orderValue),			
			@taxAmount 		=	(i.taxamt)
			--@itemno         =   i.RetItemNo
			
	From  InvoiceDetails i Inner join 
	 [Merchandising].[Product] p ON  i.RetItemNo=p.SKU
	WHERE	i.acctno = @acctNo	
	AND     i.invoiceversion=@invoiceVersion
												
		
	END
	ELSE				
	BEGIN
	
	SELECT	 @quantity = quantity, @price = i.price, @orderValue = i.OrdVal from invoicedetails i
		WHERE	i.acctno = @acctNo
		AND		i.agrmtno = @agreementNo
		AND i.invoiceversion = @invoiceversion
		AND i.returnquantity IS NULL
		AND i.ItemId=@ItemId
		AND i.stocklocn= @stockLocn 	
		
				

					SELECT  	
			@acctNo 		=	i.acctno,
			@agreementNo	=	i.agrmtno,
			@itemID			=	i.ItemID,			
			@quantity 		=	@quantity,
			@delQty			=	@delQty,
			@stockLocn 		=	i.stocklocn,
			@price 			=	@price,
			@orderValue 	=	(@orderValue),			
			@taxAmount 		=	(i.taxamt)
			
	From  InvoiceDetails i Inner join 
	 [Merchandising].[Product] p ON  i.itemno=p.SKU
	WHERE	i.acctno = @acctNo	
	AND     i.invoiceversion=@invoiceVersion
	
	
					
						
	END
	END

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END



	

 


	

 







	

 


	

 




