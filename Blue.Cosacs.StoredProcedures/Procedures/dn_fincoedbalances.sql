if  exists (select * from sysobjects where name =  'dn_fincoedbalances' )
drop procedure dn_fincoedbalances
go

create procedure dn_fincoedbalances 
@datefrom datetime, @dateto datetime,@return int out
as
set @return = 0
select convert(smallint,left(acctno,3)) as BranchNo,acctno as AccountNo,Balance 
from sec_account where datesecuritised
between @datefrom and @dateto
set @return =@@error
return @return
go