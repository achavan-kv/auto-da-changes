-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
CREATE TABLE Merchandising.Brand (
	Id int IDENTITY(1,1) NOT NULL,
	BrandCode varchar(6) NOT NULL,
	BrandName varchar(25) NOT NULL,
	CONSTRAINT [PK_Brand] PRIMARY KEY CLUSTERED ([Id] ASC))



