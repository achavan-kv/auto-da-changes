-- Get17MonthsDataForScoreCard 

IF OBJECT_ID('dbo.Get17MonthsDataForScoreCard') IS NOT NULL
	DROP PROCEDURE dbo.Get17MonthsDataForScoreCard
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create PROCEDURE [dbo].[Get17MonthsDataForScoreCard]
	-- Add the parameters for the stored procedure here

@CustId varchar(20),
@Period  INT
	
AS
BEGIN
DECLARE @FirstDateOfCurrentMonth date =  DATEADD(month, DATEDIFF(month, 1, getdate()), 0)  --2018-08-01 00:00:00.000
DECLARE @dateStart date =  CAST(EOMONTH(DATEADD(month, - (@Period+1), @FirstDateOfCurrentMonth))AS date)  --'2017-03-31'

DECLARE @dateEnd date = @FirstDateOfCurrentMonth  --'2018-08-01'

Declare @TableVar table
     (EXTRACT_DATE date,
	  CUSTOMER_ID varchar(20),
	  ACCOUNT_NUMBER char(12),
	  DATE_ACCOUNT_OPENED date,
	  OUTSTANDING_BALANCE money,
	  Date_Obs date,
	  BALANCE_ARREARS money,
	  DAYS_ARREARS int,
	  Agreement_Total money
	  -- AGREEMENT_TOTAL1 money
      )

DECLARE @numberOfMonths SMALLINT

    SELECT CustomerID,
           AccountNumber,
           Date_Account_Opened,
           Period_Closing_Date,
           NumInstal,
           Agreement_Total,
           Outstanding_Balance,
           PastDue_Balance,
           Days_in_Arrears,
           Cancelation_Date,
           Instalment,
           Paid_Installments,
           Outstanfing_Installments,
           BranchNo,
           Terms_Type,
           Last_payment_date
    INTO #tempExport
    FROM (
            SELECT ca.custid AS CustomerID,
                   a.acctno AS AccountNumber,
                   CAST(a.dateacctopen AS DATE) AS Date_Account_Opened,
                   CASE 
                       WHEN DATEPART(MONTH, DATEADD(DAY, DATEPART(DAY, i.[datefirst]) - 1, @dateStart)) = DATEPART(MONTH, @dateStart)    --Dueday is not correct for most accounts...
                            THEN DATEADD(DAY, DATEPART(DAY, i.[datefirst]) - 1, @dateStart)
                       ELSE DATEADD(DAY, -1, @dateEnd)
                   END AS Period_Closing_Date,
                   i.instalno AS NumInstal,
                   a.agrmttotal AS Agreement_Total,
                   CAST(0 AS MONEY) AS Outstanding_Balance,
                   CAST(0 AS MONEY) AS PastDue_Balance,
                   0 AS Days_in_Arrears,
                   CAST(NULL AS DATE) AS Cancelation_Date,
                   i.instalamount AS Instalment,
                   CAST(0.00 AS MONEY) AS Paid_Installments,
                   CAST(0.00 AS MONEY) AS Outstanfing_Installments,
                   LEFT(a.acctno, 3) AS BranchNo,
                   a.termstype AS Terms_Type,
                   CAST(NULL AS DATE) AS Last_payment_date,
                   COALESCE(s.statuscode, a.currstatus) AS currentStatus
            FROM custacct ca 
            INNER JOIN acct a
            ON ca.acctno = a.acctno 
			INNER JOIN agreement ag   -- added by nilesh
			ON ag.acctno = a.acctno 
            AND	ca.hldorjnt = 'H'
			AND	ca.custid =@custid
			 --AND a.dateacctopen >= @dateStart
    --         AND a.dateacctopen < @dateEnd
				AND (ag.datedel >= @dateStart or a.datelastpaid>= @dateStart)
				  AND (ag.datedel < @dateEnd or ag.datedel < @dateEnd)
                AND a.accttype NOT IN ('C', 'S') --and a.currStatus != 'S'
            INNER JOIN instalplan i
            ON a.acctno = i.acctno
            LEFT OUTER JOIN [status] s
            ON s.acctno = ca.acctno
                AND s.datestatchge = (SELECT MAX(datestatchge) 
                                      FROM [status] 
                                      WHERE acctno = s.acctno 
                                        AND cast(datestatchge as date) <= @dateStart)
									
         ) AS a
   -- WHERE a.currentStatus != 'S'
		

