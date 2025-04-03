SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO
-- PC 04/Dec/2006	//CR 866 Adding additional fields used in scoring
-- PC 28/Nov/2006	//CR 843 Adding details from a second credit bureau
-- AA 13/05/04		applicant 2 income and additional income now included in income
-- RD 23/12/05 67779
-- IP 03/08/11 RI - System Integration changes
-- IP 30/10/12		#11535 - Return the sum of agreement totals of all accounts for the customer
--					excluding the most recent credit account.
-- IP 31/10/12		#11536 - Return the sum of outstanding balance of all accounts for the customer
--					excluding the most recent credit account.
-- IP 18/01/13      #12038 - UAT181 - due to accounts for a customer having the same dateprop, 
--					sub-query returned more than one value. Therefore selecting top 1 account.
if exists (select * from dbo.sysobjects
           where id = object_id('[dbo].[DN_GetScoreDetailsByParamSP]')
           and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_GetScoreDetailsByParamSP]
GO


CREATE procedure DN_GetScoreDetailsByParamSP

    -- Parameters

    @AcctNo                             VARCHAR(12),
    @Return                             INT                 OUTPUT,

    @accttype                           VARCHAR(2)          OUTPUT,
    @addresstime                        INT                 OUTPUT,
    @agreement                          DECIMAL(20,5)       OUTPUT,
    @Arrears                            DECIMAL(20,5)       OUTPUT,
    @bankaccttype                       VARCHAR (2)         OUTPUT,
    @bankBankname                       VARCHAR(20)         OUTPUT,
    @Bankruptcies                       INT                 OUTPUT,
    @Bankruptcies12Months               INT                 OUTPUT,
    @Bankruptcies24months               INT                 OUTPUT,
    @BankruptciesAvgValue               DECIMAL(20,5)       OUTPUT,
    @Bankruptciestimesincelast          INT                 OUTPUT,
    @BankruptciesTotalValue             DECIMAL(20,5)       OUTPUT,
    @CurHiEver                          CHAR(1)            OUTPUT,
    @CurHiNow                           CHAR(1)            OUTPUT,
    @CurNumAcc                          DECIMAL(20,5)       OUTPUT,
    @CurRecent                          CHAR(1)            OUTPUT,
    @custaddresspocode                  VARCHAR (6)         OUTPUT,
    @custaddressResstatus               VARCHAR(2)          OUTPUT,
    @customerAge                        INT                 OUTPUT,
    @customerEthnicity                  CHAR(1)            OUTPUT,
    @customerTitle                      VARCHAR(25)         OUTPUT,
    @custtelarea                        VARCHAR(6)          OUTPUT,
    @custtelTelno                       VARCHAR(20)         OUTPUT,
    @DateLastAcceptedGRMaxCurrent       CHAR(1)             OUTPUT,
    @DateLastAcceptedGRMaxSettled       CHAR(1)             OUTPUT,
    @DepPercent                         DECIMAL(20,5)       OUTPUT,
    @employmentEmpmtstatus              CHAR(1)            OUTPUT,
    @employmentPayfreq                  CHAR(1)            OUTPUT,
    @EmploymentTime                     INT                 OUTPUT,
    @expenses                           DECIMAL (20,5)      OUTPUT,
    @gender                             CHAR(1)            OUTPUT,
    @hasdelivery                        INT                 OUTPUT,
    @hashomephone                       CHAR(1)            OUTPUT,
    @hasworkphone                       CHAR(1)            OUTPUT,
    @income                             DECIMAL(20,5)       OUTPUT,
    @IncomePercent                      DECIMAL(20,5)       OUTPUT,
    @instalno                           INTEGER             OUTPUT,
    @InstalPercent                      DECIMAL(20,5)       OUTPUT,
    @InstalPercentAllAccounts           DECIMAL(20,5)       OUTPUT,
    @itemcount                          INT                 OUTPUT,
    @jemploymentEmpmtstatus             VARCHAR (3)         OUTPUT,
    @jemploymentworktype                VARCHAR(2)          OUTPUT,
    @Lawsuits                           INT                 OUTPUT,
    @Lawsuits12Months                   INT                 OUTPUT,
    @Lawsuits24months                   INT                 OUTPUT,
    @LawsuitsAvgValue                   DECIMAL(20,5)       OUTPUT,
    @LawsuitsTotalValue                 DECIMAL(20,5)       OUTPUT,
    @LawsuitTimeSinceLast               INT                 OUTPUT,
    @LoanAmt                            DECIMAL(20,5)       OUTPUT,
    @mobilephone                        CHAR(1)             OUTPUT,
    @monthssincedelivery                DECIMAL(20,5)       OUTPUT,
    @NumAppsLst90                       DECIMAL(20,5)       OUTPUT,
    @paddresstime                       INT                 OUTPUT,
    @paymentmethod                      VARCHAR(5)         OUTPUT,
    @PreviousEnquiries                  INT                 OUTPUT,
    @PreviousEnquiries12Months          INT                 OUTPUT,
    @PreviousEnquiriesAvgValue          DECIMAL(20,5)       OUTPUT,
    @PreviousEnquiriesAvgValue12Months  DECIMAL(20,5)       OUTPUT,
    @PreviousEnquiriesTotalValue        DECIMAL(20,5)       OUTPUT,
    @privilege                          CHAR(1)            OUTPUT,
    @ProdCat                            INT                 OUTPUT,
    @ProdCode                           VARCHAR(8)          OUTPUT,
    @proposalDependants                 INT                 OUTPUT,
    @proposalMaritalstat                CHAR(1)            OUTPUT,
    @proposalnationality                VARCHAR (2)         OUTPUT,
    @proptype                           CHAR(4)            OUTPUT,
    @RFLimit                            DECIMAL(20,5)       OUTPUT,
    @rindicset                          CHAR(1)            OUTPUT,
    @score                              INT                 OUTPUT,
    @SetHiEver                          CHAR(1)            OUTPUT,
    @SetHiNow                           CHAR(1)            OUTPUT,
    @SetLargest                         CHAR(1)            OUTPUT,
    @SetLargestSize                     CHAR(1)            OUTPUT,
    @SetNumAcc                          INT                 OUTPUT,
    @SetRecent                          CHAR(1)            OUTPUT,
    @termstype                          VARCHAR(2)          OUTPUT,
    @timecurrbank                       INTEGER             OUTPUT,
    @totalliability                     DECIMAL(20,5)       OUTPUT,
    @TotValue                           DECIMAL(20,5)       OUTPUT,
    @worktype                           CHAR (2)           OUTPUT,
    @monthssinceaccepted                DECIMAL(20,5)       OUTPUT,
	@TransportType						TTransportType		OUTPUT, --CR 866    
	@EducationLevel						TEducationLevel		OUTPUT, --CR 866    		
	@DistanceFromStore					SMALLINT			OUTPUT, --CR 866                        
	@Industry							TIndustry			OUTPUT, --CR 866
	@JobTitle							TJobTitle			OUTPUT, --CR 866     
	@Organisation						TOrganisation		OUTPUT,  --CR 866    
	@TelAreaCat							VARCHAR(128)         OUTPUT,
	@agreementTotAccts					DECIMAL(20,5)		OUTPUT, --IP - 30/10/12 - #11535  
	@outstbalTotAccts					DECIMAL(20,5)		OUTPUT	--IP - 30/10/12 - #11536  

