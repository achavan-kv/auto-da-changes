-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

EXEC sp_RENAME 'Warehouse.Booking.ReceivingLocationId', 'ReceivingLocation', 'COLUMN'