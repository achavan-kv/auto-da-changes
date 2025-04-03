-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF  EXISTS(
SELECT TOP 1 *
FROM INFORMATION_SCHEMA.COLUMNS
WHERE [TABLE_NAME] = 'User'
AND [COLUMN_NAME] = 'IsDelegate'
AND [TABLE_SCHEMA] = 'Admin')

BEGIN
 ALter table Admin.[User]
 Drop column IsDelegate

END