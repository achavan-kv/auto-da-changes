-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


declare @parametercategory int
set @parametercategory = (select max(parametercategory)
							from CountryMaintenance
							where CodeName like 'IC%')


INSERT INTO [CountryMaintenance]
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
    SELECT TOP 1
           CountryCode
           ,@parametercategory
           ,'Months since address change'
           ,'12'
           ,'numeric'
           ,0
           ,''
           ,''
           ,'If the address change requires manual approval is on, customers whose address has changed in the last X months AND have score below Y will be disqualified'
           ,'IC_addressmonths'
    FROM CountryMaintenance
GO

ALTER TABLE InstantCreditParameters
ADD addressmonths varchar
GO


declare @parametercategory int
set @parametercategory = (select max(parametercategory)
							from CountryMaintenance
							where CodeName like 'IC%')


INSERT INTO [CountryMaintenance]
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
    SELECT TOP 1
           CountryCode
           ,@parametercategory
           ,'Months since employment change'
           ,'TRUE'
           ,'checkbox'
           ,0
           ,''
           ,''
           ,'If the employment change requires manual approval is on, customers whose employment has changed in the last X months AND have score below Y will be disqualified'
           ,'IC_employmonths'
    FROM CountryMaintenance
GO

ALTER TABLE InstantCreditParameters
ADD employmonths varchar
GO

