-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[WTRReport]') AND type in (N'U'))
DROP TABLE [dbo].[WTRReport]
GO

CREATE TABLE [dbo].[WTRReport](
	[SortOrder] varchar(1) NOT NULL,
	[Category] varchar(25) NULL,
	[Product Category] varchar(64) NOT NULL,
	DepartmentCode INT null,
	[Branchno] int NULL,
	[BranchName] varchar(20) NULL,
	[SalesType] varchar(15) NULL,
	[ActualValue] float NULL,
	[ActualPct] float NULL,
	[ActualValueLY] float NOT NULL,
	[Variance] float NULL,
	[ActualGP] float NULL,
	[ActualGPPct] float NULL,
	[ActualGPPctLY] float NOT NULL,
	[YTDSales] money NULL,
	[YTDSalesPct] float NULL,
	[YTDSalesLY] money NOT NULL,
	[YTDVariance] money NULL,
	[YTDGP] money NULL,
	[YTDGPPct] float NULL,
	[YTDGPLY] money NOT NULL,
	[YTDGPPctLY] float NOT NULL
) ON [PRIMARY]

GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TradingSummary]') AND type in (N'U'))
DROP TABLE [dbo].[TradingSummary]
GO

CREATE TABLE [dbo].[TradingSummary](
	[SortOrder] varchar(1) NOT NULL,
	[WeekNo] int NULL,
	[WeekEndDate] datetime NULL,
	[Category] varchar(25) NULL,
	[Product Category] varchar(64) NOT NULL,
	DepartmentCode INT null,
	[Branchno] int NULL,
	[SalesType] varchar(15) NULL,
	[ActualValue] float NULL,
	[ActualPct] float NULL,
	[ActualGP] float NULL,
	[ActualGPPct] float NULL,
	[YTDSales] money NULL,
	[YTDSalesPct] float NULL,
	[YTDGP] money NULL,
	[YTDGPPct] float NULL
	) ON [PRIMARY]

-- TODO add category/Department code

-- Create table to hold column headings for Spreadsheet
	IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TradingSummaryHdr]') AND type in (N'U'))
		DROP TABLE [dbo].[TradingSummaryHdr]
	go
	
	select 'SortOrder' as SortOrder,'ProductGroup' as Category,
	'ProductCategory' as ProductCategory,'Department' as Department,'BranchNo' as Branchno,'BranchName' as BranchName,'SalesType' as SalesType,
	'ActualValue' as ActualValue,'ActualPct' as ActualPct,'ActualValueLY' as ActualValueLY,'Variance' as Variance,'ActualGP' as ActualGP,
	'ActualGPPct' as ActualGPPct,'ActualGPPctLY' as ActualGPPctLY,
	'YTDSales' as YTDSales,'YTDSalesPct' as YTDSalesPct,'YTDSalesLY' as YTDSalesLY,'YTDVariance' as YTDVariance,
	'YTDGP' as YTDGP,'YTDGPPct' as YTDGPPct,'YTDGPLY' as YTDGPLY,'YTDGPPctLY' as YTDGPPctLY
	into  TradingSummaryHdr


