-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
DECLARE @command NVARCHAR(4000)


SELECT @command = name  FROM sysobjects
WHERE name LIKE '%DF__User__PasswordS%'
AND xtype ='D'
and parent_obj = OBJECT_ID('[Admin].[User]')

SET @command = 'ALTER TABLE [Admin].[User] DROP CONSTRAINT [' + @command + ']'

exec sp_executesql @command    
GO

DECLARE @command NVARCHAR(4000)

SELECT @command = name  FROM sysobjects
WHERE name LIKE '%UQ__User__%'
AND xtype ='UQ'
and parent_obj = OBJECT_ID('[Admin].[User]')

SET @command = 'ALTER TABLE [Admin].[User] DROP CONSTRAINT [' + @command + ']'

exec sp_executesql @command    
go

alter table [Admin].[User] drop column Passwordsalt;

alter table [Admin].[User] alter column [Password] varchar(100);
alter table [Admin].[User] alter column ExternalLogin varchar(50);
alter table [Admin].[User] alter column [Login] varchar(50);

go

CREATE UNIQUE NONCLUSTERED INDEX IX_User_Login ON Admin.[User]
	(
	Login
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO