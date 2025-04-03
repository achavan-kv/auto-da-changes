-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS (SELECT * FROM countrymaintenance WHERE codename = 'InstalChgAcct')
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
	   'Installation Charge Account',
	   '',
	   'text',
	   '0',
	   '',
	   '',
	   'The special account number for Installation Charges',
	   'InstalChgAcct' 
FROM dbo.Country
