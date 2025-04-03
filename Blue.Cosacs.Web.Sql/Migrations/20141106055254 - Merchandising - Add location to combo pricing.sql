-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
IF  EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID(N'Merchandising.[ComboProductPrice]'))
DROP TABLE Merchandising.[ComboProductPrice]
GO

IF  EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID(N'Merchandising.[ComboProduct]'))
DROP TABLE Merchandising.[ComboProduct]
GO

CREATE TABLE Merchandising.[ComboProduct] (
	Id [int] NOT NULL IDENTITY(1,1),
	ProductId int NOT NULL,
	ComboId int NOT NULL,
	Quantity int NOT NULL	
	CONSTRAINT [PK_ComboProducts] PRIMARY KEY (Id ASC),
	CONSTRAINT [FK_ComboProduct_Product] FOREIGN KEY ([ProductId]) REFERENCES [Merchandising].[Product]([Id]),	
	CONSTRAINT [FK_ComboProduct_Combo] FOREIGN KEY ([ComboId]) REFERENCES [Merchandising].[Combo]([Id]))	

CREATE UNIQUE NONCLUSTERED INDEX [IX_ComboProduct] ON [Merchandising].[ComboProduct] ([ProductId], [ComboId])
	
CREATE TABLE Merchandising.[ComboProductPrice] (
	Id [int] NOT NULL IDENTITY(1,1),
	ComboProductId int NOT NULL,
	LocationId [int] NOT NULL,
	RegularPrice decimal(15,4) NULL,
	CashPrice decimal(15,4) NULL,
	DutyFreePrice decimal(15,4) NULL,
	CONSTRAINT [PK_ComboProductPrices] PRIMARY KEY (Id ASC),
	CONSTRAINT [FK_ComboLocation_Location] FOREIGN KEY([LocationId]) REFERENCES [Merchandising].[Location] ([Id]),
	CONSTRAINT [FK_ComboProductPrice_ComboProduct] FOREIGN KEY ([ComboProductId]) REFERENCES [Merchandising].[ComboProduct]([Id]))	

CREATE UNIQUE NONCLUSTERED INDEX [IX_ComboProductPrice] ON [Merchandising].[ComboProductPrice] ([ComboProductId], [LocationId])