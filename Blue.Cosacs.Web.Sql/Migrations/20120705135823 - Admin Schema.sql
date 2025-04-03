IF NOT object_id('[Admin].[User]') IS NULL
BEGIN 
	DROP TABLE [Admin].RolePermission;
	DROP TABLE [Admin].Permission;
	DROP TABLE [Admin].PermissionCategory
	DROP TABLE [Admin].[UserRole];
	DROP TABLE [Admin].[Role];
	DROP TABLE [Admin].[User];
	DROP SCHEMA [Admin]

END
GO

CREATE SCHEMA Admin
GO

CREATE TABLE Admin.[User]
(
	Id INT IDENTITY(1,1),
	BranchNo SMALLINT NOT NULL,
	[Login] VARCHAR(256) UNIQUE NONCLUSTERED NOT NULL,
	[Password] VARCHAR(256) NOT NULL DEFAULT NEWID(),
	[PasswordSalt] VARCHAR(128) NOT NULL DEFAULT NEWID(),
	PasswordExpireDate DATETIME NOT NULL,
	FirstName VARCHAR(50) NOT NULL,
	LastName VARCHAR(50) NOT NULL,
	ExternalLogin VARCHAR(256) NULL,
	LegacyPassword BIGINT SPARSE NULL
)
GO

ALTER TABLE Admin.[USER]
ADD CONSTRAINT PK_User PRIMARY KEY CLUSTERED (Id)
GO

CREATE TABLE Admin.[Role]
(
	Id INT IDENTITY(1,1),
	[Name] VARCHAR(50) UNIQUE NONCLUSTERED NOT NULL,
	[Description] VARCHAR(200) NOT NULL
)
GO

ALTER TABLE Admin.[Role]
ADD CONSTRAINT PK_Role PRIMARY KEY CLUSTERED (Id)
GO

CREATE TABLE Admin.[UserRole]
(
	RoleId INT NOT NULL,
	UserId INT NOT NULL,
)
GO

ALTER TABLE Admin.[UserRole]
ADD CONSTRAINT PK_UserRole PRIMARY KEY CLUSTERED (RoleId, UserId)
GO

ALTER TABLE Admin.[UserRole]
ADD CONSTRAINT FK_UserRole_UserID FOREIGN KEY (UserId) REFERENCES Admin.[User] (Id)
GO

ALTER TABLE Admin.[UserRole]
ADD CONSTRAINT FK_UserRole_RoleId FOREIGN KEY (RoleId) REFERENCES Admin.Role (Id)
GO

CREATE TABLE Admin.Permission
(
	Id INT IDENTITY(1,1),
	Name VARCHAR(100) UNIQUE NONCLUSTERED NOT NULL, 
	CategoryId INT NOT NULL,
	[Description] VARCHAR(200) NOT NULL,
	[Deny] BIT NOT NULL DEFAULT 0 
)
GO

ALTER TABLE Admin.Permission
ADD CONSTRAINT PK_PermissionId PRIMARY KEY CLUSTERED (Id)
GO

CREATE TABLE Admin.PermissionCategory
(
	Id INT IDENTITY(1,1),
	Name VARCHAR(50) 
)

ALTER TABLE Admin.PermissionCategory
ADD CONSTRAINT PK_PermissionsCategory PRIMARY KEY CLUSTERED (Id)
GO

ALTER TABLE Admin.Permission
ADD CONSTRAINT FK_Permission_PermissionsCategory FOREIGN KEY (CategoryId) REFERENCES Admin.PermissionCategory (Id)
GO

CREATE TABLE Admin.RolePermission
(
	RoleId INT NOT NULL, 
	PermissionId INT NOT NULL
)
GO

ALTER TABLE Admin.RolePermission
ADD CONSTRAINT PK_RolePermission PRIMARY KEY CLUSTERED (RoleId,PermissionId)
GO

ALTER TABLE Admin.RolePermission
ADD CONSTRAINT FK_RolePermission_PermissionId FOREIGN KEY (PermissionId) REFERENCES Admin.Permission (Id)
GO

ALTER TABLE Admin.RolePermission
ADD CONSTRAINT FK_RolePermission_RoleId FOREIGN KEY (RoleId) REFERENCES Admin.Role (Id)
GO


INSERT INTO  Admin.[User]
		( BranchNo ,
		  Login ,
		  PasswordExpireDate ,
		  FirstName ,
		  LastName ,
		  ExternalLogin ,
		  LegacyPassword
		)
SELECT 
		branchno ,
		empeeno,
		GETDATE(),
		firstname ,
		lastname, 
		FactEmployeeNo ,
		CASE
			WHEN ISNUMERIC(password) = 1 THEN [password]
			ELSE NULL
		END AS OldPassword
FROM dbo.courtsperson


INSERT INTO Admin.Role
		( Name, Description )
SELECT code,codedescript FROM dbo.code
		  WHERE category = 'ET1'
          
INSERT INTO Admin.UserRole
		( RoleId, UserId )
SELECT R.Id,U.Id  FROM dbo.courtsperson c
INNER JOIN Admin.Role R ON c.empeetype = R.Name
INNER JOIN Admin.[User] U ON c.empeeno = U.Login

INSERT INTO Admin.PermissionCategory
		( Name )
VALUES  ( 'Cosacs Tasks'  -- Name - varchar(50)
		  )

INSERT INTO admin.Permission
		( Name ,
		  CategoryId ,
		  Description 
		)
SELECT TaskName,1,TaskName FROM dbo.Task

INSERT INTO Admin.RolePermission
		( RoleId, PermissionId )
SELECT R.Id, P.Id
FROM dbo.RolePermissions CRP
INNER JOIN task T ON CRP.TaskID = T.TaskID
INNER JOIN Admin.Permission P ON P.Name = T.TaskName
INNER JOIN Admin.Role R ON R.Name = CRP.Role
