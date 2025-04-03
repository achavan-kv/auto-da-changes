insert into Admin.Permission (Id, CategoryId, [Name], [Description])
values (1432, 14, 'Delivery Confirmation All Branches', 'Allows user to confirm an item as delivered successfully or rejected for any branch')

update Admin.Permission
set [Description] = 'Allows user to confirm an item as delivered successfully or rejected for their branch'
where Id = 1408