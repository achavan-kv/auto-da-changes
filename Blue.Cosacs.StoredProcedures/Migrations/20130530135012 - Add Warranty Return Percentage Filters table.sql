-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
CREATE TABLE [Warranty].[WarrantyReturnFilter] (
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[WarrantyReturnId] [int] NOT NULL,
	[LevelId] [int] NULL,
	[TagId] [int] NULL
)

ALTER TABLE [Warranty].[WarrantyReturnFilter]
ADD CONSTRAINT [PK_WarrantyReturnFilter] PRIMARY KEY (Id)

ALTER TABLE [Warranty].[WarrantyReturnFilter]
ADD CONSTRAINT [FK_WarrantyReturnFilter_WarrantyLevel] FOREIGN KEY([LevelId])
REFERENCES [Warranty].[Level] ([Id])
ON DELETE SET NULL

ALTER TABLE [Warranty].[WarrantyReturnFilter]
ADD CONSTRAINT [FK_WarrantyReturnFilter_WarrantyReturn] FOREIGN KEY([WarrantyReturnId])
REFERENCES [Warranty].[WarrantyReturn] ([Id])

ALTER TABLE [Warranty].[WarrantyReturnFilter]
ADD CONSTRAINT [FK_WarrantyReturnFilter_WarrantyTag] FOREIGN KEY([TagId])
REFERENCES [Warranty].[Tag] ([Id])
