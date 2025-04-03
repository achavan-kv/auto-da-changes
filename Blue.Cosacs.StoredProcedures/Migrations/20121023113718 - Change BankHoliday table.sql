-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
CREATE TABLE Config.Tmp_PublicHoliday
	(
	Date datetime NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE Config.Tmp_PublicHoliday SET (LOCK_ESCALATION = TABLE)
GO
IF EXISTS(SELECT * FROM Config.BankHoliday)
	 EXEC('INSERT INTO Config.Tmp_PublicHoliday (Date)
		SELECT CONVERT(datetime, Date) FROM Config.BankHoliday WITH (HOLDLOCK TABLOCKX)')
GO
DROP TABLE Config.BankHoliday
GO
EXECUTE sp_rename N'Config.Tmp_PublicHoliday', N'PublicHoliday', 'OBJECT' 
GO
ALTER TABLE Config.PublicHoliday ADD CONSTRAINT
	PK_BankHoliday PRIMARY KEY CLUSTERED 
	(
	Date
	) WITH( PAD_INDEX = OFF, FILLFACTOR = 90, STATISTICS_NORECOMPUTE = ON, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO

