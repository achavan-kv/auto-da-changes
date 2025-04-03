-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

ALTER TABLE [Merchandising].[GoodsReceipt]
ALTER COLUMN DateApproved DateTime NULL

ALTER TABLE [Merchandising].[GoodsReceiptDirect]
ALTER COLUMN DateApproved DateTime NULL