AS
    SET NOCOUNT ON
    -- Local variables
    DECLARE @CurWeightAvg               DECIMAL(20,5),
            @custaddressDatein          DATETIME,
            @custid                     VARCHAR(20),
            @DateProp                   SMALLDATETIME,
            @employmentWorktype         VARCHAR(2),
            @monthlygross               DECIMAL(20,5),
            @mthlyrent                  MONEY,
            @num                        INTEGER,
            @otherpmnts                 DECIMAL(20,5),
            @RejLst90                   CHAR(1),
            @SetWeightAvg               DECIMAL(20,5),
            @TotalInstal                DECIMAL(20,5),
            @TotalOutStBal              DECIMAL(20,5),
            @status                     DECIMAL(20,5),

            -- JOINT Applicant
            @jCurNumAcc                 DECIMAL(20,5),
            @jSetNumAcc                 DECIMAL(20,5),
            @jCurRecent                 CHAR(1),
            @jSetRecent                 CHAR(1),
            @jCurHiEver                 CHAR(1),
            @jSetHiEver                 CHAR(1),
            @tempchar                   CHAR(1),
            @tempfloat                  DECIMAL(20,5),
            @jSetLargest                CHAR(1),
            @jSetLargestSize            CHAR(1),
            @jTotalInstal               DECIMAL(20,5),
            @jTotalOutStBal             DECIMAL(20,5),
            @jNumAppsLst90              INT,
            @jRejLst90                  CHAR(1),
            @Jcustid                    VARCHAR(20)

    SET @status = 0
    SET @return = 0

    EXECUTE SP_transactdata
        @acctno =@acctno ,
        @TotValue= @TotValue output, @LoanAmt=@loanamt  output,@ProdCat= @prodcat output,
        @ProdCode=@prodcode output, @CurHiEver=@curhiever  output,
        @CurRecent=@currecent  output,  @sethiever = @SetHiEver output,
        @SetRecent =@setrecent output, @SetLargest=@setlargest output,
        @SetLargestSize=@setlargestsize output,  @TotalInstal=@totalinstal output,
        @TotalOutStBal =@totaloutstbal output, @CurNumAcc=@curnumacc output,
        @SetNumAcc=@setnumacc output,@DepPercent=@deppercent output,
        @InstalPercent =@instalpercent output,     @InstalPercentAllAccounts =@InstalPercentAllAccounts output,
        @jCurHiEver=@jcurhiever output,
        @jCurRecent=@jcurrecent output, @jSetHiEver=@jsethiever output,
        @jSetRecent=@jsetrecent output,@jsetlargest =@jSetLargest  output,
        @jSetLargestSize=@jSetLargestSize output, @jTotalInstal=@jTotalInstal output,
        @jTotalOutStBal=@jTotalOutStBal output, @jCurNumAcc=@jCurNumAcc,
        @jSetNumAcc=@jSetNumAcc output, @NumAppsLst90=@NumAppsLst90 output,
        @RejLst90=@RejLst90 output, @jNumAppsLst90=@jNumAppsLst90 output,
        @jRejLst90=@jRejLst90 output,  @CurHiNow=@CurHiNow output,
        @SetHiNow=@SetHiNow output, @CurWeightAvg=@CurWeightAvg output,
        @SetWeightAvg=@SetWeightAvg output,@custid = @custid output, @noselect= 1,
        @jcustid = @jcustid output

    if @status = 0
    begin
        --LiveWire Call 67970
        --changed to a left join as users can enter time at bank details only
        --without entering bank name and account type

        select  @bankBankname = bank.Bankname,
                @bankaccttype = bankacct.code,
                @timecurrbank = datediff(month,dateopened, getdate())
        from    bankacct
				left join bank on bankacct.bankcode= bank.bankcode
							   and bankacct.bankcode != ''
        where   bankacct.custid = @custid
        and     dateopened=(select Max(dateopened) from bankacct where custid = @custid)

        Set @status =@@error
    end

    if @status = 0
    begin
        select
            @accttype = AT.accttype ,
            @agreement = agreement.agrmttotal - deposit,
            @termstype = A.termstype,
            @paymentmethod = agreement.paymentmethod
        from acct A
        inner join accttype AT on A.accttype = AT.genaccttype
        join agreement on agreement.acctno = A.acctno
        where A.acctno = @acctno

        SET @status = @@error
        SET @totalliability = @TotalOutStBal + @agreement
    end

    if @status = 0
    begin
        select @instalno = instalno
        from instalplan
        where acctno =@acctno
        Set @status =@@error
    end

    if @status = 0
    begin
        select @itemcount=isnull(sum(l.Quantity),0)
        from LINEITEM l, stockitem s
        where   l.acctno = @acctno and
                s.ItemType = 'S' and
                --s.itemno =l.itemno and
                s.ID =l.ItemID and									--IP - 03/08/11 - RI
                s.stocklocn=l.stocklocn
                and iskit = 0
        Set @status =@@error
    end

    if @status = 0
    begin
        select
            @custaddressDatein =datein ,
            @custaddressresstatus=resstatus,
            @custaddresspocode=cuspocode,
            @proptype = proptype,
            @mthlyrent = isnull (mthlyrent,0)
        from custaddress
        where (datemoved is null or datemoved =N'1-Jan-1900')
        and custid=@custid and addtype = 'H'
        Set @status =@@error
     end

     if @status = 0
     begin
         declare @Maxstatus VARCHAR (1),
                 @Maxacctno CHAR(12),
                 @datestatus smallDateTime,
                 @recentacctno CHAR(12),
                 @recentdate smallDateTime

         select @recentdate = Max (DateProp),
                @recentacctno = max(acctno)
         from   proposal
         where  custid = @custid
         and    acctno != @acctno
         and    propresult = 'A'

         set @status = @@error

        if @recentdate is not null
        begin
            select  @Maxstatus = max(statuscode),
                    @maxacctno = acctno
            from status
            where exists (select c.acctno from custacct c , acct a
                          where c.custid = @custid
                          and c.acctno not in (@acctno,@recentacctno)
                          and c.hldorjnt = 'H'
                          and a.acctno = c.acctno
                          and a.currstatus != 'S'
                          and status.acctno = a.acctno)
            and statuscode between '1' and '8'
            group by acctno

            select @datestatus = isnull (Min(datestatchge),'1-Jan-1900')
            from   status
            where  acctno = @maxacctno and statuscode = @Maxstatus

            set @DateLastAcceptedGRMaxCurrent ='N'
            if @recentdate > @datestatus set @DateLastAcceptedGRMaxCurrent ='Y'

            set @DateLastAcceptedGRMaxSettled ='N'
            set @maxacctno = null
            set @Maxstatus = null

            select @Maxstatus = max(statuscode), @maxacctno = acctno
            from status
            where exists
                (select c.acctno from custacct c ,acct a
                 where c.custid = @custid
                 and c.acctno not in (@acctno,@recentacctno)
                 and c.hldorjnt='H'
                 and a.acctno = c.acctno
                 and a.currstatus ='S'
                 and status.acctno =a.acctno)
            and statuscode between '1' and '8'
            group by acctno

            set @status = @@error

            select @datestatus = isnull (Min(datestatchge),'1-Jan-1900')
            from status
            where acctno = @maxacctno
            and statuscode = @Maxstatus

            -- date last accepted should only be set to Y for settled a/c if there is a settled account
            if @recentdate > @datestatus and @datestatus !='1-Jan-1900'
                set @DateLastAcceptedGRMaxSettled='Y'
                
            SET @monthssinceaccepted = datediff(dd, @recentdate, getdate()) / 30.4375
        end
    end


    if @status = 0
    begin
        set @addresstime = datediff(month,@custaddressDatein,getdate()) -
        CASE WHEN datepart(day, @custaddressDatein) > datepart(day, getdate()) THEN 1
             ELSE 0
        END

        select  @hasdelivery = Count(*)
        from    custaddress
        where   custid =@custid
        and     addtype in ('D','D1', 'D2','D3')
        and     (datemoved is null or datemoved =N'1-Jan-1900')

        Set @status = @@error
        if @hasdelivery > 0 set @hasdelivery = 1
    end

    if @status = 0
    begin
        select  @customerTitle= title,
                @customerEthnicity = ethnicity,
                @customerAge = age,
                @gender = sex
        from customer
        where custid = @custid

        Set @status = @@error
    end

    if @status = 0
    begin
        set @custtelTelno =N'' --KEF set to blank in case set above
        set @hasworkphone = 'N'
        select @custtelTelno = telno
        from custtel
        where custid =@custid
        and (datediscon is null or datediscon = '1-jan-1900')
        and tellocn= 'W'

        Set @status =@@error
        set @custtelarea = left(@custtelTelno, 3)
        Set @status =@@error

        if @custtelTelno is not null and @custtelTelno !=N''
            set @hasworkphone =N'Y'
    end

