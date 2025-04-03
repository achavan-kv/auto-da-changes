---- Author:  Ilyas Parker 
---- Date:    August 2010

--/*    This procedure will load the batch of Standing order transactions into a table and 
--      validate the the batch totals.

--*/
if exists (select * from dbo.sysobjects where id = object_id('[dbo].[StandingOrderValFixed]') 
        and OBJECTPROPERTY(id, 'IsProcedure') = 1)
Drop Procedure .[dbo].[StandingOrderValFixed]
Go
SET QUOTED_IDENTIFIER OFF
Go

Create Procedure .dbo.StandingOrderValFixed
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : StandingOrderValFixed.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : StandingOrderValFixed
-- Author       : Ilyas Parker
-- Date         : 2010
--
-- This procedure will validate the payment files from banks.
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
--19/08/10   ip  CR1092 - COASTER to CoSACS - Batch file processing.
--09/09/10   ip  CR1092 - UAT5.4 - UAT(6) - Check if account number length is (12) for it to be a valid account number.
--13/09/10   ip  CR1092 - UAT5.4 - UAT(8) - If file was not processed then write error to the Interfaceerror table.
--14/09/10   ip  CR1092 - UAT5.4 - UAT(13) - If the account number is all 0's then set as error as Storderprocess table 
--							       has a constraint that prevents an account number with all 0's from being inserted.
--12/10/10   ip  CR1112 - UAT5.4 - UAT(49) - Insert exception into SOException table for missing Batch Header/Trailer
-- ================================================
-- Add the parameters for the stored procedure here

		@BankName varchar(16),
		@RunNo int,
		@RunDate datetime,
		@RowCount int,
		@return int out

AS
	Declare @filename varchar(32),
        @count int,
        @record varchar(999),
        @RecType char(1),
        @Acctno char(12),
        @TxnDate datetime,
        @TxnValue money,
        @TransRefno int,
        @Bank varchar(16),
        @ValError char(1),
        @dateformat varchar(16),
        @acctnobegin smallint,
        @acctnolength Smallint,
        @moneybegin smallint,
        @moneylength smallint,
        @moneypoint smallint,
        @headline smallint,
        @datebegin smallint,
        @datelength smallint,
        @hastrailer int,				-- UAT86 16/04/10
        @trailerbegin smallint,
        @trailerlength smallint,
        @TotalTxnValue money,
        @TrailerTxnValue money,
        @RecCount int,
        @ProcessError varchar(500),
        @recDate varchar(12),				-- IP - 09/08/10 - CR1092 - COASTER to CoSACS Enhancements
        @loop int,							-- IP - 10/08/10 - CR1092 - COASTER to CoSACS Enhancements
        @dataError bit,						-- IP - 11/08/10 - CR1092 - COASTER to CoSACS Enhancements
        @headerIdBegin int,					-- IP - 13/08/10 - CR1092 - COASTER to CoSACS Enhancements
        @headerIdLength int,				-- IP - 13/08/10 - CR1092 - COASTER to CoSACS Enhancements
        @headerId varchar(20),				-- IP - 13/08/10 - CR1092 - COASTER to CoSACS Enhancements
        @trailerIdBegin int,				-- IP - 13/08/10 - CR1092 - COASTER to CoSACS Enhancements
        @trailerIdLength int,				-- IP - 13/08/10 - CR1092 - COASTER to CoSACS Enhancements
        @trailerId varchar(20),				-- IP - 13/08/10 - CR1092 - COASTER to CoSACS Enhancements
        @headerIdentifier varchar(20),		-- IP - 13/08/10 - CR1092 - COASTER to CoSACS Enhancements
        @trailerIdentifier varchar(20),		-- IP - 13/08/10 - CR1092 - COASTER to CoSACS Enhancements
        @fieldError varchar(200),			--IP - 17/08/10 - CR1092 - COASTER to CoSACS Enhancements
        @foundTrailer bit,					--IP - 17/08/10 - CR1092 - COASTER to CoSACS Enhancements
        @IsBatch bit,						--IP - 18/08/10 - CR1092 - COASTER to CoSACS Enhancements
        @batchHeaderIdBegin int,			--IP - 18/08/10 - CR1092 - COASTER to CoSACS Enhancements
		@batchHeaderIdLength int,			--IP - 18/08/10 - CR1092 - COASTER to CoSACS Enhancements
		@batchHeaderId varchar(20),			--IP - 18/08/10 - CR1092 - COASTER to CoSACS Enhancements
		@batchHeaderHasTotal bit,			--IP - 18/08/10 - CR1092 - COASTER to CoSACS Enhancements
		@batchHeaderMoneyBegin int,			--IP - 18/08/10 - CR1092 - COASTER to CoSACS Enhancements
		@batchHeaderMoneyLength int,		--IP - 18/08/10 - CR1092 - COASTER to CoSACS Enhancements
		@batchTrailerIdBegin int,			--IP - 18/08/10 - CR1092 - COASTER to CoSACS Enhancements
		@batchTrailerIdLength int,			--IP - 18/08/10 - CR1092 - COASTER to CoSACS Enhancements
		@batchTrailerId varchar(20),		--IP - 18/08/10 - CR1092 - COASTER to CoSACS Enhancements
		@batchTrailerHasTotal bit,			--IP - 18/08/10 - CR1092 - COASTER to CoSACS Enhancements
		@batchTrailerMoneyBegin int,		--IP - 18/08/10 - CR1092 - COASTER to CoSACS Enhancements
		@batchTrailerMoneyLength int,		--IP - 18/08/10 - CR1092 - COASTER to CoSACS Enhancements
		@batchEnd bit,						--IP - 18/08/10 - CR1092 - COASTER to CoSACS Enhancements
		@batchHeaderIdentifier varchar(20),	--IP - 18/08/10 - CR1092 - COASTER to CoSACS Enhancements
		@batchTrailerIdentifier varchar(20),--IP - 18/08/10 - CR1092 - COASTER to CoSACS Enhancements
		@batchTotalValue money,				--IP - 18/08/10 - CR1092 - COASTER to CoSACS Enhancements
		@batchTotTxnValue money,			--IP - 18/08/10 - CR1092 - COASTER to CoSACS Enhancements
		@prevBatchTotalValue money,			--IP - 18/08/10 - CR1092 - COASTER to CoSACS Enhancements
		@foundBatchHeader bit,				--IP - 18/08/10 - CR1092 - COASTER to CoSACS Enhancements
		@foundBatchTrailer bit,				--IP - 18/08/10 - CR1092 - COASTER to CoSACS Enhancements
		@prevHeaderRecord varchar(999),		--IP - 19/08/10 - CR1092 - COASTER to CoSACS Enhancements
		@firstBatch bit,					--IP - 19/08/10 - CR1092 - COASTER to CoSACS Enhancements	
		@fileExt varchar(3),				--IP - 27/08/10 - CR1092 - COASTER to CoSACS Enhancements	
		@batchNo int,						--IP - 31/08/10 - CR1092 - COASTER to CoSACS Enhancements	
		@monthFileProcessed	int,			--IP - 02/09/10 - CR1092 - COASTER to CoSACS Enhancements	
		@hiRefNo int,						--IP - 06/09/10 - CR1092 - COASTER to CoSACS Enhancements	
		@HOBranch int,						--IP - 06/09/10 - CR1092 - COASTER to CoSACS Enhancements
		@lastFoundHeaderTrailer	char(1),	--IP - 11/10/10 - CR1112 - UAT5.4 - UAT(49)
		@currentFoundHeaderTrailer char(1),	--IP - 11/10/10	- CR1112 - UAT5.4 - UAT(49)	
		@lastFoundHeaderTrailerRecord varchar(999), --IP - 12/10/10 - CR1112 - UAT5.4 - UAT(49)
		@lastFoundHeaderTrailerRecCount int	--IP - 12/10/10 - CR1112 - UAT5.4 - UAT(49)
					
