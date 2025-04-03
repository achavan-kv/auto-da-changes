
IF TYPE_ID(N'Merchandising.CintOrderStatsTVP') IS NOT NULL 
DROP TYPE Merchandising.CintOrderStatsTVP
GO


CREATE TYPE Merchandising.CintOrderStatsTVP AS  TABLE
(
	PrimaryReference varchar(50) NOT NULL,
	Sku Varchar(20) NOT NULL,
	ReferenceType varchar(20) NOT NULL,
	SecondaryReference varchar(20) NOT NULL,
	ParentSku varchar(20) NOT NULL,
	StockLocation varchar(50) NOT NULL,
	OrderId INT NOT NULL 
)
GO

