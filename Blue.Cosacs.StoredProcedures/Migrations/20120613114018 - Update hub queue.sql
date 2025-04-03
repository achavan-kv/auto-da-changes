UPDATE hub.Queue
SET SubscriberTypeName = 'Blue.Cosacs.Warehouse.Subscribers.BookingSubmit'
WHERE name = 'Warehouse.Booking.Submit'


UPDATE hub.Queue
SET SubscriberTypeName = 'Blue.Cosacs.Warehouse.Subscribers.BookingCancel'
WHERE name = 'Warehouse.Booking.Cancel'


UPDATE hub.Queue
SET SubscriberTypeName = 'Blue.Cosacs.Subscribers.WarehouseDeliver'
WHERE name = 'Cosacs.Booking.Deliver'


UPDATE hub.Queue
SET SubscriberTypeName = 'Blue.Cosacs.Subscribers.WarehouseCancel'
WHERE name = 'Cosacs.Booking.Cancel'
