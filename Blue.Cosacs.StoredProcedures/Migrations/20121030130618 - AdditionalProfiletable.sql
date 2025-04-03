
CREATE TABLE Admin.AdditionalUserProfile
	(
	Id int NOT NULL IDENTITY (1, 1),
	UserId int NOT NULL,
	ProfileId int NOT NULL,
	Active bit NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE Admin.AdditionalUserProfile ADD CONSTRAINT
	PK_AdditionalUserProfile PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE Admin.AdditionalUserProfile SET (LOCK_ESCALATION = TABLE)
GO

ALTER TABLE admin.AdditionalUserProfile
ADD CONSTRAINT U_AdditionalUserProfile UNIQUE (UserId,ProfileId)

