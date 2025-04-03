-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
CREATE TABLE Cron.Schedule
	(
	Id int NOT NULL IDENTITY (1, 1),
	EndpointId smallint NOT NULL,
	DailyAt varchar(5) NULL,
	EveryFewMinutes int NULL,
	CronExpression varchar(500) NULL
	)  ON [PRIMARY]
GO
ALTER TABLE Cron.Schedule ADD CONSTRAINT
	PK_Schedule_1 PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO