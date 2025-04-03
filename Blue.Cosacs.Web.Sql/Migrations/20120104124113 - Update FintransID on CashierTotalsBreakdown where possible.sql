-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

select f.id as FintransId,f.paymethod,c.id as CashierTotalsId,f.transvalue		
into #cashierbreakdown
from fintrans f INNER JOIN CashierTotals c on f.empeeno = c.empeeno 
	INNER JOIN CashierTotalsBreakdown b on cashiertotalid=c.id and b.paymethod=f.paymethod
where  DATEADD(mi,-5,f.datetrans) between c.datefrom and c.dateto
--and f.empeeno=9104 
and transtypecode in('OVE','sho')



UPDATE CashierTotalsBreakdown
	set FintransId=t.FintransId
from #cashierbreakdown t,CashierTotalsBreakdown b
where t.CashierTotalsId=b.cashiertotalid and t.paymethod=b.paymethod and t.transvalue*-1=b.[difference]

