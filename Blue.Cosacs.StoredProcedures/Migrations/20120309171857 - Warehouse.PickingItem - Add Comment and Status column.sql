-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

ALTER TABLE Warehouse.PickingItem ADD Comment VARCHAR(4000) NULL
ALTER TABLE Warehouse.Picking ADD Comment VARCHAR(4000) NULL
ALTER TABLE Warehouse.PickingItem ADD Status VARCHAR(50) NULL
