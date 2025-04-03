ALTER TABLE Warehouse.Booking
ADD ScheduleRejected BIT NULL
GO

ALTER TABLE Warehouse.Booking
ADD ScheduleRejectedReason VARCHAR(50) NULL
GO

ALTER TABLE Warehouse.Booking
ADD DeliveryRejected BIT NULL
Go

ALTER TABLE Warehouse.Booking
ADD DeliveryRejectedReason VARCHAR(50) NULL
GO

ALTER TABLE Warehouse.Booking
ADD DeliveryConfirmedBy INT NULL
GO

ALTER TABLE Warehouse.Booking
ADD CONSTRAINT PickingRejectedCheck CHECK (NOT (PickingRejectedReason IS NOT NULL AND ISNULL(PickingRejected,0) != 1))
GO

ALTER TABLE Warehouse.Booking
ADD CONSTRAINT ScheduleRejectedCheck CHECK (NOT (ScheduleRejectedReason IS NOT NULL AND ISNULL(ScheduleRejected,0) != 1))
GO

ALTER TABLE Warehouse.Booking
ADD CONSTRAINT DeliveryRejectedCheck CHECK (NOT (DeliveryRejectedReason IS NOT NULL AND ISNULL(DeliveryRejected,0) != 1))
GO