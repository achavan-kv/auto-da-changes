update hub.queue
set SubscriberClrAssemblyName = null
   ,SubscriberClrTypeName = null
   ,SubscriberHttpUrl = '/cosacs/Warehouse/BookingSubmit'
   ,SubscriberHttpMethod = 'POST'
where Binding = 'Warehouse.Booking.Submit'

update hub.queue
set SubscriberClrAssemblyName = null
   ,SubscriberClrTypeName = null
   ,SubscriberHttpUrl = '/cosacs/Warehouse/BookingCancel'
   ,SubscriberHttpMethod = 'POST'
where Binding = 'Warehouse.Booking.Cancel'