-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


-- =======================================================================================
-- Author			: SHUBHAM GAIKWAD
-- Create Date		: 28 July 2020
-- Description		: Add country parameter ApplyNewDispIncomeChanges, SpouseRentFactor
-- =======================================================================================

GO

IF NOT EXISTS(SELECT 1 FROM [dbo].[CountryMaintenance] WHERE CodeName ='ApplyNewDispIncomeChanges')
BEGIN
INSERT INTO [dbo].[CountryMaintenance]
           (CountryCode
           ,ParameterCategory
           ,[Name]
           ,[Value]
           ,[Type]
           ,[Precision]
           ,[OptionCategory]
           ,[OptionListName]
           ,[Description]
           ,[CodeName])
     VALUES((SELECT countrycode FROM country WITH(NOLOCK))
			,(SELECT code FROM code WHERE codedescript = 'Credit Sanctioning' AND category = 'CMC') 
            ,'Allow Process for Calculation of Disposable Income'
            ,'false'
            ,'checkbox'
            ,'0'
            ,''
            ,''
            ,'This field have the values True or False, If set as true then Process for calculation of Disposable Income changes will be applicable.'
            ,'ApplyNewDispIncomeChanges')

END
GO

--------------------------------------------------------------------------------------------------------------------------------------------
--------------------------------------------------------------------------------------------------------------------------------------------

IF NOT EXISTS(SELECT 1 FROM [dbo].[CountryMaintenance] WHERE CodeName ='SpouseRentFactor')
BEGIN
INSERT INTO [dbo].[CountryMaintenance]
           (CountryCode
           ,ParameterCategory
           ,[Name]
           ,[Value]
           ,[Type]
           ,[Precision]
           ,[OptionCategory]
           ,[OptionListName]
           ,[Description]
           ,[CodeName])
     VALUES((SELECT countrycode FROM country WITH(NOLOCK))
			,(SELECT code FROM code WHERE codedescript = 'Credit Sanctioning' AND category = 'CMC')        
            ,'Rent/Mortgage % for Disposable Income'
            ,'50'
            ,'numeric'
            ,'0'
            ,''
            ,''
            ,'Allow user to define the % value to be considered to calculate the Rent/Mortgage , if person is married and spouse is working'
            ,'SpouseRentFactor')

END
GO