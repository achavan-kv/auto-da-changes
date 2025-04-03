-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
ALTER TABLE [Merchandising].[Product]
DROP CONSTRAINT FK_Product_StockLevel
	
ALTER TABLE [Merchandising].[Product]
DROP COLUMN StockLevelId

DROP TABLE [Merchandising].[ProductStockLevel]

CREATE TABLE [Merchandising].[ProductStockLevel] (
	Id int NOT NULL IDENTITY(1,1),
	LocationId int NOT NULL,
	ProductId int NOT NULL,
	StockOnHand int NOT NULL DEFAULT(0),
	StockOnOrder int NOT NULL DEFAULT(0),
	StockAvailable int NOT NULL DEFAULT(0),
	CONSTRAINT [PK_ProductStockLevel] PRIMARY KEY CLUSTERED (Id ASC),
	CONSTRAINT [FK_ProductStockLevel_Location] FOREIGN KEY ([LocationId]) REFERENCES [Merchandising].[Location](Id),
	CONSTRAINT [FK_ProductStockLevel_Product] FOREIGN KEY ([ProductId]) REFERENCES [Merchandising].[Product](Id)
)