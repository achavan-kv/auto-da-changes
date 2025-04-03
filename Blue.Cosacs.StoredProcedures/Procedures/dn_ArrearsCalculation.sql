if exists (select * from sysobjects where name = 'dn_arrearscalculation')
drop procedure dn_ArrearsCalculation
go
/* AA created 14 Feb 2006 New .Net procedure for arrears calculation 
AA 06 Mar 2007 - this is the one used by Eod Arrears Calculation*/
create procedure dn_ArrearsCalculation 
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : dn_ArrearsCalculation.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Calculates arrears, updates balance, datenextdue. 
-- Author       : Alex A
-- Date         : 1827
--
-- This procedure will calculate the arrears on accounts.
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  ------------ 
-- 12/08/09 jec  UAT793 Arrears counter not set correctly
-- 16/09/09 ip   UAT798 Recurring Arrears counter was incorrectly being updated even though the account had not been in arrears
--				 for atleast 7 days.
-- 29/10/09 ip   CoSACS Improvement - Max Months In Arrears
-- 03/12/09 ip/jc UAT(798) - correct recurring arrears calc.
-- 24/12/09 ip   UAT(798) - Added ISNULL check
-- 17/02/11 ip   #2996 - LW73174 - Outstanding Bal. & Arrears to reflect zero - Updated the arrears to 0
--				 for accounts that are settled with a balance of 0 where the arrears > 0
-- 11/11/11 ip   #3906 - PaidPcent not updated for a CashLoan account. Included check on transtype code 'CLD' (Cash Loan Disbursement)
-- 16/02/12 jec  Correct Arrears calc for storecard - hopefully?
-- 14/03/12 jc/ip#9794 - Cater for Store Card recurring arrears
-- 20/03/12 ip   #9805 - Arrears for Store Card was not being calculated correctly
-- 28/03/12 ip   #9846 - Customer.Recurringarrears not updated correctly 
-- ===============================================================================================
	-- Add the parameters for the stored procedure here
@rundate  datetime = null,@return int OUT
AS

SET NOCOUNT ON 
-- updating accounts status to where currently settled with balance...

SELECT MAX(statuscode) AS status,a.acctno  , outstbal,isAmortized ,IsAmortizedOutStandingBal 
INTO #accts 
FROM acct a
join status s ON a.acctno = s.acctno
WHERE a.currstatus = 'S' AND ABS(a.outstbal) >1 -- only really concerned about accounts with significant balances - others can be rounding. 
AND s.statuscode NOT IN('S','U','O')
GROUP BY a.acctno,outstbal, isAmortized ,IsAmortizedOutStandingBal

