-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS (SELECT * FROM dbo.CountryMaintenance WHERE CodeName = 'SCardMaxNoCutsPreapprove')
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
		   'Store Card Max Number of Customers for Preapproval',
		   '0',
		   'numeric',
		   '0',
		   '',
		   '',
		   'End Of Day StoreCard Maximum number of Customers for Pre-approval in Store Card Qualification Job. 0 means no limit',
		   'SCardMaxNoCutsPreapprove' 
	FROM dbo.Country
END
GO  