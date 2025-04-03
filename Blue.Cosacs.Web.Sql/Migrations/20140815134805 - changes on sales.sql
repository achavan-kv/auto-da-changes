-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON

ALTER TABLE Sales.OrderItem
	DROP CONSTRAINT FK_OrderDetail_SalesOrder

ALTER TABLE Sales.OrderPayment
	DROP CONSTRAINT FK_OrderPayment_SalesOrder

CREATE TABLE Sales.Tmp_ItemType
	(
	Id tinyint NOT NULL,
	Name varchar(64) NOT NULL
	)  ON [PRIMARY]

ALTER TABLE Sales.OrderReturnItem
	DROP CONSTRAINT FK_ReturnItem_ItemType

ALTER TABLE Sales.OrderItem
	DROP CONSTRAINT FK_OrderDetail_ItemType

DROP TABLE Sales.ItemType

EXECUTE sp_rename N'Sales.Tmp_ItemType', N'ItemType', 'OBJECT' 

ALTER TABLE Sales.ItemType ADD CONSTRAINT
	PK_ItemType PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]


CREATE TABLE Sales.Tmp_OrderItem
	(
	Id int NOT NULL IDENTITY (1, 1),
	ParentItemId int NULL,
	OrderId int NOT NULL,
	ItemTypeId tinyint NOT NULL,
	ItermNo varchar(32) NOT NULL,
	Description varchar(128) NOT NULL,
	Quantity smallint NOT NULL,
	Price dbo.BlueAmount NOT NULL,
	TaxRate dbo.BluePercentage NOT NULL,
	TaxAmount  AS (CONVERT([decimal](19,3),round([Price]*([TaxRate]/(100.00)),(3),(1)),(0))) PERSISTED ,
	WarrantyLengthMonths tinyint SPARSE  NULL,
	WarrantyEffectiveDate date SPARSE  NULL,
	WarrantyContractNo varchar(20) NULL
	)  ON [PRIMARY]

DROP TABLE Sales.OrderItem

EXECUTE sp_rename N'Sales.Tmp_OrderItem', N'OrderItem', 'OBJECT' 

ALTER TABLE Sales.OrderItem ADD CONSTRAINT
	PK_OrderDetail PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

ALTER TABLE Sales.OrderItem ADD CONSTRAINT
	FK_OrderDetail_SalesOrder FOREIGN KEY
	(
	OrderId
	) REFERENCES Sales.[Order]
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  CASCADE 
	

ALTER TABLE Sales.OrderItem ADD CONSTRAINT
	FK_OrderDetail_ItemType FOREIGN KEY
	(
	ItemTypeId
	) REFERENCES Sales.ItemType
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	

ALTER TABLE Sales.OrderItem ADD CONSTRAINT
	FK_OrderItem_OrderItem FOREIGN KEY
	(
	ParentItemId
	) REFERENCES Sales.OrderItem
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 

EXECUTE sp_rename N'Sales.OrderReturn.BranchNumber', N'Tmp_BranchNo', 'COLUMN' 

EXECUTE sp_rename N'Sales.OrderReturn.Tmp_BranchNo', N'BranchNo', 'COLUMN' 

CREATE TABLE dbo.OrderReturnCustomer
	(
	OrderReturnId int NOT NULL,
	Title varchar(8) SPARSE  NULL,
	FirstName varchar(32) NOT NULL,
	Middle varchar(64) SPARSE  NULL,
	LastName varchar(32) NOT NULL,
	Email varchar(64) NULL,
	HomePhone varchar(32) NULL,
	MobilePhone varchar(32) NULL,
	AddressLine1 varchar(64) NULL,
	AddressLine2 varchar(64) SPARSE  NULL,
	PostCode varchar(16) NULL
	)  ON [PRIMARY]

ALTER TABLE dbo.OrderReturnCustomer ADD CONSTRAINT
	PK_OrderReturnCustomer PRIMARY KEY CLUSTERED 
	(
	OrderReturnId
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

ALTER TABLE dbo.OrderReturnCustomer ADD CONSTRAINT
	FK_OrderReturnCustomer_OrderReturn FOREIGN KEY
	(
	OrderReturnId
	) REFERENCES Sales.OrderReturn
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  CASCADE 


ALTER TABLE Sales.OrderPayment
	DROP CONSTRAINT FK_OrderPayment_PaymentType

ALTER TABLE Sales.PaymentType
	DROP COLUMN CreatedOn, CreatedBy

CREATE TABLE dbo.PaymentMethod
	(
	Id tinyint NOT NULL,
	PaymentTypeId tinyint NOT NULL,
	Description varchar(64) NOT NULL,
	Active bit NOT NULL,
	CreatedOn smalldatetime NOT NULL,
	CreatedBy int NOT NULL
	)  ON [PRIMARY]

ALTER TABLE dbo.PaymentMethod ADD CONSTRAINT
	PK_PaymentMethod PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

ALTER TABLE dbo.PaymentMethod ADD CONSTRAINT
	FK_PaymentMethod_PaymentType FOREIGN KEY
	(
	PaymentTypeId
	) REFERENCES Sales.PaymentType
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 

CREATE TABLE Sales.Tmp_OrderPayment
	(
	Id int NOT NULL IDENTITY (1, 1),
	OrderId int NOT NULL,
	PaymentMethodId tinyint NOT NULL,
	Amount dbo.BlueAmount NOT NULL,
	Change dbo.BlueAmount NOT NULL,
	Bank varchar(32) NULL,
	CardType varchar(16) NULL,
	CardNo smallint NULL,
	ChequeNo smallint NULL,
	BankAccountNo varchar(32) NULL,
	CurrencyRate dbo.BluePercentage NULL,
	CurrencyAmount dbo.BlueAmount NULL
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
	) REFERENCES Sales.[Order]
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  CASCADE 
	
ALTER TABLE Sales.OrderPayment ADD CONSTRAINT
	FK_OrderPayment_PaymentMethod FOREIGN KEY
	(
	PaymentMethodId
	) REFERENCES dbo.PaymentMethod
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 

CREATE TABLE Sales.Tmp_ReturnReason
	(
	Id tinyint NOT NULL,
	Description varchar(32) NOT NULL,
	Active bit NOT NULL,
	CreatedBy int NOT NULL,
	CreatedOn smalldatetime NOT NULL
	)  ON [PRIMARY]

ALTER TABLE Sales.OrderReturnItem
	DROP CONSTRAINT FK_ReturnItem_ReturnReason

DROP TABLE Sales.ReturnReason

EXECUTE sp_rename N'Sales.Tmp_ReturnReason', N'ReturnReason', 'OBJECT' 

ALTER TABLE Sales.ReturnReason ADD CONSTRAINT
	PK_ReturnReason PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]


ALTER TABLE Sales.OrderReturnItem ADD CONSTRAINT
	FK_ReturnItem_ItemType FOREIGN KEY
	(
	ItemTypeId
	) REFERENCES Sales.ItemType
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	

ALTER TABLE Sales.OrderReturnItem ADD CONSTRAINT
	FK_ReturnItem_ReturnReason FOREIGN KEY
	(
	ReturnReasonId
	) REFERENCES Sales.ReturnReason
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
