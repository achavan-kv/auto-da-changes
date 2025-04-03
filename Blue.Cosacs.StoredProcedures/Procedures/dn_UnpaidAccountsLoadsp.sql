if exists (select * from sysobjects where name ='dn_UnpaidAccountsLoadsp')
drop procedure dn_unpaidaccountsloadsp
go
create procedure dn_UnpaidAccountsLoadsp
@branchno smallint, --0 for all
@empeenosale integer, --0 for all
@return integer OUTPUT
AS declare
@vbranchno varchar(5) 

set @return = 0

set nocount on 
if @branchno >0 
  set @vbranchno = convert(varchar,@branchno) + '%'
else
  set @vbranchno ='%'

select a.acctno,a.termstype,a.dateacctopen,
a.agrmttotal,
a.datelastpaid,
a.paidpcent,
a.accttype,
convert(money ,0) as deposit,
convert(money ,0) as paid,
convert(money ,0) as topay,
convert(integer ,0) as empeenosale ,
convert(varchar(64) ,'') as salesperson,
convert(varchar(1000),'' ) as notes,
convert(varchar(40),'') as custid,
GetDate() as dateprop
into #unpaidaccounts
from acct a
where a.acctno like @vbranchno
and (a.currstatus ='U' 
or a.currstatus in ('0','1')
and not exists ( select * from custacct c where c.acctno = a.acctno and c.hldorjnt ='H'))


create clustered index ix_hashunpaid on #unpaidaccounts(acctno)


update #unpaidaccounts 
set notes = propnotes, custid = proposal.custid, dateprop = proposal.dateprop
 from proposal where proposal.acctno = #unpaidaccounts.acctno
and proposal.dateprop = (select max(p.dateprop) from proposal p where p.acctno = proposal.acctno)

update #unpaidaccounts 
set notes ='No customer - either link to customer or cancel' where not exists ( select * from custacct c where c.acctno = #unpaidaccounts.acctno and c.hldorjnt ='H')

update #unpaidaccounts 
set deposit = g.deposit,
empeenosale = g.empeenosale
from agreement g
where g.acctno = #unpaidaccounts.acctno

-- instalpredel = quasi deposit
update #unpaidaccounts 
set deposit = i.instalamount 
from instalplan i
where i.acctno =#unpaidaccounts.acctno 
and exists 
  (select * from termstype t where
   t.termstype =#unpaidaccounts.termstype
   and t.instalpredel ='Y')

update #unpaidaccounts 
set paid = (select -sum(transvalue)
from fintrans f where f.transtypecode in ('PAY','SCX','XFR','COR','RET','REF','DDN','DDR','DDE')
and f.acctno =#unpaidaccounts.acctno)

update #unpaidaccounts
set topay =deposit -isnull(paid,0)

if @empeenosale <>0
  delete from #unpaidaccounts
  where empeenosale != @empeenosale

update #unpaidaccounts
set salesperson = c.FullName
from Admin.[User] c 
where c.Id =#unpaidaccounts.empeenosale

select 
 salesperson + ' ' + convert(varchar,empeenosale) as salesperson,
 acctno,
 accttype,
 dateacctopen,
 agrmtTotal,
 isnull(convert(varchar,datelastpaid,103),' ') as datelastpaid,
 isnull(topay,0) as topay,
 paidpcent,
 Notes,
 custid,
 dateprop
from #unpaidaccounts

 set @return =@@error

go
/*
exec dn_UnpaidAccountsLoadsp
@branchno =910, --0 for all
@empeenosale= 0,--0 for all
@return =0 */
