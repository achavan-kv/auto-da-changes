-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS(SELECT 1 FROM [$Migration] WHERE Id = 20150203113128)
BEGIN 
	IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE name = 'IX_custaddress_addtype_datemoved' AND object_id = OBJECT_ID('custaddress'))
	BEGIN
		CREATE NONCLUSTERED INDEX IX_custaddress_addtype_datemoved ON dbo.custaddress 
		(
			addtype, datemoved
		)
		INCLUDE 
		(
			custid, zone
		)
		UPDATE STATISTICS custaddress

		CREATE NONCLUSTERED INDEX IX_CMBailiffAllocationRules ON dbo.CMBailiffAllocationRules
		(
			BranchorZone
		) WITH
		( 
			STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON
		) 
		UPDATE STATISTICS CMBailiffAllocationRules

		CREATE NONCLUSTERED INDEX IX_follupalloc_datedealloc ON dbo.follupalloc (datedealloc)
		UPDATE STATISTICS follupalloc
	END
END