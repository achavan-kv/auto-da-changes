ALTER TABLE Warehouse.Booking
DROP COLUMN CurrentQuantity
GO

ALTER TABLE Warehouse.Booking
DROP COLUMN LoadQuantity
GO

ALTER TABLE Warehouse.Booking
ADD DeliverQuantity INT NULL
GO

ALTER TABLE Warehouse.Booking
ADD CurrentQuantity AS COALESCE(DeliverQuantity, ScheduleQuantity, PickQuantity,Quantity)
Go

