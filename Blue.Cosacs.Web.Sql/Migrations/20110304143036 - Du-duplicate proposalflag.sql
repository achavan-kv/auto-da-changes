-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

------------------------------------
-- De-duplicate proposal flag table
------------------------------------


select *
into #temp
from proposalflag

alter table #temp
add id int identity(1, 1)

select * 
into #backup
from proposalflag

select custid, acctno, dateprop, checktype, max(id) id
into #temp2
from #temp
group by custid, acctno, dateprop, checktype

drop table proposalflag


select origbr,custid,dateprop,checktype,datecleared,empeenopflg,unclearedby,Acctno
into proposalflag
from #temp t1
where id = (
		select id
		from #temp2 t2
		where t2.custid = t1.custid
			and t2.acctno = t1.acctno
			and t2.dateprop = t1.dateprop
			and t2.checktype = t1.checktype
			)
			
