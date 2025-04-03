-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


/*********************/
/***   Save Data   ***/
/*********************/
SELECT 
	Id,
	Name,
	CategoryId,
	[Description]
INTO
	Permission_tmp#
FROM 
	Admin.Permission

/***********************/
/***  Drop Old Table ***/
/***********************/

IF  NOT OBJECT_ID(N'Admin.FK_Permission_PermissionsCategory') IS NULL 
	ALTER TABLE Admin.Permission DROP CONSTRAINT FK_Permission_PermissionsCategory
GO

IF  NOT OBJECT_ID(N'Admin.FK_RolePermission_PermissionId') IS NULL 
	ALTER TABLE Admin.RolePermission DROP CONSTRAINT FK_RolePermission_PermissionId
GO

IF  NOT OBJECT_ID(N'Admin.Permission') IS NULL
	DROP TABLE Admin.Permission
GO

/****************************/
/***   Create new table   ***/
/****************************/

CREATE TABLE Admin.Permission
(
	Id				int				NOT NULL,
	Name			varchar(100)	NOT NULL,
	CategoryId		int				NOT NULL,
	[Description]	varchar(300)	NOT NULL,
	CONSTRAINT PK_PermissionId PRIMARY KEY CLUSTERED 
	(
		Id ASC
	)WITH 
	(
		PAD_INDEX  = OFF, 
		STATISTICS_NORECOMPUTE  = OFF, 
		IGNORE_DUP_KEY = OFF, 
		ALLOW_ROW_LOCKS  = ON, 
		ALLOW_PAGE_LOCKS  = ON
	),
	UNIQUE NONCLUSTERED 
	(
		Name ASC
	)WITH 
	(
		PAD_INDEX  = OFF, 
		STATISTICS_NORECOMPUTE  = OFF, 
		IGNORE_DUP_KEY = OFF, 
		ALLOW_ROW_LOCKS  = ON, 
		ALLOW_PAGE_LOCKS  = ON
	)
)

GO

SET ANSI_PADDING ON
GO

ALTER TABLE Admin.Permission  WITH CHECK 
	ADD CONSTRAINT FK_Permission_PermissionsCategory FOREIGN KEY
	(
		CategoryId
	)
	REFERENCES Admin.PermissionCategory 
	(
		Id
	)
GO

ALTER TABLE Admin.Permission CHECK CONSTRAINT FK_Permission_PermissionsCategory
GO

/*********************************************/
/***   Insert old values into new table   ****/
/********************************************/
INSERT INTO Admin.Permission
	(Id, Name, CategoryId, [Description])
SELECT 
	Id,
	Name,
	CategoryId,
	[Description]
FROM 
	Permission_tmp#
	
/*************************************/
/***   RolePermission Consttrain   ***/
/*************************************/

ALTER TABLE Admin.RolePermission ADD CONSTRAINT
	FK_RolePermission_PermissionId FOREIGN KEY
	(
		PermissionId
	) REFERENCES Admin.Permission
	(
		Id
	) 
GO

ALTER TABLE Admin.RolePermission CHECK CONSTRAINT FK_RolePermission_PermissionId
GO
