-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

Delete W 
from warrantyband w 
where not exists(select * from stockinfo s where s.itemno=w.waritemno)
