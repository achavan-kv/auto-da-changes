SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


IF EXISTS (	SELECT * 
			FROM	SYSOBJECTS 
			WHERE	NAME = 'GetStockMovementForNegativeStockReport'
					AND xtype = 'P')
BEGIN 
	DROP PROCEDURE [Merchandising].GetStockMovementForNegativeStockReport
END
GO


CREATE PROCEDURE [Merchandising].[GetStockMovementForNegativeStockReport]
-- =======================================================================================
-- Procedure Name	: [Merchandising].[GetStockMovementForNegativeStockReport]
-- Author			: Ritesh
-- Date Creation	: 3 Jan 2020
-- Description		: This Procedure is used to get the stock movement details till end date
--                    for provided product Id and location Id used for negative report.
-- =======================================================================================
	@EndDate DATE,
	@ProductIds dbo.IntTVP READONLY,
	@ProductLocationIds dbo.IntTVP READONLY
AS
BEGIN

	SET NOCOUNT ON;

	CREATE TABLE #NegativeStockTransfer 
	( 
		[TransactionId] VARCHAR(100) NULL,
		ProductId INT NULL,
		SKU VARCHAR(20) NULL,
		LongDescription VARCHAR(240) NULL,
		ProductTags VARCHAR(MAX) NULL,
		BrandName VARCHAR(25) NULL,
		StoreTypes VARCHAR(MAX) NULL,
		Division VARCHAR(100) NULL,
		Department VARCHAR(100) NULL,
		Class VARCHAR(100) NULL,
		[LocationId] INT NULL,
		[Type] VARCHAR(50) NULL,
		Narration VARCHAR(500) NULL,
		[Quantity] INT NULL,
		[Date] DATE NULL,
		[DateUTC] DATETIME NULL,
		[DateProcessedUTC] DATETIME NULL,
		[UserId] INT NULL,
		[IsDirect] BIT NULL, 
		SendingLocationId INT NULL,
		ReceivingLocationId INT NULL
	)

--------- DIRECT TRANSFER SENDING LOCATION---------------

	INSERT INTO #NegativeStockTransfer 
	(	
		[TransactionId],
		ProductId,
		[LocationId],
		[Type],
		Narration,
		[Quantity],
		[Date],
		[DateUTC],
		[DateProcessedUTC],
		[UserId],
		[IsDirect], 
		SendingLocationId,
		ReceivingLocationId
	)
	SELECT
		CONVERT(VARCHAR(MAX), t.Id) [TransactionId],
		tp.ProductId,
		t.SendingLocationId [LocationId],
		'Transfer' [Type],
		 'Stock Transfer (Direct) #' + CONVERT(VARCHAR(MAX), t.Id) + ' - From {0} to {1} (' + ISNULL(t.ReferenceNumber, 'No reference') + ')' Narration,
		ISNULL(0 - tp.Quantity,0) [Quantity],
		null [Date],
		t.CreatedDate [DateUTC],
		t.CreatedDate [DateProcessedUTC],
		t.CreatedById [UserId],
		NULL [IsDirect], 
		t.SendingLocationId,
		t.ReceivingLocationId		
	FROM
		Merchandising.StockTransfer t WITH(NOLOCK)
		JOIN Merchandising.StockTransferProduct tp WITH(NOLOCK)
			ON t.id = tp.StockTransferId 
			AND t.ViaLocationId IS NULL
		AND EXISTS(SELECT 1 FROM @ProductIds WHERE Id = tp.ProductId)
		AND EXISTS (SELECT 1 FROM @ProductLocationIds WHERE Id = t.SendingLocationId)
	WHERE t.CreatedDate <= @EndDate OPTION (MAXDOP 4)

