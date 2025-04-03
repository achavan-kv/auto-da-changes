UPDATE Warehouse.Booking
SET acctno = 1
WHERE Acctno IS NULL


ALTER TABLE Warehouse.Booking 
ALTER COLUMN AcctNo Char(12) NOT NULL

