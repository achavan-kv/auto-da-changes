-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
CREATE TABLE [Warranty].[WarrantyReturn](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[WarrantyLength] [smallint] NULL,
	[ElapsedMonths] [smallint] NOT NULL,
	[PercentageReturn] [decimal](4, 2) NOT NULL,
	[BranchType] [varchar](20) NULL,
	[BranchNumber] [smallint] NULL,
	[WarrantyId] [int] NULL
)

ALTER TABLE [Warranty].[WarrantyReturn]
ADD CONSTRAINT [PK_WarrantyReturn] PRIMARY KEY (Id)

ALTER TABLE [Warranty].[WarrantyReturn]  
ADD CONSTRAINT [FK_WarrantyReturn_branch] FOREIGN KEY([BranchNumber])
REFERENCES [dbo].[branch] ([branchno])

ALTER TABLE [Warranty].[WarrantyReturn]
ADD CONSTRAINT [FK_WarrantyReturn_Warranty] FOREIGN KEY([WarrantyId])
REFERENCES [Warranty].[Warranty] ([Id])
ON DELETE CASCADE
