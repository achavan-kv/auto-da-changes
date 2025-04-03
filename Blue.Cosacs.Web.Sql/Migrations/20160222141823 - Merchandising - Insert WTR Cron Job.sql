-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS (SELECT 1 FROM Cron.Endpoint WHERE Id = 210)
BEGIN
	INSERT INTO Cron.[Endpoint]
	VALUES (210, 'Merchandising WTR Report', '/cosacs/Merchandising/WeeklyTradingReport/CronExport', 'Merchandising')
END

IF NOT EXISTS (SELECT 1 FROM Cron.Job WHERE EndpointId = 210)
BEGIN
	INSERT INTO cron.Job
	VALUES (210, '1970-01-01 06:00:00', NULL, NULL, 0)
END