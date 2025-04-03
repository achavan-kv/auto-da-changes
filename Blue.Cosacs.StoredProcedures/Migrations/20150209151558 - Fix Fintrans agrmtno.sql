-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

--First scenario
--Problem that caused this issue seems to have been fixed since October....fixing data
select 
    f.acctno, f.agrmtno, ag.agrmtno as correctagrmtno
into 
    #fintrans1
from 
    fintrans f
inner join 
    acct a on f.acctno = a.acctno
inner join 
    agreement ag on ag.acctno = a.acctno
where 
    a.accttype in ('R', 'O', 'C')
    and not exists(select * from service.request r
                    where r.Id = right(f.agrmtno, len(f.agrmtno) - 3))
    and f.agrmtno > 1
    and ftnotes != 'DNSR'
    and f.agrmtno != ag.agrmtno
order by 
    f.datetrans desc

update 
    fintrans
set 
    agrmtno = f1.correctagrmtno
from 
    fintrans ft1 
inner join 
    #fintrans1 f1 on ft1.acctno = f1.acctno
    and (ft1.agrmtno = f1.agrmtno
            or isnull(ft1.agrmtno,0) != f1.correctagrmtno)
    and ft1.ftnotes != 'DNSR'

   
drop table #fintrans1

--Second scenario
select
     f.acctno, f.agrmtno, ag.agrmtno as correctagrmtno 
into 
     #fintrans2
from 
    fintrans f
inner join 
    acct a on f.acctno = a.acctno
inner join 
    agreement ag on ag.acctno = a.acctno
where 
    a.accttype in ('R', 'O', 'C')
    and f.agrmtno = 0
    and f.agrmtno != ag.agrmtno
order by f.datetrans desc


update 
    fintrans
set 
    agrmtno = f1.correctagrmtno
from 
    fintrans ft1 
inner join 
    #fintrans2 f1 on ft1.acctno = f1.acctno
    and isnull(ft1.agrmtno,0) != f1.correctagrmtno

drop table #fintrans2

--Scenario 3
select 
    f.acctno, f.agrmtno, ag.agrmtno as correctagrmtno  
into 
     #fintrans3
from 
    fintrans f
inner join 
    acct a on f.acctno = a.acctno
inner join 
    agreement ag on ag.acctno = a.acctno
where 
    a.accttype in ('R', 'O', 'C')
    and f.agrmtno is null
order by f.datetrans desc

update 
    fintrans
set 
    agrmtno = f1.correctagrmtno
from 
    fintrans ft1 
inner join 
    #fintrans3 f1 on ft1.acctno = f1.acctno
    and ft1.agrmtno is null

drop table #fintrans3
