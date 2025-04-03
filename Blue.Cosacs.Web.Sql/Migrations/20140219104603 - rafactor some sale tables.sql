-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

ALTER TABLE Sales.SalesOrderItem
	DROP CONSTRAINT FK_SalesOrderItem_StockInfo
GO

ALTER TABLE Sales.SalesOrderDetailWarranties
	DROP CONSTRAINT FK_SalesOrderDetailWarranties_Warranty
GO

ALTER TABLE Sales.SalesOrderItem
	DROP CONSTRAINT FK_SalesOrderItem_SalesOrder
GO

ALTER TABLE Sales.SalesOrderReturnedWarranty
	DROP CONSTRAINT FK_SalesOrderReturnedWarranty_SalesOrder
GO

ALTER TABLE Sales.SalesOrderReturnedItem
	DROP CONSTRAINT FK_SalesOrderReturnedItem_SalesOrder
GO

CREATE TABLE Sales.Tmp_SalesOrderItem
	(
	Id int NOT NULL IDENTITY (1, 1),
	SalesOrderId int NOT NULL,
	ItemId int NULL,
	ItemNumber varchar(18) NOT NULL,
	Description varchar(128) NOT NULL,
	UnityPrice dbo.BlueAmount NOT NULL,
	TaxRate dbo.BluePercentage NOT NULL,
	Quantity smallint NOT NULL,
	TaxAmount  AS CONVERT(decimal(19,3),round(UnityPrice * (TaxRate / 100.00) ,3, 1), 0)
	)  ON [PRIMARY]
GO

SET IDENTITY_INSERT Sales.Tmp_SalesOrderItem ON
GO

