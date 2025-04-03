SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE Name = 'DN_GetBookingsSP' AND Type = 'P')
BEGIN
    DROP PROCEDURE DN_GetBookingsSP
END
GO

CREATE PROCEDURE DN_GetBookingsSP

--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_GetBookingsSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Monitor Bookings
-- Author       : G Johnson
-- Date         : 13 June 2005
-- Version      : 002
--
-- Change Control
-- --------------
-- Date      	By  Description
-- ----      	--  -----------
-- 26/07/2005	RD  CR527 Initial Creation of the Store Procedure
-- 03/08/2005   GJ  Changed invalid reference to acct.securitised to a.securitised 
-- 08/08/2005	RD  Modified to correct error founding during initial testing
-- 15/08/2005   RD  Modified to correct value for accts opened & canx in the same period
-- 31/08/2005	RD  Modified to correct UAT issues 
-- 14/12/2005	RD  Modified to corerct UAT issue 266
-- 10/02/2006   RD  Modified to use the this procedure for Man Rpt1 
-- 22/02/2006	RD  Man Rpt 1 UAT issue 39 to ensure that value and revised are poupulated correctly
-- 22/02/2006   RD  67977 Modified to get the correct value for Salestax, warranty and Discount
-- 23/02/2006   RD  67977 Further changes done to get values from lineitemaudit table for removed items
-- 27/02/2006   RD  UAT issue 11 for v3526 added missing link for Speical account for a.branchno = b.branchno 
-- 10/03/2006   RD  Modified to only load special accounts with Paid & Taken custid as part of 68003
-- 15/03/2006	RD  67915 Modified to split 'Salesperson" into two columns: "Salesperson ID" and "Salesperson Name" 
-- 17/07/2006	RD  UAT issue 40 Modified to agreement total for RF account open without product
-- 17/07/2006   RD  v5.0.0.0 UAT issue 33 - Incorrect value for revise
------------------------------------------------------------------------------------------------------
-- 28/09/2006   KEF 68394 re-written as report is incorrect - changing calc to use lineitemaudit table
------------------------------------------------------------------------------------------------------
-- 03/08/2011   IP  RI - System Integration Changes
-- 26/11/2020   ST  #7403047 - Duplicate record shown after click on search button for respective sales person.
--------------------------------------------------------------------------------

    -- Parameters
    @branchNo         VARCHAR(12),
    @salespersonNo    VARCHAR(12),
    @datefrom         DATETIME,
    @dateto           DATETIME,
    @includeCash      SMALLINT,
    @includeHP        SMALLINT,
    @includeNonSec    SMALLINT,
    @includePaidTaken SMALLINT,
    @includeRf        SMALLINT,
    @includeSec       SMALLINT,
    @rollUpResults    SMALLINT,
    @Return           INTEGER = 0 OUT

AS -- DECLARE
    -- Local variables

BEGIN

declare --sqltext is our user defined datatype VARCHAR(4000) as we replace all VARCHARs with chars for non-thai countries
	@statement sqltext,
	@statement1a varchar(500),
	@statement1b varchar(100),
	@statement1c varchar(100),
	@statement1d varchar(100),
	@statement1e varchar(1000),
	@statement2a sqltext,
	@statement2aa sqltext,
    @statement2b sqltext,
    @statement2c sqltext,
 	@statement2d sqltext,
 	@statement2e sqltext,
 	@statement3a sqltext,
 	@statement3b sqltext,
 	@statement3c sqltext,
 	@statement3d sqltext,
 	@statement3e sqltext,
 	@statement3f sqltext,
 	@statement3g sqltext,
 	@statement4a sqltext,
 	@statement4b sqltext,
 	@statement4c sqltext,
 	@statement4d sqltext,
 	@statement4e sqltext,
 	@statement4f sqltext,
 	@statement4g sqltext,
 	@statement4h sqltext,
 	@statement4ia sqltext,
 	@statement4i sqltext,
 	@statement4j sqltext,
 	@statement4k sqltext,
    @error varchar(100)

create table #totals 
	(acctno varchar (12), 
	 dateoccurred datetime,
	 branchname varchar(45), 
	 EmpeeNo int,	-- 67915 RD 15/03/06
	 EmployeeName varchar(101), 
	 accttype varchar(2), 
	 accttypegroup varchar(2), 
	 value money, 
	 cancelled money, 
	 revised money, 
	 SalesTax money,  
	 balance money, 
	 warranty money default 0, 
	 discount money,
     Securitised char(1),
     new_count smallint,
     revise_count smallint,
     cancel_count smallint)

