-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

GO
-- *************************************************************************************************
-- Developer:	Amit
-- Date:		15 June 2020
-- Purpose:		Include flag in CountryMaintenance to "Include Warranties In Online Product Export".
-- *************************************************************************************************

DECLARE @CountryCode CHAR(2)

-- Populate CountryCode
SELECT TOP 1 @CountryCode = countrycode FROM country WITH(NOLOCK)


IF NOT EXISTS (SELECT * FROM [CountryMaintenance] WITH(NOLOCK) WHERE CodeName='IncWarrantyInOnlineProdExport')
BEGIN
		
	INSERT INTO CountryMaintenance	(CountryCode,ParameterCategory,Name,Value,[Type],[Precision],OptionCategory,OptionListName,[Description],CodeName)
	VALUES ( @CountryCode 
			,(SELECT code FROM code WHERE codedescript = 'Warranties' AND category = 'CMC')
			,'Include Warranties In Online Product Export'
			,'True'
			,'checkbox'
			,0
			,''
			,''
			,'If set to false then it will exclude all warranty items from the ecommerce file which is exported from EOD Online Product Export job.'
			,'IncWarrantyInOnlineProdExport'); 

END

GO