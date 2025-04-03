SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
if exists (select * FROM dbo.sysobjects WHERE id = object_id('[dbo].[DbArrearsCalc]') AND OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DbArrearsCalc]
GO

CREATE PROCEDURE DbArrearsCalc  
  
--------------------------------------------------------------------------------  
--  
-- Project      : eCoSACS r 2002 Strategic Thought Ltd.  
-- File Name    : DbArrearsCalc.sql  
-- File Type    : MSSQL Server Stored Procedure Script  
-- Title        : Calculate Account Arrears (or Advance)  
--  
-- Also may UPDATE Acct.OutStBal  
-- Also calls DBAgrDate which can UPDATE Agreement.DateDel,  
-- Instalplan.DateFirst AND Instalplan.DateLast.  
-- Can also affect Agreement.AgrmtTotal, Acct.AgrmtTotal  
-- for Cash Accounts.  
--  
-- Change Control  
-- --------------  
-- Date      By  Description  
-- ----      --  -----------  
-- 15/02/01  AA  Problem with instalpredel coming up with incorrect Arrears  
-- 12/09/01  AA  Problem with procedure bombing out in Trinidad FR/heat 64561  
-- 24/09/01  AA  SETting DateLast if Null  
-- 23/07/02  DSR CR252 Option Ready Finance - return arrears value  
-- 14/11/02  DSR Second "IF SUBSTRING(@AcctNo, 4, 1) = '4'" code block removed  
--               as both redundant AND in error. (See comment below)  
-- 22/11/02  DSR Add @Return parameter for use in .NET  
-- AA 10/05/04 returning immediately account balance zero AND SETtled.  
-- 13/05/04  DSR But make sure null return value is not returned  
-- AA 19/05/04 Jamaica changes for account type  
-- AA 31/05/04 change to not calculate arrears for special accounts  
-- AA 19/01/04 63890 datefirst will be actual datefirst no - mths deferred rubbish  
-- AA 16/08/05 arrears calculation taking into account variable instalments  
-- AA 03/11/06 68585 Unable to process payment on an account due to divide by zero.  
-- jec 21/12/07 Added Comments   
-- IP  21/01/11 #2926 - Outstanding Balance on Acct table was not being updated. Removed code previously put in  
-- 01/03/11 jec CR1090 Instalment Waiver  
-- 12/10/11  jec CR1232 #3291 new disbursement screen - include CLD transtypecode
-- 16/02/12 jec  Correct Arrears calc for storecard - hopefully?  
-- 20/03/12 ip   #9805 - Arrears for Store Card was not being calculated correctly.
-- 02/04/12 ip   #9857 - Arrears was incorrect for an account that had past it's datelast. If the outstanding balance is 0, the arrears should reflect the same.
-- 08/07/12 ip   #19284 - CR15594 - Arrears set incorrectly for Ready Assist account - ignore Instalment Prior Delivery on Terms Type.
--------------------------------------------------------------------------------  
  
  
    -- Parameters  
    @AcctNo         char(12)    = ' ',   
    @CountPcent     float       = 0.0,   
    @NoDates        smallint    = 0,  
    @Arrears        money       = 0.0   OUTPUT,  
    @Noupdates       BIT = 0 ,   
    @datefrom  DATETIME = '1-jan-1900',  
    @OutStBal       MONEY = 0 OUTPUT,  
    @Return         INTEGER     = 0     OUTPUT  
      
AS  
 IF @datefrom = '1-jan-1900'  
  SET @datefrom = GETDATE()  
  
    -- Local variables  
DECLARE     @AgrmtNo        int,     @i              integer,   
    @dte            DATETIME,     @di             integer,   
    @dn             integer,     @db             integer,   
    @mi             integer,     @yi             integer,   
    @mn             integer,     @yn             integer,   
    @nsd            integer,     @tot            integer,   
    @mon            money,     @Owed           money,   
    @Paid           money,     @d1             DATETIME,   
    @d2             DATETIME,     @bno            smallint,   
         @atype          char(1),   
    @ttype          varchar(2),     @Deposit        money,   
    @AccountNo      varchar(12),    @DateFirst      DATETIME,   
    @DateLast       DATETIME,     @InstalNo       integer,   
    @AS400Bal       money,     @InstalAmount   money,   
    @DateAgrmt      DATETIME,     @AgrmtTotal     money,   
    @Transvalue     money,     @DateAcctOpen   DATETIME,   
    @dPaid          DATETIME,     @InstalPreDel   char(1),   
    @CurrStatus     char(1),     @DateDel        DATETIME,   
    @DateNextDue    DATETIME,    @dnd            DATETIME,   
    @state          integer,    @zero           tinyint,  
   @mthsdeferred SMALLINT,  @deltot   MONEY,
   @today DATETIME,
   @IsReadyAssist BIT,		--#19284 - CR15594
   @isAmortized    int,
   @IsAmortizedOutStandingBal int
    
