/* Procedure to calculate daily interest charge 
Issue 65 prevent null errors*/
if  exists (select * from sysobjects  where name =  'orintdailycharges' )
drop procedure orintdailycharges
go
create procedure orintdailycharges @runno integer, @branchno integer, @rundate datetime
as 
declare @status integer,@startdate datetime,@finishdate datetime, @varbranch varchar (6)
set @varbranch ='___0%' -- no longer doing a branch at a time - now a big chunk 28 June 2006
set @startdate = dateadd(day,-7,@rundate)
set @finishdate = dateadd(day,-1,@rundate)

set nocount on

   select a.acctno, a.datefrom	, a.dateto,c.mpr,a.arrears
   into #initialarrears
   from arrearsdaily a, chargesdata c
   where c.acctno =a.acctno and c.runno =@runno
   and (a.dateto >=@startdate or a.dateto is null)
   and a.acctno like @varbranch

   update i
   set i.arrears = a.arrears
   from acct a
   inner join #initialarrears i on i.Acctno = a.acctno
   inner join chargesdata cd on cd.acctno = i.Acctno
   where a.arrears < i.arrears
   and cd.arrears  > 0.001
   and cd.runno = @runno
   and  not (cd.currstatus ='' and cd.oldstatus ='1') and 
   not (cd.currstatus ='1' and cd.oldstatus ='1') 
         

   update #initialarrears
   set datefrom = @startdate where datefrom <@startdate 

--now getting a total

   select acctno, convert(money,sum(mpr * arrears *(DateDiff(day,datefrom,dateto)+1))) as interest
   into #charges
   from #initialarrears where dateto is not null and datefrom <= dateto
   group by acctno
   create clustered index ix_charges_acctno12342 on #charges (acctno)

   update #charges set interest =interest + ( select sum(mpr *arrears * (DateDiff(day,datefrom,@finishdate)+1))
   from #initialarrears
   where #charges.acctno =#initialarrears.acctno and dateto is null and datefrom <= @finishdate)

   insert into #charges (acctno, interest) select acctno, sum(mpr * arrears * (DateDiff(day,datefrom,@finishdate)+1))
   from #initialarrears where dateto is null and datefrom <= @finishdate
   and not exists (select * from #charges where #initialarrears.acctno =#charges.acctno)
   group by acctno

   update #charges set interest = interest/30.42

/*   select top 100 acctno,interest from #charges*/
   update chargesdata set intchargesdue = isnull(round(interest,2),0)
   from #charges
   where runno = @runno and  arrears > 0.001 and
   #charges.acctno =chargesdata.acctno and
   not (currstatus ='' and oldstatus ='1') and
   not (currstatus ='1' and oldstatus ='1') and
   not (currstatus ='2' and oldstatus ='1')
   and chargesdata.acctno like  @varbranch

go
--sp_help arrearsdaily
--exec orintdailycharges @runno =25, @branchno =770, @rundate ='19-sep-2004'
