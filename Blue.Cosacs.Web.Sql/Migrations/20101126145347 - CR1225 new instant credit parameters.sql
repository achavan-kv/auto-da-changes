-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

--New No of Months for Referral Parameter

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
           ,'Referral History Months'
           ,0
           ,'numeric'
           ,0
           ,''
           ,''
           ,'This is the number of months proposal history to check for a referral'
           ,'IC_ReferralMonths'
    FROM CountryMaintenance
GO


--New minimum score for address 

declare @parametercategory int
set @parametercategory = (select max(parametercategory)
							from CountryMaintenance
							where CodeName like 'IC%')


declare @minscore int
set @minscore = (select value
							from CountryMaintenance
							where CodeName = 'IC_MinCredScore')

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
           ,'Maximum score for Change in Address check'
           ,@minscore
           ,'numeric'
           ,0
           ,''
           ,''
           ,'customers with this score or lower will be checked for address changes if this check is on'
           ,'IC_AddressCheckScore'
    FROM CountryMaintenance
GO


--New minimum score for employment 

declare @parametercategory int
set @parametercategory = (select max(parametercategory)
							from CountryMaintenance
							where CodeName like 'IC%')


declare @minscore int
set @minscore = (select value
							from CountryMaintenance
							where CodeName = 'IC_MinCredScore')

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
           ,'Maximum score for Change in employment check'
           ,@minscore
           ,'numeric'
           ,0
           ,''
           ,''
           ,'customers with this score or lower will be checked for employment changes if this check is on'
           ,'IC_EmploymentCheckScore'
    FROM CountryMaintenance
GO

ALTER TABLE InstantCreditParameters
ADD ReferralMonths int
GO

ALTER TABLE InstantCreditParameters
ADD AddressCheckScore int
GO

ALTER TABLE InstantCreditParameters
ADD EmploymentCheckScore int
GO


