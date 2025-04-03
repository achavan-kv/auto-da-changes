UPDATE Warehouse.Booking
SET orderedon = GETDATE(),
    damaged = 0,
    assemblyreq = 0
    
ALTER TABLE Warehouse.Booking
ALTER COLUMN OrderedOn DATETIME NOT NULL


ALTER TABLE Warehouse.Booking
ALTER COLUMN damaged BIT NOT NULL

ALTER TABLE Warehouse.Booking
ALTER COLUMN assemblyreq BIT NOT NULL