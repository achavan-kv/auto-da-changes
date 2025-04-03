-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
ALTER TABLE StoreCardAudit ALTER COLUMN oldvalue VARCHAR(500) NULL 
ALTER TABLE StoreCardAudit ALTER COLUMN newvalue VARCHAR(500) NULL 
