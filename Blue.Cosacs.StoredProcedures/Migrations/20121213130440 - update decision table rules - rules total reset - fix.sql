 IF NOT EXISTS(SELECT * FROM [Config].[DecisionTableType] WHERE [Key] = N'SR.DecisionTable.Payment')
 BEGIN
    INSERT [Config].[DecisionTableType] ([Key]) VALUES (N'SR.DecisionTable.Payment')
 END
