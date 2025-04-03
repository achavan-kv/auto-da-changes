-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
IF NOT EXISTS (SELECT * FROM dbo.CountryMaintenance WHERE CodeName = 'SCardInterestFreeDays')
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
	   '33',
	   'Store Card Interest Free Days',
	   '1',
	   'numeric',
	   '0',
	   '',
	   '',
	   'This is the Interest Free Period for Store Card Accounts. Statements will produced this number of days prior to the Payment Due Date',
	   'SCardInterestFreeDays' 
FROM dbo.Country
