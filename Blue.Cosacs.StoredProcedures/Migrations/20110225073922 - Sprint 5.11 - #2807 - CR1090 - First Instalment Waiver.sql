-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

--Add new Country Parameter for first instalment waiver qualification rule based on minimum credit score

IF EXISTS(SELECT * FROM dbo.CountryMaintenance WHERE CodeName = 'InstalWaiverMinScore')
DELETE FROM CountryMaintenance where codename = 'InstalWaiverMinScore'

IF NOT EXISTS (SELECT * FROM dbo.CountryMaintenance WHERE CodeName = 'InstalWaiverMinScore')
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
	   '11',
	   'Instalment Waiver Minimum Credit Score',
	   '0',
	   'numeric',
	   '0',
	   '',
	   '',
	   'This is the minimum score needed in order for an account to qualify for first instalment waiver. If this parameter is set to zero first instalment waiver is not enabled',
	   'InstalWaiverMinScore' 
FROM dbo.Country


IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.columns  WHERE table_name ='Instalplan' AND column_name = 'InstalmentWaived')
BEGIN
    alter table Instalplan add InstalmentWaived bit NOT NULL DEFAULT(0)
END


