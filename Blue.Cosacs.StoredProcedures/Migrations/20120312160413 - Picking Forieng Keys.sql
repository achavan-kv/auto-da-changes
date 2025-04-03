-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

ALTER TABLE Warehouse.Picking ADD CONSTRAINT FK_Picking_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES CourtsPerson(EmpeeNo)
ALTER TABLE Warehouse.Picking ADD CONSTRAINT FK_Picking_CheckedBy FOREIGN KEY (CheckedBy) REFERENCES CourtsPerson(EmpeeNo)
ALTER TABLE Warehouse.Picking ADD CONSTRAINT FK_Picking_ConfirmedBy FOREIGN KEY (ConfirmedBy) REFERENCES CourtsPerson(EmpeeNo)
ALTER TABLE Warehouse.Picking ADD CONSTRAINT FK_Picking_PickedBy FOREIGN KEY (PickedBy) REFERENCES CourtsPerson(EmpeeNo)
ALTER TABLE Warehouse.PickingItem ADD CONSTRAINT FK_PickingItem_AssignedBy FOREIGN KEY (AssignedBy) REFERENCES CourtsPerson(EmpeeNo)
