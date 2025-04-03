-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
-- Related to issue: #15285

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE  Table_Name = 'User' AND  Column_Name = 'FactEmployeeNo'
           AND TABLE_SCHEMA = 'Admin')
BEGIN
	ALTER TABLE Admin.[User] Alter column FactEmployeeNo Varchar(4) NULL
END