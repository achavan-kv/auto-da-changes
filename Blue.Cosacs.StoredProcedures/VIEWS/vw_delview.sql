
If exists (select * from sysobjects where name = 'delview' )
drop view delview
go
create view delview
as select sum (transvalue) as transvalue,acctno
from fintrans
where transtypecode in ('DEL','GRT','ADD')
group by acctno
go