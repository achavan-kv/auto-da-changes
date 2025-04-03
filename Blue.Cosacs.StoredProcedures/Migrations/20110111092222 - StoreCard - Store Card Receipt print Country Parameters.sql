-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS (SELECT * FROM dbo.CountryMaintenance WHERE CodeName = 'SCardReceiptDisplayFooter')
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
	   'Store Card Receipt Display Footer',
	   'true',
	   'checkbox',
	   '1',
	   '',
	   '',
	   'Do you want to display a footer on the Store Card receipt?',
	   'SCardReceiptDisplayFooter' 
FROM dbo.Country

IF NOT EXISTS (SELECT * FROM dbo.CountryMaintenance WHERE CodeName = 'SCardReceiptDisplayTitle')
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
	   'Store Card Receipt Display Title',
	   'true',
	   'checkbox',
	   '1',
	   '',
	   '',
	   'Do you want to display the Store Card receipt title?',
	   'SCardReceiptDisplayTitle' 
FROM dbo.Country

IF NOT EXISTS (SELECT * FROM dbo.CountryMaintenance WHERE CodeName = 'SCardReceiptTitle')
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
	   'Store Card Receipt Title',
	   'Store Card Receipt',
	   'Text',
	   '1',
	   '',
	   '',
	   'The title text to identify a Store Card receipt.',
	   'SCardReceiptTitle' 
FROM dbo.Country

IF NOT EXISTS (SELECT * FROM dbo.CountryMaintenance WHERE CodeName = 'SCardReceiptFooter')
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
	   'Store Card Receipt Footer',
	   '',
	   'text',
	   '1',
	   '',
	   '',
	   'Information to be printed at the bottom of the Store Card receipt e.g. Thank you for shopping at COURTS!',
	   'SCardReceiptFooter' 
FROM dbo.Country



IF NOT EXISTS (SELECT * FROM dbo.CountryMaintenance WHERE CodeName = 'SCardReceiptDisplaySignature')
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
	   'Store Card Receipt Signature',
	   'true',
	   'checkbox',
	   '1',
	   '',
	   '',
	   'Do you want to display the signature area on Store Card receipts?',
	   'SCardReceiptDisplaySignature' 
FROM dbo.Country


IF NOT EXISTS (SELECT * FROM dbo.CountryMaintenance WHERE CodeName = 'SCardReceiptSignatureText')
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
	   'Store Card Receipt Signature Text',
	   'Signature',
	   'text',
	   '1',
	   '',
	   '',
	   'The text to be displayed for signatures.',
	   'SCardReceiptSignatureText' 
FROM dbo.Country



