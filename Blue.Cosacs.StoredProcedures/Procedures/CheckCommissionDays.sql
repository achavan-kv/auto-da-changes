--Actions will only be considered for commission 
--after this number of days have passed. A warning will popup in the end of day screen if the time period since the last commission run is one day less than this period
IF EXISTS (SELECT * FROM sysobjects WHERE name = 'CheckCommissionDays')
DROP PROCEDURE CheckCommissionDays 
GO 
create PROCEDURE CheckCommissionDays 
@NumdaysSinceLastRun int OUT,
@return int
AS -- procedure checks to see whether commission has been run previously and if so raises Error.

DECLARE @mindaysbetween INT ,@datelast DATETIME 
SELECT @mindaysbetween =Value  FROM dbo.CountryMaintenance WHERE name = 'Max Days for Caller Commissions'
SET @return = 0

SELECT @NumdaysSinceLastRun =  DATEDIFF(DAY,datestart,GETDATE())
FROM interfacecontrol WHERE interface = 'COLLCOMMNS' AND result = 'P'
ORDER BY datestart DESC

IF @NumdaysSinceLastRun IS NOT NULL 
BEGIN 
	IF @NumdaysSinceLastRun > @mindaysbetween -1 OR @mindaysbetween = 0
	  SET @NumdaysSinceLastRun = 1000
END 
 SET @return = 1
GO 

--SELECT * FROM dbo.CountryMaintenance WHERE name LIKE 'Max Days for Caller Commissions'