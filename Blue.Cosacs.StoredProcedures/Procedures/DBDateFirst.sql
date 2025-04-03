SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 

GO
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id('[dbo].[DBDateFirst]') AND OBJECTPROPERTY(id, 'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DBDateFirst]
GO

CREATE PROCEDURE [dbo].[DBDateFirst]
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DBDateFirst.PRC
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : DBDateFirst
-- Author       : ??
-- Date         : ?
-- Version                : 002
-- Created for CR316 
-- This calculates the date first
-- 
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- DSR 29/06/01 Changed DueDayId query to use latest mandate history record
-- DSR 03/03/03 113592 Changed giro to be ignored until mandate approval date entered
-- DSR 02/05/03 Rationalised to combine Singapore Giro with RF functionality
-- AA 19/01/04 63890 updating mths deferred to add to datefirst
-- AA 08/02/06 67942 was not working as not getting the details FROM the termstype table.
-- KEF 13/09/06 68504 added extra check whenchecking for ddenabled for RF as RF sub agreements were 
--				not having their due date brought into line with existing RF accounts
-- jec 21/12/07 UAT153 Deferred months not being correctly calculated
-- 15/7/2020  10.7 CR-Called the First Installment payment date procedure if the AbilitySetFirstPaymentDate setting is enable.
-- ================================================
	-- Add the parameters for the stored procedure here

-- Created for CR316 

    @acctno     CHAR(12),
    @datedel    DATETIME,           -- Delivery Date
    @datefirst  DATETIME    OUTPUT  -- New DateFirst on InstalPlan table

AS
BEGIN
    DECLARE @DDEnabled      SMALLINT        -- Direct Debits (Giro) enabled
    DECLARE @MinPeriod      SMALLINT        -- Giro min period between delivery and datefirst
    DECLARE @DueDay         SMALLINT        -- DDMandate Due Day
    DECLARE @DueDate        SMALLDATETIME   -- DDmandate Due Date
    DECLARE @EndDate        SMALLDATETIME   -- DDmandate End Date
    DECLARE @Type           VARCHAR(2)		-- Account Type
    DECLARE @rowcount       SMALLINT		-- number of rows holder
    DECLARE @fixeddatefirst SMALLINT		-- to determine if recording due day
	DECLARE @PreffirstInstallmentDay bit	-- to determine if Preference First Installmet Day Enabled    --Added By RahulSonawane
    DECLARE @mthsdeferred SMALLINT, @mthstoadd SMALLINT, @termstype VARCHAR (2), @isAmortized BIT
	DECLARE @DateFirstInstallment DATE
    DECLARE @DateLastInstallment DATE

    -- get number of months deferred if special a/c type
	SELECT	@mthsdeferred=ISNULL(mthsdeferred,0) 
	FROM	accttype
			INNER JOIN acct ON (accttype.accttype = acct.accttype OR acct.accttype = accttype.genaccttype)
	WHERE	--(accttype.accttype = acct.accttype OR acct.accttype = accttype.genaccttype) AND
			acctno = @acctno


    IF @mthsdeferred = 0 -- check the termstype which has recently had deferred months added to it
    BEGIN
		SELECT	@termstype = termstype 
        FROM	acct 
        WHERE	acctno =@acctno

        SELECT	@mthsdeferred = ISNULL(deferredmonths,0) 
        FROM	termstype 
        WHERE	termstype = @termstype
    END

    SET @mthstoadd = 1 + @mthsdeferred

    -- Prepare for Giro processing 
    SELECT  @DDEnabled = DDEnabled,
            @MinPeriod = MinPeriod,
			@fixeddatefirst = fixeddatefirst				
    FROM    country

    -- Check for RF processing 
    SELECT  @type = accttype, @isAmortized = isAmortized
    FROM    acct
    WHERE   acctno = @acctno

    DECLARE @count_ready  INT
	SET @count_ready = 0

	SELECT	@PreffirstInstallmentDay = ISNULL(Value,0) 
	FROM	[CountryMaintenance] WITH(NOLOCK)
	WHERE	CodeName = 'AbilitySetFirstPaymentDate'   -- First Installment Payment date setting is active
    

    IF @type = 'R'
    BEGIN
        -- Align Date First for Ready Finance accounts 
        -- Find the earliest DateFirst for this customer's RF accounts

        DECLARE @custid       VARCHAR (20)
		DECLARE @IsCashLoanAcct BIT = 0

        SELECT @custid = custid
        FROM   custacct
        WHERE  acctno = @acctno
        AND    hldorjnt = 'H'

		-- Check if Acct is of CashLoan type 
		-- because AbilitySetFirstPaymentDate setting from CountryMaintenance is not applicable for cashloan account
		IF EXISTS (SELECT 1 FROM CashLoan WHERE AcctNo = @acctno)
		BEGIN
			SET @IsCashLoanAcct = 1
		END

        SELECT @count_ready = COUNT(custacct.acctno),
               @datefirst   = MIN(instalplan.datefirst),
               @dueday = DATEPART(dd,MIN(instalplan.datefirst))
        FROM   custacct
        INNER JOIN   acct       ON acct.acctno       = custacct.acctno -- ?? Check AcctType
        INNER JOIN   agreement  ON agreement.acctno  = custacct.acctno
        INNER JOIN   instalplan ON instalplan.acctno = custacct.acctno
        WHERE  custacct.custid			= @custid
        AND    custacct.hldorjnt		= 'H'
        AND    acct.AcctType			= 'R'
        AND    acct.currstatus			!='S'    -- sl should not include settled accounts
        AND    agreement.deliveryflag	= 'Y'
        AND    agreement.acctno			!= @acctno
        AND    datefirst				> CONVERT(SMALLDATETIME,'01-01-1910',105)
       
        IF @count_ready > 0  AND @isAmortized = 0
        BEGIN
			
			 -- Check if AbilitySetFirstPaymentDate setting is applied in CountryMaintenance for RF account
			 -- OR if RF account account is type of cashloan
			 IF (@PreffirstInstallmentDay = 0 OR @IsCashLoanAcct = 1)
				BEGIN
					-- There is an existing RF account with a Date First
					SET @DateFirst = DATEADD(DAY, @MinPeriod, @DateDel)
					-- Use the same day of the month
					SET @DueDate   = DATEADD(DAY, @DueDay, DATEADD(DAY, -DAY(@DateFirst), @DateFirst))
					
					-- Add a month if this day is earlier than MinPeriod after DateDel
					WHILE @DueDate < @DateFirst
					BEGIN
						SET @DueDate = DATEADD(MONTH, 1, @DueDate)
					END
					-- following fixes 69388 JH 14/03/2008
					SET @DueDate = DATEADD(MONTH, @mthsdeferred, @DueDate)	-- jec 21/12/2007 UAT 153 (part)
					SET @DateFirst = @DueDate
				END
			ELSE
				BEGIN
					--======================Added new block for First Date(If exist any First date for Customer) ===================
					EXECUTE [dbo].[CalculateFirstInstalmentDate] 
					   @acctno
					  ,@DateFirstInstallment OUTPUT																		
					SET @DateFirst = DATEADD(Month, @mthsdeferred, @DateFirstInstallment)
					--======================END new block for First Date===================
				END		
        END
        ELSE
        BEGIN

			-- Check if AbilitySetFirstPaymentDate setting is applied in CountryMaintenance for RF account
			-- OR if RF account account is type of cashloan
			IF (@PreffirstInstallmentDay = 0 OR @IsCashLoanAcct = 1)   
				BEGIN
					-- No existing RF DateFirst so DateFirst is one month after delivery
					SET @DateFirst = DATEADD(MONTH, @mthstoadd, @DateDel)
				END
			ELSE
				BEGIN
					--======================Added new block for First Date(Not exist any First date for Customer) ===================
					EXECUTE [dbo].[CalculateFirstInstalmentDate] 
					   @acctno
					  ,@DateFirstInstallment OUTPUT
					SET @DateFirst = DATEADD(Month, @mthsdeferred, @DateFirstInstallment)
					--======================END new block for First Date===================
				END
        END

    END    -- End of @type = 'R'

    IF @fixeddatefirst =2 AND @ddenabled !=1  -- use due date on instalplan table to calculate due days...
    BEGIN
   	  SELECT @dueday = dueday 
	  FROM instalplan 
	  WHERE acctno = @acctno
 		SELECT @minperiod = minperiod 
		FROM country --should use min period not deldays!
        SET @DateFirst = DATEADD(DAY, @MinPeriod, @DateDel)
            -- Use the same day of the month
        SET @DueDate   = DATEADD(DAY, @DueDay, DATEADD(DAY, -DAY(@DateFirst), @DateFirst))
            
           -- Add a month if this day is earlier than MinPeriod after DateDel
        WHILE @DueDate < @DateFirst
        BEGIN
             SET @DueDate = DATEADD(MONTH, 1, @DueDate)
        END
		  -- but allow for deferred fixed date firsts.
		  SET @datefirst = DATEADD(MONTH, @mthsdeferred, @DueDate) 
    END    

    ELSE IF @DDEnabled = 1 
    BEGIN
        -- Direct Debit (Giro) processing 
        -- DSR 29/06/01 Check whether a Giro due date should be used instead

        SELECT @DueDay  = day.DueDay,
               @EndDate = man.EndDate
        FROM   DDMandate man
		INNER JOIN Agreement agr ON agr.AcctNo = man.AcctNo
		INNER JOIN DDDueDay day ON  day.DueDayId = man.DueDayId
        WHERE  man.AcctNo = @AcctNo
        AND    man.Status = 'C'
        AND    man.ApprovalDate IS NOT NULL
        AND    man.ApprovalDate > CONVERT(SMALLDATETIME,'01-01-1900',105)
        --AND    agr.AcctNo = man.AcctNo
        AND    agr.DateDel IS NOT NULL
        AND    agr.DateDel > CONVERT(SMALLDATETIME,'01-01-1900',105)
        --AND    day.DueDayId = man.DueDayId
		  SET @rowcount = @@rowcount 
        IF @ROWCOUNT > 0
        BEGIN
            -- There is an approved Giro Mandate 
            -- Align DateFirst with the mandate due day 

            SET @DateFirst = DATEADD(DAY, @MinPeriod, @DateDel)
            -- Use the same day of the month
            SET @DueDate   = DATEADD(DAY, @DueDay, DATEADD(DAY, -DAY(@DateFirst), @DateFirst))
            
            -- Add a month if this day is earlier than MinPeriod after DateDel
            WHILE @DueDate < @DateFirst
            BEGIN
                SET @DueDate = DATEADD(MONTH, 1, @DueDate)
            END

            -- Make sure this cannot be set after a mandate End Date
            IF @DueDate >= @EndDate AND @EndDate > CONVERT(SMALLDATETIME,'01-01-1900',105) AND @DDenabled =1
            BEGIN
                -- Cannot use the mandate so DateFirst is one month after delivery
                SET @DateFirst = DATEADD(MONTH, @mthstoadd, @DateDel)
                --print 'test 2'
            END
            ELSE
            BEGIN
                -- DateFirst is the first Due Date - but check for deferred instalments
                SET @DateFirst = DATEADD(MONTH,@mthsdeferred, @DueDate)
            END
        END
        ELSE             -- Default DateFirst to X months after delivery 
        BEGIN
        --68504 KEF added check for RF as RF sub agreements were not having their due date brought into line with existing RF accounts
            IF @type != 'R'
            BEGIN            
                SET @DateFirst = DATEADD(MONTH, @mthstoadd, @DateDel)
            END
            --Print 'test 3'
        END
    END
   ELSE
    IF @type != 'R'
    BEGIN
        -- Default DateFirst to one month after delivery 
        --print 'test 4'
        SET @DateFirst = DATEADD(MONTH, @mthstoadd, @DateDel)

    END

   --procedure for updating variable date of first instalments

    DECLARE @counter SMALLINT,@datenext DATETIME
    SET @datenext =@datefirst

    SET @counter = 1

    WHILE 1 = 1 
    BEGIN
       UPDATE instalmentvariable SET dateFROM = @datenext
       WHERE acctno = @acctno 
       AND instalorder =@counter

       IF @@ROWCOUNT = 0 
          BREAK

       SELECT  @datenext= DATEADD(MONTH,1,dateto)
       FROM instalmentvariable 
       WHERE acctno = @acctno 
       AND instalorder =@counter 
        
       SET @counter = @counter + 1
    END

        
END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
