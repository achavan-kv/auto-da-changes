-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
IF NOT EXISTS (SELECT * FROM dbo.CountryMaintenance WHERE CodeName = 'PasswordMinLength')
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
	   '17',
	   'Minimum password length',
	   '5',
	   'numeric',
	   '0',
	   '',
	   '',
	   'Minimum password length allowed',
	   'PasswordMinLength' 
FROM dbo.Country
