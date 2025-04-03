-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
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
           ,'Joint account holders must qualify'
           ,'TRUE'
           ,'checkbox'
           ,0
           ,''
           ,''
           ,'If this option is on, all joint account holders for customers accounts must also qualify, for the customer to qualify'
           ,'IC_JointQualification'
    FROM CountryMaintenance
GO

ALTER TABLE InstantCreditParameters
ADD jointqual varchar
GO