--------- DIRECT TRANSFER RECEIVING LOCATION---------------


	INSERT INTO #NegativeStockTransfer 
	( 
		[TransactionId],
		ProductId,
		[LocationId],
		[Type],
		Narration,
		[Quantity],
		[Date],
		[DateUTC],
		[DateProcessedUTC],
		[UserId],
		[IsDirect], 
		SendingLocationId,
		ReceivingLocationId
	)
	SELECT
		CONVERT(VARCHAR(MAX), t.Id) [TransactionId],
		tp.ProductId,
		t.ReceivingLocationId [LocationId],
		'Transfer' [Type],
		'Stock Transfer (Direct) #' + CONVERT(VARCHAR(MAX), t.Id) + ' - From {0} to {1} (' + ISNULL(t.ReferenceNumber, 'No reference') + ')' Narration,
		tp.Quantity,
		NULL [Date],
		t.CreatedDate [DateUTC],
		t.CreatedDate [DateProcessedUTC],
		t.CreatedById [UserId],
		NULL [IsDirect], 
		t.SendingLocationId,
		t.ReceivingLocationId
	FROM
		Merchandising.StockTransfer t WITH(NOLOCK)
		JOIN Merchandising.StockTransferProduct tp WITH(NOLOCK) ON t.id = tp.StockTransferId AND t.ViaLocationId IS NULL
		AND EXISTS(SELECT 1 FROM @ProductIds WHERE id = tp.ProductId)
		AND EXISTS(SELECT 1 FROM @ProductLocationIds WHERE id = t.ReceivingLocationId)
	WHERE t.CreatedDate <= @EndDate OPTION (MAXDOP 4)


--------- VIA FIRST TRANSFER SENDING LOCATION---------------


	INSERT INTO #NegativeStockTransfer ( [TransactionId],
		ProductId,
		[LocationId],
		[Type],
		 Narration,
		 [Quantity],
		 [Date],
		[DateUTC],
		 [DateProcessedUTC],
		 [UserId],
		[IsDirect], 
		SendingLocationId,
		ReceivingLocationId)
	SELECT
		CONVERT(VARCHAR(MAX), t.Id) [TransactionId],
		tp.ProductId,
		t.SendingLocationId [LocationId],
		'Transfer' [Type],
		 'Stock Transfer (Via) #' + CONVERT(VARCHAR(MAX), t.Id) + ' - From {0} to {1} (' + ISNULL(t.ReferenceNumber, 'No reference') + ')' Narration,
		ISNULL(0 - tp.Quantity,0) [Quantity],
		null [Date],
		t.CreatedDate [DateUTC],
		t.CreatedDate [DateProcessedUTC],
		t.CreatedById [UserId],
		null [IsDirect], 
		t.SendingLocationId,
		t.ReceivingLocationId
	FROM
		Merchandising.StockTransfer t WITH(NOLOCK)
		JOIN Merchandising.StockTransferProduct tp WITH(NOLOCK) ON t.id = tp.StockTransferId AND t.ViaLocationId IS NOT NULL
		AND EXISTS (SELECT 1 FROM @ProductIds WHERE id = tp.ProductId)
		AND EXISTS (SELECT 1 FROM @ProductLocationIds WHERE id = t.SendingLocationId)
	WHERE t.CreatedDate <= @EndDate OPTION (MAXDOP 4)

--------- VIA FIRST TRANSFER RECEIVING LOCATION---------------


	INSERT INTO #NegativeStockTransfer ( [TransactionId],
		ProductId,
		[LocationId],
		[Type],
		 Narration,
		 [Quantity],
		 [Date],
		[DateUTC],
		 [DateProcessedUTC],
		 [UserId],
		[IsDirect], 
		SendingLocationId,
		ReceivingLocationId)
	SELECT
		CONVERT(VARCHAR(MAX), t.Id) [TransactionId],
		tp.ProductId,
		t.ViaLocationId [LocationId],
		'Transfer' [Type],
		'Stock Transfer (Via) #' + CONVERT(VARCHAR(MAX), t.Id) + ' - From {0} to {1} (' + ISNULL(t.ReferenceNumber, 'No reference') + ')' Narration,
		tp.Quantity,
		NULL [Date],
		t.CreatedDate [DateUTC],
		t.CreatedDate [DateProcessedUTC],
		t.CreatedById [UserId],
		NULL [IsDirect], 
		t.SendingLocationId,
		t.ReceivingLocationId
	FROM
		Merchandising.StockTransfer t WITH(NOLOCK)
		JOIN Merchandising.StockTransferProduct tp WITH(NOLOCK) ON t.id = tp.StockTransferId AND t.ViaLocationId IS NOT NULL
		AND EXISTS (SELECT 1 FROM @ProductIds WHERE id = tp.ProductId)
		AND EXISTS (SELECT 1 FROM @ProductLocationIds WHERE id = t.ViaLocationId)
	WHERE t.CreatedDate <= @EndDate OPTION (MAXDOP 4)


