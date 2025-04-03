-- Script Comment : Update Setting
-- Script Name : 5230974_Belize_Create_tbl_PriceCalcView11.sql
-- Created For	: Belize
-- Created By	: Nilesh
-- Created On	: 7/31/2018
-- Modified On	Modified By	Comment

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM SYSOBJECTS 
           WHERE NAME = 'PriceCalcView11'
           AND xtype = 'U')
BEGIN 

CREATE TABLE [Warranty].[PriceCalcView11](
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

END
GO


