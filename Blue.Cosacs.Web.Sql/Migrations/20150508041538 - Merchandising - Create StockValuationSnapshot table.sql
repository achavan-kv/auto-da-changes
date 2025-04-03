-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
--
CREATE TABLE [Merchandising].[StockValuationSnapshot](
	[Id] int Identity(1,1),
	[ProductId] int,	
	[LocationId] int,	
	[StockOnHandQuantity] int,
	[StockOnHandValue] decimal,
	[StockOnHandSalesValue] decimal,
	[SnapshotDateId] int
) ON [PRIMARY]

ALTER TABLE [Merchandising].[StockValuationSnapshot]
WITH CHECK ADD CONSTRAINT FK_StockValuation_ProductId_Product
FOREIGN KEY (ProductId)
REFERENCES [Merchandising].Product(Id) 

ALTER TABLE [Merchandising].[StockValuationSnapshot]
WITH CHECK ADD CONSTRAINT FK_StockValuation_LocationId_Location
FOREIGN KEY (LocationId)
REFERENCES [Merchandising].Location(Id)