--------- VIA SECOND TRANSFER SENDING LOCATION---------------

	INSERT INTO #NegativeStockTransfer ( [TransactionId],
		ProductId,
		[LocationId],
		[Type],
		 Narration,
		 [Quantity],
		 [Date],
		[DateUTC],
		 [DateProcessedUTC],
		 [UserId],
		[IsDirect], 
		SendingLocationId,
		ReceivingLocationId)
	SELECT
		CONVERT(VARCHAR(MAX), tr.Id) [TransactionId],
		tp.ProductId,
		t.SendingLocationId [LocationId],
		'Transfer' [Type],
		'Stock Transfer (Via) #' + CONVERT(VARCHAR(MAX), tr.Id) + ' - From {0} to {1} (' + ISNULL(tr.ReferenceNumber, 'No reference') + ')' Narration,
		isnull(0 - t.Quantity,0) [Quantity],
		null [Date],
		tr.CreatedDate [DateUTC],
		t.DateProcessed [DateProcessedUTC],
		tr.CreatedById [UserId],
		null [IsDirect], 
		t.SendingLocationId,
		t.ReceivingLocationId
	FROM
		Merchandising.StockTransferMovement t WITH(NOLOCK)
		JOIN Merchandising.StockTransferProduct tp WITH(NOLOCK) ON t.Bookingid = tp.BookingId AND t.[Type] = 'ViaTransfer'
		JOIN Merchandising.StockTransfer tr ON tr.Id = tp.StockTransferId	
		AND EXISTS (SELECT 1 FROM @ProductIds WHERE id = tp.ProductId)
		AND EXISTS (SELECT 1 FROM @ProductLocationIds WHERE id = t.SendingLocationId)	
	WHERE t.DateProcessed <= @EndDate OPTION (MAXDOP 4)

--------- VIA SECOND TRANSFER RECEIVING LOCATION---------------

	INSERT INTO #NegativeStockTransfer ( [TransactionId],
		ProductId,
		[LocationId],
		[Type],
		 Narration,
		 [Quantity],
		 [Date],
		[DateUTC],
		 [DateProcessedUTC],
		 [UserId],
		[IsDirect], 
		SendingLocationId,
		ReceivingLocationId)
	SELECT
		CONVERT(VARCHAR(MAX), tr.Id) [TransactionId],
		tp.ProductId,
		t.ReceivingLocationId [LocationId],
		'Transfer' [Type],
		'Stock Transfer (Via) #' + CONVERT(VARCHAR(MAX), tr.Id) + ' - From {0} to {1} (' + ISNULL(tr.ReferenceNumber, 'No reference') + ')' Narration,
		isnull(t.Quantity,0) [Quantity],
		null [Date],
		tr.CreatedDate [DateUTC],
		t.DateProcessed [DateProcessedUTC],
		tr.CreatedById [UserId],
		null [IsDirect], 
		t.SendingLocationId,
		t.ReceivingLocationId
	FROM
		Merchandising.StockTransferMovement t WITH(NOLOCK)
		JOIN Merchandising.StockTransferProduct tp WITH(NOLOCK) ON t.Bookingid = tp.BookingId AND t.[Type] = 'ViaTransfer'
		JOIN Merchandising.StockTransfer tr ON tr.Id = tp.StockTransferId		
		AND EXISTS (SELECT 1 FROM @ProductIds WHERE id = tp.ProductId)
		AND EXISTS (SELECT 1 FROM @ProductLocationIds WHERE id = t.ReceivingLocationId)
	WHERE t.DateProcessed <= @EndDate OPTION (MAXDOP 4)

