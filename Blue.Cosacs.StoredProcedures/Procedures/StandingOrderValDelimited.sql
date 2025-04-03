---- Author:  Ilyas Parker 
---- Date:    August 2010

--/*    This procedure will load the batch of Standing order transactions into a table and 
--      validate the the batch totals.

--*/
if exists (select * from dbo.sysobjects where id = object_id('[dbo].[StandingOrderValDelimited]') 
        and OBJECTPROPERTY(id, 'IsProcedure') = 1)
Drop Procedure .[dbo].StandingOrderValDelimited
Go
SET QUOTED_IDENTIFIER OFF
Go

Create Procedure .dbo.StandingOrderValDelimited
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : StandingOrderValDelimited.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : StandingOrderValDelimited
-- Author       : Ilyas Parker
-- Date         : 2010
--
-- This procedure will validate a delimited payment file from a bank.
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 26/08/10  IP  CR1092 - COASTER to CoSACS Enhancements - created procedure
-- 09/09/10   ip  CR1092 - UAT5.4 - UAT(6) - Check if account number length is (12) for it to be a valid account number.
-- 14/09/10   ip  CR1092 - UAT5.4 - UAT(13) - If the account number is all 0's then set as error as Storderprocess table 
--							       has a constraint that prevents an account number with all 0's from being inserted.
-- ================================================
-- Add the parameters for the stored procedure here
		@BankName varchar(16),
		@RunNo int,
		@RunDate datetime,
		@RowCount int, --This is the count of the number of files to process for a bank.
		@return int out

AS
	Declare @filename varchar(32),
        @count int,
        @storderCount int,					--IP - 26/08/10 - CR1092 - COASTER to CoSACS Enhancements
        @record varchar(999),
        @RecType char(1),
        @Acctno char(12),
        @TxnDate datetime,
        @TxnValue money,
        @TransRefno int,
        @Bank varchar(16),
        @ValError char(1),
        @dateformat varchar(16),
        @datelength smallint,
        @moneypoint smallint,
        @headline smallint,
        @hastrailer int,				
        @RecCount int,
        @ProcessError varchar(500),
        @recDate varchar(12),				-- IP - 09/08/10 - CR1092 - COASTER to CoSACS Enhancements
        @loop int,							-- IP - 10/08/10 - CR1092 - COASTER to CoSACS Enhancements
        @fieldError varchar(200),			--IP - 17/08/10 - CR1092 - COASTER to CoSACS Enhancements
        @delimiter varchar(15),				--IP - 25/08/10 - CR1092 - COASTER to CoSACS Enhancements
		@delimitedNoOfCols smallint,		--IP - 25/08/10 - CR1092 - COASTER to CoSACS Enhancements	
		@delimitedAcctNoColNo varchar(10),	--IP - 25/08/10 - CR1092 - COASTER to CoSACS Enhancements
		@delimitedDateColNo varchar(10),	--IP - 25/08/10 - CR1092 - COASTER to CoSACS Enhancements
		@delimitedMoneyColNo varchar(10),	--IP - 25/08/10 - CR1092 - COASTER to CoSACS Enhancements
		@delimiterHexVal varchar(10),		--IP - 25/08/10 - CR1092 - COASTER to CoSACS Enhancements
		@sqlText SQLText,					--IP - 26/08/10 - CR1092 - COASTER to CoSACS Enhancements
		@acctNoCol varchar(50),				--IP - 26/08/10 - CR1092 - COASTER to CoSACS Enhancements
		@transDateCol varchar(50),			--IP - 26/08/10 - CR1092 - COASTER to CoSACS Enhancements
		@transValueCol varchar(50),			--IP - 26/08/10 - CR1092 - COASTER to CoSACS Enhancements
		@noRecNotImported int,				--IP - 27/08/10 - CR1092 - COASTER to CoSACS Enhancements
		@monthFileProcessed	int,			--IP - 06/09/10 - CR1092 - COASTER to CoSACS Enhancements	
		@hiRefNo int,						--IP - 06/09/10 - CR1092 - COASTER to CoSACS Enhancements	
		@HOBranch int						--IP - 06/09/10 - CR1092 - COASTER to CoSACS Enhancements

set @return = 0

DECLARE @BCPpath VARCHAR(500)

SELECT @BCPpath = value + '\BCP' FROM CountryMaintenance
WHERE Codename = 'BCPpath' 

--IP - 06/09/10 - CR1092 -  Get Head Office Branch no.
Set @HOBranch=(select value from countrymaintenance
            where codename='hobranchno')


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

set nocount ON

