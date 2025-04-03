-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'Agreement' AND  Column_Name = 'TaxFree')
BEGIN
	ALTER TABLE Agreement ADD TaxFree bit null 
END
GO