--------- REQUISITION SENDING LOCATION---------------

	INSERT INTO #NegativeStockTransfer ( [TransactionId],
		ProductId,
		[LocationId],
		[Type],
		 Narration,
		 [Quantity],
		 [Date],
		[DateUTC],
		 [DateProcessedUTC],
		 [UserId],
		[IsDirect], 
		SendingLocationId,
		ReceivingLocationId)
	SELECT
		CONVERT(VARCHAR(MAX), t.BookingId) [TransactionId],
		tp.ProductId,
		t.SendingLocationId [LocationId],
		'Requisition' [Type],
		'Stock Requisition - Booking #' + CONVERT(VARCHAR(MAX), t.BookingId) + ' - From {0} to {1}' Narration,
		isnull(0 - t.Quantity,0) [Quantity],
		null [Date],
		tp.CreatedDate [DateUTC],
		t.DateProcessed [DateProcessedUTC],
		tp.CreatedById [UserId],
		null [IsDirect], 
		t.SendingLocationId,
		t.ReceivingLocationId
	FROM
		Merchandising.StockTransferMovement t WITH(NOLOCK)
		JOIN Merchandising.StockRequisitionProduct tp WITH(NOLOCK) ON t.Bookingid = tp.BookingId and t.[Type] = 'Requisition'	
		AND EXISTS (SELECT 1 FROM @ProductIds WHERE id = tp.ProductId)
		AND EXISTS (SELECT 1 FROM @ProductLocationIds WHERE id = t.SendingLocationId)
	WHERE t.DateProcessed <= @EndDate OPTION (MAXDOP 4)

--------- REQUISITION RECEIVING LOCATION---------------

	INSERT INTO #NegativeStockTransfer ( [TransactionId],
		ProductId,
		[LocationId],
		[Type],
		 Narration,
		 [Quantity],
		 [Date],
		[DateUTC],
		 [DateProcessedUTC],
		 [UserId],
		[IsDirect], 
		SendingLocationId,
		ReceivingLocationId)
	SELECT
		CONVERT(VARCHAR(MAX), t.BookingId) [TransactionId],
		tp.ProductId,
		t.ReceivingLocationId [LocationId],
		'Requisition' [Type],
		'Stock Requisition - Booking #' + CONVERT(VARCHAR(MAX), t.BookingId) + ' - From {0} to {1}' Narration,
		isnull(t.Quantity,0) [Quantity],
		null [Date],
		tp.CreatedDate [DateUTC],
		t.DateProcessed [DateProcessedUTC],
		tp.CreatedById [UserId],
		null [IsDirect], 
		t.SendingLocationId,
		t.ReceivingLocationId
	FROM
		Merchandising.StockTransferMovement t WITH(NOLOCK)
		JOIN Merchandising.StockRequisitionProduct tp WITH(NOLOCK) ON t.Bookingid = tp.BookingId AND t.[Type] = 'Requisition'	
		AND EXISTS (SELECT 1 FROM @ProductIds WHERE id = tp.ProductId)
		AND EXISTS (SELECT 1 FROM @ProductLocationIds WHERE id = t.ReceivingLocationId)	
	WHERE t.DateProcessed <= @EndDate OPTION (MAXDOP 4)

--------- ALLOCATION SENDING LOCATION---------------

	INSERT INTO #NegativeStockTransfer ( [TransactionId],
		ProductId,
		[LocationId],
		[Type],
		 Narration,
		 [Quantity],
		 [Date],
		[DateUTC],
		 [DateProcessedUTC],
		 [UserId],
		[IsDirect], 
		SendingLocationId,
		ReceivingLocationId)
	SELECT
		CONVERT(VARCHAR(MAX), t.BookingId) [TransactionId],
		tp.ProductId,
		t.SendingLocationId [LocationId],
		'Allocation' [Type],
		'Stock Allocation - Booking #' + CONVERT(VARCHAR(MAX), t.BookingId) + ' - From {0} to {1}' Narration,
		isnull(0 - t.Quantity,0) [Quantity],
		null [Date],
		tp.CreatedDate [DateUTC],
		t.DateProcessed [DateProcessedUTC],
		tp.CreatedById [UserId],
		null [IsDirect], 
		t.SendingLocationId,
		t.ReceivingLocationId
	FROM
		Merchandising.StockTransferMovement t WITH(NOLOCK)
		JOIN Merchandising.StockAllocationProduct tp WITH(NOLOCK) ON t.Bookingid = tp.BookingId AND t.[Type] = 'Allocation'		
		AND EXISTS (SELECT 1 FROM @ProductIds WHERE id = tp.ProductId)
		AND EXISTS (SELECT 1 FROM @ProductLocationIds WHERE id = t.SendingLocationId)
	WHERE t.DateProcessed <= @EndDate OPTION (MAXDOP 4)

