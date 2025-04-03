ALTER TABLE [Service].[TechnicianBookingDelete]
ADD [Type] varchar(12) 
GO

UPDATE [Service].[TechnicianBookingDelete]
SET [Type] = 'Repair'
GO

ALTER TABLE [Service].[TechnicianBookingDelete]
ALTER COLUMN [Type] varchar(12) NOT NULL 
GO
