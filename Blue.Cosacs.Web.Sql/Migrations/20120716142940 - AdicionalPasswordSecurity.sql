-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF EXISTS(SELECT 1 FROM CountryMaintenance WHERE CodeName = 'MinRequiredNonalphanumericChar')
    DELETE FROM CountryMaintenance WHERE CodeName = 'MinRequiredNonalphanumericChar'
	
IF EXISTS(SELECT 1 FROM CountryMaintenance WHERE CodeName = 'MinRequiredPasswordLength')
    DELETE FROM CountryMaintenance WHERE CodeName = 'MinRequiredPasswordLength'
	 
INSERT INTO CountryMaintenance
           ([CountryCode]
           ,[ParameterCategory]
           ,[Name]
           ,[Value]
           ,[Type]
           ,[Precision]
           ,[OptionCategory]
           ,[OptionListName]
           ,[Description]
           ,[CodeName])
SELECT 
	CountryCode,
	'01' AS ParameterCategory,
	'Min Non Alfanumeric charaters' AS Name,
	2 AS Value,
	'numeric' AS [type],
	0 AS Precision,
	'' AS OptionCategory,
	'' AS OptionName,
	'Minimun number of non alfa numeric characters. Example: &^%$£!._' AS Description,
	'MinRequiredNonalphanumericChar' AS CodeName
FROM 
	country
UNION ALL
SELECT 
	CountryCode,
	'01' AS ParameterCategory,
	'Min Password Length' AS Name,
	5 AS Value,
	'numeric' AS [type],
	0 AS Precision,
	'' AS OptionCategory,
	'' AS OptionName,
	'Minimun password length' AS Description,
	'MinRequiredPasswordLength' AS CodeName
FROM 
	country