--------- ALLOCATION RECEIVING LOCATION---------------

	INSERT INTO #NegativeStockTransfer ( [TransactionId],
		ProductId,
		[LocationId],
		[Type],
		 Narration,
		 [Quantity],
		 [Date],
		[DateUTC],
		 [DateProcessedUTC],
		 [UserId],
		[IsDirect], 
		SendingLocationId,
		ReceivingLocationId)
	SELECT
		CONVERT(VARCHAR(MAX), t.BookingId) [TransactionId],
		tp.ProductId,
		t.ReceivingLocationId [LocationId],
		'Allocation' [Type],
		'Stock Allocation - Booking #' + CONVERT(VARCHAR(MAX), t.BookingId) + ' - From {0} to {1}' Narration,
		isnull(t.Quantity,0) [Quantity],
		null [Date],
		tp.CreatedDate [DateUTC],
		t.DateProcessed [DateProcessedUTC],
		tp.CreatedById [UserId],
		null [IsDirect], 
		t.SendingLocationId,
		t.ReceivingLocationId
	FROM
		Merchandising.StockTransferMovement t WITH(NOLOCK)
		JOIN Merchandising.StockAllocationProduct tp WITH(NOLOCK) ON t.Bookingid = tp.BookingId AND t.[Type] = 'Allocation'
		AND EXISTS (SELECT 1 FROM @ProductIds WHERE id = tp.ProductId)
		AND  EXISTS (SELECT 1 FROM @ProductLocationIds WHERE id = t.ReceivingLocationId)
	WHERE t.DateProcessed <= @EndDate OPTION (MAXDOP 4)

--------- ADJUSTMENT ---------------

	INSERT INTO #NegativeStockTransfer ( [TransactionId],
		ProductId,
		[LocationId],
		[Type],
		 Narration,
		 [Quantity],
		 [Date],
		[DateUTC],
		 [DateProcessedUTC],
		 [UserId],
		[IsDirect], 
		SendingLocationId,
		ReceivingLocationId)
	SELECT 
		
		CONVERT(VARCHAR(MAX), a.Id) [TransactionId],
		r.ProductId,
		a.LocationId,
		'Adjustment' [Type],
		CASE WHEN c.id > 0 
			THEN 'Adjustment #'+ CONVERT(VARCHAR(MAX), a.id)+ ' - Stock Count #' + CONVERT(VARCHAR(MAX), c.id)
			ELSE 'Adjustment #'+ CONVERT(VARCHAR(MAX), a.id) + ' - ' + rsn.SecondaryReason + ' (' + ISNULL(a.ReferenceNumber, 'No reference')+')'
		END  Narration,
		r.Quantity,
		a.CreatedDate [Date],
		null [DateUTC],
		a.CreatedDate [DateProcessedUTC],
		a.CreatedById [UserId],
		null [IsDirect], 
		NULL AS SendingLocationId,
		NULL AS ReceivingLocationId
	
	FROM 
		Merchandising.StockAdjustment a WITH(NOLOCK)
		JOIN Merchandising.StockAdjustmentProduct r WITH(NOLOCK) ON r.StockAdjustmentId = a.Id
		LEFT JOIN Merchandising.StockAdjustmentSecondaryReason rsn WITH(NOLOCK) ON a.SecondaryReasonId = rsn.Id
		LEFT JOIN Merchandising.StockCount c WITH(NOLOCK) ON c.StockAdjustmentId = a.Id
		AND  EXISTS (SELECT 1 FROM @ProductIds WHERE id = r.ProductId)
		AND EXISTS (SELECT 1 FROM @ProductLocationIds WHERE id = a.LocationId)
	WHERE a.CreatedDate <= @EndDate OPTION (MAXDOP 4)

