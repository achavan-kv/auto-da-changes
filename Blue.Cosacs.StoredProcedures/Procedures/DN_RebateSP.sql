SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[DN_RebateSP]')
					and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[DN_RebateSP]
GO

CREATE PROCEDURE [dbo].[DN_RebateSP]
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_RebateSP.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Rebate Calculation
-- Author       : ??
-- Date         : ??
--
-- This procedure will get calculate the rebate
-- 
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 01/04/08  jec CR938 changes. St.Lucia Rebate straight line calc.
-- 14/07/08  sl  Change StL to match Rebate forecast for all accts - straight line for single account
-- 30/04/09  jec UAT101 Pass in @AcctNo as 'RPA' for Rebate Partial Accrual report so as not to update
--               main rebate table (use rebateRPA table instead)
-- 06/05/09  jec CR937 Unearned Credit Income RS. Keep Rebates Totals and tag with PE date
-- 08/10/09  jec UAT130 Run Rebate Forecast if RebateSP is running on Period End Date
-- 05/10/12  jec #10138 - LW75030 - SUCR - Cash Loan 
-- 06/08/14  ip  #19426 - CR18938 - Change Cash Loan Rebate Calculation for Trinidad & Tabago
-- ================================================
	-- Add the parameters for the stored procedure here

--leave in this order
    @AcctNo 		  char(12),
    @poRebate 		  money = 0  OUTPUT,
    @poRebateWithin12Mths money	= 0  OUTPUT,
    @poRebateAfter12Mths  money = 0  OUTPUT,
    @return 		  int   = 0  OUTPUT,
    @FromDate		  datetime = '01-jan-1900',
    @FromThresDate	  datetime = '01-jan-1900',
    @UntilThresDate	  datetime = '01-jan-1900',
    @RuleChangeDate	  datetime = '01-apr-2002',
    @RebateDate		  datetime = '01-jan-1900'

AS
--------------------
--DECLARE PARAMETERS
--------------------
    declare
	@delpcent		money,
	@rebpcent		money,
	@CountryCode		char(1),
    @Nearest 		smallint,
	@NoCents		smallint,
	@RebateDate_after12mths datetime,
	@sqlstr			sqltext,
    @query_text varchar (1000),
	@rebatetable		varchar(12),
    @rule           smallint, --CR888 added
    @cashLoanRule SMALLINT, -- separate rule for cash loan accounts
	@StLuciaRuleChange	DATETIME,	-- CR938 jec 01/04/08
	@PeriodEndchar varchar(12),	
	@Currdate	datetime,
	@PEMinDate  datetime,
	@PEMaxDate	datetime,
	@today datetime,
	@nextPEdate	datetime,
	@currDay datetime,			--#13313
	@returnRF INT,
	@cashLoanEarlySettPenaltyPeriod int	--#19426 - CR18938 - Trinidad & Tobago Cash Loan Early Settlement Penalty Period
	
-- UAT130 (RS) - jec 
set @Currdate =GETDATE()	
	
set @currDay = 	convert(datetime,convert(varchar(11),@Currdate)) --#13313

if(datediff(mi, @CurrDay, @CurrDate) < 480) --#13313 - running before 8am then set to previous day
begin
	set @currDay = dateadd(day, -1, @currDay)
end

set @PeriodEndchar= Convert(datetime,CAST(convert(datetime,@Currdate) as CHAR(12)),103)
set @today = CONVERT(datetime, @periodendchar, 102)				-- today no timestamp
-- get next period end date if running on period end day between 10:00 to 04:00 next day (some countries run very early if not open on Sunday)
set @nextPEdate=(select enddate from RebateForecast_PeriodEndDates 
				where enddate between dateadd(d,-1,@today) and dateadd(d,1,@today)
				and rundate='1900-01-01 00:00:00.000'
				and @Currdate between DATEADD(mi,+600,enddate) and DATEADD(mi,+240,DATEADD(d,+1,enddate)))	
set @PeriodEndchar= Convert(datetime,CAST(convert(datetime,@nextPEdate) as CHAR(12)),103)
set @returnRF=0

SET NOCOUNT ON 
-- ensure this date is set to '31-mar-2008'
set @StLuciaRuleChange = '31-mar-2008'	-- St Lucia Rule change date CR938 jec 01/04/08

-- the date is hard coded and as a country parameter. To allow for easier testing the parameter date can be set at
-- a date between 31/03/07 and 31/03/08 
if (select countrycode from country) = 'L'	-- only for St.Lucia
BEGIN
if (select value from CountryMaintenance where CodeName='SLRebateCalculationRuleDate') < @StLuciaRuleChange
	and (select value from CountryMaintenance where CodeName='SLRebateCalculationRuleDate') >= '2007-03-31'
	set @StLuciaRuleChange=(select value from CountryMaintenance where CodeName='SLRebateCalculationRuleDate')
else-- set parameter to hard coded date
	update CountryMaintenance set value='2008-03-31' where codename='SLRebateCalculationRuleDate'
End

-----------------------
--Return if cash or special acct
-----------------------
	if (select right(left(@acctno,4),1)) = '4' or  (select right(left(@acctno,4),1)) = '5'
	begin
		--return zero rebate for account
		select 	@porebate = 0,
			@return = 0
		return
	end

-----------------------
--INITIALISE PARAMETERS
-----------------------
--country parameters
	SELECT	@RebPCent    = rebpcent,
		@CountryCode = countrycode,
		@NoCents     = nocents,
		@delpcent    = globdelpcent,
		@return      = 0
	FROM	country

    select @rule = value from countrymaintenance where codename = 'RebateCalculationRule'
    select @cashLoanRule = value from countrymaintenance where codename = 'LoanRebateRule'
	select @cashLoanEarlySettPenaltyPeriod = value from countrymaintenance where codename = 'CL_EarlySettPenaltyPeriod'		--#19426 - CR18938

	if (select isnull(@UntilThresDate,'01-jan-1900')) = '01-jan-1900'
		select @UntilThresDate = convert(datetime,@currDay,111) --#13313

	if (select isnull(@RebateDate,'01-jan-1900')) = '01-jan-1900'
		select @RebateDate = convert(datetime,@currDay,111)		--#13313


-----------------------------------
--REMOVE OLD DATA AND SET RUN DATES
-----------------------------------
	if (select @acctno) = 'all'
	begin
		delete from Rebates

		update 	rebates_asat
		set	asatdate = @rebatedate,
			rundate = getdate()
	end

--------------------------------
--Ensure dates have no time part
--------------------------------
	select @UntilThresDate = convert(datetime,convert(varchar(11),@UntilThresDate))

	select @RebateDate = convert(datetime,convert(varchar(11),@RebateDate))
  

-------------------------------
--SET CALCULATION DATE + 7 days
-------------------------------
	--set date to run rebate at, rebate run date 7 days after period end, truncated so no time part
	-- remove 7 days CR956
	if @countrycode in ('S','P','M','C','Y','I','F')
	begin
	    select @RebateDate = CONVERT(DATETIME,CONVERT(CHAR(11),dateadd(day,7,@RebateDate),111),111)
	end
	else 
	begin
	    select @RebateDate = CONVERT(DATETIME,CONVERT(CHAR(11),dateadd(day,0,@RebateDate),111),111)
	end

	--set date for rebate after 12 months - date in 1 year
	select @RebateDate_after12mths = CONVERT(DATETIME,CONVERT(CHAR(11),dateadd(year,1,@RebateDate),111),111)

