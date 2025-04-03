-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


IF NOT EXISTS(select * from syscolumns where name = 'DutyFree' and object_name(id) = 'courtsperson')
BEGIN
	ALTER TABLE dbo.courtsperson ADD DutyFree CHAR(1) 
	ALTER TABLE dbo.courtsperson ADD [Check] CHAR(1) 
END
GO

	IF NOT EXISTS(SELECT * FROM Admin.[Permission] WHERE id = 1001)
	BEGIN

		declare @category INT
			select @category= id from admin.PermissionCategory where Name='Sales'
			
		exec Admin.AddPermission 1001, 'New Sales Order - Allow Duty Free Sales', @category, 'Allows users to perform Duty Free Sales'
			
		insert into Admin.[RolePermission]
		select distinct RoleId, 1001, 0
		from Admin.[UserRole] r INNER JOIN courtsperson c ON r.UserId = c.UserId
		where c.dutyfree = 'Y'

	END


IF EXISTS(select * from syscolumns where name = 'Check' and object_name(id) = 'courtsperson')
BEGIN
	ALTER TABLE dbo.courtsperson drop COLUMN DutyFree 
	ALTER TABLE dbo.courtsperson drop COLUMN [check]
END
GO
