-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

ALTER TABLE Warehouse.Truck ADD Branch SMALLINT

GO

UPDATE Warehouse.Truck SET Branch = (SELECT TOP 1 branchno from dbo.branch)

GO

ALTER TABLE Warehouse.Truck ALTER COLUMN Branch SMALLINT NOT NULL

GO

ALTER TABLE Warehouse.Truck ADD CONSTRAINT FK_Truck_Branch FOREIGN KEY (Branch) REFERENCES Branch(BranchNo)