Declare @InstalmentWaived bit  --CR1090  
   
    SET NOCOUNT ON  
    SET @AgrmtNo        = 0;  
    SET @i              = 0;  
    SET @dte            = '1900-01-01';  
    SET @di             = 0;  
    SET @dn             = 0;  
    SET @db             = 0;  
    SET @mi             = 0;  
    SET @yi             = 0;  
    SET @mn      = 0;  
    SET @yn             = 0;  
    SET @nsd            = 0;  
    SET @tot            = 0;  
    SET @mon            = 0;  
    SET @Owed           = 0;  
    SET @Paid           = 0;  
    SET @d1             = '1900-01-01';  
    SET @d2             = '1900-01-01';  
    SET @bno            = 0;  
    SET @OutStBal       = 0;  
    SET @atype          = ' ';  
    SET @ttype          = ' ';  
    SET @Arrears        = 0;  
    SET @Deposit        = 0;  
    SET @DateFirst      = '1900-01-01';  
    SET @DateLast       = '1900-01-01';  
    SET @InstalNo       = 0;  
    SET @AS400Bal       = 0;  
    SET @DateAgrmt      = '1900-01-01';  
    SET @AgrmtTotal     = 0;  
    SET @Transvalue     = 0;  
    SET @DateAcctOpen   = '1900-01-01';  
    SET @dPaid          = '1900-01-01';  
    SET @CurrStatus     = ' ';  
    SET @DateDel        = '1900-01-01';  
    SET @DateNextDue    = '1900-01-01';  
    SET @dnd            = '1900-01-01';  
    SET @state = 0;  
    SET @zero           = 0;  
	SET @today =  dbo.StripTime(@datefrom)
	SET @isAmortized	=0;
	SET @IsAmortizedOutStandingBal = 0;
  
    SELECT  @bno = branchno    
    FROM    server;   
  
    SELECT  @atype          = AcctType,   
            @ttype          = ISNULL(termstype, '00'),  
            @DateAcctOpen   = ISNULL(DateAcctOpen, ''),  
            @AS400Bal       = ISNULL(AS400Bal, 0),  
            @dPaid          = ISNULL(DateLastPaid, ''),  
            @AgrmtTotal     = AgrmtTotal,   
            @CurrStatus     = CurrStatus  ,
			@isAmortized    = isAmortized,
			@IsAmortizedOutStandingBal = IsAmortizedOutStandingBal 
    FROM    Acct   
    WHERE   @AcctNo = AcctNo   
    SET @return = 0  
    if @acctno like '___5%' -- special accounts don't have arrears  
        return 0  
    SET @mthsdeferred = 0  
     
     
    select @mthsdeferred =isnull (mthsdeferred,0) FROM accttype WHERE accttype =@atype  
     
     
    IF(@isAmortized = 0 )
    SELECT  @OutStBal = ISNULL(SUM(Transvalue), 0)   
    FROM    FinTrans   
    WHERE   AcctNo = @AcctNo   
    AND datetrans <=dateadd(second,5,@datefrom) --IP - 21/01/11 - #2926 - outstbal on Acct table not being updated - removed code  
	ELSE
	SELECT  @OutStBal =dbo.fn_CLAmortizationCalcDailyOutstandingBalance(@AcctNo)
     
   /*  
   calculate for store card accounts  
   */   
  
 if @acctno like    '___9%'  
 BEGIN  
    
   select @deltot = sum(transvalue)  
   from fintrans  
   where acctno = @acctno  
   and transtypecode in ('SCT')  
   and datetrans <=dateadd(second,5,@datefrom)  
  
    /*  
    calculate arrears for store card accounts  
    */  
  
     --select @arrears =  isnull(CASE WHEN datedue> GETDATE()  
     -- THEN prevminpay+payments					-- jec 16/02/12
     -- ELSE currminpay+payments  
     -- END , 0)  
       select @arrears =  isnull(CASE WHEN datedue > GETDATE()
		THEN isnull(outstminpay, 0)	+ payments						--IP - 20/03/12 - #9805
		ELSE currminpay + payments
		END , 0)
  From vw_storecard_arrears    
  --WHERE prevminpay > 0  
  -- and acctno=@acctno
  WHERE acctno=@acctno			-- jec 16/02/12 
  
     
   if @Arrears < 0  
   set @Arrears = 0  
  
    
    /*  
     update acct for store card accounts  
    */  
  
  update acct  
  set arrears = @arrears  
  where acctno = @acctno  
  
     
    /*  
    update arrears daily for store card accounts  
    */  
  
 
  
   UPDATE ArrearsDaily  
   SET dateto = dbo.StripTimeMinusSecond(@today) -- yesterday  
   WHERE acctno = @acctno AND dateto is null AND    
   arrearsdaily.arrears >0 AND     
   exists (select * FROM  ArrearsDaily d        
   WHERE d.acctno =@acctno AND d.arrears !=@arrears AND d.dateto is null)  
            
       
  
     
    /*  
    insert into arrears daily for store card accounts  
    */  
  
   insert into ArrearsDaily ( acctno, arrears,dateFROM,dateto)     
   select @acctno,@arrears,    @today   , NULL    
   WHERE not exists (select * FROM  ArrearsDaily d        
   WHERE d.acctno =@acctno AND d.arrears =@arrears AND d.dateto is null) AND @arrears > 0  
     
     
     
     
  return @return  
 END  
  
  
  
 select  @deltot = sum(transvalue)  
    from  fintrans   
    where  transtypecode in ('add',   
        'del',   
        'grt','CLD')  --jec CR1232 #3291  
    and acctno = @acctno;   
  
 --IP 19/12/2007 - UAT(188)  
 --Cash accounts that had a full payment prior to delivery would not have their 'Datedel' set once delivered.  
 --Therefore if account is 'Settled' and 'Outstanding Balance' '0' then set 'Datedel' for Cash account.  
   if @currstatus ='S' AND  @outstbal= 0  
 BEGIN  
   
  
  if @deltot >= @AgrmtTotal  
  BEGIN  
   update acct  
   set outstbal = 0, arrears = 0, paidpcent = 100  
   where acctno = @AcctNo  
      
     
   update arrearsdaily  
   set dateto = @datefrom  
   where dateto is null  
    and acctno = @AcctNo  
  END  
   
  
  IF SUBSTRING(@AcctNo, 4, 1) = N'4' AND @Noupdates = 0   
   BEGIN  
    DECLARE @maxdatetrans DATETIME  
    SET @maxdatetrans = (SELECT MAX(ISNULL(datetrans,'1900-01-01')) FROM fintrans f WHERE f.acctno = @acctno AND transtypecode IN ('DEL')   
    AND datetrans <=@datefrom )  
  
    UPDATE agreement  
    SET datedel = @maxdatetrans  
    WHERE acctno = @acctno  
  
    SET @return = 0  
    return 0  
   END  
  ELSE  
   begin  
    SET @return = 0  
    return 0  
   end  
 END  
    
  
    SELECT  @Deposit        = Deposit,  
            @DateDel        = ISNULL(DateDel, ''),  
            @DateAgrmt      = DateAgrmt,   
            @DateNextDue    = DateNextDue,   
            @AgrmtNo        = AgrmtNo   
    FROM    Agreement   
    WHERE   AcctNo = @AcctNo;   
  
  
    -- cash accounts or agreement total 0  
    IF  @AgrmtTotal = 0 OR SUBSTRING(@AcctNo, 4, 1) = '4'   
    BEGIN    
        IF SUBSTRING(@AcctNo, 4, 1) = '4'  
        BEGIN     
            IF @AS400Bal != @AgrmtTotal OR @AgrmtTotal = 0  
            BEGIN   
                EXECUTE DBAgrDate  
                    @AcctNo         = @AcctNo,   
                    @AgrmtTotal     = @AgrmtTotal,   
                    @CountPcent     = @CountPcent,   
                    @AgrmtNo        = @AgrmtNo,   
                    @origbr         = @bno;   
            END    
            ELSE  
            BEGIN    
                IF @AS400Bal > 0   
                BEGIN   
                    SELECT  @Paid = ISNULL(SUM(Transvalue), 0)   
                    FROM    FinTrans    
                    WHERE   AcctNo = @AcctNo AND datetrans <=@datefrom   
                    AND     TransTypeCode NOT IN  
                            ('del',   
                            'grt',   
                            'rep',   
                            'add',  
                            'RFN',   -- CR976   
                            'CLD',  --jec CR1232 #3291 Loan Disbursement  
                            'rpo',   
                            'rdl');   
                    SET @Arrears = @AS400Bal + @Paid;   
                END    
                ELSE  
                BEGIN    
                    SET @Arrears = 0;   
                END   
           END    
        END    
        ELSE  
        BEGIN    
         IF @OutStBal= 0     
            BEGIN  
                SET @Arrears = @OutStBal;   
            END  
            ELSE  
            BEGIN  
                SET @Arrears = 0;   
            END  
        END     
  
   IF @Noupdates = 0   
   BEGIN     
   EXECUTE dbbestacct  
    @origbr         = @bno,   
    @Arrears        = @Arrears,   
    @OutStBal       = @OutStBal,   
    @AcctNo         = @AcctNo,   
    @CurrStatus     = @CurrStatus,   
    @DateAcctOpen   = @DateAcctOpen,   
    @DateLastPaid   = @dPaid,   
    @NoDates        = @NoDates;  
               
  
   EXECUTE dbDateNextDue  
    @Arrears        = @Arrears,   
    @InstalAmount   = @InstalAmount,   
    @AcctNo         = @AcctNo,   
    @DateNextDue    = @DateNextDue,   
    @nsd            = @nsd,   
    @DateFirst      = @DateFirst,   
    @OutStBal       = @OutStBal,  
    @datedel  = @datedel;  
   END    
               
   -- ** FINISHED **  
   SET @Return = 0  
   RETURN 0;  
          
  END  /* IF @AgrmtTotal = 0 OR SUBSTRING(@AcctNo, 4, 1) = '4' */  
    -- VARIABLE instalments - update correct instalamount  
  IF @Noupdates = 0   
  BEGIN  
 UPDATE instalplan  
    SET instalamount = v.instalment  
    FROM instalmentvariable v  
    WHERE v.acctno = instalplan.acctno  
    AND instalamount !=v.instalment  
    AND v.dateFROM <=@datefrom   
    AND v.dateto >= @datefrom  
    AND v.acctno =@acctno  
  END  
      
  
    SELECT  @DateFirst      = DateFirst,   
            @DateLast       = dateadd(month,instalno-1,datefirst) , -- 72010 matching end of day arrears calc datelast   
            @InstalNo       = InstalNo,   
            @InstalAmount   = InstalAmount,  
            @InstalmentWaived = InstalmentWaived  -- CR1090   
    FROM    InstalPlan   
    WHERE   AcctNo = @AcctNo;   
   /* if delivery date less than these dates then not fully delivered */  
    IF @DateDel    <'1-jan-1910'    
    OR @DateDel     IS NULL  
    OR @DateFirst   <'1-jan-1910'      
    OR @DateLast    <'1-jan-1910'  
    OR @DateLast    IS NULL  
    BEGIN   
        IF @OutStBal < 0  
        BEGIN  
            SET @Arrears = @OutStBal;   
        END  
        ELSE  
        BEGIN  
            SET @Arrears = 0;   
        END  
  
        EXECUTE dbbestacct  
            @origbr    = @bno,   
            @Arrears        = @Arrears,   
            @OutStBal       = @OutStBal,   
            @AcctNo         = @AcctNo,   
            @CurrStatus     = @CurrStatus,   
            @NoDates        = @NoDates,   
            @DateAcctOpen   = @DateAcctOpen,   
            @DateLastPaid   = @dPaid;   
  
  
        IF @NoDates = 0  
        BEGIN  
            EXECUTE DBAgrDate  
                @AcctNo         = @AcctNo,   
                @AgrmtTotal     = @AgrmtTotal,   
                @CountPcent     = @CountPcent,   
                @DateFirst      = @DateFirst,   
                @AgrmtNo        = @AgrmtNo,   
                @origbr         = @bno;    
        END  
  
        -- ** FINISHED **  
        SET @Return = 0  
        RETURN 0;   
    END   
  
    -- 14/11/02  DSR Second "IF SUBSTRING(@AcctNo, 4, 1) = '4'"  
    -- code block removed as both redundant AND in error.  
  
  
    IF @InstalAmount IS NULL OR @InstalAmount = 0  
    BEGIN  
        EXECUTE dbbestacct  
            @origbr         = @bno,    
            @Arrears        = 0,   
            @OutStBal       = @OutStBal,   
            @AcctNo         = @AcctNo,   
            @CurrStatus     = @CurrStatus,   
            @DateAcctOpen   = @DateAcctOpen,   
            @DateLastPaid   = @dPaid,   
            @NoDates        = @NoDates;   
  
        IF @CurrStatus != 'U' AND @CurrStatus != 'S'  
        BEGIN  
            -- ** FINISHED **  
            SET @Return = 0  
            RETURN 0;  
        END  
    END  
  
