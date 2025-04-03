-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.columns WHERE column_name = 'notestring' AND table_name ='bailaction')
ALTER TABLE bailaction DROP COLUMN notestring 