alter table Warehouse.Booking
drop constraint [CK_Booking_DeliveryOrCollection]

alter table Warehouse.Booking
add constraint [CK_Booking_DeliveryOrCollection] check (
	DeliveryOrCollection='R' OR 
	DeliveryOrCollection='C' OR 
	DeliveryOrCollection='D' OR 
	DeliveryOrCollection='A' OR 
	DeliveryOrCollection='Q' OR 
	DeliveryOrCollection='T')