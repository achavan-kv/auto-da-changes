-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
--
-- FIX to ISSUE #15486 
UPDATE Warehouse.Booking
SET DeliveryConfirmedDate = NULL
WHERE Id IN
(
	SELECT SV.Id 
	FROM Warehouse.StatusView SV inner join Warehouse.Booking B on B.Id = SV.Id
	WHERE SV.Closed = 0 
		AND SV.CancelledId IS NULL 
		AND SV.DeliveryRejected IS NULL
		AND B.DeliveryConfirmedDate IS NOT NULL
)