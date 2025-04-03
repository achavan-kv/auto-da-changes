
DECLARE @command NVARCHAR(4000)


SELECT @command = name  FROM sysobjects
WHERE name LIKE '%DF__User__Password%'
AND xtype ='D'

SET @command = 'ALTER TABLE [Admin].[User] DROP CONSTRAINT [' + @command + ']'

exec sp_executesql @command    
GO

