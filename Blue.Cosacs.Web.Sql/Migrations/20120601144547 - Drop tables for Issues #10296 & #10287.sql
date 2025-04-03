-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'Warehouse.Loaditem') AND type in (N'U'))
DROP TABLE Warehouse.Loaditem
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'Warehouse.PickingItem') AND type in (N'U'))
DROP TABLE Warehouse.PickingItem
GO
