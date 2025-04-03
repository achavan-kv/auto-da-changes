IF EXISTS (SELECT * FROM SYS.OBJECTS AS o JOIN SYS.SCHEMAS S ON O.schema_id = S.schema_id WHERE O.NAME  = 'trAshleyProductStockMaintainance' AND S.NAME = 'Merchandising')
	DROP TRIGGER [Merchandising].[trAshleyProductStockMaintainance] 
GO
/*
	STEPS :
		1. Find PurchaseOrderProductId, and QuantityReceived from GoodsReceiptProduct ie. INSERTED magical Table
		2. Find ProductId, and PurchaseOrderId from PurchaseOrderProduct
		3. Find VendorId, and ReceivingLocationId from PurchaseOrder
		3. Check wheather the product is Ashley attributed or not,
			if Yes,
					then Check weather product exists in AshleyProductStockMaintainance for vendor or not
					if Yes,
							Update StockAvailable of table AshleyProductStockMaintainance to StockAvailable +  QuantityReceived
					if No,
							Insert ProductId, VendorId, StockAvailable, LocationId in table AshleyProductStockMaintainance
			if No,
					Do Nothing
*/

CREATE TRIGGER [Merchandising].[trAshleyProductStockMaintainance]
       ON [Merchandising].[GoodsReceiptProduct]
AFTER INSERT, UPDATE
AS
BEGIN
       SET NOCOUNT ON;
 
       DECLARE @PurchaseOrderProductId INT, @ProductId INT, @VendorId INT, @PurchaseOrderId INT,
			   @QuantityReceived INT, @ReceivingLocationId INT;
 
       SELECT @PurchaseOrderProductId = PurchaseOrderProductId, @QuantityReceived =QuantityReceived FROM INSERTED ;

	   SELECT @ProductId = ProductId, @PurchaseOrderId = PurchaseOrderId  
	   FROM  [Merchandising].[PurchaseOrderProduct] 
	   WHERE ID = @PurchaseOrderProductId;

	   SELECT @VendorId = VendorId, @ReceivingLocationId = ReceivingLocationId
	   FROM [Merchandising].[PurchaseOrder]
	   WHERE Id = @PurchaseOrderId

	   IF EXISTS (SELECT 'A' FROM [Merchandising].[ProductAttributes] WHERE ProductId = @ProductId AND IsAshleyProduct = 1) 
	   BEGIN

		IF EXISTS(SELECT 'A' FROM [Merchandising].AshleyProductStockMaintainance WHERE ProductId = @ProductId AND VendorId = @VendorId AND LocationId = @ReceivingLocationId)
		BEGIN

			UPDATE [Merchandising].AshleyProductStockMaintainance 
			SET StockAvailable = StockAvailable + @QuantityReceived
			WHERE ProductId = @ProductId AND VendorId = @VendorId AND LocationId = @ReceivingLocationId

		END
		ELSE
		BEGIN
			INSERT INTO [Merchandising].AshleyProductStockMaintainance (ProductId, VendorId, StockAvailable, LocationId)
			VALUES (@ProductId, @VendorId, @QuantityReceived, @ReceivingLocationId)
		END
	   END
END