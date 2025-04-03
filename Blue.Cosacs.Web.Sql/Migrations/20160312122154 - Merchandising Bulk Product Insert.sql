
CREATE TABLE [Merchandising].[ProductStaging](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SKU] [varchar](20) NOT NULL,
	[LongDescription] [varchar](240) NOT NULL,
	[ProductType] [varchar](100) NOT NULL,
	[Tags] [varchar](max) NULL,
	[StoreTypes] [varchar](max) NULL,
	[POSDescription] [varchar](240) NOT NULL,
	[Attributes] [varchar](max) NULL,
	[CreatedDate] [datetime] NOT NULL DEFAULT (getdate()),
	[LastUpdatedDate] [datetime] NOT NULL DEFAULT (getdate()),
	[Status] [int] NOT NULL DEFAULT ((1)),
	[PriceTicket] [bit] NOT NULL,
	[SKUStatus] [char](1) NULL,
	[CorporateUPC] [varchar](20) NULL,
	[VendorUPC] [varchar](60) NULL,
	[VendorStyleLong] [varchar](50) NULL,
	[CountryOfOrigin] [varchar](2) NULL,
	[VendorWarranty] [int] NULL,
	[ReplacingTo] [varchar](20) NULL,
	[Features] [varchar](max) NULL,
	[BrandId] [int] NULL,
	[PrimaryVendorId] [int] NULL,
	[LastStatusChangeDate] [datetime] NOT NULL DEFAULT (getutcdate()),
	[OnlineDateAdded] [datetime] NULL,
	[LabelRequired] [bit] NOT NULL DEFAULT ((0)),
	[BoxCount] [int] NOT NULL DEFAULT ((1)),
	[ProductAction] [char](1) NULL,
	[CreatedById] [int] NULL,
	[ExternalCreationDate] [datetime] NULL,
	[MagentoExportType] [varchar](20) NULL,
	BrandCode Varchar(6) NULL,
	BrandName VARCHAR(25) NULL
 CONSTRAINT [PK_ProductStaging] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
))