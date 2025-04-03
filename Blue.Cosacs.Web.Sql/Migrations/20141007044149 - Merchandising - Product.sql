-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
CREATE TABLE [Merchandising].[Product] (
	Id int NOT NULL IDENTITY(1,1),
	SKU varchar(100) NOT NULL,
	InternalUPC varchar(100) NULL,
	VendorUPC varchar(100) NULL,
	ShortDescription varchar(100) NULL,
	LongDescription text NULL,
	[Type] varchar(100) NULL,
	PrimaryBrand varchar(100) NULL,
	SecondaryBrand varchar(100) NULL,
	ModelNumber varchar(100) NULL,
	SupplierSKU varchar(100) NULL,
	CONSTRAINT [PK_Product] PRIMARY KEY (Id ASC))
