-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF EXISTS (SELECT 1 FROM sys.types WHERE is_table_type = 1 AND name = 'StockCountProducts')
DROP TYPE [Merchandising].[StockCountProducts]
GO

CREATE TYPE [Merchandising].[StockCountProducts] AS TABLE(
	[Id] int NULL,
	[StockAdjustmentId] int NULL,
	[ProductId] int NULL,
	[Quantity] int NULL,
	[Comments] VARCHAR(MAX) NULL,
	[ReferenceNumber] VARCHAR(50) NULL,
	[AverageWeightedCost] money NULL
)
GO

