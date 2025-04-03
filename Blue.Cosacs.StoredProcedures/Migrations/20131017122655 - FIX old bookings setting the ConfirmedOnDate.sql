-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
--then fix the old bookings setting the date of the schedule
UPDATE Warehouse.Booking
SET DeliveryConfirmedOnDate = (SELECT ConfirmedOn FROM Warehouse.Load WHERE Id = b.ScheduleId)
FROM warehouse.booking b
WHERE DeliveryConfirmedOnDate IS NULL

UPDATE Warehouse.Booking
SET DeliveryConfirmedOnDate = DeliveryConfirmedDate
WHERE DeliveryConfirmedOnDate IS NULL AND DeliveryConfirmedDate IS NOT NULL
