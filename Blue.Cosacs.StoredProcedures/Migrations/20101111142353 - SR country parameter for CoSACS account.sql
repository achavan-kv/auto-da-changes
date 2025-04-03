-- put your SQL code here
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
           ,'28'
           ,'Service Account Name'
           ,'Courts'
           ,'text'
           ,0
           ,''
           ,''
           ,'This is the name shown in service request when selecting the service type ie if Courts options are Courts, Non-Courts and Stock repair'
           ,'SRAcctName'
    FROM CountryMaintenance
GO

