-- Author: 	John Croft
-- Date:	March 2006
--		This procedure will run the Database Backups from the EOD process

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
SET ANSI_WARNINGS ON
go

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_DatabaseBackup]') 
        and OBJECTPROPERTY(id, 'IsProcedure') = 1)
Drop Procedure DN_DatabaseBackup
go

Create Procedure DN_DatabaseBackup

@interfaceName  varchar(12),
@return int OUTPUT

as

declare @dbName varchar(20),
--	@BackupType  varchar(12),
	@path varchar(64)

SET 	@return = 0			--initialise return code
set @dbname=db_name()

-- Get the path & file name

set @path=(select codedescript from Code
	Where category='BAK' and code=@interfaceName)

-- check backup type 
If (substring(@interfaceName,6,1) ='D')

Begin
-- Differential Backup
	BACKUP DATABASE @dbName TO  DISK = @path WITH  NOINIT ,  NOUNLOAD ,  
			DIFFERENTIAL ,  NAME = 'Differential Backup',  
			NOSKIP ,  STATS = 10,  NOFORMAT
End
else
Begin
--Complete Backup 
	BACKUP DATABASE @dbName TO  DISK = @path WITH  NOINIT ,  NOUNLOAD ,  
			NAME = 'Complete Backup',  
			NOSKIP ,  STATS = 10,  NOFORMAT
End

IF @@error != 0
	BEGIN
		SET @return = @@error
	END

Go

-- End End End End End End End End End End End End End End End End End End End End End End End 
 