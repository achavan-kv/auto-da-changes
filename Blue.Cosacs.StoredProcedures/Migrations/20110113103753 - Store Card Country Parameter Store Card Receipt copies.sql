-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here



IF NOT EXISTS (SELECT * FROM dbo.CountryMaintenance WHERE CodeName = 'SCardReceiptCopies')
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
	   '15',
	   'Store Card Receipt Copies',
	   '1',
	   'numeric',
	   '0',
	   '',
	   '',
	   'This is the number of copies of the Store Card Receipt that will be printed',
	   'SCardReceiptCopies' 
FROM dbo.Country

