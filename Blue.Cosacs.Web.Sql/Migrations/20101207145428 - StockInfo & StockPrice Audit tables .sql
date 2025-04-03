-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
--  Audit table for Stockinfo

CREATE TABLE [dbo].[StockInfoAudit](
	[Itemno] [varchar](8) NOT NULL,
	[Itemdescr1] [varchar](25) NOT NULL,
	[Itemdescr2] [varchar](40) NOT NULL,
	[Category] [smallint] NULL,
	[Supplier] [varchar](40) NULL,
	[ProdStatus] [varchar](1) NULL,
	[SupplierCode] [varchar](18) NULL,
	[Warrantable] [smallint] NULL,
	[Itemtype] [varchar](1) NOT NULL,
	[WarrantyRenewalFlag] [char](1) NOT NULL,
	[Leadtime] [smallint] NOT NULL,
	[AssemblyRequired] [char](1) NULL,
	[Taxrate] [float] NOT NULL,
	[DateChange] [datetime] not null,
 CONSTRAINT [PK_StockInfoAudit] PRIMARY KEY CLUSTERED 
(
	[itemno] ASC,
	[DateChange] asc
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

--  Audit table for StockPrice

CREATE TABLE [dbo].[StockPriceAudit](
	[Itemno] [varchar](8) NOT NULL,
	[BranchNo] [smallint] NOT NULL,
	[CreditPrice] [money] NULL,
	[CashPrice] [money] NULL,
	[DutyFreePrice] [money] NULL,
	[CostPrice] [money] NULL,
	[Refcode] [varchar](3) NULL,
	[DateChange] [datetime] not null,
 CONSTRAINT [PK_StockPriceAudit] PRIMARY KEY CLUSTERED 
(
	[itemno] ASC,
	[branchno] ASC,
	[DateChange] asc
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
)ON [PRIMARY]


	