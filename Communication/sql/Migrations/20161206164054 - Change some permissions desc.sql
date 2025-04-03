-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
UPDATE Admin.Permission
SET Description = 'Background job to send e-mails'
WHERE Id = 2801

UPDATE Admin.Permission
SET Description = 'Background job to send SMS'
WHERE Id = 2804

UPDATE Admin.Permission
SET Description = 'Email Templates'
WHERE Id = 2802