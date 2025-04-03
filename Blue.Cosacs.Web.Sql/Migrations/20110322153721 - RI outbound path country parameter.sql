-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS (SELECT * FROM dbo.CountryMaintenance WHERE CodeName = 'RIOutboundDirectory')
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
	   '25',
	   'RI outbound directory',
	   'D:\users\default',
	   'text',
	   '0',
	   '',
	   '',
	   'Output directory path for RI outbound files',
	   'RIOutboundDirectory' 
FROM dbo.Country