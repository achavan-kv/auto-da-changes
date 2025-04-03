-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
update [admin].[permission]
set name = 'Goods Receipt View'
where id = 2136

update [admin].[permission]
set name = 'Goods Receipt Edit'
where id = 2137

delete from [admin].[permission]
where name = 'User View'
and id = 2138

update [admin].[permission]
set name = 'User Can Receive Goods',
[description] = 'User appears in the Goods Received By Selection List'
where id = 2139

update [admin].[permission]
set name = 'Goods Receipt Approve'
where id = 2140

