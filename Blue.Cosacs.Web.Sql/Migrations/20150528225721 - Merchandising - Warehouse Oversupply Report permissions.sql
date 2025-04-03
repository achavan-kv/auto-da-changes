-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

insert into admin.permission
(Id,Name,CategoryId,Description)
values
(2051, 'Warehouse Oversupply Report', 20, 'Allows access to View/Print/Export the Warehouse Oversupply Report')