--------------------------------
--DELIVERY THRESHOLD CALCULATION
--------------------------------
	--Only if running frame
	if (@acctno = 'all')
	begin
      TRUNCATE TABLE settled_before_thres
		--Get all accounts settled before untilthresdate as don't need these	
      INSERT INTO [settled_before_thres] (acctno,maxdate,statuscode)
		select	a.acctno, max(datestatchge)  , statuscode
		from 	status a, acct b
		where	a.acctno = b.acctno
		and	currstatus = 'S'
		and	statuscode = 'S'
		and	DateAcctOpen >= @fromdate
		and	DateAcctOpen < dateadd(day,1,@UntilThresDate)
		and	datestatchge <= convert(varchar,@UntilThresDate)
		and	substring(a.acctno,4,1) in ('0','1','2','3','6','7','8','9')
		group by a.acctno, statuscode

		SELECT @return = @@error

		if (@return = 0)
		begin
         TRUNCATE TABLE [del_thres_reached_before]

			--Get all accounts that met threshold before fromthresdate (don't want these)*/
         INSERT INTO [del_thres_reached_before] (acctno,deltotal,DelThresDate)
			select  b.acctno, sum(transvalue) as deltotal, max(b.datetrans) 
			from    fintrans b, country c, acct a
			where   b.acctno = a.acctno
			and     b.transtypecode in ('DEL','GRT','REP','ADD','RDL','RPO','CLD')			-- #10138
			and     b.datetrans < convert(varchar,@FromThresDate)
			and	DateAcctOpen >= @fromdate
			and	DateAcctOpen < dateadd(day,1,@UntilThresDate)
			and 	not exists (select * from settled_before_thres a where  a.acctno = b.acctno)
			and    	substring(b.acctno,4,1) in ('0','1','2','3','6','7','8','9')
			group by b.acctno, a.agrmttotal, c.globdelpcent
			having ((sum(transvalue)) >= (a.agrmttotal * (c.globdelpcent/100)))

			SELECT @return = @@error
		end

		--Get all the accounts that met threshold before RuleChangeDate
		if (@return = 0)
		begin
         TRUNCATE TABLE del_thres_reached
         INSERT INTO del_thres_reached (acctno,deltotal,beforerulechange,DelThresDate)
			select	b.acctno, sum(transvalue)  , 'Y'  ,	max(datetrans)  
			from   	fintrans b , country c, acct a
			where  	b.acctno = a.acctno
			and	b.transtypecode in ('DEL','GRT','REP','ADD','RDL','RPO','CLD')			-- #10138
			and    	b.datetrans < convert(varchar,@RuleChangeDate)
			and	DateAcctOpen >= @fromdate
			and	DateAcctOpen < dateadd(day,1,@UntilThresDate)
			and	not exists (select * from settled_before_thres d where a.acctno = d.acctno)
			and	not exists (select * from del_thres_reached_before e where a.acctno = e.acctno)
			and    substring(b.acctno,4,1) in ('0','1','2','3','6','7','8','9')
			group by b.acctno, a.agrmttotal, c.globdelpcent
			having ((sum(transvalue)) >= (a.agrmttotal * (c.globdelpcent/100)))

			SELECT @return = @@error
		end

		if (@return = 0)
		begin
			--Get all the accounts that met threshold after RuleChangeDate and before UntilThresDate
			insert into dbo.del_thres_reached
			select	b.acctno, sum(transvalue) as deltotal, 'N' as beforerulechange,
				max(b.datetrans) as DelThresDate
			from   	fintrans b , country c, acct a
			where  	b.acctno = a.acctno
			and	b.transtypecode in ('DEL','GRT','REP','ADD','RDL','RPO','CLD')			-- #10138
			and    	b.datetrans < dateadd(day,1,@UntilThresDate) --KEF 67093 changed so includes transactions happening on same day asrebate calc date that have time parts (previously it only included those with time set as midnight)
			and	DateAcctOpen >= @fromdate
			and	DateAcctOpen < dateadd(day,1,@UntilThresDate)
			and 	not exists (select * from settled_before_thres d where a.acctno = d.acctno)
			and 	not exists (SELECT * From del_thres_reached f where a.acctno = f.acctno)
			and 	not exists (select * from del_thres_reached_before e where a.acctno = e.acctno)
			and	substring(b.acctno,4,1) in ('0','1','2','3','6','7','8','9')
			group by b.acctno, a.agrmttotal, c.globdelpcent
			having ((sum(transvalue)) >= (a.agrmttotal * (c.globdelpcent/100)))

			SELECT @return = @@error
		end
	end	--end code for rebate frame only

----------------
--INITIAL SELECT for main @rebates table
----------------
	IF (@return = 0)
	BEGIN
		--create temporary table to hold caluclation data
      DECLARE @rebates TABLE (
			[Acctno] char(12) NOT NULL primary key,
			[Termstype] varchar(2) NOT NULL,
	        [DateAcctOpen] datetime NOT NULL,
	        [branchno] smallint NOT NULL,
			[Arrears] money not null,
			[Deposit] money not null,
            [AgrmtTotal] money not null,
			[FullServiceChg] money not null,
			[ServiceChg] money not null,
			[ServiceChg_after12mths] money not null,
        	[DateDel] datetime not null,
			[DateFirst] datetime not null,
	        [DateLast] datetime not null,
			[InstalAmount] money not null,
            [FinInstalAmt] money not null,
			[InstalNo] smallint not null,
			[Instalno_after12mths] smallint not null,
        	[InstalFreq] char(1) not null,
			[Rebatetotal] money,
			[Rebatewithin12mths] money,
			[Rebateafter12mths] money,
            [Agrmtmths] smallint,
			[Deltot] money,
			[AgrmtDays] smallint,
			[Totalmonths] smallint,
			[Totalmonths_after12mths] smallint,
        	[Dtnetfirstin] varchar(1),
            [Agrmtyears] float,
			[AgrmtyearsRemain] smallint,		
			[Insincluded] smallint,
			[Inspcent] float,
			[Insamount] money,
			[InsamountRebated] money,			
			[InsamountRebatedwithin12mths] money,			
			[InsamountRebatedafter12mths] money,		
			[FullRebateDays] smallint,
			[Monthstogo] smallint,
			[Monthstogo_after12mths] smallint,
			[Monthstogofactor] money,
			[Monthstogofactor_after12mths] money,
			[Totalmonthsfactor] money,
			[Totalmonthsfactor_after12mths] money,
			[DelThresDate] datetime,
        	[Rule78] char(4),
			[Calcdone] char(1),
			[Rebatemonthsarrears] smallint,
			[Insplit] smallint,
			[Insplit_after12mths] smallint
		)  

		SELECT @return = @@error
	end

	IF (@return = 0)
	BEGIN
		if (select @acctno) in ('all','RPA')
		begin
			--all accounts
			insert into @rebates
			SELECT  Act.acctno,
				isnull(Act.termstype,''),
		        Act.DateAcctOpen,
		        Act.Branchno,
				isnull(Act.arrears, 0.00),
				isnull(A.deposit, 0.00),
	            isnull(A.agrmttotal, 0.00),
				convert(money,a.ServiceChg),
				convert(money,a.ServiceChg),
				convert(money,a.ServiceChg),
	        	isnull(A.datedel, '01-jan-1900'),
				isnull(I.datefirst, '01-jan-1900'),
		        isnull(I.datelast, '01-jan-1900'),
				isnull(I.instalamount, 0.00),
	            isnull(I.Fininstalamt, 0.00),
				isnull(I.instalno, 0.00),
				isnull(I.instalno, 0.00),
	        	isnull(I.instalfreq, ' '), 
				convert (money,0),
				convert (money,0),
				convert (money,0),
	            convert (smallint,0),
				convert (money,0),
				convert (smallint,0),
				convert (smallint,0),
				convert (smallint,0),
	        	convert (varchar (2),''),
	            convert (float, 0),
				convert (smallint,0),				
				convert (smallint, -99),
				convert (float,-99),
				convert (money,0),
				convert (money,0),					
				convert (money,0),					
				convert (money,0),					
				convert (smallint,0),
				convert (smallint,0),
				convert (smallint,0),
				convert (money,0),
				convert (money,0),
				convert (money,0),
				convert (money, 0),
				convert (datetime, D.DelThresDate, 111), --don't want time part
	        	convert (varchar(4),'78-1'), 
				convert (char (1),'N'),
				convert (smallint,0),
				convert (smallint,0),
				convert (smallint,0)
	        	FROM	acct Act
	        		INNER JOIN agreement A ON  Act.acctno = A.acctno
						       and Act.CurrStatus != 'S'
						       and Act.outstbal > 1
						       and Act.DateAcctOpen >= @fromdate
						       and Act.DateAcctOpen < dateadd(day,1,@UntilThresDate)
				INNER JOIN dbo.del_thres_reached d ON Act.acctno = d.acctno
	        		LEFT JOIN instalplan I ON A.acctno = I.acctno
  
		        SELECT @return = @@error
		end
		else
		begin
			--1 account
			insert into @rebates
			SELECT  Act.acctno,
				isnull(Act.termstype,''),
		        Act.DateAcctOpen,
		        Act.Branchno,
				isnull(Act.arrears, 0.00),
				isnull(A.deposit, 0.00),
	            isnull(A.agrmttotal, 0.00),
				convert(money,a.ServiceChg),
				convert(money,a.ServiceChg),
				convert(money,a.ServiceChg),
	        	isnull(A.datedel, '01-jan-1900'),
				isnull(I.datefirst, '01-jan-1900'),
		        isnull(I.datelast, '01-jan-1900'),
				isnull(I.instalamount, 0.00),
	            isnull(I.Fininstalamt, 0.00),
				isnull(I.instalno, 0.00),
				isnull(I.instalno, 0.00),
	        	isnull(I.instalfreq, ' '), 
				convert (money,0),
				convert (money,0),
				convert (money,0),
	            convert (smallint,0),
				convert (money,0),
				convert (smallint,0),
				convert (smallint,0),
				convert (smallint,0),
	        	convert (varchar (2),''),
	            convert (float, 0),
				convert (smallint,0),				
				convert (smallint, -99),
				convert (float,-99),
				convert (money,0),
				convert (money,0),					
				convert (money,0),					
				convert (money,0),				
				convert (smallint,0),
				convert (smallint,0),
				convert (smallint,0),
				convert (money,0),
				convert (money,0),
				convert (money,0),
				convert (money, 0),
				convert (datetime, '01-jan-1900', 111), --don't want time part
	        	convert (varchar(4),'78-1'), 
				convert (char (1),'N'),
				convert (smallint,0),
				convert (smallint,0),
				convert (smallint,0)
	        	FROM	acct Act
	        		INNER JOIN agreement A ON  Act.acctno = A.acctno
						       and Act.CurrStatus != 'S'
						       and Act.outstbal > 1
						       and Act.DateAcctOpen >= @fromdate
						       and Act.DateAcctOpen < dateadd(day,1,@UntilThresDate)
						       and Act.Acctno = @acctno
	        		LEFT JOIN instalplan I ON A.acctno = I.acctno
	  
		        SELECT @return = @@error

			declare @num int
			select @num = count(*) from @rebates
			   
			if @num = 0
			begin
				select 	@porebate = 0,
					@return = 0
					return   
			end
		end
	END

----------------
--Update Insplit if acct in instalmentvariable table
----------------
	if (@return = 0)
	begin
		update	r
		set	Insplit = 1
		from	instalmentvariable i,@rebates r
		where	i.acctno = r.acctno
		and	@rebatedate <= dateto
		and	instalorder = 1

	        SELECT @return = @@error
	end
	if (@return = 0)
	begin
		--else account must be in split 2
		update	r
		set	Insplit = 2
		from	instalmentvariable i,@rebates r
		where	i.acctno = r.acctno
		and	Insplit = 0

	        SELECT @return = @@error
	end

--accounts not in the intalmentvariable table will remain Insplit = 0

----------------
--update split data that's different to initial insert
----------------
	if (@return = 0)
	begin
		update	r
		set	Datefirst = Datefrom,
			Datelast = Dateto,
			Instalno = instalmentnumber,
			Instalamount = instalment,
			Servicechg = servicecharge
		from	instalmentvariable i,@rebates r
		where	i.acctno = r.acctno
		and	Insplit = instalorder

	        SELECT @return = @@error
	end

----------------
--Update Insplit_after12mths if acct in instalmentvariable table
----------------
	if (@return = 0)
	begin
		update	r
		set	Insplit_after12mths = 1
		from	instalmentvariable i,@rebates r
		where	i.acctno = r.acctno
		and	@RebateDate_after12mths <= dateto
		and	instalorder = 1

	        SELECT @return = @@error
	end
	if (@return = 0)
	begin
		--else in split 2
		update	r
		set	Insplit_after12mths = 2
		from	instalmentvariable i,@rebates r
		where	i.acctno = r.acctno
		and	Insplit_after12mths = 0

	        SELECT @return = @@error
	end

----------------
--update split data for after 12mths that's different to initial insert
----------------
	if (@return = 0)
	begin
		update	r
		set	Instalno_after12mths = instalmentnumber,
			Servicechg_after12mths = servicecharge
		from	instalmentvariable i,@rebates r
		where	i.acctno = r.acctno
		and	Insplit_after12mths = instalorder

	        SELECT @return = @@error
	end

----------------------
--UPDATE STATIC VALUES
----------------------
	if (@return = 0)
	begin
		--update delivery total from fintrans
        	update	r
	        set	DelTot = isnull((select sum(f.transvalue)
		        	 from	fintrans f
	        		 where	f.acctno = r.AcctNo
		        	 and	f.transtypecode IN ('DEL', 'REP', 'RPO', 'GRT', 'RDL', 'ADD','CLD')),0)					-- #10138
           FROM @rebates r
	        SELECT @return = @@error
	end
	IF (@return = 0)
	BEGIN
		if (select @acctno) in ('all','RPA')		-- jec 30/04/09
		begin
			--remove account from table if delivery total <= 0
		        delete from @rebates  where deltot <= 0

		        SELECT @return = @@error
		end
		else if (select deltot from @rebates) <= 0
		begin
			--return zero rebate for account
			select 	@porebate = 0,
				@return = 0
			return
		end
	END
	if (@return = 0)
	begin
		if (select @acctno) in ('all','RPA')		-- jec 30/04/09
		begin
			--update rule for calculation - only for rebate frame, otherwise must use 78-2
			if (select CountryCode from country) = 'M'
			begin
			        UPDATE @rebates
	        		SET    Rule78 = '78-2'
			        WHERE  termstype in ('04','05','06')
				AND    DelThresDate > @rulechangedate
	
			        SELECT @return = @@error
			end
			else
			begin		
                --CR888 Caribbean need to use rule 78-0 for all rebate frame totals regardless of delivery threshold date
    			if (select CountryCode from country) in ('N','B','Z','D','G','A','J','K','L','V','T')
                	begin
			        UPDATE @rebates 
		        	SET    Rule78 = '78+1' --CR900 changed to use rule 78+1 for the Caribbean

			        SELECT @return = @@error
                end
                else
                begin
			        UPDATE @rebates
		        	SET    Rule78 = '78-2'
		        	WHERE  DelThresDate NOT BETWEEN '01-Jan-1900' AND @RuleChangeDate
		
			        SELECT @return = @@error
                end
				--St Lucia update rule for calculation for accounts opened on/after 31/03/2008  CR938 jec 01/04/08
				if @CountryCode = 'L' and (select @acctno) not in ('all','RPA')		-- jec 30/04/09
				begin
			        UPDATE @rebates
	        		SET    Rule78 = 'Term'			-- indicates straight line over Term of agreement
			        WHERE  DateAcctOpen >= @StLuciaRuleChange
	
			        SELECT @return = @@error
				end
				
			end
		end
		else --not running frame so must calculate using rule in country parameter
		begin		
		
			--#19426 - CR18938 
			if(@CountryCode = 'T')
			begin
				declare @cashLoan bit 
				set @cashLoan = 0

				if exists(select 'a' from CashLoan where acctno = @acctno)
				begin
					set @cashLoan = 1

                    UPDATE @rebates
        		    SET    Rule78 =  case when @cashLoan = 1 and @RebateDate <  Convert(datetime,Convert(varchar(11),dateadd(month, @cashLoanEarlySettPenaltyPeriod, DateFirst)))  
										then '78' + convert(char(2),@cashLoanRule)
								 else '780' end
				end
                else 
                begin
				        UPDATE @rebates
        		        SET    Rule78 = '78' + convert(char(2),@rule)
                end 
			end
			else
			begin
				 --CR888 Use country parameter 'Rebate Calculation Rule' to determine rule for individual rebates
                IF EXISTS(SELECT 'a' FROM CashLoan WHERE AcctNo = @AcctNo)
                BEGIN
                    UPDATE @rebates
        		    SET    Rule78 = '78' + convert(char(2),@cashLoanRule)
                END
                ELSE
                BEGIN
				    UPDATE @rebates
        		    SET    Rule78 = '78' + convert(char(2),@rule)
                END
			end	

		--St Lucia update rule for calculation for accounts opened on/after 31/03/2008  CR938 jec 01/04/08
			if @CountryCode = 'L' and (select @acctno) not in ('all','RPA')		-- jec 30/04/09
					begin
                        IF EXISTS (SELECT 'a' FROM CashLoan WHERE acctno = @AcctNo) AND (SELECT value FROM CountryMaintenance WHERE CodeName = 'LoanRebateRule') != 'Term'
                        BEGIN
                            UPDATE @rebates
	        			    SET    Rule78 = '78' + CONVERT(CHAR(2),@cashLoanRule)
						    WHERE  DateAcctOpen >= @StLuciaRuleChange
                        END
                        ELSE
                        BEGIN
						    UPDATE @rebates
	        			    SET    Rule78 = 'Term'			-- indicates straight line over Term of agreement
						    WHERE  DateAcctOpen >= @StLuciaRuleChange
	                    END	
						SELECT @return = @@error
			end
		end
	end
	--update agreement months and agreement days, needed for datelast update (if not set already)
	if (@return = 0)
	begin
	        update	@rebates
		set	AgrmtMths = InstalNo - 1
		where	InstalFreq = 'M'

	        SELECT @return = @@error
	end
	if (@return = 0)
	begin
		update	@rebates
		set	AgrmtDays = (InstalNo - 1) * 14
		where	InstalFreq = N'F'

	        SELECT @return = @@error
	end
	if (@return = 0)
	begin
	        update	@rebates
		set	AgrmtDays = (InstalNo - 1) * 7  where InstalFreq = 'W'

	        SELECT @return = @@error
	end
	if (@return = 0)
	begin
 	        update	@rebates
		set	AgrmtMths = (InstalNo - 1) * 6  where InstalFreq = 'B'

	        SELECT @return = @@error
	end
	--update datelast if not set already
	IF (@return = 0)
	BEGIN
		update	@rebates
        	set	DateLast = DATEADD(mm, AgrmtMths, DateFirst)
	        where	AgrmtMths != 0
		and	isnull(datelast,'01-jan-1900') <= '01-jan-1910'
		and	isnull(datefirst,'01-jan-1900') > '01-jan-1910' --only set if datefirst is set

	        SELECT @return = @@error
	end
	IF (@return = 0)
	BEGIN	
	        update 	@rebates
        	set 	DateLast = DATEADD(dd, AgrmtDays, DateFirst)
	        where 	AgrmtMths = 0
		and 	AgrmtDays != 0

	        SELECT @return = @@error
	END

	IF (@return = 0)
	BEGIN
		--update dtnetfirstin and FullRebateDays from termstypetable
		update r
		set    DtNetFirstIn = t.dtnetfirstin,
		       FullRebateDays = t.FullRebateDays
		FROM   termstypetable t,@rebates r
		WHERE  r.termstype = t.TermsType

	        SELECT @return = @@error
	END
	--update inspcent, insincluded from intratehistory
	IF (@return = 0)
	BEGIN
	        update r
        	set    InsPcent	   = I.inspcent,
	               Insincluded = I.insincluded
	        FROM   IntRateHistory I,@rebates r
        	WHERE  r.termstype = I.TermsType
	        AND    DateAcctOpen >= I.datefrom
            AND    band in ('','A') --CR806 all bands will have same inspcent. If bands not implemented then will band will be blank. Date from and to of blank bands will not overlap with A,B,C,D bands.
        	AND    (DateAcctOpen <= I.dateto OR I.dateto = '01-jan-1900')

	        SELECT @return = @@error
	END
	--if intratehistory record not found then need to use earliest record in intratehistory
	IF (@return = 0)
	BEGIN
      DECLARE @intratehistory TABLE (termstype VARCHAR(2),earliest SMALLDATETIME)
      INSERT INTO @intratehistory(TermsType,earliest)
		select 	termstype, min(datefrom) --as earliest
		from	intratehistory
        where   band in ('','A') --CR806 all bands will have same inspcent. If bands not implemented then will band will be blank. Date from and to of blank bands will not overlap with A,B,C,D bands.
		group by termstype
      

      declare @intratehistory2 TABLE(TermsType VARCHAR(2),inspcent FLOAT,intrate FLOAT,intrate2 FLOAT,insincluded SMALLINT)
      INSERT INTO @intratehistory2 (TermsType,inspcent,intrate,intrate2,insincluded)
		select 	i2.termstype, inspcent, intrate, intrate2, insincluded
		from	@intratehistory i2, intratehistory i
		where	i2.termstype = i.termstype
		and	datefrom = earliest
        AND    band in ('','A') --CR806 all bands will have same inspcent. If bands not implemented then will band will be blank. Date from and to of blank bands will not overlap with A,B,C,D bands.

	        update 	r
        	set    	InsPcent    = I.inspcent,
	               	Insincluded = I.insincluded
	        FROM   	@intratehistory2 I,@rebates r
        	WHERE  	r.termstype = I.TermsType
		and	(r.InsPcent	= -99
		or	r.Insincluded	= -99)

	        SELECT @return = @@error
	END
	--67601 KEF if no record for termstype, whatever date then remove account from calculation as can't calculate - found in Fiji
	IF (@return = 0)
	BEGIN
			delete from @Rebates
			where (InsPcent	= -99
			or	Insincluded	= -99)

			SELECT @return = @@error
	end

	IF (@return = 0)
	BEGIN
		--update total months for accounts with no split interest rate should be calculated and not use instalno as historically some data in incorrect
		update @rebates
		set    TotalMonths = ISNULL((AgrmtTotal-Deposit-FinInstalAmt)/InstalAmount,0)+1.9,
		       TotalMonths_after12mths = ISNULL((AgrmtTotal-Deposit-FinInstalAmt)/InstalAmount,0)+1.9 --set the same if not split interest
		where  InstalAmount != 0
		and    Insplit = 0

		SELECT @return = @@error
	end
	IF (@return = 0)
	BEGIN
		--update total months for accounts with split interest rate - use instalno as can't calculate as don't have split agrmttotal
		update @rebates
		set    TotalMonths = Instalno
		where  Insplit <> 0

		SELECT @return = @@error
	end
	IF (@return = 0)
	BEGIN
		--update total months for accounts with split interest rate - use instalno as can't calculate as don't have split agrmttotal
		update @rebates
		set    TotalMonths_after12mths = Instalno_after12mths
		where  Insplit_after12mths <> 0

		SELECT @return = @@error
	end
--PRINT 'here'
	IF (@return = 0)
	BEGIN
		--update totalmonths again if instalfreq not monthly
		update @rebates
		set    TotalMonths = ((TotalMonths-1)*6)
		where  InstalFreq = 'B'
		and    Insplit = 0

		SELECT @return = @@error
	end
 	IF (@return = 0)
	BEGIN
		update @rebates
		set    TotalMonths = TotalMonths + ISNULL(DATEDIFF(mm, DateFirst, DateAcctOpen),0)
		where  DateAcctOpen < DateFirst
		and    InstalFreq = 'B'
		and    Insplit = 0

		SELECT @return = @@error
	end
	IF (@return = 0)
	BEGIN
		update @rebates
		set    TotalMonths = TotalMonths/2
		where  InstalFreq = 'F'
		and    Insplit = 0

		SELECT @return = @@error
	end
	IF (@return = 0)
	BEGIN
		update @rebates
		set    TotalMonths = TotalMonths/4
		where  InstalFreq = 'W'
		and    Insplit = 0

		SELECT @return = @@error
	end

	IF (@return = 0)
	BEGIN
		--update totalmonths again if dtnetfirstin set
		update @rebates
		set    TotalMonths = TotalMonths - 1,
		       TotalMonths_after12mths = TotalMonths_after12mths - 1
		where  DtNetFirstIn = 'Y'

		SELECT @return = @@error
	end

	IF (@return = 0)
	BEGIN
		if (select @acctno) in ('all','RPA')		-- jec 30/04/09
		begin
			--remove any accts where totalmonths <= 0
			delete from @rebates
			where 	TotalMonths <= 0

			SELECT @return = @@error
		end
		else if (select totalmonths from @rebates) <= 0
		begin
			--return zero rebate for account
			select 	@porebate = 0,
				@return = 0

			return
		end
	END

	IF (@return = 0)
	BEGIN
		--update agrmtyears
		update 	@rebates
		set	AgrmtYears = CONVERT(FLOAT,TotalMonths)/12, 
			-- This code will give AgrmtYearsRemain = zero when @RebateDate > date of last instalment
			AgrmtYearsRemain = (DATEDIFF(dd,@RebateDate,DateLast) / 365	+
								ABS(DATEDIFF(dd,@RebateDate,DateLast) / 365)) / 2 -- CR938 jec 01/04/08
		where	insplit = 0

		SELECT @return = @@error
	end
	IF (@return = 0)
	BEGIN
		--update agrmtyears
		update 	r
		set	AgrmtYears = CONVERT(FLOAT,i.Instalno)/12 --need to use total instalno, not splits --can rely on instalno to be correct now (historically we calculated totalmonths as couldn't trust instalno)
		from	instalplan i,@rebates r
		where	insplit <> 0
		and	i.acctno = r.acctno

		SELECT @return = @@error
	end

	--update insurance amount
	IF (@return = 0)
	BEGIN
	-- 69764-6 St Lucia change ins calc SL
	if (@countrycode = 'S' or @countrycode = 'H' or @countrycode = 'I' or @countrycode = 'Y' 
		or (@countrycode = 'L' and @acctno not in ('all','RPA')))		-- jec 30/04/09)
 		begin /* KEF 68430 Negative rebates fix, need to remove deposit as Insurance is calculated without it. Agreed with Kim and Raymond */
			update 	@rebates
			set    	InsAmount = round(((convert(float,AgrmtTotal) - convert(float,Deposit) - convert(float,ServiceChg)) * convert(float,AgrmtYears) * (convert(float,InsPCent) / 100)) ,2)

			where	insincluded = 1

			SELECT @return = @@error
		end
		else /* KEF 68430 Negative rebates fix, other countries don't want this change so leaving calculation as is */
		begin
			update 	@rebates
			set    	InsAmount = round((convert(float,AgrmtTotal) * convert(float,AgrmtYears) * (convert(float,InsPCent) / 100)) ,2)
			where	insincluded = 1

			SELECT @return = @@error
		end

		IF (@return = 0)
			BEGIN
			--St Lucia update rule for calculation for accounts opened on/after 31/03/2008  CR938 jec 01/04/08
				if @CountryCode = 'L' and (select @acctno) not in ('all','RPA')		-- jec 30/04/09
					begin
						UPDATE @rebates
    					set InsAmountRebated=InsAmount		--, InsAmountRebatedWithin12mths=InsAmount, InsAmountRebatedAfter12mths=InsAmount
							where rule78 ='Term'		-- Update all rebates where rule78 = 'Term'
							
						SELECT @return = @@error
				end
			End
		IF @return = 0
		 if (@countrycode = 'Y')
         BEGIN 
            --Accounts delivered before 01/05/2005 have insurance of 1.25%
		    update @rebates
		    set    ServiceChg = (ServiceChg - ((AgrmtTotal - FullServiceChg)*0.0125)),
		    	   ServiceChg_after12mths = (ServiceChg_after12mths - ((AgrmtTotal - FullServiceChg)*0.0125))
            where  DelThresDate < '01-may-2005'

            --Accounts delivered between 01/05/2005 and 27/08/2006 have insurance of 0.90%
		    update @rebates
		    set    ServiceChg = (ServiceChg - ((AgrmtTotal - FullServiceChg)*0.009)),
		    	   ServiceChg_after12mths = (ServiceChg_after12mths - ((AgrmtTotal - FullServiceChg)*0.009))
            where  DelThresDate between '01-may-2005' and '27-aug-2006'

            --Accounts delivered after 27/08/2006 have the calculated insurance premium excluded from the rebate calc.
   			update @rebates
		    set    ServiceChg = (ServiceChg - InsAmount),
		    	   ServiceChg_after12mths = (ServiceChg_after12mths - InsAmount)
            where  DelThresDate > '27-aug-2006'
        END 
		ELSE 
		BEGIN
			update @rebates
		    	set    ServiceChg = (ServiceChg - InsAmount),
		    	       ServiceChg_after12mths = (ServiceChg_after12mths - InsAmount)

		    	SELECT @return = @@error
		end
	END

-----------------------
--UPDATE CHANGABLE DATA part 1
-----------------------
	--update rebate for accounts valid for full refund if paid within FullRebateDays value 
	--remove 7 days from rebate calculation date
	--rebate = full service charge (including insurance) if account within FullRebateDays of delivery date
	IF (@return = 0)
	BEGIN
		update	@rebates
		set	rebatetotal = fullservicechg,
			calcdone = 'Y'
		where	FullRebateDays > 0
		and	DATEDIFF(dd,ISNULL(datedel,'01-jan-1900'),@RebateDate) <= FullRebateDays
	 	and	isnull(DateDel,'01-jan-1900') >= '01-jan-1910'

		SELECT @return = @@error
	end

	IF (@return = 0)
   		BEGIN
		--return rebate if running for 1 account and within FullRebateDays value
		if (select @acctno) not in ('all','RPA')		-- jec 30/04/09
		begin
			--only return if rebate was set
			if (select calcdone from @rebates ) = 'Y'
			begin
				select 	@porebate = rebatetotal,
					@return = 0
				from	@rebates

			    	return
			end
		end
	end

------------------------------------------------------
--UPDATE CHANGABLE DATA part 2 for main r table
------------------------------------------------------
	--update monthstogo
	IF (@return = 0)
	BEGIN	
		UPDATE @rebates
		SET    MonthsToGo = (select case
				     when   day(@RebateDate) <= day(Datelast)
				     then   datediff(month,@RebateDate,Datelast)
				     else   datediff(month,@RebateDate,Datelast) - 1
				     end)

		SELECT @return = @@error
	END

	--set rebate to zero if monthstogo <= 0
	IF (@return = 0)
	BEGIN
		if (select @acctno) in ('all','RPA')		-- jec 30/04/09
		begin
		        update 	@rebates
	      		set 	rebatetotal = 0,
				rebatewithin12mths = 0,
				rebateafter12mths = 0,
				calcdone = 'Y'
			where	MonthsToGo <= 0
			and    	calcdone = 'N'
			and	Insplit <> 1 --don't make rebate zero if in split 1 as rebate=servicechg until in 2nd split

			SELECT @return = @@error
		end
		else if (select monthstogo from @rebates where Insplit <> 1) <= 0
		begin
			--return zero rebate for account
			select 	@porebate = 0,
				@return = 0

			return
		end
	end

	--update MonthsToGo again if bigger than totalmonths
	IF (@return = 0)
	BEGIN
       --KEF 69596 Mauritius, Madagascar, Singapore, Thailand and Indonesia will only reduce by 1 if monthstogo exceeds totalmonths. If it equals then will leave.
		IF (@CountryCode = 'S' OR @CountryCode= 'H' OR @CountryCode= 'I' OR @CountryCode= 'M' OR @CountryCode= 'C')
		begin
    		update 	@rebates
    		set    	MonthsToGo = TotalMonths - 1
    		where  	MonthsToGo > TotalMonths --remove equals sign
    		and    	calcdone = 'N'
        end
		else
		begin
    		update 	@rebates
    		set    	MonthsToGo = TotalMonths - 1
    		where  	MonthsToGo >= TotalMonths
    		and    	calcdone = 'N'
        end

		SELECT @return = @@error
	end

	--if monthstogo > 12 then calc for 12 months time
	--update monthstogo_after12mths
	IF (@return = 0)
	BEGIN	
		UPDATE 	@rebates
		SET    	MonthsToGo_after12mths = MonthsToGo - 12
		WHERE	MonthsToGo > 12

		SELECT @return = @@error
	END
	--if insplit = 1 and insplit_after12mths = 2 then will need to use split 2 datelast
	IF (@return = 0)
	BEGIN	
		UPDATE r
		SET    MonthsToGo_after12mths = (select case
				     when   day(@RebateDate_after12mths) <= day(Dateto)
				     then   datediff(month,@RebateDate_after12mths,Dateto)
				     else   datediff(month,@RebateDate_after12mths,Dateto) - 1
				     end)
		from	instalmentvariable i,@rebates r
		where	i.acctno = r.acctno
		and	insplit = 1
		and	insplit_after12mths = 2
		and	instalorder = 2 --datelast for 2nd split

		SELECT @return = @@error
	END

	--update MonthsToGo_after12mths to zero if -ve
	IF (@return = 0)
	BEGIN	
		UPDATE 	@rebates
		SET    	MonthsToGo_after12mths = 0
		where	MonthsToGo_after12mths < 0

		SELECT @return = @@error
	END

	--update MonthsToGoFactor for rule 78-2 for rebate total
	IF (@return = 0)
	BEGIN
		update 	@rebates
		set 	MonthsToGoFactor = (MonthsToGo - ((left(right(rule78,2),2)*-1)-1)) * (MonthsToGo - (left(right(rule78,2),2)*-1))--, --CR900
		where   calcdone = 'N'
			and rule78 !='Term'		-- Update all rebates where rule78 not 'Term'	CR938 jec 01/04/08

		SELECT @return = @@error
	end

	IF (@return = 0)
	BEGIN
	--St Lucia update rule for calculation for accounts opened on/after 31/03/2008  CR938 jec 01/04/08
		if @CountryCode = 'L' and (select @acctno) not in ('all','RPA')		-- jec 30/04/09
			begin
				UPDATE @rebates
    			set 	MonthsToGoFactor = MonthsToGo
					where   calcdone = 'N'
					and rule78 ='Term'		-- Update all rebates where rule78 = 'Term'

				SELECT @return = @@error
		end
	End

	--update MonthsToGoFactor for rule 78-2 for rebateafter12mths
	IF (@return = 0)
	BEGIN
		update 	@rebates
		set 	MonthsToGoFactor_after12mths = (MonthsToGo_after12mths - ((left(right(rule78,2),2)*-1)-1)) * (MonthsToGo_after12mths - (left(right(rule78,2),2)*-1))--, --CR900
		where  	calcdone = 'N'
		and	    MonthsToGo_after12mths > 0	--needed to avoid incorrect results
		and rule78 !='Term'		-- Update all rebates where rule78 not 'Term'	CR938 jec 01/04/08

		SELECT @return = @@error
	end

	IF (@return = 0)
	BEGIN
	--St Lucia update rule for calculation for accounts opened on/after 31/03/2008  CR938 jec 01/04/08
		if @CountryCode = 'L' and (select @acctno) not in ('all','RPA')		-- jec 30/04/09
			begin
				UPDATE @rebates
    			set 	MonthsToGoFactor_after12mths = MonthsToGo_after12mths
					where   calcdone = 'N'
					and	    MonthsToGo_after12mths > 0	--needed to avoid incorrect results
					and rule78 ='Term'		-- Update all rebates where rule78 = 'Term'

				SELECT @return = @@error
		end
	End

	--update TotalMonthsFactor
	IF (@return = 0)
	BEGIN
		update	@rebates
		set 	TotalMonthsFactor = TotalMonths * (TotalMonths + 1),
			TotalMonthsFactor_after12mths = TotalMonths_after12mths * (TotalMonths_after12mths + 1)

		SELECT @return = @@error
	end
	IF (@return = 0)
	BEGIN
	--St Lucia update rule for calculation for accounts opened on/after 31/03/2008  CR938 jec 01/04/08
		if @CountryCode = 'L' and (select @acctno) not in ('all','RPA')		-- jec 30/04/09
			begin
				UPDATE @rebates
    			set 	TotalMonthsFactor = TotalMonths, TotalMonthsFactor_after12mths = TotalMonths_after12mths
					where  rule78 ='Term'		-- Update all rebates where rule78 = 'Term'

				SELECT @return = @@error
		end
	End

	-- Calculate CPI Rebate - St. Lucia Only at present
	IF (@return = 0)
	BEGIN
	--St Lucia update rule for calculation for accounts opened on/after 31/03/2008  CR938 jec 01/04/08
		if @CountryCode = 'L' and (select @acctno) not in ('all','RPA')		-- jec 30/04/09
			begin
				UPDATE @rebates
    			set 	InsamountRebated = ((Insamount * AgrmtyearsRemain / Agrmtyears) * @RebPCent / 100)
					where  totalmonthsfactor > 0
					and calcdone = 'N'
					and	 rule78 ='Term'		-- Update all rebates where rule78 = 'Term'
					
				SELECT @return = @@ERROR
		-- Calculate CPI Rebate in 12 months
				IF (@return = 0)
				BEGIN
					UPDATE @rebates
    				set 	InsamountRebatedafter12mths = ((Insamount * (AgrmtyearsRemain - 1) / Agrmtyears) * @RebPCent / 100)
						where  totalmonthsfactor_after12mths > 0
						and (AgrmtyearsRemain - 1) > 0		-- must have at least a year remaining on term
						and calcdone = 'N'
						and	 rule78 ='Term'		-- Update all rebates where rule78 = 'Term'
						
					SELECT @return = @@ERROR
				End
			end
	End
	

	--calculate rebatetotal
	IF (@return = 0)
	BEGIN
		update	@rebates
		set	rebatetotal = ((ServiceChg * MonthsToGoFactor / TotalMonthsFactor) * @RebPCent / 100) + InsamountRebated,  --CR938 jec 01/04/08
 --can't do here as need to exclude if totalmonthsfactor_after12mths = 0 to avoid divide by zero errors, can't do in same
 --query as could exclude accounts with valid rebatetotal
			calcdone = 'P'	--may need to redo for split accounts
		where	totalmonthsfactor > 0
		and	calcdone = 'N'

		SELECT @return = @@error
	end

	--calculate rebatetotal for after12mths
	IF (@return = 0)
	BEGIN
		update	@rebates
		set	rebateafter12mths = ((ServiceChg_After12mths * MonthsToGoFactor_after12mths / TotalMonthsFactor_after12mths) 
							* @RebPCent / 100) + InsamountRebatedafter12mths  --CR938 jec 01/04/08
		where	totalmonthsfactor_after12mths > 0
		and	calcdone = 'P' --only update accts that were set above

		SELECT @return = @@error
	end

--------------------
--recalculate rebate for variable interest and instalments
--------------------
	--if acct in split 1 then check if rebate should be servicechg (including insamount) for split 2
	IF (@return = 0)
	BEGIN
		--find accts still in first split and calc rebate < servicecharge for 2nd split
      DECLARE @rebates_service TABLE(acctno CHAR(12) PRIMARY KEY,servicechgfor2ndsplit MONEY)
      INSERT INTO @rebates_service(acctno,servicechgfor2ndsplit)
		select	r.acctno, servicecharge --as servicechgfor2ndsplit
		from 	instalmentvariable i, @rebates r
		where 	Insplit = 1
		and	i.acctno = r.acctno
		and	rebatetotal < servicecharge
		and	Instalorder = 2
		and	calcdone = 'P'	--don't want accounts that have already been set as full service charge or set to zero as invalid for rebate

		SELECT @return = @@error
	end
	IF (@return = 0)
	BEGIN
		--update rebate
		update	ra
		set	rebatetotal = servicechgfor2ndsplit,
			calcdone = 'S' --may need to update after12tmhs still
		from	@rebates_service r,@rebates ra
		where  	ra.acctno = r.acctno
		and	calcdone = 'P'

		SELECT @return = @@error
	end

	--check if still in split 1 in 12 moths time, if so repeat above check for correct rebate
	IF (@return = 0)
	BEGIN
		--find accts still in first split in 12 mths and calc rebate < servicecharge for 2nd split
      DECLARE @rebates_service_after12mths TABLE (acctno CHAR(12) PRIMARY KEY,servicechgfor2ndsplit MONEY)
      INSERT INTO @rebates_service_after12mths(acctno,servicechgfor2ndsplit)
		select	r.acctno, servicecharge as servicechgfor2ndsplit
		from 	instalmentvariable i, @rebates r
		where 	Insplit_after12mths = 1
		and	i.acctno = r.acctno
		and	r.rebateafter12mths < servicecharge
		and	Instalorder = 2
		and	calcdone = 'S'	
		SELECT @return = @@error
	end
	IF (@return = 0)
	BEGIN
		--update rebate
		update	reb
		set	rebateafter12mths = servicechgfor2ndsplit,
			calcdone = 'Y'
		from	@rebates_service_after12mths r,@rebates reb
		where  	reb.acctno = r.acctno

		SELECT @return = @@error
	end

	--Round to the required precision
	IF (@return = 0)
	BEGIN
		IF (@CountryCode = 'I' OR @CountryCode= 'C')
		begin
			SELECT @Nearest = 100

			SELECT @return = @@error
		end
		ELSE IF @NoCents = 1
		begin
			SELECT @Nearest = 1

			SELECT @return = @@error
		end
	end
    IF (@return = 0)
    BEGIN
    	IF (@Nearest = 1) --no cents is set
		BEGIN
        	UPDATE 	@rebates
		 	SET 	rebatetotal = FLOOR(rebatetotal), --Round down to nearest 1
				rebateafter12mths = FLOOR(rebateafter12mths),
				InsamountRebated = FLOOR(InsamountRebated),
				InsamountRebatedafter12mths = FLOOR(InsamountRebatedafter12mths)	-- CR938 jec 02/04/08

   			SELECT @return = @@error
   		end
   		ELSE IF (@Nearest = 100) --Large currency, Ind or Mad
   		BEGIN
   	    	UPDATE 	@rebates
		 	SET 	rebatetotal = ROUND(floor(rebatetotal),-2,1), -- Floor then round to nearest 100
				rebateafter12mths = ROUND(floor(rebateafter12mths),-2,1),
				InsamountRebated = ROUND(FLOOR(InsamountRebated),-2,1),
				InsamountRebatedafter12mths = ROUND(FLOOR(InsamountRebatedafter12mths),-2,1)	-- CR938 jec 02/04/08

  			SELECT @return = @@error
       	END
		else --Always round to two decimal places max precision at least
		begin
   	    	UPDATE 	@rebates
		 	SET 	rebatetotal = ROUND(rebatetotal,2), --2 decimal places
				rebateafter12mths = ROUND(rebateafter12mths,2),
				InsamountRebated = ROUND(InsamountRebated,2),
				InsamountRebatedafter12mths = ROUND(InsamountRebatedafter12mths,2)	-- CR938 jec 02/04/08

	    	SELECT @return = @@error
		end
	END

	DECLARE @RoundRebate CHAR(5)
	SELECT @RoundRebate = value FROM countrymaintenance WHERE name = 'Round Service Charge'

	IF(@RoundRebate = 'True')
	BEGIN
		UPDATE 	@rebates
		SET 	rebatetotal = case
			when (rebatetotal*10-floor(rebatetotal*10))between .1 and .5 then (floor(rebatetotal*10)+.5)/10
			when (rebatetotal*10-floor(rebatetotal*10))> .5 then (floor(rebatetotal*10)+1.0)/10
			else rebatetotal
			END, --0.05 decimal places
				rebateafter12mths = ROUND(rebateafter12mths,2)
		
		SELECT @return = @@error

	END

    --CR900 - Check if rebate is larger than service charge - insurance, if larger make rebate = service charge - insurance
    IF (@return = 0)
    BEGIN
        UPDATE 	@rebates
		SET 	rebatetotal = FullServiceChg
	    WHERE   rebatetotal > FullServiceChg
        and     FullServiceChg > 0

    	SELECT @return = @@error
    END

-------------------------------------------------------------------------
--return rebate if single account as don't need to do anything below here
-------------------------------------------------------------------------
	if (select @acctno) not in ('all','RPA')		-- jec 30/04/09
	begin
		select 	@porebate = rebatetotal
		from	@rebates

	    	return
	end

--------------------------------------------
--do further updates required for frame only
--------------------------------------------
	--check rebateafter12mths is not < 0
	IF (@return = 0)
	begin
		update	@rebates
		set	rebateafter12mths = 0
		where	rebateafter12mths < 0

	    	SELECT @return = @@error
	end

	--set rebatewithin12mths
	IF (@return = 0)
	begin
		update	@rebates
		set	rebatewithin12mths = rebatetotal - rebateafter12mths,
			InsamountRebatedWithin12mths = InsamountRebated - InsamountRebatedafter12mths	-- CR938 jec 01/04/08

	    	SELECT @return = @@error
	end

-------------------------------
--calculate rebatemonthsarrears
-------------------------------
	IF (@return = 0)
	begin
		update	@rebates
		set	Rebatemonthsarrears = isnull(cast(0.5+(Arrears/InstalAmount) as integer),0)
		WHERE 	cast(0.5+(Arrears/InstalAmount) as float) < 1000
		AND 	InstalAmount > 1

	    	SELECT @return = @@error
	end

	if (select @acctno) = 'all'		-- jec 30/04/09
	begin
---------------------------------
--insert rebate values into table
---------------------------------
	IF (@return = 0)
	begin	
		insert into rebates
			(acctno, branchno, termstype, DateAcctOpen, Arrears, Deposit, AgrmtTotal,
			FullServiceChg, ServiceChg, ServiceChg_after12mths, DateDel, DateFirst,
			DateLast, InstalAmount, FinInstalAmt, InstalNo, InstalNo_after12mths,
			InstalFreq, Rebate, Rebatewithin12mths, Rebateafter12mths, Agrmtmths,
			Deltot, AgrmtDays, Totalmonths, Totalmonths_after12mths, Dtnetfirstin,
			Agrmtyears, Insincluded, Inspcent, Insamount,
			FullRebateDays, Monthstogo, Monthstogo_after12mths, Monthstogofactor,
			Monthstogofactor_after12mths, Totalmonthsfactor, Totalmonthsfactor_after12mths,
			DelThresDate, Rule78, Calcdone, monthsarrears, insplit, insplit_after12mths,
			AgrmtyearsRemain,InsamountRebated,InsamountRebatedwithin12mths,InsamountRebatedafter12mths)	--CR938 jec 02/04/08
 		select	acctno, branchno, termstype, DateAcctOpen, Arrears, Deposit, AgrmtTotal,
			FullServiceChg, ServiceChg, ServiceChg_after12mths, DateDel, DateFirst, 
			DateLast, InstalAmount, FinInstalAmt, InstalNo, InstalNo_after12mths,
			InstalFreq, Rebatetotal, Rebatewithin12mths, Rebateafter12mths, Agrmtmths,
			Deltot, AgrmtDays, Totalmonths, Totalmonths_after12mths, Dtnetfirstin,
			Agrmtyears, Insincluded, Inspcent, Insamount,
			FullRebateDays, Monthstogo, Monthstogo_after12mths, Monthstogofactor,
			Monthstogofactor_after12mths, Totalmonthsfactor, Totalmonthsfactor_after12mths,
			DelThresDate, Rule78, Calcdone, Rebatemonthsarrears, insplit, insplit_after12mths,
			AgrmtyearsRemain,InsamountRebated,InsamountRebatedwithin12mths,InsamountRebatedafter12mths	--CR938 jec 02/04/08
		from	@rebates

	    	SELECT @return = @@error
	end

----------------------------------------
--populate rebates_total table by branch
-----------------------------------------
	IF (@return = 0)
	begin
		insert into rebates_total (sequence, branchno, arrearsgroup, rebate, rebatewithin12mths,
					   rebateafter12mths,PeriodEndDate)
		SELECT 	1, branchno, 'IN ADV/UP TO 1 MTH', sum(rebate), sum(rebatewithin12mths),
			sum(rebateafter12mths),@RebateDate		-- CR937 06/05/09
		FROM	rebates
		WHERE	monthsarrears !> 1
		GROUP BY branchno

		insert into rebates_total (sequence, branchno, arrearsgroup, rebate, rebatewithin12mths,
					   rebateafter12mths,PeriodEndDate)
		SELECT 	2, branchno, '>1 MTH UP TO 2 MTH', sum(rebate), sum(rebatewithin12mths),
			sum(rebateafter12mths),@RebateDate		-- CR937 06/05/09
		FROM	rebates
		WHERE 	monthsarrears !> 2
		AND 	monthsarrears > 1
		GROUP BY branchno

		insert into rebates_total (sequence, branchno, arrearsgroup, rebate, rebatewithin12mths,
					   rebateafter12mths,PeriodEndDate)
		SELECT 	3, branchno, '>2 MTH UP TO 3 MTH', sum(rebate), sum(rebatewithin12mths),
			sum(rebateafter12mths),@RebateDate		-- CR937 06/05/09
		FROM	rebates
		WHERE 	monthsarrears !> 3
		AND 	monthsarrears > 2
		GROUP BY branchno

		insert into rebates_total (sequence, branchno, arrearsgroup, rebate, rebatewithin12mths,
					   rebateafter12mths,PeriodEndDate)
		SELECT 	4, branchno, '>3 MTH UP TO 4 MTH', sum(rebate), sum(rebatewithin12mths),
			sum(rebateafter12mths),@RebateDate		-- CR937 06/05/09
		FROM	rebates
		WHERE 	monthsarrears !> 4
		AND 	monthsarrears > 3
		GROUP BY branchno

		insert into rebates_total (sequence, branchno, arrearsgroup, rebate, rebatewithin12mths,
					   rebateafter12mths,PeriodEndDate)
		SELECT 	5, branchno, '>4 MTH UP TO 6 MTH', sum(rebate), sum(rebatewithin12mths),
			sum(rebateafter12mths),@RebateDate		-- CR937 06/05/09
		FROM	rebates
		WHERE 	monthsarrears !> 6
		AND 	monthsarrears > 4
		GROUP BY branchno

		insert into rebates_total (sequence, branchno, arrearsgroup, rebate, rebatewithin12mths,
					   rebateafter12mths,PeriodEndDate)
		SELECT 	6, branchno, '>6 MTH UP TO 12 MTH', sum(rebate), sum(rebatewithin12mths),
			sum(rebateafter12mths),@RebateDate		-- CR937 06/05/09
		FROM	rebates
		WHERE 	monthsarrears !> 12
		AND 	monthsarrears > 6
		GROUP BY branchno

		insert into rebates_total (sequence, branchno, arrearsgroup, rebate, rebatewithin12mths,
					   rebateafter12mths,PeriodEndDate)
		SELECT 	7, branchno, '> 12 MTH', sum(rebate), sum(rebatewithin12mths),
				sum(rebateafter12mths),@RebateDate		-- CR937 06/05/09
		FROM	rebates
		WHERE 	monthsarrears > 12
		GROUP BY branchno

		insert into rebates_total (sequence, branchno, arrearsgroup, rebate, rebatewithin12mths,
					   rebateafter12mths,PeriodEndDate)
		SELECT 	8, branchno, 'TOTALS', sum(rebate), sum(rebatewithin12mths),
			sum(rebateafter12mths),@RebateDate		-- CR937 06/05/09
		FROM	rebates
		GROUP BY branchno

	    	SELECT @return = @@error
	end
	
----------------------------------------------------
--Run rebate forecast if actual date is a Period End 
-----------------------------------------------------
	IF (@return = 0)
	begin
	
	if @nextPEdate is not null		-- PEdate when run at period end between 21:00 and 04:00 (next day)
		and exists (select * from RebateForecast_PeriodEndDates
						where enddate = @nextPEdate and rundate='1900-01-01 00:00:00.000')
		Begin
			exec DN_RebateForecastSP @PeriodEndchar,@returnRF out
			SELECT @return = @@error
		End		
	End
	End		-- jec 30/04/09
	
-----------------------------------------------------------------------------------------
-- Running from Rebate Partial Accrual report - output to rebates_RPA tables
------------------------------------------------------------------------------------------	
	if (select @acctno) = 'RPA'		-- jec 30/04/09
	begin
---------------------------------
--insert rebate values into table
---------------------------------
	IF (@return = 0)
	begin	
		insert into rebates_RPA
			(acctno, branchno, termstype, DateAcctOpen, Arrears, Deposit, AgrmtTotal,
			FullServiceChg, ServiceChg, ServiceChg_after12mths, DateDel, DateFirst,
			DateLast, InstalAmount, FinInstalAmt, InstalNo, InstalNo_after12mths,
			InstalFreq, Rebate, Rebatewithin12mths, Rebateafter12mths, Agrmtmths,
			Deltot, AgrmtDays, Totalmonths, Totalmonths_after12mths, Dtnetfirstin,
			Agrmtyears, Insincluded, Inspcent, Insamount,
			FullRebateDays, Monthstogo, Monthstogo_after12mths, Monthstogofactor,
			Monthstogofactor_after12mths, Totalmonthsfactor, Totalmonthsfactor_after12mths,
			DelThresDate, Rule78, Calcdone, monthsarrears, insplit, insplit_after12mths,
			AgrmtyearsRemain,InsamountRebated,InsamountRebatedwithin12mths,InsamountRebatedafter12mths)	--CR938 jec 02/04/08
 		select	acctno, branchno, termstype, DateAcctOpen, Arrears, Deposit, AgrmtTotal,
			FullServiceChg, ServiceChg, ServiceChg_after12mths, DateDel, DateFirst, 
			DateLast, InstalAmount, FinInstalAmt, InstalNo, InstalNo_after12mths,
			InstalFreq, Rebatetotal, Rebatewithin12mths, Rebateafter12mths, Agrmtmths,
			Deltot, AgrmtDays, Totalmonths, Totalmonths_after12mths, Dtnetfirstin,
			Agrmtyears, Insincluded, Inspcent, Insamount,
			FullRebateDays, Monthstogo, Monthstogo_after12mths, Monthstogofactor,
			Monthstogofactor_after12mths, Totalmonthsfactor, Totalmonthsfactor_after12mths,
			DelThresDate, Rule78, Calcdone, Rebatemonthsarrears, insplit, insplit_after12mths,
			AgrmtyearsRemain,InsamountRebated,InsamountRebatedwithin12mths,InsamountRebatedafter12mths	--CR938 jec 02/04/08
		from	@rebates

	    	SELECT @return = @@error
	end

----------------------------------------
--populate rebates_total table by branch
-----------------------------------------
	IF (@return = 0)
	begin
		insert into rebates_total_RPA (sequence, branchno, arrearsgroup, rebate, rebatewithin12mths,
					   rebateafter12mths)
		SELECT 	1, branchno, 'IN ADV/UP TO 1 MTH', sum(rebate), sum(rebatewithin12mths),
			sum(rebateafter12mths)
		FROM	rebates_RPA
		WHERE	monthsarrears !> 1
		GROUP BY branchno

		insert into rebates_total_RPA (sequence, branchno, arrearsgroup, rebate, rebatewithin12mths,
					   rebateafter12mths)
		SELECT 	2, branchno, '>1 MTH UP TO 2 MTH', sum(rebate), sum(rebatewithin12mths),
			sum(rebateafter12mths)
		FROM	rebates_RPA
		WHERE 	monthsarrears !> 2
		AND 	monthsarrears > 1
		GROUP BY branchno

		insert into rebates_total_RPA (sequence, branchno, arrearsgroup, rebate, rebatewithin12mths,
					   rebateafter12mths)
		SELECT 	3, branchno, '>2 MTH UP TO 3 MTH', sum(rebate), sum(rebatewithin12mths),
			sum(rebateafter12mths)
		FROM	rebates_RPA
		WHERE 	monthsarrears !> 3
		AND 	monthsarrears > 2
		GROUP BY branchno

		insert into rebates_total_RPA (sequence, branchno, arrearsgroup, rebate, rebatewithin12mths,
					   rebateafter12mths)
		SELECT 	4, branchno, '>3 MTH UP TO 4 MTH', sum(rebate), sum(rebatewithin12mths),
			sum(rebateafter12mths)
		FROM	rebates_RPA
		WHERE 	monthsarrears !> 4
		AND 	monthsarrears > 3
		GROUP BY branchno

		insert into rebates_total_RPA (sequence, branchno, arrearsgroup, rebate, rebatewithin12mths,
					   rebateafter12mths)
		SELECT 	5, branchno, '>4 MTH UP TO 6 MTH', sum(rebate), sum(rebatewithin12mths),
			sum(rebateafter12mths)
		FROM	rebates_RPA
		WHERE 	monthsarrears !> 6
		AND 	monthsarrears > 4
		GROUP BY branchno

		insert into rebates_total_RPA (sequence, branchno, arrearsgroup, rebate, rebatewithin12mths,
					   rebateafter12mths)
		SELECT 	6, branchno, '>6 MTH UP TO 12 MTH', sum(rebate), sum(rebatewithin12mths),
			sum(rebateafter12mths)
		FROM	rebates_RPA
		WHERE 	monthsarrears !> 12
		AND 	monthsarrears > 6
		GROUP BY branchno

		insert into rebates_total_RPA (sequence, branchno, arrearsgroup, rebate, rebatewithin12mths,
					   rebateafter12mths)
		SELECT 	7, branchno, '> 12 MTH', sum(rebate), sum(rebatewithin12mths),
				sum(rebateafter12mths)
		FROM	rebates_RPA
		WHERE 	monthsarrears > 12
		GROUP BY branchno

		insert into rebates_total_RPA (sequence, branchno, arrearsgroup, rebate, rebatewithin12mths,
					   rebateafter12mths)
		SELECT 	8, branchno, 'TOTALS', sum(rebate), sum(rebatewithin12mths),
			sum(rebateafter12mths)
		FROM	rebates_RPA
		GROUP BY branchno

	    	SELECT @return = @@error
	end
	
	End		-- jec 30/04/09

IF @return != 0
	BEGIN
	   	
		SET @query_text = convert(varchar,@return) + ' ' + 'Rebate Report data failed'

-- pass error back. .net will handle pass/fail etc
     raiserror (@query_text,1,1)

	END

GO