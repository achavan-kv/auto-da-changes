IF EXISTS (SELECT * FROM sys.objects so
           INNER JOIN sys.schemas ss ON so.schema_id = ss.schema_id
           WHERE so.type = 'V'
           AND so.NAME = 'StatusView'
           AND ss.name = 'Warehouse')
DROP VIEW  Warehouse.StatusView
GO

CREATE VIEW Warehouse.StatusView
AS
SELECT Booking.Id,
       StockBranch,
       DeliveryBranch,
       Exception,
       DeliveryRejected,
       ScheduleRejected,
       PickingRejected,
       PickingAssignedBy,
       c.id AS CancelledId,
       case when CurrentQuantity = 0 then 1 else 0 end as Closed,
       DeliveryOrCollection,
       PickUpNotePrintedBy
FROM Warehouse.Booking
LEFT JOIN Warehouse.Cancellation c ON Warehouse.Booking.Id = C.Id
