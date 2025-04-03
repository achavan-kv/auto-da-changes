UPDATE hub.Queue
SET SubscriberTypeName = 'Blue.Cosacs.Warehouse.Subscribers.WarehouseSubmit'
WHERE name = 'Warehouse.Booking.Submit'


UPDATE hub.Queue
SET SubscriberTypeName = 'Blue.Cosacs.Warehouse.Subscribers.WarehouseCancel'
WHERE name = 'Warehouse.Booking.Cancel'