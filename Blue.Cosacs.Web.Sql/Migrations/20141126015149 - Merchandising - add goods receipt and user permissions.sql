-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

insert into [Admin].Permission
(CategoryId, Id, Name, [Description])
values
(21, 2136, 'GoodsReceiptView', 'Allows the user to view goods receipts'),
(21, 2137, 'GoodsReceiptEdit', 'Allows the user to create and edit goods receipts'),
(21, 2138, 'UserView', 'Allows the user to view user details')