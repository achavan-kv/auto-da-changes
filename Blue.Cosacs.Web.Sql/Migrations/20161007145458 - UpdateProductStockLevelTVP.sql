IF OBJECT_ID('Merchandising.UpdateProductStockLevel') IS NOT NULL
DROP PROCEDURE Merchandising.UpdateProductStockLevel
GO

IF TYPE_ID(N'Merchandising.UpdateProductStockLevelTVP') IS NOT NULL 
DROP TYPE Merchandising.UpdateProductStockLevelTVP
GO


CREATE TYPE Merchandising.UpdateProductStockLevelTVP AS  TABLE
(
	[LocationId] [int] NOT NULL,
	[ProductId] [int] NOT NULL,
	[StockOnHand] [int] NOT NULL,
	[StockOnOrder] [int] NOT NULL,
	[StockAvailable] [int] NOT NULL
)
GO
