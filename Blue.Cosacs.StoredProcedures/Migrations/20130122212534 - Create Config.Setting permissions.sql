
DECLARE @catid INT = 0
SELECT @catid=id FROM Admin.PermissionCategory WHERE name='System Administration'
EXEC Admin.AddPermission 1202, 'View Settings configuration', @catid, 'Allows user to view the System Settings'
EXEC Admin.AddPermission 1203, 'Customise Settings configuration', @catid, 'Allows user to edit the System Settings'
