-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
insert into Hub.[Queue] (Id, Name) values (1, 'Warehouse.Booking.Submit')
go
insert into Hub.[Queue] (Id, Name) values (2, 'Warehouse.Booking.Cancel')
go