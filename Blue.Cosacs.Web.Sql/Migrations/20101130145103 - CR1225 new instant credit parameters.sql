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
           ,'Revise Months'
           ,0
           ,'numeric'
           ,0
           ,''
           ,''
           ,'This is the number of months which an account can be revised without re-passing qualification'
           ,'IC_ReviseMonths'
    FROM CountryMaintenance
GO