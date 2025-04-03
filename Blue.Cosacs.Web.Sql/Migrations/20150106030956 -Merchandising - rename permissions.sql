-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
update [Admin].[Permission]
set name = 'Stock Adjustment Reasons Edit'
where id = 2145

update [Admin].[Permission]
set name = 'Stock Adjustment Reasons View'
where id = 2144