UPDATE #accts 
SET outstbal = (SELECT SUM(transvalue )
FROM fintrans f WHERE f.acctno= #accts.acctno and #accts.isAmortized = 0)

--CR CashLoan Amortizaton, outstbal calculation according to new formula
UPDATE #accts 
SET outstbal = dbo.fn_CLAmortizationCalcDailyOutstandingBalance(f.acctno)
FROM fintrans f WHERE f.acctno= #accts.acctno and #accts.isAmortized = 1 and #accts.IsAmortizedOutStandingBal = 1

UPDATE acct 
SET currstatus = b.status, outstbal = b.outstbal, lastupdatedby= 99999
FROM #accts b
WHERE b.outstbal > 0 
AND b.acctno = acct.acctno 

UPDATE acct 
SET currstatus = '1',outstbal = b.outstbal, lastupdatedby = 99999
FROM #accts  b
WHERE b.outstbal < 0 
AND b.acctno = acct.acctno 

DROP TABLE #accts

SET @return = 0  
     if @rundate is null
       begin
         if (DATEPART(hour,getdate()) <7 )
				SELECT @rundate = dateadd(hour,-1-DATEPART(hour,getdate()),GETDATE());
		else 
				SELECT @rundate = GETDATE()
		END
		

		
     set transaction isolation level read uncommitted
     set nocount on 
     declare @status int ,@arrearsrunno int,@debug smallint
     set @debug = 1
  
    -- call the payment holiday month thing
    exec dbaddpaymentholidaymonth

    select @arrearsrunno =isnull (max (runno), 0) from interfacecontrol where interface = 'Arrears'
/*	set 	@arrearsrunno = @arrearsrunno  + 1                                                                        
		insert into interfacecontrol  (interface, runno, datestart, datefinish, result)
		values ('Arrears',@arrearsrunno,getdate(),'1-jan-1900','')*/

     set nocount  OFF
     --eeff
	 
    /* Heat call 66006 updating settled percentage paid to 100% where not cancelled and not already 100% */
      update acct set paidpcent = 100 where outstbal =0 and currstatus ='S' and
                paidpcent !=100 and agrmttotal> 0 and not exists (select * from  
                cancellation where cancellation.acctno =acct.acctno ) 
       
     
	  update acct
		 set outstbal = (
					select sum(transvalue) 
					from fintrans f
					where f.acctno = acct.acctno
					)
		where currstatus = 'S'
			and outstbal != 0 and isAmortized = 0

        --CR CashLoan Amortizaton, outstbal calculation according to new formula
	  update acct
		 set outstbal = (dbo.fn_CLAmortizationCalcDailyOutstandingBalance(f.acctno))
		from fintrans f where f.acctno = acct.acctno and currstatus = 'S'
			and outstbal != 0 and isAmortized = 1 and IsAmortizedOutStandingBal = 1
				
	 
	 
	 --IP - 17/02/11 - #2996 - LW73174                    
     update acct set arrears = 0
      where outstbal = 0
      and arrears > 0

      
	  update ad
		set dateto = @rundate
		from arrearsdaily ad
		inner join acct a on ad.acctno = a.acctno
		where dateto is null
			and a.currstatus = 'S'
			and a.outstbal = 0
			and a.arrears = 0

    update instalplan
        set instalamount = v.instalment  
    from instalmentvariable v  
    where v.acctno = instalplan.acctno  
    and instalplan.instalamount !=v.instalment  
    and v.datefrom <=getdate()  
    and v.dateto >=getdate()  
                --and v.acctno like @vbranch depending on performance may reinstate this

     CREATE TABLE #arrstable  (acctno char(12) not null primary key, termstype char(2),dateacctopen datetime,
     datelastpaid datetime ,agrmttotal money, accttype char(2), 
     arrears money, outstbal money, paidpcent float, 
     currstatus char(1),transvalue money, transvaluetotal money, deposit money,  
     datenextdue datetime, deltot money,  datedel datetime, 
     instalamount money, datelast datetime,datefirst datetime,
     instalfreq char(2),  amountdue money, amountpaid money, 
     nsd int, di int,  mi int,
     yi int, daynow int, monthnow int, 
     yearnow int , db int,instalpredel char(1),
     balexcharges MONEY ,repovalue MONEY, instalmentwaived bit , isAmortized int,IsAmortizedOutStandingBal int )

     insert into #arrstable
    (acctno , termstype ,dateacctopen ,
     datelastpaid  ,agrmttotal , 
     accttype , arrears , outstbal , paidpcent , 
     currstatus ,transvalue ,transvaluetotal , deposit ,  
     datenextdue, deltot ,  datedel , 
     instalamount , datelast ,datefirst ,instalfreq , 
     amountdue , amountpaid , nsd , 
     di ,  mi ,  yi , 
     daynow , monthnow , yearnow  , db,instalpredel,balexcharges, InstalmentWaived ,isAmortized,IsAmortizedOutStandingBal )

     select    
     a.acctno,a.termstype,a.dateacctopen,
	 a.datelastpaid,a.agrmttotal, a.accttype,
     a.arrears, a.outstbal,a.paidpcent, 
     a.currstatus, 0, 0, g.deposit,  
     g.datenextdue, sum(f.transvalue),  g.datedel, 
     i.instalamount,dateadd(month,i.instalno-1,datefirst),i.datefirst,
     i.instalfreq, 0, 0, 
     0 , 0 , 0 , 
     0 , 0 , 0 , 
      0 , 0 ,  'N',a.outstbal ,i.InstalmentWaived ,a.isAmortized,a.IsAmortizedOutStandingBal    
     from acct a, agreement g, instalplan i ,fintrans f
     where g.acctno = a.acctno and i.acctno = a.acctno 
     and g.agrmtno = 1 and i.agrmtno = 1 
     and not (a.currstatus ='S' and outstbal = 0)
     and f.acctno =a.acctno and f.transtypecode in ('DEL','GRT','ADD','CLD')						--#3906
	 and a.accttype != 'T'
     -- fudge probably a liverwire issue to say that you cannot have an expired account which 
     -- was opened less than 3 months ago - suggests the datelast has not been set properly  
     group by      a.acctno,a.termstype,a.dateacctopen,
     a.datelastpaid,a.agrmttotal, a.accttype,
     a.arrears, a.outstbal,a.paidpcent, 
     a.currstatus,  g.deposit,  
     g.datenextdue,g.datedel, 
     i.instalamount,i.instalno,i.datefirst,
     i.instalfreq,i.InstalmentWaived
	 ,a.isAmortized,a.IsAmortizedOutStandingBal 


     DECLARE @daynow TINYINT, @monthnow TINYINT
     SET @daynow = DATEPART(DAY,@rundate)
     SET @monthnow= DATEPART(MONTH,@rundate) 
	/*adding extra 3 days for accounts due on 29/30/31st so go into next month*/
	if @daynow = 1 -- 1st of Month
	BEGIN
		IF  @monthnow =3 -- 1st March
		BEGIN
			 update  #arrstable
			 SET datefirst = DATEADD(DAY,3,datefirst) --Just need to get it into next month so it doesn't go into arrears'
			WHERE DATEPART(DAY,datefirst) IN (29,30,31) 
		END 
		
		IF @monthnow IN (5,7,10,12) -- 30 days hath september...
		BEGIN 
			update  #arrstable
			SET datefirst = DATEADD(DAY,1,datefirst) 
			WHERE DATEPART(DAY,datefirst) =31
		END
	END


     
     update acct set arrears = 0 where dateacctopen > dateadd (month, - 3, getdate())  
     and arrears = outstbal 

    
     update t set paidpcent =  isnull(a.paidpcent,0)
     from acct a  ,#arrstable   t
     where a.acctno =   t.acctno 
    
    /* below setting delivery dates and date of  first instalments for all those accounts which have
           not has theirs set yet */
    declare @acctno char(12),@percentagefordeliverydate float,@counter int
    set @counter = 0
    select @percentagefordeliverydate=globdelpcent from country

	EXEC dbdatenextdueALL @currentdate = @rundate  -- calculate datenext due in bulk for all accounts... 
	
	
    DECLARE acct_cursor CURSOR 
  	FOR SELECT acctno
        from #arrstable t
	 where
        (datedel is null or 
         datelast is null or
         datelast <'01-jan-1910' or datedel = '01-jan-1900' or
         datefirst ='01-jan-1900'
         
         )
	 and exists ( select * from fintrans f where f.acctno = t.acctno
         and f.transtypecode ='DEL')

    
   OPEN acct_cursor
   FETCH NEXT FROM acct_cursor INTO @acctno

   WHILE (@@fetch_status <> -1)
  BEGIN
       IF (@@fetch_status <> -2)
       BEGIN
	    exec  dbarrearscalc @acctno =@acctno,
                                @countpcent = @percentagefordeliverydate
            if @debug = 1
            begin
               set @counter = @counter +1
               if @counter % 100 = 0 
                  print 'setting delivery dates... ' + convert(varchar,@counter)
            end
       END
       FETCH NEXT FROM acct_cursor INTO @acctno
   END
   CLOSE acct_cursor
   DEALLOCATE acct_cursor


    /* now unsetting delivery dates for those who have had it
           set, but have now had their delivery amt < percentage 
            of agrement total 
    status =
    execute procedure dbremovedeliverydates(branchno = branchno)
    */

    update   a 
    set instalpredel =  t.instalpredel  
    from termstype t   ,#arrstable  a
    where t.termstype =   a.termstype

