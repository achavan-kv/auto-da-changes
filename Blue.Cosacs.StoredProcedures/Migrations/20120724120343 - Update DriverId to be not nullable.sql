-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF EXISTS(select * from syscolumns where name = 'DriverId' and object_name(id) = 'Truck')
BEGIN
	alter table Warehouse.Truck alter column DriverId int NOT NULL 
END	

IF EXISTS(select * from syscolumns where name = 'DriverId' and object_name(id) = 'Load')
BEGIN
	alter table Warehouse.Load alter column DriverId int NOT NULL 
END	



