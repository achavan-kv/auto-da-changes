DROP INDEX [Merchandising_GoodsReceiptProduct_GoodsReceiptId] ON [Merchandising].[GoodsReceiptProduct]
GO


ALTER TABLE [Merchandising].[GoodsReceiptProduct]
ALTER COLUMN [LastLandedCost] dbo.blueamount NOT NULL

CREATE NONCLUSTERED INDEX [Merchandising_GoodsReceiptProduct_GoodsReceiptId] ON [Merchandising].[GoodsReceiptProduct]
(
	[GoodsReceiptId] ASC
)
INCLUDE ( 	[PurchaseOrderProductId],
	[QuantityReceived],
	[QuantityCancelled],
	[LastLandedCost]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO