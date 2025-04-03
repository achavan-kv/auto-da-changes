-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS (SELECT * FROM dbo.CountryMaintenance WHERE CodeName = 'StoreCardStatementFrequency')
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
		   'Default Store Card Statement Frequency',
		   'M',
		   'Text',
		   '0',
		   '',
		   '',
		   'Default StoreCard Statement Frequency for new cards - M- Monthly, B - Bi Monthly, Q - Quarterly, S - Six Monthly',
		   'StoreCardStatementFrequency' 
	FROM dbo.Country
END
go 
