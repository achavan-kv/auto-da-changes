-- Author:  John Croft  
-- Date:    March 2006

/*    This procedure will rename the Standing Order raw data files

*/

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
SET ANSI_WARNINGS ON
go

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_StandingOrderRename]') 
        and OBJECTPROPERTY(id, 'IsProcedure') = 1)
Drop Procedure .[dbo].[DN_StandingOrderRename]
Go
SET QUOTED_IDENTIFIER OFF
Go

Create Procedure .dbo.DN_StandingOrderRename

-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_StandingOrderRename
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : DN_StandingOrderVal 
-- Author       : John Croft
-- Date         : 2007
--
-- This procedure will move the files successfully processed 
-- to a Processed folder\date beneath the banks directory
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 12/08/10 ip  CR1092 - COASTER to CoSACS Enhancements
-- ====================================================
@runNo Int,
@return int OUTPUT

as

set nocount on

declare @path varchar(200),
        @filename varchar(32),
        @processed char(1), --IP - 12/08/10 - CR1092
        @source varchar(16), --IP - 12/08/10 - CR1092
        @folderDate varchar(8)	--IP - 12/08/10 - CR1092

SET @return = 0			--initialise return code
SET @folderDate = (cast(convert(varchar(8), getdate(), 112) as int))



--IP - 12/08/10 - CR1092 - Move the processed files to beneath the (bankname)\processed\date folder
Declare SC cursor for select source, filename, processed from SOFilesProcessed 
where runno = @runNo
and processed = 'Y'

    Open SC
    Fetch next from SC into @source, @filename, @processed
    While @@Fetch_status = 0
    Begin
    
    --Create the processed directory beneath the bank and a sub date folder
    set @path = 'MD d:\cosdata\sodata\' + @source + '\Processed\' + @folderDate
    exec master.dbo. xp_cmdshell @path
    
    --Move the file to beneath the processed\date folder
    set @path = 'MOVE d:\cosdata\sodata\' + @source + '\' + @filename + ' d:\cosdata\sodata\' + @source + '\Processed\' + @folderDate
    exec master.dbo. xp_cmdshell @path
 
    Fetch next from SC into  @source, @filename, @processed

    End

-- Close & Deallocate 
Close SC
Deallocate SC

IF @@error != 0
	BEGIN
		SET @return = @@error
	END

Go

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End 

