CREATE TABLE [Merchandising].[StockAllocationProduct](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[StockAllocationId] [int] NOT NULL,
	[ReceivingLocationId] int not null,
	[ProductId] [int] NOT NULL,
	[Quantity] [int] NOT NULL,
 CONSTRAINT [PK_Merchandising_StockAllocationProduct] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [Merchandising].[StockAllocationProduct]  WITH CHECK ADD  CONSTRAINT [FK_Merchandising_StockAllocationProduct_Product] FOREIGN KEY([ProductId])
REFERENCES [Merchandising].[Product] ([Id])
GO

ALTER TABLE [Merchandising].[StockAllocationProduct] CHECK CONSTRAINT [FK_Merchandising_StockAllocationProduct_Product]
GO

ALTER TABLE [Merchandising].[StockAllocationProduct]  WITH CHECK ADD  CONSTRAINT [FK_Merchandising_StockAllocationProduct_StockAllocation] FOREIGN KEY([StockAllocationId])
REFERENCES [Merchandising].[StockAllocation] ([Id])
GO

ALTER TABLE [Merchandising].[StockAllocationProduct] CHECK CONSTRAINT [FK_Merchandising_StockAllocationProduct_StockAllocation]
GO

ALTER TABLE [Merchandising].StockAllocationProduct  WITH NOCHECK ADD  CONSTRAINT [FK_StockAllocationProduct_ReceivingLocationId] FOREIGN KEY([ReceivingLocationId])
REFERENCES  [Merchandising].[Location] ([Id])
GO

ALTER TABLE [Merchandising].StockAllocationProduct CHECK CONSTRAINT [FK_StockAllocationProduct_ReceivingLocationId]
GO
