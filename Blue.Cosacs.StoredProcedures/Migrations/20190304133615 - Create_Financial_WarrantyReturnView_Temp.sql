
IF OBJECT_ID('Financial.WarrantyReturnView_Temp', 'U') IS NOT NULL
	DROP TABLE Financial.WarrantyReturnView_Temp 
Go

CREATE TABLE [Financial].[WarrantyReturnView_Temp](
	[Id] [int] NOT NULL,
	[ContractNumber] [varchar](20) NOT NULL,
	[DeliveredOn] [date] NOT NULL,
	[AccountType] [char](3) NOT NULL,
	[Department] [varchar](3) NOT NULL,
	[SalePrice] [decimal](12, 4) NOT NULL,
	[CostPrice] [decimal](12, 4) NOT NULL,
	[BranchNo] [smallint] NOT NULL,
	[WarrantyNo] [varchar](20) NOT NULL,
	[WarrantyLength] [smallint] NOT NULL,
	[MessageId] [int] NOT NULL,
	[ElapsedMonths] [int] NOT NULL,
	[PercentageReturn] [decimal](5, 2) NULL,
	[Level] [int] NOT NULL,
	[TaxRate] [numeric](4, 2) NULL,
	[FreeWarrantyLength] [smallint] NOT NULL
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

CREATE NONCLUSTERED INDEX [NonClusteredIndex-20180910-124356] ON [Financial].[WarrantyReturnView_Temp]
(
	[ContractNumber] ASC,
	[Id] ASC,
	[Level] ASC,
	[DeliveredOn] ASC,
	[ElapsedMonths] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


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

SET ANSI_PADDING OFF
GO


