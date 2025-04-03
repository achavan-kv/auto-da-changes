-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
IF  EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID(N'Merchandising.[SetProductPrice]'))
DROP TABLE [Merchandising].[SetProductPrice]
GO

IF  EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID(N'Merchandising.[SetProduct]'))
DROP TABLE [Merchandising].[SetProduct]
GO

CREATE TABLE [Merchandising].[SetProduct](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProductId] [int] NOT NULL,
	[SetId] [int] NOT NULL,
	[Quantity] [int] NOT NULL
	CONSTRAINT [PK_Set] PRIMARY KEY (Id ASC),
	CONSTRAINT [FK_SetProduct_Product] FOREIGN KEY([ProductId]) REFERENCES [Merchandising].[Product] ([Id]))

CREATE UNIQUE NONCLUSTERED INDEX [IX_SetProduct] ON [Merchandising].[SetProduct] ([SetId], [ProductId])

CREATE TABLE [Merchandising].[SetProductPrice](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SetProductId] [int] NOT NULL,
	[PriceId] [int] NOT NULL,	
	CONSTRAINT [PK_SetProductPrice] PRIMARY KEY (Id ASC),
	CONSTRAINT [FK_SetProductPrice_SetProduct] FOREIGN KEY([SetProductId]) REFERENCES [Merchandising].[SetProduct] ([Id]),
	CONSTRAINT [FK_SetProductPrice_Price] FOREIGN KEY([PriceId]) REFERENCES [Merchandising].[RetailPrice] ([Id]))

CREATE UNIQUE NONCLUSTERED INDEX [IX_SetProductPrice] ON [Merchandising].[SetProductPrice] ([SetProductId], [PriceId])