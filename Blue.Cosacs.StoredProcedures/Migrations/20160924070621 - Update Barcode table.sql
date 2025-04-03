-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

ALTER TABLE [dbo].[BarCodeItem] DROP CONSTRAINT [PK_BarCodeItem]
GO

ALTER TABLE barcodeitem
ALTER COLUMN itemno VARCHAR(18) NOT NULL
GO

ALTER TABLE [dbo].[BarCodeItem] ADD  CONSTRAINT [PK_BarCodeItem] PRIMARY KEY CLUSTERED 
(
	[itemno] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

     