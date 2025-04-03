-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
-- Related to issue: #11989

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE  Table_Name = 'SR_Summary' AND  Column_Name = 'BookingId')
BEGIN
	ALTER TABLE SR_Summary ADD BookingId INT NULL
END
