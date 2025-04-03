SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[CalculateBduRebate]')
					and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[CalculateBduRebate]
GO

CREATE PROCEDURE [dbo].[CalculateBduRebate]

--leave in this order
    @AcctNo 		  char(12) = '',
    @poRebate 		  money = 0 OUTPUT,
    @return 		  int   = 0 OUTPUT,
    @FromDate		  datetime = '01-jan-1900',
    @UntilThresDate	  datetime = '01-jan-1900',
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
	@StLuciaRuleChange	DATETIME,	-- CR938 jec 01/04/08
	@Currdate	datetime,
	@currDay datetime,			--#13313
	@returnRF INT
	
-- UAT130 (RS) - jec 
set @Currdate =GETDATE()	
	
set @currDay = 	convert(datetime,convert(varchar(11),@Currdate)) --#13313

if(datediff(mi, @CurrDay, @CurrDate) < 480) --#13313 - running before 8am then set to previous day
begin
	set @currDay = dateadd(day, -1, @currDay)
end


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

    select @rule = value from countrymaintenance where codename = 'BduRebateCalculationRule'
   
	if (select isnull(@UntilThresDate,'01-jan-1900')) = '01-jan-1900'
		select @UntilThresDate = convert(datetime,@currDay,111) --#13313

	if (select isnull(@RebateDate,'01-jan-1900')) = '01-jan-1900'
		select @RebateDate = convert(datetime,@currDay,111)		--#13313


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
		if (select deltot from @rebates) <= 0
		begin
			--return zero rebate for account
			select 	@porebate = 0,
				@return = 0
			return
		end
	END

	if (@return = 0)
	BEGIN

        --When being written off, all accounts should use the BDU rule
			     UPDATE @rebates
        		    SET    Rule78 = '78' + convert(char(2),@rule)
			
	END

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
		if (select totalmonths from @rebates) <= 0
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
		or (@countrycode = 'L'))		-- jec 30/04/09)
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
				if @CountryCode = 'L'	-- jec 30/04/09
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
			--only return if rebate was set
			if (select calcdone from @rebates ) = 'Y'
			begin
				select 	@porebate = rebatetotal,
					@return = 0
				from	@rebates

			    	return
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
		if (select monthstogo from @rebates where Insplit <> 1) <= 0
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
		if @CountryCode = 'L'
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
		if @CountryCode = 'L'
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
		if @CountryCode = 'L'
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
		if @CountryCode = 'L' 
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
    IF (@return = 0)
	begin
		select 	@porebate = rebatetotal
		from	@rebates

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

GO