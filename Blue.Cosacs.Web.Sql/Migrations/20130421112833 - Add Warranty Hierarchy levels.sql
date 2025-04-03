CREATE SCHEMA Warranty
GO

CREATE TABLE Warranty.Category
(
	Id int IDENTITY(1,1) NOT NULL,
	Name varchar(100) UNIQUE NOT NULL,
	Position int NOT NULL
)

ALTER TABLE Warranty.Category
ADD CONSTRAINT [PK_WarrantyCategories] PRIMARY KEY (Id)



CREATE TABLE Warranty.Tag
(
	Id int IDENTITY(1,1) NOT NULL,
	CategoryId int,
	Name varchar(100) UNIQUE NOT NULL
)

ALTER TABLE Warranty.Tag
ADD CONSTRAINT [PK_WarrantyTags] PRIMARY KEY (Id)

ALTER TABLE [Warranty].[Tag] WITH CHECK ADD CONSTRAINT [FK_Tag_Categories] FOREIGN KEY([CategoryId])
REFERENCES [Warranty].[Category] ([Id])
ON DELETE CASCADE
GO



CREATE TABLE Warranty.ProductWarranty
(
	Id int IDENTITY(1,1) NOT NULL,
	Name varchar(100) NOT NULL
)

ALTER TABLE Warranty.ProductWarranty
ADD CONSTRAINT [PK_ProductWarranty] PRIMARY KEY (Id)



CREATE TABLE Warranty.ProductWarrantyTags
(
	ProductWarrantyId int NOT NULL,
	TagId int NOT NULL
)

ALTER TABLE Warranty.ProductWarrantyTags
ADD CONSTRAINT [PK_WarrantyProductWarrantyTags] PRIMARY KEY (ProductWarrantyId, TagId)

ALTER TABLE [Warranty].[ProductWarrantyTags] WITH CHECK ADD CONSTRAINT [FK_ProductWarrantyTags_ProductWarranty] FOREIGN KEY([ProductWarrantyId])
REFERENCES [Warranty].[ProductWarranty] ([Id])
ON DELETE CASCADE

ALTER TABLE [Warranty].[ProductWarrantyTags] WITH CHECK ADD CONSTRAINT [FK_ProductWarrantyTags_Tag] FOREIGN KEY([TagId])
REFERENCES [Warranty].[Tag] ([Id])
ON DELETE CASCADE
GO
