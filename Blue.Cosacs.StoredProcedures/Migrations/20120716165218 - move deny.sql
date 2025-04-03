
DECLARE @command NVARCHAR(4000)


SELECT @command = name  FROM sysobjects
WHERE name LIKE '%Permission__Deny%'
AND xtype ='D'

SET @command = 'ALTER TABLE [Admin].[Permission] DROP CONSTRAINT [' + @command + ']'

exec sp_executesql @command    
GO


ALTER TABLE Admin.Permission
DROP COLUMN [Deny]
GO

ALTER TABLE admin.RolePermission
ADD [Deny] BIT NOT NULL CONSTRAINT DenyDefault DEFAULT 0
GO





