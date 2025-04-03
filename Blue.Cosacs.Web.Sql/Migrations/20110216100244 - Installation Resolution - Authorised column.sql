ALTER TABLE dbo.InstallationResolution
ADD AuthorisedBy INT NULL,
	AuthorisedOn DATETIME NULL
GO

UPDATE dbo.InstallationResolution
SET AuthorisedBy = ISNULL(LastUpdatedBy, CreatedBy),
	AuthorisedOn = ISNULL(LastUpdatedOn, CreatedOn)
WHERE IsCompleted = 1
