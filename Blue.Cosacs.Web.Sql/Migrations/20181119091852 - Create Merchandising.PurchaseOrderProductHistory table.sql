IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'PurchaseOrderProductHistory'
               AND TABLE_SCHEMA = 'Merchandising')
BEGIN

CREATE TABLE [Merchandising].[PurchaseOrderProductHistory](
	[Id] [int]  NULL,
	[ProductId] [int]  NULL,
	[Sku] [varchar](100)  NULL,
	[Description] [varchar](240) NULL,
	[RequestedDeliveryDate] [date] NOT NULL,
	[QuantityOrdered] [int] NOT NULL,
	[UnitCost] [decimal](19, 4) NOT NULL,
	[Comments] [varchar](max) NULL,
	[EstimatedDeliveryDate] [date] NULL,
	[PurchaseOrderId] [int] NOT NULL,
	[PreLandedUnitCost] [decimal](19, 4) NOT NULL,
	[PreLandedExtendedCost] [decimal](19, 4) NOT NULL,
	[LabelRequired] [bit] NULL,
	[QuantityCancelled] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

END
GO