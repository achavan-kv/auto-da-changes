---- Author:  John Croft  
---- Date:    March 2006

--/*    This procedure will load the batch of Standing order transactions into a table by executing one of two procedures
--		based on the file being a fixed or delimited file, and then validate the the batch totals.

--*/
if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_StandingOrderVal]') 
        and OBJECTPROPERTY(id, 'IsProcedure') = 1)
Drop Procedure .[dbo].[DN_StandingOrderVal]
Go
SET QUOTED_IDENTIFIER OFF
Go

Create Procedure .dbo.DN_StandingOrderVal 
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_StandingOrderVal.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : DN_StandingOrderVal 
-- Author       : John Croft
-- Date         : 2007
--
-- This procedure will validate the payment files from banks.
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 16/04/10 jec UAT86 Allow file without a trailer.
-- 09/08/10 ip  CR1092 - COASTER to CoSACS Enhancements
-- 27/08/10 ip  CR1092 - COASTER to CoSACS Enhancements - Dynamically create the StorderDelimited table.
-- 31/08/10 ip  CR1092 - COASTER to CoSACS Enhancements - check for any warnings written to interfaceerror table. If so need to mark the EOD as warning.
-- 01/09/10 ip	CR1092 - COASTER to CoSACS Enhancements - If the BCPPath has been setup incorrectly do not process any files and insert error into Interfaceerror and SOException table.
-- ================================================
	-- Add the parameters for the stored procedure here
    @RunNo Int,
    @containsWarnings bit out,	--IP - 31/08/10 - CR1092
    @return int out

as
set nocount ON


DECLARE @BCPpath VARCHAR(500),
		@BCPPathCheck varchar(200),		--01/09/10 - CR1092
		@BCPFileName varchar(200),		--01/09/10 - CR1092
		@BCPCheck varchar(500),			--01/09/10 - CR1092
		@CheckExists int				--01/09/10 - CR1092
		

SELECT @BCPpath = value + '\BCP' FROM CountryMaintenance
WHERE Codename = 'BCPpath' 

set @return=0
set @containsWarnings = 0	--IP -31/08/10 - CR1092


--IP - 10/08/10 - CR1092 - Create temporary table to hold the filenames to be processed from a bank directory
IF NOT EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES
 		WHERE  table_name = 'SOFilesToProcess')
create table SOFilesToProcess
(
	FileName varchar(100)
)

truncate table StOrderTxn 

Declare @filename varchar(32),
        @BankName varchar(16),	
        @ProcessError varchar(500),
        @rowCount int,				-- IP - 10/08/10 - CR1092 - COASTER to CoSACS Enhancements
        @runDate datetime,			-- IP - 11/08/10 - CR1092 - COASTER to CoSACS Enhancements
		@fileExt varchar(3),		--IP - 20/08/10 - CR1092 - COASTER to CoSACS Enhancements	
		@isDelimited bit,			--IP - 25/08/10 - CR1092 - COASTER to CoSACS Enhancements	
		@delimitedNoOfCols int,		--IP - 27/08/10 - CR1092 - COASTER to CoSACS Enhancements				
		@loop int,					--IP - 27/08/10 - CR1092 - COASTER to CoSACS Enhancements
		@sqltext SQLTEXT			--IP - 27/08/10 - CR1092 - COASTER to CoSACS Enhancements
		
--IP - 11/08/10 - CR1092 - Set RunDate
set @runDate = GETDATE()
 
--IP - 01/09/10 - CR1092
select @BCPPathCheck = value from countrymaintenance WHERE Codename = 'BCPpath' 
set @BCPFileName = '\bcp.exe'
set @BCPCheck = @BCPPathCheck + @BCPFileName

EXEC master..xp_fileexist @BCPCheck, @CheckExists out --Check if the BCP Path is setup correctly.

