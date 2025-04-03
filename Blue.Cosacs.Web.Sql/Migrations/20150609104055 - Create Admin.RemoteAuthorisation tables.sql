
IF OBJECT_ID(N'Admin.RemoteAuthorisationBranch', N'U') IS NOT NULL
BEGIN
    ALTER TABLE [Admin].[RemoteAuthorisationBranch] DROP CONSTRAINT FK_Admin_RemoteAuthorisationBranch_RemoteAuthorisation_Id;
    ALTER TABLE [Admin].[RemoteAuthorisationBranch] DROP CONSTRAINT PK_Admin_RemoteAuthorisationBranch_Id;
    DROP TABLE [Admin].[RemoteAuthorisationBranch];
END


IF OBJECT_ID(N'Admin.RemoteAuthorisation', N'U') IS NOT NULL
BEGIN
    ALTER TABLE [Admin].[RemoteAuthorisation] DROP CONSTRAINT PK_Admin_RemoteAuthorisation_Id;
    DROP TABLE [Admin].[RemoteAuthorisation];
END



CREATE TABLE [Admin].[RemoteAuthorisation] (
    Id INT NOT NULL IDENTITY (1, 1),
    BranchId SMALLINT NOT NULL
) ON [PRIMARY];

ALTER TABLE [Admin].[RemoteAuthorisation]
ADD CONSTRAINT PK_Admin_RemoteAuthorisation_Id PRIMARY KEY (Id) ON [PRIMARY];



CREATE TABLE [Admin].[RemoteAuthorisationBranch] (
    Id INT NOT NULL IDENTITY (1, 1),
    RemoteAuthorisationId INT NOT NULL,
    BranchId SMALLINT NOT NULL
) ON [PRIMARY];

ALTER TABLE [Admin].[RemoteAuthorisationBranch]
ADD CONSTRAINT PK_Admin_RemoteAuthorisationBranch_Id PRIMARY KEY (Id) ON [PRIMARY];



ALTER TABLE [Admin].[RemoteAuthorisationBranch]
ADD CONSTRAINT FK_Admin_RemoteAuthorisationBranch_RemoteAuthorisation_Id FOREIGN KEY(RemoteAuthorisationId) REFERENCES [Admin].[RemoteAuthorisation](Id)
ON UPDATE NO ACTION
ON DELETE NO ACTION

