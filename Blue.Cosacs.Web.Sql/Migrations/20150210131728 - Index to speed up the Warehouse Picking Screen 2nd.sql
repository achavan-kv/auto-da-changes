-- transaction: true

IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name='IX_WarehouseBooking_CustomerDelOrCollTruckPickup' AND object_id = OBJECT_ID('[Warehouse].[Booking]'))
BEGIN
	CREATE NONCLUSTERED INDEX IX_WarehouseBooking_CustomerDelOrCollTruckPickup
	ON [Warehouse].[Booking] ([CustomerName], [DeliveryOrCollectionDate], [TruckId], [PickUp])
END

UPDATE STATISTICS [Warehouse].[Booking]
