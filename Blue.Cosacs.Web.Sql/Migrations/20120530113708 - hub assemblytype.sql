UPDATE hub.Queue
SET SubscriberTypeName = 'Blue.Cosacs.Subscribers.BookingDeliver'
WHERE name = 'Cosacs.Booking.Deliver'

UPDATE hub.Queue
SET SubscriberTypeName = 'Blue.Cosacs.Subscribers.BookingCancel'
WHERE name = 'Cosacs.Booking.Cancel'