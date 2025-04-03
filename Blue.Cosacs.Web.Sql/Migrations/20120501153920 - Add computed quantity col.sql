
ALTER TABLE Warehouse.Booking
ADD CurrentQuantity AS COALESCE(ScheduleQuantity,LoadQuantity, PickQuantity,Quantity)