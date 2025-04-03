-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS(select * from countrymaintenance where codename = 'TaxInvPrintAfterPayment')
BEGIN
	INSERT INTO CountryMaintenance(CountryCode, ParameterCategory, Name, Value, [Type], [Precision], OptionCategory, OptionListName, [Description], CodeName)
	SELECT c.countrycode, 15, 'Tax Invoice Print After Payment', 'True', 'checkbox', 1, '', '','If true, the Tax Invoice will be printed when a payment is processed and Thermal print is enabled.', 'TaxInvPrintAfterPayment'
	FROM Country c
END
