-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


IF NOT EXISTS (SELECT * FROM dbo.CountryMaintenance WHERE CodeName = 'HPQualInstantCredit')
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
	   '27',
	   'HP Accounts can Qualify for Instant Credit',
	   'False',
	   'checkbox',
	   '0',
	   '',
	   '',
	   'Determines Whether HP Accounts can Qualify for Instant Credit',
	   'HPQualInstantCredit' 
FROM dbo.Country
