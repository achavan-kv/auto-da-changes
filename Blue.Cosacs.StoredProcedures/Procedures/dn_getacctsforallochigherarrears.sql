if exists (select * from dbo.sysobjects where id = object_id('[dbo].[dn_getacctsforallochigherarrears]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[dn_getacctsforallochigherarrears]
GO

create procedure dn_getacctsforallochigherarrears 
@datemovedarrsfrom datetime,
@datemovedarrsto datetime,
@datemovedrestriction varchar(2)
as
select a.acctno,a.arrears,a.datefrom,a.dateto,
convert(money,0) as arrsexcharges, convert(money,0) as previousarrears
into #arrsmovement
from arrearsdaily a,#allocquery f
where f.acctno = a.acctno 
-- now remove those with

create clustered index ix_arrsmovement324 on #arrsmovement(acctno)
-- excluding interest charges- bit concerned that interest charges are 
-- datetime stamped and datefrom is truncated so taking off 23 hours from the interest charge
update #arrsmovement set arrsexcharges = arrears - isnull(
(select sum(transvalue) from fintrans where transtypecode in ('INT','ADM')
AND fintrans.acctno =#arrsmovement.acctno and dateadd(hour,-23,fintrans.datetrans) <=#arrsmovement.datefrom),0)

-- now removing duplicates i.e. where the movement was only caused by interest charges 
delete from #arrsmovement where exists (select * from #arrsmovement a
where a.acctno = #arrsmovement.acctno and a.arrsexcharges = #arrsmovement.arrsexcharges
and a.datefrom >#arrsmovement.datefrom)

-- now removing where movement is down - only interest in accounts with increased arrears
-- so need to get what was previous arrears
-- DSR Added 'sum' in 'sum(arrsexcharges)'. This should not be required because DateFrom should
-- be unique for an account, but testing has had problems after changing system date.
update #arrsmovement set previousarrears = (select sum(arrsexcharges)  from #arrsmovement a
where a.acctno =#arrsmovement.acctno and a.datefrom = (select max(b.datefrom) 
from #arrsmovement b where b.acctno =a.acctno and a.acctno =#arrsmovement.acctno
    and b.datefrom <#arrsmovement.datefrom))

delete from #arrsmovement where previousarrears >=arrsexcharges


if @datemovedrestriction ='NA' -- remove where arrears > instalamount - as only interested in new accounts in arrears
-- which will be one instalment in arrears only
   delete from #arrsmovement where exists (select * from instalplan i where i.acctno =#arrsmovement.acctno
    and arrsexcharges > instalamount)

delete from #allocquery where not exists 
(select * from #arrsmovement a where  ((a.datefrom between @datemovedarrsfrom and @datemovedarrsto) or
(a.dateto between @datemovedarrsfrom and @datemovedarrsto)) and a.acctno =#allocquery.acctno)

go

