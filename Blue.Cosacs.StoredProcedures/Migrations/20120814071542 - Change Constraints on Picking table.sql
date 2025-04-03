-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[Warehouse].[FK_Picking_CheckedBy]') AND parent_object_id = OBJECT_ID(N'[Warehouse].[Picking]'))
ALTER TABLE [Warehouse].[Picking] DROP CONSTRAINT [FK_Picking_CheckedBy]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[Warehouse].[FK_Picking_ConfirmedBy]') AND parent_object_id = OBJECT_ID(N'[Warehouse].[Picking]'))
ALTER TABLE [Warehouse].[Picking] DROP CONSTRAINT [FK_Picking_ConfirmedBy]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[Warehouse].[FK_Picking_CreatedBy]') AND parent_object_id = OBJECT_ID(N'[Warehouse].[Picking]'))
ALTER TABLE [Warehouse].[Picking] DROP CONSTRAINT [FK_Picking_CreatedBy]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[Warehouse].[FK_Picking_PickedBy]') AND parent_object_id = OBJECT_ID(N'[Warehouse].[Picking]'))
ALTER TABLE [Warehouse].[Picking] DROP CONSTRAINT [FK_Picking_PickedBy]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[Warehouse].[FK_Load_CreatedBy]') AND parent_object_id = OBJECT_ID(N'[Warehouse].[Load]'))
ALTER TABLE [Warehouse].[Load] DROP CONSTRAINT [FK_Load_CreatedBy]
GO



ALTER TABLE [Warehouse].[Picking]  WITH CHECK ADD  CONSTRAINT [FK_Picking_CheckedBy] FOREIGN KEY([CheckedBy])
REFERENCES Admin.[user] ([Id])
GO

ALTER TABLE [Warehouse].[Picking] CHECK CONSTRAINT [FK_Picking_CheckedBy]
GO

ALTER TABLE [Warehouse].[Picking]  WITH CHECK ADD  CONSTRAINT [FK_Picking_ConfirmedBy] FOREIGN KEY([ConfirmedBy])
REFERENCES Admin.[user] ([Id])
GO

ALTER TABLE [Warehouse].[Picking] CHECK CONSTRAINT [FK_Picking_ConfirmedBy]
GO

ALTER TABLE [Warehouse].[Picking]  WITH CHECK ADD  CONSTRAINT [FK_Picking_CreatedBy] FOREIGN KEY([Createdby])
REFERENCES Admin.[user] ([Id])
GO

ALTER TABLE [Warehouse].[Picking] CHECK CONSTRAINT [FK_Picking_CreatedBy]
GO

ALTER TABLE [Warehouse].[Picking]  WITH CHECK ADD  CONSTRAINT [FK_Picking_PickedBy] FOREIGN KEY([PickedBy])
REFERENCES Admin.[user] ([Id])
GO

ALTER TABLE [Warehouse].[Picking] CHECK CONSTRAINT [FK_Picking_PickedBy]
GO

ALTER TABLE [Warehouse].[Load]  WITH CHECK ADD  CONSTRAINT [FK_Load_CreatedBy] FOREIGN KEY([Createdby])
REFERENCES Admin.[user] ([Id])
GO

ALTER TABLE [Warehouse].[Load] CHECK CONSTRAINT [FK_Load_CreatedBy]
GO