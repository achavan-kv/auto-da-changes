-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
IF NOT EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE CONSTRAINT_NAME='PK_LineitemAudit')
	ALTER TABLE dbo.LineitemAudit ADD CONSTRAINT PK_LineitemAudit PRIMARY KEY NONCLUSTERED 
		(
			LineItemAuditID
		) 
		WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

UPDATE STATISTICS dbo.LineitemAudit
