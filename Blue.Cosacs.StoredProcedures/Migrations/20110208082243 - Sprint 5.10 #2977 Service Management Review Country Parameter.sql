-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS (SELECT * FROM dbo.CountryMaintenance WHERE CodeName = 'MonthsSinceSRResolved')
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
	   'Months Since Service Request Resolved',
	   '0',
	   'numeric',
	   '0',
	   '',
	   '',
	   'A Service Request must exceed the number of months set for this parameter in addition to the Service Request having an outstanding balance in order for a right-click option to appear in the Service Management Review screen to write off the service cash account for Service Requests awaiting payments',
	   'MonthsSinceSRResolved' 
FROM dbo.Country