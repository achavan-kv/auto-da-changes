-- transaction: false

DECLARE @sql nvarchar(1000)
set @sql = 'ALTER DATABASE [' +  db_name() + '] SET PAGE_VERIFY CHECKSUM  WITH NO_WAIT';
execute sp_executesql @sql
GO