--    exec UpdateEodStatus @interface='Arrears'
        
    update #arrstable set deposit = instalamount where instalpredel = 'Y' and
    deposit < 0.01 and InstalmentWaived = 0

--    exec UpdateEodStatus @interface='Arrears'

    update   a   set  
    transvalue =isnull((select sum(f.transvalue)  
    from fintrans f  
    where f.acctno =a.acctno and f.datetrans <@rundate),0)
    from #arrstable  a where a.isAmortized=0
	
    --CR CashLoan Amortizaton, transvalue calculation according to new formula which will be updated in outstbal for accounts
	update   a   set  
    transvalue =isnull(dbo.fn_CLAmortizationCalcDailyOutstandingBalance(f.acctno),0)
    from #arrstable  a , fintrans f  
    where f.acctno =a.acctno and f.datetrans <@rundate 
	and a.isAmortized=1 and a.IsAmortizedOutStandingBal=1
	
	--CR CashLoan Amortizaton, added transvaluetotal as a new column in #arrstable which will add total oustanding amount for that account
	--and not as per new formula as it is required in arrear calculation ahead where we need total amount
	update   a   set  
    transvaluetotal =isnull((select sum(f.transvalue)  
    from fintrans f  
    where f.acctno =a.acctno and f.datetrans <@rundate),0)
    from #arrstable  a where a.isAmortized=1 and a.IsAmortizedOutStandingBal=1  
    
	update   a   set  
    balexcharges =isnull((select sum(f.transvalue)  
    from fintrans f  
    where f.acctno =a.acctno and f.datetrans <@rundate and f.transtypecode not in ('INT','ADM')),0)
    from #arrstable  a   where a.isAmortized=0 

	--CR CashLoan Amortizaton, balexcharges calculation according to new formula
	update   a   set  
    balexcharges =(isnull(dbo.fn_CLAmortizationCalcDailyOutstandingBalance(f.acctno),0)- isnull((select sum(f.transvalue)  
    from fintrans f where f.acctno =a.acctno and f.datetrans <@rundate and f.transtypecode in ('INT','ADM')),0))
    from #arrstable  a , fintrans f  
    where f.acctno =a.acctno and f.datetrans <@rundate 
	and a.isAmortized=1 and a.IsAmortizedOutStandingBal=1
    
