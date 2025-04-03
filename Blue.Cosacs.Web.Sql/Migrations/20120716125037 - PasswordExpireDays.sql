-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF EXISTS(SELECT 1 FROM CountryMaintenance WHERE CodeName = 'PasswordExpireDays')
	RAISERROR ('CodeName "PasswordExpireDays" already exists in this database', 20, 1, '') WITH LOG, NOWAIT
	
    
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
	'Password Expire Days' AS Name,
	30 AS Value,
	'numeric' AS [type],
	0 AS Precision,
	'' AS OptionCategory,
	'' AS OptionName,
	'Numer of days untill the user must change his password' AS Description,
	'PasswordExpireDays' AS CodeName
FROM 
	country

