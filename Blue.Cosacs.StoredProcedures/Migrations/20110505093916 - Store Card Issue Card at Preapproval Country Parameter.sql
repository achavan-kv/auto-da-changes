-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS (SELECT * FROM dbo.CountryMaintenance WHERE CodeName = 'StoreCardIssueCardPreAppr')
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
		   'Issue Store Card at Pre-Approval End Of Day',
		   'False',
		   'checkbox',
		   '0',
		   '',
		   '',
		   'If true, Store Card accounts will be created for Customers when running the Store Card pre-approval End Of Day routine',
		   'StoreCardIssueCardPreAppr' 
	FROM dbo.Country
END
