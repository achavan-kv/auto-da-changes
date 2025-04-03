-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = 'IX_Warranty_WarrantySale_WarrantyContractNo' AND object_id = OBJECT_ID('[Warranty].[WarrantySale]'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Warranty_WarrantySale_WarrantyContractNo] ON [Warranty].[WarrantySale]
    (
	    [WarrantyContractNo] 
    )
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
END
GO

