-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


-- *************************************************************************************************
-- Developer:	Amit
-- Date:		08 July 2020
-- Purpose:		Include flag in CountryMaintenance to "Enable MMI".
-- *************************************************************************************************

DECLARE @CountryCode CHAR(2)

-- Populate CountryCode
SELECT TOP 1 @CountryCode = countrycode FROM country WITH(NOLOCK)


IF NOT EXISTS (SELECT * FROM [CountryMaintenance] WITH(NOLOCK) WHERE CodeName='EnableMMI')
BEGIN
		
	INSERT INTO CountryMaintenance	(CountryCode,ParameterCategory,Name,Value,[Type],[Precision],OptionCategory,OptionListName,[Description],CodeName)
	VALUES ( @CountryCode 
			,(SELECT code FROM code WHERE codedescript = 'Credit Sanctioning' AND category = 'CMC')
			,'Enable MMI'
			,'False'
			,'checkbox'
			,0
			,''
			,''
			,'This will enable all MMI functionality.'
			,'EnableMMI'); 

END