-- If less than 7 am use previous day  
 if (DATEPART(hour,getdate()) <7 )  
  SELECT @d1 = dateadd(hour,-1-DATEPART(hour,getdate()),GETDATE());  
 else -- current day  
   SELECT @d1 = GETDATE()  
       
-- @?i are from Instalplan.datefirst  
    SET @di = DATEPART(day,   @DateFirst);   
    SET @mi = DATEPART(month, @DateFirst);   
    SET @yi = DATEPART(year,  @DateFirst);   
  
  
 DECLARE @repos MONEY -- we are going to check repos - if these put the account into credit then we will not zeroise the arrears.  
 -- repos and redeliveries after repo  
 SELECT @repos = ISNULL(SUM(transvalue ),0) FROM fintrans WHERE acctno = @acctno AND transtypecode IN ('rep','rdl')   
 -- will be -ve credit value so need to increase real balance -- will be plus  
  
-- is current date< date of last instalment  
    IF @d1 < @DateLast OR @DateLast = ''  
    BEGIN  
-- @?n are from current date  
        SET @dn = DATEPART(day,   @datefrom);   
        SET @mn = DATEPART(month, @datefrom);   
        SET @yn = DATEPART(year,  @datefrom);   
  
-- if day now is after datefirst day set date before (@db) to 1  
        IF @dn - @di > 0  
        BEGIN  
           SET @db = 1;   
        END  
        ELSE  
        BEGIN  
            SET @db = 0;   
        END  
    END  
    ELSE -- all due so arrears equals outstanding balance...   
    BEGIN   
        --SET @Arrears = ISNULL(@OutStBal, 0) -@repos;   
         SET @Arrears = case when ISNULL(@OutStBal, 0) <= 0 then 0 else ISNULL(@OutStBal, 0) -@repos end;  --IP - 02/04/12 - #9857 
  
        EXECUTE dbbestacct  
            @origbr         = @bno,   
            @Arrears        = @Arrears,   
            @OutStBal       = @OutStBal,   
            @AcctNo         = @AcctNo,   
            @CurrStatus     = @CurrStatus,   
            @DateAcctOpen   = @DateAcctOpen,   
            @DateLastPaid   = @dPaid,   
            @NoDates        = @NoDates;   
  
        EXECUTE dbDateNextDue  
            @Arrears        = @Arrears,   
            @InstalAmount   = @InstalAmount,   
            @AcctNo         = @AcctNo,   
            @DateNextDue    = @DateNextDue,   
            @nsd            = @nsd,   
            @DateFirst      = @DateFirst,   
            @OutStBal       = @OutStBal,   
            @DateDel        = @DateDel;   
  
        IF  @CurrStatus != 'U' AND @CurrStatus != 'S'  
        BEGIN  
            -- ** FINISHED **  
            SET @Return = 0  
            RETURN;   
        END  
    END  
      
  
