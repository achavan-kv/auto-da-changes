-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

declare @parameterCat int

set @parameterCat = (select ParameterCategory from CountryMaintenance where codename = 'RebateCalculationRule')

IF NOT EXISTS(select * from countrymaintenance where codename = 'BduRebateCalculationRule')
BEGIN
	
	insert into countrymaintenance(CountryCode, ParameterCategory, Name, Value, Type, Precision, OptionCategory, OptionListName, Description, CodeName)
	select c.CountryCode, @parameterCat, 'BDU Rebate Calculation Rule', 1, 'numeric', 0, '', '', 'This is the rebate calculation rule that will be used when calculating the BDU value when doing a BDW. To set the rule to 78+1 set the value to 1.', 'BduRebateCalculationRule'
	from country c
END

