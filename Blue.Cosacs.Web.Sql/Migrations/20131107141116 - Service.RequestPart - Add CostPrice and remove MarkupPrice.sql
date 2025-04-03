alter table Service.RequestPart
add CostPrice money
go

update Service.RequestPart
set CostPrice = Price
go

update Service.RequestPart
set Price = MarkupPrice
where MarkupPrice is not null and MarkupPrice > 0
go

alter table Service.RequestPart
drop column MarkupPrice