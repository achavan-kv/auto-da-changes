-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS (SELECT * FROM dbo.CountryMaintenance WHERE CodeName = 'SCardInterestFreeDays')
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
		   'Store Card Interest Free Days',
		   'M',
		   'Text',
		   '0',
		   '',
		   '',
		   'StoreCard Statement Interest Free Days. Number of days between Statement and Payment Due Date ',
		   'SCardInterestFreeDays' 
	FROM dbo.Country
END
GO  
