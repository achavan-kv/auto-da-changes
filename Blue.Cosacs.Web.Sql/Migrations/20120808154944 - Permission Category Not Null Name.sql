-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
CREATE TABLE Admin.Tmp_PermissionCategory
(
	Id		int			NOT NULL IDENTITY (1, 1),
	Name	varchar(50) NOT NULL
)  ON [PRIMARY]
GO

SET IDENTITY_INSERT Admin.Tmp_PermissionCategory ON
GO

INSERT INTO Admin.Tmp_PermissionCategory 
	(Id, Name)
SELECT 
	Id, 
	Name 
FROM 
	Admin.PermissionCategory WITH (HOLDLOCK TABLOCKX)
GO

SET IDENTITY_INSERT Admin.Tmp_PermissionCategory OFF
GO

ALTER TABLE Admin.Permission
	DROP CONSTRAINT FK_Permission_PermissionsCategory
GO

DROP TABLE Admin.PermissionCategory
GO

EXECUTE sp_rename N'Admin.Tmp_PermissionCategory', N'PermissionCategory', 'OBJECT' 
GO

ALTER TABLE Admin.PermissionCategory ADD CONSTRAINT
	PK_PermissionsCategory PRIMARY KEY CLUSTERED 
	(
		Id
	) WITH
	( 
		STATISTICS_NORECOMPUTE = OFF, 
		IGNORE_DUP_KEY = OFF, 
		ALLOW_ROW_LOCKS = ON, 
		ALLOW_PAGE_LOCKS = ON
	) ON [PRIMARY]
GO
