alter table Service.Request
add WarrantyNumber varchar(50)
go

update Service.Request
set WarrantyNumber = cast(WarrantyContractId as varchar(50))
