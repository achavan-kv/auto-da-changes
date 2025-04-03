-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
IF  EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID(N'Merchandising.[SetLocation]'))
DROP TABLE [Merchandising].[SetLocation]
GO

IF  EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID(N'Merchandising.[SetProduct]'))
DROP TABLE [Merchandising].[SetProduct]
GO

CREATE TABLE [Merchandising].[SetProduct](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProductId] [int] NOT NULL,
	[SetId] [int] NOT NULL,
	[Quantity] [int] NOT NULL
	CONSTRAINT [PK_SetProduct] PRIMARY KEY (Id ASC),
	CONSTRAINT [FK_SetProduct_Product] FOREIGN KEY([ProductId]) REFERENCES [Merchandising].[Product] ([Id]),
	CONSTRAINT [FK_SetProduct_Set] FOREIGN KEY([SetId]) REFERENCES [Merchandising].[Product] ([Id]))

CREATE UNIQUE NONCLUSTERED INDEX [IX_SetProduct] ON [Merchandising].[SetProduct] ([SetId], [ProductId])

CREATE TABLE [Merchandising].[SetLocation](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[LocationId] [int] NOT NULL,
	[RegularPrice] decimal(15,4) NULL,
	[CashPrice] decimal(15,4) NULL,
	[DutyFreePrice] decimal(15,4) NULL,
	[SetId] [int] NOT NULL,
	CONSTRAINT [PK_SetLocation] PRIMARY KEY (Id ASC),
	CONSTRAINT [FK_SetLocation_Location] FOREIGN KEY([LocationId]) REFERENCES [Merchandising].[Location] ([Id]),
	CONSTRAINT [FK_SetLocation_Set] FOREIGN KEY([SetId]) REFERENCES [Merchandising].[Product] ([Id]))

CREATE UNIQUE NONCLUSTERED INDEX [IX_SetProductPrice] ON [Merchandising].[SetLocation] ([LocationId], [SetId])