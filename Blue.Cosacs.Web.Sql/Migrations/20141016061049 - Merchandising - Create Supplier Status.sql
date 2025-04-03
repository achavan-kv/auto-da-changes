-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

CREATE TABLE [Merchandising].[SupplierStatus] (
	Id int NOT NULL IDENTITY(1,1),
	Name varchar(50) NOT NULL,
	IsActive bit NOT NULL DEFAULT(0),
	CONSTRAINT [PK_SupplierStatus] PRIMARY KEY CLUSTERED (Id ASC)
)

INSERT INTO [Merchandising].[SupplierStatus] (Name, IsActive) VALUES ('Active', 1);
INSERT INTO [Merchandising].[SupplierStatus] (Name, IsActive) VALUES ('Inactive', 0);

ALTER TABLE [Merchandising].[Supplier]
	ADD [Status] int NOT NULL DEFAULT(1),
	CONSTRAINT [FK_Supplier_Status] FOREIGN KEY ([Status]) REFERENCES [Merchandising].[SupplierStatus](Id)