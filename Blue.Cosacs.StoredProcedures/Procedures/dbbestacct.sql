if exists (select * from dbo.sysobjects where id = object_id('[dbo].[dbbestacct]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[dbbestacct]
GO

CREATE procedure [dbo].[dbbestacct]
    @acctno   char(12) = ' ', 
    @outstbal money = 0, 
    @origbr  smallint = 0, 
    @arrears money = 0, 
    @datelastpaid DATETIME = '1900-01-01', 
    @instalamount money = 0, 
    @dateacctopen DATETIME = '1900-01-01', 
    @nodates   smallint = 0, 
    @currstatus    char(1) = ' '   
as 
declare
    @agrmtno smallint, 
    @i integer, 
    @dte DATETIME, 
    @datestatchge datetime, 
    @statuscode char(1), 
    @bno smallint 
    set @agrmtno = 0;
    set @i = 0;
    set @dte = '1-jan-1900'
    set @statuscode = ''
    set @bno = 0;
    select  @bno = branchno 
    from  server; 
    if @nodates = 0
    BEGIN
        select  @dte = isnull(min(datetrans), '') 
        from  fintrans 
        where acctno = @acctno and transtypecode ='PAY'; 
        if @dte is null
        BEGIN
            set @dte = ''; 
        END
        select  @datelastpaid = isnull(max(datetrans), '') 
        from  fintrans 
        where acctno = @acctno 
        and transtypecode in ('PAY', 'DDN', 'DDR', 'DDE'); 
        if @datelastpaid is null
        BEGIN
            set @datelastpaid = ''; 
        END
        select @dateacctopen = isnull(dateacctopen, ''),
            @currstatus = isnull(currstatus, '') 
        from acct 
        where acctno = @acctno; 
        if @currstatus is null
        BEGIN
            set @currstatus = '1'; 
        END
        if (@dateacctopen > @dte 
        and @dte != '' 
        and @dte is not null
        and @dte >'1-jan-1980') 
        or  (@dateacctopen = '' and @dte >'1-jan-1980')
        BEGIN
            set @dateacctopen = @dte; 
        END

		DECLARE @repos MONEY -- we are going to check repos - if these put the account into credit then we will not zeroise the arrears.
		-- repos and redeliveries after repo
		SELECT @repos = ISNULL(SUM(transvalue ),0) FROM fintrans WHERE acctno = @acctno AND transtypecode IN ('rep','rdl') 
		-- will be -ve credit value so need to increase real balance -- will be plus
        if @arrears > @outstbal
        BEGIN
            if @outstbal > 0
            BEGIN
                set @arrears = @outstbal; 
            END
            ELSE
            BEGIN
                set @arrears = 0; 
            END
        END
    END
    -- using as400bal to store arrears ex charges
    declare @as400bal money
    select @as400bal = sum(transvalue) from fintrans where acctno = @acctno and transtypecode not in ('INT','ADM')


	 IF EXISTS (SELECT 'A' FROM ACCT WHERE acctno = @acctno and isAmortized =1 and IsAmortizedOutStandingBal =1)
	BEGIN 
	-------Calculate outstanding balance------------------------
	Declare @outstandingBalance DECIMAL(15,4)
	EXEC CLAmortizationCalcDailyOutstandingBalance @acctno,@outstandingBalance OUT  --SP returning new Oustanding balance
	SET @OutStBal = @outstandingBalance
	SET	@as400bal = @outstandingBalance -  (Select sum(transvalue) from fintrans where acctno = @acctno and transtypecode  in ('INT','ADM'))
	END
		 

    update  acct 
    set origbr = @bno, 
    arrears = @arrears, 
    outstbal = @outstbal, 
    as400bal = @as400bal,
    dateacctopen = @dateacctopen, 
    datelastpaid = datelastpaid 
    where acctno = @acctno; 

	--IP - 06/12/2007 - UAT(188)
	--Previously the 'datedel' was not being set for a Cash account when it had been delivered.
	IF SUBSTRING(@AcctNo, 4, 1) = '4'
	BEGIN

		DECLARE @maxdatetrans DATETIME
		SET @maxdatetrans = (SELECT MAX(ISNULL(datetrans,'1900-01-01')) FROM fintrans f WHERE f.acctno = @acctno AND transtypecode IN ('DEL'))

		UPDATE agreement
		SET datedel = @maxdatetrans
		WHERE acctno = @acctno

	END 
	
RETURN

go
