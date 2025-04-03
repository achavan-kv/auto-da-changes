if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_RebateForecastSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_RebateForecastSP]
GO

CREATE PROCEDURE [dbo].[DN_RebateForecastSP]
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_RebateForecastSP.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Rebate Forecast  
-- Author       : ??
-- Date         : ??
--
-- This procedure will update the Rebate Forecast report A/B/C/D tables
-- for the current period end
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 05/05/09 jec  Correct sequence in report A & B for '>12 Mths'
-- 07/05/09 jec  CR937 Add PeriodEndDateSP procedure to calculate/Add future PE dates
-- 08/02/10 jec CR909 Malaysia v4 merge
-- 05/10/12  jec #10138 - LW75030 - SUCR - Cash Loan 
-- ================================================
	-- Add the parameters for the stored procedure here
    @PeriodEndchar varchar(12),
    @return INT = 0 OUTPUT
AS
--------------------
--DECLARE PARAMETERS
--------------------
    declare
	@RuleChangeDate	datetime,
	@FromThresDate	datetime,
	@UntilThresDate	datetime,
	@FromThresDateReportB	datetime,
	@FromDate	datetime,
	@RebateDate	datetime,
	@delpcent	money,
	@actualdelpcent money,
	@rebpcent	money,
	@CountryCode	char(2),
	@Global90Days	varchar(2),
        @Nearest 	smallint,
	@NoCents	SMALLINT,
	@description	varchar (256),
	@loopnumber	smallint,
	@porebate	varchar(12),
	@PeriodEnd	datetime,
	@RunPeriodEnd	datetime,
	@sqlstr		varchar(750),
	@previousperiodend datetime,
	@previousyearend datetime,
	@sequence	smallint,
--	@currentsequence smallint,
	@yearendsequence smallint,
	@columnname	varchar(3),
    @rule           smallint --CR888 added
--,@return int --for testing only
--,@PeriodEndchar varchar(12) --for testing only

-----------------------
--INITIALISE PARAMETERS
-----------------------
	select	@RuleChangeDate	= '01-Apr-2002',
		@FromThresDate	= '01-jan-1900',
		@FromDate	= '01-jan-1900',
		@loopnumber	= 1,
		@periodend 	= CONVERT(datetime, @periodendchar, 102),
		@RunPeriodEnd 	= @periodend,
		@UntilThresDate	= @periodend,
		@sequence 	= 10
	SELECT @return = @@error
	--find termstype for 90 day accounts
/*KEF not used anymore	if (@return = 0)
	begin
		select	@Global90Days = (select termstype from termstypetable
				         where description = '90 DAYS')
		SELECT @return = @@error
	end*/
	if (@return = 0)
	begin
		SELECT	@RebPCent = rebpcent,
			@CountryCode = countrycode,
			@NoCents = nocents,
			@delpcent = globdelpcent
		FROM	country

		SELECT @return = @@error
	end
	--find previous period end
	if (@return = 0)
	begin
		select @previousperiodend = isnull((select max(enddate) from rebateforecast_periodenddates
					     where enddate < @runperiodend and enddate <> '01-jan-1900'),'31-mar-2005')
    		SELECT @return = @@error
	end
	if (@return = 0)
	begin
		select @FromThresDateReportB = dateadd(day,1,@previousperiodend) --report b starts from day after last periodend
    		SELECT @return = @@error
	end
	if (@return <> 0)
	begin
		insert into RebateForecast_errorlog
		select	getdate(), 'Error occurred in INITIALISE PARAMETERS'
		return @return
	end

--------------------------------------
--CHECK IF RUN ALREADY THIS PERIOD END
--------------------------------------
	if (select isnull(rundate,'01-jan-1900') from RebateForecast_periodenddates where @periodend = enddate) > '01-jan-1900'
	begin
		insert into RebateForecast_errorlog
		select getdate(), 'Already run for period end ' + convert(varchar,@periodend)
		return @return
	end

---------------------------
--CHECK DATE NOT IN FUTURE --inserted from 5.1 - no issue number used - Alex added on 27/06/07
---------------------------
	if @periodend > getdate()
	begin
		insert into RebateForecast_errorlog
		select getdate(), 'Can not run for future date ' + convert(varchar,@periodend)
		return @return
	end

------------------------
--SAVE/REMOVE OLD DATA 1
------------------------
	--Remove old data - anything over 13 months old --should be over 12 period ends old but this is roughly the same
	if (@return = 0)
	begin
	    	delete from RebateForecastA
		where	periodend < dateadd(month,-13,getdate())
		SELECT @return = @@error
	end
	if (@return = 0)
	begin
		delete from RebateForecastB
		where	periodend < dateadd(month,-13,getdate())
		SELECT @return = @@error
	end
	if (@return = 0)
	begin
		delete from RebateForecastC
		where	period_end < dateadd(month,-13,getdate())
		SELECT @return = @@error
	end
	--copy current into temp previous data
	if (@return = 0)
	begin
		select * into #RebateForecast_byaccount_previous from RebateForecast_byaccount_current
	        SELECT @return = @@error
	end
	--Remove previous year end data - only if new year end
    	if (@return = 0)
	begin
	    	if (month(@RunPeriodEnd) = 3 and day(@RunPeriodEnd) = 31) --year end period end is always 31-mar (Clare confirmed)
	    	begin
		    drop table RebateForecast_byaccount_yearend
	 	    SELECT @return = @@error
	    	    if (@return = 0)
	    	    begin
			truncate table RebateForecastD
	 		SELECT @return = @@error
	            end
	    	end
	end
    	if (@return <> 0)
	begin
		insert into RebateForecast_errorlog
		select	getdate(), 'Error occurred in SAVE/REMOVE OLD DATA 1'
		return @return
	end


--------------------------------
--DELIVERY THRESHOLD CALCULATION
--------------------------------
	--Get all the accounts that met threshold before rule change date (01-Apr-2002)
	if (@return = 0)
	begin
		select	b.acctno, sum(transvalue) as deltotal, 'Y' as beforerulechange,
			max(b.datetrans) as DelThresDate, 'B' as report --will change to A further down if in previous report
		into	#del_thres_reached2
		from	fintrans b, country c, acct a
		where	b.acctno = a.acctno
		and     b.transtypecode in ('DEL','GRT','REP','ADD','RDL','RPO','CLD')			-- #10138
		and	b.datetrans < @RuleChangeDate
		and	substring(b.acctno,4,1) in ('0','1','2','3','6','7','8','9')
		and	currstatus <> 'S'
		group by b.acctno, a.agrmttotal
		having ((sum(transvalue)) >= (a.agrmttotal * (@delpcent/100)))

		SELECT @return = @@error
	end
	--Get all the accounts that met threshold after RuleChangeDate and in previous report
	if (@return = 0)
	begin
		select	b.acctno, sum(transvalue) as deltotal, 'N' as beforerulechange,
			max(b.datetrans) as DelThresDate, 'B' as report
		into	#del_thres_reached
		from	fintrans b , acct a
		where	b.acctno = a.acctno
		and     b.transtypecode in ('DEL','GRT','REP','ADD','RDL','RPO','CLD')			-- #10138