--------- GOODS RECEIPT ---------------

	INSERT INTO #NegativeStockTransfer ( [TransactionId],
		ProductId,
		[LocationId],
		[Type],
		 Narration,
		 [Quantity],
		 [Date],
		[DateUTC],
		 [DateProcessedUTC],
		 [UserId],
		[IsDirect], 
		SendingLocationId,
		ReceivingLocationId)
	SELECT
		CONVERT(VARCHAR(MAX), x.Id) [TransactionId],
		o.ProductId,
		x.LocationId,
		'GoodsReceipt' [Type],
		'Goods Receipt #'+ CONVERT(VARCHAR(MAX),x.id) + ' Delivery/Invoice: ' + COALESCE(x.VendorDeliveryNumber, x.VendorInvoiceNumber, 'No reference')+' - Purchase Order: ' + cast(o.PurchaseOrderId as varchar(20)) Narration,
		SUM(y.QuantityReceived) [Quantity],
		x.DateReceived [Date],
		null [DateUTC],
		x.CreatedDate [DateProcessedUTC],
		x.CreatedById [UserId],
		CAST(0 AS bit) [IsDirect], 
		NULL AS SendingLocationId,
		NULL AS ReceivingLocationId
	FROM
		Merchandising.GoodsReceipt x WITH(NOLOCK)
		JOIN Merchandising.GoodsReceiptProduct y WITH(NOLOCK) ON y.GoodsReceiptId = x.Id and x.CostConfirmed IS NOT NULL
		JOIN Merchandising.PurchaseOrderProduct o WITH(NOLOCK) ON y.PurchaseOrderProductId = o.Id
		JOIN Merchandising.PurchaseOrder z WITH(NOLOCK) ON o.PurchaseOrderId = z.Id
		AND EXISTS (SELECT 1 FROM @ProductIds WHERE id = o.ProductId)
		AND EXISTS (SELECT 1 FROM @ProductLocationIds WHERE id = x.LocationId)
	WHERE x.CreatedDate <= @EndDate
	GROUP BY
		 x.id
		,x.LocationId
		,x.DateReceived
		,x.CreatedDate
		,x.CreatedById
		,o.ProductId
		,o.PurchaseOrderId
		,x.VendorDeliveryNumber
		,x.VendorInvoiceNumber
    HAVING SUM(y.QuantityReceived) != 0 OPTION (MAXDOP 4)

--------- GOODS RECEIPT (DIRECT)---------------

	INSERT INTO #NegativeStockTransfer ( [TransactionId],
		ProductId,
		[LocationId],
		[Type],
		 Narration,
		 [Quantity],
		 [Date],
		[DateUTC],
		 [DateProcessedUTC],
		 [UserId],
		[IsDirect], 
		SendingLocationId,
		ReceivingLocationId)
	SELECT
		CONVERT(VARCHAR(MAX), x.Id) [TransactionId],
		y.ProductId,
		x.LocationId,
		'DirectGoodsReceipt' [Type],
		'Direct Goods Receipt #' + CONVERT(VARCHAR(MAX), x.Id)+ ' Delivery/Invoice: ' + COALESCE(x.VendorDeliveryNumber, x.VendorInvoiceNumber, 'No reference') Narration,
		y.QuantityReceived [Quantity],
		x.DateReceived [Date],
		null [DateUTC],
		x.CreatedDate [DateProcessedUTC],
		x.CreatedById [UserId],
		CAST(1 as bit) [IsDirect], 
		NULL AS SendingLocationId,
		NULL AS ReceivingLocationId
	FROM
		Merchandising.GoodsReceiptDirect x WITH(NOLOCK)
		JOIN Merchandising.GoodsReceiptDirectProduct y WITH(NOLOCK) ON y.GoodsReceiptDirectId = x.Id
		AND EXISTS (SELECT 1 FROM @ProductIds WHERE id = y.ProductId)
		AND EXISTS (SELECT 1 FROM @ProductLocationIds WHERE id = x.LocationId)
	WHERE x.CreatedDate <= @EndDate OPTION (MAXDOP 4)

