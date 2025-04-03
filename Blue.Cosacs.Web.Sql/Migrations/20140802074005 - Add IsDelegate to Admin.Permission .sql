-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


-- IsDelegate field is NULL on v9 and v10. On master there is another migration that makes the field NOT NULL
IF EXISTS(
SELECT TOP 1 *
FROM INFORMATION_SCHEMA.COLUMNS
WHERE [TABLE_NAME] = 'Permission'
AND [COLUMN_NAME] = 'IsDelegate'
AND [TABLE_SCHEMA] = 'Admin')

BEGIN

ALTER TABLE Admin.Permission
ALTER COLUMN IsDelegate Bit NULL

END

GO
IF NOT EXISTS(
SELECT TOP 1 *
FROM INFORMATION_SCHEMA.COLUMNS
WHERE [TABLE_NAME] = 'Permission'
AND [COLUMN_NAME] = 'IsDelegate'
AND [TABLE_SCHEMA] = 'Admin')

BEGIN

ALTER TABLE Admin.Permission
	ADD IsDelegate Bit NULL
END


GO
UPDATE Admin.Permission
Set IsDelegate = 0
WHERE IsDelegate IS NULL