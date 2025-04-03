IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'GoodsReceiptProductHistory'
               AND TABLE_SCHEMA = 'Merchandising')
BEGIN
CREATE TABLE [Merchandising].[GoodsReceiptProductHistory](
	[Id] [int]  NULL,
	[GoodsReceiptId] [int]  NULL,
	[PurchaseOrderProductId] [int]  NULL,
	[QuantityReceived] [int]  NULL,
	[QuantityBackOrdered] [int]  NULL,
	[ReasonForCancellation] [varchar](100) NULL,
	[QuantityCancelled] [int] NULL,
	[LastLandedCost] [dbo].[BlueAmount] NOT NULL
) ON [PRIMARY]

END
GO