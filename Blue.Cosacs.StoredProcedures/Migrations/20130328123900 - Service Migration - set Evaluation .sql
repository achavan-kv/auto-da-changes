-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

UPDATE service.Request
set Evaluation=codedescript
from service.Request r INNER JOIN SR_ServiceRequest sr on r.id=sr.ServiceRequestNo
			INNER JOIN code c on sr.ServiceEvaln=c.code and c.category='SREVALN'

