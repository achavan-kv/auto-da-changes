-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

CREATE TABLE Sales.SalesOrder
(	
	Id						INT NOT NULL IDENTITY (1, 1),
	CustomerId				varchar(20) NULL,
	CustomerTitle			VARCHAR(8)  SPARSE NULL,
	CustomerFisrtName		VARCHAR(32)  SPARSE NULL,
	CustomerLastName		VARCHAR(32)  SPARSE NULL,
	CustomerEmail			VARCHAR(128) SPARSE NULL,
	CustomerHomePhone		VARCHAR(32)  SPARSE NULL,
	CustomerMobilePhone		VARCHAR(32)  SPARSE NULL,
	CustomerAddressLine1	VARCHAR(64)  SPARSE NULL,
	CustomerAddressLine2	VARCHAR(64)  SPARSE NULL,
	CustomerTown			VARCHAR(32)  SPARSE NULL,
	CustomerPostalCode		VARCHAR(16)  SPARSE NULL,
	Amount					dbo.BlueAmount	 NOT NULL,
	NetAmount				dbo.BlueAmount   NOT NULL,
	Pos						TINYINT NULL,
	CreatedOn				smalldatetime NOT NULL,
	CreatedBy				INT NOT NULL
)
GO

ALTER TABLE Sales.SalesOrder ADD CONSTRAINT
	PK_SalesOrder PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE Sales.SalesOrder ADD CONSTRAINT
	FK_SalesOrder_User FOREIGN KEY
	(
		CreatedBy
	) REFERENCES Admin.[User]
	(
		Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE Sales.SalesOrder ADD CONSTRAINT
	FK_SalesOrder_customer FOREIGN KEY
	(
		CustomerId
	) REFERENCES dbo.customer
	(
		custid
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO

CREATE TABLE Sales.SalesOrderPayment
(
	Id				INT NOT NULL IDENTITY (1, 1),
	SalesOrderId	INT NOT NULL,
	Amount			dbo.BlueAmount NOT NULL,
	Method			VarChar(32) NOT NULL
)
GO

ALTER TABLE Sales.SalesOrderPayment ADD CONSTRAINT
	PK_SalesOrderPayment PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO

ALTER TABLE Sales.SalesOrderPayment ADD CONSTRAINT
	FK_SalesOrderPayment_SalesOrder FOREIGN KEY
	(
		SalesOrderId
	) REFERENCES Sales.SalesOrder
	(
		Id
	) 
	 ON UPDATE  NO ACTION 
	 ON DELETE  CASCADE 
	
GO

CREATE TABLE Sales.SalesOrderItem
(
	Id				INT NOT NULL IDENTITY (1, 1),
	SalesOrderId	INT NOT NULL,
	ItemId			INT NOT NULL,
	ItemNumber		VARCHAR(18) NOT NULL,
	Description		VARCHAR(64) NOT NULL,
	UnityPrice		dbo.BlueAmount NOT NULL,
	TaxRate			dbo.BluePercentage NOT NULL,
	Quantity		SMALLINT NOT NULL,
	TaxAmount       AS (CONVERT(Decimal(19, 3), ROUND(UnityPrice *  (1 + (TaxRate / 100.00)), 3, 1)))
)  
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
	) 
	 ON UPDATE  NO ACTION 
	 ON DELETE  CASCADE 
	
GO
ALTER TABLE Sales.SalesOrderItem ADD CONSTRAINT
	FK_SalesOrderItem_StockInfo FOREIGN KEY
	(
		ItemId
	) REFERENCES dbo.StockInfo
	(
		Id
	) 
	 ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO

CREATE TABLE Sales.SalesOrderDetailWarranties
(
	Id					SMALLINT NOT NULL IDENTITY (1, 1),
	SalesOrderItemId	INT NOT NULL,
	WarrantyId			INT NOT NULL,
	WarrantyLength		tinyint NOT NULL,
	WarrantyPrice		dbo.BlueAmount NOT NULL,
	WarrantyTaxRate		dbo.BluePercentage NOT NULL,
	ItemSeriallNumber	VARCHAR(128) NULL
)
GO

ALTER TABLE Sales.SalesOrderDetailWarranties ADD CONSTRAINT
	PK_SalesOrderDetailWarranties PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE Sales.SalesOrderDetailWarranties ADD CONSTRAINT
	FK_SalesOrderDetailWarranties_SalesOrderItem FOREIGN KEY
	(
		SalesOrderItemId
	) REFERENCES Sales.SalesOrderItem
	(
		Id
	) 
	 ON UPDATE  NO ACTION 
	 ON DELETE  CASCADE 
	
GO
ALTER TABLE Sales.SalesOrderDetailWarranties ADD CONSTRAINT
	FK_SalesOrderDetailWarranties_Warranty FOREIGN KEY
	(
		WarrantyId
	) REFERENCES Warranty.Warranty
	(
		Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO

CREATE TABLE Sales.SalesOrderDetailInstallation
(
	Id					smallint NOT NULL IDENTITY (1, 1),
	SalesOrderItemId	int NOT NULL,
	CostPrice			dbo.BlueAmount NOT NULL,
	UnitPrice    		dbo.BlueAmount NOT NULL,
	TaxRate				dbo.BluePercentage NOT NULL,
	CreatedOn			smalldatetime NOT NULL,
	CreatedBy			int NOT NULL
)  
GO
ALTER TABLE Sales.SalesOrderDetailInstallation ADD CONSTRAINT
	PK_SalesOrderDetailInstallation PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE Sales.SalesOrderDetailInstallation ADD CONSTRAINT
	FK_SalesOrderDetailInstallation_SalesOrderItem FOREIGN KEY
	(
		SalesOrderItemId
	) REFERENCES Sales.SalesOrderItem
	(
		Id
	) 
	 ON UPDATE  NO ACTION 
	 ON DELETE  CASCADE 
	
GO
