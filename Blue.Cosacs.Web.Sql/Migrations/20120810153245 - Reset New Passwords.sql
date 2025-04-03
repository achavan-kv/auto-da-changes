-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
UPDATE Admin.[User] 
SET Password = NULL, LegacyPassword = 2398
WHERE Password IS NOT NULL