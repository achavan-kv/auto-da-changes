alter table Merchandising.StockAllocationProduct
add BookingId int null

alter table Merchandising.StockRequisitionProduct
add BookingId int null

alter table Merchandising.StockTransferProduct
add BookingId int null
go
update Merchandising.StockAllocationProduct
set BookingId = 0

update Merchandising.StockRequisitionProduct
set BookingId = 0

update Merchandising.StockTransferProduct
set BookingId = 0

alter table Merchandising.StockAllocationProduct
alter column BookingId int not null

alter table Merchandising.StockRequisitionProduct
alter column BookingId int not null

alter table Merchandising.StockTransferProduct
alter column BookingId int not null
