-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS(select * from countrymaintenance where codename = 'RepoDelUnitPrice')
BEGIN
	INSERT INTO CountryMaintenance(CountryCode, ParameterCategory, Name, Value, [Type], [Precision], OptionCategory, OptionListName, [Description], CodeName)
	SELECT c.countrycode, 08, 'Repo Redelivery unit price', 'True', 'checkbox', 0, '', '','If true, the Repo unit price will be used otherwise zero price will be used when scheduling for delivery', 'RepoDelUnitPrice'
	FROM Country c
END

