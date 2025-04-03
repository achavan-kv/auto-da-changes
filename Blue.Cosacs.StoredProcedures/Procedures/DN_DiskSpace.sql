-- Author: 	John Croft
-- Date:	March 2006
--		This procedure check there is enough disk space for backup by 
--		extracing the size of the database and available space on the save drive
--		(as specified in the code table category 'BAK'

-- Modified:  '%File%' changed to '%File(%' due to error caused by directory name containing
--            "file" e.g. "\Program Files"
--            increased @databasename to 100			29/09/06 jec
--			 Set @OK = 'PT' if backup path in error		08/02/07 jec

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_DiskSpace]') 
                        and OBJECTPROPERTY(id, 'IsProcedure') = 1)
Drop Procedure DN_DiskSpace
go

Create Procedure DN_DiskSpace

@BackupType  varchar(12),
@DatabaseSize decimal(15,0) output,
@FreeDiskSpace decimal(15,0) output,
@Ok char(2) output,
@return int OUTPUT

as

SET  @return = 0 --initialise return code

DECLARE @BCPpath VARCHAR(500)

SELECT @BCPpath = value + '\BCP' FROM CountryMaintenance
WHERE Codename = 'BCPpath' 


set nocount on
IF EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES
 	   WHERE  table_name = 'zz_Directory')
 drop table zz_Directory

create table zz_Directory
(
DirText varchar(100)
)
	
set @Ok='NO'  -- default - not enough space

declare @databaseName varchar(100)    -- increased from 50
-- get database path from system table
set @databaseName=(select filename from master..sysdatabases
where name=db_name())

declare @path varchar(200)
-- delete old directory listing
set @path = '"del d:\users\default\dir*.csv" ' 
exec master.dbo.xp_cmdshell @path
-- list directory for current database
set @path = '"dir "' + @databasename + '" >>d:\users\default\dir.csv" '
exec master.dbo.xp_cmdshell @path

-- import directory list into database table
set @path = '"' + @BCPpath + '" ' + db_name() +
	'..zz_Directory' + ' in ' +
 'd:\users\default\Dir.csv ' + '-c -t, -q -T'

exec master.dbo.xp_cmdshell @path

--------------------------------------------------

Declare @filesize  decimal(15,0),
	@freespace decimal(15,0),
	@y int,
	@len1 int

set @len1=(select len(dirtext) from zz_directory
where dirText like'%File(%') 
--select @len1

set @y=(select charindex('File(s)',DirText,1) from zz_directory
where dirText like'%File(%')
--select @y

-- extract filesize as decimal from Directory table
set @filesize=(
select cast(replace(replace(replace(replace(right(DirText,@len1-@y+1),'File(s)',''),' bytes',''),',',''),' ','') 
	as decimal (15,0)) 
from zz_directory
where dirText like'%File(%')
 
-- extract freespace as decimal from Directory table
set @len1=(select len(dirtext) from zz_directory
where dirText like'%Dir(%') 
--select @len1

set @y=(select charindex('Dir(s)',DirText,1) from zz_directory
where dirText like'%Dir(%')
--select @y
set @freespace=(
select cast(replace(replace(replace(replace(right(DirText,@len1-@y+1),'Dir(s)',''),' bytes free',''),',',''),' ','')
 	as decimal (15,0)) 
from zz_directory
where dirText like'% Dir(%')

--select @filesize as 'Database Size',@freespace as FreeSpace

--------------------------------------------------------------------------
-- Get path of save directory from Code table

declare @len int,
	@x int,
	@last int,
	@backuppath varchar(100),
    @backupname varchar(100),
    @likebackupname varchar(100),
    @backupsize decimal(15,0),
    @minfreespace decimal(15,0)    -- minimum free space allowed after backup

set @minfreespace=50000000    -- 50mb
-- get backup row from code table
set @len=(select len(codedescript) from code
WHERE code=@BackupType and category = 'BAK')
-- find last '\' in codescript
set @x=1
set @last=0
while @x>=@last	
begin
-- search for '\' 
set @x=(select charindex('\',codedescript,@last+1) from code
WHERE code=@BackupType and category = 'BAK')
if @x>@last
set @last=@x
end
--select @last
-- get path (upto last '\')
select @backuppath=(select left(codedescript,@last) from code
WHERE code=@BackupType and category = 'BAK')
-- get backup name
select @backupname=(select right(codedescript,@len-@last) from code
WHERE code=@BackupType and category = 'BAK')
-- set up like 
set @likebackupname='%' + @backupname 
-- does file exist in directory
if exists (select * from zz_directory
where dirtext like @likebackupname)
    Begin
    -- get file size
        set @backupsize= (select cast(replace(replace(substring(DirText,20,16),',','')
            ,' ','') 
	        as decimal (15,0)) 
        from zz_directory
        where dirtext like @likebackupname)
    End
    -- file doesn't exist 
else set @backupsize=0

delete zz_Directory

-- delete old directory listing
set @path = '"del d:\users\default\dir*.csv" ' 
exec master.dbo.xp_cmdshell @path
-- list directory for current database
set @path = '"dir "' + @backuppath + '" >>d:\users\default\dir.csv" '
exec master.dbo.xp_cmdshell @path

-- import directory list into database table
set @path = '"' + @BCPpath + '" ' + db_name() +
	'..zz_Directory' + ' in ' +
 'd:\users\default\Dir.csv ' + '-c -t, -q -T'

exec master.dbo.xp_cmdshell @path
 
--------------------------------------------------

Declare @filesize2  decimal(15,0),
	@freespace2 decimal(15,0)
	--@y int,
	--@len1 int

set @len1=(select len(dirtext) from zz_directory
where dirText like'%File(%') 
--select @len1

set @y=(select charindex('File(s)',DirText,1) from zz_directory
where dirText like'%File(%')
--select @y

-- extract filesize as decimal from Directory table
set @filesize2=(
select cast(replace(replace(replace(replace(right(DirText,@len1-@y+1),'File(s)',''),' bytes',''),',',''),' ','') 
	as decimal (15,0)) 
from zz_directory
where dirText like'%File(%')
 
-- extract freespace as decimal from Directory table
set @len1=(select len(dirtext) from zz_directory
where dirText like'%Dir(%') 
--select @len1

set @y=(select charindex('Dir(s)',DirText,1) from zz_directory
where dirText like'%Dir(%')
--select @y
set @freespace2=(
select cast(replace(replace(replace(replace(right(DirText,@len1-@y+1),'Dir(s)',''),' bytes free',''),',',''),' ','')
 	as decimal (15,0)) 
from zz_directory
where dirText like'% Dir(%')

--select @filesize as 'Database Size',@freespace2 as 'Save Directory Space'
--Select @filesize2 as 'Database Size',@freespace2 as FreeSpace

--space required = database size 
set @DatabaseSize=@fileSize
--space available = freespace + size of existing backup (being overwritten) - min free space    
set @FreeDiskSpace=@freespace2+@backupsize-@minfreespace 

If @DatabaseSize<=@FreeDiskSpace
    set @Ok='OK' 

If @FreeDiskSpace is null
	set @Ok='PT'	-- Path error 
    
SET @return = @@error    

Return  

Go

-- End End End End End End End End End End End End End End End End End End End End End End End End 


