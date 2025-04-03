-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
drop table Warehouse.Truck
go

CREATE TABLE Warehouse.Truck
	(
	Id int NOT NULL,
	Name varchar(50) NULL
	)  ON [PRIMARY]
GO
ALTER TABLE Warehouse.Truck ADD CONSTRAINT
	PK_Truck PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE Warehouse.PickingItem ADD CONSTRAINT
	FK_PickingItem_Truck FOREIGN KEY
	(
	TruckId
	) REFERENCES Warehouse.Truck
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO