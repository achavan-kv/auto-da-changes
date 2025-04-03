-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS (SELECT * FROM dbo.CountryMaintenance WHERE CodeName = 'SCardPreExpiryMonths')
BEGIN
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
		   '33',
		   'Store Card Expiry Months after Pre-approval',
		   '6',
		   'numeric',
		   '0',
		   '',
		   '',
		   'Number of months after wish a store card preapproval will lapse ',
		   'SCardPreExpiryMonths' 
	FROM dbo.Country
END
GO  