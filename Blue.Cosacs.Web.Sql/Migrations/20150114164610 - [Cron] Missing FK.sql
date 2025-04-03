-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
ALTER TABLE Cron.Schedule ADD CONSTRAINT
	FK_Schedule_Endpoint FOREIGN KEY
	(
	EndpointId
	) REFERENCES Cron.Endpoint
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO