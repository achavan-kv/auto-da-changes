-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


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