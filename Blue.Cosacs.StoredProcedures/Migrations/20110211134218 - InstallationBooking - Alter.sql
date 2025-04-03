ALTER TABLE dbo.InstallationBooking
DROP COLUMN Zone;

GO

ALTER TABLE dbo.InstallationBooking
ADD StartSlot SMALLINT NOT NULL,
	NoOfSlots SMALLINT NOT NULL,
	RebookingReasonCode VARCHAR(12) NULL,
	IsDeleted BIT NOT NULL DEFAULT(0),
	DeletedBy INT NULL,
	DeletedOn DATETIME NULL;
