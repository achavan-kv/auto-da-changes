-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
CREATE TABLE [Merchandising].[ProductTag] (
	Id int NOT NULL IDENTITY(1,1),
	ProductId int NOT NULL,
	Tag varchar(100) NOT NULL,
	TagType varchar(100) NOT NULL,
	CONSTRAINT [PK_ProductTag] PRIMARY KEY CLUSTERED (Id ASC)
)

ALTER TABLE [Merchandising].[ProductTag] WITH CHECK ADD CONSTRAINT [FK_ProductTag_Product] FOREIGN KEY ([ProductId]) REFERENCES [Merchandising].[Product]([Id])
ALTER TABLE [Merchandising].[ProductTag] CHECK CONSTRAINT [FK_ProductTag_Product]