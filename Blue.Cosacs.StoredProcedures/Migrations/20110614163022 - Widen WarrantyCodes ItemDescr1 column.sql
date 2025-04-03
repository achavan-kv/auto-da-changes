-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
ALTER TABLE WarrantyCodes
ALTER COLUMN itemdescr1 VARCHAR(32) NOT NULL
