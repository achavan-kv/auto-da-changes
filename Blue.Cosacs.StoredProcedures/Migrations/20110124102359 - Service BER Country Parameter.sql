-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


IF NOT EXISTS (SELECT * FROM dbo.CountryMaintenance WHERE CodeName = 'ServiceBERCostPricePCent')
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
	   'Service BER Costprice %',
	   '10',
	   'numeric',
	   '0',
	   '',
	   '',
	   'This is the percentage of the costprice of the item which is BER that will be applied to the costprice when calculating the courts parts total',
	   'ServiceBERCostPricePCent' 
FROM dbo.Country