-- Calc number of months since datefirst to get number of instalments due  
--  year (now)- year (datefirst) + month (now) - month (datefirst) + 1 (if day (datefirst) before day (now))   
    SET @nsd = 12*(@yn-@yi)+@mn-@mi+@db;   
  
    IF @DateFirst = ''  
    BEGIN  
        SET @nsd = 0;   
    END  

	--#19284 - CR15594
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
  
    -- 63890    SET @nsd = @nsd-@mthsdeferred;   
    SELECT  @InstalPreDel = ISNULL(InstalPreDel,'N')   
    FROM    termstype   
    WHERE   termstype = @ttype;   
  
    IF @InstalPreDel = 'Y'  
  and @InstalmentWaived=0  --CR1090 Instalment not waived  
  and @IsReadyAssist = 0   --#19284 - CR15594 - and not a Ready Assist
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
    if exists (select * from instalmentvariable where acctno =@acctno)  
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
            'RFN',    -- CR976  
            'CLD',  --jec CR1232 #3291 Loan Disbursement    
            'rpo',   
            'rdl')   
    AND DateTrans > @dte;   
    -- include Refinance Deposit in paid amount      CR976 jec 17/04/09  
    SELECT  @Paid = @paid + ISNULL(SUM(Transvalue), 0)   
    FROM    FinTrans   
    WHERE   AcctNo = @AcctNo   
    AND     TransTypeCode IN  
            ('RFD')   
    and transvalue<0      
    AND DateTrans > @dte;   
  
    IF @Paid IS NULL  
    BEGIN  
        SET @Paid = 0;   
    END  
 -- correct calc.  @paid is -ve and @deposit is +ve        jec 17/04/09  
    IF  abs(@Paid) >= @Deposit AND @CurrStatus = 'U'  -- was @Paid > @Deposit AND @CurrStatus = 'U'  
    BEGIN  
        SET @CurrStatus = '1';   
    END  
  
    IF @Paid IS NULL  
    BEGIN  
        SET @Paid = 0;   
    END  
  
    SET @Arrears = @mon+@Paid;   
  
  
     -- inserting into arrearsdaily if changed - so brings up any changes instantaneously  
      
    IF @Noupdates = 0   
    BEGIN  
  UPDATE ArrearsDaily  SET dateto =     dateadd(day,-1,@today) -- yesterday  
     WHERE acctno = @acctno AND dateto is null AND    
     arrearsdaily.arrears >0 AND     
     exists (select * FROM  ArrearsDaily d        
     WHERE d.acctno =@acctno AND d.arrears !=@arrears AND d.dateto is null)  
           
     insert into ArrearsDaily ( acctno, arrears,dateFROM,dateto)     
     select @acctno,@arrears,    @today   , NULL         WHERE not exists (select * FROM  ArrearsDaily d        
     WHERE d.acctno =@acctno AND d.arrears =@arrears AND d.dateto is null) AND @arrears > 0  
    END   
      
 IF @OutStBal = 0  AND @repos =0 -- don't automatically settle if outstanding repos  
 BEGIN  
  SELECT  @Paid = ISNULL(SUM(Transvalue), 0)   
  FROM    FinTrans    
  WHERE   AcctNo = @AcctNo   
  AND     TransTypeCode in  
    ('grt',   
    'del',   
    'add',  
    'CLD',   --jec CR1232 #3291 Loan Disbursement  
    'RFD',    -- CR976 jec 17/04/09  Refinance Deposit  
    'RFN');   -- CR976 jec 16/04/09  
  
  IF  @Paid = @AgrmtTotal AND @Paid != 0  
  BEGIN  
   SET @CurrStatus = 'S';   
  END  
 END  
 if @instalamount >0  
    begin  
  SET @state = CAST(@Arrears/@InstalAmount AS INT);   
  IF @nsd-@state > 50   
  BEGIN  
   SET @dnd = '';   
  END  
  ELSE  
  BEGIN  
    
   declare @addMonthsToDateFirst int = 0

    if((@nsd - @state)  < 0)
    begin
        set @addMonthsToDateFirst = 0
    end
    else
    begin
        set @addMonthsToDateFirst = @nsd - @state
    end

   SET @dnd = DATEADD(month,@addMonthsToDateFirst,@DateFirst);   
  END  
    end  
    else  
 begin -- if no instalamount then return 0 as can be no arrears.  
  set @arrears = 0   
 end  
  
 IF @Noupdates = 0   
 BEGIN  
 EXECUTE dbbestacct  
        @origbr         = @bno,  
        @Arrears        = @Arrears,   
        @OutStBal       = @OutStBal,   
        @AcctNo         = @AcctNo,   
        @InstalAmount   = @InstalAmount,   
        @CurrStatus     = @CurrStatus,   
        @DateAcctOpen   = @DateAcctOpen,   
        @DateLastPaid   = @dPaid,  
        @NoDates        = @NoDates;   
  
    EXECUTE dbDateNextDue  
        @Arrears        = @Arrears,  
        @InstalAmount   = @InstalAmount,   
        @AcctNo         = @AcctNo,   
        @DateNextDue    = @DateNextDue,   
        @nsd            = @nsd,   
        @DateFirst      = @DateFirst,   
        @OutStBal       = @OutStBal,   
        @DateDel        = @DateDel;  
  
 END   
    SET @Return = @@ERROR  
    RETURN 0  
  