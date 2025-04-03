-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


IF NOT EXISTS (SELECT * FROM dbo.CountryMaintenance WHERE CodeName = 'DisplayAssemblyOptions')
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
		   '13',
		   'Display Assembly Options',
		   'False',
		   'checkbox',
		   '0',
		   '',
		   '',
		   'If true, a popup displaying assembly options will appear upon entering an item in the New Sales Order screen if this item requires assembly',
		   'DisplayAssemblyOptions' 
	FROM dbo.Country
END


