-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
CREATE TABLE Warehouse.DriverCommission
(
	Id			int IDENTITY(1,1)	NOT NULL,
	CreatedOn	smalldatetime		NOT NULL,
	CreatedBy	int					NOT NULL
) 
GO

ALTER TABLE Warehouse.DriverCommission ADD CONSTRAINT
	PK_DriverCommission PRIMARY KEY CLUSTERED 
	(
		Id
	) WITH
	( 
		STATISTICS_NORECOMPUTE = OFF, 
		IGNORE_DUP_KEY = OFF, 
		ALLOW_ROW_LOCKS = ON, 
		ALLOW_PAGE_LOCKS = ON
	) 
GO

ALTER TABLE Warehouse.[Load] ADD
	DriverCommissionId int NULL
GO

ALTER TABLE Warehouse.Load ADD CONSTRAINT
	FK_Load_DriverCommission FOREIGN KEY
	(
		DriverCommissionId
	) REFERENCES Warehouse.DriverCommission
	(
		Id
	) ON UPDATE NO ACTION ON DELETE NO ACTION 
	
GO