--		and	b.datetrans <= @Previousperiodend	--included in last report
--		and	b.datetrans <= @runperiodend		--do from current period end or won't get returns
		and	b.datetrans < dateadd(day,1,@runperiodend)		--do from current period end or won't get returns in case of time part excluding transactions done on runperiodendend
		and	not exists (select * from #del_thres_reached2 f where a.acctno = f.acctno)-- and beforerulechange = 'Y')
		and	substring(b.acctno,4,1) in ('0','1','2','3','6','7','8','9')
		and	currstatus <> 'S'
		group by b.acctno, a.agrmttotal
		having ((sum(transvalue)) >= (a.agrmttotal * (@delpcent/100)))

		SELECT @return = @@error
	end
	--Merge tables so all in 1
	if (@return = 0)
	begin
		insert into #del_thres_reached
		select	* from #del_thres_reached2

		SELECT @return = @@error
	end
	--Update to indicate if Report A -  if included in previous report so must be report a this time
	if (@return = 0)
	begin
/* KEF 07/03/08 CR927 want to report on new business even at year end so removing this code
		--if year end then update everything to A as there's no new business
		if (month(@RunPeriodEnd) = 3 and day(@RunPeriodEnd) = 31)
		begin
			update	#del_thres_reached
			set	report = 'A'

			SELECT @return = @@error
		end
		else
		begin*/
			update	#del_thres_reached
			set	report = 'A'
			from 	#RebateForecast_byaccount_previous
			where	#RebateForecast_byaccount_previous.acctno = #del_thres_reached.acctno

			SELECT @return = @@error
--		end --KEF CR927 removed
	end
/*	--Get all the accounts that met threshold after RuleChangeDate and not in Report A
	if (@return = 0)
	begin
		select	b.acctno, sum(transvalue) as deltotal, 'N' as beforerulechange,
			max(b.datetrans) as DelThresDate, 'B' as report --new business
		into	#del_thres_reached3
		from	fintrans b , acct a
		where	b.acctno = a.acctno
		and     b.transtypecode in ('DEL','GRT','REP','ADD','RDL','RPO')
		and	b.datetrans <= @runperiodend--@UntilThresDate
		and	not exists (select * from #del_thres_reached f where a.acctno = f.acctno)-- and beforerulechange = 'Y')
		and	substring(b.acctno,4,1) in ('0','1','2','3','6','7','8','9')
		and	currstatus <> 'S'
		group by b.acctno, a.agrmttotal
		having ((sum(transvalue)) >= (a.agrmttotal * (@delpcent/100)))

		SELECT @return = @@error
	end
	if (@return = 0)
	begin
		insert into #del_thres_reached
		select	* from #del_thres_reached3

		SELECT @return = @@error
	end
*/
    	if (@return <> 0)
	begin
		insert into RebateForecast_errorlog
		select	getdate(), 'Error occurred in DELIVERY THRESHOLD CALCULATION'
		return @return
	end

----------------
--INITIAL SELECT
----------------
	IF (@return = 0)
	BEGIN
		SELECT  acctno = Act.acctno,
			TermsType = ISNULL(Act.termstype,''),
	        DateAcctOpen = Act.dateacctopen,
			Deposit = ISNULL(A.deposit, 0.00),
            AgrmtTotal = ISNULL(A.agrmttotal, 0.00),
			ServiceChg = convert(money,a.ServiceChg),
        	DateDel = A.datedel,
			DateFirst = I.datefirst,
	        DateLast = I.datelast,
			InstalAmount = ISNULL(I.instalamount, 0.00),
            FinInstalAmt = ISNULL(I.Fininstalamt, 0.00),
			InstalNo = ISNULL(I.instalno, 0.00),
        	InstalFreq = I.instalfreq,
			porebate_p1 = convert (money,0),
			porebate_p2 = convert (money,0),
			porebate_p3 = convert (money,0),
			porebate_p4 = convert (money,0),
			porebate_p5 = convert (money,0),
			porebate_p6 = convert (money,0),
			porebate_p7 = convert (money,0),
			porebate_p8 = convert (money,0),
			porebate_p9 = convert (money,0),
			porebate_p10 = convert (money,0),
			porebate_p11 = convert (money,0),
			porebate_p12 = convert (money,0),
           	agrmtmths = convert (smallint,0),
			deltot = convert (money,0),
			AgrmtDays = convert (smallint,0),
			totalmonths = convert (smallint, 0),
			convert (smallint,0) as monthstogo,
            convert (varchar (2),'') as dtnetfirstin,
          	convert (float, 0) as agrmtyears,
			convert (smallint, -99) as insincluded,
     	    convert (float,0) as servpcent, --69377 KEF Changed to zero from -99 as it will mess up Report B averages if not set
			convert (float,-99) as inspcent,
			convert (money,0) as insamount,
			convert (float,0) as monthstogofactor,
			convert (float, 0) as totalmonthsfactor,
			CONVERT(datetime, D.DelThresDate, 111) as DelThresDate, --don't want time part
			Report = D.Report,
            convert (varchar(4),'78-1') AS Rule78,
			convert (char (1),'N') as calcdone,
			arrears = Act.arrears,
			convert (smallint,0) as rebatemonthsarrears,
			currstatus = Act.currstatus,
			convert (smallint,0) as Insplit,
			convert (char(1),'N') as splitdataupdatedone,
			convert (smallint,0) as FullRebateDays
	        into	#RebateForecast_byaccount_current
        	FROM	acct Act
        	INNER   JOIN agreement A ON Act.acctno = A.acctno AND Act.CurrStatus != 'S' and Act.outstbal > 1
			INNER   JOIN #del_thres_reached d ON Act.acctno = d.acctno
        	LEFT    JOIN instalplan I ON A.acctno = I.acctno
  
	        SELECT @return = @@error
	END
	IF (@return = 0)
	BEGIN
		create clustered index ix_RebateForecast_byaccount_current on #RebateForecast_byaccount_current (acctno)
	        SELECT @return = @@error
	end
    	if (@return <> 0)
	begin
		insert into RebateForecast_errorlog
		select	getdate(), 'Error occurred in INITIAL SELECT'
		return @return
	end

----------------------
--UPDATE STATIC VALUES
----------------------
	--update delivery total from fintrans
	if (@return = 0)
	begin
        	update	#RebateForecast_byaccount_current
	        set	DelTot = ISNULL((select sum(f.transvalue)    -- Changed to get total delivery instead of first del type transaction
        	FROM	fintrans f
	        WHERE	f.acctno = #RebateForecast_byaccount_current.AcctNo
        	AND	f.transtypecode IN ('DEL', 'REP', 'RPO', 'GRT', 'RDL', 'ADD','CLD')),0)				-- #10138

	        SELECT @return = @@error
	end
	if (@return = 0)
	begin
	        delete from #RebateForecast_byaccount_current where deltot <= 0
	        SELECT @return = @@error
	end
    	if (@return <> 0)
	begin
		insert into RebateForecast_errorlog
		select	getdate(), 'Error occurred in UPDATE STATIC VALUES - delivery total from fintrans'
		return @return
	end

	--update rule for calculation
	if (@return = 0)
	begin
		if (select CountryCode from country) = 'M'
		begin
		        UPDATE #RebateForecast_byaccount_current
        		SET    Rule78 = '78-2'
		        WHERE  #RebateForecast_byaccount_current.termstype in ('04','05','06')
			AND    #RebateForecast_byaccount_current.delthresdate > '01-apr-2002'

		        SELECT @return = @@error
		end
		else
		begin
            --CR888 Caribbean need to use rule 78-0 for all rebate frame totals regardless of delivery threshold date
			if (select CountryCode from country) in ('N','B','Z','D','G','A','J','K','L','V','T')
            begin
		        UPDATE #RebateForecast_byaccount_current
	        	SET    Rule78 = '78+1' --CR900

		        SELECT @return = @@error
            end
            else
            begin
		        UPDATE #RebateForecast_byaccount_current
	        	SET    Rule78 = '78-2'
	        	WHERE  #RebateForecast_byaccount_current.DelThresDate NOT BETWEEN '01-Jan-1900' AND '01-Apr-2002'
	
		        SELECT @return = @@error
            end
		end
	end
    if (@return <> 0)
	begin
		insert into RebateForecast_errorlog
		select	getdate(), 'Error occurred in UPDATE STATIC VALUES - rule for calculation'
		return @return
	end

	--update agreement months and agreement days, needed for datelast update (if not set already)
	if (@return = 0)
	begin
	        update	#RebateForecast_byaccount_current
		set	AgrmtMths = InstalNo - 1
		where	InstalFreq = 'M'
	        SELECT @return = @@error
	end
	if (@return = 0)
	begin
		update	#RebateForecast_byaccount_current
		set	AgrmtDays = (InstalNo - 1) * 14
		where	InstalFreq = 'F'
	        SELECT @return = @@error
	end
	if (@return = 0)
	begin
	        update	#RebateForecast_byaccount_current
		set	AgrmtDays = (InstalNo - 1) * 7  where InstalFreq = 'W'
	        SELECT @return = @@error
	end
	if (@return = 0)
	begin
 	        update	#RebateForecast_byaccount_current
		set	AgrmtMths = (InstalNo - 1) * 6  where InstalFreq = 'B'
	        SELECT @return = @@error
	end
    	if (@return <> 0)
	begin
		insert into RebateForecast_errorlog
		select	getdate(), 'Error occurred in UPDATE STATIC VALUES - agreement months and agreement days'
		return @return
	end

	--update datelast if not set already
	IF (@return = 0)
	BEGIN
		update	#RebateForecast_byaccount_current
        	set	DateLast = DATEADD(mm, AgrmtMths, DateFirst)
	        where	AgrmtMths != 0
		and	isnull(datelast,'01-jan-1900') <= '01-jan-1910'
		and	isnull(datefirst,'01-jan-1900') > '01-jan-1910' --only set if datefirst is set
	        SELECT @return = @@error
	end
	IF (@return = 0)
	BEGIN	
	        update #RebateForecast_byaccount_current
        	set DateLast = DATEADD(dd, AgrmtDays, DateFirst)
	        where AgrmtMths = 0 and AgrmtDays != 0
	        SELECT @return = @@error
	END
    	if (@return <> 0)
	begin
		insert into RebateForecast_errorlog
		select	getdate(), 'Error occurred in UPDATE STATIC VALUES - datelast if not set already'
		return @return
	end

	--update dtnetfirstin and FullRebateDays from termstypetable
	IF (@return = 0)
	BEGIN
		update #RebateForecast_byaccount_current
		set    DtNetFirstIn = termstypetable.dtnetfirstin,
		       FullRebateDays = termstypetable.FullRebateDays
		FROM   termstypetable
		WHERE  #RebateForecast_byaccount_current.termstype = termstypetable.TermsType

	        SELECT @return = @@error
	END
    	if (@return <> 0)
	begin
		insert into RebateForecast_errorlog
		select	getdate(), 'Error occurred in UPDATE STATIC VALUES - dtnetfirstin from termstypetable'
		return @return
	end

	--update inspcent, insincluded, servpcent from intratehistory
	IF (@return = 0)
	BEGIN
	    update #RebateForecast_byaccount_current
      	set    InsPcent	   = I.inspcent,
	           Insincluded = I.insincluded
        from   intratehistory i
        where  i.termstype = #RebateForecast_byaccount_current.termstype
        and    dateacctopen >= i.datefrom
        and    band in ('','A') --all bands will have the same inspcent. If bands not implemented then will band will be blank. Date from and to of blank bands will not overlap with A,B,C,D bands.
      	and    (DateAcctOpen <= I.dateto OR I.dateto = '01-jan-1900')

--KEF 69377 changed code so inspcent will be set regardless of band (matching with the Rebate Calculation frmae)
/*     	       servpcent   = I.intrate --69377 KEF will do this after as getting problem with blank score bands not picking up inspcent and it's affecting rebate value. only need this for the report B averages
        from   intratehistory i, proposal p --CR806 KEF replaced from and where clause to handle bands
        where  (isnull(p.scoringband,'') = i.band or i.band = '') --find band, or match with blank for older accts
        and    #RebateForecast_byaccount_current.acctno = p.acctno
        and    #RebateForecast_byaccount_current.termstype = i.termstype
        and    DateAcctOpen >= I.datefrom
        and    (DateAcctOpen <= I.dateto or I.dateto = '01-jan-1900')*/
--        from   intratehistory i, proposal p, acct a /* CR806 KEF replaced from and where clause to handle bands */
/*        where  (p.scoringband = i.band or i.band = '') --find band, or match with blank for older accts
        and    p.acctno = a.acctno
        and    a.termstype = i.termstype
        and    DateAcctOpen >= I.datefrom
        and    (DateAcctOpen <= I.dateto or I.dateto = '01-jan-1900')*/
/*	    FROM   IntRateHistory I
     	WHERE  #RebateForecast_byaccount_current.termstype = I.TermsType
	    AND    DateAcctOpen >= I.datefrom
      	AND    (DateAcctOpen <= I.dateto OR I.dateto = '01-jan-1900')
*/
	    SELECT @return = @@error
	END

    --KEF 69377 Servpcent update needs to be done seperately so can check for band
    IF (@return = 0)
	BEGIN
	    update #RebateForecast_byaccount_current
      	set    servpcent   = I.intrate
        from   intratehistory i, proposal p 
        where  (isnull(p.scoringband,'') = i.band or i.band = '') --find band, or match with blank for older accts
        and    #RebateForecast_byaccount_current.acctno = p.acctno
        and    #RebateForecast_byaccount_current.termstype = i.termstype
        and    DateAcctOpen >= I.datefrom
        and    (DateAcctOpen <= I.dateto or I.dateto = '01-jan-1900')

	    SELECT @return = @@error
	END

	--if intratehistory record not found then need to use earliest record in intratehistory, regardless of band
	IF (@return = 0)
	BEGIN
		select 	termstype, min(datefrom) as earliest
		into	#intratehistory
		from	intratehistory
        where   band in ('','A') --CR806 all bands will have same inspcent. If bands not implemented then will band will be blank. Date from and to of blank bands will not overlap with A,B,C,D bands.
		group by termstype

		select 	i2.termstype, inspcent, intrate, insincluded
		into	#intratehistory2
		from	#intratehistory i2, intratehistory i
		where	i2.termstype = i.termstype
		and	    datefrom = earliest
        AND     band in ('','A') --CR806 all bands will have same inspcent. If bands not implemented then will band will be blank. Date from and to of blank bands will not overlap with A,B,C,D bands.

        update 	#RebateForecast_byaccount_current
       	set    	InsPcent	   = I.inspcent,
               	Insincluded = I.insincluded,
       	       	servpcent   = I.intrate
        FROM   	#intratehistory2 I
       	WHERE  	#RebateForecast_byaccount_current.termstype = I.TermsType
		and	    (#RebateForecast_byaccount_current.InsPcent	= -99
		or	    #RebateForecast_byaccount_current.Insincluded	= -99
		or	    #RebateForecast_byaccount_current.servpcent	= -99)

        SELECT @return = @@error
	END
	--67601 KEF if no record for termstype, whatever date then remove account from calculation as can't calculate - found in Fiji
	IF (@return = 0)
	BEGIN
		delete from #RebateForecast_byaccount_current
		where (#RebateForecast_byaccount_current.InsPcent	= -99
		or	#RebateForecast_byaccount_current.Insincluded	= -99)
		--or	#RebateForecast_byaccount_current.servpcent	= -99)--KEF 69377 removed this as not setting to -99 now - not need for actual rebate calc

	        SELECT @return = @@error
	end
    	if (@return <> 0)
	begin
		insert into RebateForecast_errorlog
		select	getdate(), 'Error occurred in UPDATE STATIC VALUES - inspcent, insincluded, servpcent'
		return @return
	end

	--update total months
	IF (@return = 0)
	BEGIN
		--accts with no variable innterest rate
		update #RebateForecast_byaccount_current
		set    TotalMonths = ISNULL((AgrmtTotal-Deposit-FinInstalAmt)/InstalAmount,0)+1.9
		where  InstalAmount != 0
		and    Insplit = 0

		SELECT @return = @@error
	end

	--update totalmonths again if instalfreq not monthly
	IF (@return = 0)
	BEGIN
		update #RebateForecast_byaccount_current
		set    TotalMonths = ((TotalMonths-1)*6)
		where  InstalFreq = 'B'
		and    Insplit = 0

		SELECT @return = @@error
	end
 	IF (@return = 0)
	BEGIN
		update #RebateForecast_byaccount_current
		set    TotalMonths = TotalMonths + ISNULL(DATEDIFF(mm, DateFirst, DateAcctOpen),0)
		where  DateAcctOpen < DateFirst
		and    InstalFreq = 'B'
		and    Insplit = 0

		SELECT @return = @@error
	end
	IF (@return = 0)
	BEGIN
		update #RebateForecast_byaccount_current
		set    TotalMonths = TotalMonths/2
		where  InstalFreq = 'F'
		and    Insplit = 0

		SELECT @return = @@error
	end
	IF (@return = 0)
	BEGIN
		update #RebateForecast_byaccount_current
		set    TotalMonths = TotalMonths/4
		where  InstalFreq = 'W'
		and    Insplit = 0

		SELECT @return = @@error
	end
	--update totalmonths again if dtnetfirstin set
	IF (@return = 0)
	BEGIN
		update #RebateForecast_byaccount_current
		set    TotalMonths = TotalMonths - 1
		where  DtNetFirstIn = 'Y'
		SELECT @return = @@error
	end
	--remove any accts where totalmonths <= 0
	IF (@return = 0)
	BEGIN
		delete from #RebateForecast_byaccount_current
		where 	TotalMonths	<= 0
	        SELECT @return = @@error
	END
   	if (@return <> 0)
	begin
		insert into RebateForecast_errorlog
		select	getdate(), 'Error occurred in UPDATE STATIC VALUES - total months'
		return @return
	end

	--update agrmtyears
	IF (@return = 0)
	BEGIN
		update 	#RebateForecast_byaccount_current
		set	AgrmtYears = CONVERT(FLOAT,TotalMonths)/12
		SELECT @return = @@error
	end
   	if (@return <> 0)
	begin
		insert into RebateForecast_errorlog
		select	getdate(), 'Error occurred in UPDATE STATIC VALUES - agrmtyears'
		return @return
	end

	--update insurance amount
	IF (@return = 0)
	BEGIN
	-- 69764-6 St Lucia change ins calc SL
if (@countrycode = 'S' or @countrycode = 'H'  or @countrycode = 'I'  )
		begin /* KEF 68430 Negative rebates fix, need to remove deposit as Insurance is calculated without it. Agreed with Kim and Raymond */
			update 	#RebateForecast_byaccount_current
--			set    	InsAmount = round(((convert(float,AgrmtTotal) - convert(float,ServiceChg)) * convert(float,AgrmtYears) * (convert(float,InsPCent) / 100)) ,2)
			set    	InsAmount = round(((convert(float,AgrmtTotal) - convert(float,Deposit) - convert(float,ServiceChg)) * convert(float,AgrmtYears) * (convert(float,InsPCent) / 100)) ,2)
			where	insincluded = 1
			SELECT @return = @@error
		/*CR909 removed this as need to do as 1 statement below
			IF (@return = 0)
			BEGIN
			    	update #RebateForecast_byaccount_current
			    	set    ServiceChg = (ServiceChg - InsAmount)
			    	SELECT @return = @@error
			end   */
end
	else /* KEF 68430 Negative rebates fix, other countries don't want this change so leaving calculation as is */
	begin
		update 	#RebateForecast_byaccount_current
		set    	InsAmount = round((convert(float,AgrmtTotal) * convert(float,AgrmtYears) * (convert(float,InsPCent) / 100)) ,2)
		where	insincluded = 1
		SELECT @return = @@error
	end
    /* KEF 07/01/08 CR909 Malaysia rebate changes start */
	IF (@return = 0)
	begin
        if (@countrycode = 'Y')
        begin
            --Accounts delivered before 01/05/2005 have insurance of 1.25%
			update #RebateForecast_byaccount_current
		    set    ServiceChg = (ServiceChg - ((AgrmtTotal - ServiceChg)*1.25/100)) --IP - 18/02/10 - CR1072 - LW 70882 - General Fixes from 4.3 - Merge - changed to *1.25/100
            where  DelThresDate < '01-may-2005'

            --Accounts delivered between 01/05/2005 and 27/08/2006 have insurance of 0.90%
			update #RebateForecast_byaccount_current
		    set    ServiceChg = (ServiceChg - ((AgrmtTotal - ServiceChg)*0.009)) --IP - 18/02/10 - CR1072 - LW 70882 - General Fixes from 4.3 - Merge - changed to 0.009
            where  DelThresDate between '01-may-2005' and '27-aug-2006'

            --Accounts delivered after 27/08/2006 can have a calculated insurance premium using inspcent
   			update #RebateForecast_byaccount_current
		    set    ServiceChg = (ServiceChg - InsAmount)
            where  DelThresDate > '27-aug-2006'
        end
        else
		BEGIN
			update #RebateForecast_byaccount_current
		    	set    ServiceChg = (ServiceChg - InsAmount)
		    	SELECT @return = @@error
		end
	end
	END
   	if (@return <> 0)
	begin
		insert into RebateForecast_errorlog
		select	getdate(), 'Error occurred in UPDATE STATIC VALUES - insurance amount'
		return @return
	end

--update rebatemonthsarrears
	IF (@return = 0)
	BEGIN
		update	#RebateForecast_byaccount_current
		set	Rebatemonthsarrears = isnull(cast(0.5+(Arrears/InstalAmount) as integer),0)
		WHERE 	cast(0.5+(Arrears/InstalAmount) as float) < 1000
		AND 	InstalAmount > 1

		SELECT @return = @@error
	end
   	if (@return <> 0)
	begin
		insert into RebateForecast_errorlog
		select	getdate(), 'Error occurred in UPDATE STATIC VALUES - rebatemonthsarrears'
		return @return
	end

---------------------------------------
--UPDATE CHANGABLE DATA FOR EACH PERIOD
---------------------------------------
	while @loopnumber < 13
	begin
		--initialise calc parameters (need to do this here for loop)
		if (@return = 0)
		begin
		        update	#RebateForecast_byaccount_current
			set	calcdone = 'N',
				MonthsToGo = 0,
				MonthsToGoFactor = 0,
				TotalMonthsFactor = 0

		        SELECT @return = @@error
		end
		if (@return <> 0)
		begin
			insert into RebateForecast_errorlog
			select	getdate(), 'Error occurred in UPDATE CHANGABLE DATA FOR EACH PERIOD - initialise calc parameters'
			return @return
		end

		--set columnname to update
		if (@return = 0)
		begin
			select @porebate = 'porebate_p' + convert(varchar,@loopnumber)
		        SELECT @return = @@error
		end
		if (@return <> 0)
		begin
			insert into RebateForecast_errorlog
			select	getdate(), 'Error occurred in UPDATE CHANGABLE DATA FOR EACH PERIOD - columnname'
			return @return
		end

		--set date to run rebate at
		if (select @loopnumber) <> 1
		begin
			--find next period end if not first time in loop
			IF (@return = 0)
			BEGIN
				select @PeriodEnd = (select min(enddate) from rebateforecast_periodenddates where enddate > @periodend and enddate <> '01-jan-1900')
			        SELECT @return = @@error
			end
		end

		if (@return <> 0)
		begin
			insert into RebateForecast_errorlog
			select	getdate(), 'Error occurred in UPDATE CHANGABLE DATA FOR EACH PERIOD - set date to run rebate at'
			return @return
		end

		--rebate run date 7 days after period end, truncated so no time part
		IF (@return = 0)
		BEGIN

	-- remove 7 days CR956
	if @countrycode in ('S','P','M','C','Y','I','F')
	begin
	select @RebateDate = dateadd(day,7,@Periodend)
	end
	else 
	begin
			select @RebateDate = @Periodend
	end
	
	--set date for rebate after 12 months - date in 1 year
    DECLARE  @RebateDate_after12mths DATETIME 
	select @RebateDate_after12mths = dateadd(year,1,@Periodend)

	        	SELECT @return = @@error
		end
		if (@return <> 0)
		begin
			insert into RebateForecast_errorlog
			select	getdate(), 'Error occurred in UPDATE CHANGABLE DATA FOR EACH PERIOD - add 7 days to rebate calcualtion date'
			return @return
		end

-----------------------------------------------------
--CR440 variable instalment and interest rate changes
-----------------------------------------------------
		--Update split if acct in instalmentvariable table
		if (@return = 0)
		begin
			--update all accts in instalmentvariable table to split 2
			update	#RebateForecast_byaccount_current
			set	Insplit = 2
			from	instalmentvariable i
			where	i.acctno = #RebateForecast_byaccount_current.acctno
	
		        SELECT @return = @@error
		end

		if (@return = 0)
		begin
			--now change to split 1 if rebatedate <= dateto for first split
        		update	#RebateForecast_byaccount_current
	        	set	Insplit = 1
			from	instalmentvariable i
			where	i.acctno = #RebateForecast_byaccount_current.acctno
			and	@rebatedate <= dateto
			and	instalorder = 1

		        SELECT @return = @@error
		end

		--if first time in loop then need to set data correctly for split 1 
		if (@return = 0)
		begin
			if (select @loopnumber) = 1
			begin
				update	#RebateForecast_byaccount_current
				set	Datefirst = Datefrom,
					Datelast = Dateto,
					Instalno = instalmentnumber,
					Instalamount = instalment,
					Servicechg = servicecharge,
					totalmonths = instalmentnumber
				from	instalmentvariable i
				where	i.acctno = #RebateForecast_byaccount_current.acctno
				and	Insplit = instalorder
	
			        SELECT @return = @@error

				--update splitdataupdatedone to Y if in split 2 as won't need to redo data
				if (@return = 0)
				begin
					update	#RebateForecast_byaccount_current
					set	splitdataupdatedone = 'Y'
					where	Insplit = 2
	
				        SELECT @return = @@error
				end
			end
			else --redo data if now in split 2 and was in split 1 for last run
			begin
				update	#RebateForecast_byaccount_current
				set	Datefirst = Datefrom,
					Datelast = Dateto,
					Instalno = instalmentnumber,
					Instalamount = instalment,
					Servicechg = servicecharge,
					splitdataupdatedone = 'Y',
					totalmonths = instalmentnumber
				from	instalmentvariable i
				where	i.acctno = #RebateForecast_byaccount_current.acctno
				and	Insplit = 2
				and	splitdataupdatedone = 'N'
				and	instalorder = 2

			        SELECT @return = @@error
			end
		end

		--update rebate for interest free accounts
		--rebate = service charge if account within 90days of delivery date (add insamount back into servicechg)
		IF (@return = 0)
    		BEGIN
			select @sqlstr = (' set	 '+ @porebate +' = servicechg+InsAmount,  '+
		 			  ' calcdone = ''Y'' '+
	 				  ' WHERE (DATEDIFF(dd,ISNULL(datedel,''01-jan-1900''),'''+convert(varchar,@PeriodEnd)+''') <= FullRebateDays)'+ --remove extra 7 days
					  ' AND  FullRebateDays > 0 ' +
	 				  ' AND	 isnull(DateDel,''01-jan-1900'') >= ''01-jan-1910'' ')
        		SELECT @return = @@error
		end
		IF (@return = 0)
		BEGIN
			execute	('update #RebateForecast_byaccount_current '+ @sqlstr)
	        	SELECT @return = @@error
		end

		if (@return <> 0)
		begin
			insert into RebateForecast_errorlog
			select	getdate(), 'Error occurred in UPDATE CHANGABLE DATA FOR EACH PERIOD - rebate for interest free accounts'
			return @return
		end

		--update monthstogo
		IF (@return = 0)
		BEGIN	
			UPDATE #RebateForecast_byaccount_current
			SET    MonthsToGo = (select case
					     when   day(@RebateDate)<= day(Datelast)
					     then   datediff(month,@RebateDate,Datelast)
					     else   datediff(month,@RebateDate,Datelast) - 1
					     end)
			SELECT @return = @@error
		END
		--set rebate to zero if monthtogo <= 0
		IF (@return = 0)
		BEGIN	
		        execute (' update #RebateForecast_byaccount_current '+
	        		 ' set '+ @porebate +' = 0, '+
				 '	  calcdone = ''Y'' '+
		        	 ' where  MonthsToGo <= 0 '+
				 ' and    calcdone = ''N'' '+
				 ' and	  Insplit <> 1 ')
			SELECT @return = @@error
		end

		--update MonthsToGo again if bigger than totalmonths
		IF (@return = 0)
		BEGIN
           --KEF 69596 Mauritius, Madagascar, Singapore, Thailand and Indonesia will only reduce by 1 if monthstogo exceeds totalmonths. If it equals then will leave.
    		IF (@CountryCode = 'S' OR @CountryCode= 'H' OR @CountryCode= 'I' OR @CountryCode= 'M' OR @CountryCode= 'C')
    		begin
    			update #RebateForecast_byaccount_current
    			set    MonthsToGo = TotalMonths - 1
    			where  MonthsToGo > TotalMonths --remove equals sign
            end
            else
    		begin
    			update #RebateForecast_byaccount_current
    			set    MonthsToGo = TotalMonths - 1
    			where  MonthsToGo >= TotalMonths
            end

			SELECT @return = @@error
		end
		if (@return <> 0)
		begin
			insert into RebateForecast_errorlog
			select	getdate(), 'Error occurred in UPDATE CHANGABLE DATA FOR EACH PERIOD - MonthsToGo'
			return @return
		end

/* CR888 changed monthstogofactor so it will work for any rule
		--update MonthsToGoFactor for rule 78-1
		IF (@return = 0)
		BEGIN
			update 	#RebateForecast_byaccount_current
		        set 	MonthsToGoFactor = MonthsToGo * (MonthsToGo - 1)
		        where 	Rule78 = '78-1'
			SELECT @return = @@error
		end
		--update MonthsToGoFactor for rule 78-2
		IF (@return = 0)
		BEGIN
			update 	#RebateForecast_byaccount_current
			set 	MonthsToGoFactor = (MonthsToGo - 1) * (MonthsToGo - 2)
			where 	Rule78 = '78-2'
			SELECT @return = @@error
		end*/

    	--update MonthsToGoFactor for rule 78-2 for rebate total
    	IF (@return = 0)
    	BEGIN
    		update 	#RebateForecast_byaccount_current
--    		set 	MonthsToGoFactor = (MonthsToGo - (right(rule78,1)-1)) * (MonthsToGo - right(rule78,1))
		    set 	MonthsToGoFactor = (MonthsToGo - ((left(right(rule78,2),2)*-1)-1)) * (MonthsToGo - (left(right(rule78,2),2)*-1))--, --CR900
    		where   calcdone = 'N'

    		SELECT @return = @@error
    	end
--CR888 end of monthstogofactor changes

		if (@return <> 0)
		begin
			insert into RebateForecast_errorlog
			select	getdate(), 'Error occurred in UPDATE CHANGABLE DATA FOR EACH PERIOD - MonthsToGoFactor'
			return @return
		end

		--update TotalMonthsFactor
		IF (@return = 0)
		BEGIN
			update	#RebateForecast_byaccount_current
			set 	TotalMonthsFactor = TotalMonths * (TotalMonths + 1)
			SELECT @return = @@error
		end
		if (@return <> 0)
		begin
			insert into RebateForecast_errorlog
			select	getdate(), 'Error occurred in UPDATE CHANGABLE DATA FOR EACH PERIOD - TotalMonthsFactor'
			return @return
		end
		--update @porebate and calcdone
		IF (@return = 0)
		BEGIN
			select @sqlstr = ''	
			select @sqlstr = (' set '+ convert(varchar,@porebate) +' = (ServiceChg * MonthsToGoFactor) '+
					  ' where  totalmonthsfactor > 0 '+
				 	  ' and    calcdone = ''N'' ') 
			SELECT @return = @@error
		end
		IF (@return = 0)
		BEGIN
			execute ('update #RebateForecast_byaccount_current '+ @sqlstr)
			SELECT @return = @@error
		end
		IF (@return = 0)
		BEGIN
			select @sqlstr = ''	
			select @sqlstr = (' set '+ convert(varchar,@porebate) +' = round(convert(money,('+convert(varchar,@porebate)+')/ convert(money,TotalMonthsFactor)),2) '+
					  ' where  totalmonthsfactor > 0 '+
				 	  ' and    calcdone = ''N'' ') 
			SELECT @return = @@error
		end
		IF (@return = 0)
		BEGIN
			execute ('update #RebateForecast_byaccount_current '+ @sqlstr)
			SELECT @return = @@error
		end
		IF (@return = 0)
		BEGIN
			select @sqlstr = ''	
			select @sqlstr = (' set '+ convert(varchar,@porebate) +' = '+convert(varchar,@porebate)+ ' * '+convert(varchar,@RebPCent)+ '/ 100, '+
					  '	   calcdone = ''P'' '+	--set to P as may need to do again if variable rate
					  ' where  totalmonthsfactor > 0 '+
				 	  ' and    calcdone = ''N'' ') 
			SELECT @return = @@error
		end
		IF (@return = 0)
		BEGIN
			execute ('update #RebateForecast_byaccount_current '+ @sqlstr)
			SELECT @return = @@error
		end
		if (@return <> 0)
		begin
			insert into RebateForecast_errorlog
			select	getdate(), 'Error occurred in UPDATE CHANGABLE DATA FOR EACH PERIOD - porebate and calcdone'
			return @return
		end

--------------------
--recalculate rebate for variable interest and instalments
--------------------
		--if acct in split 1 then check if rebate should be servicechg (including insamount) for split 2
		IF (@return = 0)
		BEGIN
			--find accts still in first split and calc rebate < servicecharge for 2nd split
			select @sqlstr = ''	
			select @sqlstr = (' r.acctno, servicecharge as servicechgfor2ndsplit ' +
			' into	rebates_service '+ 
			' from 	instalmentvariable i, #RebateForecast_byaccount_current r '+
			' where Insplit = 1 '+
			' and	i.acctno = r.acctno '+
			' and '+ convert(varchar,@porebate) +' < servicecharge '+
			' and	Instalorder = 2 '+
			' and	calcdone = ''P'' ') --don't want accounts that have already been set as full service charge or set to zero as invalid for rebate

			SELECT @return = @@error
		end
		IF (@return = 0)
		BEGIN
			execute ('select '+ @sqlstr)
			SELECT @return = @@error
		end

		IF (@return = 0)
		BEGIN
			select @sqlstr = ''	
			select @sqlstr = (' set '+ convert(varchar,@porebate) +' = servicechgfor2ndsplit, '+
					  '	   calcdone = ''Y'' '+
					  ' from   rebates_service r '+
					  ' where  #RebateForecast_byaccount_current.acctno = r.acctno '+
				 	  ' and    calcdone = ''P'' ') 
			SELECT @return = @@error
		end
		IF (@return = 0)
		BEGIN
			execute ('update #RebateForecast_byaccount_current '+ @sqlstr)
			SELECT @return = @@error
		end
		IF (@return = 0)
		BEGIN
			drop table rebates_service

			SELECT @return = @@error
		end
		if (@return <> 0)
		begin
			insert into RebateForecast_errorlog
			select	getdate(), 'Error occurred in UPDATE CHANGABLE DATA FOR EACH PERIOD - porebate recalculated for variable rates'
			return @return
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
  	    	IF (@Nearest = 1)
			BEGIN
	        	    execute (' UPDATE #RebateForecast_byaccount_current '+
			 		' SET '+ @porebate +' = FLOOR('+@poRebate+') ') --Round down to nearest 1
	    			SELECT @return = @@error
	    	end
            ELSE IF (@Nearest = 100)
            BEGIN
		    		execute (' UPDATE #RebateForecast_byaccount_current	'+
		            		 ' SET '+ @porebate +' = ROUND('+@poRebate+',-2,1) ') --Round to nearest 100
	    			SELECT @return = @@error
	        END
			else --Always round to two decimal places max precision at least
			BEGIN
					execute (' UPDATE #RebateForecast_byaccount_current '+
					' SET '+ @porebate +' = ROUND('+@poRebate+',2) ')
			    	SELECT @return = @@error
			end
		END

        --CR900 - Check if rebate is larger than service charge - insurance, if larger make rebate = service charge - insurance
/*        IF (@return = 0)
        BEGIN
            execute (' UPDATE #RebateForecast_byaccount_current '+
					' SET '+ @porebate +' = ServiceChg '+
            	    ' WHERE  '+ @porebate +' > ServiceChg '+
                    ' and     ServiceChg > 0 ')

	    	SELECT @return = @@error
        end */
		--69493 - KEF 10/01/08 need to exclude 90 day accounts from this as they need insurance included in rebate
        --        commented above code and re-did to allow for 90 day accounts
        IF (@return = 0)
        BEGIN
			select @sqlstr = (' SET '+ @porebate +' = ServiceChg '+
            				  ' WHERE  '+ @porebate +' > ServiceChg '+
							  ' and     ServiceChg > 0 '+
						      ' and not ((DATEDIFF(dd,ISNULL(datedel,''01-jan-1900''),'''+convert(varchar,@PeriodEnd)+''') <= FullRebateDays)'+ --remove extra 7 days
							  '           AND  FullRebateDays > 0)') 

        	SELECT @return = @@error
		end
		IF (@return = 0)
		BEGIN
			execute (' UPDATE #RebateForecast_byaccount_current '+ @sqlstr)
        	SELECT @return = @@error
		end

		if (@return <> 0)
		begin
			insert into RebateForecast_errorlog
			select	getdate(), 'Error occurred in UPDATE CHANGABLE DATA FOR EACH PERIOD - rounding'
			return @return
		end

		--increase loop number
		if (@return = 0)
		begin		
			select @loopnumber = @loopnumber + 1
			SELECT @return = @@error
		end
		else
		begin
			break
		end
	end
	if (@return <> 0)
	begin
		insert into RebateForecast_errorlog
		select	getdate(), 'Error occurred in UPDATE CHANGABLE DATA FOR EACH PERIOD - end of loop'
		return @return
	end

----------
--REPORT A
----------
	IF (@return = 0)
	begin
		insert into RebateforecastA
		select 'In Adv/Up To 1 Mth', isnull(sum(porebate_p1),0), isnull(sum(porebate_p2),0), isnull(sum(porebate_p3),0),
			isnull(sum(porebate_p4),0), isnull(sum(porebate_p5),0), isnull(sum(porebate_p6),0), isnull(sum(porebate_p7),0),
			isnull(sum(porebate_p8),0), isnull(sum(porebate_p9),0), isnull(sum(porebate_p10),0),
			isnull(sum(porebate_p11),0), isnull(sum(porebate_p12),0), 10, @runperiodend, LEFT(acctno,3)
		from	#RebateForecast_byaccount_current
		where	report = 'A' --delthresdate between @FromThresDate and @previousperiodend
		and	rebatemonthsarrears <= 1
		and	currstatus not in ('6','7','8')
		GROUP BY LEFT(acctno,3)

		SELECT @return = @@error
	end
	IF (@return = 0)
	begin
		insert into RebateforecastA
		select '>1 Mth Up to 2 Mths', isnull(sum(porebate_p1),0), isnull(sum(porebate_p2),0), isnull(sum(porebate_p3),0),
			isnull(sum(porebate_p4),0), isnull(sum(porebate_p5),0), isnull(sum(porebate_p6),0), isnull(sum(porebate_p7),0),
			isnull(sum(porebate_p8),0), isnull(sum(porebate_p9),0), isnull(sum(porebate_p10),0), isnull(sum(porebate_p11),0),
			isnull(sum(porebate_p12),0), 20, @runperiodend, LEFT(acctno,3)
		from	#RebateForecast_byaccount_current
		where	report = 'A'
		and	rebatemonthsarrears <= 2
		AND	rebatemonthsarrears > 1 
		and	currstatus not in ('6','7','8')
		GROUP BY LEFT(acctno,3)

		SELECT @return = @@error
	end
	IF (@return = 0)
	begin
		insert into RebateforecastA
		select  '>2 Mths Up to 3 Mths', isnull(sum(porebate_p1),0), isnull(sum(porebate_p2),0), isnull(sum(porebate_p3),0),
			isnull(sum(porebate_p4),0), isnull(sum(porebate_p5),0), isnull(sum(porebate_p6),0), isnull(sum(porebate_p7),0),
			isnull(sum(porebate_p8),0), isnull(sum(porebate_p9),0), isnull(sum(porebate_p10),0), isnull(sum(porebate_p11),0),
			isnull(sum(porebate_p12),0), 30, @runperiodend, LEFT(acctno,3)
		from	#RebateForecast_byaccount_current
		where	report = 'A'
		and	rebatemonthsarrears <= 3
		AND	rebatemonthsarrears > 2 
		and	currstatus not in ('6','7','8')
		GROUP BY LEFT(acctno,3)

		SELECT @return = @@error
	end
	IF (@return = 0)
	begin
		insert into RebateforecastA
		select '>3 Mth Up to 4 Mths', isnull(sum(porebate_p1),0), isnull(sum(porebate_p2),0), isnull(sum(porebate_p3),0),
			isnull(sum(porebate_p4),0), isnull(sum(porebate_p5),0), isnull(sum(porebate_p6),0), isnull(sum(porebate_p7),0),
			isnull(sum(porebate_p8),0), isnull(sum(porebate_p9),0), isnull(sum(porebate_p10),0), isnull(sum(porebate_p11),0),
			isnull(sum(porebate_p12),0), 40, @runperiodend, LEFT(acctno,3)
		from	#RebateForecast_byaccount_current
		where	report = 'A'
		and	rebatemonthsarrears <= 4
		AND	rebatemonthsarrears > 3
		and	currstatus not in ('6','7','8')
		GROUP BY LEFT(acctno,3)

		SELECT @return = @@error
	end
	IF (@return = 0)
	begin
		insert into RebateforecastA
		select '>4 Mth Up to 6 Mths', isnull(sum(porebate_p1),0), isnull(sum(porebate_p2),0), isnull(sum(porebate_p3),0),
			isnull(sum(porebate_p4),0), isnull(sum(porebate_p5),0), isnull(sum(porebate_p6),0), isnull(sum(porebate_p7),0),
			isnull(sum(porebate_p8),0), isnull(sum(porebate_p9),0), isnull(sum(porebate_p10),0), isnull(sum(porebate_p11),0),
			isnull(sum(porebate_p12),0), 50, @runperiodend, LEFT(acctno,3)
		from	#RebateForecast_byaccount_current
		where	report = 'A'
		and	rebatemonthsarrears <= 6
		AND	rebatemonthsarrears > 4
		and	currstatus not in ('6','7','8')
		GROUP BY LEFT(acctno,3)

		SELECT @return = @@error
	end
	IF (@return = 0)
	begin
		insert into RebateforecastA
		select '>6 Mth Up to 12 Mths', isnull(sum(porebate_p1),0), isnull(sum(porebate_p2),0), isnull(sum(porebate_p3),0),
			isnull(sum(porebate_p4),0), isnull(sum(porebate_p5),0), isnull(sum(porebate_p6),0), isnull(sum(porebate_p7),0),
			isnull(sum(porebate_p8),0), isnull(sum(porebate_p9),0), isnull(sum(porebate_p10),0), isnull(sum(porebate_p11),0),
			isnull(sum(porebate_p12),0), 60, @runperiodend, LEFT(acctno,3)
		from	#RebateForecast_byaccount_current
		where	report = 'A'
		and	rebatemonthsarrears <= 12
		AND	rebatemonthsarrears > 6 
		and	currstatus not in ('6','7','8')
		GROUP BY LEFT(acctno,3)

		SELECT @return = @@error
	end
	IF (@return = 0)
	begin
		insert into RebateforecastA
		select '>12 Mths', isnull(sum(porebate_p1),0), isnull(sum(porebate_p2),0), isnull(sum(porebate_p3),0),
			isnull(sum(porebate_p4),0), isnull(sum(porebate_p5),0), isnull(sum(porebate_p6),0), isnull(sum(porebate_p7),0),
			isnull(sum(porebate_p8),0), isnull(sum(porebate_p9),0), isnull(sum(porebate_p10),0), isnull(sum(porebate_p11),0),
			isnull(sum(porebate_p12),0), 65, @runperiodend, LEFT(acctno,3)
		from	#RebateForecast_byaccount_current
		where	report = 'A'
		and	rebatemonthsarrears > 12 
		and	currstatus not in ('6','7','8')
		GROUP BY LEFT(acctno,3)

		SELECT @return = @@error
	end
	IF (@return = 0)
	begin
		insert into RebateforecastA
		select 'Status Code 6', isnull(sum(porebate_p1),0), isnull(sum(porebate_p2),0), isnull(sum(porebate_p3),0),
			isnull(sum(porebate_p4),0), isnull(sum(porebate_p5),0), isnull(sum(porebate_p6),0), isnull(sum(porebate_p7),0),
			isnull(sum(porebate_p8),0), isnull(sum(porebate_p9),0), isnull(sum(porebate_p10),0), isnull(sum(porebate_p11),0),
			isnull(sum(porebate_p12),0), 70, @runperiodend, LEFT(acctno,3)
		from	#RebateForecast_byaccount_current
		where	report = 'A'
		and	currstatus = '6'
		GROUP BY LEFT(acctno,3)	
	
		SELECT @return = @@error
	end
	IF (@return = 0)
	begin
		insert into RebateforecastA
		select 'Status Code 7', isnull(sum(porebate_p1),0), isnull(sum(porebate_p2),0), isnull(sum(porebate_p3),0),
			isnull(sum(porebate_p4),0), isnull(sum(porebate_p5),0), isnull(sum(porebate_p6),0), isnull(sum(porebate_p7),0),
			isnull(sum(porebate_p8),0), isnull(sum(porebate_p9),0), isnull(sum(porebate_p10),0), isnull(sum(porebate_p11),0),
			isnull(sum(porebate_p12),0), 80, @runperiodend, LEFT(acctno,3)
		from	#RebateForecast_byaccount_current
		where	report = 'A'
		and	currstatus = '7'
		GROUP BY LEFT(acctno,3)	

		SELECT @return = @@error
	end
	IF (@return = 0)
	begin
		insert into RebateforecastA
		select 'Status Code 8', isnull(sum(porebate_p1),0), isnull(sum(porebate_p2),0), isnull(sum(porebate_p3),0),
			isnull(sum(porebate_p4),0), isnull(sum(porebate_p5),0), isnull(sum(porebate_p6),0), isnull(sum(porebate_p7),0),
			isnull(sum(porebate_p8),0), isnull(sum(porebate_p9),0), isnull(sum(porebate_p10),0), isnull(sum(porebate_p11),0),
			isnull(sum(porebate_p12),0), 90, @runperiodend, LEFT(acctno,3)
		from	#RebateForecast_byaccount_current
		where	report = 'A'
		and	currstatus = '8'
		GROUP BY LEFT(acctno,3)

		SELECT @return = @@error
	end
	IF (@return = 0)
	begin
		insert into RebateforecastA
		select  'Totals', isnull(sum(porebate_p1),0), isnull(sum(porebate_p2),0), isnull(sum(porebate_p3),0),
			isnull(sum(porebate_p4),0), isnull(sum(porebate_p5),0), isnull(sum(porebate_p6),0), isnull(sum(porebate_p7),0),
			isnull(sum(porebate_p8),0), isnull(sum(porebate_p9),0), isnull(sum(porebate_p10),0), isnull(sum(porebate_p11),0),
			isnull(sum(porebate_p12),0), 100, @runperiodend, LEFT(acctno,3)
		from	#RebateForecast_byaccount_current
		where	report = 'A'
		GROUP BY LEFT(acctno,3)

		SELECT @return = @@error
	end
	if (@return <> 0)
	begin
		insert into RebateForecast_errorlog
		select	getdate(), 'Error occurred in REPORT A'
		return @return
	end

----------
--REPORT B
----------
	IF (@return = 0)
	begin
		insert into RebateforecastB
		select 'In Adv/Up To 1 Mth', isnull(sum(porebate_p1),0), isnull(sum(porebate_p2),0), isnull(sum(porebate_p3),0),
			isnull(sum(porebate_p4),0), isnull(sum(porebate_p5),0), isnull(sum(porebate_p6),0), isnull(sum(porebate_p7),0),
			isnull(sum(porebate_p8),0), isnull(sum(porebate_p9),0), isnull(sum(porebate_p10),0), isnull(sum(porebate_p11),0),
			isnull(sum(porebate_p12),0), 10, @runperiodend, LEFT(acctno,3)
		from	#RebateForecast_byaccount_current
		where	report = 'B' --delthresdate between @FromThresDateReportB and @runperiodend
		and	rebatemonthsarrears <= 1
		and	currstatus not in ('6','7','8')
		GROUP BY LEFT(acctno,3)

		SELECT @return = @@error
	end
--shouldn't need these arrears sections as new accts won't be in arrears
	IF (@return = 0)
	begin
		insert into RebateforecastB
		select '>1 Mth Up to 2 Mths', isnull(sum(porebate_p1),0), isnull(sum(porebate_p2),0), isnull(sum(porebate_p3),0),
			isnull(sum(porebate_p4),0), isnull(sum(porebate_p5),0), isnull(sum(porebate_p6),0), isnull(sum(porebate_p7),0),
			isnull(sum(porebate_p8),0), isnull(sum(porebate_p9),0), isnull(sum(porebate_p10),0), isnull(sum(porebate_p11),0),
			isnull(sum(porebate_p12),0), 20, @runperiodend, LEFT(acctno,3)
		from	#RebateForecast_byaccount_current
		where	report = 'B'
		and	rebatemonthsarrears <= 2
		AND	rebatemonthsarrears > 1 
		and	currstatus not in ('6','7','8')
		GROUP BY LEFT(acctno,3)

		SELECT @return = @@error
	end
	IF (@return = 0)
	begin
		insert into RebateforecastB
		select '>2 Mths Up to 3 Mths', isnull(sum(porebate_p1),0), isnull(sum(porebate_p2),0), isnull(sum(porebate_p3),0),
			isnull(sum(porebate_p4),0), isnull(sum(porebate_p5),0), isnull(sum(porebate_p6),0), isnull(sum(porebate_p7),0),
			isnull(sum(porebate_p8),0), isnull(sum(porebate_p9),0), isnull(sum(porebate_p10),0), isnull(sum(porebate_p11),0),
			isnull(sum(porebate_p12),0), 30, @runperiodend, LEFT(acctno,3)
		from	#RebateForecast_byaccount_current
		where	report = 'B'
		and	rebatemonthsarrears <= 3
		AND	rebatemonthsarrears > 2 
		and	currstatus not in ('6','7','8')
		GROUP BY LEFT(acctno,3)		

		SELECT @return = @@error
	end
	IF (@return = 0)
	begin
		insert into RebateforecastB
		select '>3 Mth Up to 4 Mths', isnull(sum(porebate_p1),0), isnull(sum(porebate_p2),0), isnull(sum(porebate_p3),0),
			isnull(sum(porebate_p4),0), isnull(sum(porebate_p5),0), isnull(sum(porebate_p6),0), isnull(sum(porebate_p7),0),
			isnull(sum(porebate_p8),0), isnull(sum(porebate_p9),0), isnull(sum(porebate_p10),0), isnull(sum(porebate_p11),0),
			isnull(sum(porebate_p12),0), 40, @runperiodend, LEFT(acctno,3)
		from	#RebateForecast_byaccount_current
		where	report = 'B'
		and	rebatemonthsarrears <= 4
		AND	rebatemonthsarrears > 3
		and	currstatus not in ('6','7','8')
		GROUP BY LEFT(acctno,3)	
	
		SELECT @return = @@error
	end
	IF (@return = 0)
	begin
		insert into RebateforecastB
		select '>4 Mth Up to 6 Mths', isnull(sum(porebate_p1),0), isnull(sum(porebate_p2),0), isnull(sum(porebate_p3),0),
			isnull(sum(porebate_p4),0), isnull(sum(porebate_p5),0), isnull(sum(porebate_p6),0), isnull(sum(porebate_p7),0),
			isnull(sum(porebate_p8),0), isnull(sum(porebate_p9),0), isnull(sum(porebate_p10),0), isnull(sum(porebate_p11),0),
			isnull(sum(porebate_p12),0), 50, @runperiodend, LEFT(acctno,3)
		from	#RebateForecast_byaccount_current
		where	report = 'B'
		and	rebatemonthsarrears <= 6
		AND	rebatemonthsarrears > 4
		and	currstatus not in ('6','7','8')
		GROUP BY LEFT(acctno,3)

		SELECT @return = @@error
	end
	IF (@return = 0)
	begin
		insert into RebateforecastB
		select '>6 Mth Up to 12 Mths', isnull(sum(porebate_p1),0), isnull(sum(porebate_p2),0), isnull(sum(porebate_p3),0),
			isnull(sum(porebate_p4),0), isnull(sum(porebate_p5),0), isnull(sum(porebate_p6),0), isnull(sum(porebate_p7),0),
			isnull(sum(porebate_p8),0), isnull(sum(porebate_p9),0), isnull(sum(porebate_p10),0), isnull(sum(porebate_p11),0),
			isnull(sum(porebate_p12),0), 60, @runperiodend, LEFT(acctno,3)
		from	#RebateForecast_byaccount_current
		where	report = 'B'
		and	rebatemonthsarrears <= 12
		AND	rebatemonthsarrears > 6 
		and	currstatus not in ('6','7','8')
		GROUP BY LEFT(acctno,3)

		SELECT @return = @@error
	end
	IF (@return = 0)
	begin
		insert into RebateforecastB
		select '>12 Mths', isnull(sum(porebate_p1),0), isnull(sum(porebate_p2),0), isnull(sum(porebate_p3),0),
			isnull(sum(porebate_p4),0), isnull(sum(porebate_p5),0), isnull(sum(porebate_p6),0), isnull(sum(porebate_p7),0),
			isnull(sum(porebate_p8),0), isnull(sum(porebate_p9),0), isnull(sum(porebate_p10),0), isnull(sum(porebate_p11),0),
			isnull(sum(porebate_p12),0), 65, @runperiodend,  LEFT(acctno,3)
		from	#RebateForecast_byaccount_current
		where	report = 'B'
		and	rebatemonthsarrears > 12 
		and	currstatus not in ('6','7','8')
		GROUP BY LEFT(acctno,3)

		SELECT @return = @@error
	end
	IF (@return = 0)
	begin
		insert into RebateforecastB
		select 'Status Code 6', isnull(sum(porebate_p1),0), isnull(sum(porebate_p2),0), isnull(sum(porebate_p3),0),
			isnull(sum(porebate_p4),0), isnull(sum(porebate_p5),0), isnull(sum(porebate_p6),0), isnull(sum(porebate_p7),0),
			isnull(sum(porebate_p8),0), isnull(sum(porebate_p9),0), isnull(sum(porebate_p10),0), isnull(sum(porebate_p11),0),
			isnull(sum(porebate_p12),0), 70, @runperiodend, LEFT(acctno,3)
		from	#RebateForecast_byaccount_current
		where	report = 'B'
		and	currstatus = '6'
		GROUP BY LEFT(acctno,3)

		SELECT @return = @@error
	end
	IF (@return = 0)
	begin
		insert into RebateforecastB
		select 'Status Code 7', isnull(sum(porebate_p1),0), isnull(sum(porebate_p2),0), isnull(sum(porebate_p3),0),
			isnull(sum(porebate_p4),0), isnull(sum(porebate_p5),0), isnull(sum(porebate_p6),0), isnull(sum(porebate_p7),0),
			isnull(sum(porebate_p8),0), isnull(sum(porebate_p9),0), isnull(sum(porebate_p10),0), isnull(sum(porebate_p11),0),
			isnull(sum(porebate_p12),0), 80, @runperiodend, LEFT(acctno,3)
		from	#RebateForecast_byaccount_current
		where	report = 'B'
		and	currstatus = '7'
		GROUP BY LEFT(acctno,3)

		SELECT @return = @@error
	end
	IF (@return = 0)
	begin
		insert into RebateforecastB
		select 'Status Code 8', isnull(sum(porebate_p1),0), isnull(sum(porebate_p2),0), isnull(sum(porebate_p3),0),
			isnull(sum(porebate_p4),0), isnull(sum(porebate_p5),0), isnull(sum(porebate_p6),0), isnull(sum(porebate_p7),0),
			isnull(sum(porebate_p8),0), isnull(sum(porebate_p9),0), isnull(sum(porebate_p10),0), isnull(sum(porebate_p11),0),
			isnull(sum(porebate_p12),0), 90, @runperiodend, LEFT(acctno,3)
		from	#RebateForecast_byaccount_current
		where	report = 'B'
		and	currstatus = '8'
		GROUP BY LEFT(acctno,3)

		SELECT @return = @@error
	end
	IF (@return = 0)
	begin
		insert into RebateforecastB
		select 'Totals', isnull(sum(porebate_p1),0), isnull(sum(porebate_p2),0), isnull(sum(porebate_p3),0),
			isnull(sum(porebate_p4),0), isnull(sum(porebate_p5),0), isnull(sum(porebate_p6),0), isnull(sum(porebate_p7),0),
			isnull(sum(porebate_p8),0), isnull(sum(porebate_p9),0), isnull(sum(porebate_p10),0), isnull(sum(porebate_p11),0),
			isnull(sum(porebate_p12),0), 100, @runperiodend, LEFT(acctno,3)
		from	#RebateForecast_byaccount_current
		where	report = 'B'
		GROUP BY LEFT(acctno,3)

		SELECT @return = @@error
	end
	IF (@return = 0)
	BEGIN
		--Averages by branch
		insert into RebateforecastB
		select 'Avg agrmt len, int%, ins%', isnull(avg(instalno),0), isnull(avg(servpcent),0),
			isnull(avg(inspcent),0), 0, 0, 0, 0, 0, 0, 0, 0, 0, 110, @runperiodend, LEFT(acctno,3)
		from	#RebateForecast_byaccount_current
		where	report = 'B'
		GROUP BY LEFT(acctno,3)
		--Averages for whole country, branch number = 0
		insert into RebateforecastB
		select 'Avg agrmt len, int%, ins%', isnull(avg(instalno),0), isnull(avg(servpcent),0),
			isnull(avg(inspcent),0), 0, 0, 0, 0, 0, 0, 0, 0, 0, 110, @runperiodend, 0
		from	#RebateForecast_byaccount_current
		where	report = 'B'
		

		SELECT @return = @@error
	end
	if (@return <> 0)
	begin
		insert into RebateForecast_errorlog
		select	getdate(), 'Error occurred in REPORT B'
		return @return
	end

----------
--YEAR END
----------
	--If year end then populate year end table from #RebateForecast_byaccount_current (do as second loop as need to use Report A for totals)
	if (month(@RunPeriodEnd) = 3 and day(@RunPeriodEnd) = 31)
	begin
		IF (@return = 0)
		begin
			select * into dbo.RebateForecast_byaccount_yearend from #RebateForecast_byaccount_current
		 	SELECT @return = @@error
		end
		if (@return <> 0)
		begin
			insert into RebateForecast_errorlog
			select	getdate(), 'Error occurred in YEAR END - save data and create tableD'
			return @return
		end
		--reset variables
		if (@return = 0)
		begin
			select 	@loopnumber = 1,
			   	@porebate = 'P' + convert(varchar,@loopnumber),
			   	@periodend = @runperiodend	--reset for new loop
		        SELECT @return = @@error
		end
		--need loop to insert each row
		while @loopnumber < 13
		begin
			if (@return = 0)
			begin
				select @sqlstr = ''
		 	    	select @sqlstr = (' select ' + convert(varchar,@sequence) +',''' + convert(varchar,@PeriodEnd)+''', '+@porebate + 
					     	  '        , 0, 0, 0, 0, 0, 0, 0, 0, 0, branchno '+
				     	    	  ' from   RebateforecastA ' +
				     	   	  ' where  arrearslevel = ''TOTALS'' '+
				     	    	  ' and    periodend = '' ' + convert(varchar,@RunPeriodEnd) + '''') --current year end date
		        	SELECT @return = @@error
			end
			if (@return = 0)
			begin
			    	execute (' insert into RebateforecastD ' + @sqlstr)
		            	SELECT @return = @@error
			end
			if (@return <> 0)
			begin
				insert into RebateForecast_errorlog
				select	getdate(), 'Error occurred in YEAR END - insert data in loop'
				return @return
			end
			--increase loopnumber
			IF (@return = 0)
    			BEGIN
			    	select	@sequence = @sequence + 10,
			   	   	@loopnumber = @loopnumber + 1,
				   	@porebate = 'P' + convert(varchar,@loopnumber),
				   	@PeriodEnd = (select min(enddate) from rebateforecast_periodenddates where enddate > @periodend and enddate <> '01-jan-1900')
			    	SELECT @return = @@error
			end
    			else
    			begin
			    	break
    			end
		end
		--year end actual is same as year end forecast
		if (@return = 0)
	    	begin
			update 	RebateforecastD
			set	actual = year_end_forecast
			from 	RebateforecastD
			where	period_end = @runperiodend
		
		        SELECT @return = @@error
	    	end
		if (@return <> 0)
		begin
			insert into RebateForecast_errorlog
			select	getdate(), 'Error occurred in YEAR END - end of loop'
			return @return
		end
	end
	if (@return <> 0)
	begin
		insert into RebateForecast_errorlog
		select	getdate(), 'Error occurred in YEAR END - copy existing RebateForecastD'
		return @return
	end

----------
--REPORT D
----------
	--get date of last year end
	if (@return = 0)
	begin
		select @previousyearend = (select max(enddate) from RebateForecast_periodenddates
		where enddate <= @runperiodend and day(enddate) = 31 and month(enddate) = 3)
	        SELECT @return = @@error
	end
	if @previousyearend = @runperiodend
	begin
		--set to 1 to start
		if (@return = 0)
		begin
			select @porebate = 'porebate_p1'
		        SELECT @return = @@error
		end
	end
	else
	begin
		--find correct columnname
		if (@return = 0)
		begin
			select @loopnumber = 2
		        SELECT @return = @@error
		end

		while @loopnumber < 13
		begin
 		   	if (@return = 0)
    			begin
	    			select @porebate = 'porebate_p' + convert(varchar,@loopnumber)
            			SELECT @return = @@error
			end
		     	if (@return = 0)
		    	begin
				select @previousyearend = (select min(enddate) from rebateforecast_periodenddates
		  				   	   where enddate > @previousyearend)
			    	SELECT @return = @@error
			end
		     	if (@return = 0)
		    	begin
		    		if (@previousyearend = @runperiodend)
    	    			begin
	    				break
    	    			end
		    		else
		    		begin
	        			select @loopnumber = @loopnumber + 1
				        SELECT @return = @@error
				end
			end
			else
			begin
			    	break
			end
		end
	end
	if (@return <> 0)
	begin
		insert into RebateForecast_errorlog
		select	getdate(), 'Error occurred in REPORT D'
		return @return
	end

----------
--REPORT C
----------
	--only populate these figures (for reports c and d) if not year end run
--cr927    	if not (month(@RunPeriodEnd) = 3 and day(@RunPeriodEnd) = 31) --year end period end is always 31-mar (Clare confirmed)
--cr927    	begin
		--initial insert
		if (@return = 0)
		begin
			insert into RebateforecastC
			-- CR931 - IP - 10/04/08 - Changed as now totals will be retrieved from 'RebateforecastA' even
			-- if there are none in 'RebateforecastB'.
			select	10, @runperiodend, ISNULL(a.p2, 0) + ISNULL(b.p2,0), 0, 0, 0, 0, 0, 0, 0, 0, 0, a.branchno
			from	RebateforecastA a LEFT OUTER JOIN RebateforecastB b ON 
				a.arrearslevel = b.arrearslevel
				and a.periodend = b.periodend 
				AND a.branchno = b.branchno
			WHERE a.arrearslevel = 'TOTALS' AND a.periodend = @previousperiodend
 
--			select	10, @runperiodend, a.p2 + b.p2, 0, 0, 0, 0, 0, 0, 0, 0, 0, a.branchno
--			from	RebateforecastA a, RebateforecastB b
--			where	a.arrearslevel = 'TOTALS'
--			and	b.arrearslevel = 'TOTALS'
--			and 	a.periodend = @previousperiodend
--			and 	b.periodend = @previousperiodend
			--GROUP BY a.branchno

	        	SELECT @return = @@error
		end
print 'x'
-----------------
--REPORTS C AND D
-----------------
		--actual forecast
		if (@return = 0)
	    	begin
/*			update 	RebateforecastC
			set	actual = p1
			from 	RebateforecastA
			where	arrearslevel = 'TOTALS'
			and 	periodend = @runperiodend*/
			
			--IP CR931 - Added 'branch' column as want to update by branch
			select 	LEFT(acctno, 3) AS branch, sum(porebate_p1) as rebatetotal
			into 	#actualc
			from 	#rebateforecast_byaccount_current c
			where 	c.report = 'a'
			GROUP BY LEFT(acctno, 3)
--			and 	exists (select acctno from #rebateforecast_byaccount_previous p
--				    	where p.report = 'a' and p.acctno = c.acctno)
		
		        SELECT @return = @@error
	    	END
		--IP - CR931 - Added join, to join by branch.
		if (@return = 0)
	    	begin
			update 	RebateforecastC
			set	actual = isnull(rebatetotal,0)
			from 	#actualc
			where	period_end = @runperiodend
			AND RebateforecastC.Branchno = #actualc.branch

		        SELECT @return = @@error
	    	END
		--IP CR931 - Added 'branch' column as want to update by branch
		if (@return = 0)
	    	begin
			
			select 	LEFT(acctno, 3)AS branch, sum(porebate_p1) as rebatetotal
			into 	#actuald
			from 	#rebateforecast_byaccount_current c
			where	report = 'a'
			and 	exists (select acctno from rebateforecast_byaccount_yearend p
					where report = 'a' and p.acctno = c.acctno)
			GROUP BY LEFT(acctno, 3)


/*			update 	RebateforecastD
			set	actual = p1
			from 	RebateforecastA
			where	arrearslevel = 'TOTALS'
			and 	RebateforecastD.period_end = @runperiodend
			and 	RebateforecastA.periodend = @runperiodend*/
		
		        SELECT @return = @@error
	    	END
		--IP - CR931 - Added join, to join by branch.
		if (@return = 0)
	    	begin
			update 	RebateforecastD
			set	actual = isnull(rebatetotal,0)
			from 	#actuald
			where	RebateforecastD.period_end = @runperiodend
			AND RebateforecastD.Branchno = #actuald.branch

		        SELECT @return = @@error
	    	END

--IP - CR931 - created temp table #RebateForecast_bybranch to hold sum of rebates by branch.
--settled early - exists in old but not new and currstatus is 'S'
--IP - Branch Total
		if (@return = 0)
		BEGIN
			SELECT sum(p.porebate_p2) AS porebatesum, LEFT(p.acctno, 3) AS branch --diff is sum of old calc as no current rebate
			INTO #RebateForecast_bybranch
					 from	#RebateForecast_byaccount_previous p, acct a
					 where	not exists (select acctno from #RebateForecast_byaccount_current c
							    where c.acctno = p.acctno and c.report = 'A')
					 and	a.acctno = p.acctno
--					 and 	p.report = 'A'
					 and 	p.porebate_p2 <> 0
					 and	a.currstatus = 'S'
					GROUP BY LEFT(p.acctno, 3)
		
			UPDATE RebateforecastC
			SET	account_settled_early = ISNULL(r.porebatesum, 0)
			FROM #RebateForecast_bybranch r
			WHERE period_end = @runperiodend
			AND RebateforecastC.Branchno = r.branch

		SELECT @return = @@error
		END

--IP - Commented out original 
--		if (@return = 0)
--	    	begin
--			update 	RebateforecastC
--			set	account_settled_early = isnull(
--					(select	sum(p.porebate_p2) --diff is sum of old calc as no current rebate
--					 from	#RebateForecast_byaccount_previous p, acct a
--					 where	not exists (select acctno from #RebateForecast_byaccount_current c
--							    where c.acctno = p.acctno and c.report = 'A')
--					 and	a.acctno = p.acctno
----					 and 	p.report = 'A'
--					 and 	p.porebate_p2 <> 0
--					 and	a.currstatus = 'S'),0)
--			where	period_end = @runperiodend
--
--		        SELECT @return = @@error
--	    	end

--IP - CR931 - 

			TRUNCATE TABLE #RebateForecast_bybranch

			if (@return = 0)
			BEGIN

			SELECT @sqlstr = ''
			SELECT @sqlstr = ('	 insert into #RebateForecast_bybranch' +
							 '	 select sum('+@porebate+'), left(p.acctno, 3)' +
							 '	 from	 RebateForecast_byaccount_yearend p, acct a '+
							 '	 where	 not exists (select acctno from #RebateForecast_byaccount_current c '+
							 '	  		     where c.acctno = p.acctno and c.report = ''A'') '+
							 '	 and	a.acctno = p.acctno ' +
							 ' 	 and	p.report = ''A'' '+
							 '	 and 	p.'+@porebate+' <> 0 '+
							 '	 and	a.currstatus = ''S'' group by left(p.acctno, 3)')
			
			EXECUTE(@sqlstr)

			SELECT @return = @@error
			END
			
			if (@return = 0)
			BEGIN	
			

SET @sqlstr = 'UPDATE RebateforecastD' +
					 ' set account_settled_early = isnull(r.porebatesum, 0)' +
					 ' from #RebateForecast_bybranch r ' +
					 'where RebateforecastD.period_end = '''+convert(varchar,@runperiodend)+'''
					  and RebateforecastD.Branchno = r.branch'


EXECUTE (@sqlstr)

			SELECT @return = @@error
			END
			
			if (@return <> 0)
			begin
				insert into RebateForecast_errorlog
				select	getdate(), 'Error occurred in REPORTS C AND D - settled early'
				return @return
			END

--IP - Commented out original 

--	    	if (@return = 0)
--	    	begin
--			select @sqlstr = ''
--			select @sqlstr = (' set	  account_settled_early = isnull('+
--					 '	 (select sum('+@porebate+') '+ --diff is sum of old calc as no current rebate
--					 '	  from	 RebateForecast_byaccount_yearend p, acct a '+
--					 '	  where	 not exists (select acctno from #RebateForecast_byaccount_current c '+
--					 '	  		     where c.acctno = p.acctno and c.report = ''A'') '+
--					 '	  and	a.acctno = p.acctno ' +
--					 ' 	  and	p.report = ''A'' '+
--					 '	  and 	p.'+@porebate+' <> 0 '+
----					 ' 	  and 	RebateforecastD.period_end = '''+convert(varchar,@runperiodend)+''''+
--					 '	  and	 a.currstatus = ''S''),0) '+
--					 '  where RebateforecastD.period_end = '''+convert(varchar,@runperiodend)+'''')
--		        SELECT @return = @@error
--	    	end
--	    	if (@return = 0)
--	    	begin
--			execute (' update RebateforecastD ' + @sqlstr)
--		        SELECT @return = @@error
--		end
--		if (@return <> 0)
--		begin
--			insert into RebateForecast_errorlog
--			select	getdate(), 'Error occurred in REPORTS C AND D - settled early'
--			return @return
--		end

--IP - CR931 - 

--balance is now <= 1 (no settled status though)

	TRUNCATE TABLE #RebateForecast_bybranch
	--IP - CR931 - Branch Total
	if (@return = 0)
		BEGIN
			INSERT INTO #RebateForecast_bybranch
			SELECT sum(p.porebate_p2), LEFT(P.ACCTNO, 3)--diff is sum of old calc as no current rebate
					FROM #RebateForecast_byaccount_previous p, acct a
					WHERE NOT EXISTS (select acctno from #RebateForecast_byaccount_current c
							   where c.acctno = p.acctno and c.report = 'A')
					and	a.acctno = p.acctno
					--and 	p.report = 'A'
					and 	p.porebate_p2 <> 0
					and	a.currstatus <> 'S'
					and	a.outstbal <= 1 
					GROUP BY LEFT(P.ACCTNO, 3)

----		--IP - CR931 - Company total
----		BEGIN
----			INSERT INTO #RebateForecast_bybranch
----			SELECT sum(p.porebate_p2), 999 --
----					FROM #RebateForecast_byaccount_previous p, acct a
----					WHERE NOT EXISTS (select acctno from #RebateForecast_byaccount_current c
----							   where c.acctno = p.acctno and c.report = 'A')
----					and 	a.acctno = p.acctno
------					and 	p.report = 'A'
----					and 	p.porebate_p2 <> 0
----					and	a.currstatus <> 'S'
----					and	a.outstbal <= 1 
----		END
	--IP - CR931 - Update RebateforecastC with branch and company totals.
			UPDATE RebateforecastC
			SET outstanding_balance_below_1 = ISNULL (r.porebatesum,0)			 
					FROM #RebateForecast_bybranch r
					WHERE period_end = @runperiodend
					AND RebateforecastC.Branchno = r.branch

			SELECT @return = @@error
		
		END	


--IP - Original commented out
--		--balance is now <= 1 (no settled status though)
--if (@return = 0)
--	    	begin
--			update 	RebateforecastC
--			set	outstanding_balance_below_1 = isnull(
--					(select	sum(p.porebate_p2) --diff is sum of old calc as no current rebate
--					 from	#RebateForecast_byaccount_previous p, acct a
--					 where	not exists (select acctno from #RebateForecast_byaccount_current c
--							    where c.acctno = p.acctno and c.report = 'A')
--					 and	a.acctno = p.acctno
----					 and 	p.report = 'A'
--					 and 	p.porebate_p2 <> 0
--					 and	a.currstatus <> 'S'
--					 and	a.outstbal <= 1),0)
--			where	period_end = @runperiodend
--
--		        SELECT @return = @@error
--	    	end

--IP - CR931 

--IP - CR931
			TRUNCATE TABLE #RebateForecast_bybranch
			if (@return = 0)
			BEGIN
			select @sqlstr = ('	  insert into #RebateForecast_bybranch' + 
							 '	  select sum('+@porebate+'),left(p.acctno, 3)'+ 
							 '	  from	 RebateForecast_byaccount_yearend p, acct a '+
							 '	  where	 not exists (select acctno from #RebateForecast_byaccount_current c '+
					         '  	     			 where c.acctno = p.acctno and c.report = ''A'') '+
							 '	  and	a.acctno = p.acctno ' +
					         ' 	  and	p.report = ''A'' '+
					         '	  and 	p.'+@porebate+' <> 0 '+
					         '	  and	a.currstatus <> ''S'' '+
							 '	  and	 a.outstbal <= 1 group by left(p.acctno, 3)')
			EXECUTE(@sqlstr)
			
			SELECT @return = @@error
			END
--			
--			select @sqlstr = '	  INSERT INTO #RebateForecast_bybranch' +
--							 '	  select sum('+@porebate+'), ''999''' +
--							 '	  from	 RebateForecast_byaccount_yearend p, acct a '+
--							 '	  where	 not exists (select acctno from #RebateForecast_byaccount_current c '+
--					         '  	     			 where c.acctno = p.acctno and c.report = ''A'') '+
--							 '	  and	a.acctno = p.acctno ' +
--					         ' 	  and	p.report = ''A'' '+
--					         '	  and 	p.'+@porebate+' <> 0 '+
--					         '	  and	a.currstatus <> ''S'' '+
--							 '	  and	 a.outstbal <= 1'
--			
			if (@return = 0)
			BEGIN
			SET @sqlstr = 'UPDATE RebateforecastD' +
					' SET outstanding_balance_below_1 = isnull(r.porebatesum, 0)' +
					'FROM #RebateForecast_bybranch r '+
					'WHERE RebateforecastD.period_end = '''+convert(varchar,@runperiodend)+'''  
					 AND RebateforecastD.Branchno = r.branch'  
			
			EXECUTE (@sqlstr)
			SELECT @return = @@error		
			END	

			if (@return <> 0)
			begin
				insert into RebateForecast_errorlog
				select	getdate(), 'Error occurred in REPORTS C AND D - outstbal below 1'
			return @return
			end

--IP - Original commented out

--if (@return = 0)
--	    	begin
--			select @sqlstr = ''
--			select @sqlstr = (' set	  outstanding_balance_below_1 = isnull('+
--					  '	 (select sum('+@porebate+') '+ --diff is sum of old calc as no current rebate
--					  '	  from	 RebateForecast_byaccount_yearend p, acct a '+
--					  '	  where	 not exists (select acctno from #RebateForecast_byaccount_current c '+
--					  '  	     		     where c.acctno = p.acctno and c.report = ''A'') '+
--					  '	  and	a.acctno = p.acctno ' +
--					  ' 	  and	p.report = ''A'' '+
--					  '	  and 	p.'+@porebate+' <> 0 '+
--					  '	  and	a.currstatus <> ''S'' '+
----					  ' 	  and 	RebateforecastD.period_end = '''+convert(varchar,@runperiodend)+''''+
--					  '	  and	 a.outstbal <= 1),0) '+
--					  ' where RebateforecastD.period_end = '''+convert(varchar,@runperiodend)+'''')
--		        SELECT @return = @@error
--	    	end
--	    	if (@return = 0)
--	    	begin
--			execute (' update RebateforecastD ' + @sqlstr)
--		        SELECT @return = @@error
--		end
--		if (@return <> 0)
--		begin
--			insert into RebateForecast_errorlog
--			select	getdate(), 'Error occurred in REPORTS C AND D - outstbal below 1'
--			return @return
--		end

--IP - CR931

--IP - CR931
--not met delivery threshold
			TRUNCATE TABLE #RebateForecast_bybranch

			if (@return = 0)
	    	BEGIN
			SELECT 	@actualdelpcent = @delpcent/100
		        SELECT @return = @@error
	    	END


			if (@return = 0)
	    	begin
			--get list of accounts left and their delivery total
			select	a.acctno, p.agrmttotal, p.porebate_p2, sum(transvalue) as deltotal
			into	#unaccounted1_reportc
			from	#RebateForecast_byaccount_previous p, acct a, fintrans f
			where	not exists (select acctno from #RebateForecast_byaccount_current c
					    where c.acctno = p.acctno and c.report = 'A')
			and	a.acctno = p.acctno
			and	a.acctno = f.acctno
			and     f.transtypecode in ('DEL','GRT','REP','ADD','RDL','RPO','CLD')         -- #10138
		 	and 	datetrans < dateadd(day,1,@runperiodend)
--			and 	p.report = 'A'
			and 	p.porebate_p2 <> 0
			and	a.currstatus <> 'S'
			and	a.outstbal > 1
			group by a.acctno, p.agrmttotal, p.porebate_p2
		        SELECT @return = @@error
	    	END
			
			--IP - CR931 - Select sum of rebate by branch
			IF (@return = 0)
			BEGIN
				INSERT INTO #RebateForecast_bybranch
				SELECT SUM(u.porebate_p2), LEFT(u.acctno, 3) 
				FROM #unaccounted1_reportc u
				WHERE u.deltotal < (u.agrmttotal * @actualdelpcent)
				GROUP BY LEFT(u.acctno, 3)
				
				SELECT @return = @@error
			END
			--IP - Added join to branch, as column updated based on branch
			IF(@return = 0)
			BEGIN
				UPDATE RebateforecastC
				SET	below_delivery_threshold = ISNULL(r.porebatesum, 0)
				FROM #RebateForecast_bybranch r
				WHERE period_end = @runperiodend
				AND RebateforecastC.Branchno = r.branch

				SELECT @return = @@error
			END

--IP - Original commented out
----		--not met delivery threshold
--if (@return = 0)
--	    	begin
--			select 	@actualdelpcent = @delpcent/100
--		        SELECT @return = @@error
--	    	end
--		if (@return = 0)
--	    	begin
--			--get list of accounts left and their delivery total
--			select	a.acctno, p.agrmttotal, p.porebate_p2, sum(transvalue) as deltotal
--			into	#unaccounted1_reportc
--			from	#RebateForecast_byaccount_previous p, acct a, fintrans f
--			where	not exists (select acctno from #RebateForecast_byaccount_current c
--					    where c.acctno = p.acctno and c.report = 'A')
--			and	a.acctno = p.acctno
--			and	a.acctno = f.acctno
--			and     f.transtypecode in ('DEL','GRT','REP','ADD','RDL','RPO')
--		 	and 	datetrans < dateadd(day,1,@runperiodend)
----			and 	p.report = 'A'
--			and 	p.porebate_p2 <> 0
--			and	a.currstatus <> 'S'
--			and	a.outstbal > 1
--			group by a.acctno, p.agrmttotal, p.porebate_p2
--		        SELECT @return = @@error
--	    	end
--	    	if (@return = 0)
--	    	begin
--			update 	RebateforecastC
--			set	below_delivery_threshold = isnull(
--					(select	sum(porebate_p2) --diff is sum of old calc as no current rebate
--					 from	#unaccounted1_reportc u
--					 where	deltotal < (agrmttotal * @actualdelpcent)),0)
--			where	period_end = @runperiodend
--
--		        SELECT @return = @@error
--	    	end

--IP - CR931

TRUNCATE TABLE #RebateForecast_bybranch

			IF (@return = 0)
	    	BEGIN
				if exists (select * from dbo.sysobjects where id = object_id('[dbo].[unaccounted1_reportd]') and OBJECTPROPERTY(id, 'IsUserTable') = 1)
				drop table [dbo].[unaccounted1_reportd]
			END
			
			if (@return = 0)
			BEGIN
				
				select @sqlstr = ''
				select @sqlstr = 
							(@porebate + ' as porebate, sum(transvalue) as deltotal '+
							' into	dbo.unaccounted1_reportd '+
							' from	RebateForecast_byaccount_yearend p, acct a, fintrans f '+
							' where not exists (select acctno from #RebateForecast_byaccount_current c '+
							' 	 where c.acctno = p.acctno and c.report = ''A'') '+
							' and	a.acctno = p.acctno '+
							' and	a.acctno = f.acctno '+
							' and  f.transtypecode in (''DEL'',''GRT'',''REP'',''ADD'',''RDL'',''RPO'',''CLD'') '+			-- #10138
		 					' and 	datetrans < dateadd(day,1,'''+convert(varchar,@runperiodend)+''')'+
							' and 	p.report = ''A'' '+
							' and 	p.' +@porebate+ ' <> 0 '+
							' and	a.currstatus <> ''S'' '+
							' and	a.outstbal > 1 '+
							' group by a.acctno, p.agrmttotal, p.' + convert(varchar,@porebate))
								SELECT @return = @@error
			SELECT @return = @@error
			END
			
			IF(@return = 0)
			BEGIN
				EXECUTE(' select a.acctno, p.agrmttotal, p.' + @sqlstr)
			SELECT @return = @@error
			END
			
			--IP - CR931 - Temorary table used to hold the sum of rebates by branch
			--which will be used later to update 'RebateforecastD' by branch.
			IF(@return = 0)
			BEGIN
				SELECT @sqlstr = ''
				SET @sqlstr = ' INSERT INTO #RebateForecast_bybranch ' +
							  ' SELECT SUM(u.porebate), LEFT(u.acctno, 3)' +
							  ' FROM unaccounted1_reportd u ' +
							  ' WHERE u.deltotal < (u.agrmttotal * '+convert(varchar,@actualdelpcent)+') ' +
							  ' GROUP BY LEFT(u.acctno, 3)'
				EXECUTE(@sqlstr)
				SELECT @return = @@error
			END
			--IP - CR931 - Column on RebateforecastD is updated by branch from the totals
			-- stored in temporary table #RebateForecast_bybranch 
			IF(@return = 0)
			BEGIN
				SELECT @sqlstr = ''
				SET @sqlstr = ' UPDATE RebateforecastD ' +
							  ' SET below_delivery_threshold = isnull(r.porebatesum, 0) ' +
							  ' FROM #RebateForecast_bybranch r ' +
							  ' WHERE RebateforecastD.period_end = '''+convert(varchar,@runperiodend)+'''
								AND RebateforecastD.Branchno = r.branch'
				EXECUTE(@sqlstr)
				SELECT @return = @@error
			END
		
			IF(@return <> 0)
			BEGIN
				INSERT INTO RebateForecast_errorlog
				SELECT	getdate(), 'Error occurred in REPORTS C AND D - delivery threshold not met'
			   return @return
			END

--IP - Original commented out
--  	if (@return = 0)
--	    	begin
--			if exists (select * from dbo.sysobjects where id = object_id('[dbo].[unaccounted1_reportd]') and OBJECTPROPERTY(id, 'IsUserTable') = 1)
--			drop table [dbo].[unaccounted1_reportd]
--		end
--	    	if (@return = 0)
--	    	begin
--			select @sqlstr = ''
--			select @sqlstr = 
--			(@porebate + ' as porebate, sum(transvalue) as deltotal '+
--			 ' into	dbo.unaccounted1_reportd '+
--			 ' from	RebateForecast_byaccount_yearend p, acct a, fintrans f '+
--			 ' where not exists (select acctno from #RebateForecast_byaccount_current c '+
--			 ' 	 where c.acctno = p.acctno and c.report = ''A'') '+
--			 ' and	a.acctno = p.acctno '+
--			 ' and	a.acctno = f.acctno '+
--			 ' and  f.transtypecode in (''DEL'',''GRT'',''REP'',''ADD'',''RDL'',''RPO'') '+
--		 	 ' and 	datetrans < dateadd(day,1,'''+convert(varchar,@runperiodend)+''')'+
--			 ' and 	p.report = ''A'' '+
--			 ' and 	p.' +@porebate+ ' <> 0 '+
--			 ' and	a.currstatus <> ''S'' '+
--			 ' and	a.outstbal > 1 '+
--			 ' group by a.acctno, p.agrmttotal, p.' + convert(varchar,@porebate))
--		         SELECT @return = @@error
--		end
--	    	if (@return = 0)
--	    	begin
--			execute (' select a.acctno, p.agrmttotal, p.' + @sqlstr)
--		        SELECT @return = @@error
--		end
--	    	if (@return = 0)
--	    	begin
--			select @sqlstr = ''
--			select @sqlstr = (' set	  below_delivery_threshold = isnull('+
--					  '	 (select sum(porebate) '+ --diff is sum of old calc as no current rebate
--					  '	  from	 unaccounted1_reportd u '+
----					  '	  where	 RebateforecastD.period_end = '''+convert(varchar,@runperiodend)+''''+
--					  ' 	  where	 deltotal < (agrmttotal * '+convert(varchar,@actualdelpcent)+ ')) ,0) '+
--					 '  where RebateforecastD.period_end = '''+convert(varchar,@runperiodend)+'''')
--		        SELECT @return = @@error
--	    	end
--	    	if (@return = 0)
--	    	begin
--			execute (' update RebateforecastD ' + @sqlstr)
--		        SELECT @return = @@error
--		end
--		if (@return <> 0)
--		begin
--			insert into RebateForecast_errorlog
--			select	getdate(), 'Error occurred in REPORTS C AND D - delivery threshold not met'
--			return @return
--		end

--IP - CR931
			--unaccounted 1
			TRUNCATE TABLE #RebateForecast_bybranch
			
			--IP - Changed so that the sum of rebate is now stored by branch.
			IF(@return = 0)
			BEGIN
				INSERT INTO #RebateForecast_bybranch
				SELECT SUM(u.porebate_p2), LEFT(u.acctno, 3)
				FROM #unaccounted1_reportc u
				WHERE deltotal >= (agrmttotal * @actualdelpcent)
				GROUP BY LEFT(u.acctno, 3)
				
				SELECT @return = @@error
			END
			
			--IP - RebateforecastC column is now updated by branch from
			-- the totals stored in the temporary table  #RebateForecast_bybranch
			IF(@return = 0)
			BEGIN
				UPDATE RebateforecastC
				SET unaccounted = ISNULL (r.porebatesum, 0)
				FROM #RebateForecast_bybranch r
				WHERE period_end = @runperiodend
				AND RebateforecastC.Branchno = r.branch

			SELECT @return = @@error
			END
		
			TRUNCATE TABLE #RebateForecast_bybranch
			
			--IP - Changed so that the sum of the rebate is now stored by branch
			IF(@return = 0)
			BEGIN
				SELECT @sqlstr = ''
				SET @sqlstr = ' INSERT INTO #RebateForecast_bybranch ' +
							  ' SELECT SUM(u.porebate), LEFT(u.acctno, 3) ' +
							  ' FROM unaccounted1_reportd u ' +
							  ' WHERE u.deltotal >= (u.agrmttotal * '+convert(varchar,@actualdelpcent)+ ')' +
							  ' GROUP BY LEFT(u.acctno, 3) '
				
				EXECUTE(@sqlstr)

			SELECT @return = @@error
			END
			
			--IP - RebateforecastD is now updated by branch based on the totals
			-- stored in the temporary table #RebateForecast_bybranch
			IF(@return = 0)
			BEGIN
				SELECT @sqlstr = ''
				SET @sqlstr = ' UPDATE RebateForecastD ' +
							  ' SET unaccounted = isnull(r.porebatesum, 0) ' +
							  ' FROM #RebateForecast_bybranch r ' +
							  ' WHERE RebateforecastD.period_end = '''+convert(varchar,@runperiodend)+'''
								AND RebateForecastD.Branchno = r.branch '
				
				EXECUTE(@sqlstr)
			
			SELECT @return = @@error
			END

			IF(@return <> 0)
			BEGIN
				INSERT INTO RebateForecast_errorlog
				SELECT getdate(), 'Error occurred in REPORTS C AND D - unaccounted 1'
				return @return
			END

--IP - Original commented out

--		--unaccounted 1
-- 	if (@return = 0)
--	    	begin
----used by STL to find accounts making-up the unaccounted1 value
----select * from dbo.unaccounted1_reportd
----where deltotal >= (agrmttotal * @actualdelpcent)
--			update 	RebateforecastC
--			set	unaccounted = isnull(
--					(select	sum(porebate_p2) --diff is sum of old calc as no current rebate
--					 from	#unaccounted1_reportc u
--					 where	deltotal >= (agrmttotal * @actualdelpcent)),0)
--			where	period_end = @runperiodend
--
--		        SELECT @return = @@error
--	    	end
--	    	if (@return = 0)
--	    	begin
--			select @sqlstr = ''
--			select @sqlstr = (' set	  unaccounted = isnull('+
--					  '	 (select sum(porebate) '+ --diff is sum of old calc as no current rebate
--					  '	  from	 unaccounted1_reportd u '+
----					  '	  where	 RebateforecastD.period_end = '''+convert(varchar,@runperiodend)+''''+
--					  ' 	  where	 deltotal >= (agrmttotal * '+convert(varchar,@actualdelpcent)+ ')) ,0) '+
--					 '  where RebateforecastD.period_end = '''+convert(varchar,@runperiodend)+'''')
--		        SELECT @return = @@error
--	    	end
--	    	if (@return = 0)
--	    	begin
--			execute (' update RebateforecastD ' + @sqlstr)
--		        SELECT @return = @@error
--		end
--		if (@return <> 0)
--		begin
--			insert into RebateForecast_errorlog
--			select	getdate(), 'Error occurred in REPORTS C AND D - unaccounted 1'
--			return @return
--		end

		--due date changed and agreement total changed - exists in both but different rebate value and agrmttotal and datefirst diff to previous table
		--CR931 - Changed #duedateagrmttotal to store sum of rebate differences by branch.
		if (@return = 0)
		begin
			--get accounts into temp table first
			select	LEFT(p.acctno, 3) AS branch,isnull((sum(p.porebate_p2) - sum(c.porebate_p1)),0) as diff --diff is sum of old calc less sum of new calc
			into	#duedateagrmttotal
			from	#RebateForecast_byaccount_current c, #RebateForecast_byaccount_previous p
			where	c.acctno = p.acctno
			and	c.porebate_p1 <> p.porebate_p2
			and	c.report = 'A'
--			and	p.report = 'A'
			and	c.agrmttotal <> p.agrmttotal
			and	c.datefirst <> p.DATEFIRST
			GROUP BY LEFT(p.acctno, 3)

		        SELECT @return = @@error
		END
		--IP - CR931 - Updating RebateforecastC column by branch using the 
		-- totals stored in temporary table #duedateagrmttotal
		if (@return = 0)
		BEGIN
			UPDATE 	RebateforecastC
			SET	agreement_revised_and_due_date_changed = diff
			FROM	#duedateagrmttotal
			WHERE	period_end = @runperiodend
			AND		RebateforecastC.Branchno = #duedateagrmttotal.branch
					/*(select	sum(p.porebate_p2) - sum(c.porebate_p1) --diff is sum of old calc less sum of new calc
					 from	#RebateForecast_byaccount_current c, #RebateForecast_byaccount_previous p
					 where	c.acctno = p.acctno
					 and	c.porebate_p1 <> p.porebate_p2
					 and	c.report = 'A'
					 and	p.report = 'A'
					 and	c.agrmttotal <> p.agrmttotal
					 and	c.datefirst <> p.datefirst),0)*/

		        SELECT @return = @@error
		END

--IP - CR931 

		TRUNCATE TABLE #RebateForecast_bybranch
		--IP - Save the difference for the rebate by branch into #RebateForecast_bybranch
		--which will be used later to update 'RebateForecastD'.
		IF(@return = 0)
		BEGIN
			SELECT @sqlstr = ''
			SET @sqlstr = ' INSERT INTO #RebateForecast_bybranch ' +
						  ' SELECT SUM(p.'+@porebate+') - SUM(c.porebate_p1), LEFT(p.acctno, 3) ' +
						  ' FROM #RebateForecast_byaccount_current c, RebateForecast_byaccount_yearend p ' +
						  ' WHERE c.acctno = p.acctno ' +
						  ' AND c.porebate_p1 <> p.'+@porebate +
						  ' AND c.report = ''A'' ' +
						  ' AND p.report = ''A'' ' +
						  ' AND c.agrmttotal <> p.agrmttotal ' +
						  ' AND c.datefirst <> p.datefirst ' +
						  ' GROUP BY LEFT(p.acctno, 3) '

			EXECUTE(@sqlstr)
		
		SELECT @return = @@error
		END
		
		--IP - Update RebateforecastD column by branch using the totals stored in
		-- the temporary table #RebateForecast_bybranch
		IF(@return = 0)
		BEGIN
			SELECT @SQLSTR = ''
			SET @SQLSTR = ' UPDATE RebateForecastD ' +
						  ' SET agreement_revised_and_due_date_changed = ISNULL(r.porebatesum, 0) ' +
						  ' FROM #RebateForecast_bybranch r ' +
						  ' WHERE RebateforecastD.period_end = '''+convert(varchar,@runperiodend)+'''' +
						  ' AND RebateforecastD.Branchno = r.branch '

			EXECUTE(@sqlstr)
	
		SELECT @return = @@error
		END

--IP - Original commented out
--	if (@return = 0)
--		begin
--			select @sqlstr = ''
--			select @sqlstr = (
--				 ' set	  agreement_revised_and_due_date_changed = isnull('+
--				 '	 (select sum(p.'+@porebate+') - sum(c.porebate_p1) '+ --diff is sum of old calc less sum of new calc
--				 '	  from	 #RebateForecast_byaccount_current c, RebateForecast_byaccount_yearend p '+
--				 '	  where	 c.acctno = p.acctno '+
--				 '	  and	 c.porebate_p1 <> p.'+@porebate +
--				 '	  and	 c.report = ''A'' '+
--				 '	  and	 p.report = ''A'' '+
--				 '	  and	 c.agrmttotal <> p.agrmttotal '+
----				 ' 	  and 	 RebateforecastD.period_end = '''+convert(varchar,@runperiodend)+''''+
--				 '	  and	 c.datefirst <> p.datefirst),0) '+
--				 '  where RebateforecastD.period_end = '''+convert(varchar,@runperiodend)+'''')
--		        SELECT @return = @@error
--		end
--	    	if (@return = 0)
--	    	begin
--			execute (' update RebateforecastD ' + @sqlstr)
--		        SELECT @return = @@error
--		END

		--agreement total changed - exists in both but different rebate value and agrmttotal diff to previous table
		--IP - CR931 - Changed to display totals by branch.
		IF (@return = 0)
		BEGIN
			SELECT	LEFT(p.acctno, 3) AS branch, isnull((sum(p.porebate_p2) - sum(c.porebate_p1)),0) as diff --diff is sum of old calc less sum of new calc
			INTO	#agrmttotal
			FROM	#RebateForecast_byaccount_current c, #RebateForecast_byaccount_previous p
			WHERE	c.acctno = p.acctno
			AND	c.porebate_p1 <> p.porebate_p2
			AND	c.report = 'A'
--			and	p.report = 'A'
			AND	c.agrmttotal <> p.agrmttotal
			AND	c.datefirst = p.DATEFIRST
			GROUP BY LEFT(p.acctno, 3)

		        SELECT @return = @@error
		END
		
		--IP - Update RebateforecastC column by branch based on the totals
		-- stored in temporary table #agrmttotal
		IF (@return = 0)
	    	BEGIN
			UPDATE 	RebateforecastC
			SET	agreement_revised_so_rebate_recalculated = diff
			FROM	#agrmttotal
			WHERE	period_end = @runperiodend
			AND RebateforecastC.Branchno = #agrmttotal.branch
/*					(select	sum(p.porebate_p2) - sum(c.porebate_p1) --diff is sum of old calc less sum of new calc
					 from	#RebateForecast_byaccount_current c, #RebateForecast_byaccount_previous p
					 where	c.acctno = p.acctno
					 and	c.porebate_p1 <> p.porebate_p2
					 and	c.report = 'A'
					 and	p.report = 'A'
					 and	c.agrmttotal <> p.agrmttotal
					 and	c.datefirst = p.datefirst),0)*/
		        SELECT @return = @@error
	    	END

--IP - CR931 - 

TRUNCATE TABLE #RebateForecast_bybranch
			
			--IP - Store rebate difference totals by branch.
			IF(@return = 0)
			BEGIN
				SELECT @sqlstr = ''
				SET @sqlstr = ' INSERT INTO #RebateForecast_bybranch ' +
							  ' SELECT SUM(p.'+@porebate+') - SUM(c.porebate_p1), LEFT(p.acctno, 3) ' + --diff is sum of old calc less sum of new calc
							  ' FROM #RebateForecast_byaccount_current c, RebateForecast_byaccount_yearend p ' +
							  ' WHERE c.acctno = p.acctno ' +
							  ' AND	 c.porebate_p1 <> p.'+@porebate +
							  '	AND c.report = ''A'' ' +
							  ' AND p.report = ''A'' ' +
							  ' AND c.agrmttotal <> p.agrmttotal ' +
							  ' AND c.datefirst = p.datefirst ' +
							  ' GROUP BY LEFT(p.acctno, 3) ' 
				
				EXECUTE(@sqlstr)
				
			SELECT @return = @@error 
			END
			
			--IP - Update RebateforecastD column by branch based on the totals 
			-- stored in the temporary table #RebateForecast_bybranch
			IF(@return = 0)
			BEGIN
				SELECT @sqlstr = ''
				SET @sqlstr = ' UPDATE RebateforecastD ' +
							  ' SET agreement_revised_so_rebate_recalculated = ISNULL(r.porebatesum, 0) ' +
							  ' FROM #RebateForecast_bybranch r ' +
							  ' WHERE RebateForecastD.period_end = '''+convert(varchar,@runperiodend)+'''' +
							  ' AND RebateForecastD.Branchno = r.branch '
				
				EXECUTE(@sqlstr)
		
			SELECT @return = @@error 
			END

			IF(@return <> 0)
			BEGIN
				INSERT INTO RebateForecast_errorlog
				SELECT getdate(), 'Error occurred in REPORTS C AND D - agrmttotal'
				return @return
			END

--IP - Original commented out
--	    	if (@return = 0)
--	    	begin
--			select @sqlstr = ''
--			select @sqlstr = (
--				 ' set	  agreement_revised_so_rebate_recalculated = isnull('+
--				 '	 (select sum(p.'+@porebate+') - sum(c.porebate_p1) '+ --diff is sum of old calc less sum of new calc
--				 '	  from	 #RebateForecast_byaccount_current c, RebateForecast_byaccount_yearend p '+
--				 '	  where	 c.acctno = p.acctno '+
----				 ' 	  and 	 RebateforecastD.period_end = '''+convert(varchar,@runperiodend)+''''+
--				 '	  and	 c.porebate_p1 <> p.'+@porebate +
--				 '	  and	 c.report = ''A'' '+
--				 '	  and	 p.report = ''A'' '+
--				 '	  and	 c.agrmttotal <> p.agrmttotal '+
--				 '	  and	 c.datefirst = p.datefirst),0) '+
--				 '  where RebateforecastD.period_end = '''+convert(varchar,@runperiodend)+'''')
--		        SELECT @return = @@error
--	    	end
--	    	if (@return = 0)
--	    	begin
--			execute (' update RebateforecastD ' + @sqlstr)
--		        SELECT @return = @@error
--		end
--		if (@return <> 0)
--		begin
--			insert into RebateForecast_errorlog
--			select	getdate(), 'Error occurred in REPORTS C AND D - agrmttotal'
--			return @return
--		end

--IP - CR931 - 

	--due date changed - exists in both but different rebate value and agrmttotal and datefirst diff to previous table
	--IP - Added 'branch' as totals of the difference in rebate are required by branch.
		IF (@return = 0)
	    	BEGIN
				SELECT	LEFT(p.acctno, 3) AS branch, isnull((sum(p.porebate_p2) - sum(c.porebate_p1)),0) AS diff --diff is sum of old calc less sum of new calc
				INTO	#duedate
				FROM	#RebateForecast_byaccount_current c, #RebateForecast_byaccount_previous p
				WHERE	c.acctno = p.acctno
				AND	c.porebate_p1 <> p.porebate_p2
				AND	c.report = 'A'
--				AND	p.report = 'A'
				AND	c.agrmttotal = p.agrmttotal
				AND	c.datefirst <> p.DATEFIRST
				GROUP BY LEFT(p.acctno, 3)

		        SELECT @return = @@error
			END
			
			--IP - CR931 - RebateforecastC column updated by branch based on the totals
			-- stored in the temporary table #duedate
	    	IF (@return = 0)
	    	BEGIN
				UPDATE	RebateforecastC
				SET	due_date_changed = diff
				FROM	#duedate
				WHERE	period_end = @runperiodend
				AND	RebateforeCastC.Branchno = #duedate.branch
					/*(select	sum(p.porebate_p2) - sum(c.porebate_p1) --diff is sum of old calc less sum of new calc
					 from	#RebateForecast_byaccount_current c, #RebateForecast_byaccount_previous p
					 where	c.acctno = p.acctno
					 and	c.porebate_p1 <> p.porebate_p2
					 and	c.report = 'A'
					 and	p.report = 'A'
					 and	c.agrmttotal = p.agrmttotal
					 and	c.datefirst <> p.datefirst),0)*/
		        SELECT @return = @@error
	    	END

--IP - CR931 
			TRUNCATE TABLE #RebateForecast_bybranch
			
			--IP - Rebate totals are stored by branch.
			IF(@return = 0)
			BEGIN
				SELECT @sqlstr = ''
				SET @sqlstr = ' INSERT INTO #RebateForecast_bybranch ' +
							  ' SELECT SUM(p.'+@porebate+') - SUM(c.porebate_p1), LEFT(p.acctno, 3) ' +
							  ' FROM #RebateForecast_byaccount_current c, RebateForecast_byaccount_yearend p ' +
							  ' WHERE c.acctno = p.acctno ' +
							  '	AND	c.porebate_p1 <> p.'+@porebate +
							  ' AND c.report = ''A'' ' +
							  ' AND p.report = ''A'' ' +
							  ' AND c.agrmttotal = p.agrmttotal ' +
							  ' AND c.datefirst <> p.datefirst ' +
							  ' GROUP BY LEFT(p.acctno, 3)'
				
			EXECUTE(@sqlstr)
			
			SELECT @return = @@error
			END
		
			--IP - RebateforecastD column updated by branch based on the totals
			-- stored in the temporary table #RebateForecast_bybranch
			IF(@return = 0)
			BEGIN
				SELECT @sqlstr = ''
				SET @sqlstr = ' UPDATE RebateforecastD ' +
							  ' SET due_date_changed = isnull(r.porebatesum, 0) ' +
							  ' FROM #Rebateforecast_bybranch r ' +
							  ' WHERE RebateforecastD.period_end = '''+convert(varchar,@runperiodend)+''' ' +
							  ' AND RebateforecastD.Branchno = r.branch'

			EXECUTE(@sqlstr)
			
			SELECT @return = @@error
			END

			IF(@return <> 0)
			BEGIN
				INSERT INTO RebateForecast_errorlog
				SELECT getdate(), 'Error occurred in REPORTS C AND D - due date'
				return @return
			END

--IP - Original commented out
--		--due date changed - exists in both but different rebate value and agrmttotal and datefirst diff to previous table
--		if (@return = 0)
--	    	begin
--			select	isnull((sum(p.porebate_p2) - sum(c.porebate_p1)),0) as diff --diff is sum of old calc less sum of new calc
--			into	#duedate
--			from	#RebateForecast_byaccount_current c, #RebateForecast_byaccount_previous p
--			where	c.acctno = p.acctno
--			and	c.porebate_p1 <> p.porebate_p2
--			and	c.report = 'A'
----			and	p.report = 'A'
--			and	c.agrmttotal = p.agrmttotal
--			and	c.datefirst <> p.datefirst
--
--		        SELECT @return = @@error
--		end
--	    	if (@return = 0)
--	    	begin
--			update 	RebateforecastC
--			set	due_date_changed = diff
--			from	#duedate
--			where	period_end = @runperiodend
--					/*(select	sum(p.porebate_p2) - sum(c.porebate_p1) --diff is sum of old calc less sum of new calc
--					 from	#RebateForecast_byaccount_current c, #RebateForecast_byaccount_previous p
--					 where	c.acctno = p.acctno
--					 and	c.porebate_p1 <> p.porebate_p2
--					 and	c.report = 'A'
--					 and	p.report = 'A'
--					 and	c.agrmttotal = p.agrmttotal
--					 and	c.datefirst <> p.datefirst),0)*/
--		        SELECT @return = @@error
--	    	end
--	    	if (@return = 0)
--	    	begin
--			select @sqlstr = ''
--			select @sqlstr = (
--				 ' set	  due_date_changed = isnull('+
--				 '	 (select sum(p.'+@porebate+') - sum(c.porebate_p1) '+ --diff is sum of old calc less sum of new calc
--				 '	  from	 #RebateForecast_byaccount_current c, RebateForecast_byaccount_yearend p '+
--				 '	  where	 c.acctno = p.acctno '+
----				 ' 	  and 	 RebateforecastD.period_end = '''+convert(varchar,@runperiodend)+''''+
--				 '	  and	 c.porebate_p1 <> p.'+@porebate +
--				 '	  and	 c.report = ''A'' '+
--				 '	  and	 p.report = ''A'' '+
--				 '	  and	 c.agrmttotal = p.agrmttotal '+
--				 '	  and	 c.datefirst <> p.datefirst),0) '+
--					 '  where RebateforecastD.period_end = '''+convert(varchar,@runperiodend)+'''')
--		        SELECT @return = @@error
--	    	end
--	    	if (@return = 0)
--	    	begin
--			execute (' update RebateforecastD ' + @sqlstr)
--		        SELECT @return = @@error
--		end
--		if (@return <> 0)
--		begin
--			insert into RebateForecast_errorlog
--			select	getdate(), 'Error occurred in REPORTS C AND D - due date'
--			return @return
--		end


--IP - CR931
		--datelast (and not datefirst) has been changed - exists in both but different rebate value and agrmttotal and datefirst diff to previous table
		--IP - CR931 - Added 'branch' column to display totals by branch.
	    	IF (@return = 0)
	    	BEGIN
				SELECT LEFT(p.acctno, 3) AS branch, isnull((sum(p.porebate_p2) - sum(c.porebate_p1)),0) AS diff --diff is sum of old calc less sum of new calc
				INTO #datelast
				FROM #RebateForecast_byaccount_current c, #RebateForecast_byaccount_previous p
				WHERE c.acctno = p.acctno
				AND c.porebate_p1 <> p.porebate_p2
				AND c.report = 'A'
--				AND p.report = 'A'
				AND c.agrmttotal = p.agrmttotal
				AND c.datefirst = p.datefirst
				AND c.datelast <> p.datelast
				GROUP BY LEFT(p.acctno, 3)

		        SELECT @return = @@error
	    	END
			
			--IP CR931 - Update RebateforecastC column by branch based on the totals
			-- stored in the temporary table #datelast
	    	IF (@return = 0)
	    	BEGIN
				UPDATE 	RebateforecastC
				SET	date_last_changed = diff
				FROM	#datelast
				WHERE	period_end = @runperiodend
				AND RebateforecastC.Branchno = #datelast.branch
					/*(select	sum(p.porebate_p2) - sum(c.porebate_p1) --diff is sum of old calc less sum of new calc
					 from	#RebateForecast_byaccount_current c, #RebateForecast_byaccount_previous p
					 where	c.acctno = p.acctno
					 and	c.porebate_p1 <> p.porebate_p2
					 and	c.report = 'A'
					 and	p.report = 'A'
					 and	c.agrmttotal = p.agrmttotal
					 and	c.datefirst = p.datefirst
					 and	c.datelast <> p.datelast),0)*/
		        SELECT @return = @@error
	    	END

--IP - CR931 - 
			TRUNCATE TABLE #RebateForecast_bybranch
			
			--IP - Store rebate totals by branch.
			IF(@return = 0)
			BEGIN
				SELECT @sqlstr = ''
				SET @sqlstr = ' INSERT INTO #RebateForecast_bybranch ' +
							  ' SELECT SUM(p.'+@porebate+') - SUM(c.porebate_p1), LEFT(p.acctno, 3) ' +
							  ' FROM #RebateForecast_byaccount_current c, RebateForecast_byaccount_yearend p ' +
							  ' WHERE c.acctno = p.acctno ' +
							  ' AND c.porebate_p1 <> p.'+@porebate + 
							  ' AND	c.report = ''A'' ' +
							  ' AND p.report = ''A'' ' +
							  ' AND c.agrmttotal = p.agrmttotal ' +
							  ' AND c.datefirst = p.datefirst ' +
							  ' AND c.datelast <> p.datelast ' +
							  ' GROUP BY LEFT(p.acctno, 3) ' 

				EXECUTE(@sqlstr)
	
			SELECT @return = @@error	
			END
		
			--IP - Update RebateforecastD column by branch based on the totals
			-- stored in the temporary table #RebateForecast_bybranch
			IF(@return = 0)
			BEGIN
				SELECT @sqlstr = ''
				SET @sqlstr = ' UPDATE RebateforecastD ' +
							  ' SET date_last_changed = ISNULL(r.porebatesum, 0) ' +
							  ' FROM #RebateForecast_bybranch r ' +
							  ' WHERE RebateforecastD.period_end = '''+convert(varchar,@runperiodend)+'''' +
							  ' AND RebateforecastD.Branchno = r.branch'

				EXECUTE(@sqlstr)

			SELECT @return = @@error
			END

			IF(@return <>0)
			BEGIN
				INSERT INTO RebateForecast_errorlog
				SELECT getdate(), 'Error occurred in REPORTS C AND D - date last'
				return @return
			END

--IP - Original commented out

--		--datelast (and not datefirst) has been changed - exists in both but different rebate value and agrmttotal and datefirst diff to previous table
--	    	if (@return = 0)
--	    	begin
--			select	isnull((sum(p.porebate_p2) - sum(c.porebate_p1)),0) as diff --diff is sum of old calc less sum of new calc
--			into	#datelast
--			from	#RebateForecast_byaccount_current c, #RebateForecast_byaccount_previous p
--			where	c.acctno = p.acctno
--			and	c.porebate_p1 <> p.porebate_p2
--			and	c.report = 'A'
----			and	p.report = 'A'
--			and	c.agrmttotal = p.agrmttotal
--			and	c.datefirst = p.datefirst
--			and	c.datelast <> p.datelast
--
--		        SELECT @return = @@error
--	    	end
--	    	if (@return = 0)
--	    	begin
--			update 	RebateforecastC
--			set	date_last_changed = diff
--			from	#datelast
--			where	period_end = @runperiodend
--					/*(select	sum(p.porebate_p2) - sum(c.porebate_p1) --diff is sum of old calc less sum of new calc
--					 from	#RebateForecast_byaccount_current c, #RebateForecast_byaccount_previous p
--					 where	c.acctno = p.acctno
--					 and	c.porebate_p1 <> p.porebate_p2
--					 and	c.report = 'A'
--					 and	p.report = 'A'
--					 and	c.agrmttotal = p.agrmttotal
--					 and	c.datefirst = p.datefirst
--					 and	c.datelast <> p.datelast),0)*/
--		        SELECT @return = @@error
--	    	end
--	    	if (@return = 0)
--	    	begin
--			select @sqlstr = ''
--			select @sqlstr = (
--				 ' set	  date_last_changed = isnull('+
--				 '	 (select sum(p.'+@porebate+') - sum(c.porebate_p1) '+ --diff is sum of old calc less sum of new calc
--				 '	  from	 #RebateForecast_byaccount_current c, RebateForecast_byaccount_yearend p '+
--				 '	  where	 c.acctno = p.acctno '+
----				 ' 	  and 	 RebateforecastD.period_end = '''+convert(varchar,@runperiodend)+''''+
--				 '	  and	 c.porebate_p1 <> p.'+@porebate +
--				 '	  and	 c.report = ''A'' '+
--				 '	  and	 p.report = ''A'' '+
--				 '	  and	 c.agrmttotal = p.agrmttotal '+
--				 '	  and	 c.datefirst = p.datefirst '+
--				 '	  and	 c.datelast <> p.datelast),0) '+
--					 '  where RebateforecastD.period_end = '''+convert(varchar,@runperiodend)+'''')
--		        SELECT @return = @@error
--	    	end
--	    	if (@return = 0)
--	    	begin
--			execute (' update RebateforecastD ' + @sqlstr)
--		        SELECT @return = @@error
--		end
--		if (@return <> 0)
--		begin
--			insert into RebateForecast_errorlog
--			select	getdate(), 'Error occurred in REPORTS C AND D - date last'
--			return @return
--		end

--CR931

		--unaccounted 2 - exists in both but different rebate value and agrmttotal and datefirst diff to previous table
		--IP - CR931 - Added 'branch' column as totals required by branch.
	    	IF (@return = 0)
	    	BEGIN
				SELECT	LEFT(p.acctno, 3) AS branch, isnull((sum(p.porebate_p2) - sum(c.porebate_p1)),0) AS diff --sum of old calc less sum of new calc
				INTO	#unaccounted2
				FROM	#RebateForecast_byaccount_current c, #RebateForecast_byaccount_previous p
				WHERE	c.acctno = p.acctno
				AND	c.porebate_p1 <> p.porebate_p2
				AND c.report = 'A'
--				AND	p.report = 'A'
				AND c.agrmttotal = p.agrmttotal
				AND c.datefirst = p.datefirst
				AND c.datelast = p.datelast
				GROUP BY LEFT(p.acctno, 3)

		        SELECT @return = @@error
	    	END

			--IP - Update RebateforecastC column by branch based on the totals
			-- stored in the temporary table #unaccounted2
	    	IF (@return = 0)
	    	BEGIN
				UPDATE 	RebateforecastC
				SET	unaccounted = unaccounted + diff
				FROM	#unaccounted2
				WHERE	period_end = @runperiodend
				AND RebateforecastC.Branchno = #unaccounted2.branch
					/*(select	sum(p.porebate_p2) - sum(c.porebate_p1) --sum of old calc less sum of new calc --need to check which way round
					 from	#RebateForecast_byaccount_current c, #RebateForecast_byaccount_previous p
					 where	c.acctno = p.acctno
					 and	c.porebate_p1 <> p.porebate_p2
					 and	c.report = 'A'
					 and	p.report = 'A'
					 and	c.agrmttotal = p.agrmttotal
					 and	c.datefirst = p.datefirst
					 and	c.datelast = p.datelast),0)*/
		        SELECT @return = @@error
	    	END

	    	IF (@return = 0)
	    	BEGIN
				IF EXISTS (select * from dbo.sysobjects where id = object_id('[dbo].[unaccounted2_d]') and OBJECTPROPERTY(id, 'IsUserTable') = 1)
				DROP TABLE [dbo].[unaccounted2_d]
			END

--IP - CR931 - 
			--IP - Store the rebate totals by branch.
			IF(@return = 0)
			BEGIN
				SELECT @sqlstr = ''
				SET @sqlstr = ' SELECT LEFT(p.acctno, 3) AS branch, ISNULL((SUM(p.'+@porebate+') - SUM(c.porebate_p1)), 0) AS diff ' +
							  ' INTO [dbo].[unaccounted2_d] ' +
							  ' FROM #RebateForecast_byaccount_current c, RebateForecast_byaccount_yearend p ' +
							  ' WHERE c.acctno = p.acctno ' +
							  ' AND c.porebate_p1 <> p.'+@porebate+
							  ' AND c.report = ''A'' ' +
							  ' AND p.report = ''A'' ' +
							  ' AND c.agrmttotal = p.agrmttotal ' +
							  ' AND c.datefirst = p.datefirst ' +
							  ' AND c.datelast = p.datelast ' +
							  ' GROUP BY LEFT(p.acctno, 3)'

				EXECUTE(@sqlstr)
			SELECT @return = @@error
			END
			
			--IP - Update RebateforecastD column by branch based on the totals 
			-- stored in the temporary table unaccounted2_d
			IF(@return = 0)
			BEGIN
				SELECT @sqlstr = ''
				SET @sqlstr = ' UPDATE RebateforecastD ' +
							  ' SET unaccounted = unaccounted + diff ' +
							  ' FROM [dbo].[unaccounted2_d]' +
							  ' WHERE RebateforecastD.period_end = '''+convert(varchar,@runperiodend)+'''' +
							  ' AND RebateforecastD.Branchno = unaccounted2_d.branch'
			
				EXECUTE(@sqlstr)

			SELECT @return = @@error
			END

			IF(@return <> 0)
			BEGIN
				INSERT INTO RebateForecast_errorlog
				SELECT getdate(), 'Error occurred in REPORTS C AND D - unaccounted 2'
				return @return
			END

----IP - Original commented out
--		--unaccounted 2 - exists in both but different rebate value and agrmttotal and datefirst diff to previous table
--	    	if (@return = 0)
--	    	begin
--			select	isnull((sum(p.porebate_p2) - sum(c.porebate_p1)),0) as diff --sum of old calc less sum of new calc
--			into	#unaccounted2
--			from	#RebateForecast_byaccount_current c, #RebateForecast_byaccount_previous p
--			where	c.acctno = p.acctno
--			and	c.porebate_p1 <> p.porebate_p2
--			and	c.report = 'A'
----			and	p.report = 'A'
--			and	c.agrmttotal = p.agrmttotal
--			and	c.datefirst = p.datefirst
--			and	c.datelast = p.datelast
--
--		        SELECT @return = @@error
--	    	end
--	    	if (@return = 0)
--	    	begin
--			update 	RebateforecastC
--			set	unaccounted = unaccounted + diff
--			from	#unaccounted2
--			where	period_end = @runperiodend
--					/*(select	sum(p.porebate_p2) - sum(c.porebate_p1) --sum of old calc less sum of new calc --need to check which way round
--					 from	#RebateForecast_byaccount_current c, #RebateForecast_byaccount_previous p
--					 where	c.acctno = p.acctno
--					 and	c.porebate_p1 <> p.porebate_p2
--					 and	c.report = 'A'
--					 and	p.report = 'A'
--					 and	c.agrmttotal = p.agrmttotal
--					 and	c.datefirst = p.datefirst
--					 and	c.datelast = p.datelast),0)*/
--		        SELECT @return = @@error
--	    	end
--	    	if (@return = 0)
--	    	begin
--			if exists (select * from dbo.sysobjects where id = object_id('[dbo].[unaccounted2_d]') and OBJECTPROPERTY(id, 'IsUserTable') = 1)
--			drop table [dbo].[unaccounted2_d]
--		end
--	    	if (@return = 0)
--	    	begin
--			select @sqlstr = ''
--			select @sqlstr = (
--				@porebate+') - sum(c.porebate_p1)),0) as diff  '+ --sum of old calc less sum of new calc
--				' into	[dbo].[unaccounted2_d] '+
--				' from	#RebateForecast_byaccount_current c, RebateForecast_byaccount_yearend p '+
--				' where	c.acctno = p.acctno '+
--				' and	c.porebate_p1 <> p.'+@porebate+
--				' and	c.report = ''A'' '+
--				' and	p.report = ''A'' '+
--				' and	c.agrmttotal = p.agrmttotal '+
--				' and	c.datefirst = p.datefirst '+
--				' and	c.datelast = p.datelast ')
--
--		        SELECT @return = @@error
--	    	end
--	    	if (@return = 0)
--	    	begin
--			execute (' select isnull((sum(p.' + @sqlstr)
--		        SELECT @return = @@error
--		end
--	    	if (@return = 0)
--	    	begin
--			select @sqlstr = ''
--			select @sqlstr = (
--				 ' set	  unaccounted = unaccounted + diff '+
--				 ' from	  [dbo].[unaccounted2_d] '+
--				 ' where  RebateforecastD.period_end = '''+convert(varchar,@runperiodend)+'''')
--/*				 '	 (select sum(p.'+@porebate+') - sum(c.porebate_p1) '+ --diff is sum of old calc less sum of new calc
--				 '	  from	 #RebateForecast_byaccount_current c, RebateForecast_byaccount_yearend p '+
--				 '	  where	 c.acctno = p.acctno '+
----				 ' 	  and 	RebateforecastD.period_end = '''+convert(varchar,@runperiodend)+''''+
--				 '	  and	 c.porebate_p1 <> p.'+@porebate +
--				 '	  and	 c.report = ''A'' '+
--				 '	  and	 p.report = ''A'' '+
--				 '	  and	 c.agrmttotal = p.agrmttotal '+
--				 '	  and	 c.datefirst = p.datefirst '+
--				 '	  and	 c.datelast = p.datelast),0) '+
--					 '  where RebateforecastD.period_end = '''+convert(varchar,@runperiodend)+'''')*/
--		        SELECT @return = @@error
--	    	end
--	    	if (@return = 0)
--	    	begin
--			execute (' update RebateforecastD ' + @sqlstr)
--		        SELECT @return = @@error
--		end
--		if (@return <> 0)
--		begin
--			insert into RebateForecast_errorlog
--			select	getdate(), 'Error occurred in REPORTS C AND D - unaccounted 2'
--			return @return
--		end
--	end --CR927

------------------------
--SAVE/REMOVE OLD DATA 2
------------------------
    --KEF CR927 Need to save 1 more months worth of data so store previous month's before dropping it
	if (@return = 0)
	BEGIN
--		drop table dbo.RebateForecast_byaccount_previous_previous
		TRUNCATE TABLE RebateForecast_byaccount_previous_previous
		SELECT @return = @@error
	END
--IP - put this into upgrade script
--	if (@return = 0)
	BEGIN
		INSERT INTO dbo.RebateForecast_byaccount_previous_previous
		SELECT * FROM dbo.RebateForecast_byaccount_previous
--		select * into dbo.RebateForecast_byaccount_previous_previous from dbo.RebateForecast_byaccount_previous
    SELECT @return = @@error
	end
	--Save and clear data from last run
	if (@return = 0)
	begin
--		drop table RebateForecast_byaccount_previous
		TRUNCATE TABLE RebateForecast_byaccount_previous
		SELECT @return = @@error
	end
    	if (@return = 0)
	begin
--		drop table RebateForecast_byaccount_current
		TRUNCATE TABLE RebateForecast_byaccount_current
		SELECT @return = @@error
	    end
    	if (@return <> 0)
	begin
		insert into RebateForecast_errorlog
		select	getdate(), 'Error occurred in SAVE/REMOVE OLD DATA 2'
		return @return
	end


-------------
--DATA INSERT
-------------
	if (@return = 0)
	BEGIN
		INSERT INTO dbo.RebateForecast_byaccount_current 
		SELECT * FROM #RebateForecast_byaccount_current
--		select * into dbo.RebateForecast_byaccount_current from #RebateForecast_byaccount_current
	        SELECT @return = @@error
	end
	if (@return = 0)
	BEGIN
		INSERT INTO dbo.RebateForecast_byaccount_previous
		SELECT * FROM #RebateForecast_byaccount_previous
--		select * into dbo.RebateForecast_byaccount_previous from #RebateForecast_byaccount_previous
	        SELECT @return = @@error
	end
	if (@return <> 0)
	begin
		insert into RebateForecast_errorlog
		select	getdate(), 'DATA INSERT'
		return @return
	end

----------
--RUN DATE
----------
	--update rundate in RebateForecast_periodenddates table
  	if (@return = 0)
  	begin
		update	RebateForecast_periodenddates
		set	rundate = getdate()
		where	enddate = @runperiodend
	
	        SELECT @return = @@error
	    -- Add new period end dates (if required)
	    exec PeriodEndDatesSP
  	end
	if (@return <> 0)
	begin
		insert into RebateForecast_errorlog
		select	getdate(), 'Error occurred in RUN DATE'
		return @return
	end
go

SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End 