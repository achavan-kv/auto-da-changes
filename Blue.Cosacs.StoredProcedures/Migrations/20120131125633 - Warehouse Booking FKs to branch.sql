-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
ALTER TABLE Warehouse.Booking ADD CONSTRAINT
	FK_Booking_StockBranch FOREIGN KEY
	(
	StockBranch
	) REFERENCES dbo.branch
	(
	branchno
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE Warehouse.Booking ADD CONSTRAINT
	FK_Booking_DeliveryBranch FOREIGN KEY
	(
	DeliveryBranch
	) REFERENCES dbo.branch
	(
	branchno
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
alter table Warehouse.Booking alter column DeliveryOrCollectionDate smalldatetime not null
GO