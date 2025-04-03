ALTER TABLE Service.TechnicianBooking
ADD AllocatedOn DATETIME NULL
GO

UPDATE Service.TechnicianBooking
SET AllocatedOn = GETDATE()
GO

ALTER TABLE Service.TechnicianBooking
ALTER COLUMN AllocatedOn DATETIME NOT NULL
GO
