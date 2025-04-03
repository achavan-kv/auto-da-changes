GO
SET QUOTED_IDENTIFIER ON
GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[CM_SPACalculateArrangementScheduleSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CM_SPACalculateArrangementScheduleSP]
GO
-- ============================================================================================
-- Author:		Ilyas Parker
-- Create date: 03/10/2008
-- Description:	The procedure will calculate the SPA Arrangement Schedule			
-- ============================================================================================
CREATE PROCEDURE [dbo].[CM_SPACalculateArrangementScheduleSP] 
		        @acctno varchar(12),
				@period char(1),
				@arrangementAmt money, 
				@numberOfInstalments int,
				@instalmentAmt money,
				@oddPaymentAmt money,
				@firstPaymentDate datetime,
				@finalpaydate datetime out,				
				@numberRemainInstals int,
				@remainInstalAmt money,
				@return	int	OUTPUT
as
begin
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	set nocount on;

	set @return = 0    --initialise return code
	
	set @finalpaydate=@firstPaymentDate     -- initialse finalpay date to firstpay date (incase only 1 instal)
	
	--Declare counters
	declare @index int, --Counter for 'Instalment Amount'
			@NextDateDue datetime, -- Will hold the next Date Due for the 'Arrangement Schedule'
			@prevTotalAmtDue money -- Will hold the 'TotalAmountDue'

--	--Testing parameters
--	set @acctno = '90040005000'
--	set @period ='w'
--	set @arrangementAmt = 100
--	set @numberOfInstalments = 4
--	set @instalmentAmt = 25
--	set @oddPaymentAmt = 40
--	set @firstPaymentDate = getdate()
--	--Testing Parameters
	
	--Set counters
	set @index = 1     --we want the index to be 1 for the first instalment.
	set @NextDateDue = @firstPaymentDate --Initialise this to the first payment date entered for the first record.
	--Initialise this to the instalment amount or the oddpayment amount if it has been entered.
--	set @prevTotalAmtDue = case when @oddPaymentAmt > 0 then @oddPaymentAmt else @instalmentAmt end
--	set @prevTotalAmtDue = case when @index = @numberOfInstalments and @oddPaymentAmt >0 and @numberRemainInstals=0
--						then @oddPaymentAmt else @instalmentAmt end
	set @prevTotalAmtDue =0

--	if not exists (select * from information_schema.tables where table_name = 'ArrangementSchedule')
--	begin
		--Table to store the 'Arrangement Schedule'
	create table #ArrangementSchedule
	(
		Acctno varchar(12),
		Instalment int,
		DateAdded datetime,
		DateDue datetime,
		AmountDue money,
		TotalAmountDue money
	)
--	end
--	else
--	truncate table ArrangementSchedule
	
	--While the counter 'index' for the number of instalments is < than the number of instalments entered.
	while @index <= @numberOfInstalments
		begin
			--Holds the value of the 'TotalAmountDue'
		        --set @prevTotalAmtDue = @prevTotalAmtDue + @instalmentAmt
		        set @prevTotalAmtDue =
					case when @index = @numberOfInstalments and @oddPaymentAmt >0 and @numberRemainInstals=0
						then @prevTotalAmtDue+@oddPaymentAmt else @prevTotalAmtDue+@instalmentAmt end
			--Insert entries into the 'ArrangementSchedule' table for the arrangement
			insert into #ArrangementSchedule
			select @acctno,
				   @index,
				   getdate(),				   
				   --if first instalment then 'DateDue' = First Payment Date entered through the screen
				   --else the next date in the arrangement depending on the period.
				   case when @index = 1 then @firstPaymentDate else @NextDateDue end,
					-- oddpayment = final instalment
				   case when @index = @numberOfInstalments and @oddPaymentAmt >0 and @numberRemainInstals=0
						then @oddPaymentAmt else @instalmentAmt end,
 				   @prevTotalAmtDue
				set @finalpaydate=@NextDateDue  -- last instalment date
				
				
				
				
				--Set the 'DateDue' of the instalment according to the period selected 'W' (Weekly),
				--'F' (Fortnightly), 'M'(Monthly)
--				set @NextDateDue = case when @period ='W' then dateadd(d, 7, @NextDateDue) 
--									when @period = 'F' then dateadd(d, 14, @NextDateDue) 
--									when @period = 'M' then dateadd(m, 1, @NextDateDue)
--									end
				-- calc date due from first instalment date so days revert when > 28 (i.e. day 30/31 or 28 for feb)
				set @NextDateDue = case when @period ='W' then dateadd(d, 7*@index, @firstPaymentDate) 
									when @period = 'F' then dateadd(d, 14*@index, @firstPaymentDate) 
									when @period = 'M' then dateadd(m, 1*@index, @firstPaymentDate)
									end
				--Increment the counter for the next instalment
				set @index = @index + 1
		end

	set @index = 1			
--While the counter 'index' for the number of instalments is < than the number remaining instalments (term remains).
	while @index <= @numberRemainInstals
		begin
			--Holds the value of the 'TotalAmountDue'		        
		        set @prevTotalAmtDue =
					case when @index = @numberRemainInstals and @oddPaymentAmt >0 
						then @prevTotalAmtDue+@oddPaymentAmt else @prevTotalAmtDue+@remainInstalAmt end
			--Insert entries into the 'ArrangementSchedule' table for the arrangement
			insert into #ArrangementSchedule
			select @acctno,
				   @index+@numberOfInstalments,
				   getdate(),				   
				   @NextDateDue,	
					-- oddpayment = final instalment
				   case when @index = @numberRemainInstals and @oddPaymentAmt >0 then @oddPaymentAmt else @remainInstalAmt end,
 				   @prevTotalAmtDue
				set @finalpaydate=@NextDateDue  -- last instalment date
			
				
				
				--Set the 'DateDue' of the instalment according to the period selected 'W' (Weekly),
				--'F' (Fortnightly), 'M'(Monthly)
--				set @NextDateDue = case when @period ='W' then dateadd(d, 7, @NextDateDue) 
--									when @period = 'F' then dateadd(d, 14, @NextDateDue) 
--									when @period = 'M' then dateadd(m, 1, @NextDateDue)
--									end
				-- calc date due from first instalment date so days revert when > 28 (i.e. day 30/31 or 28 for feb)
				set @NextDateDue = case when @period ='W' then dateadd(d, 7*(@index+@numberOfInstalments), @firstPaymentDate) 
									when @period = 'F' then dateadd(d, 14*(@index+@numberOfInstalments), @firstPaymentDate) 
									when @period = 'M' then dateadd(m, 1*(@index+@numberOfInstalments), @firstPaymentDate)
									end
					--Increment the counter for the next instalment
				set @index = @index + 1
		end
--Select the columns to be displayed in the 'Special Arrangements' screen.
select Acctno ,Round(Instalment,2) as 'Instalment', 
       convert(varchar(11),datedue) as 'Date Due', 
	   cast(AmountDue as decimal(12,2)) as 'Amount Due',
	   cast(TotalAmountDue as decimal(12,2)) as 'Total Amount Due'
       from #ArrangementSchedule       


    if (@@error != 0)
    begin
        set @return = @@error
    end
end

GO
