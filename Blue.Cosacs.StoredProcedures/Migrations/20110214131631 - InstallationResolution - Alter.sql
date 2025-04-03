ALTER TABLE dbo.InstallationResolution
ADD CreatedBy INT NULL,
	CreatedOn DATETIME NOT NULL DEFAULT(GETDATE()),
	LastUpdatedBy INT NULL,
	LastUpdatedOn DATETIME NULL;
GO

UPDATE dbo.InstallationResolution 
SET CreatedBy = 99999
WHERE CreatedBy IS NULL

ALTER TABLE dbo.InstallationResolution
ALTER COLUMN CreatedBy INT NOT NULL
