-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

ALTER TABLE [dbo].[WarrantyReturnCodesAudit] DROP CONSTRAINT [pk_WarrantyReturnCodesAudit]

go

ALTER TABLE [dbo].[WarrantyReturnCodesAudit] ADD  CONSTRAINT [pk_WarrantyReturnCodesAudit] PRIMARY KEY CLUSTERED 
(
	[ProductType] ASC,
	[MonthSinceDelivery] ASC,
	[WarrantyMonths] ASC,
	[DateChange] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
