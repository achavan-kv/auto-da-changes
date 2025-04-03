ALTER TABLE Warranty.WarrantyTags
DROP CONSTRAINT [UQ_WarrantyTags_Warranty_Tag]

ALTER TABLE Warranty.WarrantyTags
DROP CONSTRAINT [FK_Level_Tag]

ALTER TABLE [Warranty].[WarrantyTags] ADD CONSTRAINT [FK_WarrantyTags_Level] FOREIGN KEY([LevelId])
REFERENCES [Warranty].[Level] ([Id])
GO
