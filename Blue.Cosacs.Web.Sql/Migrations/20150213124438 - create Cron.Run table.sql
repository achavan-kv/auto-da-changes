-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

CREATE TABLE Cron.Run
    (
        Id int NOT NULL IDENTITY (1, 1),
        JobId int NOT NULL,
        StartRun smalldatetime NULL,
        EndRun smalldatetime NULL,
        LastException varchar(max) NULL
    )  ON [PRIMARY]
	
GO
ALTER TABLE Cron.Run ADD CONSTRAINT
	PK_Run_1 PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO

ALTER TABLE Cron.Run ADD CONSTRAINT
	FK_Run_Job FOREIGN KEY
	(
	JobId
	) REFERENCES Cron.Job
	(
	Id
	) ON UPDATE CASCADE 
	 ON DELETE  CASCADE 