CREATE TABLE [Merchandising].[StockRequisitionProduct](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[StockRequisitionId] [int] NOT NULL,
	[ProductId] [int] NOT NULL,
	[Quantity] [int] NOT NULL,
	[Comments] [varchar](max) NULL,
	[ReferenceNumber] [varchar](50) NULL,
	[AverageWeightedCost] [money] NOT NULL,
 CONSTRAINT [PK_Merchandising_StockRequisitionProduct] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO


ALTER TABLE [Merchandising].[StockRequisitionProduct] ADD  DEFAULT ((0)) FOR [AverageWeightedCost]
GO

ALTER TABLE [Merchandising].[StockRequisitionProduct]  WITH CHECK ADD  CONSTRAINT [FK_Merchandising_StockRequisitionProduct_Product] FOREIGN KEY([ProductId])
REFERENCES [Merchandising].[Product] ([Id])
GO

ALTER TABLE [Merchandising].[StockRequisitionProduct] CHECK CONSTRAINT [FK_Merchandising_StockRequisitionProduct_Product]
GO

ALTER TABLE [Merchandising].[StockRequisitionProduct]  WITH CHECK ADD  CONSTRAINT [FK_Merchandising_StockRequisitionProduct_StockRequisition] FOREIGN KEY([StockRequisitionId])
REFERENCES [Merchandising].[StockRequisition] ([Id])
GO

ALTER TABLE [Merchandising].[StockRequisitionProduct] CHECK CONSTRAINT [FK_Merchandising_StockRequisitionProduct_StockRequisition]
GO


