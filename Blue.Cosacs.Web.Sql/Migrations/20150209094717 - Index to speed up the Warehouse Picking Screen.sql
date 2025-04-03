-- transaction: true

IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name='IX_WarehouseBooking_PickUp' AND object_id = OBJECT_ID('[Warehouse].[Booking]'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_WarehouseBooking_PickUp
    ON [Warehouse].[Booking] ([PickUp])
    INCLUDE ([Id], [CustomerName], [DeliveryBranch], [DeliveryOrCollectionDate])
END

UPDATE STATISTICS [Warehouse].[Booking]
