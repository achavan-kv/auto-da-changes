ALTER TABLE Warranty.WarrantyTags
DROP CONSTRAINT PK_WarrantyProductWarrantyTags

ALTER TABLE Warranty.WarrantyTags
ADD Id int IDENTITY(1,1) NOT NULL

ALTER TABLE Warranty.WarrantyTags
ADD CONSTRAINT [PK_WarrantyWarrantyTags] PRIMARY KEY (Id)

ALTER TABLE Warranty.WarrantyTags
ADD CONSTRAINT [UQ_WarrantyTags_Warranty_Tag] UNIQUE NONCLUSTERED
(WarrantyId, TagId)