--    exec UpdateEodStatus @interface='Arrears'

    update   a   set  
    amountpaid =isnull((select sum(f.transvalue)  
    from fintrans f 
    where f.acctno =a.acctno and f.datetrans <@rundate
    and transtypecode not in  ('DEL','GRT','REP','ADD','RPO','RDL','CLD')),0)				--#3906
    from #arrstable  a  


    select max(datestatchge) as datestatchge,' ' as status, acctno
    into #temp_statchge  
    from status s where  
    acctno in  
    (select a.acctno from acct a, instalplan i where  
    ((a.currstatus in ('','U') and a.outstbal > 0) or  
    (a.currstatus in ('S','0') and a.outstbal > 0 and a.arrears > i.instalamount))  
     --and a.acctno like  @vbranch not restricting by branch
     and i.acctno = a.acctno ) 
     and statuscode in ('1','2','3','4','5','6','7')  
     group by acctno 
    
     update #temp_statchge   
     set status = s.statuscode from status s  
     where #temp_statchge.acctno = s.acctno and  
     #temp_statchge.datestatchge = s.datestatchge
    
     update acct  set currstatus =t.status, lastupdatedby = 99997  
     from #temp_statchge t where  
     acct.acctno = t.acctno
          


     update  #arrstable 
     set agrmttotal = 0 where 
     agrmttotal IS NULL and acctno not like '___5%' 
        
	 alter TABLE #arrstable add intfeestot money
	-- pai
	 UPDATE #arrstable 
	 SET intfeestot = ISNULL((SELECT SUM(transvalue)FROM fintrans f 
	 WHERE f.acctno= #arrstable.acctno 
	 AND f.transtypecode IN ('INT', 'ADM','FEE')),0)
      
     update  #arrstable 
     set paidpcent = isnull(convert(float,100 *((-amountpaid-intfeestot)/agrmttotal) + .51),0)   
      where agrmttotal > 0 and (100*(-amountpaid/agrmttotal) + .51) < 101 and acctno not like 
       '___5%' AND outstbal >0
    
     update acct set paidpcent =isnull(t.paidpcent,0) 
     from  #arrstable  t  
     where  
     acct.acctno = t.acctno and 
     t.paidpcent >= 0 and  
     t.paidpcent != acct.paidpcent and  
     t.paidpcent is not null 

    delete from #arrstable where datedel <= '1-jan-1910' or datedel is null  
    or datelast < '1-jan-1910' or datelast is null  
  
  
  
    delete from #arrstable where datedel <= '1-jan-1910' or datedel is null
    or datelast < '1-jan-1910' or datelast is null


    declare /*Declared earlier @daynow smallint,@monthnow smallint ,*/@yearnow smallint
    set @daynow=datepart(day,@rundate)
    set @monthnow=datepart(month,@rundate)
    set @yearnow=datepart(year,@rundate)

    update #arrstable  set di=datepart(day,datefirst), 
                   mi=datepart(month,datefirst),
                   yi=datepart(year,datefirst),
                   daynow= @daynow ,
                   monthnow= @monthnow,
                   yearnow= @yearnow
     
    update #arrstable set db = 1 where daynow-di >0  /* Today has passed the due date */  

    /* updating number of instalments due nsd based on days and months elapsed-most accounts are monthly*/   
    update #arrstable  set nsd=12*(yearnow-yi)+monthnow-mi+db where instalfreq !='B'

    /* these accounts have two instalments due a year*/
    update #arrstable  set nsd=2*(yearnow-yi)+(5 + monthnow-mi)/6 + db where instalfreq ='B' and @rundate > datefirst
    
    update #arrstable set nsd = 0 where nsd <0    
 
    update #arrstable set  amountdue =(nsd * instalamount) + deposit
 
  --  exec UpdatefromvariableSP(#arrstable = #arrstable, vbranch =vbranch )
--sp_helptext UpdatefromvariableSP
    update #arrstable set arrears = amountdue + amountpaid where 
        arrears   !=(amountdue + amountpaid)
 
         
    
   -- arrears can never exceed oustanding balance AA -8 -jul -2010 unless repo on there... 
   UPDATE #arrstable SET repovalue = (SELECT SUM(transvalue ) FROM fintrans f WHERE f.acctno = #arrstable.acctno
   AND f.transtypecode IN ('rep','rdl'))

   UPDATE	#arrstable 
   SET		arrears = transvalue - ISNULL(repovalue,0) 
   WHERE	arrears >(transvalue - ISNULL(repovalue,0))
			and isAmortized = 0

   UPDATE	#arrstable 
   SET		arrears = transvaluetotal - ISNULL(repovalue,0) 
   WHERE	arrears >(transvaluetotal - ISNULL(repovalue,0))
			and isAmortized = 1 and IsAmortizedOutStandingBal=1
   
    /* for those non amortized accounts which have passed their datelast arrears just is their balance */
   update  a   set arrears = transvalue, 
           outstbal = transvalue 
  from accttype  ,#arrstable a
   where dateadd(month,mthsdeferred,datelast) < CAST(
FLOOR( CAST( @rundate  AS FLOAT ) )
AS DATETIME)--this removes the time from the rundate so accounts are not marked past due prematurely
   and (arrears !=transvalue or outstbal !=transvalue) 
   and Datelast > '01-jan-1910' 
   and  a.accttype = accttype.accttype and a.isAmortized =0 

      /* for those amortized accounts which have passed their datelast arrears just is their balance */
   update  a   set arrears = transvaluetotal, 
           outstbal = transvalue 
  from accttype  ,#arrstable a
   where dateadd(month,mthsdeferred,datelast) < CAST(
FLOOR( CAST( @rundate  AS FLOAT ) )
AS DATETIME)--this removes the time from the rundate so accounts are not marked past due prematurely
   and (arrears !=transvaluetotal or outstbal !=transvalue) 
   and Datelast > '01-jan-1910' 
   and  a.accttype = accttype.accttype and a.isAmortized =1 and a.IsAmortizedOutStandingBal = 1
        
   update acct  
   set outstbal = t.transvalue, 
   arrears = t.arrears  ,
   as400bal = t.balexcharges
   from  #arrstable  t 
   where acct.acctno = t.acctno and 
   (acct.outstbal !=t.transvalue or acct.arrears !=t.arrears or as400bal !=t.balexcharges)
    
     -- arrears daily used to calculate charges in letters and charges based on daily movement of arrears
   update ArrearsDaily  set dateto = dateadd(minute,-1,@rundate)
   from   #arrstable  t
   where t.acctno = arrearsdaily.acctno and arrearsdaily.dateto is null and 
   (arrearsdaily.arrears >0 or t.arrears >0) and  
   exists (select * from  ArrearsDaily d   
   where d.acctno =t.acctno and d.arrears !=t.arrears and d.dateto is null)
   
   -- getting number of months to keep history for storing recurring arrears for collections process
   declare @numberofmonths int
   select @numberofmonths=isnull(max(c.Operator2),3) from CMStrategyCondition C, CMStrategy S 
   where  C.Condition ='recarrs'
   and S.Strategy = c.Strategy and S.IsActive =1

   if @numberofmonths <3 
        set @numberofmonths=3
   -- removing old arrears movement from arrears daily where >60 days old 
   delete from arrearsdaily where dateto< dateadd (month, -@numberofmonths, getdate())   
   or dateto <datefrom 
    
   insert into ArrearsDaily ( acctno, arrears,datefrom,dateto)   
   select acctno,arrears,    @rundate    , NULL  -- the current arrears is marked by dateto null
   from    #arrstable   t  
   where not exists (select * from  ArrearsDaily d      
   where d.acctno =t.acctno and d.arrears =t.arrears and d.dateto is null) and t.arrears > 0 
   
   
   -- with this      -- Jec  UAT793
	select  d.acctno,ca.custid,count(*)  as recarrs  
	into #recurArrears
	from ArrearsDaily d inner join instalplan ip on d.acctno=ip.acctno					
					inner join custacct ca on ca.Acctno=d.acctno and ca.hldorjnt='H'
	 where ( (  --d.dateto is null and 
				dateadd(day,7,d.datefrom) <getdate()	-- current arrears
					and d.arrears between .5 * ip.instalamount and 1.5 * ip.instalamount)
		-- but only if account is at least in arrears for at least seven days so if arrears cleared within 7 days then don't count'
		and (not exists (select * from ArrearsDaily G  --inner join instalplan i  on g.acctno=i.acctno 				
					where  g.acctno=d.acctno 
					and G.arrears < .5* ip.instalamount 
					AND g.datefrom = (SELECT Max(x.datefrom) FROM ArrearsDaily x WHERE x.Acctno = g.Acctno AND x.datefrom> d.datefrom ) --IP/JC - 03/12/09 - UAT(798) 
					and d.datefrom <dateadd(day,7,g.datefrom)
					)
			
			-- previous arrears was less than half an instalment in arrears
		AND EXISTS 	(SELECT * FROM ArrearsDaily r WHERE r.Acctno = d.Acctno 
		AND r.datefrom = (SELECT MAX(datefrom) FROM ArrearsDaily m WHERE m.datefrom <d.datefrom AND m.Acctno= d.Acctno)
		AND r.arrears < .5 *ip.instalamount  )
			)
			
		and d.datefrom> DATEADD(yy,-2,getdate())  -- within the last 2 years
		)	
	group by ca.custid,d.acctno
	
	--14/03/12 - #9794 - JC/IP - Cater for Store Card recurring arrears
	insert	into #recurArrears (acctno,custid, recarrs)
	select d.acctno, ca.custid, count(*)
	from ArrearsDaily d inner join storecardpaymentdetails sp on d.acctno = sp.acctno					
					inner join custacct ca on ca.Acctno=d.acctno and ca.hldorjnt='H'					
	 where ( 
			(  
				dateadd(day,7,d.datefrom) <getdate()	-- current arrears
					and d.arrears > 0)
		-- but only if account is at least in arrears for at least seven days so if arrears cleared within 7 days then don't count'
		and (not exists (select * from ArrearsDaily G  --inner join instalplan i  on g.acctno=i.acctno 				
					where  g.acctno=d.acctno 
					and G.arrears <=0 
					AND g.datefrom = (SELECT Max(x.datefrom) FROM ArrearsDaily x WHERE x.Acctno = g.Acctno AND x.datefrom> d.datefrom ) 
					and d.datefrom <dateadd(day,7,g.datefrom)
					)
			
			-- previous arrears was less than half an instalment in arrears
		AND EXISTS 	(SELECT * FROM ArrearsDaily r WHERE r.Acctno = d.Acctno 
		AND r.datefrom = (SELECT MAX(datefrom) FROM ArrearsDaily m WHERE m.datefrom <d.datefrom AND m.Acctno= d.Acctno)
		AND r.arrears <=0  )
			)
			
		and d.datefrom> DATEADD(yy,-2,getdate())  -- within the last 2 years
		)	
	group by ca.custid,d.acctno

	--IP/JC - 03/12/09 - UAT(798) 
	--Insert 0 where arrears/account up to date for 6 months
	INSERT INTO #recurArrears 
	SELECT  d.acctno,ca.custid,0
	from ArrearsDaily d inner join instalplan ip on d.acctno=ip.acctno					
					inner join custacct ca on ca.Acctno=d.acctno and ca.hldorjnt='H'
	--earliest date not in arrears later than......
	WHERE d.datefrom = (SELECT MIN(ad.datefrom)  FROM dbo.ArrearsDaily ad
					WHERE ad.acctno = d.Acctno
					AND ad.arrears < .5* ip.instalamount
	-- ....most recent date in arrears				
	AND ad.datefrom >	(SELECT ISNULL(MAX(ad2.datefrom), '1900-01-01') FROM dbo.ArrearsDaily ad2 -- 24/12/09 - UAT(798) - Added ISNULL check
					WHERE ad.acctno = ad2.Acctno
					AND ad2.arrears > .5* ip.instalamount)
	)
	AND d.datefrom <= DATEADD(m,-6,GETDATE())
	
	 -- now update Customer  
	 update customer
		set recurringarrears=isnull(recarrs,0)
	 from customer c left join #recurArrears r on c.custid=r.custid
	 where isnull(recarrs,99)>ISNULL(recurringarrears,0)					--JC/IP - 14/03/12 - #9794 - Customers who do no longer have recurring arrears
			and (recurringarrears != 0 or (recurringarrears = 0 and isnull(recarrs,99)<99))  --IP - 28/03/12 - #9846
		OR isnull(recarrs,99) = 0 -- 6 months not in arrears
	  

   select  
   a.acctno,a.outstbal,convert(money,0) as delvalue, convert(money,0) as payvalue, 
   isnull(arrears,0) as arrears,isnull(paidpcent,0) as paidpcent  
   into   #casharrs 
   from acct a where acctno like    '___4%'
   and not (a.outstbal = 0 and a.arrears = 0 and a.currstatus ='S')

    
   create clustered index ix_tempcash_acctno1 on #casharrs (acctno) 

   update   #casharrs    set  
   payvalue = isnull(( select sum(transvalue) from fintrans f where f.acctno =   #casharrs.acctno 
   and transtypecode not in ('DEL','GRT','REP','ADD','RPO','RDL','CLD') ),0)									--#3906

   update   #casharrs   set  
   delvalue = isnull(( select sum(transvalue) from fintrans f where f.acctno =   #casharrs.acctno 
   and transtypecode in ('DEL','GRT','REP','ADD','RPO','RDL','CLD') ),0)										--#3906

 

   --cash -typically company accounts can have a special extended credit period
   -- as a an account code against them
   declare @code varchar(4),@rdate datetime
   select @code = code from code where code like 'CD%' and category like 'AC%'
   
   if @code !='' and @code is not null 
   begin
     set @daynow = convert(int,(right(isnull(@code,''),2)))
     if @daynow !=0
     begin 
        set @rdate = @rundate - dateadd(day, -@daynow,@rundate)
            
        select acctno, convert(money,0) as deliveriesdue into #extracashperiod
        from acctcode where  code =@code
       
        update #extracashperiod set deliveriesdue = isnull(f.transvalue,0) 
        from fintrans f, #extracashperiod t 
        where t.acctno =f.acctno and  
        f.transtypecode in ('DEL','GRT','REP','ADD','RPO','RDL','CLD') and											--#3906
        f.datetrans <= @rundate

        update   #casharrs 
        set delvalue = isnull(deliveriesdue,0)  
        from #extracashperiod a  
        where   #casharrs.acctno = a.acctno
      end            
   end

     declare @chargesrunno int, @maxdate datetime
     select @maxdate = max(datestart) from interfacecontrol where interface = 'CHARGES'
     SELECT @chargesrunno= isnull(runno ,0)
     from interfacecontrol where interface = 'CHARGES' and datestart = @maxdate

    /* Now updating the letters and charges table with the arrears that the
    accounts will be charged on which is the
    arrears not taking into account all deliveries in the past month.*/
    if @chargesrunno > 0
    begin
          select f.acctno, sum(f.transvalue) as delvalue, convert(money,0) as outstbal 
          into   #casharrears
          from fintrans f
          inner join acct a 
          on f.acctno = a.acctno
          where transtypecode in ('DEL', 'GRT','CLD')										--#3906
              and f.datetrans > dateadd(month,-1,getdate())
              and a.accttype = 'C'
              and a.currstatus !='S'
          group by f.acctno   

        
	 --change for CR CashLoan Amortizaton
	 update   #casharrears     set outstbal = (select sum(transvalue) from fintrans f where f.acctno = #casharrears.acctno )
         from acct a where a.acctno =  #casharrears.acctno
         
        
         update chargesdata   
         set afterarrears = s.outstbal - s.delvalue  
         from   #casharrears  s    
         where chargesdata.acctno =s.acctno and   
         chargesdata.arrears <> s.outstbal - s.delvalue and   
         chargesdata.runno =@chargesrunno
       
    end

    update  #casharrs  
    set outstbal = isnull((select sum(f.transvalue)
    from fintrans f
    where f.acctno =   #casharrs.acctno ) ,0) 

		--CR for amortized cash loan
	update  #casharrs  
	set outstbal = isnull(dbo.fn_CLAmortizationCalcDailyOutstandingBalance(f.acctno),0)
	from fintrans f , acct a
	where f.acctno =   #casharrs.acctno and a.acctno =  #casharrs.acctno 
	and a.isAmortized = 1 and a.IsAmortizedOutStandingBal = 1

    update #casharrs set delvalue = -0.01 where delvalue is null
    update #casharrs set payvalue = 0 where payvalue is null
        
    update   #casharrs   set paidpcent = 
         isnull(convert(integer,(-100 * (payvalue/delvalue) + .51 )),0)  
         where delvalue > 1 and payvalue <-1 and delvalue is not null 
      and payvalue is not null and  
         isnull(convert(integer,(-100 * (payvalue/delvalue) + .51 )),0) < 150 
        
    update #casharrs set arrears = delvalue + payvalue
        where arrears !=delvalue + payvalue

     update acct
     set arrears = t.arrears,   
         outstbal = t.outstbal,  
         paidpcent = isnull(t.paidpcent,0)   
     from    #casharrs    t   
     where acct.acctno = t.acctno and   
          (acct.arrears !=t.arrears or   
           acct.outstbal !=t.outstbal or   
           acct.paidpcent !=t.paidpcent) 



	  /*
	  calculate for store card accounts
	  */
   select  
   a.acctno,a.outstbal,sum(transvalue) as delvalue, convert(money,0) as payvalue, 
   isnull(arrears,0) as arrears  
   into   #SCAccts 
   from acct a
   inner join fintrans f
		on f.acctno = a.acctno
		and f.transtypecode in ('SCT')
   where a.acctno like    '___9%'
		and not (a.outstbal = 0 and a.arrears = 0 and a.currstatus ='S')
   group by a.acctno, a.outstbal, a.arrears

   
	  /*
	  calculate payments store card accounts
	  */
    update   #SCAccts    set  
   payvalue = isnull(( select sum(transvalue) from fintrans f where f.acctno =   #SCAccts.acctno 
   and transtypecode not in ('DEL','GRT','REP','ADD','RPO','RDL','CLD', 'SCT') ),0)
   
   
	  /*
	  calculate arrears for store card accounts
	  */

   	Update	#SCAccts 
		set arrears =  isnull(CASE WHEN datedue> GETDATE()
					--THEN outstminpay+payments
					--ELSE currminpay+outstminpay+payments
					--THEN prevminpay+payments					-- jec 16/02/12
					--ELSE currminpay+payments  
					THEN isnull(outstminpay, 0) + payments		--IP - 20/03/12 - #9805
					ELSE currminpay + payments
					END , 0)
	From #SCAccts s ,vw_storecard_arrears a	
	--WHERE prevminpay > 0
	--	and s.acctno=a.acctno	
	 WHERE s.acctno=a.acctno			-- jec 16/02/12


	  /*
	  set arrears to 0 if < 0
	  */

	update #scaccts
	set arrears = 0
	where arrears < 0

	
	  /*
	   update acct for store card accounts
	  */

	update a 
	set arrears = s.arrears
	from acct a
	inner join #scaccts s 
		on s.acctno = a.acctno

		
	  /*
	  update arrears daily for store card accounts
	  */

	 update ArrearsDaily  set dateto = dateadd(minute,-1,@rundate)
	 from   #scaccts  t
	 where t.acctno = arrearsdaily.acctno and arrearsdaily.dateto is null and 
		t.arrears != arrearsdaily.arrears


		
	  /*
	  insert into arrears daily for store card accounts
	  */

		insert into arrearsdaily (Acctno,arrears,datefrom,dateto)
		select acctno, arrears, @rundate, null
		from #scaccts a
		where not exists(
					select * from arrearsdaily d
					where d.acctno = a.acctno
					and d.dateto is null
					)
		and a.arrears > 0


	--IP - 29/10/09 - CoSACS Improvements - Max Months In Arrears
	--Update the Maximum months in arrears only if the months in arrears > Maximum months in arrears
	UPDATE acct 
	SET MaxMonthsInArrears = CONVERT(INT, MonthsInArrears)
	WHERE CONVERT(INT, MonthsInArrears) > MaxMonthsInArrears
	
  return @return  


GO 