--    set @custtelTelno =NULL
    if @status = 0
    begin
        set @custtelTelno =N'' --KEF set to blank in case set above
        set @hashomephone = 'N'
        select @custtelTelno = telno
        from custtel
        where custid =@custid
        and (datediscon is null or datediscon = '1-jan-1900')
        and tellocn= 'H'

        Set @status =@@error
        set @custtelarea = left(@custtelTelno, 3)
        Set @status =@@error
        if @custtelTelno is not null and @custtelTelno !=N''
            set @hashomephone =N'Y'
    end

    if @status = 0
    begin
        set @custtelTelno =N'' --KEF set to blank in case set above
        set @mobilephone =N'N'
        select @custtelTelno = telno
        from custtel
        where custid =@custid
        and (datediscon is null or datediscon = '1-jan-1900')
        and tellocn= 'M'
        Set @status =@@error
        Set @status =@@error
        if @custtelTelno is not null and @custtelTelno !=N''
         set @mobilephone ='Y'
    end

    if @status = 0
    begin
        select  @employmentWorktype =worktype,
                @employmentEmpmtstatus =Empmtstatus,
                @employmentPayfreq =Payfreq,
                @worktype =worktype,
                @EmploymentTime =convert(decimal,datediff(month,dateemployed, getdate()))
        from employment
        where custid =@custid
        and (dateleft is null or dateleft = '1-Jan-1900')

        Set @status =@@error
    end

    if @status = 0 and @employmentEmpmtstatus !=N'' and @employmentEmpmtstatus is not null
    begin
        If  @employmentEmpmtstatus = 'H'
            set @employmentWorktype = 'HP' --(Houseperson)
        else if @employmentEmpmtstatus = 'U'
            set @employmentWorktype = 'UE' --(Unemployed)
        else if @employmentEmpmtstatus = 'D'
            set @employmentWorktype = 'DD' --(Student)
        else if @employmentEmpmtstatus = 'S'
            set @employmentWorktype = 'SS' --(Self-employed)
        else if @employmentEmpmtstatus = 'R'
            set @employmentWorktype = 'RR' --(Retired)
    end

    if @status = 0
    begin
        select @num =Count (*)
        from custcatcode
        where custid =@custid
        and datedeleted is null and code =N'CLAC'

        if @num > 0 set @privilege = 'Y'
        else set @privilege = 'N'

        Set @status =@@error
    end

    if @status = 0
    begin
        select @num = Count (*)
        from custcatcode
        where custid = @custid
        and datedeleted is null
        and code = 'R'

        if @num > 0 set @rindicset = 'Y'
        else set @rindicset  = 'N'

        Set @status =@@error
    end

    if @status = 0
    begin
        select  @jemploymentWorktype =worktype,
                @jemploymentEmpmtstatus =Empmtstatus
                --@employmentPayfreq =Payfreq,
                --@EmploymentTime =convert(decimal,datediff(month,dateemployed, getdate()))/12
        from employment
        where custid = @jcustid
        and (dateleft is null or dateleft = '1-Jan-1900')

        Set @status = @@error
    end

    if @status = 0 and @jemploymentEmpmtstatus !=N'' and @jemploymentEmpmtstatus is not null
    begin
        If  @jemploymentEmpmtstatus = 'H'
            set @jemploymentWorktype = 'HP' --(Houseperson)
        else if @jemploymentEmpmtstatus= 'U'
            set  @jemploymentWorktype = 'UE' --(Unemployed)
        else if @jemploymentEmpmtstatus= 'D'
            set @jemploymentWorktype = 'DD' --(Student)
        else if @jemploymentEmpmtstatus= 'S'
            set @jemploymentWorktype = 'SS' --(Self-employed)
        else if @jemploymentEmpmtstatus= 'R'
            set @jemploymentWorktype = 'RR' --(Retired)
    end

    if @status = 0
    begin
        select  @proposalMaritalstat = Maritalstat,
                @proposalDependants = dependants,
                @proposalnationality = nationality,
                @DateProp = DateProp,
                -- @addresstime = yrscurraddr *12,
                @paddresstime = paddmm + (yrsprevaddr * 12),
                @expenses = isnull(commitments1,0) + isnull(commitments2,0) + isnull(commitments3,0) + isnull(otherpmnts,0),
                @monthlygross = ISNULL(mthlyincome,0) + isnull(AddIncome,0) + isnull (A2AddIncome,0) + isnull (A2MthlyIncome,0),
                @otherpmnts = ISNULL(otherpmnts,0),
				@TransportType = TransportType,			-- CR 866
				@EducationLevel = EducationLevel,		-- CR 866
				@DistanceFromStore = DistanceFromStore, -- CR 866
				@Industry	=	Industry,				-- CR 866
				@JobTitle	=	JobTitle,				-- CR 866 
				@Organisation = Organisation			-- CR 866
        from proposal
        where custid = @custid
        and	acctno = @acctno
        and   dateprop =(select Max(dateprop) from proposal z
                         where  z.custid = @custid
						 and	z.acctno = @acctno)
                         --and not exists (select * from cancellation c where c.acctno =z.acctno)) --IP - 17/02/10 -CR1072 - LW 71911 - General Fixes from 4.3 - Commented out
                         ---- exclude cancelled accounts

        Set @status =@@error
    end

    if @status = 0
    begin
        if @accttype = 'R' /* Find the latest RF proposal for RFCategory */
        begin
            select top 1 @prodcat = p.rfcategory
            from   proposal p, acct a
            where  p.custid = @custid
            and    a.acctno = p.acctno
            and    a.accttype = 'R'
            order by p.dateprop DESC

            Set @status =@@error
        end
    end

    IF @status = 0
    begin
        SELECT @Arrears = SUM(isnull(a.Arrears,0))
        FROM   CustAcct ca, Acct a
        WHERE  ca.CustId = @CustId
        AND    ca.HldOrJnt = 'H'
        AND    a.AcctNo = ca.AcctNo

        Set @status =@@error
    end

    IF @status = 0
    begin
        SET @income = @monthlygross - @expenses - @mthlyrent
        Set @status =@@error
    end

    IF @status = 0
    BEGIN
        SET @IncomePercent = 0.0
        IF @income > 0 AND @monthlygross > 0
        BEGIN
            SET @IncomePercent = @income/@monthlygross * 100
            SET @status =@@error
       END
    END

    IF @status = 0
    BEGIN
		--//CR 843 This now looks at both credit bureaus
		SELECT 
			@LawsuitTimeSinceLast	   = case when isnull(A.lawsuittimesincelast, -1) > isnull(B.lawsuittimesincelast, -1) then  A.lawsuittimesincelast else B.lawsuittimesincelast end , 
			@Bankruptciestimesincelast = case when isnull(A.bankruptciestimesincelast, -1) > isnull(B.bankruptciestimesincelast, -1) then A.bankruptciestimesincelast else B.bankruptciestimesincelast end,
			@Lawsuits				   = case when isnull(A.lawsuits, -1) > isnull(B.lawsuits, -1)  then A.lawsuits else B.lawsuits end,
			@Lawsuits12Months		   = case when isnull(A.lawsuits12months, -1) > isnull(B.lawsuits12months, -1) then A.lawsuits12months else B.lawsuits12months end,
			@Lawsuits24months		   = case when isnull(A.lawsuits24months, -1) > isnull(B.lawsuits24months, -1) then A.lawsuits24months else B.lawsuits24months end,
			@LawsuitsAvgValue		   = case when isnull(A.lawsuitsavgvalue, -1) > isnull(B.lawsuitsavgvalue, -1) then A.lawsuitsavgvalue else B.lawsuitsavgvalue end,
			@LawsuitsTotalValue		   = case when isnull(A.lawsuitstotalvalue, -1) > isnull(B.lawsuitstotalvalue, -1) then A.lawsuitstotalvalue else B.lawsuitstotalvalue end,
			@Bankruptcies			   = case when isnull(A.bankruptcies, -1) > isnull(B.bankruptcies, -1) then A.bankruptcies else B.bankruptcies end,
			@Bankruptcies12Months	   = case when isnull(A.bankruptcies12months, -1) > isnull(B.bankruptcies12months, -1) then A.bankruptcies12months else B.bankruptcies12months end,
			@Bankruptcies24months	   = case when isnull(A.bankruptcies24months, -1) > isnull(B.bankruptcies24months, -1) then A.bankruptcies24months else B.bankruptcies24months end,
			@BankruptciesAvgValue	   = case when isnull(A.bankruptciesavgvalue, -1) > isnull(B.bankruptciesavgvalue, -1) then A.bankruptciesavgvalue else B.bankruptciesavgvalue end,
			@BankruptciesTotalValue	   = case when isnull(A.bankruptciestotalvalue, -1) > isnull(B.bankruptciestotalvalue, -1) then A.bankruptciestotalvalue else B.bankruptciestotalvalue end,
			@PreviousEnquiries		   = case when isnull(A.previousenquiries, -1) > isnull(B.previousenquiries, -1) then A.previousenquiries else B.previousenquiries end ,
			@PreviousEnquiriesTotalValue = case when isnull(A.previousenquiriestotalvalue, -1) > isnull(B.previousenquiriestotalvalue, -1) then A.previousenquiriestotalvalue else B.previousenquiriestotalvalue end ,
			@PreviousEnquiriesAvgValue = case when isnull(A.previousenquiriesavgvalue, -1) > isnull(B.previousenquiriesavgvalue, -1) then A.previousenquiriesavgvalue else B.previousenquiriesavgvalue end ,
			@PreviousEnquiries12Months = case when isnull(A.previousenquiries12months, -1) > isnull(B.previousenquiries12months, -1) then A.previousenquiriesavgvalue else B.previousenquiries12months end ,
			@PreviousEnquiriesAvgValue12Months = case when isnull(A.previousenquiriesavgvalue12months, -1) > isnull(B.previousenquiriesavgvalue12months, -1) then A.previousenquiriesavgvalue12months else B.previousenquiriesavgvalue12months end 
			
		FROM
			(SELECT top 1 * 
			FROM		CreditBureau
			WHERE	custid = @custid 
				AND Isnull(Source, '') IN ('', 'B') -- Records from baycorp
			ORDER BY	scoredate DESC ) A
		FULL OUTER JOIN 
			(SELECT top 1 * 
			FROM 	CreditBureau
			WHERE	custid = @custid 
				AND Isnull(Source, '') = ('D')
			ORDER BY	scoredate DESC ) B		   -- Records from DPGroup
		
		ON A.CustId = B.CustId
		WHERE A.CustId IS NOT NULL 
			OR B.CustID IS NOT NULL

   
		SET @status =@@error
    END

    IF @status = 0        /* find the no. of months since the last delivery */
    BEGIN
        DECLARE @datedel datetime

        SELECT  TOP 1
                @datedel = D.datedel
        FROM    delivery D
        INNER JOIN custacct CA ON D.acctno = CA.acctno
        INNER JOIN lineitem LI ON D.acctno = LI.acctno
        AND     D.agrmtno = LI.agrmtno
        --AND     D.itemno = LI.itemno
        AND     D.ItemID = LI.ItemID																							--IP - 03/08/11 - RI
        AND     D.stocklocn = LI.stocklocn
        --INNER JOIN stockitem s ON s.itemno = d.itemno AND s.stocklocn = d.stocklocn
        INNER JOIN stockitem s ON s.ID = d.ItemID AND s.stocklocn = d.stocklocn													--IP - 03/08/11 - RI
        WHERE  CA.custid = @CustId
        AND    CA.hldorjnt = 'H'
        AND    D.transvalue != 0
        AND    D.delorcoll = 'D'
        AND    S.itemtype = 'S'
        ORDER BY datedel DESC

        IF(@@rowcount > 0)
        BEGIN
            SET @monthssincedelivery = datediff(dd, @datedel, getdate()) / 30.4375
        END
    END

    -- RD 67779 23/12/05 Telno in custtel table with dot is selected as numeric but when selecting as int getting error
    -- hence modified the code below to resolve the issue but think should really need to check to see why
    -- telarea is changed to int when set up as varchar
    IF(ISNUMERIC(@custtelarea) = 0 OR (@custtelarea LIKE '%.%') OR (@custtelarea LIKE '%+%'))
    BEGIN	
	SET @custtelarea = '0'
    END

    SET @return = @@ERROR
    
    IF @status = 0  
    BEGIN
    -- SC 71262 5/06/2009 TelCodeCat 
    SELECT DISTINCT @TelareaCat = codedescript  
	FROM code
	WHERE category = 'TAC'
	AND  code = (SELECT MAX(DialCode) FROM custtel
				 WHERE custid = @custid
				 AND datediscon IS NULL
				 AND tellocn = 'H')
	
	--IP - 30/10/12 - #11535
	IF @status = 0
    BEGIN
        
	SELECT @agreementTotAccts = SUM(isnull(a.agrmttotal,0)), @outstbalTotAccts = SUM(isnull(a.outstbal,0))		-- IP - 31/10/12 - #11536
        FROM acct a INNER JOIN dbo.custacct ca ON a.acctno = ca.acctno
        WHERE ca.hldorjnt = 'H'
        AND ca.custid = @custid
        --AND a.acctno!= (SELECT p.acctno FROM proposal p
        AND a.acctno!= (SELECT TOP 1 p.acctno FROM proposal p							--IP - 18/01/13 - #12038 
							INNER JOIN dbo.custacct ca1 ON p.acctno = ca1.acctno
							WHERE p.custid = ca.custid
							AND ca1.hldorjnt = 'H'
							AND p.dateprop = (SELECT MAX(p1.dateprop)
												FROM proposal p1
													WHERE p1.custid = p.custid))
        Set @status =@@error
    END			 
	
    SET @return = @@ERROR
    END


GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