create table #totals2
	(acctno varchar (12), 
     agrmtno int, 
	 dateoccurred datetime,
	 branchname varchar(45), 
	 EmpeeNo int,
	 value money, 
	 SalesTax money,  
	 balance money, 
	 warranty money default 0, 
	 discount money,
     pt_count smallint)

    -----------------------------------------
    --Step 1 - put restrictions in statements
    -----------------------------------------
    ----------
    --accttype
    ----------
    if (@includeCash = 0 and @includeHP = 0 and @includeRF = 0 and @includePaidTaken = 0) --at least 1 must be set
    begin
       set @error ='Please select at least 1 account type'
       RAISERROR(@error, 16, 1) 
    end
    else
    begin
        if @includeCash = 1 --set
        begin
            set @statement1a = ' and accttype in (''C'','
        end
        else
        begin
            set @statement1a = ' and accttype in ('''',' 
        end
        if @includeHP = 1 --set
        begin
            set @statement1a = @statement1a + 
                '''O'',''H'',''B'',''D'',''E'',''F'',''G''' --need to include HP and old deferred acct types
        end
        else
        begin
            set @statement1a = @statement1a + ' '''' ' 
        end
        if @includeRF = 1 --set
        begin
            set @statement1a = @statement1a + 
                ',''R'') '
        end
        else
        begin
            set @statement1a = @statement1a + ')'
        end
