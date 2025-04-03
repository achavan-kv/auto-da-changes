-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


IF NOT EXISTS (SELECT * FROM dbo.CountryMaintenance WHERE CodeName = 'RIDispCatAsDept')
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
		   '01',
		   'Replace "Category" label with "Department"',
		   'false',
		   'checkbox',
		   '0',
		   '',
		   '',
		   'If true, labels previously displayed as "Category" will now be displayed as "Department". If false the label will remain as "Category".',
		   'RIDispCatAsDept' 
	FROM dbo.Country
END