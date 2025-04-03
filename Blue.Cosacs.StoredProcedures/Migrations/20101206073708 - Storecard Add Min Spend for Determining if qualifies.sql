-- transaction: true

-- put your SQL code here

INSERT INTO CountryMaintenance           ([CountryCode]           ,[ParameterCategory]
           ,[Name]           ,[Value]           ,[Type]
           ,[Precision]
           ,[OptionCategory]
           ,[OptionListName]
           ,[Description]
           ,[CodeName])
    SELECT TOP 1
           CountryCode           ,'33'           ,
           'Store Card Minimum Spend Limit'  ,'0'           ,'numeric','0'
           ,''
           ,''
           ,'This is the minimum amount of spend limit that a customer must have to be able to qualify for a storecard'
           ,'MinStoreCardLimit'
    FROM Country
GO