IF(@CheckExists = 1) --If the BCP Path is correct then continue to process
BEGIN --BCP Path setup ok - Start
	-- Process StorderControl       
	Declare SC cursor for select bankname, [filename], IsDelimited, DelimitedNoOfCols --IP - 25/08/10 - CR1092 - added IsDelimited --IP - 27/08/10 - CR1092 - Added DelimitedNoOfCols
		from stordercontrol 


		Open SC
		Fetch next from SC into @BankName, @fileExt, @isDelimited, @delimitedNoOfCols --IP - 25/08/10 - CR1092 - added IsDelimited --IP - 27/08/10 - CR1092 - Added @delimitedNoOfCols
		While @@Fetch_status = 0
		Begin
	    
	    
		IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.columns  WHERE table_name ='SOFilesToProcess' AND column_name = 'ID')
		BEGIN
			alter table SOFilesToProcess drop column ID
		END
	    
		truncate table SOFilesToProcess
	    
		 --IP - 10/08/10 - CR1092 - Firstly select the files beneath the Bank directory to be processed into a table
		 select 'create dir.text'
		 declare @dirPath varchar(200)
		 --set @dirPath = 'dir /b D:\cosdata\sodata\' + @BankName + '\*.dat' + ' > D:\cosdata\sodata\dir.txt' --IP - 13/08/10 - CR1092 - only look for .txt files
		 set @dirPath = 'dir /b D:\cosdata\sodata\' + @BankName + '\*.' + @fileExt + ' > D:\cosdata\sodata\dir.txt' --IP - 13/08/10 - CR1092 - only look for .txt files
		 exec master.dbo.xp_cmdshell @dirPath
	     
		 select @dirPath AS DIRECTORYPATH
	     
		 select 'Import Dir.txt'
		 set @dirPath = '"' + @BCPpath + '" ' + db_name()+
		'..SOFilesToProcess' + ' in ' +
		'd:\cosdata\sodata\dir.txt ' + '-c -t, -q -T'
		exec master.dbo.xp_cmdshell @dirPath
		
		alter table SOFilesToProcess add ID int IDENTITY
		
		--IP - 12/08/10 - CR1092 - Record the files that are being processed in a permanent table
		INSERT INTO SOFilesProcessed(RunNo,RunDate, Source, [FileName], Processed) --13/08/10 - Added RunDate
		SELECT @RunNo,@runDate, @BankName, [FileName], ''
		FROM SOFilesToProcess
		
		set @rowCount = (select count(*) from SOFilesToProcess)
		
		--IP - 13/08/10 - CR1092 - If no files to process 
		--Insert an error F = No files to process
		IF(@rowCount = 0)
		BEGIN

			INSERT INTO SOException(RunDate,RunNo,Source,[FileName],Record,Error, Field, [LineNo]) --13/08/10 - CR1092 - Added LineNo --16/08/10 - Added Field
			VALUES(@runDate,@RunNo,@BankName,'','No Files found to process for bank','F', '', @rowCount)
			
			SET @ProcessError = 'No files were found for Bank: ' + @BankName 
			
			INSERT INTO interfaceerror(interface,runno,errordate,severity,errortext)
			VALUES('STORDER',@runno,GETDATE(),'W',@ProcessError)
								
		END
		ELSE
		BEGIN --IP - 13/08/10 - CR1092 - Files found therefore process
					
							
				--IP - 10/08/10 - CR1092 - Now proceed to process each file beneath the directory
				--IP - 25/08/10 - CR1092 - Depending on the type of file (delimited or normal) need to execute the correct procedure to process the file.
				if(@isDelimited = 0)
				begin
		
					exec StandingOrderValFixed @BankName = @BankName, @RunNo = @RunNo, @runDate = @runDate, @RowCount = @RowCount, @return = 0
					
				end
				else
				begin
					
					--IP - 27/08/10 - Create table to store the records for a delimted file dynamically based on the Number of columns in the delimited file. 
					-- We need the correct number of columns otherwise the data will not be imported correctly.
					--IP - 27/08/10 - CR1092 - Firstly we need to drop the table if exists
					
					if exists(select * from sys.objects where object_id = OBJECT_ID(N'[StorderDelimited]') AND type in(N'U'))
					begin
						drop table StorderDelimited
					end
					
					create table StorderDelimited
					(
						[Column1] [varchar] (100)
					)
					
					set @loop  = 2
					while @loop <= @delimitedNoOfCols
					begin
						set @sqltext = 'alter table StorderDelimited add column'+cast(@loop as varchar(10)) + '[varchar] (100)'
						EXEC sp_executesql @sqlText
						
						set @loop = @loop + 1
					end
					
					exec StandingOrderValDelimited @BankName = @BankName, @RunNo = @RunNo, @runDate = @runDate, @RowCount = @RowCount, @return = 0
				end
			
				
		END --IP - 13/08/10 - CR1092 - End of processing files

	-- Fetch next Control 
	Fetch next from SC into @BankName, @fileExt,@isDelimited, @delimitedNoOfCols --IP - 25/08/10 - CR1092 - added IsDelimited --IP - 27/08/10 - CR1092 - Added @delimitedNoOfCols

	End
	-- Close & Deallocate 
	Close SC
	Deallocate SC


	--31/08/10 - CR1092 
	if exists(select * from interfaceerror where interface = 'STORDER'
				and runno = @RunNo
				and severity = 'W')
	begin
		set @containsWarnings = 1
	end
	
END --BCP Path setup ok - End
ELSE
BEGIN
		set @ProcessError = 'No files were processed. The BCP Path setup beneath Country Maintenance is incorrect. Please check. Path: ' + @BCPPathCheck

		--Error will automatically be written to interfaceerror when the Raiseerror is executed.
		insert into SOException(RunDate,RunNo,Source,[FileName],Record,Error,Field, [LineNo]) 
					values(@runDate,@RunNo,'','',@ProcessError,'P','', 0)
		
		raiserror (@ProcessError,16,1)
END
			
set @return=@@error

Go

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End 



