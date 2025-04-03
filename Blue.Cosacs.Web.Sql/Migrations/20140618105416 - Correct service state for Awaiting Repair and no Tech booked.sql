-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


update service.request
	set [State]='Awaiting allocation'
from service.request r 
where [state]='Awaiting repair' 
	and not exists (select * from service.TechnicianBooking t where t.requestid=r.id)


