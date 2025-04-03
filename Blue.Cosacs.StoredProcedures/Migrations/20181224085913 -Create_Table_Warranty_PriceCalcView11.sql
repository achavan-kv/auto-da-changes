-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
--Create Table [Warranty].[PriceCalcView11]
/****** Object:  Table [Warranty].[PriceCalcView11]    Script Date: 1/10/2019 7:33:18 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO
IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Warranty].[PriceCalcView11]') AND type in (N'U'))
CREATE TABLE [Warranty].[PriceCalcView11]
(
	[Id] [int] NULL,
	[WarrantyId] [int] NULL,
	[WarrantyNumber] [varchar](20) NOT NULL,
	[BranchType] [varchar](20) NULL,
	[BranchNumber] [smallint] NULL,
	[CostPrice] [decimal](20, 3) NULL,
	[RetailPrice] [decimal](20, 3) NULL,
	[RetailTaxInclusivePrice] [decimal](20, 3) NULL,
	[EffectiveDate] [date] NULL,
	[CostPriceChange] [numeric](19, 3) NULL,
	[CostPricePercentChange] [numeric](5, 2) NULL,
	[RetailPriceChange] [numeric](19, 3) NULL,
	[RetailPricePercentChange] [numeric](5, 2) NULL,
	[TaxInclusivePriceChange] [numeric](30, 9) NULL,
	[TaxInclusivePricePercentChange] [numeric](19, 3) NULL,
	[BulkEditId] [int] NULL,
	[AgrmtTaxType] [varchar](1500) NULL,
	[TaxRate] [numeric](4, 2) NULL,
	[IsFree] [bit] NULL
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO
