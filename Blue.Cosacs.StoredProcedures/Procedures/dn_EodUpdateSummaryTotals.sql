IF EXISTS (SELECT 'A' FROM SYS.PROCEDURES WHERE NAME = 'dn_EodUpdateSummaryTotals')
	DROP PROC dn_EodUpdateSummaryTotals
GO
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : dn_EodUpdateSummaryTotals.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Updates Summary Totals
-- Author       : ??
-- Date         : ??
--
--
-- 
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 04/12/09  IP UAT(566) - Typo caused incorrect 'Reopened Accounts' figure to be displayed in Summary Update Control Report
-- 17/02/12  IP #9423 - CR8262 - Include StoreCard deliveries. Split out from Cash Deliveries.
-- 11/04/12  jec #9886 CR9863 - int and adm interface - Storecard INT/ADM interfaced immediately
-- 05/10/12  jec #10138 - LW75030 - SUCR - Cash Loan 
-- 25/06/19  Rahul  #     - Update outsatnding balance for Cash Loan Account 
-- ================================================

create procedure dn_EodUpdateSummaryTotals 
		@runno int,@return int out as
declare  @datepreviousrun datetime,@daterun datetime
set @return = 0
declare @refresh int
set @refresh =0  -- use 1 for testing purposes to compare against current data to see differences

CREATE TABLE #interfacevalue (
	interface varchar (10)  NOT NULL ,
	runno int NOT NULL ,
	counttype1 varchar (25)  NOT NULL ,
	counttype2 varchar (10)  NOT NULL ,
	branchno int NOT NULL ,
	accttype varchar (10)  NOT NULL ,
	countvalue int NOT NULL ,
	value money NOT NULL )
-- this is to store totals in
CREATE TABLE #interfacevalueCase (
	interface varchar (10)  NOT NULL ,
	runno int NOT NULL ,
	counttype1 varchar (25)  NOT NULL ,
	counttype2 varchar (10)  NOT NULL ,
	branchno int NOT NULL ,
	accttype varchar (10)  NOT NULL ,
	countvalue int NOT NULL ,
	value money NOT NULL )

declare @test varchar(4)
set nocount on
set @test = 'FALSE'

if @test !='True'
update acct set outstbal = f.value from
ftview f 
where f.acctno= acct.acctno and outstbal != f.value and IsAmortizedOutStandingBal != 1 -- Update outstanding for Normal account 
--and not (outstbal = 0 and currstatus ='S') --KEF 69090 Removed restriction to match what Management Report is doing, otherwise can still get discrepencies with closing balances

update acct set outstbal = dbo.fn_CLAmortizationCalcDailyOutstandingBalance (acct.acctno) -- #CLA - Rahul D -  Update outstanding for CLA outstanding calc account
from
ftview f 
where f.acctno= acct.acctno and outstbal != f.value and IsAmortizedOutStandingBal = 1 and isAmortized =1 -- Update outstanding for CLA outstanding calc account

declare @selectrunno int

if @refresh = 0 -- new run
   set @selectrunno = 0
else
   set @selectrunno = @runno
--   delete from interfacevalue where runno= @runno and interface ='UPDSMRY'

select @datepreviousrun = datestart from interfacecontrol where interface ='updsmry'
and runno = @runno -1

