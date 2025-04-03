-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
-- Related to issue: #14392

IF EXISTS(select * from syscolumns where name = 'datechange' and object_name(id) = 'Agreement')
BEGIN
	ALTER TABLE Agreement ALTER COLUMN datechange datetime
END	
GO