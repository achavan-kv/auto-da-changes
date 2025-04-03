-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS (SELECT * FROM dbo.CountryMaintenance WHERE CodeName = 'DaysSinceSRLastUpdated')
INSERT INTO CountryMaintenance(CountryCode,
							   ParameterCategory,
							   NAME,
							   Value,
							   [Type],
							   [PRECISION],
							   OptionCategory,
							   OptionListName,
							   [Description],
							   CodeName)

SELECT CountryCode,
	   '28',
	   'Days Since Service Request Last Updated',
	   '0',
	   'numeric',
	   '0',
	   '',
	   '',
	   'A Service Request must exceed the number of days set for this parameter in addition to the Service Request having a status of (New or To Be Allocated) where a deposit has been paid against the Customers Charge To Account for a right-click option to appear in the Service Management Review screen to cancel service accounts for unallocated Service Requests',
	   'DaysSinceSRLastUpdated' 
FROM dbo.Country
