-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
-- Related to issue: #12232

IF NOT EXISTS(select * from CountryMaintenance where CodeName = 'DisplayExpressDelivery')
BEGIN
	insert into CountryMaintenance
	select country.countrycode, 13, 'Display Express Delivery', 'False', 'checkbox', 0, '', '', 'If true, Express Delivery checkbox will be displayed in the New Sales Order Screen', 'DisplayExpressDelivery'
	from country
END

