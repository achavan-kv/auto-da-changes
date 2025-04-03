-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

ALTER TABLE Cron.Schedule
	DROP CONSTRAINT FK_Schedule_Endpoint
GO
CREATE TABLE Cron.Tmp_Job
	(
	Id int NOT NULL IDENTITY (1, 1),
	EndpointId smallint NOT NULL,
	DailyAt smalldatetime NULL,
	EveryFewMinutes int NULL,
	CronExpression varchar(500) NULL
	)  ON [PRIMARY]
GO
ALTER TABLE Cron.Tmp_Job SET (LOCK_ESCALATION = TABLE)
GO
SET IDENTITY_INSERT Cron.Tmp_Job ON
GO
IF EXISTS(SELECT * FROM Cron.Schedule)
	 EXEC('INSERT INTO Cron.Tmp_Job (Id, EndpointId, DailyAt, EveryFewMinutes, CronExpression)
		SELECT Id, EndpointId, CONVERT(smalldatetime, DailyAt), EveryFewMinutes, CronExpression FROM Cron.Schedule WITH (HOLDLOCK TABLOCKX)')
GO
SET IDENTITY_INSERT Cron.Tmp_Job OFF
GO
DROP TABLE Cron.Schedule
GO
EXECUTE sp_rename N'Cron.Tmp_Job', N'Job', 'OBJECT' 
GO
ALTER TABLE Cron.Job ADD CONSTRAINT
	PK_Cron_Job PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE Cron.Job ADD CONSTRAINT
	FK_Schedule_Endpoint FOREIGN KEY
	(
	EndpointId
	) REFERENCES Cron.Endpoint
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
