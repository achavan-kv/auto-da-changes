if exists (select * from sysobjects where name ='DN_AcctNoCtrlGetSP')
drop procedure DN_AcctNoCtrlGetSP
go
create procedure DN_AcctNoCtrlGetSP
@branchno smallint, --0 for all
@return integer OUTPUT
AS 

set @return = 0

set nocount on 

select convert(varchar,a.branchno) + ' : ' + b.branchname as branchno,a.acctcat,a.acctcatdesc,a.hiallocated,a.hiallowed
from acctnoctrl a, branch b,accttype c
where (a.branchno = @branchno or @branchno = 0)
  and a.branchno = b.branchno
  and c.accttype = a.acctcat
order by a.branchno,a.acctcat

set @return =@@error

go

