-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


Update service.request	
	set [state]='Closed'
from service.request q inner join SR_Resolution r on q.id=r.ServiceRequestNo
						inner join SR_ServiceRequest sr on q.id=sr.ServiceRequestNo
where [state]='Awaiting payment' 
and sr.Status='C'
and r.DateClosed!='1900-01-01'

