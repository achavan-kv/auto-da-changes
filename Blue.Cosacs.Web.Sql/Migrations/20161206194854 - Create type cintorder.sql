CREATE TYPE [Merchandising].[CintOrderTVP] AS TABLE(
	[RunNo] [int] NOT NULL,
	[Type] [varchar](50) NOT NULL,
	[PrimaryReference] [varchar](50) NOT NULL,
	[SaleType] [varchar](50) NOT NULL,
	[SaleLocation] [varchar](50) NOT NULL,
	[Sku] [varchar](10) NOT NULL,
	[ProductId] [int] NOT NULL,
	[StockLocation] [varchar](50) NOT NULL,
	[ParentSku] [varchar](10) NULL,
	[ParentId] [int] NULL,
	[TransactionDate] [datetime] NOT NULL,
	[Quantity] [int] NOT NULL,
	[Price] [decimal] NULL,
	[Tax] [decimal] NULL,
	[SecondaryReference] [varchar](20) NULL,
	[ReferenceType] [varchar](20) NULL,
	[Discount] [decimal] NULL,
	[CashPrice] [decimal] NULL,
	[PromotionId] [int] NULL,
	[CostPrice] [decimal] NULL,
	[TempId] [int] NOT NULL
)
GO
