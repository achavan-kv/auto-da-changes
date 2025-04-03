-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
update [Admin].[Permission]
set name = 'Goods on Loan Edit'
where id = 2166

update [Admin].[Permission]
set name = 'Goods on Loan View'
where id = 2165