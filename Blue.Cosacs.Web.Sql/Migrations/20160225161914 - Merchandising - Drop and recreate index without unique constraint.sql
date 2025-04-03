-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
IF EXISTS(SELECT 'a' FROM sys.indexes 
          WHERE name = 'IX_StockAdjustmentProduct')
BEGIN
    DROP INDEX [IX_StockAdjustmentProduct] ON [Merchandising].[StockAdjustmentProduct]
END
GO

CREATE NONCLUSTERED INDEX [IX_StockAdjustmentProduct] ON [Merchandising].[StockAdjustmentProduct] (
	StockAdjustmentId ASC,
	ProductId ASC
)
GO