set @return = 0
--IP - 17/08/10 - CR1092
set @foundTrailer = 0
--IP - 18/08/10 - CR1092
set @foundBatchHeader = 0
set @foundBatchTrailer = 0

--18/08/10 - CR1092
set @batchEnd = 0

--IP - 19/08/10 - CR1092 - For batch files need to store the Batch Total from the Header/Trailer
set @prevBatchTotalValue = 0

--IP - 31/08/10 - CR1092 - Variable to identify the batch number in a batch file.
set @batchNo = 0
 
--IP - 06/09/10 - CR1092 -  Get Head Office Branch no.
Set @HOBranch=(select value from countrymaintenance
            where codename='hobranchno')

            
DECLARE @BCPpath VARCHAR(500)

SELECT @BCPpath = value + '\BCP' FROM CountryMaintenance
WHERE Codename = 'BCPpath' 


--IP - 11/08/10 - CR1092 - Create table variable to hold records from the files being processed
declare @StOrderTxn table
(
RecTyp varchar(1),
Acctno char(12),
TxnDate datetime,
TxnValue money,
TransRefno int,
Bank varchar(16),
ValError char(1),
FileName varchar(32),
[LineNo] int
)

--IP - 19/08/10 - CR1092 - Create table variable to hold records from the files being processed (when batch)
declare @StOrderBatchTxn table
(
	Record varchar(999),
	Field varchar(200),
	Error char(1),
    [LineNo] int
)

set nocount ON

--select all the values for the payment file into variables

select @dateformat = [dateformat],
	@fileExt = [filename],
	@acctnobegin = acctnobegin,
    @acctnolength = acctnolength,
    @moneybegin = moneybegin,
    @moneylength = moneylength,
    @moneypoint = moneypoint,
    @headline = headline,
    @datebegin = datebegin,
    @datelength = datelength,
    @hastrailer = hastrailer,		
    @trailerbegin = trailerbegin,
    @trailerlength = trailerlength,
    @headerIdBegin = HeaderIdBegin,						
    @headerIdLength = HeaderIdLength,				
    @headerId = HeaderId,								
    @trailerIdBegin = TrailerIdBegin,					
    @trailerIdLength = TrailerIdLength,					
    @trailerId = TrailerId,								
    @IsBatch = IsBatch,									
    @batchHeaderIdBegin = BatchHeaderIdBegin,			
	@batchHeaderIdLength = BatchHeaderIdLength,			
	@batchHeaderId = BatchHeaderId,						
	@batchHeaderHasTotal = BatchHeaderHasTotal,			
	@batchHeaderMoneyBegin = BatchHeaderMoneyBegin,		
	@batchHeaderMoneyLength = BatchHeaderMoneyLength,	
	@batchTrailerIdBegin = BatchTrailerIdBegin,			
	@batchTrailerIdLength = BatchTrailerIdLength,		
	@batchTrailerId = BatchTrailerId,					
	@batchTrailerHasTotal = BatchTrailerHasTotal,		
	@batchTrailerMoneyBegin = BatchTrailerMoneyBegin,	
	@batchTrailerMoneyLength = BatchTrailerMoneyLength
  
