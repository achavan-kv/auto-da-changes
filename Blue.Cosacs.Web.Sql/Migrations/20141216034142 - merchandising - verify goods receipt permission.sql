-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
insert into admin.Permission
(CategoryId, Id, Name, Description)
values
('21', '2143', 'Goods Receipt Verify', 'Permits the user to verify goods receipts')