select @daterun = datestart from interfacecontrol where interface ='updsmry'
and runno = @runno 
   truncate table fintransLastSummary
   insert into fintransLastSummary (acctno,branchno, transvalue,
   transtypecode,empeeno, ftnotes,
   runno, source,datetrans,
   transrefno,paymethod)
   select f.acctno,f.branchno, f.transvalue,
   f.transtypecode,f.empeeno, f.ftnotes,
   @runno, f.source,f.datetrans,
   f.transrefno,f.paymethod
   from fintrans f,acct a where f.runno = @selectrunno
   and a.acctno = f.acctno
   --and not (f.transtypecode in ('INT','ADM') and a.currstatus !='S') -- excluded int charges for settled accounts
   and (not(f.transtypecode in ('INT','ADM') and a.currstatus != 'S') or a.AcctType ='T')		-- #9886 
       
   select a.acctno,convert(smallint,left(a.acctno,3)) as branchno, f.transvalue,
   f.transtypecode,convert(varchar(10),'') as accttype,source
   into #fintrans 
   from fintransLastSummary f,acct a where 
   --f.runno = 0 and 
   a.acctno = f.acctno
   --and not (f.transtypecode in ('INT','ADM') and a.currstatus !='S') -- excluded int charges for settled accounts
   and (not(f.transtypecode in ('INT','ADM') and a.currstatus != 'S') or a.AcctType ='T')		-- #9886 
   
   update #fintrans set accttype ='SPECIAL' where acctno like '___5%'
   update #fintrans set accttype ='CASH' where acctno like '___4%'
   update #fintrans set accttype ='HP' where acctno like '___0%'
   update #fintrans set accttype ='STORECARD' where acctno like '___9%'

   /*69470 There are some legacy special accounts with a cash account number with 4th digit 4 */ 
   update #fintrans set accttype ='SPECIAL'
   FROM acct a WHERE a.acctno = #fintrans.acctno AND a.accttype ='S'

   /* bring forward the previous balances */
   insert into #interfacevalue 
       (interface,runno,counttype1,counttype2,
        branchno,accttype,countvalue,value)
   select
       'UPDSMRY',@runno,'ACCOUNTS B/F','',
        branchno,accttype,countvalue,value
   from interfacevalue
   where interface = 'UPDSMRY' 
   and runno = (@runno -1) 
   and counttype1 ='ACCOUNTS C/F'
  
   insert into #interfacevalue 
       (interface,runno,counttype1,counttype2,
        branchno,accttype,countvalue,value)
   select
       'UPDSMRY',@runno,'BALANCE B/F','',
        branchno,accttype,countvalue,value
   from interfacevalue
   where interface = 'UPDSMRY' 
   and runno = (@runno -1) 
   and counttype1 ='BALANCE C/F'
  insert into #interfacevalue 
       (interface,runno,counttype1,counttype2,
        branchno,accttype,countvalue,value)
   select
       'UPDSMRY',@runno,'RECEIPT',source,
        branchno,accttype,0,SUM(transvalue)
   from #FINTRANS
   where transtypecode in ('COR','PA1','PA2','PA4','PAY','REF','DDE','DDN','DDR','DPY','RET')
   group by accttype,source,branchno

   update #interfacevalue
   set countvalue = 
   ISNULL((select count(*)  
     from #FINTRANS f
     where transtypecode in ('COR','PA1','PA2','PA4','PAY','REF','DDE','DDN','DDR','DPY','RET')
    and f.accttype =#interfacevalue.accttype and f.branchno = #interfacevalue.branchno 
    and f.source =#interfacevalue.counttype2 ),0)
   where counttype1='RECEIPT'      
   insert into #interfacevalue 
       (interface,runno,counttype1,counttype2,
        branchno,accttype,countvalue,value)
   select
       'UPDSMRY',@runno,'DELIVERY',source,
        branchno,accttype,0,SUM(transvalue)
   from #FINTRANS
   where transtypecode in ('ADD','GRT','DEL','REB','RPO','REP','RDL','CLD')			-- #10138
   group by accttype,source,branchno
   
    --IP - 17/02/12 - #9423 - CR8262
   --First select the sum of deliveries on cash accounts that have had a payment made from a storecard account
   
   ;WITH StoreCardDelAccts
   AS
   (
	select distinct v.transferaccount as acctno
	from view_FintranswithTransfers v
	inner join storecard sc on v.acctno = sc.acctno
	inner join acct a on a.acctno = v.transferaccount
	and a.accttype = 'C'
	and v.code = 'SCT'
   )
	
   select sum(transvalue) as value, count(*) as countvalue, branchno, source
   into #StoreCardDels
   from #FINTRANS f inner join StoreCardDelAccts s on f.acctno = s.acctno
   where transtypecode in ('ADD','GRT','DEL','REB','RPO','REP','RDL','CLD')			-- #10138
   and f.accttype = 'CASH'
   group by source, branchno
   

   update #interfacevalue
   set countvalue = 
   ISNULL((select count(*)  
     from #FINTRANS f
     where transtypecode in ('ADD','GRT','DEL','REB','RPO','REP','RDL','CLD')			-- #10138
    and f.accttype =#interfacevalue.accttype and f.branchno = #interfacevalue.branchno 
    and f.source =#interfacevalue.counttype2 ),0)
   where counttype1='DELIVERY' 
    
   --IP - 17/02/12  #9423 - CR8262
   --then subtract the sum of deliveries and countvalue for StoreCard from the CASH DELIVERY total
   update #interfacevalue
   set value = #interfacevalue.value - #StoreCardDels.value,
	   countvalue = #interfacevalue.countvalue - #StoreCardDels.countvalue
   from #StoreCardDels 
   where accttype = 'CASH'
   and counttype1 = 'DELIVERY' 
   and #interfacevalue.branchno = #StoreCardDels.branchno
   and #interfacevalue.counttype2 = #StoreCardDels.[source]
   
   --insert the delivery amounts for StoreCard
   insert into #interfacevalue 
       (interface,runno,counttype1,counttype2,
        branchno,accttype,countvalue,value)
   select 'UPDSMRY',@runno,'DELIVERY',#StoreCardDels.[source],
        #StoreCardDels.branchno,'STORECARD',#StoreCardDels.countvalue, #StoreCardDels.value
   from #StoreCardDels
             
   insert into #interfacevalue 
       (interface,runno,counttype1,counttype2,
        branchno,accttype,countvalue,value)
   select
       'UPDSMRY',@runno,'ADJUSTMENT',source,
        branchno,accttype,0,SUM(transvalue)
   from #FINTRANS
   where transtypecode not in ('ADD','GRT','DEL','REB','RPO','REP','RDL','CLD',					-- #10138	
    'COR','PA1','PA2','PA4','PAY','REF','DDE','DDN','DDR','DPY','RET','INT','ADM','SCT')					--IP - 17/02/12 - #9423 - CR8262
   group by accttype,source,branchno

   update #interfacevalue
   set countvalue = 
   ISNULL((select count(*)  
     from #FINTRANS f
     where transtypecode  not in ('ADD','GRT','DEL','REB','RPO','REP','RDL','CLD',					-- #10138
    'COR','PA1','PA2','PA4','PAY','REF','DDE','DDN','DDR','DPY','RET','INT','ADM','SCT')					--IP - 17/02/12 - #9423 - CR8262
    and f.accttype =#interfacevalue.accttype and f.branchno = #interfacevalue.branchno 
    and f.source =#interfacevalue.counttype2 ),0)
   where counttype1='ADJUSTMENT'      
   insert into #interfacevalue 
       (interface,runno,counttype1,counttype2,
        branchno,accttype,countvalue,value)
   select
       'UPDSMRY',@runno,'INTONSETT',source,
        branchno,accttype,0,SUM(transvalue)
   from #FINTRANS --only settled accounts with interest included
   where transtypecode  in ('INT','ADM')
   group by accttype,source,branchno

   update #interfacevalue
   set countvalue = 
   ISNULL((select count(*)  
    from #FINTRANS f
    where transtypecode in ('INT','ADM')
    and f.accttype =#interfacevalue.accttype and f.branchno = #interfacevalue.branchno 
    and f.source =#interfacevalue.counttype2 ),0)
   where counttype1='INTONSETT'      

   insert into #interfacevaluecase
       (interface,runno,counttype1,counttype2,
        branchno,accttype,countvalue,value)
   select
       'UPDSMRY',@runno,'TOTUNINT','COSACS',
        a.branchno,
         case a.accttype when 'C' then 'CASH'
                         when 'S' then 'SPECIAL'
						 When 'T' then 'STORECARD'
                         else 'HP'
         end

      ,0,SUM(f.transvalue)
   from fintrans f, acct a--only unsettled accounts with interest included
   where f.transtypecode  in ('INT','ADM') and runno ='0' and a.currstatus !='S'
   and a.AcctType!='T'		-- #9886
   and a.acctno = f.acctno 
   group by a.accttype,a.branchno

	insert into #interfacevalue
       (interface,runno,counttype1,counttype2,
        branchno,accttype,countvalue,value)
    select interface,runno,counttype1,counttype2,
    branchno,accttype,0,sum(value)
    from #interfacevaluecase
    group by interface,runno,counttype1,counttype2,
    branchno,accttype

    delete from #interfacevaluecase

	insert into #interfacevaluecase
       (interface,runno,counttype1,counttype2,
        branchno,accttype,countvalue,value)
   select
       'UPDSMRY',@runno,'INTADM',source,
        a.branchno,
         case a.accttype when 'C' then 'CASH'
                         when 'S' then 'SPECIAL'
						 When 'T' then 'STORECARD'
                         else 'HP'
         end

      ,0,SUM(f.transvalue)
   from fintrans f, acct a
   where f.transtypecode  in ('INT','ADM') 
   and f.datetrans between @datepreviousrun and @daterun
   and a.acctno = f.acctno
   group by a.accttype,f.source,a.branchno

	insert into #interfacevalue
       (interface,runno,counttype1,counttype2,
        branchno,accttype,countvalue,value)
    select interface,runno,counttype1,counttype2,
    branchno,accttype,0,sum(value)
    from #interfacevaluecase
    group by interface,runno,counttype1,counttype2,
    branchno,accttype

    delete from #interfacevaluecase
   
    -- getting interest totals	for unsettled accounts
   select f.*, convert(varchar(10),'') as accttype into #interest from acct a, fintrans f
   where a.acctno=f.acctno and f.transtypecode in ('INT','ADM')
   and f.runno =0 and a.currstatus !='S'
   and f.datetrans between @datepreviousrun and @daterun

   update #interest set accttype ='SPECIAL' where acctno like '___5%'
   update #interest set accttype ='CASH' where acctno like '___4%'
   update #interest set accttype ='HP' where acctno like '___0%'
   update #interest set accttype ='STORECARD' where acctno like '___9%'


   insert into #interfacevalue 
       (interface,runno,counttype1,counttype2,
        branchno,accttype,countvalue,value)
   select
       'UPDSMRY',@runno,'INTONUNSETT',source,
        branchno,accttype,0,SUM(transvalue)
   from #interest
   group by accttype,source,branchno

   update #interfacevalue
   set countvalue = 
   ISNULL((select count(*)  
    from #interest f
    where f.accttype =#interfacevalue.accttype and f.branchno = #interfacevalue.branchno 
    and f.source =#interfacevalue.counttype2 ),0)
    where counttype1='INTONSETT'

   -- number of new accounts

   insert into #interfacevaluecase 
       (interface,runno,counttype1,counttype2,
        branchno,accttype,countvalue,value)
   select
       'UPDSMRY',@runno,'NEWACCT','COSACS',
        branchno,
	CASE accttype
           WHEN 'C' THEN 'CASH'
           WHEN 'S' THEN 'SPECIAL'
		   WHEN 'T' THEN 'STORECARD'
           ELSE 'HP'
        END, count(*),0
   from acct 
   where dateacctopen between @datepreviousrun and @daterun
   group by accttype,branchno

	insert into #interfacevalue
       (interface,runno,counttype1,counttype2,
        branchno,accttype,countvalue,value)
    select interface,runno,counttype1,counttype2,
    branchno,accttype,sum(countvalue),0
    from #interfacevaluecase
    group by interface,runno,counttype1,counttype2,
    branchno,accttype

    delete from #interfacevaluecase
     -- now insert missing records   
	insert into #interfacevalue
       (interface,runno,counttype1,counttype2,
        branchno,accttype,countvalue,value)
    select
       'UPDSMRY',@runno,'NEWACCT','COSACS',
        branchno,'CASH',0,0
     from branch where not exists
	(select * from #interfacevalue i where runno =@runno 
     and i.branchno = branch.branchno and i.counttype1='NEWACCT'
     and i.counttype2='COSACS' and i.accttype ='CASH')
	
	insert into #interfacevalue
       (interface,runno,counttype1,counttype2,
        branchno,accttype,countvalue,value)
    select
       'UPDSMRY',@runno,'NEWACCT','COSACS',
        branchno,'HP',0,0
     from branch where not exists
	(select * from #interfacevalue i where runno =@runno 
     and i.branchno = branch.branchno and i.counttype1='NEWACCT'
     and i.counttype2='COSACS' and i.accttype ='HP')

   insert into #interfacevalue
       (interface,runno,counttype1,counttype2,
        branchno,accttype,countvalue,value)
    select
       'UPDSMRY',@runno,'NEWACCT','COSACS',
        branchno,'SPECIAL',0,0
     from branch where not exists
	(select * from #interfacevalue i where runno =@runno 
     and i.branchno = branch.branchno and i.counttype1='NEWACCT'
     and i.counttype2='COSACS' and i.accttype ='SPECIAL')
	
  --reopened accounts
   insert into #interfacevaluecase
       (interface,runno,counttype1,counttype2,
        branchno,accttype,countvalue,value)
   select
       'UPDSMRY',@runno,'ACCOUNTS C/F','',
        branchno,
	CASE accttype
           WHEN 'C' THEN 'CASH'
           WHEN 'S' THEN 'SPECIAL'
		   WHEN 'T' THEN 'STORECARD'
           ELSE 'HP'
        END, count(*),0
   from acct 
   where currstatus !='S'
    and not (currstatus = '0' and outstbal = 0)
   group by accttype,branchno

   
   insert into #interfacevalue
       (interface,runno,counttype1,counttype2,
        branchno,accttype,countvalue,value)
    select interface,runno,counttype1,counttype2,
    branchno,accttype,sum(countvalue),0
    from #interfacevaluecase
    group by interface,runno,counttype1,counttype2,
    branchno,accttype

    delete from #interfacevaluecase
  --reopened accounts

   -- intermediate table used as hp accounts were inserting multiple rows for R's and hps
   insert into #interfacevalueCase 
       (interface,runno,counttype1,counttype2,
        branchno,accttype,countvalue,value)
   select
       'UPDSMRY',@runno,'BALANCE C/F','',
        branchno,
	CASE accttype
           WHEN 'C' THEN 'CASH'
           WHEN 'S' THEN 'SPECIAL'
		   WHEN 'T' THEN 'STORECARD'
           ELSE 'HP'
        END, 0,isnull(sum(outstbal),0)
   from acct
   group by  accttype,branchno

    insert into #interfacevalue
       (interface,runno,counttype1,counttype2,
        branchno,accttype,countvalue,value)
    select interface,runno,counttype1,counttype2,
    branchno,accttype,0,sum(value)
    from #interfacevaluecase
    group by interface,runno,counttype1,counttype2,
    branchno,accttype

-- Uat 363 Balance carried forward now being reduced by interest as on old system
  UPDATE v SET value = v.value -ISNULL(u.value,0)
  FROM #interfacevalue v, #interfacevalue u
  WHERE  v.counttype1 = 'BALANCE C/F'
   AND u.counttype1 = 'TOTUNINT' 
   AND v.accttype = u.accttype 
   AND v.branchno =u.branchno

	delete from #interfacevalueCase

   /* doing settled accounts*/
   insert into #interfacevalueCase 
       (interface,runno,counttype1,counttype2,
        branchno,accttype,countvalue,value)
   select
       'UPDSMRY',@runno,'SETTLED','',
        branchno,
	CASE accttype
           WHEN 'C' THEN 'CASH'
           WHEN 'S' THEN 'SPECIAL'
		   WHEN 'T' THEN 'STORECARD'
           ELSE 'HP'
        END, isnull(count(*),0),0
   from acct a
   where currstatus ='S' and exists (select * from status s
   where s.statuscode = 'S'
   and datestatchge between @datepreviousrun and @daterun 
   and s.acctno = a.acctno ) 
   group by  accttype,branchno

    insert into #interfacevalue
       (interface,runno,counttype1,counttype2,
        branchno,accttype,countvalue,value)
    select interface,runno,counttype1,counttype2,
    branchno,accttype,sum(countvalue),0
    from #interfacevaluecase
    group by interface,runno,counttype1,counttype2,
    branchno,accttype

	delete from #interfacevalueCase

	insert into #interfacevalue
       (interface,runno,counttype1,counttype2,
        branchno,accttype,countvalue,value)
    select
       'UPDSMRY',@runno,'SETTLED','COSACS',
        branchno,'CASH',0,0
     from branch where not exists
	(select * from #interfacevalue i where runno =@runno 
     and i.branchno = branch.branchno and i.counttype1='SETTLED'
     and i.counttype2='COSACS' and i.accttype ='CASH')
	
	insert into #interfacevalue
       (interface,runno,counttype1,counttype2,
        branchno,accttype,countvalue,value)
    select
       'UPDSMRY',@runno,'SETTLED','COSACS',
        branchno,'HP',0,0
     from branch where not exists
	(select * from #interfacevalue i where runno =@runno 
     and i.branchno = branch.branchno and i.counttype1='SETTLED'
     and i.counttype2='COSACS' and i.accttype ='HP')

   insert into #interfacevalue
       (interface,runno,counttype1,counttype2,
        branchno,accttype,countvalue,value)
    select
       'UPDSMRY',@runno,'SETTLED','COSACS',
        branchno,'SPECIAL',0,0
     from branch where not exists
	(select * from #interfacevalue i where runno =@runno 
     and i.branchno = branch.branchno and i.counttype1='SETTLED'
     and i.counttype2='COSACS' and i.accttype ='SPECIAL')
	

  -- if no of accounts c/f = New accounts - Settled + Reopened + Accounts b/f
  --Then reopened accounts =accounts cf - new accounts + settled - Accounts b/f 
   declare @reopened table 
   (branchno SMALLINT,accttype VARCHAR(10),
    accountscf INT,newaccts INT,settled INT,accountsbf INT,reopened int)
    
   INSERT INTO @reopened(branchno,accttype)
   SELECT branchno,accttype FROM #interfacevalue
   GROUP BY branchno,accttype

   UPDATE r
   SET accountscf = x.countvalue
   FROM @reopened r, (
		SELECT c.branchno, c.accttype, sum(c.countvalue) as countvalue
		FROM #interfacevalue c
		WHERE c.counttype1='ACCOUNTS C/F'
		group by c.branchno, c.accttype)  x	   
	where x.branchno = r.branchno
		AND x.accttype = r.accttype
   
   UPDATE r
   SET accountsbf = x.countvalue
   FROM @reopened r, (
		SELECT c.branchno, c.accttype, sum(c.countvalue) as countvalue
		FROM #interfacevalue c
		WHERE c.counttype1='ACCOUNTS B/F'
		group by c.branchno, c.accttype)  x	   
	where x.branchno = r.branchno
		AND x.accttype = r.accttype 
   
   UPDATE r
   SET settled = x.countvalue
   FROM @reopened r, (
		SELECT c.branchno, c.accttype, sum(c.countvalue) as countvalue
		FROM #interfacevalue c
		WHERE c.counttype1='SETTLED'
		group by c.branchno, c.accttype)  x	   
	where x.branchno = r.branchno
		AND x.accttype = r.accttype  

UPDATE r
   SET newaccts = x.countvalue
   FROM @reopened r, (
		SELECT c.branchno, c.accttype, sum(c.countvalue) as countvalue
		FROM #interfacevalue c
		WHERE c.counttype1='NEWACCT'
		group by c.branchno, c.accttype)  x	   
	where x.branchno = r.branchno
		AND x.accttype = r.accttype  
   
   insert into #interfacevalue 
       (interface,runno,counttype1,counttype2,
        branchno,accttype,countvalue,value)
   select
       'UPDSMRY',@runno,'REOPENED','COSACS',
        branchno, accttype, isnull(accountscf,0) - ISNULL(newaccts,0) + 
      ISNULL(settled,0)-ISNULL(accountsbf,0),0
   FROM @reopened
   
   SELECT * FROM @reopened

   if @test ='True'
   begin
       print 'Dont Match'
       select i.accttype as oldaccttype,i.value as oldvalue,i.countvalue as oldcount,
       t.value as newvalue,t.countvalue as newcount,t.* 
       from #interfacevalue t,interfacevalue i    
       where t.counttype1 = i.counttype1
       and t.counttype2 = i.counttype2
       and t.runno = i.runno and 
       (i.value !=t.value or i.countvalue !=t.countvalue)
       and i.accttype = t.accttype
       and i.branchno = t.branchno
       and i.interface ='UPDSMRY'
       
       order by i.branchno,i.accttype,i.counttype1
      print 'in test but not current'
      select * from #interfacevalue t where
      not exists (select * from interfacevalue i
      where i.accttype = t.accttype
       and i.branchno = t.branchno and t.counttype1 = i.counttype1
       and t.counttype2 = i.counttype2
       and t.runno = i.runno and i.interface ='UPDSMRY' )

      print 'in current but not test'
      select * from interfacevalue i where
      not exists (select * from #interfacevalue t
      where i.accttype = t.accttype
       and i.branchno = t.branchno and t.counttype1 = i.counttype1
       and t.counttype2 = i.counttype2
       and t.runno = i.runno )
       and i.runno = @runno and i.interface = 'updsmry'
   end
   else
   begin
        delete from interfacevalue where runno = @runno and interface ='updsmry'
        insert into interfacevalue
        (interface,runno,counttype1,counttype2,
         branchno,accttype,countvalue,value)
	select interface,runno,counttype1,counttype2,
         branchno,accttype,countvalue,value
         from #interfacevalue         
       
   end


