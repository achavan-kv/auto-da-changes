-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
ALTER TABLE Warehouse.PickingItem
	DROP CONSTRAINT FK_Picking
GO
ALTER TABLE Warehouse.PickingItem
	DROP CONSTRAINT FK_PickingBooking
GO
ALTER TABLE Warehouse.Booking SET (LOCK_ESCALATION = TABLE)
GO
CREATE TABLE Warehouse.Tmp_PickingItem
	(
	Id int NOT NULL,
	BookingId int NOT NULL,
	TruckId int NOT NULL,
	PickingId int NULL,
	Pickedby int NULL
	)  ON [PRIMARY]
GO
ALTER TABLE Warehouse.Tmp_PickingItem SET (LOCK_ESCALATION = TABLE)
GO
IF EXISTS(SELECT * FROM Warehouse.PickingItem)
	 EXEC('INSERT INTO Warehouse.Tmp_PickingItem (BookingId, TruckId, PickingId, Pickedby)
		SELECT BookingId, TruckId, PickingId, Pickedby FROM Warehouse.PickingItem WITH (HOLDLOCK TABLOCKX)')
GO
DROP TABLE Warehouse.PickingItem
GO
EXECUTE sp_rename N'Warehouse.Tmp_PickingItem', N'PickingItem', 'OBJECT' 
GO
ALTER TABLE Warehouse.PickingItem ADD CONSTRAINT
	PK_PickingItem PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE Warehouse.PickingItem ADD CONSTRAINT
	FK_PickingBooking FOREIGN KEY
	(
	BookingId
	) REFERENCES Warehouse.Booking
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE Warehouse.PickingItem ADD CONSTRAINT
	FK_Picking FOREIGN KEY
	(
	PickingId
	) REFERENCES Warehouse.Picking
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
