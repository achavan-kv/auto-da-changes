-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
update Hub.Queue
set Binding = 'Warranty.Sale.Cancel'
where Id = 17