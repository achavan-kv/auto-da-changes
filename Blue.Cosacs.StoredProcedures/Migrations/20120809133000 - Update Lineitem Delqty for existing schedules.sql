-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
-- Related to issue: #13552

update lineitem
set delqty = s.quantity
from ScheduleToBookings s inner join lineitem l on l.id = s.LineitemId