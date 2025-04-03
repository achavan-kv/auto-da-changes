-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 

IF (EXISTS (SELECT 1 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'Scheduling'))
BEGIN
		DROP TABLE Scheduling.Job
		DROP TABLE Scheduling.Schedule

		IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.VIEWS 
                 WHERE TABLE_SCHEMA = 'Scheduling' AND TABLE_NAME='HubQueue'))
		BEGIN
			DROP VIEW Scheduling.HubQueue
		END
		
		DROP SCHEMA Scheduling
END