--------- VENDOR RETURN ---------------

	INSERT INTO #NegativeStockTransfer ( [TransactionId],
		ProductId,
		[LocationId],
		[Type],
		 Narration,
		 [Quantity],
		 [Date],
		[DateUTC],
		 [DateProcessedUTC],
		 [UserId],
		[IsDirect], 
		SendingLocationId,
		ReceivingLocationId)
	SELECT
		CONVERT(VARCHAR(MAX), x.Id) [TransactionId],
		o.ProductId,
		g.LocationId,
		'VendorReturn' [Type],
		'Return to Vendor #' + CONVERT(VARCHAR(MAX), x.Id) + ' (' + ISNULL(x.ReferenceNumber, 'No reference')+') - Goods Receipt: #' + CONVERT(VARCHAR(MAX),r.GoodsReceiptId) Narration,
		y.QuantityReturned * -1 [Quantity],
		null [Date],
		x.CreatedDate DateUTC,
		x.CreatedDate [DateProcessedUTC],
		x.CreatedById [UserId],
		CAST(0 as bit) [IsDirect], 
		NULL AS SendingLocationId,
		NULL AS ReceivingLocationId
	FROM
		Merchandising.VendorReturn x WITH(NOLOCK)
		JOIN Merchandising.VendorReturnProduct y WITH(NOLOCK) ON y.VendorReturnId = x.Id and x.ReceiptType != 'Direct'
		JOIN Merchandising.GoodsReceipt g WITH(NOLOCK) ON g.Id = x.GoodsReceiptId
		AND EXISTS (SELECT 1 FROM @ProductLocationIds WHERE id = g.LocationId)
		JOIN Merchandising.GoodsReceiptProduct r WITH(NOLOCK) ON y.GoodsReceiptProductId = r.Id
		JOIN Merchandising.PurchaseOrderProduct o WITH(NOLOCK) ON r.PurchaseOrderProductId = o.Id
		AND EXISTS (SELECT 1 FROM @ProductIds WHERE id = o.ProductId)
    WHERE y.QuantityReturned != 0 AND x.CreatedDate <= @EndDate OPTION (MAXDOP 4)

--------- VENDOR RETURN (DIRECT)---------------

	INSERT INTO #NegativeStockTransfer ( [TransactionId],
		ProductId,
		[LocationId],
		[Type],
		 Narration,
		 [Quantity],
		 [Date],
		[DateUTC],
		 [DateProcessedUTC],
		 [UserId],
		[IsDirect], 
		SendingLocationId,
		ReceivingLocationId)
	SELECT
		CONVERT(VARCHAR(MAX), x.Id) [TransactionId],
		r.ProductId,
		d.LocationId,
		'VendorReturn' [Type],
		'Return to Vendor #' + CONVERT(VARCHAR(MAX),x.id) + ' (' + ISNULL(x.ReferenceNumber, 'No reference')+') -  Goods Receipt: #' + CONVERT(VARCHAR(MAX), d.Id)  Narration,
		y.QuantityReturned * -1 [Quantity],
		null [Date],
		x.CreatedDate DateUTC,
		x.CreatedDate [DateProcessedUTC],
		x.CreatedById [UserId],
		CAST(1 as bit) [IsDirect], 
		NULL AS SendingLocationId,
		NULL AS ReceivingLocationId
	FROM 
		Merchandising.VendorReturn x WITH(NOLOCK)
		JOIN Merchandising.VendorReturnProduct y WITH(NOLOCK) ON y.VendorReturnId = x.Id and x.ReceiptType = 'Direct'
		JOIN Merchandising.GoodsReceiptDirectProduct r WITH(NOLOCK) ON y.GoodsReceiptProductId = r.Id
		JOIN Merchandising.GoodsReceiptDirect d WITH(NOLOCK) ON r.GoodsReceiptDirectId = d.Id
		AND EXISTS (SELECT 1 FROM @ProductIds WHERE id = r.ProductId)
		AND EXISTS (SELECT 1 FROM @ProductLocationIds WHERE id = d.LocationId)
	WHERE x.CreatedDate <= @EndDate OPTION (MAXDOP 4)

