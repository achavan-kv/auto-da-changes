-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


select transtypecode, 
	   datechange, 
	   count (*) as Number
into #duplicates
from transtypeaudit
group by transtypecode, datechange
having count(*) > 1

select distinct ta.*
into #transtypeaudittemp
from transtypeaudit ta inner join #duplicates d 
	 on ta.transtypecode = d.transtypecode
and ta.datechange = d.datechange

delete from transtypeaudit
where exists(select * from #duplicates d
				where d.transtypecode = transtypeaudit.transtypecode
				and d.datechange = transtypeaudit.datechange)


insert into transtypeaudit select * from #transtypeaudittemp

