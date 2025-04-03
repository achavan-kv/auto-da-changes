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
           ,'Settled Account Length'
           ,'12'
           ,'numeric'
           ,0
           ,''
           ,''
           ,'If a customer is qualifying under a recent settled account the account must have been open for at least this many months'
           ,'IC_settledmonths'
    FROM CountryMaintenance
GO

ALTER TABLE InstantCreditParameters
ADD settledlength varchar
GO
