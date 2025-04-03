-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
CREATE TABLE [Merchandising].[PurchaseOrder](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[VendorId] [int] NOT NULL,
	[VendorName] varchar(100) NOT NULL,
	[DeliveryDate] [date] NOT NULL,
	[ReceivingLocationId] [int] NOT NULL,
	[ReceivingLocationName] varchar(100) NOT NULL,
	[ReferenceNumbers] [text] NULL,
	[Currency] varchar(100),
	[Comments] [text] NULL,
 CONSTRAINT [PK_PurchaseOrder] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

ALTER TABLE [Merchandising].[PurchaseOrder]  WITH CHECK ADD  CONSTRAINT [FK_PurchaseOrder_Location] FOREIGN KEY([ReceivingLocationId])
REFERENCES [Merchandising].[Location] ([Id])
GO

ALTER TABLE [Merchandising].[PurchaseOrder] CHECK CONSTRAINT [FK_PurchaseOrder_Location]
GO

ALTER TABLE [Merchandising].[PurchaseOrder]  WITH CHECK ADD  CONSTRAINT [FK_PurchaseOrder_Supplier] FOREIGN KEY([VendorId])
REFERENCES [Merchandising].[Supplier] ([Id])
GO

ALTER TABLE [Merchandising].[PurchaseOrder] CHECK CONSTRAINT [FK_PurchaseOrder_Supplier]
GO


CREATE TABLE [Merchandising].[PurchaseOrderProduct](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProductId] [int] NOT NULL,
	[ProductDescription] varchar(100) NOT NULL,
	[Sku] [varchar](100) NOT NULL,
	[Description] [varchar](100) NULL,
	[DeliveryDate] [date] NOT NULL,
	[Quantity] [int] NOT NULL,
	[UnitCost] [money] NOT NULL,
	[Comments] [text] NULL,
 CONSTRAINT [PK_PurchaseOrderProduct] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

ALTER TABLE [Merchandising].[PurchaseOrderProduct]  WITH CHECK ADD  CONSTRAINT [FK_PurchaseOrderProduct_Product] FOREIGN KEY([ProductId])
REFERENCES [Merchandising].[Product] ([Id])
GO

ALTER TABLE [Merchandising].[PurchaseOrderProduct] CHECK CONSTRAINT [FK_PurchaseOrderProduct_Product]
GO

