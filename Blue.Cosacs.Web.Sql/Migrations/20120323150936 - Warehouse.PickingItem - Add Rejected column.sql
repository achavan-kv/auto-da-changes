-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

ALTER TABLE Warehouse.PickingItem ADD Rejected BIT NULL

GO

UPDATE Warehouse.PickingItem SET Rejected = 0

GO

ALTER TABLE Warehouse.PickingItem ALTER COLUMN Rejected BIT NOT NULL