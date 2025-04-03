-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


IF EXISTS(SELECT * FROM dbo.CountryMaintenance WHERE CodeName = 'StoreCardMaxDaysEODRun')
BEGIN
	UPDATE dbo.CountryMaintenance
	SET Description = 'Store Card Statements End Of Day routine will run automatically if the number of days since the last run is at least the value set in this parameter'
	WHERE CodeName = 'StoreCardMaxDaysEODRun'
END