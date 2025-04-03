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

CREATE TABLE Sales.PaymentType
	(
	Id tinyint NOT NULL,
	Description varchar(64) NOT NULL,
	CreatedOn smalldatetime NOT NULL,
	CreatedBy int NOT NULL
	)  ON [PRIMARY]

ALTER TABLE Sales.PaymentType ADD CONSTRAINT
	PK_PaymentType PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

EXECUTE sp_rename N'Sales.OrderDetail', N'OrderItem', 'OBJECT' 

EXECUTE sp_rename N'Sales.OrderItem.Number', N'ItermNo', 'COLUMN' 

EXECUTE sp_rename N'Sales.OrderItem.WarrantyLenght', N'WarrantyLengthMonth', 'COLUMN' 

EXECUTE sp_rename N'Sales.ReturnItem', N'SalesReturnItem', 'OBJECT' 

EXECUTE sp_rename N'Sales.SalesReturnItem.Number', N'ItemNo', 'COLUMN' 

ALTER TABLE Sales.OrderCustomer
	DROP CONSTRAINT FK_OrderCustomer_SalesOrder

ALTER TABLE Sales.OrderPayment
	DROP CONSTRAINT FK_OrderPayment_SalesOrder

EXECUTE sp_rename N'Sales.SalesOrder', N'Order', 'OBJECT' 

EXECUTE sp_rename N'Sales.[Order].BranchNumber', N'BranchNo', 'COLUMN' 

CREATE TABLE Sales.Tmp_OrderPayment
	(
	Id int NOT NULL IDENTITY (1, 1),
	OrderId int NOT NULL,
	PaymentTypeId tinyint NULL,
	Amount dbo.BlueAmount NOT NULL,
	Change dbo.BlueAmount NOT NULL,
	Bank varchar(32) NULL,
	CardType varchar(16) NULL,
	CardNumber bigint NULL,
	ChequeNumber smallint NULL,
	BankAccountNumber varchar(32) NULL
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
	FK_OrderPayment_PaymentType FOREIGN KEY
	(
	PaymentTypeId
	) REFERENCES Sales.PaymentType
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 

EXECUTE sp_rename N'Sales.OrderCustomer.PostalCode', N'PostCode', 'COLUMN' 

ALTER TABLE Sales.OrderCustomer
	DROP CONSTRAINT PK_OrderCustomer

ALTER TABLE Sales.OrderCustomer ADD CONSTRAINT
	PK_OrderCustomer PRIMARY KEY CLUSTERED 
	(
	OrderId
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]


ALTER TABLE Sales.[Order] ADD CONSTRAINT
	FK_Order_OrderCustomer FOREIGN KEY
	(
	Id
	) REFERENCES Sales.OrderCustomer
	(
	OrderId
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	

ALTER TABLE Sales.OrderCustomer
	DROP COLUMN Id

EXECUTE sp_rename N'Sales.SalesReturn', N'OrderReturn', 'OBJECT' 
EXECUTE sp_rename N'Sales.SalesReturnItem', N'OrderReturnItem', 'OBJECT' 
