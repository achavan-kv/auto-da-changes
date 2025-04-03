ALTER TABLE Service.TechnicianBooking
ADD [Type] Varchar(12)
GO

UPDATE Service.TechnicianBooking
SET [Type] = 'Repair'
GO

ALTER TABLE Service.TechnicianBooking
ALTER COLUMN [Type] Varchar(12) NOT NULL
GO