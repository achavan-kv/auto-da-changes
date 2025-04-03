-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
insert into [Admin].Permission
(CategoryId, Id, Name, [Description])
values
(21, 2140, 'GoodsReceiptApprove', 'Allows the user to approve goods receipts')