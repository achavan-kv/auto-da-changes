-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
CREATE TABLE Merchandising.CintOrder (
	Id INT IDENTITY(1,1) NOT NULL,
	RunNo INT NOT NULL,
	[Type] varchar(50) NOT NULL,
	Reference varchar(50) NOT NULL,
	SaleType varchar(50) NOT NULL,
	SaleLocation varchar(50) NOT NULL,
	Sku varchar(10) NOT NULL,
	ProductId int NOT NULL,
	StockLocation varchar(50) NOT NULL,
	ParentSku varchar(10) NULL,
	ParentId int NULL,
	TransactionDate datetime NOT NULL,
	Quantity int NOT NULL,
	Price decimal NOT NULL,
	Tax decimal NOT NULL,
	CONSTRAINT [PK_CintOrder] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
))

