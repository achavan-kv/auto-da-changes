-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
CREATE TABLE [Merchandising].[TaxRate] (
	Id int NOT NULL IDENTITY(1,1),
	Name varchar(50) NOT NULL,
	Rate decimal NOT NULL DEFAULT(0),
	EffectiveDate date NOT NULL,	
	ProductId int,
	CONSTRAINT [PK_TaxRate] PRIMARY KEY CLUSTERED (Id ASC),
	CONSTRAINT [FK_TaxRate_Product] FOREIGN KEY ([ProductId]) REFERENCES [Merchandising].[Product](Id)
)

