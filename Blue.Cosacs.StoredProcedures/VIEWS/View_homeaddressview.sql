
drop view homeaddressview
go
create view dbo.homeaddressview as
select 
ca.acctno,
ca.custid,
isnull(cu.cusaddr1,'') as cusaddr1,
isnull(cu.cusaddr2,'') as cusaddr2,
isnull(cu.cusaddr3,'') as cusaddr3,
isnull(cu.cuspocode,'') as cuspocode
from custacct ca
left join custaddress cu on
ca.custid = cu.custid
where ca.hldorjnt = 'H' and (cu.datemoved is null or cu.datemoved ='')
and cu.addtype ='H'
AND   not exists ( SELECT *
                                   FROM   custaddress cust 
                                   WHERE  cust.custid = cu.custid
				   AND	  CUST.DATEIN > cu.DATEIN
                                   AND    cust.addtype = 'H'
                                   AND    (cust.datemoved = '' 
                                           OR cust.datemoved is null)
                                  )

go
Grant select on homeaddressview to public
GO