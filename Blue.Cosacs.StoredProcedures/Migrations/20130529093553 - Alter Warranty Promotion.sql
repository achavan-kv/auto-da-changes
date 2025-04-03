sp_rename 'Warranty.WarrantyPromotionPrice', 'WarrantyPromotion', 'OBJECT'
go

sp_rename 'Warranty.WarrantyPromotion.PercentageOfRetailPrice', 'PercentageDiscount', 'COLUMN'
go

alter table Warranty.WarrantyPromotion
drop constraint FK_WarrantyPromotionPrice_WarrantyPrice

alter table Warranty.WarrantyPromotion
drop column WarrantyPriceId

alter table Warranty.WarrantyPromotion
add LevelFilters varchar(256)

alter table Warranty.WarrantyPromotion
add BranchType varchar(20)

alter table Warranty.WarrantyPromotion
add BranchNumber smallint

alter table Warranty.WarrantyPromotion
add constraint FK_WarrantyPromotion_BranchNumber foreign key (BranchNumber)
references [dbo].[branch] ([branchno])
