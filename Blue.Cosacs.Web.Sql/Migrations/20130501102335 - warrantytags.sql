AlTER TABLE [Warranty].[WarrantyTags] 
DROP CONSTRAINT [UC_Level]
GO

alter table warranty.warrantyTags
drop column level
GO

alter table warranty.warrantyTags
ADD LevelId INT NOT NULL
GO

ALTER TABLE [Warranty].[WarrantyTags]  WITH CHECK ADD  CONSTRAINT [FK_Level_Tag] FOREIGN KEY([LevelId])
REFERENCES [Warranty].[Level] ([Id])
GO

ALTER TABLE [Warranty].[WarrantyTags] CHECK CONSTRAINT [FK_Level_Tag]
GO


ALTER TABLE [Warranty].[WarrantyTags] ADD  CONSTRAINT [UC_Level] UNIQUE NONCLUSTERED 
(
	[WarrantyId] ASC,
	[LevelId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
