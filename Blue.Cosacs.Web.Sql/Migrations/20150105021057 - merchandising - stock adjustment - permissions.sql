insert into [Admin].Permission
(CategoryId, Id, Name, [Description])
values
(21, 2146, 'Stock Adjustments View', 'Allows the user to view the stock adjustment')

insert into [Admin].Permission
(CategoryId, Id, Name, [Description])
values
(21, 2147, 'Stock Adjustment Edit', 'Allows the user to create stock adjustments')

insert into [Admin].Permission
(CategoryId, Id, Name, [Description])
values
(21, 2148, 'Stock Adjustment Authorise', 'Allows the user to authorise stock adjustments')

update [Admin].Permission
set Name = 'Stock Adjustment Reasons View'
where name like 'ViewStockAdjustmentReasons'

update [Admin].Permission
set Name = 'Stock Adjustment Reasons Edit'
where name like 'EditStockAdjustmentReasons'