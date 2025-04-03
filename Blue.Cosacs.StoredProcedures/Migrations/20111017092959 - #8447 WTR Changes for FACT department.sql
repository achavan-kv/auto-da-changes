-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

/****** Object:  Table [dbo].[WTRReport]    Script Date: 10/17/2011 09:46:32 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[WTRReport]') AND type in (N'U'))
DROP TABLE [dbo].[WTRReport]
GO



/****** Object:  Table [dbo].[WTRReport]    Script Date: 10/17/2011 09:46:33 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[WTRReport](
	[SortOrder] [varchar](1) NOT NULL,
	[Category] [varchar](25) NOT NULL,
	[Product Category] [varchar](64) NOT NULL,
	[DepartmentCode] [int] NOT NULL,
	[Class] [varchar](3) NOT NULL,
	[ClassDescr] [varchar](64) NULL,
	[Branchno] [int] NOT NULL,
	[BranchName] [varchar](20) NULL,
	[SalesType] [varchar](15) NOT NULL,
	[ActualValue] [float] NULL,
	[ActualValueLY] [float] NOT NULL,
	[Variance] [float] NULL,
	[ActualGP] [float] NULL,
	[ActualGPLY] [float] NOT NULL,
	[VarianceGP] [float] NOT NULL,
	[YTDSales] [money] NULL,
	[YTDSalesLY] [money] NOT NULL,
	[YTDVariance] [money] NULL,
	[YTDGP] [money] NULL,
	[YTDGPLY] [money] NOT NULL,
	[YTDGPVariance] [Money] NOT NULL
 CONSTRAINT [pk_WTRReport] PRIMARY KEY CLUSTERED 
(
	[Branchno] ASC,
	[SortOrder] ASC,
	[SalesType] ASC,
	[Category] ASC,
	[DepartmentCode] ASC,
	[Class] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO




/****** Object:  Table [dbo].[TradingSummaryHdr]    Script Date: 10/17/2011 10:01:12 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TradingSummaryHdr]') AND type in (N'U'))
DROP TABLE [dbo].[TradingSummaryHdr]
GO


/****** Object:  Table [dbo].[TradingSummaryHdr]    Script Date: 10/17/2011 10:01:12 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[TradingSummaryHdr](
	[SortOrder] [varchar](9) NOT NULL,
	[Category] [varchar](12) NOT NULL,
	[ProductCategory] [varchar](15) NOT NULL,
	[Department] [varchar](10) NOT NULL,
	[Class] [varchar](5) NOT NULL,
	[ClassDescription] [varchar](16) NOT NULL,
	[Branchno] [varchar](8) NOT NULL,
	[BranchName] [varchar](10) NOT NULL,
	[SalesType] [varchar](9) NOT NULL,
	[ActualValue] [varchar](11) NOT NULL,
	[ActualValueLY] [varchar](13) NOT NULL,
	[Variance] [varchar](8) NOT NULL,
	[ActualGP] [varchar](8) NOT NULL,
	[ActualGPLY] [varchar](13) NOT NULL,
	[VarianceGP] [varchar](13) NOT NULL,
	[YTDSales] [varchar](8) NOT NULL,
	[YTDSalesLY] [varchar](10) NOT NULL,
	[YTDVariance] [varchar](11) NOT NULL,
	[YTDGP] [varchar](5) NOT NULL,
	[YTDGPLY] [varchar](7) NOT NULL,
	[YTDGPVariance] [varchar](20) NULL
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


insert into [TradingSummaryHdr]
(
	SortOrder,Category,ProductCategory,Department,Class,ClassDescription,Branchno,BranchName,SalesType,ActualValue,ActualValueLY,Variance,ActualGP,
	ActualGPLY,VarianceGP,YTDSales,YTDSalesLY,YTDVariance,YTDGP,YTDGPLY,YTDGPVariance
)
values
(
	'SortOrder','Category','ProductCategory','Department','Class','ClassDescription','Branchno','BranchName','SalesType','ActualValue','ActualValueLY',
	'Variance','ActualGP','ActualGPLY','VarianceGP','YTDSales','YTDSalesLY','YTDVariance','YTDGP','YTDGPLY','YTDGPVariance'
)
