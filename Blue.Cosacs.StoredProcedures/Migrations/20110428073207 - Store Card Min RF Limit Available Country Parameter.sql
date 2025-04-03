-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS (SELECT * FROM dbo.CountryMaintenance WHERE CodeName = 'StoreCardMinRFAvail')
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
		   'Store Card Minimum RF Limit Available',
		   '0',
		   'numeric',
		   '0',
		   '',
		   '',
		   'This is the minimum amount that should be available on the existing RF Limit for a customer to be able to qualify for a storecard',
		   'StoreCardMinRFAvail' 
	FROM dbo.Country
END
