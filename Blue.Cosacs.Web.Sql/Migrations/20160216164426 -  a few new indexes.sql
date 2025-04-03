-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
CREATE NONCLUSTERED INDEX Merchandising_GoodsReceiptProduct_GoodsReceiptId
ON Merchandising.GoodsReceiptProduct (GoodsReceiptId)
INCLUDE (PurchaseOrderProductId,QuantityReceived,QuantityCancelled,LastLandedCost)
GO

CREATE NONCLUSTERED INDEX IX_PurchaseOrderProduct_PurchaseOrderId
ON Merchandising.PurchaseOrderProduct (PurchaseOrderId)
INCLUDE (Id,ProductId,QuantityOrdered,QuantityCancelled)
GO

CREATE NONCLUSTERED INDEX IX_VendorReturnProduct_VendorReturnId
ON Merchandising.VendorReturnProduct (VendorReturnId)
INCLUDE (QuantityReturned)
GO

CREATE NONCLUSTERED INDEX Merchandising_GoodsReceiptResume_ReferenceNumberCsl_ReceiptProductId
ON Merchandising.GoodsReceiptResume (ReferenceNumberCsl, ReceiptProductId)
INCLUDE (locationid, productid, lastlandedcost, vendorid, purchaseorderid)
GO
