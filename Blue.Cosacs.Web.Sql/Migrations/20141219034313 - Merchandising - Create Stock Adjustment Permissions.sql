-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
insert into [Admin].Permission
(CategoryId, Id, Name, [Description])
values
(21, 2144, 'ViewStockAdjustmentReasons', 'Allows the user to view the stock adjustment reasons admin area')

insert into [Admin].Permission
(CategoryId, Id, Name, [Description])
values
(21, 2145, 'EditStockAdjustmentReasons', 'Allows the user to edit the stock adjustment reasons')