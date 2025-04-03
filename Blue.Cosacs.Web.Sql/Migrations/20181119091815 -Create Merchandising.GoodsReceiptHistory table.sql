IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'GoodsReceiptHistory'
               AND TABLE_SCHEMA = 'Merchandising')
BEGIN
CREATE TABLE [Merchandising].[GoodsReceiptHistory](
	[Id] [int]  NULL,
	[LocationId] [int]  NULL,
	[ReceivedById] [int]  NULL,
	[VendorDeliveryNumber] [varchar](100) NULL,
	[VendorInvoiceNumber] [varchar](100) NULL,
	[DateReceived] [date] NOT NULL,
	[Comments] [varchar](max) NULL,
	[ReceivedBy] [varchar](100) NULL,
	[Location] [varchar](100) NULL,
	[OriginalPrint] [datetime] NULL,
	[DateApproved] [datetime] NULL,
	[ApprovedById] [int] NULL,
	[ApprovedBy] [varchar](100) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedById] [int] NULL,
	[CreatedBy] [varchar](100) NULL,
	[CostConfirmed] [datetime] NULL,
	[CostConfirmedById] [int] NULL,
	[CostConfirmedBy] [varchar](100) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO