-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
-- Related to issue: #12116

IF NOT EXISTS(select * from CountryMaintenance where codename = 'InstallationStockAccount')
BEGIN

	INSERT INTO CountryMaintenance (CountryCode, ParameterCategory, Name, Value, Type, Precision, OptionCategory, OptionListName, Description, CodeName)
	select countrycode, '28', 'Installation Stock Account', '', 'text', 0, '', '', 'The special account number for the installation parts inventory', 'InstallationStockAccount'
	from Country
END


