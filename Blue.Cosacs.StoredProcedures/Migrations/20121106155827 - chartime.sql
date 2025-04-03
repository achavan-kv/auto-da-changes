ALTER TABLE Service.Technician 
ADD CONSTRAINT DF_Technician_StartTime DEFAULT '0800' FOR StartTime
GO
ALTER TABLE Service.Technician 
ADD CONSTRAINT DF_Technician_EndTime DEFAULT '1700' FOR EndTime
GO

ALTER TABLE Service.Technician 
ADD CONSTRAINT DF_Technician_Slots DEFAULT '8' FOR Slots
GO