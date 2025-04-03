-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [Warranty].[WarrantyPotentialSale](
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[InvoiceNumber] [varchar](50) NULL,
	[SaleBranch] [smallint] NOT NULL,
	[SoldOn] [date] NULL,
	[SoldBy] [varchar](50) NULL,
	[SoldById] [int] NULL,
	--[CustomerAccount] [char](12) NULL,
	[CustomerId] [varchar](50) NULL,
	--[CustomerTitle] [varchar](25) NULL,
	--[CustomerFirstName] [varchar](50) NULL,
	--[CustomerLastName] [varchar](60) NULL,
	--[CustomerAddressLine1] [varchar](50) NULL,
	--[CustomerAddressLine2] [varchar](50) NULL,
	--[CustomerAddressLine3] [varchar](50) NULL,
	--[CustomerPostcode] [varchar](10) NULL,
	[ItemId] [int] NULL,
	[ItemNumber] [varchar](25) NULL,
	[ItemUPC] [varchar](25) NULL,
	[ItemPrice] [money] NULL,
	--[ItemDescription] [varchar](100) NULL,
	--[ItemBrand] [varchar](50) NULL,
	--[ItemModel] [varchar](50) NULL,
	[ItemSupplier] [varchar](50) NULL,
	--[WarrantyContractNo] [varchar](20) NULL,
	[WarrantyId] [int] NULL,
	[WarrantyNumber] [varchar](20) NULL,
	[WarrantyLength] [smallint] NULL,
	[WarrantyTaxRate] [decimal](4, 2) NOT NULL,
	[WarrantyIsFree] [bit] NULL,
	[WarrantyCostPrice] [money] NULL,
	[WarrantyRetailPrice] [money] NULL,
	[WarrantySalePrice] [money] NULL,
	[Status] [varchar](20) NULL,
	[ItemSerialNumber] [varchar](50) NULL,
	[StockLocation] [smallint] NULL,
	--[CustomerNotes] [varchar](4000) NULL,
	[ItemCostPrice] [money] NULL,
	[ItemDeliveredOn] [date] NULL,
	[LineItemIdentifier] [int] NULL,
	[isItemReturned] [bit] NOT NULL DEFAULT ((0))
)

GO

SET ANSI_PADDING OFF
GO
