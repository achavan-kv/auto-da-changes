-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


IF OBJECT_ID('Merchandising.WTRWarrantyReturnValuesView_Temp', 'U') IS NOT NULL
	DROP TABLE Merchandising.WTRWarrantyReturnValuesView_Temp 
Go


CREATE TABLE [Merchandising].[WTRWarrantyReturnValuesView_Temp](
	[WarrantyNumber] [varchar](20) NOT NULL,
	[WarrantyContractNumber] [varchar](10) NOT NULL,
	[Department] [varchar](3) NOT NULL,
	[BranchNo] [smallint] NOT NULL,
	[OriginalSalePrice] [decimal](12, 4) NOT NULL,
	[OriginalCostPrice] [decimal](12, 4) NOT NULL,
	[PercentageReturn] [decimal](5, 2) NULL,
	[SalePrice] [decimal](25, 11) NULL,
	[CostPrice] [decimal](12, 4) NOT NULL,
	[ReturnValue] [decimal](28, 13) NULL,
	[ReturnCost] [decimal](23, 10) NULL,
	[ReturnDate] [date] NULL,
	[SaleType] [varchar](6) NOT NULL
) ON [PRIMARY]

GO