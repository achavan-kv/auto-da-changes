-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

UPDATE CountryMaintenance 
	SET Name = 'Min Non Alphanumeric characters',
	[Description] = 'Minimum number of non alphanumeric characters. Example: &^%$£!._'
WHERE
	Name = 'Min Non Alfanumeric charaters'

UPDATE CountryMaintenance 
	SET [Description] = 'Minimum password length'
WHERE
	[Description] = 'Minimun password length'
