-- Servicer report for Malaysia....
IF EXISTS (SELECT * FROM sysobjects WHERE NAME = 'Service_ReportWriteLine')
DROP PROCEDURE Service_ReportWriteLine
GO
CREATE PROCEDURE Service_ReportWriteLine 
@fieldname VARCHAR(128),
@value FLOAT
AS 
DECLARE @CalcDate DATETIME
SELECT @CalcDate = CalcDate FROM Service_Data
--ALTER TABLE service_report ALTER COLUMN resultid VARCHAR(64)
INSERT INTO Service_Report (CalcDate, ResultId,ResultValue)
VALUES (@CalcDate,@fieldname,@value)
IF @@ERROR !=0
   PRINT 'error doing ' + @fieldname
GO
