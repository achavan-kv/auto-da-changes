IF NOT EXISTS (SELECT * FROM sys.columns
               WHERE OBJECT_NAME(object_id) = 'booking'
               AND name = 'PickingAssignedDate')
BEGIN
ALTER TABLE Warehouse.Booking
ADD PickingAssignedDate DATETIME
END