--------- CINT ORDER ---------------


	INSERT INTO #NegativeStockTransfer ( [TransactionId],
		ProductId,
		[LocationId],
		[Type],
		 Narration,
		 [Quantity],
		 [Date],
		[DateUTC],
		 [DateProcessedUTC],
		 [UserId],
		[IsDirect], 
		SendingLocationId,
		ReceivingLocationId)
	SELECT
		x.PrimaryReference + '-' + x.ReferenceType + ':' + x.SecondaryReference AS TransactionId,
		x.ProductId,
		l.Id AS LocationId,
		'Sale' AS [Type],
		x.[Type] + ' - Account ' + x.PrimaryReference + ' ' + x.ReferenceType + ': ' + x.SecondaryReference AS Narration,
		x.Quantity * -1 AS Quantity,
		NULL AS [Date],
		ISNULL(co.TransactionDate, x.TransactionDate) AS DateUTC,
		x.TransactionDate AS DateProcessedUTC,
		NULL AS UserId,
		NULL AS IsDirect, 
		NULL AS SendingLocationId,
		NULL AS ReceivingLocationId
	FROM Merchandising.CintOrder x WITH(NOLOCK)
	INNER JOIN Merchandising.Product p WITH(NOLOCK)
		ON x.[Type] not IN ('RegularOrder', 'CancelOrder') 
		AND p.Sku = x.Sku 
	INNER JOIN Merchandising.Location l WITH(NOLOCK)
		ON RIGHT(l.SalesId, 3) = x.StockLocation
	LEFT JOIN Merchandising.CintOrder co WITH(NOLOCK)
		ON co.Id = x.CintRegularOrderId
	AND EXISTS (SELECT 1 FROM @ProductIds WHERE id = x.ProductId)
	AND EXISTS (SELECT 1 FROM @ProductLocationIds WHERE id = l.Id)
	WHERE x.TransactionDate <= @EndDate OPTION (MAXDOP 4)



	DELETE FROM #NegativeStockTransfer
	WHERE	ProductId NOT IN (
								SELECT	p.id
								FROM	Merchandising.Product p 
										LEFT JOIN Merchandising.ProductHierarchySummaryView ph 
											ON ph.ProductId = p.Id
										LEFT JOIN Merchandising.Brand b 
											ON p.Brandid = b.Id
								WHERE	p.ProductType IN ('RegularStock', 'RepossessedStock', 'SparePart','ProductWithoutStock')
										AND (p.PreviousProductType='RegularStock' OR p.PreviousProductType IS NULL) 
								)

	UPDATE	T
	SET		SKU = P.SKU,
			LongDescription = P.LongDescription ,
			ProductTags = P.Tags,
			BrandName = B.BrandName,
			StoreTypes = P.storetypes,
			Division = PH.DivisionName,
			Department = PH.DepartmentName,
			Class = ph.ClassName
	FROM	#NegativeStockTransfer t
			INNER JOIN Merchandising.Product p 
				ON t.productid = p.id
			LEFT JOIN Merchandising.ProductHierarchySummaryView ph 
				ON ph.ProductId = p.Id
			LEFT JOIN Merchandising.Brand b 
				ON p.Brandid = b.Id
	WHERE	p.ProductType in ('RegularStock', 'RepossessedStock', 'SparePart','ProductWithoutStock')
			and (p.PreviousProductType='RegularStock' or p.PreviousProductType is null) 


	SELECT	row_number() over( order by [DateProcessedUTC] ) id,
			x.Division,
			x.Department,
			x.Class,
			TransactionId,
			x.ProductId,
			x.SKU,
			REPLACE(ISNULL(x.LongDescription,''),'#',''),
			x.BrandName,
			x.ProductTags,
			l.Id [LocationId],
			l.Name [Location],
			l.Fascia,
			[Type],
			REPLACE(ISNULL(Narration,''),'#',''),
			Quantity,
			[Date],
			[DateUTC],
			DateProcessedUTC,
			UserId,
			u.FullName [User],
			x.IsDirect, 
			x.SendingLocationId,
			x.ReceivingLocationId
  FROM		#NegativeStockTransfer x
			LEFT JOIN [Admin].[User] u 
				ON x.UserId = u.id
			INNER JOIN Merchandising.Location l 
				ON x.LocationId = l.Id
			AND 
			(
				NULLIF(NULLIF(x.storetypes, ''), '[]') IS NULL 
				OR x.storetypes LIKE '%"' + l.storetype + '"%'
			) OPTION (MAXDOP 4)


 DROP TABLE #NegativeStockTransfer

END
GO






