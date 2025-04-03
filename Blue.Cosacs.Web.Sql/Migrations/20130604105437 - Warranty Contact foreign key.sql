alter table [Warranty].[WarrantyContact]
drop constraint FK_WarrantySale_Contact

ALTER TABLE [Warranty].[WarrantyContact] WITH CHECK ADD CONSTRAINT [FK_WarrantyContact_WarrantySale] FOREIGN KEY([WarrantySaleId])
REFERENCES [Warranty].[WarrantySale] ([Id])
ON DELETE CASCADE
