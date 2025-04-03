-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
-- Related to issue: #10970



IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE  Table_Name = 'Charge' AND  Column_Name = 'Labour'
           AND TABLE_SCHEMA = 'Service')
BEGIN
	ALTER TABLE Service.Charge ADD Labour Money NULL
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE  Table_Name = 'Charge' AND  Column_Name = 'PartsOther'
           AND TABLE_SCHEMA = 'Service')
BEGIN
	ALTER TABLE Service.Charge ADD PartsOther Money NULL
END
