-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

ALTER TABLE Admin.RolePermission
	DROP CONSTRAINT FK_RolePermission_RoleId
GO
ALTER TABLE Admin.UserRole
	DROP CONSTRAINT FK_UserRole_RoleId
GO
ALTER TABLE Admin.RolePermission ADD CONSTRAINT
	FK_RolePermission_RoleId FOREIGN KEY
	(
	RoleId
	) REFERENCES Admin.Role
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  CASCADE 
	
GO
ALTER TABLE Admin.UserRole ADD CONSTRAINT
	FK_UserRole_RoleId FOREIGN KEY
	(
	RoleId
	) REFERENCES Admin.Role
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  CASCADE 
	
GO
