-- transaction: false
declare @level int
declare @sql nvarchar(1000)

SELECT @level = compatibility_level
FROM sys.databases WHERE name = db_name()

IF (@level < 100)
BEGIN
set @sql = 'ALTER DATABASE [' +  db_name() + '] SET COMPATIBILITY_LEVEL = 100';
execute sp_executesql @sql
print @sql
END
GO


