-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 

CREATE TABLE [Merchandising].[StockTransferProduct](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[StockTransferId] [int] NOT NULL,
	[ProductId] [int] NOT NULL,
	[Quantity] [int] NOT NULL,
	[Comments] [varchar](max) NULL,
	[ReferenceNumber] [varchar](50) NULL,
	[AverageWeightedCost] [money] NOT NULL,
 CONSTRAINT [PK_Merchandising_StockTransferProduct] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO


ALTER TABLE [Merchandising].[StockTransferProduct] ADD  DEFAULT ((0)) FOR [AverageWeightedCost]
GO

ALTER TABLE [Merchandising].[StockTransferProduct]  WITH CHECK ADD  CONSTRAINT [FK_Merchandising_StockTransferProduct_Product] FOREIGN KEY([ProductId])
REFERENCES [Merchandising].[Product] ([Id])
GO

ALTER TABLE [Merchandising].[StockTransferProduct] CHECK CONSTRAINT [FK_Merchandising_StockTransferProduct_Product]
GO

ALTER TABLE [Merchandising].[StockTransferProduct]  WITH CHECK ADD  CONSTRAINT [FK_Merchandising_StockTransferProduct_StockTransfer] FOREIGN KEY([StockTransferId])
REFERENCES [Merchandising].[StockTransfer] ([Id])
GO

ALTER TABLE [Merchandising].[StockTransferProduct] CHECK CONSTRAINT [FK_Merchandising_StockTransferProduct_StockTransfer]
GO


