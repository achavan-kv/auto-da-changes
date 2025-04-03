CREATE SCHEMA Scheduling
GO

CREATE TABLE Scheduling.Schedule
(
	Id int IDENTITY(1,1) NOT NULL,
	Name varchar(100) UNIQUE NOT NULL,
	OnceAt TIME,
	DailyAt TIME,
	RepeatInterval int,
	MessageType varchar(100) NOT NULL,
	MessageBody text NOT NULL,
	DeleteAfterRun bit NOT NULL DEFAULT 0,
	Status varchar(20) NOT NULL,
	CreatedAt datetime,
	LastRunAt datetime
)

ALTER TABLE Scheduling.Schedule
ADD CONSTRAINT [PK_Scheduling_Schedule] PRIMARY KEY (Id)
