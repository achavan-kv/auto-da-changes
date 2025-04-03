-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
ALTER TABLE Admin.[User]
	DROP CONSTRAINT FK_User_branch
GO
ALTER TABLE dbo.branch SET (LOCK_ESCALATION = TABLE)
GO

ALTER TABLE Admin.[User]
	DROP CONSTRAINT DF_User_Lock
GO
CREATE TABLE Admin.Tmp_User
	(
		Id						int				NOT NULL IDENTITY (1, 1),
		BranchNo				smallint		NOT NULL,
		[Login]					varchar(50)		NULL,
		[Password]				varchar(100)	NULL,
		LastChangePassword		smalldatetime	NOT NULL,
		FirstName				varchar(50)		NOT NULL,
		LastName				varchar(50)		NOT NULL,
		ExternalLogin			varchar(50)		NULL,
		LegacyPassword			bigint			SPARSE NULL,
		eMail					varchar(256)	NULL,
		Locked					bit				NOT NULL,
		FullName				AS (([FirstName]+' ')+[LastName])
	)  ON [PRIMARY]
GO

ALTER TABLE Admin.Tmp_User ADD CONSTRAINT
	DF_User_Lock DEFAULT ((0)) FOR Locked
GO

SET IDENTITY_INSERT Admin.Tmp_User ON
GO

INSERT INTO Admin.Tmp_User 
	(Id, BranchNo, Login, Password, LastChangePassword, FirstName, LastName, ExternalLogin, LegacyPassword, eMail, Locked)
SELECT 
	Id, 
	BranchNo, 
	[Login], 
	[Password], 
	CONVERT(smalldatetime, PasswordExpireDate), 
	FirstName, 
	LastName, 
	ExternalLogin, 
	LegacyPassword, 
	eMail, 
	Locked 
FROM 
	Admin.[User] WITH (HOLDLOCK TABLOCKX)
GO

SET IDENTITY_INSERT Admin.Tmp_User OFF
GO

ALTER TABLE Warehouse.Booking
	DROP CONSTRAINT FK_Booking_Assigned_By
GO

ALTER TABLE Admin.UserRole
	DROP CONSTRAINT FK_UserRole_UserID
GO

ALTER TABLE dbo.courtsperson
	DROP CONSTRAINT FK_CourtsPerson_User
GO

DROP TABLE Admin.[User]
GO

EXECUTE sp_rename N'Admin.Tmp_User', N'User', 'OBJECT' 
GO

ALTER TABLE Admin.[User] ADD CONSTRAINT
	PK_User PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO

CREATE UNIQUE NONCLUSTERED INDEX IX_User_Login ON Admin.[User]
	(
	[Login]
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

ALTER TABLE Admin.[User] ADD CONSTRAINT
	FK_User_branch FOREIGN KEY
	(
	BranchNo
	) REFERENCES dbo.branch
	(
	branchno
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO

ALTER TABLE dbo.courtsperson ADD CONSTRAINT
	FK_CourtsPerson_User FOREIGN KEY
	(
	UserId
	) REFERENCES Admin.[User]
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO

ALTER TABLE dbo.courtsperson SET (LOCK_ESCALATION = TABLE)
GO

ALTER TABLE Admin.UserRole ADD CONSTRAINT
	FK_UserRole_UserID FOREIGN KEY
	(
	UserId
	) REFERENCES Admin.[User]
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO

ALTER TABLE Admin.UserRole SET (LOCK_ESCALATION = TABLE)
GO

ALTER TABLE Warehouse.Booking ADD CONSTRAINT
	FK_Booking_Assigned_By FOREIGN KEY
	(
	PickingAssignedBy
	) REFERENCES Admin.[User]
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
