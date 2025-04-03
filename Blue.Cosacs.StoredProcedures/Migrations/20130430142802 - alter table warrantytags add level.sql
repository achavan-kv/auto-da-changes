ALTER TABLE Warranty.WarrantyTags
ADD Level INT NOT NULL
GO

ALTER TABLE Warranty.WarrantyTags
ADD CONSTRAINT UC_Level UNIQUE (WarrantyID, Level)
GO
