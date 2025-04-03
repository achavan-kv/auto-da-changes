-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF EXISTS (
	SELECT 1 
	FROM sys.tables t 
	inner join sys.schemas s on s.schema_id = t.schema_id and s.name = 'Merchandising'
	where t.name = 'TaxRate'
	)
DROP TABLE [Merchandising].[TaxRate]

CREATE TABLE [Merchandising].[TaxRate] (
	Id int NOT NULL IDENTITY(1,1),
	Name varchar(50) NOT NULL,
	Rate decimal(15,4) NOT NULL,
	EffectiveDate date NOT NULL,	
	ProductId int,
	CONSTRAINT [PK_TaxRate] PRIMARY KEY CLUSTERED (Id ASC),
	CONSTRAINT [FK_TaxRate_Product] FOREIGN KEY ([ProductId]) REFERENCES [Merchandising].[Product](Id)
)

ALTER TABLE [Merchandising].[TaxRate]
ADD CONSTRAINT DF_TaxRate_Rate 
DEFAULT 0 FOR [Rate]

