-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'IntRateHistory' AND  Column_Name = 'AdminValue')
BEGIN
	ALTER TABLE IntRateHistory Add AdminValue float not null default 0
END
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'IntRateHistoryDeletedTemp' AND  Column_Name = 'AdminValue')
BEGIN
	ALTER TABLE IntRateHistoryDeletedTemp Add AdminValue float not null default 0
END
GO