--***** Will do Cash and Go values seperately to other account types *****
    end

    ----------
    --branchno
    ----------
    if @branchno <> '0' --not set to all
    begin
        set @statement1b = ' and left(i.acctno,3) = '''+ @branchno + ''''
    end
    else
    begin
       set @statement1b = '' 
    end

    ----------------
    --securitisation
    ----------------
    if @includeNonSec = 0 and @includeSec = 0 --not set to all
    begin
       set @error ='Please select at least 1 securitisation option'
       RAISERROR(@error, 16, 1) 
    end
    if @includeNonSec = 1 and @includeSec = 1
    begin
       set @statement1c = ''
    end
    else
    begin
        if @includeNonSec = 1 and @includeSec = 0
        begin
           set @statement1c = ' and securitised <> ''Y'' '
        end
        if @includeNonSec = 0 and @includeSec = 1
        begin
            set @statement1c = @statement1c + 
                ' and securitised = ''Y'' '
        end
    end
--print '@statement1c ' + @statement1c
    ---------------------------------------
    --restrict data if salesperson selected
    ---------------------------------------
    if @salespersonNo != '0' --not set to 'all'
    begin
        set @statement1d = ' and ag.empeenosale = '+ convert(varchar,@salespersonNo)
    end
    else
    begin
        set @statement1d = ''
    end

    -------------------------------------
    --construct restrictions where clause
    -------------------------------------
    set @statement1e = @statement1a + @statement1b + @statement1c + @statement1d

--print 'here: ' + @statement1e

    -------------------------------------------------------------
    --Step 2 Get figures for all account types except cash and go
    -------------------------------------------------------------
    --No need to do this if only Cash and Go selected
    if not (@includeCash = 0 and @includeHP = 0 and @includeRF = 0) --only cash and go selected
    begin
        --------------
        --new accounts
        --------------
       set @statement2a = 
    	    ' INSERT INTO #Totals (Acctno, Dateoccurred, BranchName, EmpeeNo,  '+
    	    '        EmployeeName, Accttype, Accttypegroup, Value, Cancelled, Revised, SalesTax, Balance, warranty, Discount, Securitised, '+
            '        new_count, revise_count, cancel_count) '+
    	    ' SELECT i.acctno, CONVERT(datetime,CONVERT(char(10),i.datechange,101)), convert(varchar(20),left(i.acctno,3)), ag.empeenosale, '+
    	    '        '''', Accttype, '''', sum(i.valueafter)-sum(i.valuebefore), 0, 0, isnull(sum(TaxAmtAfter)-sum(TaxAmtbefore),0), 0, 0, 0, Securitised, '+
            '        1, 0, 0 '+
    	    ' FROM	 lineitemaudit i, acct a, agreement ag '+
        	' WHERE	 i.source = ''NewAccount'' '+ --indicates new account screen
    	    ' AND 	 i.datechange >= convert(VARCHAR,'''+ convert(varchar,@datefrom) +''') '+ 
    	    ' AND    i.datechange <= convert(VARCHAR,'''+ convert(varchar,@dateto) +''') '+ --add 1 day to remove time part errors
            ' AND    i.acctno = a.acctno '+ --join tables
            ' AND    i.acctno = ag.acctno '+ --join tables
            --' AND    not exists (select itemno from kitproduct k where k.itemno = i.itemno) '+ --Exclude kit items
            ' AND    not exists (select ItemID from kitproduct k where k.ItemID = i.ItemID) '+ --Exclude kit items							--IP - 03/08/11 - RI					
            @statement1e +		
            ' GROUP BY i.acctno, accttype, CONVERT(datetime,CONVERT(char(10),i.datechange,101)), '+
            '          ag.empeenosale, Securitised '
--print '2a' + @statement2a
        --execute statement
        exec sp_executesql @statement2a

        ----------
        --accttype
        ----------
        --update RF with product as all RF accounts in table at this point were opened with product
        set @statement2aa = 
            ' UPDATE #Totals '+
            ' SET    accttypegroup  = ''RP'' '+    --RF account with Product
--            ' from   acct a '+
            ' where  #totals.accttype = ''R'' '
--            ' and    a.acctno = #totals.acctno '+
--            ' and    CONVERT(datetime,CONVERT(char(10),dateacctopen,101)) = dateoccurred ' --check dateacctopen is same as dateoccured
        --execute statement
        exec sp_executesql @statement2aa

--print 'here3'
        --------------------
        --cancelled accounts
        --------------------
        --Update existing records with any cancellation value
        set @statement2b = 
            ' UPDATE #Totals '+
            ' SET    Cancelled = isnull((select sum(i.valueafter)-sum(i.valuebefore)'+    
            '                     from	 lineitemaudit i, agreement ag '+
        	'                     where  i.source in (''CancelAccount'',''GRTCancelRev'',''GRTCancel'') '+ --indicates cancelled account
            '                     and    i.acctno = #Totals.acctno '+ --join on acctno
            '                     and    i.acctno = ag.acctno '+ --join on acctno
            '                     and    ag.empeenosale = #Totals.EmpeeNo '+ --join on empeeno
            '                     and    CONVERT(datetime,CONVERT(char(10),i.datechange,101)) = #Totals.Dateoccurred '+ --join on datechanged, remove timepart
    	    '                     and 	 i.datechange >= convert(VARCHAR,'''+ convert(varchar,@datefrom) +''') '+ 
    	    '                     and    i.datechange <= convert(VARCHAR,'''+ convert(varchar,@dateto) +''') '+ --add 1 day to remove time part errors
            --'                     and    not exists (select itemno from kitproduct k where k.itemno = i.itemno) '+ --Exclude kit items
            '                     and    not exists (select ItemID from kitproduct k where k.ItemID = i.ItemID) '+ --Exclude kit items							--IP - 03/08/11 - RI
                                  @statement1e + '),0), '+
            '        SalesTax = SalesTax+isnull((select sum(TaxAmtafter)-sum(TaxAmtbefore)'+
            '                     from	 lineitemaudit i, agreement ag '+
        	'                     where  i.source in (''CancelAccount'',''GRTCancelRev'', ''GRTCancel'') '+ --indicates cancelled account
            '                     and    i.acctno = #Totals.acctno '+ --join on acctno
            '                     and    i.acctno = ag.acctno '+ --join on acctno
            '                     and    ag.empeenosale = #Totals.EmpeeNo '+ --join on empeeno
            '                     and    CONVERT(datetime,CONVERT(char(10),i.datechange,101)) = #Totals.Dateoccurred '+ --join on datechanged, remove timepart
    	    '                     and 	 i.datechange >= convert(VARCHAR,'''+ convert(varchar,@datefrom) +''') '+ 
    	    '                     and    i.datechange <= convert(VARCHAR,'''+ convert(varchar,@dateto) +''') '+ --add 1 day to remove time part errors
            --'                     and    not exists (select itemno from kitproduct k where k.itemno = i.itemno) '+ --Exclude kit items
            '                     and    not exists (select ItemID from kitproduct k where k.ItemID = i.ItemID) '+ --Exclude kit items							--IP - 03/08/11 - RI
                                  @statement1e + ' ),0) '
        --execute statement
        exec sp_executesql @statement2b
--select '2b', * from #totals
--print 'here4'
        --Insert new rows for cancellation records
        set @statement2c = 
    	    ' INSERT INTO #Totals (Acctno, Dateoccurred, BranchName, EmpeeNo,  '+
    	    '        EmployeeName, Accttype, Accttypegroup, Value, Cancelled, Revised, SalesTax, Balance, warranty, Discount, Securitised, '+
            '        new_count, revise_count, cancel_count) '+
    	    ' SELECT i.acctno, CONVERT(datetime,CONVERT(char(10),i.datechange,101)), convert(varchar(20),left(i.acctno,3)), ag.empeenosale, '+
    	    '        '''', Accttype, '''', 0, sum(i.valueafter)-sum(i.valuebefore), 0, isnull(sum(TaxAmtafter)-sum(TaxAmtbefore),0), 0, 0, 0, Securitised, '+
            '        0, 0, 0 '+
    	    ' FROM	 lineitemaudit i, acct a, agreement ag '+
        	' WHERE	 i.source in (''CancelAccount'',''GRTCancelRev'',''GRTCancel'') '+ --indicates new account screen
    	    ' AND 	 i.datechange >= convert(VARCHAR,'''+ convert(varchar,@datefrom) +''') '+ 
    	    ' AND    i.datechange <= convert(VARCHAR,'''+ convert(varchar,@dateto) +''') '+ --add 1 day to remove time part errors
            ' AND    i.acctno = a.acctno '+ --account meets restrictions for report
            ' AND    i.acctno = ag.acctno '+ 
            ' AND    not exists (select acctno from #Totals t2 '+
            '                    where  i.acctno = t2.acctno'+ --join on acctno
            '                    and    ag.empeenosale = t2.EmpeeNo '+ --join on empeeno'+
            '                    and    CONVERT(datetime,CONVERT(char(10),i.datechange,101)) = t2.Dateoccurred) '+ --join on datechanged)'+ --account information not already in table (if so then already got value from above update)
            --' AND    not exists (select itemno from kitproduct k where k.itemno = i.itemno) '+ --Exclude kit items
            ' AND    not exists (select ItemID from kitproduct k where k.ItemID = i.ItemID) '+ --Exclude kit items				--IP - 03/08/11 - RI
            @statement1e +
            ' GROUP BY i.acctno, accttype, CONVERT(datetime,CONVERT(char(10),i.datechange,101)), '+
            '          ag.empeenosale, Securitised '
        --execute statement
        exec sp_executesql @statement2c
--select '2c', * from #totals    
--print 'here5'
        ---------------------------------------
        --revised accounts (except Cash and Go)
        ---------------------------------------
        --Update existing records with any cancellation value
        set @statement2d = 
            ' UPDATE #Totals '+
            ' SET    Revised = isnull((select sum(i.valueafter)-sum(i.valuebefore)'+ --after - before
            '                   from   lineitemaudit i, agreement ag '+
        	'                   where  i.source in (''Revise'',''GRTExchangeRev'',''GRTExchange'') '+ --indicates revision on account
            '                   and    i.acctno = #Totals.acctno '+ --join on acctno
            '                   and    i.acctno = ag.acctno '+ --join on acctno
            '                   and    ag.empeenosale = #Totals.EmpeeNo '+ --join on empeeno
            '                   and    CONVERT(datetime,CONVERT(char(10),i.datechange,101)) = #Totals.Dateoccurred '+ --join on datechanged, remove timepart
    	    '                   and    i.datechange >= convert(VARCHAR,'''+ convert(varchar,@datefrom) +''') '+ 
    	    '                   and    i.datechange <= convert(VARCHAR,'''+ convert(varchar,@dateto) +''') '+ --add 1 day to remove time part errors
            --'                   and    not exists (select itemno from kitproduct k where k.itemno = i.itemno) '+ --Exclude kit items
            '                   and    not exists (select ItemID from kitproduct k where k.ItemID = i.ItemID) '+ --Exclude kit items				--IP - 03/08/11 - RI
                                @statement1e + ' ),0), ' +
            '        SalesTax = SalesTax+isnull((select sum(TaxAmtAfter)-sum(TaxAmtBefore)'+
            '                     from	 lineitemaudit i, agreement ag '+
        	'                     where  i.source in (''Revise'',''GRTExchangeRev'', ''GRTExchange'') '+ --indicates revised account
            '                     and    i.acctno = #Totals.acctno '+ --join on acctno
            '                     and    i.acctno = ag.acctno '+ --join on acctno
            '                     and    ag.empeenosale = #Totals.EmpeeNo '+ --join on empeeno
            '                     and    CONVERT(datetime,CONVERT(char(10),i.datechange,101)) = #Totals.Dateoccurred '+ --join on datechanged, remove timepart
    	    '                     and 	 i.datechange >= convert(VARCHAR,'''+ convert(varchar,@datefrom) +''') '+ 
    	    '                     and    i.datechange <= convert(VARCHAR,'''+ convert(varchar,@dateto) +''') '+ --add 1 day to remove time part errors
            --'                     and    not exists (select itemno from kitproduct k where k.itemno = i.itemno) '+ --Exclude kit items
            '                     and    not exists (select ItemID from kitproduct k where k.ItemID = i.ItemID) '+ --Exclude kit items				--IP - 03/08/11 - RI
                                @statement1e + ' ),0), ' +
            '        revise_count = isnull((select count(distinct i.acctno) '+
            '                        from	 lineitemaudit i, agreement ag '+
        	'                        where  i.source in (''Revise'', ''GRTExchange'') '+ --indicates revised account - not exchange reversal as this counts as -1 so do in later update
            '                        and    i.acctno = #Totals.acctno '+ --join on acctno
            '                        and    i.acctno = ag.acctno '+ --join on acctno
            '                        and    ag.empeenosale = #Totals.EmpeeNo '+ --join on empeeno
            '                        and    CONVERT(datetime,CONVERT(char(10),i.datechange,101)) = #Totals.Dateoccurred '+ --join on datechanged, remove timepart
    	    '                        and 	i.datechange >= convert(VARCHAR,'''+ convert(varchar,@datefrom) +''') '+ 
    	    '                        and    i.datechange <= convert(VARCHAR,'''+ convert(varchar,@dateto) +''') '+ --add 1 day to remove time part errors
            --'                        and    not exists (select itemno from kitproduct k where k.itemno = i.itemno) '+ --Exclude kit items
            '                        and    not exists (select ItemID from kitproduct k where k.ItemID = i.ItemID) '+ --Exclude kit items			--IP - 03/08/11 - RI
                                @statement1e + ' ),0) '
        --execute statement
        exec sp_executesql @statement2d
--print 'here6'
        --Insert new rows for revised records
        set @statement2e = 
    	    ' INSERT INTO #Totals (Acctno, Dateoccurred, BranchName, EmpeeNo,  '+
    	    '        EmployeeName, Accttype, Accttypegroup, Value, Cancelled, Revised, SalesTax, Balance, warranty, Discount, Securitised, '+
            '        new_count, revise_count, cancel_count) '+
    	    ' SELECT i.acctno, CONVERT(datetime,CONVERT(char(10),i.datechange,101)), convert(varchar(20),left(i.acctno,3)), ag.empeenosale, '+
    	    '        '''', Accttype, '''', 0, 0, sum(i.valueafter)-sum(i.valuebefore), isnull((sum(TaxAmtAfter)-sum(TaxAmtBefore)),0), 0, 0, 0, Securitised, '+
            '        0, 0, 0 '+
    	    ' FROM	 lineitemaudit i, acct a, agreement ag '+
        	' WHERE	 i.source in (''Revise'',''GRTExchangeRev'', ''GRTExchange'') '+ --indicates new account screen
    	    ' AND 	 i.datechange >= convert(VARCHAR,'''+ convert(varchar,@datefrom) +''') '+ 
    	    ' AND    i.datechange <= convert(VARCHAR,'''+ convert(varchar,@dateto) +''') '+ --add 1 day to remove time part errors
            ' AND    i.acctno = a.acctno '+ --account meets restrictions for report
            ' AND    i.acctno = ag.acctno '+ --account meets restrictions for report
            ' AND    not exists (select acctno from #Totals t2 '+
            '                    where  i.acctno = t2.acctno'+ --join on acctno
            '                    and    ag.empeenosale = t2.EmpeeNo '+ --join on empeeno'+
            '                    and    CONVERT(datetime,CONVERT(char(10),i.datechange,101)) = t2.Dateoccurred) '+ --join on datechanged)'+ --account information not already in table (if so then already got value from above update)
            --' AND    not exists (select itemno from kitproduct k where k.itemno = i.itemno) '+ --Exclude kit items
            ' AND    not exists (select ItemID from kitproduct k where k.ItemID = i.ItemID) '+ --Exclude kit items				--IP - 03/08/11 - RI
            @statement1e +
            ' GROUP BY i.acctno, accttype, CONVERT(datetime,CONVERT(char(10),i.datechange,101)), '+
          '          ag.empeenosale, Securitised '
        --execute statement
        exec sp_executesql @statement2e
--select '2e', * from #totals order by acctno, dateoccurred
    end
--print 'here7'

    ------------------------------------
    --Step 3 Get figures for Cash and Go
    ------------------------------------
    -----------------------------------------------------------
    --new accounts + returns - all under agreement value column
    -----------------------------------------------------------
    IF @includePaidTaken = 1 and @includeNonSec = 1 --no paid and taken account securitised so no need to do if option not set
	BEGIN
        --set salesperson restriction
        if @salespersonNo <> 0 --salesperson selected
        begin
            set @statement1b = @statement1b + ' AND d.notifiedby = ' + convert(varchar,@salespersonNo)
        end

        set @statement3a = 
    	    ' INSERT INTO #Totals2 (Acctno, Agrmtno, Dateoccurred, BranchName,  '+
    	    '        EmpeeNo, Value, SalesTax, Balance, Warranty, Discount, pt_count) '+
    	    ' SELECT d.acctno, d.Agrmtno, CONVERT(datetime,CONVERT(char(10),datetrans,101)), convert(varchar(20),left(d.acctno,3)),'+
    	    '        d.notifiedby, sum(d.transvalue), 0, 0, 0, 0, 0 '+
    	    ' FROM	 delivery d, custacct i, stockitem s, acct a '+ --need stockitem table to check taxrate
        	' WHERE	 d.datetrans >= convert(VARCHAR,'''+ convert(varchar,@datefrom) +''') '+ 
    	    ' AND    d.datetrans <= convert(VARCHAR,'''+ convert(varchar,@dateto) +''') '+ --add 1 day to remove time part errors
            ' AND    d.acctno = i.acctno '+ --join tables
            --' AND    d.itemno = s.itemno '+ --join tables
            ' AND    d.ItemID = s.ID '+ --join tables								--IP - 03/08/11 - RI
            ' AND    d.stocklocn = s.stocklocn '+ --join tables
			' AND    d.acctno = a.acctno '+ --join tables
			' AND    (i.custid like ''PAID%'' or a.accttype = ''s'') '+
            --' AND    not exists (select itemno from kitproduct k where k.itemno = d.itemno) '+ --Exclude kit items
            ' AND    not exists (select ItemID from kitproduct k where k.ItemID = d.ItemID) '+ --Exclude kit items				--IP - 03/08/11 - RI
            @statement1b +
            ' GROUP BY d.acctno, d.Agrmtno, CONVERT(datetime,CONVERT(char(10),datetrans,101)), '+
            '          d.notifiedby'
        --execute statement
--    print 'here8: ' + @statement3a
        exec sp_executesql @statement3a
  

       ----------------
        --Update SalesTax
        ----------------
		 set @statement3b = 
            ' update #totals2 '+
            ' set    Salestax = isnull((SELECT sum(l.taxamt) '+ 
    	    '                    FROM	lineitem l, delivery d, custacct i'+ 
        	'                    WHERE	d.datetrans >= convert(VARCHAR,'''+ convert(varchar,@datefrom) +''') '+ 
    	    '                    AND    d.datetrans <= convert(VARCHAR,'''+ convert(varchar,@dateto) +''') '+ --add 1 day to remove time part errors
            '                    AND    d.acctno = #totals2.acctno '+ --join tables
            '                    AND    d.Agrmtno = #totals2.Agrmtno '+ --join tables
            '                    AND    CONVERT(datetime,CONVERT(char(10),d.datetrans,101)) = CONVERT(datetime,CONVERT(char(10),#totals2.dateoccurred,101)) '+ --join tables
            '                    AND    d.notifiedby = #totals2.empeeno '+ --join tables
            '                    AND    d.acctno = i.acctno '+ --join tables
			'                    AND    d.agrmtno = l.agrmtno '+ --join tables
            '                    AND    d.acctno = l.acctno '+ --join tables							--IP - 03/08/11 - RI
            '                    AND    d.stocklocn = l.stocklocn '+ --join tables
			'                    AND    d.itemid = l.itemid),0)'

		 exec sp_executesql @statement3b


        --------------
        --update count
        --------------
        set @statement3c = 
            ' update #totals2 '+
            ' set    pt_count = 1 '+
            ' from   #totals2 t2 '+
            ' where  value > 0 ' --if > 0 count as sale. If sale and return in same day then will be 0. if sale one day and return after same day then will count as 1, next update will count return as -1 so report will show 0 if run for date range that had sale and return 
        --execute statement
        exec sp_executesql @statement3c

        set @statement3d = 
            ' update #totals2 '+
            ' set    pt_count = -1 '+
            ' from   #totals2 t2 '+
            ' where  value < 0 ' --if > 0 count as sale. If sale and return in same day then will be 0. if sale one day and return after same day then will count as 1, next update will count return as -1 so report will show 0 if run for date range that had sale and return 
        --execute statement
        exec sp_executesql @statement3d

--select 'bef3e', * from #totals2
        -----------------
        --update discount
        -----------------
        set @statement3e = 
            ' update #totals2 '+
            ' set    discount = isnull((SELECT sum(d.transvalue) '+ 
    	    '                    FROM	delivery d, custacct i, stockitem s, acct a '+ --need stockitem table to check taxrate
        	'                    WHERE	d.datetrans >= convert(VARCHAR,'''+ convert(varchar,@datefrom) +''') '+ 
    	    '                    AND    d.datetrans <= convert(VARCHAR,'''+ convert(varchar,@dateto) +''') '+ --add 1 day to remove time part errors
            '                    AND    d.acctno = #totals2.acctno '+ --join tables
            '                    AND    d.Agrmtno = #totals2.Agrmtno '+ --join tables
            '                    AND    CONVERT(datetime,CONVERT(char(10),d.datetrans,101)) = CONVERT(datetime,CONVERT(char(10),#totals2.dateoccurred,101)) '+ --join tables
            '                    AND    d.notifiedby = #totals2.empeeno '+ --join tables
            '                    AND    d.acctno = i.acctno '+ --join tables
			'                    AND    d.acctno = a.acctno '+ --join tables
            --'                    AND    d.itemno = s.itemno '+ --join tables
            '                    AND    d.ItemID = s.ID '+ --join tables											--IP - 03/08/11 - RI
            '                    AND    d.stocklocn = s.stocklocn '+ --join tables
            '                    AND    (i.custid like ''PAID%'' or a.accttype = ''s'') '+
            '                    AND    category in (select code from code where category = ''PCDIS'') '+
                                 @statement1b + ' ),0) '
--            '                    GROUP BY d.acctno, CONVERT(datetime,CONVERT(char(10),datetrans,101)), d.notifiedby),0) '
        --execute statement
--    print 'here9: ' + @statement3e
        exec sp_executesql @statement3e


        -----------------
        --update warranty
        ----------------
        set @statement3f = 
            ' update #totals2 '+
            ' set    warranty = isnull((SELECT sum(d.transvalue) '+ 
    	    '                    FROM	delivery d, custacct i, stockitem s, acct a '+ --need stockitem table to check taxrate
        	'                    WHERE	d.datetrans >= convert(VARCHAR,'''+ convert(varchar,@datefrom) +''') '+ 
    	    '                    AND    d.datetrans <= convert(VARCHAR,'''+ convert(varchar,@dateto) +''') '+ --add 1 day to remove time part errors
            '                    AND    d.acctno = #totals2.acctno '+ --join tables
            '                    AND    d.Agrmtno = #totals2.Agrmtno '+ --join tables
            '                    AND    CONVERT(datetime,CONVERT(char(10),d.datetrans,101)) = CONVERT(datetime,CONVERT(char(10),#totals2.dateoccurred,101)) '+ --join tables
            '                    AND    d.notifiedby = #totals2.empeeno '+ --join tables
            '                    AND    d.acctno = i.acctno '+ --join tables
			'                    AND    d.acctno = a.acctno '+ --join tables
            --'                    AND    d.itemno = s.itemno '+ --join tables
            '                    AND    d.ItemID = s.ID '+ --join tables							--IP - 03/08/11 - RI
            '                    AND    d.stocklocn = s.stocklocn '+ --join tables
            '                    AND    (i.custid like ''PAID%'' OR a.accttype = ''S'') '+
            '                    AND    category in (select distinct code from code where category = ''WAR'') '+
                                 @statement1b + ' ),0) '
--            '                    GROUP BY d.acctno, CONVERT(datetime,CONVERT(char(10),datetrans,101)), d.notifiedby),0) '
        --execute statement
--    print 'here10: ' + @statement3f
        exec sp_executesql @statement3f
--select '3f', * from #totals2

        ---------------------------
        --insert into #totals table
        ---------------------------
        set @statement3g = 
            ' insert into #totals (Acctno, Dateoccurred, BranchName, EmpeeNo, '+
    	    '        EmployeeName, Accttype, Accttypegroup, Value, Cancelled, Revised, SalesTax, Balance, warranty, Discount, Securitised, '+
            '        new_count, revise_count, cancel_count) '+
            ' select acctno, Dateoccurred, BranchName, EmpeeNo,  '+
    	    '        '''', ''CG'', ''CG'', sum(Value), 0, 0, sum(SalesTax), 0, sum(warranty), sum(Discount), ''N'', '+
            '        sum(pt_count), 0, 0 '+
            ' from   #totals2 t2 '+
            ' group by acctno, Dateoccurred, BranchName, EmpeeNo '
        --execute statement
        exec sp_executesql @statement3g
