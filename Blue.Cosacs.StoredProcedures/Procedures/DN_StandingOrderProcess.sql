-- Author:  John Croft  
-- Date:    March 2006

/*    This procedure will write the Standing order transactions to the StorderProcess table 
      and checks the Status and outstanding balance to determine if the account has to be 
      settled or re-opened.

*/
if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_StandingOrderProcess]') 
        and OBJECTPROPERTY(id, 'IsProcedure') = 1)
Drop Procedure .[dbo].[DN_StandingOrderProcess]
Go

Create Procedure .dbo.DN_StandingOrderProcess
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_StandingOrderProcess.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : DN_StandingOrderProcess 
-- Author       : John Croft
-- Date         : 2007
--
-- This procedure will validate the payment files from banks.
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 16/04/10 jec UAT86 Allow file without a trailer.
-- 12/08/10 ip  CR1092 - COASTER to CoSACS Enhancements - Modified procedure so that it does not use a cursor.
-- 16/08/10 ip  CR1092 - Set Error = 'B' = Write Off for accounts settled and have a BDW Balance - Payments will be made to the BDWRecover account.
-- 18/08/10 ip  CR1092 - Set Error = 'A' = Accounts that do not exist - Payments will be made to the Sundry account.
-- 03/09/10 ip  CR1112 - Tallyman Interest Charges - Mark transactions as Interest transactions based on setup on Payment File Definition.
-- 06/09/10 ip  CR1112 - Tallyman Interest Charges - Set Error = 'E' for accounts that do not exist for interest files being processed.
-- 15/09/10 ip  CR1092 - UAT5.4 - UAT14 - Transaction value in the file was (-), this was then incorrectly converted to be positive for a payment file.
--						 changed by making the value absolute then converting to a (-) value if a payment file.
-- ================================================
	-- Add the parameters for the stored procedure here 

        @RunNo Int,
        @bankName varchar(16),
        @fileName varchar(32)
as

set nocount on


declare @RunDate datetime,
		@isInterest bit,			--IP - 06/09/10 - CR1112 
		@ProcessError varchar(500),
		@noRecSuccessfullyProcessed int,	--IP - 27/08/10 - CR1092 - COASTER to CoSACS Enhancements	
		@noRecUnsuccessfull int				--IP - 27/08/10 - CR1092 - COASTER to CoSACS Enhancements	

set @RunDate = GETDATE()

--IP - 06/09/10 - CR1092 
set @isInterest = (select isInterest from stordercontrol
					where bankname = @bankName)
					
-- Process StorderTxn      
insert into StOrderProcess (RunNo,Acctno,TransRefno,TransValue,Datetrans,Error,Bankname, [FileName], [LineNo], IsInterest) --IP - 16/08/10 - CR1092 - Added FileName & LineNo 
select @RunNo, 
	   case when s.Acctno = '' then '999999999999' else s.Acctno end, --IP - 27/08/10 - CR1092
	   s.TransRefno,
	   case when @isInterest = 0 then ABS(s.TxnValue) * -1 else ABS(s.TxnValue) end,		-- reverse sign on value - Payments are negative --IP - 03/09/10 - CR1112 - Do not reverse sign for Interest transactions. --IP - 15/09/10 - CR1092 - UAT5.4 - UAT(14)
	   s.TxnDate,
	   case 
				when a.outstbal - s.TxnValue = 0 and @isInterest = 0 then 'W'	--IP - 03/09/10 - CR1112 - Only set for Payment Transactions
				when a.currstatus ='S' and (a.bdwbalance = 0 and a.bdwcharges = 0) then 'S'
				when a.currstatus ='S' and (a.bdwbalance > 0 or a.bdwcharges > 0) then 'B'		--IP - 16/08/10 - CR1092
				when a.currstatus is null and s.acctno!='' then 'A'  --IP - 18/08/10 - CR1092 - A = Account does not exist
				--when s.Acctno = '999999999999' then 'W'			--UAT86 --IP - 26/08/10 - CR1092 - commented out
				when s.ValError = 'E' then 'E' --IP - 18/08/10 - CR1092 - E = Error, these accounts should not get processed.
				else ' '
				end,
	   s.Bank,
	   s.[FileName],
	   s.[LineNo],
	   @isInterest	--IP - 06/09/10 - CR1112 - Tallyman Interest Charges
	        
from storderTxn s
left outer join Acct a on s.acctno=a.acctno
where s.RecTyp = 'D'

--IP - 16/08/10 - CR1092 - Insert into SOException table where account number does not exist
if(@isInterest = 0)
begin
	set @ProcessError = 'Account does not exist'
end
else
begin
	set @ProcessError = 'Account does not exist, interest transaction will not be processed'
end

INSERT INTO SOException(RunDate,RunNo,[Source],[FileName],Record,Error, Field, [LineNo])
select @RunDate, @RunNo, s.bankname, s.[FileName], @ProcessError, 'A', s.acctno, s.[LineNo]
from StOrderProcess s
where s.runno = @RunNo
and s.[FileName] = @fileName
and s.error = 'A'

--IP - 06/09/10 - CR1112 
if(@isInterest = 1)
begin
	update StOrderProcess
	set error = 'E'
	where runno = @RunNo
	and [FileName] = @fileName
	and error = 'A'
end

--IP - 27/08/10 - CR1092 - If some records were logged with errors, indicate the number that were successfull and the number that were unsuccessfull.
if((select count(*) from storderprocess
	where error = 'E') > 0)
begin
	
	--Select the number of records that were successfully processed, and the number that were unsuccessfull.
	set @noRecSuccessfullyProcessed = (select count(*) from storderprocess 
										where runno = @RunNo
										and [FileName] = @fileName 
										and error!='E')
	set @noRecUnsuccessfull = (select count(*) from storderprocess where runno = @RunNo
										and [FileName] = @fileName 
										and error='E')
										
	set @ProcessError = 'File: ' + @filename + ' for Bank: ' + @BankName + ' processed '
							+Cast(@noRecSuccessfullyProcessed as varchar(10))+ ' records successfully and ' 
							+cast(@noRecUnsuccessfull as varchar(10))+ ' records unsuccessfully'
	
	insert into interfaceerror(interface,runno,errordate,severity,errortext)
	values('STORDER',@runno,GETDATE(),'W',@ProcessError)
	
end
go

-- End End End End End End End End End End End End End End End End End End End End End End End End               