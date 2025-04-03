CREATE TABLE Scheduling.Job
(
	Id int IDENTITY(1,1) NOT NULL,
	ScheduleId int,
	RunAt datetime
)

ALTER TABLE Scheduling.Job
ADD CONSTRAINT [PK_Scheduling_Job] PRIMARY KEY (Id)

ALTER TABLE Scheduling.Job
ADD CONSTRAINT [FK_Scheduling_Job_ScheduleId]
FOREIGN KEY (ScheduleId) REFERENCES Scheduling.Schedule (Id)
