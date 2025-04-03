alter table warehouse.booking
drop column currentquantity
GO


ALTER TABLE Warehouse.Booking
ADD CurrentQuantity AS COALESCE(DeliverQuantity, ScheduleQuantity,PickQuantity,Quantity,0) 