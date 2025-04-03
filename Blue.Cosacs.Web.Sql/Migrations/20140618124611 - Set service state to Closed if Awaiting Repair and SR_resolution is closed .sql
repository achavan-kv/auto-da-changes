-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


Update service.request	
	set [state]='Closed'
from service.request q inner join SR_Resolution r on q.id=r.ServiceRequestNo
where [state]='Awaiting repair' 
and r.DateClosed!='1900-01-01'