--select '3g', * from #totals
    end

    ------------------------------------------------------------------
    --Step 4 - Update remaining columns from other tables/calculations
    ------------------------------------------------------------------
    -----------------
    --Update warranty (not cash and go)
    -----------------
    set @statement4a = 
        '    update #Totals '+
        '    set    warranty = isnull((select sum(i.valueafter)-sum(i.valuebefore)'+ --after - before
        '                       from   lineitemaudit i, stockitem s, agreement ag '+
    	--'                       where  i.itemno = s.itemno '+ --join stockitem
    	'                       where  i.ItemID = s.ID '+ --join stockitem									--IP - 03/08/11 - RI
        '                       and    i.stocklocn = s.stocklocn '+ --join stockitem
        '                       and    s.category in (select distinct code from code where category = ''WAR'') '+ --warranty category
        '                       and    i.acctno = #Totals.acctno '+ --join lineitemaudit
        '                       and    i.acctno = ag.acctno '+ --join lineitemaudit
        '                       and    ag.empeenosale = #Totals.EmpeeNo '+ --join on lineitemaudit
        '                       and    CONVERT(datetime,CONVERT(char(10),i.datechange,101)) = #Totals.Dateoccurred '+ --join lineitemaudit on datechanged, remove timepart
   	    '                       and    i.datechange >= convert(VARCHAR,'''+ convert(varchar,@datefrom) +''') '+ 
   	    '                       and    i.datechange <= convert(VARCHAR,'''+ convert(varchar,@dateto) +''') '+ --add 1 day to remove time part errors
                                @statement1e + ' ),0) '+
        '     where accttype <> ''CG'' '
    --execute statement
--print'here11 ' + @statement4a
    exec sp_executesql @statement4a


    -----------------
    --Update Discount (not cash and go)
    -----------------
    set @statement4b = 
        '    update #Totals '+
        '    set    discount = isnull((select sum(i.valueafter)-sum(i.valuebefore) '+ --after - before
        '                       from   lineitemaudit i, stockitem s, agreement ag '+
    	--'                       where  i.itemno = s.itemno '+ --join stockitem
        '                       where  i.ItemID = s.ID '+ --join stockitem								--IP - 03/08/11 - RI
        '                       and    i.stocklocn = s.stocklocn '+ --join stockitem
        '                       and    s.category in (select code from code where category = ''PCDIS'') '+ --warranty category
        '                       and    i.acctno = #Totals.acctno '+ --join lineitemaudit
        '                       and    i.acctno = ag.acctno '+ --join lineitemaudit
        '                       and    ag.empeenosale = #Totals.EmpeeNo '+ --join on lineitemaudit
        '                       and    CONVERT(datetime,CONVERT(char(10),i.datechange,101)) = #Totals.Dateoccurred '+ --join lineitemaudit on datechanged, remove timepart
   	    '                       and    i.datechange >= convert(VARCHAR,'''+ convert(varchar,@datefrom) +''') '+ 
   	    '                       and    i.datechange <= convert(VARCHAR,'''+ convert(varchar,@dateto) +''') '+ --add 1 day to remove time part errors
                                @statement1e + ' ),0) '+
        '     where accttype <> ''CG'' '
    --execute statement
--print'here12 ' + @statement4b
    exec sp_executesql @statement4b


    ----------------
    --Update Balance (all accttypes)
    ----------------
    set @statement4c = 
        '    update #Totals '+
        '    set    balance = value + revised + cancelled - Salestax '+ --add cancellations as coming out as negative in report
        '    from   Admin.[User] c '+
        '    where  #Totals.empeeno = c.id '
    --execute statement
    exec sp_executesql @statement4c


    ----------------------
    --Update Count columns (not cash and go)
    ----------------------
    --cash and go seperate as need to count agrmtno not acctno

    --new account done on insert

    --cancel
    set @statement4d = 
        '    update #Totals '+
        '    set    cancel_count = -1 '+ --if > 0 then reverse cancel so count = -1
    	'    where  Cancelled > 0 '
    --execute statement
    exec sp_executesql @statement4d

    set @statement4e = 
        '    update #Totals '+
        '    set    cancel_count = 1 '+ --if < 0 then cancel so count = 1
    	'    where  Cancelled < 0 '
    --execute statement
    exec sp_executesql @statement4e

    --revise
    --done revise (ie count = 1)
    --cancel revision (cancel exchange collection notes), so count = -1
    set @statement4f = 
        '    update #Totals '+
        '    set    revise_count = -1 '+ --if < 0 then cancel so count = 1
        '    from   lineitemaudit i, agreement ag '+
        '    where  i.source in (''GRTExchangeRev'') '+ --indicates exchange reversal
        '    and    i.acctno = #Totals.acctno '+ --join on acctno
        '    and    i.acctno = ag.acctno '+ --join on acctno
        '    and    ag.empeenosale = #Totals.EmpeeNo '+ --join on empeeno
        '    and    CONVERT(datetime,CONVERT(char(10),i.datechange,101)) = #Totals.Dateoccurred '+ --join on datechanged, remove timepart
   	    '    and    i.datechange >= convert(VARCHAR,'''+ convert(varchar,@datefrom) +''') '+ 
   	    '    and    i.datechange <= convert(VARCHAR,'''+ convert(varchar,@dateto) +''') '+ --add 1 day to remove time part errors
        @statement1e
    --execute statement
--print'here9 ' + @statement4f
    exec sp_executesql @statement4f


    ----------------------
    --Update Employee name (all accttypes)
    ----------------------
    update #Totals
    set    EmployeeName = c.FullName
    from   Admin.[User] c
    where  #Totals.empeeno = c.Id


    --------------------
    --Update Branch name (all accttypes)
    --------------------
    update #Totals
    set    BranchName = #Totals.BranchName + ' : ' + b.BranchName
    from   branch b
    where  #Totals.branchname = b.branchno


    -----------------
    --Update accttype    
    -----------------
    --Cash and go already done on insert
    --RF with product done on insert of new account, now need to update revisons/cancellations if acct marked as RP
--select '4iabef', * from #totals order by acctno
    set @statement4ia =
        '    update #Totals '+
        '    set    accttypegroup = ''RP'' '+
        '    where  exists (select acctno from #totals t2 '+
        '                   where t2.acctno = #totals.acctno '+
        '                   and accttypegroup = ''RP'') '
    --execute statement
    exec sp_executesql @statement4ia
--select '4iaaft', * from #totals order by acctno

    --RF no products - must be remaining 'R' types
    set @statement4i =
        '    update #Totals '+
        '    set    accttypegroup = ''RN'' '+
        '    where  accttype = ''R'' '+
        '    and    accttypegroup <> ''RP'' '
    --execute statement
    exec sp_executesql @statement4i

    --Cash
    set @statement4j =
        '    update #Totals '+
        '    set    accttypegroup = ''CA'' '+
        '    where  accttype = ''C'' '
    --execute statement
    exec sp_executesql @statement4j

    --Everything else must be HP
    set @statement4k =
        '    update #Totals '+
        '    set    accttypegroup = ''HP'' '+
        '    where  accttypegroup not in (''RN'', ''RP'', ''CA'', ''CG'') '
    --execute statement
    exec sp_executesql @statement4k

    ----------------------------------------
    --Get final results for report on screen - don't need count, only need that for management report 1
    ----------------------------------------
    SET @statement = ''

    SET @statement = ' SELECT DISTINCT BranchName, EmpeeNo, EmployeeName, '
    
    if @rollUpResults = 1               
       begin  
         SET @statement = @statement + ' accttypegroup as "accttype", '+
          ' SUM(ISNULL(value,0)) as "value", '+
          ' SUM(ISNULL(cancelled,0)) as "cancelled", '+ 
          ' SUM(ISNULL(revised,0)) as "Revised", '+
          ' SUM(ISNULL(SalesTax,0)) as "salestax", '+
          ' SUM(ISNULL(balance,0)) as "Balance", '+ 
          ' SUM(ISNULL(warranty,0)) as "supershield", '+
          ' SUM(ISNULL(discount,0)) as "Discount" '+
          ' FROM #totals group by BranchName, EmpeeNo, EmployeeName, accttypegroup ' 
          set @statement = @statement + ' with rollup '
       end
    else
       begin
         SET @statement = @statement + 'accttypegroup as "accttype", ISNULL(value,0) as "value",' +
          'ISNULL(cancelled,0) as "cancelled",ISNULL(revised,0) as "Revised",' +
          'ISNULL(SalesTax,0) as "salestax", ISNULL(balance,0) as "Balance",' + 
          'ISNULL(warranty,0) as "supershield", ISNULL(discount,0) as "Discount"' + 
          'FROM #totals ORDER BY BranchName,EmpeeNo, EmployeeName, accttypegroup'
       end 
        
    exec sp_executesql @statement

    SET @Return = @@ERROR
    RETURN @Return
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