from stordercontrol
where bankname = @BankName
	
	--IP - 10/08/10 - CR1092 - Now proceed to process each file beneath the directory
			set @loop = 1
			while @loop <= @rowCount
			begin --IP - 10/08/10 - CR1092
				
				delete @StOrderTxn	--IP - 11/08/10 - CR1092 - Delete before starting to process the next file.
				truncate table StOrderTxn	--IP - 11/08/10 - CR1092
				
					select @filename = [filename] from SOFilesToProcess where ID = @loop	--IP - 10/08/10 - CR1092 - Set the filename to the file to be processed
					
					set @monthFileProcessed = (select cast(datepart(mm,max(rundate))as int) 
												from SOFilesProcessed 
												where [filename] = @filename
												and SOFilesProcessed.Processed='Y') --IP - 02/09/10 - CR1092
					
					--IP 06/09/10 - CR1092 - Get the hirefno from the head office branch which will be used to update the accounts transrefnos.
					select @hiRefNo = hirefno from branch where branchno = @HOBranch    
					
					--IP - 13/08/10 - CR1092 - File already processed this month
				IF NOT EXISTS(select * from SOFilesProcessed s
								where s.[FileName] = @filename
								and s.Processed = 'Y'
								and @monthFileProcessed = cast(DATEPART(mm, @rundate) as int))	--IP - 02/09/10 - CR1092
				BEGIN-- IP - 13/08/10 - CR1092 - Check if file processed
					
					-- initialise totals
						set @dataError = 0	
						set @TotalTxnValue=0
						set @TrailerTxnValue=0
						set @RecCount=0
						set @batchNo = 0 --IP - 06/09/10 - CR1092
						set @foundTrailer = 0 --IP - 06/09/10 - CR1092
						set @prevBatchTotalValue = 0 --IP - 06/09/10 - CR1092
						set @firstBatch = 0 --IP - 06/09/10 - CR1092
						set @lastFoundHeaderTrailer = 'X' --IP - 11/10/10 - CR1112 - UAT5.4 - UAT(49) - Set to 'X'. If there is no Header for the first batch this should remain set to 'X', therefore we know first Batch Header is missing.
						set @currentFoundHeaderTrailer = 'X' --IP - 11/10/10 - CR1112 - UAT5.4 - UAT(49)
						set @lastFoundHeaderTrailerRecCount = 0 --IP - 12/10/10 - CR1112 - UAT5.4 - UAT(49)
			
					TRUNCATE TABLE .dbo.Storder

					-- Import standing order data file
					declare @path varchar(200)
					
					--IP - 27/08/10 - CR1092 - If the file has the extension .OPM then need to use the hexadecimal value in the switches for LF
					
					if(@fileExt = 'OPM')
					begin
						set @path = '"' + @BCPpath + '" ' + db_name()+
							'.dbo.Storder' + ' in ' +
						 'd:\cosdata\sodata\' + @BankName+ '\'+@filename + ' -t -c -r0x0A -q -Usa -P'	--IP - 10/08/10 - CR1092

						select 'Import data file'	
						exec master.dbo.xp_cmdshell @path
					end
					else
					begin
						set @path = '"' + @BCPpath + '" ' + db_name()+
							'.dbo.Storder' + ' in ' +
						 'd:\cosdata\sodata\' + @BankName+ '\'+@filename + ' -c -r\n -q -Usa -P'	--IP - 10/08/10 - CR1092
						select @path AS PATH
						select 'Import data file'	
						exec master.dbo.xp_cmdshell @path
					end
					

					-- AA prevent error if white space at end of file. 
					--DELETE FROM Storder WHERE ISNULL(record,'')='' --IP - 26/08/10 - These lines should be logged as 'E' and not processed, previously deleting these lines gave incorrect lineno in exception table.
					
					set @count=(select count(*) from Storder)
					If @count > 0
					begin
					
					--IP - 13/08/10 - CR1092 - Insert a record into exception table if file successfully imported
					--S = Successfull.
					INSERT INTO SOException(RunDate,RunNo,Source,[FileName],Record,Error,Field, [LineNo]) --IP - 13/08/10 - CR1092 - Added LineNo --16/08/10 - Added Field
					VALUES(@runDate,@RunNo,@BankName,@filename,'File successfully imported for bank','S','', 0)
										
					select convert(varchar(5),@count) + ' Records to Process for bank: '+ @bankname + ' ,filename: ' + @filename	--IP - 10/08/10 - CR1092

						Declare SO cursor for select isnull(Record,'') from Storder
						Open SO
						Fetch next from SO into @Record
						While @@Fetch_status = 0
							Begin
								
								----IP - 18/08/10 - CR1092 - Initialise for each record fetched
								set @batchEnd = 0	
								set @foundBatchHeader = 0
								set @foundBatchTrailer = 0
								set @currentFoundHeaderTrailer = '' --IP - 12/10/10 - CR1112 - UAT5.4 - UAT(49)
								
								--IP - 13/08/10 - CR1092 - If the Header has not been identified then do not process
								--and insert a data error H = Header error
								IF(@headline = 1 and @RecCount = 0)
								BEGIN
									set @headerIdentifier = UPPER(SUBSTRING(@Record, @headerIdBegin, @headerIdLength))
									
									IF(@headerIdentifier != @headerId)
									BEGIN
										set @dataError = 1
										set @fieldError = 'No Header was found. The Header Identifier expected: ' + @headerIdentifier --17/08/10 - CR1092 
										
										INSERT INTO SOException(RunDate,RunNo,Source,[FileName],Record,Error,Field, [LineNo]) --IP - 13/08/10 - CR1092 - Added LineNo --16/08/10 - Added Field
										VALUES(@runDate,@RunNo,@BankName,@filename,@record,'H',@fieldError, @RecCount+1)
								
									END
								END
							
							set @RecCount=@RecCount+1
							set @ValError=' '
							
							set @trailerIdentifier = UPPER(SUBSTRING(@Record, @trailerIdBegin, @trailerIdLength)) --IP - 17/08/10 - CR1092
							
							--IP - 18/08/10 - CR1092
							if(@IsBatch = 1)
							begin
									
									--Select text into the below variables which will be used to identify if this record is the Batch Header/Trailer
									--against the actual identifiers set from the payment file definition.
									set @batchHeaderIdentifier = UPPER(SUBSTRING(@record, @batchHeaderIdBegin, @batchHeaderIdLength))
									set @batchTrailerIdentifier = UPPER(SUBSTRING(@record, @batchTrailerIdBegin, @batchTrailerIdLength))
									
									--If the Batch Header has been identified
									if(@batchHeaderIdentifier = @batchHeaderId) --IP - 03/09/10
									begin
										set @foundBatchHeader = 1 --Bool to indicate that the Batch Header has been found.
										
										set @currentFoundHeaderTrailer = 'H' --IP - 11/10/10 - CR1112 - UAT5.4 - UAT(49)
											
										if(@batchHeaderHasTotal = 1) --If the Batch Header contains the totals for the batch
										begin
										
											set @batchEnd = 1 --Set bool to indicate the end of a batch (Batch Header to next Batch Header)
											
											set @batchTotalValue = SUBSTRING(@record, @batchHeaderMoneyBegin, @batchHeaderMoneyLength)
											-- divide value by 100 if no decimal (1=decimal, 0=no decimal)       
											If @moneypoint=0					
											set @batchTotalValue=@batchTotalValue/100
											
											if(@firstBatch > 0) --If this is not the first batch header
											begin
												set @batchNo = @batchNo + 1 --IP - 31/08/10 - CR1092
												
												if(ISNULL(@prevBatchTotalValue,0) <> @batchTotTxnValue)
												begin
													
													set @fieldError = @prevBatchTotalValue --IP - 17/08/10 - CR1092
													
													--	--IP - 19/08/10 - CR1092 - Batch was successfull.
													--INSERT INTO SOException(RunDate,RunNo,Source,[FileName],Record,Error, Field,[LineNo]) --IP - 13/08/10 - CR1092 - Added LineNo -- 16/08/10 - Added Field
													--VALUES(@runDate,@RunNo,@BankName,@filename,@prevHeaderRecord,'B','Batch unsuccessfull Total: ' + @prevBatchTotalValue, @RecCount)
						
													--IP - 19/08/10 - CR1092 
													INSERT INTO SOException(RunDate,RunNo,[Source],[FileName],Record,Error, Field,[LineNo]) --IP - 13/08/10 - CR1092 - Added LineNo -- 16/08/10 - Added Field
													select @runDate,@RunNo,@BankName,@filename,s.record,s.Error,s.Field, s.[LineNo]
													from @StOrderBatchTxn s
													
													--IP - 31/08/10 - CR1092
													set @ProcessError = 'Batch No: ' +cast(@batchNo as varchar(10)) + ' for file: ' + @filename + ' for Bank: ' + @BankName 
													+ '. Header Total does not match the transaction totals'
													
								
													insert into interfaceerror(interface,runno,errordate,severity,errortext)
													values('STORDER',@runno,GETDATE(),'W',@ProcessError)
													
												end
												else
												begin
													--IP - 19/08/10 - CR1092 - Batch was successfull.
													INSERT INTO SOException(RunDate,RunNo,[Source],[FileName],Record,Error, Field,[LineNo]) --IP - 13/08/10 - CR1092 - Added LineNo -- 16/08/10 - Added Field
													VALUES(@runDate,@RunNo,@BankName,@filename,@prevHeaderRecord,'B','Batch processed successfully', @RecCount)
												end
											end
										
											
											set @prevBatchTotalValue = @batchTotalValue --Holds the Batch Total so to compare this total with the transactions total for the batch
											set @prevHeaderRecord = @record	--IP - 19/08/10 - CR1092 - Hold the record, so if the Batch totals do not match then output this record to SOException
											
											--IP - 19/08/10 - CR1092 - Initialise to 0 when next Batch Header found.
											set @TotalTxnValue = 0
											set @batchTotTxnValue = 0
											delete from @StOrderBatchTxn	--IP - 19/08/10 - CR1092 - Delete from this table for next batch
											
											insert into @StOrderBatchTxn(Record, Field, Error, [LineNo])
											select @record, 'Batch unsuccessfull Total:' + cast(@prevBatchTotalValue as varchar(200)), 'B', @RecCount

											set @firstBatch = 1 --IP - 19/08/10 - CR1092
										end
									end
									
									
									if(@batchTrailerIdentifier = @batchTrailerId)
									begin
									
										set @foundBatchTrailer = 1
										
										set @currentFoundHeaderTrailer = 'T'	--IP - 11/10/10 - CR1112 - UAT5.4 - UAT(49)
										
										if(@batchTrailerHasTotal = 1)
										begin
											set @batchEnd = 1
											set @batchTotalValue = SUBSTRING(@record, @batchTrailerMoneyBegin, @batchTrailerMoneyLength)
										end
									end
									
							end	
							
								--    If file contains header skip fist record 
								If (@headline=1 and @RecCount>1) or @headline=0
					                
								Begin
									
								--    If records processed < total records in file - process detail                                                                      
								if (((@trailerIdentifier!= @trailerId and @foundTrailer = 0)--IP - 17/08/10 - CR1092 - If there is a Trailer and we have not reached the Trailer
									or @hastrailer=0)and @foundBatchTrailer = 0 and @foundBatchHeader = 0)	-- UAT86 16/04/10  -- No trailer  --IP - 19/08/10 - CR1092 - Do not process for Batch Header/Trailer
								
								--    Detail Record       
								Begin
									
									set @rectype='D'
									
									--IP - 11/08/10 - CR1092 - Validate the account number is numeric
									if(ISNUMERIC(replace(substring(@Record,@acctnobegin,@AcctnoLength),'-',''))=1 and LEN(replace(substring(@Record,@acctnobegin,@AcctnoLength),'-','')) = 12
									AND replace(substring(@Record,@acctnobegin,@AcctnoLength),'-','')!='000000000000') --IP - 09/09/10 - CR1092 UAT5.4 UAT(6) --IP - 14/09/10 - CR1092 - UAT5.4 - UAT(13)
									begin
										set @Acctno=replace(substring(@Record,@acctnobegin,@AcctnoLength),'-','') --IP - 10/08/10 - CR1092 - Remove hyphens from account number.
									end
									else
									begin
									
										--Data error A = Data error with Account Number
										--set @dataError = 1 --18/08/10 - CR1092 - Do not set
										
										--IP - 17/08/10 - CR1092
										IF(substring(@Record,@acctnobegin,@AcctnoLength) = '' or substring(@Record,@acctnobegin,@AcctnoLength) is null)
										begin
											set @Acctno = ''
											set @fieldError = 'No account number entered'
										end
										else
										begin
											set @Acctno = ''
											set @fieldError = 'Account number not in correct format: ' + substring(@Record,@acctnobegin,@AcctnoLength)
										end
										
										INSERT INTO SOException(RunDate,RunNo,Source,[FileName],Record,Error,Field, [LineNo]) --IP - 13/08/10 - CR1092 - Added LineNo --16/08/10 - Added Field
										VALUES(@runDate,@RunNo,@BankName,@filename,@record,'A',@fieldError, @RecCount)
										
										set @ValError = 'E' --IP - 18/08/10 - CR1092
										
									end
					                
									set @recDate = isnull(SUBSTRING(@Record, @datebegin, @dateLength),'')	--IP - 09/08/10 - CR1092 - Select the transaction date
					                
					                if(@recDate ='') --IP - 26/08/10 - If no transaction date entered then set as the rundate.
					                begin
										set  @TxnDate = convert(char(8),@runDate,112)
										
										SET @fieldError = 'No transaction date supplied. The transaction will be processsed with todays date.'
											
										INSERT INTO SOException(RunDate,RunNo,Source,[FileName],Record,Error,Field, [LineNo])	--IP - 13/08/10 - CR1092 - Added LineNo --16/08/10 - Added Field
										VALUES(@runDate,@RunNo,@BankName,@filename,@record,'D',@fieldError , @RecCount)
										
					                end
					                else
					                begin
										--If the dateformat begins with the day then firstly need to reverse the date so that it is in year month date format.
										IF  SUBSTRING(@dateformat,1,1) = 'd'
										BEGIN
											if(@datelength = 6)
											begin
												set @recDate = (SUBSTRING(@recDate,5,2) + substring(@recDate,3,2 ) + SUBSTRING(@recDate,1,2))
											end
											if(@datelength = 8)
											begin
												if(@dateformat = 'ddmmyyyy') --Need to check specifically for this format as datelength could be 8 with format 'dd-mm-yy' meaning the position of the dates change.
												begin
													set @recDate = (SUBSTRING(@recDate,5,4) + substring(@recDate,3,2 ) + SUBSTRING(@recDate,1,2))
												end
												else
												begin
													set @recDate = (SUBSTRING(@recDate,7,2) + substring(@recDate,4,2 ) + SUBSTRING(@recDate,1,2))
												end
											end
											if(@datelength = 10)
											begin
												set @recDate = (SUBSTRING(@recDate,7,4) + substring(@recDate,4,2 ) + SUBSTRING(@recDate,1,2))
											end
										END
							
						                
										set @recDate = REPLACE(@recDate, '-','')
										set @recDate = REPLACE(@recDate, '/','')
										
									
										BEGIN TRY --IP - 11/08/10 - CR1072 - Catch data exceptions on the transaction date
											--Set the date of the transaction to be in the Year Month Day format.
											If @datelength = 6
											BEGIN
												set @TxnDate = convert(char(6),@recDate,112)
											END
											ELSE IF @datelength = 8
											BEGIN
												set @TxnDate = convert(char(8),@recDate,112)
											END
											ELSE IF @datelength = 10
											BEGIN
												set @TxnDate = convert(char(8),@recDate,112)
											END
											
										END TRY
										BEGIN CATCH 
										
											--Data error D = Data error with Date
											--set @dataError = 1	--IP - 11/08/10 - CR1092
											
											--IP - 16/08/10 - CR1092 - If there is a problem with the date then set to todays date and write a record to the exception table
											--but still continue to process.
											SET @TxnDate =  convert(char(8),@runDate,112)
											SET @fieldError = 'Error with date supplied: '+ SUBSTRING(@Record, @datebegin, @dateLength)+ ' The transaction will be processsed with todays date' --IP - 17/08/10 - CR1092
											
											INSERT INTO SOException(RunDate,RunNo,Source,[FileName],Record,Error,Field, [LineNo])	--IP - 13/08/10 - CR1092 - Added LineNo --16/08/10 - Added Field
											VALUES(@runDate,@RunNo,@BankName,@filename,@record,'D', @fieldError , @RecCount)
										
										END CATCH
									end
									-- check date within last 100 days
									If (@TxnDate < getdate()-100 and @ValError!='E') --IP - 18/08/10 - CR1092
									begin
										set @ValError='D'
									end
									else if(@TxnDate > dateadd(day,(10),getdate())) --IP - 01/09/10 - CR1092 - Previously transaction dates > dateadd(day, 10, getdate()) would cause a check constraint error when inserting into Fintrans
									begin
										set @ValError='D'
										set @TxnDate = @RunDate
										
										SET @fieldError =  'Error with date supplied: '+ SUBSTRING(@Record, @datebegin, @dateLength)+ ' .The date is greater than 10 days in the future. The transaction will be processsed with todays date' 
										
										INSERT INTO SOException(RunDate,RunNo,Source,[FileName],Record,Error,Field, [LineNo])	--IP - 13/08/10 - CR1092 - Added LineNo --16/08/10 - Added Field
										VALUES(@runDate,@RunNo,@BankName,@filename,@record,'D', @fieldError , @RecCount)
									end
									
									--IP - 11/08/10 - CR1092 - Validation on Money
									BEGIN TRY --IP - 31/08/10 - CR1092 - Added BEGIN TRY/CATCH
									if(ISNUMERIC(replace(replace(substring(@Record,@moneybegin,@moneyLength),' ',''),'-',''))=1) --IP - 01/09/10 - CR1092 - some files have transvalue (signed) therefore remove the sign
									begin
										set @Txnvalue=convert(money,replace(replace(substring(@Record,@moneybegin,@moneyLength),' ',''),'-','')) --IP - 01/09/10 - CR1092
									end
									else
									begin
										
										--Data error M = Data error with money
										--set @dataError = 1 --IP - 18/08/10 - CR1092 - Do not set
										set @TxnValue = 0
										set @fieldError = 'Incorrect or no transaction value encountered: '+ substring(@Record,@moneybegin,@moneyLength) --IP - 17/08/10 - CR1092
										
										INSERT INTO SOException(RunDate,RunNo,Source,[FileName],Record,Error,Field,[LineNo])	--IP - 13/08/10 - CR1092 - Added LineNo --16/08/10 - Added Field
										VALUES(@runDate,@RunNo,@BankName,@filename,@record,'M',@fieldError, @RecCount)
										
										set @ValError = 'E' --IP - 18/08/10 - CR1092
									end
									END TRY
									BEGIN CATCH
											set @TxnValue = 0
											
											set @fieldError = 'Incorrect or no transaction value encountered: '+ substring(@Record,@moneybegin,@moneyLength) 
											
											INSERT INTO SOException(RunDate,RunNo,Source,[FileName],Record,Error,Field,[LineNo])	
											VALUES(@runDate,@RunNo,@BankName,@filename,@record,'M',@fieldError, @RecCount)
											
											set @ValError = 'E' 
											
									END CATCH

									-- divide value by 100 if no decimal (1=decimal, 0=no decimal)       
									If @moneypoint=0					
										set @Txnvalue=@Txnvalue/100
									set @TotalTxnValue=@TotalTxnValue+@Txnvalue
									
									
									--IP - 18/08/10 - CR1092
									if(@IsBatch = 1)
									begin
										set @batchTotTxnValue = @TotalTxnValue
										
										--IP - 19/08/10 - CR1092 - Store records temporaily for a batch
										--and if the batch failed due to totals not matching the batch total then write records to SOException
										insert into @StOrderBatchTxn(Record, Field, Error, [LineNo])
										select @record, cast(@Txnvalue as varchar(200)), 'X', @RecCount
										
									end
									-- get nexr reference no
									--exec @TransRefNo=nexttransrefno @HOBranch, 0 --IP - 11/08/10 - CR1092 - Transrefno updated at the end in proc: DN_StandingOrderVal.sql
									set @TransRefno = 0
									set @Bank=@BankName

									-- Add record to StOrderTxn table
									--IP - 11/08/10 - CR1092 - Insert into table variable first, then permanent table later if no errors encountered in the data.
									insert into @StOrderTxn (RecTyp,Acctno,TxnDate,TxnValue,TransRefno,Bank,ValError, [FileName], [LineNo]) --13/08/10 - CR1092 - Added FileName, LineNo
									select @RecType,@Acctno,@TxnDate,@TxnValue,@TransRefno,@Bankname,@ValError, @filename, @RecCount
									--insert into StOrderTxn (RecTyp,Acctno,TxnDate,TxnValue,TransRefno,Bank,ValError)
									--select @RecType,@Acctno,@TxnDate,@TxnValue,@TransRefno,@Bankname,@ValError
									
							   End
								--Else	-- UAT86 16/04/10 

										--IP - 12/10/10 - CR1112 - UAT5.4 - UAT(49)
								IF(@IsBatch = 1)
								begin
									
									--Missing Batch Header
									if((@lastFoundHeaderTrailer = 'X' or @lastFoundHeaderTrailer = 'T') and @currentFoundHeaderTrailer = 'T' and @foundBatchTrailer = 1)
									begin

											set @fieldError = 'No Header was found for batch trailer. The Header Identifier expected: ' + @batchHeaderId 
											
											INSERT INTO SOException(RunDate,RunNo,Source,[FileName],Record,Error,Field, [LineNo]) 
											VALUES(@runDate,@RunNo,@BankName,@filename,@record,'H',@fieldError, @RecCount)
									end
									
									--Missing Batch Trailer
									if(@lastFoundHeaderTrailer = 'H' and @currentFoundHeaderTrailer = 'H' and @foundBatchHeader = 1)
									begin
		
										set @fieldError = 'No Trailer was found for batch header. The Trailer Identifier expected: ' + @batchTrailerId 
											
											INSERT INTO SOException(RunDate,RunNo,Source,[FileName],Record,Error,Field, [LineNo]) 
											VALUES(@runDate,@RunNo,@BankName,@filename,@lastFoundHeaderTrailerRecord,'T',@fieldError, @lastFoundHeaderTrailerRecCount)
									end
									
									--IP - 12/10/10 - CR1112 - UAT5.4 - UAT(49)
									if(@currentFoundHeaderTrailer = 'H')
									begin

										set @lastFoundHeaderTrailer = 'H' 	
										set @lastFoundHeaderTrailerRecord = @record
										set @lastFoundHeaderTrailerRecCount = @RecCount 
										
									end
									
									if(@currentFoundHeaderTrailer = 'T')
									begin
									
										set @lastFoundHeaderTrailer = 'T'
								
									end
									
								end
							   Begin
								--    Trailer Record
							   --If @RecCount=@Count --IP - 17/08/10 - CR1092 - Commented out
							   If (@hastrailer=1 and @trailerIdentifier = @trailerId) --IP - 17/08/10 - CR1092 - We have identified the Trailer
								Begin
									set @foundTrailer = 1	--IP - 17/08/10 - CR1092 - There is a trailer
									
									set @rectype='T'
									set @Acctno=null
									set @TxnDate=null
									set @Txnvalue=case		-- UAT86 16/04/10
										when @hastrailer = 1 and @trailerlength > 0 then convert(money,substring(@Record,@trailerbegin,@trailerLength)) --IP - 11/08/10 - CR1092
										else @TotalTxnValue
										end
									-- divide value by 100 if no decimal        
									If @moneypoint=0
										and @hastrailer=1		-- UAT86 23/04/10  -- Has trailer
										set @Txnvalue=@Txnvalue/100
									set @TransRefNo=9999999
									set @Bank=@BankName
									set @TrailerTxnValue=@Txnvalue
									-- Add record to StOrderTxn table
									
									--IP - 11/08/10 - CR1092 - Insert into temporary table first, then permanent table later if no errors encountered.
									insert into @StOrderTxn (RecTyp,Acctno,TxnDate,TxnValue,TransRefno,Bank, [FileName], [LineNo]) --13/08/10 - CR1092 - Added FileName, LineNo
									select @RecType,@Acctno,@TxnDate,@TxnValue,@TransRefno,@Bankname, @filename, @RecCount
									--insert into StOrderTxn (RecTyp,Acctno,TxnDate,TxnValue,TransRefno,Bank)
									--select @RecType,@Acctno,@TxnDate,@TxnValue,@TransRefno,@Bankname

									--IP - 17/08/10 - CR1092 - Check if the Trailer Total matches the record totals.
									If @TotalTxnValue <> @TrailerTxnValue and @trailerlength > 0 --IP - 27/08/10 - CR1092 - Only check if trailer money length specified in definition.
									Begin
											
										  --IP - 11/08/10 - CR1092
										  --Data error T = Data error with Trailer  
										  set @dataError = 1	--IP - 11/08/10 - CR1092  
										  set @fieldError = 'Trailer and transaction totals do not match: ' + substring(@Record,@trailerbegin,@trailerLength) --IP - 17/08/10 - CR1092
										  
										 INSERT INTO SOException(RunDate,RunNo,Source,[FileName],Record,Error, Field,[LineNo]) --IP - 13/08/10 - CR1092 - Added LineNo -- 16/08/10 - Added Field
										 VALUES(@runDate,@RunNo,@BankName,@filename,@record,'T',@fieldError, @RecCount)
										 
										-- SET @ProcessError = 'File: ' + @filename + ' for Bank: ' + @BankName + ' could not be processed due to Header/Trailer related errors'
						
										--INSERT INTO interfaceerror(interface,runno,errordate,severity,errortext)
										--VALUES('STORDER',@runno,GETDATE(),'W',@ProcessError)
									   
									End
						
								End
									
							   End
							End 
							
					-- Fetch next record

								Fetch next from SO into @Record
							
								--IP - 19/08/10 - CR1092 - If this was the last record in a Batch
								--then need to check if the Header totals match the total transactions for the batch
								IF @@Fetch_status != 0 AND @IsBatch = 1
								BEGIN
									if(@prevBatchTotalValue <> @batchTotTxnValue)
									begin
										SELECT @prevBatchTotalValue AS PREVIOUSBATCH
										select @batchTotTxnValue AS BATCHTOTAL
										
										set @fieldError = @prevBatchTotalValue --IP - 17/08/10 - CR1092
										
										--INSERT INTO SOException(RunDate,RunNo,Source,[FileName],Record,Error, Field,[LineNo]) --IP - 13/08/10 - CR1092 - Added LineNo -- 16/08/10 - Added Field
										--VALUES(@runDate,@RunNo,@BankName,@filename,@prevHeaderRecord,'H','Batch Unsuccessfull - Total:' + @fieldError, @RecCount)
								   
										INSERT INTO SOException(RunDate,RunNo,Source,[FileName],Record,Error, Field,[LineNo]) --IP - 13/08/10 - CR1092 - Added LineNo -- 16/08/10 - Added Field
													select @runDate,@RunNo,@BankName,@filename,s.record,s.Error,s.Field, s.[LineNo]
													from @StOrderBatchTxn s	
									end
									else
									begin
											--IP - 19/08/10 - CR1092 - Batch was successfull.
													INSERT INTO SOException(RunDate,RunNo,[Source],[FileName],Record,Error, Field,[LineNo]) --IP - 13/08/10 - CR1092 - Added LineNo -- 16/08/10 - Added Field
													VALUES(@runDate,@RunNo,@BankName,@filename,@prevHeaderRecord,'B','Batch processed successfully', @RecCount)
									end
								END
								
							 end
					-- Close & Deallocate 
					Close SO
					Deallocate SO
					
					--IP - 17/08/10 - CR1092 - CR1092 - If the Trailer has not been identified then do not process
							--and insert a data error T = Trailer error
							IF(@hastrailer = 1 and @foundTrailer = 0)
							BEGIN

										set @dataError = 1
										set @fieldError = 'No trailer found' --IP - 17/08/10 - CR1092
										
										INSERT INTO SOException(RunDate,RunNo,Source,[FileName],Record,Error,Field, [LineNo]) --IP - 13/08/10 - CR1092 - Added LineNo --16/08/10 - Added Field
										VALUES(@runDate,@RunNo,@BankName,@filename,'','T',@fieldError, 0)
										
							END
							
					-- Check totals
						--IP - 17/08/10 - CR1092 - Moved to above
						--If @TotalTxnValue <> @TrailerTxnValue 
						--	Begin
						--		select @TotalTxnValue AS T1
						--		SELECT @TrailerTxnValue AS T2
						--		  --IP - 11/08/10 - CR1092
						--		  --Data error T = Data error with Trailer  
						--		  set @dataError = 1	--IP - 11/08/10 - CR1092  
						--		  set @fieldError = substring(@Record,@trailerbegin,@trailerLength) --IP - 17/08/10 - CR1092
								  
						--		 INSERT INTO SOException(RunDate,RunNo,Source,[FileName],Record,Error, Field,[LineNo]) --IP - 13/08/10 - CR1092 - Added LineNo -- 16/08/10 - Added Field
						--		 VALUES(@runDate,@RunNo,@BankName,@filename,@record,'T',@fieldError, @RecCount)
							   
						--	End
						
						--IP - 11/08/10 - CR1092 - If no errors were encountered in the data then insert data into permanent table 		
						IF(@dataError = 0)
						begin
							
							insert into StOrderTxn (RecTyp,Acctno,TxnDate,TxnValue,TransRefno,Bank,ValError, [FileName], [LineNo]) --13/08/10 - CR1092 - Added FileName, LineNo
							select RecTyp,Acctno,TxnDate,TxnValue,TransRefno,Bank,ValError, [FileName], [LineNo]
							from @StOrderTxn
							
							--12/08/10 - CR1092 - File has been imported and no data errors found, therefore update processed = 'Y'
							Update SOFilesProcessed
							set processed = 'Y' 
							where FileName = @filename 
							and runno = @RunNo
							

							--IP - 06/09/10 - CR1092 - Moved from procedure DN_StandingOrderVal as should be processed for each file.
							if ((select count(*) from StOrderTxn) > 0) --IP - 13/08/10 - CR1092 - Only update if there are records in StorderTxn
							begin
								update StOrderTxn
								set TransRefno = @hiRefNo + id
								where RecTyp = 'D'
								and ValError!='E'

								--Update the branch table with the max transrefno used.
								update branch
								set hirefno = (select MAX(transrefno) from StOrderTxn
												 where RecTyp = 'D')
								where branchno = @HOBranch
							end
							
							--IP - 06/09/10 - CR1092 - process data for each file. Moved from DN_StandingOrderVal
							exec DN_StandingOrderProcess @RunNo, @BankName,@filename	--IP - 06/09/10 - CR1092 - Pass in BankName and filename
							
						end
						
					
					End 
					else
					Begin
					
						--IP - 10/08/10 - CR1092
						--Data error N = No records to process 
						set @dataError = 1
						        
								 INSERT INTO SOException(RunDate,RunNo,Source,[FileName],Record,Error,Field, [LineNo]) --IP - 13/08/10 - CR1092 - Added LineNo --16/08/10 - Added Field
								 VALUES(@runDate,@RunNo,@BankName,@filename,'No records found to process for bank','F','', 0)
					End
					
					--IP - 11/08/10 - CR1092 - If a data error was encountered in the file, then write an entry into the interfaceerror.
					IF @dataError = 1
					BEGIN
							
							--12/08/10 - CR1092 - File has been imported and data errors found, therefore update processed = 'N'
							Update SOFilesProcessed
							set processed = 'N' 
							where FileName = @filename 
							and runno = @RunNo
							
							--IP - 13/09/10 - CR1092 - UAT5.4 - UAT(8)
							SET @ProcessError = 'File: ' + @filename + ' for Bank: ' + @BankName + ' could not be processed due to Header/Trailer or other file related errors'
						
							INSERT INTO interfaceerror(interface,runno,errordate,severity,errortext)
							VALUES('STORDER',@runno,GETDATE(),'W',@ProcessError)
							
					END
				END--IP - 13/08/10 - CR1092 - File already processed this month
				ELSE
				BEGIN
					
					INSERT INTO SOException(RunDate,RunNo,Source,[FileName],Record,Error, Field,[LineNo]) --IP - 13/08/10 - CR1092 - Added LineNo --16/08/10 - Added Field
					VALUES(@runDate,@RunNo,@BankName,@filename,'File has already been processed for bank this month','F','', 0)
		
					SET @ProcessError = 'File: ' + @filename + ' for Bank: ' + @BankName + ' has already been processed this month'
		
					INSERT INTO interfaceerror(interface,runno,errordate,severity,errortext)
					VALUES('STORDER',@runno,GETDATE(),'W',@ProcessError)
					
					Update SOFilesProcessed
							set processed = 'N' 
							where FileName = @filename 
							and runno = @RunNo
					
				END
				set @loop = @loop + 1  --IP - 10/08/10 - CR1092

			END --IP - 10/08/10 - CR1092
			
set @return=@@error

Go