-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS (SELECT * FROM dbo.CountryMaintenance WHERE CodeName = 'StoreCardExpMnths')
INSERT INTO CountryMaintenance(
			CountryCode,
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
	  '33',
	  'Default Expiry Months',
	  '48',
	  'numeric',
	  '0',
	  '',
	  '',
	  'Number of months to determine the expiry date of a Store Card',
	  'StoreCardExpMnths' 
FROM dbo.Country