DECLARE @acctno char(12),
		@CustomerID varchar(20), 
           @Date_Account_Opened date,
           @Period_Closing_Date date,
           @NumInstal smallint,
			@Agreement_Total money,
           @Cancelation_Date date,
           @Instalment money,
           @Paid_Installments MONEY,
          -- @Outstanding_Installments MONEY,
           @BranchNo varchar(3),
           @Terms_Type nvarchar(2),
           @Last_payment_date date
DECLARE results_cursor CURSOR FORWARD_ONLY FOR 
   SELECT  CustomerID, AccountNumber,
           Date_Account_Opened,
           Period_Closing_Date,
           NumInstal,
           Agreement_Total,
           Cancelation_Date,
           Instalment,
           Paid_Installments,
          -- Outstanding_Installments,
           BranchNo,
           Terms_Type,
           Last_payment_date FROM #tempExport
   OPEN results_cursor
    FETCH NEXT FROM results_cursor 
    INTO 
	@CustomerID, 
	@acctno,
           @Date_Account_Opened,
           @Period_Closing_Date,
           @NumInstal,
			@Agreement_Total,
           @Cancelation_Date,
           @Instalment,
           @Paid_Installments,
          -- @Outstanding_Installments,
           @BranchNo,
           @Terms_Type,
           @Last_payment_date
    
	WHILE @@FETCH_STATUS = 0    
	BEGIN    
		
		----------------------------------------------------------------------
		SET @numberOfMonths = DATEDIFF(MONTH, @dateStart, @dateEnd) --+ 1
		set @dateEnd =	EOMONTH	(DATEADD(MONTH, 1, @dateStart))

		WHILE (@numberOfMonths > 0)
		BEGIN
		------------------------EOD code------------------------------------------------------------

		--Arrears procedure as OF EOD
        --======================================================================

        declare @Arrears money = 1.0,
                @datefrom DATETIME = @dateEnd,
                @dueday int,
                @arrearsDate DATETIME = @dateEnd
       
        while (@Arrears > 0 AND @datefrom IS NOT NULL)
        BEGIN 
             --Local variables  
            DECLARE
                @OutStBal       MONEY,
                @dte            DATETIME,     
                @di             integer,   
                @dn             integer,     
                @db             integer,   
                @mi             integer,     
                @yi             integer,   
                @mn             integer,     
                @yn             integer,   
                @nsd            integer,     
                @mon            money,     
                @Paid           money,     
                @d1             DATETIME,      
                @atype          char(1),   
                @ttype          varchar(2),     
                @Deposit        money,   
                @DateFirst      DATETIME,   
                @DateLast       DATETIME,  
                @InstalAmount   money,   
                @DateAgrmt      DATETIME,     
                @AgrmtTotal     money,   
                @DateAcctOpen   DATETIME,   
                @CurrStatus     char(1),
				@isAmortized    int,
				@IsAmortizedOutStandingBal int,
                @IsReadyAssist  bit,
                @InstalPreDel   char(1)

      
            Declare @InstalmentWaived bit
   
                SET NOCOUNT ON  

                set @Arrears       = 0.0
                SET @dte            = '1900-01-01';  
                SET @di             = 0;  
                SET @dn             = 0;  
                SET @db             = 0;  
                SET @mi             = 0;  
                SET @yi             = 0;  
                SET @mn             = 0;  
                SET @yn             = 0;  
                SET @nsd            = 0;  
                SET @mon            = 0;  
                SET @Paid           = 0;  
                SET @d1             = '1900-01-01';  
                SET @OutStBal       = 0;  
                SET @atype          = ' ';  
                SET @ttype          = ' ';  
                SET @Arrears        = 0;  
                SET @Deposit        = 0;  
                SET @DateFirst      = '1900-01-01';  
                SET @DateLast       = '1900-01-01';  
                SET @DateAgrmt      = '1900-01-01';  
                SET @AgrmtTotal     = 0;  
                SET @DateAcctOpen   = '1900-01-01';  
                SET @CurrStatus     = ' ';  
				SET @isAmortized	=0;
				SET @IsAmortizedOutStandingBal = 0;
                SET @dueday         = 0;
  
                SELECT  @atype          = AcctType,   
                        @ttype          = ISNULL(termstype, '00'),  
                        @DateAcctOpen   = ISNULL(DateAcctOpen, ''), 
                        @CurrStatus     = CurrStatus ,
						@isAmortized    = isAmortized,
						@IsAmortizedOutStandingBal = IsAmortizedOutStandingBal  
                FROM    Acct   
                WHERE   @AcctNo = AcctNo   

                SELECT @AgrmtTotal = COALESCE(aa.NewAgreementTotal , a.agrmttotal),
                       @Deposit = COALESCE(aa.Newdeposit, a.deposit)
                FROM agreement a
                LEFT OUTER JOIN agreementAudit aa
                ON a.acctno = aa.acctno
                WHERE a.acctno = @AcctNo
                    AND aa.datechange = (SELECT MAX(datechange) FROM agreementAudit WHERE acctno = aa.acctno and datechange < @datefrom)

				--Cashloan Outstanding balance CR change, if account is amortized and according to new outstanding balance then
				--will execute Spp for new logic
				IF(@isAmortized = 0 )
                SELECT  @OutStBal = ISNULL(SUM(Transvalue), 0)   
                FROM    FinTrans   
                WHERE   AcctNo = @AcctNo   
                AND datetrans < @datefrom
				ELSE
				 SELECT  @OutStBal =dbo.fn_CLAmortizationCalcDailyOutstandingBalance(@AcctNo)

                SELECT  @DateAgrmt      = DateAgrmt
                FROM    Agreement   
                WHERE   AcctNo = @AcctNo;   
    
                SELECT  @DateFirst = datefirst,
                        @DateLast = datelast,
                        @InstalAmount = InstalAmount,  
                        @InstalmentWaived = InstalmentWaived,
                        @dueday = dueday
                FROM    InstalPlan   
                WHERE   AcctNo = @AcctNo;   
    
                SELECT @d1 = @dateFrom
       
                SET @di = DATEPART(day,   @DateFirst);   
                SET @mi = DATEPART(month, @DateFirst);   
                SET @yi = DATEPART(year,  @DateFirst);   

                DECLARE @repos MONEY 

                SELECT @repos = ISNULL(SUM(transvalue ),0) 
                FROM fintrans 
                WHERE acctno = @acctno 
                    AND transtypecode IN ('rep','rdl') 
                   AND datetrans < @datefrom


            -- is current date< date of last instalment  
                IF @d1 < @DateLast OR @DateLast = ''  
                BEGIN  
            -- @?n are from current date  
                    SET @dn = DATEPART(day,   @datefrom);   
                    SET @mn = DATEPART(month, @datefrom);   
                    SET @yn = DATEPART(year,  @datefrom);   
  
            -- if day now is after datefirst day set date before (@db) to 1  
                    --IF @dn - @di > 0  
                    --BEGIN  
                    --   SET @db = 1;   
                    --END  
                    --ELSE  
                    --BEGIN  
                    --    SET @db = 0;   
                    --END  
                END  
                 ELSE -- all due so arrears equals outstanding balance...   
                BEGIN   
                     SET @Arrears = case when ISNULL(@OutStBal, 0) <= 0 then 0 else ISNULL(@OutStBal, 0) -@repos end; 
                END
  
            -- Calc number of months since datefirst to get number of instalments due  
            --  year (now)- year (datefirst) + month (now) - month (datefirst) + 1 (if day (datefirst) before day (now))   
                SET @nsd = 12*(@yn-@yi)+@mn-@mi+@db; 
				
				
  
             IF @DateFirst = ''  
                BEGIN  
                    SET @nsd = 0;   
                END  

	           IF EXISTS(select * from ReadyAssistDetails
				            where acctno = @AcctNo
				            and (status is null or status = 'Active'))
	            BEGIN
		
		            SET @IsReadyAssist = 1
	            END
	            ELSE
	            BEGIN
		            SET @IsReadyAssist = 0
	            END
  
                SELECT  @InstalPreDel = ISNULL(InstalPreDel,'N')   
                FROM    termstype   
                WHERE   termstype = @ttype;   
  
               IF @InstalPreDel = 'Y'  
              and @InstalmentWaived=0  --Instalment not waived  
              and @IsReadyAssist = 0   --and not a Ready Assist
                BEGIN  
                    IF CAST(@Deposit AS MONEY) = 0  
                    BEGIN  
                        SET @Deposit = @InstalAmount;   
                    END  
                END  
  
                IF @nsd < 1  
                BEGIN  
                    SET @nsd = 0;   
                END  
      
                IF @OutStBal - @repos <= 0 OR @OutStBal IS NULL  
                BEGIN  
                    SET @nsd = 0;   
                END  
  
                SET @mon = @nsd * ISNULL(@InstalAmount, 0)  
                declare @balancedue money  
				
                -- for variable instalments store differently  
                    if exists (select * from instalmentvariable where acctno = @acctno)  
                begin     
                   --' instalments for previous variable '   
                   select @balancedue = isnull(sum(instalmentnumber * instalment),0) from instalmentvariable  
                   where dateto <@datefrom and datefrom >'1-jan-1910' and acctno =@acctno  
  
                   --'  instalments for current variable '   
                   select @balancedue =@balancedue + isnull(datediff(month,datefrom,@datefrom) * instalment,0) from instalmentvariable  
                   where dateto >@datefrom  and datefrom >'1-jan-1910' and datefrom <@datefrom and acctno =@acctno  
  
               set @mon = @balancedue  
                end  
  
                SET @mon = @mon + ISNULL(@Deposit, 0);   
  
                IF  @atype = 'm'   
                BEGIN  
                    SET @dte = (@DateAgrmt);   
                END  
                ELSE  
                BEGIN  
                    SET @dte = '';   
                END  
 
                SELECT  @Paid = ISNULL(SUM(Transvalue), 0)   
                FROM    FinTrans   
                WHERE   AcctNo = @AcctNo   
                AND     TransTypeCode NOT IN  
                        ('del',   
                        'grt',   
                        'rep',   
                        'add',  
                        'RFN',
                        'CLD',  --Loan Disbursement    
                        'rpo',   
                        'rdl')   
                AND DateTrans < @datefrom;    

                 IF @Paid IS NULL  
                BEGIN  
                    SET @Paid = 0;   
                END  
                --@paid is -ve and @deposit is +ve
                IF  abs(@Paid) >= @Deposit AND @CurrStatus = 'U'  -- was @Paid > @Deposit AND @CurrStatus = 'U'  
                BEGIN  
                    SET @CurrStatus = '1';   
                END  
	             IF @Paid IS NULL  
                BEGIN  
                    SET @Paid = 0;   
                END 
               SET @Arrears = ISNULL(@mon,0) + ISNULL(@Paid,0);   
  

              if @instalAmount = 0 OR @agrmttotal = 0 OR @OutStBal = 0
                set @arrears = 0

                IF(@Arrears <= 0)
              BEGIN 
                
                --This is the same magic we use to come up with the date next due in the arrears procedure
                Declare	@year SMALLINT,
                        @month SMALLINT
        
                select @arrearsDate = [datefirst] from instalplan where acctno = @acctno
    
               IF DATEPART(DAY,@arrearsDate) >= DATEPART(DAY,@datefrom) -- set the date to this month
		                SET @arrearsDate = CASE WHEN ISDATE ( CAST(DATEPART(yy,@datefrom) * 10000 + 
		                                                   DATEPART(m,@datefrom) * 100 +  
		                                                   DATEPART(dd, @arrearsDate) AS CHAR(8))
								                     ) = 1 --check ISDATE valid if 31-dec but not 31-feb
					                  THEN CAST( CAST( (DATEPART(yy,@datefrom) * 10000 + 
					                                    DATEPART(m,@datefrom) * 100 + 
					                                    DATEPART(d, @arrearsDate)) AS CHAR(8)) AS DATETIME)
					                  ELSE CAST( CAST( (DATEPART(yy,@datefrom) * 10000 + 
					                                    DATEPART(m,@datefrom) * 100 + 
					                                    01) AS CHAR(8)) AS DATETIME) 
					                  END
		
	
	
               IF DATEPART(DAY,@arrearsDate) < DATEPART(DAY,@datefrom)  -- date next due will be next month
                BEGIN
	
	                SET @year = DATEPART(yy,@datefrom)
	                SET @month = DATEPART(m,@datefrom)
	   
	                IF @month= 12 
	    
	                SET @year = @year + 1 -- due date going to be next year 70427
	
	
			
	                SET @arrearsDate = CASE WHEN ISDATE(CAST( (@year * 10000 + 
		                                                DATEPART(m,DATEADD(m,1,@datefrom)) * 100 + 
		                                                DATEPART(d, @arrearsDate)) AS CHAR(8))) = 1 --check ISDATE valid if 31-dec but not 31-feb
					                THEN CAST( CAST( (@year * 10000 + 
					                    DATEPART(m,DATEADD(m,1,@datefrom)) * 100 + 
					                    datepart(day, @arrearsDate)) AS CHAR(8)) AS DATETIME)
					                ELSE CAST( CAST( (@year * 10000 + 
					                    DATEPART(m,DATEADD(m,2,@datefrom)) * 100 + 
					                    01) AS CHAR(8)) AS DATETIME) 
					                END 
						
                END 

              END

              --Set the outstanding balance but only in the first itteration
			 /* IF @datefrom = @dateEnd
			  BEGIN
				  UPDATE tempExport_sample
				  SET Outstanding_Balance = @OutStBal
				  WHERE AccountNumber = @acctno
              END*/
			  
			 
              --if the customer is in arrears, then calculate for the previous payment date
             SELECT @datefrom = MAX(datetrans)
                FROM fintrans 
                WHERE ACCTNO = @acctno
                   AND transtypecode IN ('PAY','XFR','SCX','DDN', 'SCT') 
                   AND transvalue < 0 
                   AND datetrans < @datefrom
				  
				   /* SET  @datefrom = @dateEnd */
              
        END 
		--Print 'End of Inner while'


        --Update the days in arrears
        /*UPDATE tempExport_sample
        SET Days_in_Arrears = DATEDIFF(DAY, @arrearsDate, @dateEnd)
        WHERE AccountNumber = @acctno */

        --Update PastDue_Balance - Arrears
        /*UPDATE tempExport_sample
        SET PastDue_Balance = @Arrears
        WHERE AccountNumber = @acctno*/

        --Update Last_payment_date
        /*UPDATE tempExport_sample
        SET Last_payment_date = (SELECT MAX(datetrans) 
                                 FROM fintrans 
                                 WHERE acctno = @acctno 
                                    AND transtypecode IN ('PAY', 'XFR', 'SCX', 'DDN', 'SCT')
                                    AND transvalue < 0
                                    AND datetrans < @dateEnd)
        WHERE AccountNumber = @acctno */
		set @Last_payment_date = (SELECT MAX(datetrans) 
                                 FROM fintrans 
                                 WHERE acctno = @acctno 
                                    AND transtypecode IN ('PAY', 'XFR', 'SCX', 'DDN', 'SCT')
                                    AND transvalue < 0
                                    AND datetrans < @dateEnd);
        
        --Update number of installments paid
   --     IF @Deposit = @Paid
   --     BEGIN
   --        /* UPDATE tempExport_sample
   --         SET Paid_Installments = CAST(0 AS MONEY),
   --             Outstanfing_Installments = NumInstal
   --         WHERE AccountNumber = @acctno */
			--set @Paid_Installments=CAST(0 AS MONEY)
			--set  @Outstanding_Installments = @NumInstal
   --     END
   --     ELSE
   --     BEGIN
   --         /* UPDATE tempExport_sample
   --         SET Paid_Installments = CAST(ROUND((ABS(@Paid) - @Deposit) / Instalment, 2) AS MONEY),
   --             Outstanfing_Installments = NumInstal - CAST(ROUND((ABS(@Paid) - @Deposit) / Instalment, 2) AS MONEY)
   --         WHERE AccountNumber = @acctno
   --             AND Instalment != 0*/
			--	Print ('inside @Instalment != 0');
			--	--if(@Instalment != 0)
			--	--Begin
			--	--set @Instalment =CAST(ROUND((ABS(@Paid) - @Deposit) / @Instalment, 2) AS MONEY)
			--	--set  @Outstanding_Installments =@NumInstal - CAST(ROUND((ABS(@Paid) - @Deposit) / @Instalment, 2) AS MONEY) 
			--	--end
   --     END

		-------------------------EOD code------------------------------------------------------------
		
		if (@dateEnd < @FirstDateOfCurrentMonth)	
		begin	
		--INSERT INTO #TempTable  
		INSERT INTO @TableVar
		SELECT cast(@dateEnd as date) , @CustomerID, @acctno, cast(@Date_Account_Opened as date),@OutStBal,cast(@FirstDateOfCurrentMonth as date),@Arrears,DATEDIFF(DAY, @arrearsDate, @dateEnd),@Agreement_Total--,@agrmttotal
    
		 end
		-------------------------------------------------------------------------------------------		

		SET @dateStart = DATEADD(MONTH, 1, @dateStart)
		SET @dateEnd = EOMONTH	(DATEADD(MONTH, 1, @dateEnd))
		
		SET @numberOfMonths -= 1

		END---End of main while 
		SET @dateStart  = CAST(EOMONTH(DATEADD(month, - (@Period+1), GETDATE()))AS date)  --'2014-01-01'
		SET @dateEnd  = getdate()  --'2018-01-01'

     ----------------------------------------------------------------- 
    FETCH NEXT FROM results_cursor     
	INTO 
	@CustomerID, 
	@acctno,
           @Date_Account_Opened,
           @Period_Closing_Date,
           @NumInstal,
			@Agreement_Total,
           @Cancelation_Date,
           @Instalment,
           @Paid_Installments,
         --  @Outstanding_Installments,
           @BranchNo,
           @Terms_Type,
           @Last_payment_date  
   
	END  -- End of while   
 CLOSE results_cursor;
 DEALLOCATE results_cursor;
 SELECT * FROM @TableVar
END -- Procedure end