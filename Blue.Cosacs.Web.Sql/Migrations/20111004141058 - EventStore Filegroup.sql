-- transaction: false

declare @db sysname
declare @sql nvarchar(4000)
select @db = DB_NAME()
SELECT @sql = 'ALTER DATABASE [' + @db + '] ADD FILEGROUP [AUDIT]'

print @sql
exec sp_executesql @sql

declare @fname nvarchar(4000)
select @fname = "filename" 
from sys.sysaltfiles 
where dbid = DB_ID(@db) AND "filename" LIKE '%.mdf'

set @fname = REPLACE(@fname, '.mdf', '_Audit.ndf')

set @sql =
('ALTER DATABASE [' + @db + '] ADD FILE (
NAME = ''[' +@db + '_Audit]'', FILENAME = ''' + @fname + ''') TO FILEGROUP [AUDIT]')

print @sql
exec sp_executesql @sql

