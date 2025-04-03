-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS(
SELECT TOP 1 *
FROM INFORMATION_SCHEMA.TABLES
WHERE [TABLE_NAME] = 'PermissionDelegate'
AND [TABLE_SCHEMA] = 'Admin')

BEGIN
CREATE TABLE Admin.PermissionDelegate
(
	MainPermission Int NOT NULL CONSTRAINT FK_Permission_MainPermission_Id FOREIGN KEY REFERENCES Admin.Permission(Id),
	DelegatePermission Int NOT NULL CONSTRAINT FK_DelegatePermission_MainPermission_Id FOREIGN KEY REFERENCES Admin.Permission(Id),

	CONSTRAINT PK_AdminPermissionDelegate PRIMARY KEY CLUSTERED
	(
		MainPermission ASC,
		DelegatePermission ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
 END

 GO
