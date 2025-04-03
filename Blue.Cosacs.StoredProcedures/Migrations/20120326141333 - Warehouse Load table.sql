-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


CREATE TABLE [Warehouse].[Load](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Createdby] [int] NOT NULL,
	[CreatedOn] [smalldatetime] NOT NULL,
	[DriverId] INT NULL
 CONSTRAINT [PK_Load] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [Warehouse].[Load]  WITH CHECK ADD  CONSTRAINT [FK_Load_Driver] FOREIGN KEY([DriverId])
REFERENCES [Warehouse].[Driver] ([Id])
GO

ALTER TABLE [Warehouse].[Load] CHECK CONSTRAINT [FK_Load_Driver]
GO

ALTER TABLE [Warehouse].[Load]  WITH CHECK ADD  CONSTRAINT [FK_Load_CreatedBy] FOREIGN KEY([Createdby])
REFERENCES [dbo].[courtsperson] ([empeeno])
GO

ALTER TABLE [Warehouse].[Load] CHECK CONSTRAINT [FK_Load_CreatedBy]
GO

CREATE TABLE [Warehouse].[LoadItem](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[LoadId] [int] NOT NULL,
	[PickingItemId] [int] NOT NULL,	
	[Comment] [varchar](4000) NULL,
	[Order] INT Null
 CONSTRAINT [PK_LoadItem] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [Warehouse].[LoadItem]  WITH CHECK ADD  CONSTRAINT [FK_LoadItem_PickingItem] FOREIGN KEY([PickingItemId])
REFERENCES [Warehouse].[PickingItem] ([Id])
GO

ALTER TABLE [Warehouse].[LoadItem] CHECK CONSTRAINT [FK_LoadItem_PickingItem]
GO

ALTER TABLE [Warehouse].[LoadItem]  WITH CHECK ADD  CONSTRAINT [FK_LoadItem_Load] FOREIGN KEY([LoadId])
REFERENCES [Warehouse].[Load] ([Id])
GO

ALTER TABLE [Warehouse].[LoadItem] CHECK CONSTRAINT [FK_LoadItem_Load]
GO

