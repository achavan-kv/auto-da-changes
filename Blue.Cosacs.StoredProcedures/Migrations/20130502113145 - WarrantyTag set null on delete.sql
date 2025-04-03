ALTER TABLE [Warranty].[WarrantyTags]
DROP CONSTRAINT [FK_ProductWarrantyTags_Tag]


ALTER TABLE [Warranty].[WarrantyTags] WITH CHECK ADD CONSTRAINT [FK_WarrantyTags_Tag] FOREIGN KEY([TagId])
REFERENCES [Warranty].[Tag] ([Id])
ON DELETE SET NULL
GO
