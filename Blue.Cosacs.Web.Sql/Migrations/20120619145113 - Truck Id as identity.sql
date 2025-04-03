-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

ALTER TABLE Warehouse.Truck
	DROP CONSTRAINT FK_Truck_Driver
GO
ALTER TABLE Warehouse.Truck
	DROP CONSTRAINT FK_Truck_Branch
GO
CREATE TABLE Warehouse.Tmp_Truck
	(
	Id int NOT NULL IDENTITY (1, 1),
	Name varchar(50) NULL,
	Branch smallint NOT NULL,
	DriverId int NULL
	)  ON [PRIMARY]
GO
ALTER TABLE Warehouse.Tmp_Truck SET (LOCK_ESCALATION = TABLE)
GO
SET IDENTITY_INSERT Warehouse.Tmp_Truck ON
GO
IF EXISTS(SELECT * FROM Warehouse.Truck)
	 EXEC('INSERT INTO Warehouse.Tmp_Truck (Id, Name, Branch, DriverId)
		SELECT Id, Name, Branch, DriverId FROM Warehouse.Truck WITH (HOLDLOCK TABLOCKX)')
GO
SET IDENTITY_INSERT Warehouse.Tmp_Truck OFF
GO
ALTER TABLE Warehouse.Booking
	DROP CONSTRAINT FK_Booking_Truck
GO
DROP TABLE Warehouse.Truck
GO
EXECUTE sp_rename N'Warehouse.Tmp_Truck', N'Truck', 'OBJECT' 
GO
ALTER TABLE Warehouse.Truck ADD CONSTRAINT
	PK_Truck PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE Warehouse.Truck ADD CONSTRAINT
	FK_Truck_Branch FOREIGN KEY
	(
	Branch
	) REFERENCES dbo.branch
	(
	branchno
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE Warehouse.Truck ADD CONSTRAINT
	FK_Truck_Driver FOREIGN KEY
	(
	DriverId
	) REFERENCES Warehouse.Driver
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE Warehouse.Booking ADD CONSTRAINT
	FK_Booking_Truck FOREIGN KEY
	(
	TruckId
	) REFERENCES Warehouse.Truck
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
