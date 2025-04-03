DELETE FROM SERVICE.technician

ALTER TABLE service.Technician
ADD StartTime TIME(0) NOT NULL 

ALTER TABLE service.Technician
ADD EndTime TIME(0) NOT NULL 

ALTER TABLE service.Technician
ADD Slots INT NOT NULL 



