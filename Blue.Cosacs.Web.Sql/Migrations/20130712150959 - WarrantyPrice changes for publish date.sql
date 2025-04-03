alter table Warranty.WarrantyPrice
add EffectiveDate date NOT NULL DEFAULT getdate()
