
-- Hierarchy delegate
insert into admin.Permission
(Id, Name, CategoryId, Description, IsDelegate)
select 2169, 'xx', CategoryId, Description, IsDelegate 
from admin.Permission
where id = 2103

update admin.Permission
set IsDelegate = 1, name = 'Hierarchy View delegate' 
where id = 2103
GO
update admin.Permission
set  name = 'Hierarchy View' 
where id = 2169
GO



-- Location View Delegate
insert into admin.Permission
(Id, Name, CategoryId, Description, IsDelegate)
select 2170, 'xx', CategoryId, Description, IsDelegate 
from admin.Permission
where id = 2101

update admin.Permission
set IsDelegate = 1, name = 'Location View delegate' 
where id = 2101

update admin.Permission
set  name = 'Location View' 
where id = 2170

-- Vendors View Delegate
insert into admin.Permission
(Id, Name, CategoryId, Description, IsDelegate)
select 2171, 'xx', CategoryId, Description, IsDelegate 
from admin.Permission
where id = 2110

update admin.Permission
set IsDelegate = 1, name = 'Vendor View delegate' 
where id = 2110

update admin.Permission
set  name = 'Vendor View' 
where id = 2171

--  vendor view  delegate
	--  vendor view
	insert into [Admin].[PermissionDelegate]
	(MainPermission, DelegatePermission)
	select 2171, 2110

	--  stock recieved 
	insert into [Admin].[PermissionDelegate]
	(MainPermission, DelegatePermission)
	select 2049, 2110

-- locationview  delegate

	-- Location view
	insert into [Admin].[PermissionDelegate]
	(MainPermission, DelegatePermission)
	select 2170, 2101

	-- Negative stock
	insert into [Admin].[PermissionDelegate]
	(MainPermission, DelegatePermission)
	select 2050, 2101

	-- Buyers sales
	insert into [Admin].[PermissionDelegate]
	(MainPermission, DelegatePermission)
	select 2053, 2101

	-- stock received 
	insert into [Admin].[PermissionDelegate]
	(MainPermission, DelegatePermission)
	select 2049, 2101

		-- stock valuation  
	insert into [Admin].[PermissionDelegate]
	(MainPermission, DelegatePermission)
	select 2042, 2101

-- hierarchy delegate

	-- hierarchy View
	insert into [Admin].[PermissionDelegate]
	(MainPermission, DelegatePermission)
	select 2169, 2103

	-- Negative stock
	insert into [Admin].[PermissionDelegate]
	(MainPermission, DelegatePermission)
	select 2050, 2103

	-- Buyers sales
	insert into [Admin].[PermissionDelegate]
	(MainPermission, DelegatePermission)
	select 2053, 2103

	-- stock received 
	insert into [Admin].[PermissionDelegate]
	(MainPermission, DelegatePermission)
	select 2049, 2103

	-- stock valuation  
	insert into [Admin].[PermissionDelegate]
	(MainPermission, DelegatePermission)
	select 2042, 2103
