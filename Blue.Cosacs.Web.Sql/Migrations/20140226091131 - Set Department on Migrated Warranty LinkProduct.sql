-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

-- update Warranty Product Link with dept of max category count

;with refcat as (
select refcode,category,count(*) as nbr from stockitem
where refcode!='ZZ'
	--and  category not in(12,82) 
	and exists (select * from Warranty.Link where Name='Mig_Ref_' + refcode)
group by refcode,category
--order by refcode,count(*) desc
)

select distinct refcode,r.category,c.category as Dept
into #refcat 
from refcat r inner join code c on cast(r.category as varchar(3)) = c.code and c.category in ('PCE','PCF','PCW','PCO')
where nbr =(select max(nbr) from refcat r2 where r.refcode=r2.refcode group by refcode)
order by refcode

update warranty.LinkProduct
set Level_1=Dept
from warranty.LinkProduct p inner join #refcat  r on p.RefCode=r.refcode

