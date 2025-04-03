-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
insert into Admin.Permission
(CategoryId, Id, Name, Description)
VALUES
(21, 2130, 'CostPriceView', 'Allows users to view product costs'),
(21, 2131, 'CostPriceEdit', 'Allows users to edit costs')