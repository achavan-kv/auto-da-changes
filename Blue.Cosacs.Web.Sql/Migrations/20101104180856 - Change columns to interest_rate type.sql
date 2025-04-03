alter table StoreCardRateAudit
 alter column RetailRateFixed interest_rate null
go

alter table StoreCardRateAudit
 alter column RetailRateVariable interest_rate null
go

alter table StoreCardRate
 alter column RetailRateFixed interest_rate null
go

alter table StoreCardRate
 alter column RetailRateVariable interest_rate null

go

alter table StoreCard
 alter column FixedRate interest_rate null
go
