alter table Warranty.WarrantyPromotion
drop column LevelFilters

alter table Warranty.WarrantyPromotion
add WarrantyId int

alter table Warranty.WarrantyPromotion
add constraint FK_WarrantyPromotion_WarrantyId foreign key (WarrantyId)
references [Warranty].[Warranty] ([Id])

create table Warranty.WarrantyPromotionFilter
(
	Id int IDENTITY(1,1) NOT NULL,
	WarrantyPromotionId int NOT NULL,
	LevelId int,
	TagId int
)

ALTER TABLE Warranty.WarrantyPromotionFilter
ADD CONSTRAINT [FK_WarrantyPromotionFilter_WarrantyPromotion]
FOREIGN KEY ([WarrantyPromotionId])
REFERENCES [Warranty].[WarrantyPromotion] ([Id])
ON DELETE NO ACTION

ALTER TABLE Warranty.WarrantyPromotionFilter
ADD CONSTRAINT [FK_WarrantyPromotionFilter_WarrantyLevel]
FOREIGN KEY ([LevelId])
REFERENCES [Warranty].[Level] ([Id])
ON DELETE SET NULL

ALTER TABLE Warranty.WarrantyPromotionFilter
ADD CONSTRAINT [FK_WarrantyPromotionFilter_WarrantyTag]
FOREIGN KEY ([TagId])
REFERENCES [Warranty].[Tag] ([Id])
ON DELETE NO ACTION
