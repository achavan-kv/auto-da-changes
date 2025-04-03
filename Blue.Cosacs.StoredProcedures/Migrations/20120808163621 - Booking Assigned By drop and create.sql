-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
ALTER TABLE Warehouse.Booking
	DROP CONSTRAINT FK_Booking_AssignedBy
GO

ALTER TABLE Warehouse.Booking ADD CONSTRAINT
	FK_Booking_Assigned_By FOREIGN KEY
	(
	PickingAssignedBy
	) REFERENCES Admin.[User]
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO