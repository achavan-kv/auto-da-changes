-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF EXISTS (select * from sys.indexes where name ='idx_WarrantySale_WarrantyContractNo')
	DROP INDEX [idx_WarrantySale_WarrantyContractNo] ON [Warranty].[WarrantySale]
GO
CREATE NONCLUSTERED INDEX idx_WarrantySale_WarrantyContractNo
ON [Warranty].[WarrantySale] ([WarrantyContractNo])
GO