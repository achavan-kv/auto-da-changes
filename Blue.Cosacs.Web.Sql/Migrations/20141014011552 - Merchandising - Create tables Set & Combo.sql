-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

ALTER TABLE Merchandising.Product
ALTER COLUMN ShortDescription VARCHAR(100) NOT NULL

ALTER TABLE Merchandising.Product
ALTER COLUMN LongDescription VARCHAR(100) NOT NULL

CREATE UNIQUE NONCLUSTERED INDEX [IX_Merchandising_Product] ON [Merchandising].[Product] (SKU)

CREATE TABLE Merchandising.[Combo] (
	Id int not null primary key references Merchandising.Product(Id),
	StartDate Date not null,
	EndDate Date not null)

CREATE TABLE Merchandising.[Set] (
	Id int not null primary key references Merchandising.Product(Id),
	Price money not null)
	
cReAtE TaBlE Merchandising.[ComboProduct] (
	Id [int] NOT NULL IDENTITY(1,1),
	ProductId int NOT NULL,
	ComboId int NOT NULL,
	Quantity int NOT NULL,
	Price money NOT NULL,
	CONSTRAINT [PK_ComboProducts] PRIMARY KEY (Id ASC))

CREATE TABLE Merchandising.[SetProduct] (
	Id [int] NOT NULL IDENTITY(1,1),
	ProductId int NOT NULL,
	SetId int NOT NULL,
	Quantity int not null,
	CONSTRAINT [PK_SetProducts] PRIMARY KEY (Id ASC))


ALTER TABLE [Merchandising].[ComboProduct] WITH CHECK ADD CONSTRAINT [FK_ComboProduct_Product] FOREIGN KEY ([ProductId]) REFERENCES [Merchandising].[Product]([Id])
ALTER TABLE [Merchandising].[SetProduct] WITH CHECK ADD CONSTRAINT [FK_SetProduct_Product] FOREIGN KEY ([ProductId]) REFERENCES [Merchandising].[Product]([Id])

ALTER TABLE [Merchandising].[ComboProduct] WITH CHECK ADD CONSTRAINT [FK_ComboProduct_Combo] FOREIGN KEY ([ComboId]) REFERENCES [Merchandising].[Combo]([Id])
ALTER TABLE [Merchandising].[SetProduct] WITH CHECK ADD CONSTRAINT [FK_SetProduct_Set] FOREIGN KEY ([SetId]) REFERENCES [Merchandising].[Set]([Id])

CREATE UNIQUE NONCLUSTERED INDEX [IX_ComboProduct] ON [Merchandising].[ComboProduct] ([ProductId], [ComboId])
CREATE UNIQUE NONCLUSTERED INDEX [IX_SetProduct] ON [Merchandising].[SetProduct] ([ProductId], [SetId])