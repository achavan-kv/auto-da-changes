CREATE TABLE Warranty.WarrantyPromotionPrice
(
	Id int IDENTITY(1,1) NOT NULL,
	WarrantyPriceId int NOT NULL,
	StartDate smalldatetime NOT NULL,
	EndDate smalldatetime NOT NULL,
	RetailPrice money,
	PercentageOfRetailPrice decimal(4,2)
)

ALTER TABLE Warranty.WarrantyPromotionPrice
ADD CONSTRAINT [PK_WarrantyPromotionPrice] PRIMARY KEY (Id)

ALTER TABLE Warranty.WarrantyPromotionPrice
ADD CONSTRAINT [FK_WarrantyPromotionPrice_WarrantyPrice]
FOREIGN KEY ([WarrantyPriceId])
REFERENCES [Warranty].[WarrantyPrice] ([Id])
ON DELETE CASCADE
