-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
IF OBJECT_ID('Sales.SalesOrderItemInstallation') IS NOT NULL
BEGIN 
	SET QUOTED_IDENTIFIER ON
	SET ARITHABORT ON
	SET NUMERIC_ROUNDABORT OFF
	SET CONCAT_NULL_YIELDS_NULL ON
	SET ANSI_NULLS ON
	SET ANSI_PADDING ON
	SET ANSI_WARNINGS ON

	DROP TABLE Sales.SalesOrderItemInstallation
	DROP TABLE Sales.SalesOrderItemWarranties
	DROP TABLE Sales.SalesOrderPayment
	DROP TABLE Sales.SalesOrderReturnedItem
	DROP TABLE Sales.SalesOrderReturnedWarranty
	DROP TABLE Sales.SalesOrderItem
	DROP TABLE Sales.SalesOrder

	CREATE TABLE Sales.SalesOrder
		(
		Id int NOT NULL IDENTITY (1, 1),
		TotalAmount dbo.BlueAmount NOT NULL,
		TotalTaxAmount dbo.BlueAmount NOT NULL,
		TotalDiscount dbo.BlueAmount NOT NULL,
		BranchNumber smallint NOT NULL,
		CreatedOn smalldatetime NOT NULL,
		CreatedBy int NOT NULL
		)  ON [PRIMARY]

	ALTER TABLE Sales.SalesOrder ADD CONSTRAINT
		PK_SalesOrder PRIMARY KEY CLUSTERED 
		(
		Id DESC
		) WITH( PAD_INDEX = ON, FILLFACTOR = 70, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

	CREATE TABLE Sales.OrderDetail
		(
		Id int NOT NULL,
		OrderId int NOT NULL,
		Price dbo.BlueAmount NULL
		)  ON [PRIMARY]

	ALTER TABLE Sales.OrderDetail ADD CONSTRAINT
		PK_OrderDetail PRIMARY KEY CLUSTERED 
		(
		Id
		) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]


	ALTER TABLE Sales.OrderDetail ADD CONSTRAINT
		FK_OrderDetail_SalesOrder FOREIGN KEY
		(
		OrderId
		) REFERENCES Sales.SalesOrder
		(
		Id
		) ON UPDATE  NO ACTION 
		 ON DELETE  CASCADE 

	CREATE TABLE Sales.OrderPayment
		(
		Id int NOT NULL,
		OrderId int NOT NULL,
		Amount dbo.BlueAmount NOT NULL,
		Method varchar(32) NOT NULL
		)  ON [PRIMARY]

	ALTER TABLE Sales.OrderPayment ADD CONSTRAINT
		PK_OrderPayment PRIMARY KEY CLUSTERED 
		(
		Id
		) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]


	ALTER TABLE Sales.OrderPayment ADD CONSTRAINT
		FK_OrderPayment_SalesOrder FOREIGN KEY
		(
		OrderId
		) REFERENCES Sales.SalesOrder
		(
		Id
		) ON UPDATE  NO ACTION 
		 ON DELETE  CASCADE 

	CREATE TABLE Sales.OrderCustomer
		(
		Id int NOT NULL IDENTITY (1, 1),
		OrderId int NOT NULL,
		Title varchar(8) SPARSE  NULL,
		FirstName varchar(32) NOT NULL,
		Middle varchar(64) SPARSE  NULL,
		LastName varchar(32) NOT NULL,
		Email varchar(64) NULL,
		HomePhone varchar(32) NULL,
		MobilePhone varchar(32) NULL,
		AddressLine1 varchar(64) NULL,
		AddressLine2 varchar(64) SPARSE  NULL,
		PostalCode varchar(16) NULL
		)  ON [PRIMARY]

	ALTER TABLE Sales.OrderCustomer ADD CONSTRAINT
		PK_OrderCustomer PRIMARY KEY CLUSTERED 
		(
		Id
		) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]


	ALTER TABLE Sales.OrderCustomer ADD CONSTRAINT
		FK_OrderCustomer_SalesOrder FOREIGN KEY
		(
		OrderId
		) REFERENCES Sales.SalesOrder
		(
		Id
		) ON UPDATE  NO ACTION 
		 ON DELETE  CASCADE 

	CREATE TABLE Sales.ReturnReason
		(
		Id tinyint NOT NULL,
		Description varchar(32) NOT NULL,
		CreatedBy int NOT NULL,
		CreatedOn smalldatetime NOT NULL
		)  ON [PRIMARY]

	ALTER TABLE Sales.ReturnReason ADD CONSTRAINT
		PK_ReturnReason PRIMARY KEY CLUSTERED 
		(
		Id
		) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

	CREATE TABLE Sales.ItemType
		(
		Id tinyint NOT NULL,
		Description varchar(128) NOT NULL,
		CreatedOn smalldatetime NOT NULL,
		CreatedBy int NOT NULL
		)  ON [PRIMARY]

	ALTER TABLE Sales.ItemType ADD CONSTRAINT
		PK_ItemType PRIMARY KEY CLUSTERED 
		(
		Id
		) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]



	ALTER TABLE Sales.OrderDetail
		DROP CONSTRAINT FK_OrderDetail_SalesOrder

	ALTER TABLE Sales.OrderPayment
		DROP CONSTRAINT FK_OrderPayment_SalesOrder

	CREATE TABLE Sales.SalesReturn
		(
		Id int NOT NULL IDENTITY (1, 1),
		OrderId int NOT NULL,
		ReturnDate date NOT NULL,
		TotalAmount dbo.BlueAmount NOT NULL,
		Comments varchar(1024) NULL,
		BranchNumber smallint NOT NULL,
		CreatedOn smalldatetime NOT NULL,
		CreatedBy int NOT NULL
		)  ON [PRIMARY]

	ALTER TABLE Sales.SalesReturn ADD CONSTRAINT
		PK_SalesReturn PRIMARY KEY CLUSTERED 
		(
		Id DESC
		) WITH( PAD_INDEX = ON, FILLFACTOR = 85, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]


	ALTER TABLE Sales.SalesReturn ADD CONSTRAINT
		FK_SalesReturn_SalesOrder FOREIGN KEY
		(
		OrderId
		) REFERENCES Sales.SalesOrder
		(
		Id
		) ON UPDATE  NO ACTION 
		 ON DELETE  NO ACTION 


	CREATE TABLE Sales.ReturnItem
		(
		Id int NOT NULL,
		ReturnId int NOT NULL,
		ItemTypeId tinyint NOT NULL,
		ReturnReasonId tinyint NOT NULL,
		Number varchar(32) NOT NULL,
		Description varchar(128) NOT NULL,
		Quantity smallint NOT NULL,
		Amount dbo.BlueAmount NOT NULL
		)  ON [PRIMARY]

	ALTER TABLE Sales.ReturnItem ADD CONSTRAINT
		PK_ReturnItem PRIMARY KEY CLUSTERED 
		(
		Id
		) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]


	ALTER TABLE Sales.ReturnItem ADD CONSTRAINT
		FK_ReturnItem_SalesReturn FOREIGN KEY
		(
		ReturnId
		) REFERENCES Sales.SalesReturn
		(
		Id
		) ON UPDATE  NO ACTION 
		 ON DELETE  CASCADE 
	

	ALTER TABLE Sales.ReturnItem ADD CONSTRAINT
		FK_ReturnItem_ItemType FOREIGN KEY
		(
		ItemTypeId
		) REFERENCES Sales.ItemType
		(
		Id
		) ON UPDATE  NO ACTION 
		 ON DELETE  NO ACTION 
	

	ALTER TABLE Sales.ReturnItem ADD CONSTRAINT
		FK_ReturnItem_ReturnReason FOREIGN KEY
		(
		ReturnReasonId
		) REFERENCES Sales.ReturnReason
		(
		Id
		) ON UPDATE  NO ACTION 
		 ON DELETE  NO ACTION 
	

	CREATE TABLE Sales.Tmp_OrderPayment
		(
		Id int NOT NULL IDENTITY (1, 1),
		OrderId int NOT NULL,
		Amount dbo.BlueAmount NOT NULL,
		Method varchar(32) NOT NULL
		)  ON [PRIMARY]


	DROP TABLE Sales.OrderPayment

	EXECUTE sp_rename N'Sales.Tmp_OrderPayment', N'OrderPayment', 'OBJECT' 

	ALTER TABLE Sales.OrderPayment ADD CONSTRAINT
		PK_OrderPayment PRIMARY KEY CLUSTERED 
		(
		Id
		) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]


	ALTER TABLE Sales.OrderPayment ADD CONSTRAINT
		FK_OrderPayment_SalesOrder FOREIGN KEY
		(
		OrderId
		) REFERENCES Sales.SalesOrder
		(
		Id
		) ON UPDATE  NO ACTION 
		 ON DELETE  CASCADE 
	

	CREATE TABLE Sales.Tmp_OrderDetail
		(
		Id int NOT NULL IDENTITY (1, 1),
		OrderId int NOT NULL,
		ItemTypeId tinyint NOT NULL,
		Number varchar(32) NOT NULL,
		Description varchar(128) NOT NULL,
		Quantity smallint NOT NULL,
		Price dbo.BlueAmount NOT NULL,
		TaxRate dbo.BluePercentage NOT NULL,
		TaxAmount  AS (CONVERT([decimal](19,3),round([Price]*([TaxRate]/(100.00)),(3),(1)),(0))) PERSISTED ,
		WarrantyLenght tinyint SPARSE  NULL,
		WarrantyEffectiveDate date SPARSE  NULL
		)  ON [PRIMARY]

	DROP TABLE Sales.OrderDetail

	EXECUTE sp_rename N'Sales.Tmp_OrderDetail', N'OrderDetail', 'OBJECT' 

	ALTER TABLE Sales.OrderDetail ADD CONSTRAINT
		PK_OrderDetail PRIMARY KEY CLUSTERED 
		(
		Id
		) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]


	ALTER TABLE Sales.OrderDetail ADD CONSTRAINT
		FK_OrderDetail_SalesOrder FOREIGN KEY
		(
		OrderId
		) REFERENCES Sales.SalesOrder
		(
		Id
		) ON UPDATE  NO ACTION 
		 ON DELETE  CASCADE 
	
	ALTER TABLE Sales.OrderDetail ADD CONSTRAINT
		FK_OrderDetail_ItemType FOREIGN KEY
		(
		ItemTypeId
		) REFERENCES Sales.ItemType
		(
		Id
		) ON UPDATE  NO ACTION 
		 ON DELETE  NO ACTION 
END	


