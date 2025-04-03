
--Script for Address Standardization CR2019 - 025

IF NOT EXISTS(SELECT 1 FROM [Admin].[Permission] WHERE Id = 1207)
BEGIN
	INSERT INTO [Admin].[Permission]
			   ([Id]
			   ,[Name]
			   ,[CategoryId]
			   ,[Description]
			   ,[IsDelegate])
		 VALUES
			   (1207
			   ,'Address Master management'
			   ,12
			   ,'Allow users to Create/Edit/Delete Address Master detail.'
			   ,0)
END
GO
