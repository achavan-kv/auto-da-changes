IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'PurchaseOrderHistory'
               AND TABLE_SCHEMA = 'Merchandising')
BEGIN
CREATE TABLE [Merchandising].[PurchaseOrderHistory](
	[Id] [int]  NULL,
	[VendorId] [int]  NULL,
	[Vendor] [varchar](100)  NULL,
	[RequestedDeliveryDate] [date] NOT NULL,
	[ReceivingLocationId] [int] NOT NULL,
	[ReceivingLocation] [varchar](100) NOT NULL,
	[ReferenceNumbers] [varchar](max) NULL,
	[Currency] [varchar](3) NULL,
	[Comments] [varchar](max) NULL,
	[Status] [varchar](20) NULL,
	[OriginalPrint] [datetime] NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedById] [int] NULL,
	[CreatedBy] [varchar](max) NULL,
	[PaymentTerms] [varchar](60) NULL,
	[OriginSystem] [varchar](10) NULL,
	[CorporatePoNumber] [varchar](20) NULL,
	[ShipDate] [date] NULL,
	[ShipVia] [varchar](60) NULL,
	[PortOfLoading] [varchar](60) NULL,
	[Attributes] [varchar](max) NULL,
	[CommissionChargeFlag] [char](1) NULL,
	[CommissionPercentage] [varchar](10) NULL,
	[ExpiredDate] [date] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

END
GO

