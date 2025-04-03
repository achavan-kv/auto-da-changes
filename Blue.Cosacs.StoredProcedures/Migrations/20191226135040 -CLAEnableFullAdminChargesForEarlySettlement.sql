GO
IF NOT EXISTS(SELECT 1 FROM [dbo].[CountryMaintenance] WHERE CodeName ='CL_ApplyFullAdmin')
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
			,'30'       
            ,'Flag to Apply full Admin Charges to early Sett Amt'
            ,'false'
            ,'checkbox'
            ,'0'
            ,''
            ,''
            ,'If this flag is true full Admin Charges are applied for early Settlement If false  Admin charges till date will be applied'
            ,'CL_ApplyFullAdmin')

END
GO
