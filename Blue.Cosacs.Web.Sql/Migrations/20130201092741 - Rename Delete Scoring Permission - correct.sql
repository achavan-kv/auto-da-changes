-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


UPDATE admin.Permission
set [Description]='Allows user to access and edit the Customise Credit Matrices screen under the Scoring Menu accessed via the Systems Maintenance menu'
where id=59

UPDATE admin.Permission
	set name='Scoring - Import scoring Matrix',[Description]='Allows user access to the Scoring menu option under System Configuration Menu to Import scoring Matrix'
where id =60

if exists (select * from sys.objects where name = 'adminPermission_removed')
DROP TABLE adminPermission_removed

select * into adminPermission_removed
from admin.Permission
where id in(57,59)


delete admin.RolePermission where PermissionId in(57,59)

delete admin.Permission where id in(57,59)




