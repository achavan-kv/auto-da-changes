-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

insert into [Admin].Permission
(CategoryId, Id, Name, [Description])
values
(21, 2134, 'PurchaseOrderView', 'Allows the user to view purchase orders'),
(21, 2135, 'PurchaseOrderEdit', 'Allows the user to create and edit purchase orders')