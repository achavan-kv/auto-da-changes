-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
-- Related to issue: #11791

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE  Table_Name = 'Request' AND  Column_Name = 'ItemModelNumber'
           AND TABLE_SCHEMA = 'Service')
BEGIN
	ALTER TABLE Service.Request ADD ItemModelNumber VARCHAR(50) NULL
END
