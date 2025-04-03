-- transaction: true

IF EXISTS (SELECT * FROM sys.indexes 
           WHERE name='IDX_TruckId_PickingId_DeliverQuantity_Exception_PickUp' AND OBJECT_ID = OBJECT_ID('Warehouse.Booking'))
BEGIN
    DROP INDEX [IDX_TruckId_PickingId_DeliverQuantity_Exception_PickUp] ON [Warehouse].[Booking]
END

IF EXISTS (SELECT * FROM sys.indexes 
           WHERE name='IDX_TruckId_ScheduleRejected_Exception_inc_OriginalId' AND OBJECT_ID = OBJECT_ID('Warehouse.Booking'))
BEGIN
    DROP INDEX [IDX_TruckId_ScheduleRejected_Exception_inc_OriginalId] ON [Warehouse].[Booking]
END


CREATE NONCLUSTERED INDEX IDX_TruckId_PickingId_DeliverQuantity_Exception_PickUp ON [Warehouse].[Booking] (
    [TruckId], [PickingId], [DeliverQuantity], [Exception], [PickUp]
)

CREATE NONCLUSTERED INDEX IDX_TruckId_ScheduleRejected_Exception_inc_OriginalId ON [Warehouse].[Booking] (
    [TruckId], [ScheduleRejected], [Exception]
) INCLUDE ([OriginalId])