IF EXISTS(SELECT * FROM Sales.SalesOrderItem)
	 EXEC('INSERT INTO Sales.Tmp_SalesOrderItem (Id, SalesOrderId, ItemId, ItemNumber, Description, UnityPrice, TaxRate, Quantity)
		SELECT Id, SalesOrderId, ItemId, ItemNumber, Description, UnityPrice, TaxRate, Quantity FROM Sales.SalesOrderItem WITH (HOLDLOCK TABLOCKX)')
GO

SET IDENTITY_INSERT Sales.Tmp_SalesOrderItem OFF
GO

ALTER TABLE Sales.SalesOrderDetailWarranties
	DROP CONSTRAINT FK_SalesOrderDetailWarranties_SalesOrderItem
GO

ALTER TABLE Sales.SalesOrderDetailInstallation
	DROP CONSTRAINT FK_SalesOrderDetailInstallation_SalesOrderItem
GO

ALTER TABLE Sales.SalesOrderReturnedItem
	DROP CONSTRAINT FK_SalesOrderReturnedItem_SalesOrderItem
GO

DROP TABLE Sales.SalesOrderItem
GO

EXECUTE sp_rename N'Sales.Tmp_SalesOrderItem', N'SalesOrderItem', 'OBJECT' 
GO

ALTER TABLE Sales.SalesOrderItem ADD CONSTRAINT
	PK_SalesOrderItem PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO

ALTER TABLE Sales.SalesOrderItem ADD CONSTRAINT
	FK_SalesOrderItem_SalesOrder FOREIGN KEY
	(
	SalesOrderId
	) REFERENCES Sales.SalesOrder
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  CASCADE 
	
GO

CREATE TABLE Sales.Tmp_SalesOrderReturnedItem
	(
	Id smallint NOT NULL IDENTITY (1, 1),
	SaleOrderId int NOT NULL,
	ItemId int NULL,
	ItemNumber varchar(18) NOT NULL,
	Description varchar(128) NOT NULL,
	Quantity smallint NOT NULL
	)  ON [PRIMARY]
GO

SET IDENTITY_INSERT Sales.Tmp_SalesOrderReturnedItem ON
GO

IF EXISTS(SELECT * FROM Sales.SalesOrderReturnedItem)
	 EXEC('INSERT INTO Sales.Tmp_SalesOrderReturnedItem (Id, SaleOrderId, Quantity)
		SELECT Id, SaleOrderId, Quantity FROM Sales.SalesOrderReturnedItem WITH (HOLDLOCK TABLOCKX)')
GO

SET IDENTITY_INSERT Sales.Tmp_SalesOrderReturnedItem OFF
GO

DROP TABLE Sales.SalesOrderReturnedItem
GO

EXECUTE sp_rename N'Sales.Tmp_SalesOrderReturnedItem', N'SalesOrderReturnedItem', 'OBJECT' 
GO

ALTER TABLE Sales.SalesOrderReturnedItem ADD CONSTRAINT
	PK_SalesOrderReturnedItem PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO

ALTER TABLE Sales.SalesOrderReturnedItem ADD CONSTRAINT
	FK_SalesOrderReturnedItem_SalesOrder FOREIGN KEY
	(
	SaleOrderId
	) REFERENCES Sales.SalesOrder
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO

CREATE TABLE Sales.Tmp_SalesOrderItemInstallation
	(
	Id smallint NOT NULL IDENTITY (1, 1),
	SalesOrderItemId int NOT NULL,
	CostPrice dbo.BlueAmount NOT NULL,
	UnityPrice dbo.BlueAmount NOT NULL,
	TaxAmount  AS CONVERT(decimal(19,3), round(UnityPrice * (TaxRate / 100.00), 3, 1), 0),
	TaxRate dbo.BluePercentage NOT NULL
	)  ON [PRIMARY]
GO

SET IDENTITY_INSERT Sales.Tmp_SalesOrderItemInstallation ON
GO

IF EXISTS(SELECT * FROM Sales.SalesOrderDetailInstallation)
	 EXEC('INSERT INTO Sales.Tmp_SalesOrderItemInstallation (Id, SalesOrderItemId, CostPrice, UnityPrice, TaxRate)
		SELECT Id, SalesOrderItemId, CostPrice, UnitPrice, TaxRate FROM Sales.SalesOrderDetailInstallation WITH (HOLDLOCK TABLOCKX)')
GO

SET IDENTITY_INSERT Sales.Tmp_SalesOrderItemInstallation OFF
GO

DROP TABLE Sales.SalesOrderDetailInstallation
GO

EXECUTE sp_rename N'Sales.Tmp_SalesOrderItemInstallation', N'SalesOrderItemInstallation', 'OBJECT' 
GO

ALTER TABLE Sales.SalesOrderItemInstallation ADD CONSTRAINT
	PK_SalesOrderDetailInstallation PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO

ALTER TABLE Sales.SalesOrderItemInstallation ADD CONSTRAINT
	FK_SalesOrderDetailInstallation_SalesOrderItem FOREIGN KEY
	(
	SalesOrderItemId
	) REFERENCES Sales.SalesOrderItem
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  CASCADE 
	
GO

CREATE TABLE Sales.Tmp_SalesOrderItemWarranties
	(
	Id smallint NOT NULL IDENTITY (1, 1),
	SalesOrderItemId int NOT NULL,
	WarrantyId int NULL,
	Number varchar(20) NOT NULL,
	Description varchar(32) NOT NULL,
	Length tinyint NOT NULL,
	Price dbo.BlueAmount NOT NULL,
	TaxRate dbo.BluePercentage NOT NULL,
	TaxAmount  AS CONVERT(decimal(19,3), round(Price * (TaxRate / 100.00), 3, 1), 0),
	ItemSeriallNumber varchar(128) NULL
	)  ON [PRIMARY]
GO

ALTER TABLE Sales.SalesOrderReturnedWarranty
	DROP CONSTRAINT FK_SalesOrderReturnedWarranty_SalesOrderDetailWarranties
GO

DROP TABLE Sales.SalesOrderDetailWarranties
GO

EXECUTE sp_rename N'Sales.Tmp_SalesOrderItemWarranties', N'SalesOrderItemWarranties', 'OBJECT' 
GO

ALTER TABLE Sales.SalesOrderItemWarranties ADD CONSTRAINT
	PK_SalesOrderDetailWarranties PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO

ALTER TABLE Sales.SalesOrderItemWarranties ADD CONSTRAINT
	FK_SalesOrderDetailWarranties_SalesOrderItem FOREIGN KEY
	(
	SalesOrderItemId
	) REFERENCES Sales.SalesOrderItem
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  CASCADE 
GO

ALTER TABLE Sales.SalesOrderItemWarranties ADD CONSTRAINT
	FK_SalesOrderDetailWarranties_Warranty FOREIGN KEY
	(
	WarrantyId
	) REFERENCES Warranty.Warranty
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO

CREATE TABLE Sales.Tmp_SalesOrderReturnedWarranty
	(
	Id smallint NOT NULL,
	Number varchar(20) NOT NULL,
	Description varchar(32) NOT NULL,
	SaleOrderId int NOT NULL,
	SalesOrderDetailWarrantiesId smallint NOT NULL,
	Amount dbo.BlueAmount NOT NULL
	)  ON [PRIMARY]
GO

IF EXISTS(SELECT * FROM Sales.SalesOrderReturnedWarranty)
	 EXEC('INSERT INTO Sales.Tmp_SalesOrderReturnedWarranty (Id, SaleOrderId, SalesOrderDetailWarrantiesId, Amount)
		SELECT Id, SaleOrderId, SalesOrderDetailWarrantiesId, Amount FROM Sales.SalesOrderReturnedWarranty WITH (HOLDLOCK TABLOCKX)')
GO

DROP TABLE Sales.SalesOrderReturnedWarranty
GO

EXECUTE sp_rename N'Sales.Tmp_SalesOrderReturnedWarranty', N'SalesOrderReturnedWarranty', 'OBJECT' 
GO

ALTER TABLE Sales.SalesOrderReturnedWarranty ADD CONSTRAINT
	PK_SalesOrderReturnedWarranty PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO

ALTER TABLE Sales.SalesOrderReturnedWarranty ADD CONSTRAINT
	FK_SalesOrderReturnedWarranty_SalesOrder FOREIGN KEY
	(
	SaleOrderId
	) REFERENCES Sales.SalesOrder
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO