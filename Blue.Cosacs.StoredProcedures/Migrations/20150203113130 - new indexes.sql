-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
IF NOT EXISTS(SELECT 1 FROM sys.indexes i where i.name = 'IX_custaddress_addtype_datemoved')
BEGIN 
	CREATE NONCLUSTERED INDEX IX_custaddress_addtype_datemoved ON dbo.custaddress 
	(
		addtype, datemoved
	)
	INCLUDE 
	(
		custid, zone
	)
END

UPDATE STATISTICS custaddress

IF NOT EXISTS(SELECT 1 FROM sys.indexes i where i.name = 'IX_CMBailiffAllocationRules')
BEGIN 
	CREATE NONCLUSTERED INDEX IX_CMBailiffAllocationRules ON dbo.CMBailiffAllocationRules
	(
		BranchorZone
	) WITH
	( 
		STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON
	) 
END

UPDATE STATISTICS CMBailiffAllocationRules

IF NOT EXISTS(SELECT 1 FROM sys.indexes i where i.name = 'IX_follupalloc_datedealloc')
BEGIN 
	CREATE NONCLUSTERED INDEX IX_follupalloc_datedealloc ON dbo.follupalloc (datedealloc)
END

UPDATE STATISTICS follupalloc