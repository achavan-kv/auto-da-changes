-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
CREATE TABLE [Merchandising].[CostPrice] (
	Id int NOT NULL IDENTITY(1,1),	
	SupplierCost money NOT NULL DEFAULT(0),	
	LastLandedCost money NOT NULL DEFAULT(0),
	AverageWeightedCost money NOT NULL DEFAULT(0),
	SupplierCurrency varchar(10) NOT NULL,
	ProductId int,
	LastUpdated datetime NOT NULL,
	CONSTRAINT [PK_CostPrice] PRIMARY KEY CLUSTERED (Id ASC),
	CONSTRAINT [FK_CostPrice_Product] FOREIGN KEY ([ProductId]) REFERENCES [Merchandising].[Product](Id)	
)