--select all the values for the payment file into variables
select @dateformat = [dateformat],
	   @datelength = datelength,
	   @moneypoint = moneypoint,
	   @headline = headline,
       @hastrailer = hastrailer,	
       @delimiter = Delimiter,
       @delimitedNoOfCols = DelimitedNoOfCols,
       @delimitedAcctNoColNo = DelimitedAcctNoColNo,
       @delimitedDateColNo = DelimitedDateColNo,
       @delimitedMoneyColNo = DelimitedMoneyColNo

from stordercontrol
where bankname = @BankName
	
--IP - set the hexadecimal value for the delimiter to be used in the bcp command --IP - 16/09/10 - CR1092 - select hex value from delimiter table.

SET @delimiterHexVal = (SELECT HexValue FROM StorderDelimiters WHERE [Delimiter] = @delimiter)
	--IP - 10/08/10 - CR1092 - Now proceed to process each file beneath the directory
			set @loop = 1
			while @loop <= @rowCount
			begin --IP - 10/08/10 - CR1092
				
				truncate table StOrderTxn	--IP - 11/08/10 - CR1092 - truncate before starting to process the next file.

					select @filename = [filename] from SOFilesToProcess where ID = @loop	--IP - 10/08/10 - CR1092 - Set the filename to the file to be processed
					
					set @monthFileProcessed = (select cast(datepart(mm,max(rundate))as int) 
												from SOFilesProcessed 
												where [filename] = @filename
												and SOFilesProcessed.Processed='Y') --IP - 06/09/10 - CR1092
					
					--IP 06/09/10 - CR1092 - Get the hirefno from the head office branch which will be used to update the accounts transrefnos.
					select @hiRefNo = hirefno from branch where branchno = @HOBranch    
					
					--IP - 13/08/10 - CR1092 - File already processed this month
				IF NOT EXISTS(select * from SOFilesProcessed s
								where s.[FileName] = @filename
								and s.Processed = 'Y'
								and @monthFileProcessed = cast(DATEPART(mm, @rundate) as int))	--IP - 06/09/10 - CR1092
				BEGIN-- IP - 13/08/10 - CR1092 - Check if file processed
					
					-- initialise totals
					set @RecCount=0
						
					TRUNCATE TABLE .dbo.Storder --Table will be used to import the data just to obtain the number of rows in the file
					TRUNCATE TABLE .dbo.StorderDelimited
					TRUNCATE TABLE .dbo.StorderDelimitedTemp	--IP - 26/08/10

					-- Import standing order data file
					declare @path varchar(200),
							@path2 varchar(200)
					set @path = '"' + @BCPpath + '" ' + db_name()+
						'.dbo.StorderDelimited' + ' in ' +
					   'd:\cosdata\sodata\' + @BankName+ '\'+@filename + ' -t' +@delimiterHexVal+ ' -c -r\n -q -Usa -P'	--IP - 25/08/10 - CR1092 - added @DelimiterHex
	
					select 'Import data file'	
					
					--Need to import into Storder table to obtain total number of rows in the file as with delimited files when imported into 
					--the storderdelimited table (which contains multiple columns) if the file record does not have correct number of delimiters
					--a record is not imported. If there is a difference in the count imported from both imports then do not process the file.
					set @path2 = '"' + @BCPpath + '" ' + db_name()+
						'.dbo.Storder' + ' in ' +
					   'd:\cosdata\sodata\' + @BankName+ '\'+@filename + ' -t' +@delimiterHexVal+ ' -c -r\n -q -Usa -P'	--IP - 25/08/10 - CR1092 - added @DelimiterHex
					exec master.dbo.xp_cmdshell @path
					exec master.dbo.xp_cmdshell @path2
					
					--IP - 26/08/10 - Now need to insert the AccountNo, Date and Money values into StorderDelimitedTemp table.
					--This table will be used in the cursor to select the values
					set @sqltext = 'INSERT INTO StorderDelimitedTemp (AcctNo, TransDate, TransValue) ' +
								   'SELECT ' +@delimitedAcctNoColNo+ ' ,' +@delimitedDateColNo+ ' ,' +@delimitedMoneyColNo +
								   ' FROM StorderDelimited'
					
					EXEC sp_executesql @sqlText
					
					set @count=(select count(*) from StorderDelimitedTemp)
					set @storderCount = (select count(*) from Storder)
					
					
					If @count > 0
					begin
						if(@count = @storderCount) --If all records have been successfully imported
						begin
							--IP - 13/08/10 - CR1092 - Insert a record into exception table if file successfully imported
							--S = Successfull.
							INSERT INTO SOException(RunDate,RunNo,Source,[FileName],Record,Error,Field, [LineNo]) --IP - 13/08/10 - CR1092 - Added LineNo --16/08/10 - Added Field
							VALUES(@runDate,@RunNo,@BankName,@filename,'File successfully imported for bank','S','', 0)
												
							select convert(varchar(5),@count) + ' Records to Process for bank: '+ @bankname + ' ,filename: ' + @filename	--IP - 10/08/10 - CR1092

								Declare SO cursor for select isnull(AcctNo,''), isnull(TransDate,''), isnull(TransValue,'') from StorderDelimitedTemp
								Open SO
								Fetch next from SO into @acctNoCol, @transDateCol, @transValueCol
								While @@Fetch_status = 0
									Begin
										
										
									----IP - 18/08/10 - CR1092 - Initialise for each record fetched
									set @RecCount=@RecCount+1
									set @ValError=' '

										--If file contains header skip fist record 
										If (@headline=1 and @RecCount>1) or @headline=0
							                   
										Begin
										--If records processed < total records in file - process detail                                                                      
										 If @RecCount<@Count
											or @hastrailer=0		
										
										-- Detail Record       
										Begin
											set @rectype='D'
											
											--Validate the account number is numeric
											if(ISNUMERIC(replace(@acctNoCol, '-',''))=1 and LEN(replace(@acctNoCol, '-',''))=12 
												AND replace(@acctNoCol, '-','')!='000000000000') --IP - 09/09/10 - CR1092 - UAT5.4 - UAT(6) --IP - 14/09/10 - CR1092 - UAT5.4 - UAT(13)
											begin
												set @Acctno = replace(@acctNoCol,'-','')
											end
											else
											begin
											
												--Data error A = Data error with Account Number
												
												IF(@acctNoCol = '' or @acctNoCol is null)
												begin
													set @Acctno = '' 
													set @fieldError = 'No account number entered'
												end
												else
												begin
													set @Acctno = ''
													set @fieldError = 'Account number not in correct format: ' + @acctNoCol
												end
												
												INSERT INTO SOException(RunDate,RunNo,Source,[FileName],Record,Error,Field, [LineNo]) 
												VALUES(@runDate,@RunNo,@BankName,@filename,@delimitedAcctNoColNo,'A',@fieldError, @RecCount)
												
												set @ValError = 'E' 
												
											end
							                
							               
											if(@transDateCol = '' or @transDateCol is null) --If no transaction date has been set then set to the rundate.
											begin
												set @TxnDate = convert(char(8),@runDate,112)

												SET @fieldError = 'No transaction date supplied. The transaction will be processsed with todays date.'
											
												INSERT INTO SOException(RunDate,RunNo,Source,[FileName],Record,Error,Field, [LineNo])	
												VALUES(@runDate,@RunNo,@BankName,@filename,@delimitedMoneyColNo,'D',@fieldError , @RecCount)
											end
											else
											begin
												set @recDate = @transDateCol --Select the transaction date

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
												
											
												BEGIN TRY --Catch data exceptions on the transaction date
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
												
													--If there is a problem with the date then set to todays date and write a record to the exception table
													--but still continue to process.
													SET @TxnDate =  convert(char(8),@runDate,112)
													SET @fieldError = 'Error with date supplied. The transaction will be processsed with todays date. The column with the error is: ' + @delimitedDateColNo 
													
													INSERT INTO SOException(RunDate,RunNo,Source,[FileName],Record,Error,Field, [LineNo])	
													VALUES(@runDate,@RunNo,@BankName,@filename,@delimitedDateColNo,'D',@fieldError , @RecCount)
												
												END CATCH
											end
											-- check date within last 100 days
											If (@Txndate > getdate() or @TxnDate < getdate()-100 and @ValError!='E')
												set @ValError='D'
												
											--Validation on Money
											if(ISNUMERIC(@transValueCol)=1)
											begin
												set @Txnvalue=convert(money,@transValueCol)
											end
											else
											begin
												
												--Data error M = Data error with money
												set @TxnValue = 0
												if(@transValueCol = '' or @transValueCol is null)
												begin
														set @fieldError = 'No transaction value entered'
												end
												else
												begin
														set @fieldError = 'Incorrect transaction value encountered: '+@transValueCol
												end
											
												
												INSERT INTO SOException(RunDate,RunNo,Source,[FileName],Record,Error,Field,[LineNo])	
												VALUES(@runDate,@RunNo,@BankName,@filename,@delimitedMoneyColNo,'M',@fieldError, @RecCount)
												
												set @ValError = 'E'
											end
										
											-- divide value by 100 if no decimal (1=decimal, 0=no decimal)       
											If @moneypoint=0					
												set @Txnvalue=@Txnvalue/100
											--set @TotalTxnValue=@TotalTxnValue+@Txnvalue

											set @TransRefno = 0
											set @Bank=@BankName

											-- Add record to StOrderTxn table
											--IP - 11/08/10 - CR1092 - Insert into table variable first, then permanent table later if no errors encountered in the data.
											insert into StOrderTxn (RecTyp,Acctno,TxnDate,TxnValue,TransRefno,Bank,ValError, [FileName], [LineNo]) --13/08/10 - CR1092 - Added FileName, LineNo
											select @RecType,@Acctno,@TxnDate,@TxnValue,@TransRefno,@Bankname,@ValError, @filename, @RecCount
				
											
									   End
										--Else	-- UAT86 16/04/10 
									   Begin
									--    Trailer Record
									   If @RecCount=@Count 
										Begin
											
											set @rectype='T'
											set @Acctno=null
											set @TxnDate=null
											set @Txnvalue=null
											-- divide value by 100 if no decimal        
											set @TransRefNo=9999999
											set @Bank=@BankName
											-- Add record to StOrderTxn table
											
											--IP - 11/08/10 - CR1092 - Insert into temporary table first, then permanent table later if no errors encountered.
											insert into StOrderTxn (RecTyp,Acctno,TxnDate,TxnValue,TransRefno,Bank, [FileName], [LineNo]) --13/08/10 - CR1092 - Added FileName, LineNo
											select @RecType,@Acctno,@TxnDate,@TxnValue,@TransRefno,@Bankname, @filename, @RecCount

										End
										
											
									   End
									End 
									
									
							-- Fetch next record
										Fetch next from SO into @acctNoCol, @transDateCol, @transValueCol
									
										
									 end
							-- Close & Deallocate 
							Close SO
							Deallocate SO
				
							--File has been imported and processed successfully therefore update processed = 'Y'
							Update SOFilesProcessed
							set processed = 'Y' 
							where [FileName] = @filename 
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
						
						end--All records successfully imported
						else
						begin --Not all records were successfully imported therefore do not process the file.
							
								set @noRecNotImported = (select @storderCount - @count) --Select the number of records that were not imported.
								
								INSERT INTO SOException(RunDate,RunNo,Source,[FileName],Record,Error,Field, [LineNo]) 
								VALUES(@runDate,@RunNo,@BankName,@filename,Cast(@noRecNotImported as varchar(10))+ ' record(s) were not successfully imported','F','', 0)
								 
								SET @ProcessError = Cast(@noRecNotImported as varchar(10))+ ' record(s) were not succesfully imported for ' + 'File: ' + @filename + ' Bank: ' + @BankName
						
								INSERT INTO interfaceerror(interface,runno,errordate,severity,errortext)
								VALUES('STORDER',@runno,GETDATE(),'W',@ProcessError)
							
								--File has been imported but no rows found, therefore update processed = 'N'
								Update SOFilesProcessed
								set processed = 'N' 
								where FileName = @filename 
								and runno = @RunNo
						end
							
					End 
					else
					Begin
					
						--Data error N = No records to process 
						        
								INSERT INTO SOException(RunDate,RunNo,Source,[FileName],Record,Error,Field, [LineNo]) 
								VALUES(@runDate,@RunNo,@BankName,@filename,'No records found to process for bank','F','', 0)
								 
								SET @ProcessError = 'No records found to process for ' + 'File: ' + @filename + ' for Bank: ' + @BankName
						
								INSERT INTO interfaceerror(interface,runno,errordate,severity,errortext)
								VALUES('STORDER',@runno,GETDATE(),'W',@ProcessError)
							
								--File has been imported but no rows found, therefore update processed = 'N'
								Update SOFilesProcessed
								set processed = 'N' 
								where [FileName] = @filename 
								and runno = @RunNo
					End
					

				END--File already processed this month
				ELSE
				BEGIN
					
					INSERT INTO SOException(RunDate,RunNo,Source,[FileName],Record,Error, Field,[LineNo]) 
					VALUES(@runDate,@RunNo,@BankName,@filename,'File has already been processed for bank this month','F','', 0)
		
					SET @ProcessError = 'File: ' + @filename + ' for Bank: ' + @BankName + ' has already been processed this month'
		
					INSERT INTO interfaceerror(interface,runno,errordate,severity,errortext)
					VALUES('STORDER',@runno,GETDATE(),'W',@ProcessError)
					
					Update SOFilesProcessed
							set processed = 'N' 
							where [FileName] = @filename 
							and runno = @RunNo
					
				END
				set @loop = @loop + 1  

			END 