-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
-- Related to issue: #12734

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE  Table_Name = 'SR_Summary' AND  Column_Name = 'ReplacementIssued')
BEGIN
	ALTER TABLE SR_Summary ADD ReplacementIssued BIT NULL
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE  Table_Name = 'SR_Summary' AND  Column_Name = 'ReplacementActioned')
BEGIN
	ALTER TABLE SR_Summary ADD ReplacementActioned BIT NULL
END

