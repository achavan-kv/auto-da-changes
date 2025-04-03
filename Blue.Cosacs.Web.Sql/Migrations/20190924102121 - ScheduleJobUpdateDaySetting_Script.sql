
IF NOT EXISTS (SELECT * FROM [Config].[Setting] WHERE [Id]='ScheduleJobUpdateDaySetting')
BEGIN  

INSERT INTO [Config].[Setting]
           ([Namespace]
           ,[Id]
           ,[ValueBit]
           ,[ValueInt]
           ,[ValueDateTime]
           ,[ValueDecimal]
           ,[ValueString]
           ,[ValueText]
           ,[ValueFile])
     VALUES
           ('Blue.Cosacs.Merchandising'
           ,'ScheduleJobUpdateDaySetting'
           ,NULL
           ,1
           ,NULL
           ,NULL
           ,'1'
           ,'This is used to configure for the execution of scheduled job for getting updates only'
           ,NULL)
END




GO
