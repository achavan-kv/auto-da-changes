-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

ALTER TABLE Warehouse.Picking ADD PickedBy INT NULL
GO
ALTER TABLE Warehouse.Picking ADD PickedOn SMALLDATETIME NULL
GO
sp_rename 'Warehouse.Picking.EnteredBy', 'ConfirmedBy', 'COLUMN'
GO
ALTER TABLE Warehouse.PickingItem DROP COLUMN PickedBy