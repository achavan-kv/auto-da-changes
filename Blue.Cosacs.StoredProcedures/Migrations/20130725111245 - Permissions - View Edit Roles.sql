update Admin.Permission
set [Name] = 'Sys Config - View Roles', [Description] = 'Allow the user to view user roles'
where Id = 392

insert into Admin.Permission
(Id, CategoryId, Name, [Description])
values (1205, 12, 'Sys Config - Edit Roles', 'Allow the user to edit role permissions and members')
