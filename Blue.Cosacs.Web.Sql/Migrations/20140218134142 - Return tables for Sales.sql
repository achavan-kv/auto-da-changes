-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

ALTER TABLE Sales.SalesOrder 
	ADD OriginalOrderId int NULL
GO

ALTER TABLE Sales.SalesOrder ADD CONSTRAINT FK_SalesOrder_SalesOrder FOREIGN KEY
(
	OriginalOrderId
) REFERENCES Sales.SalesOrder
(
	Id
) 
	ON UPDATE  NO ACTION 
	ON DELETE  NO ACTION 
GO

CREATE TABLE Sales.SalesOrderReturnedWarranty
(
	Id								smallint NOT NULL,
	SaleOrderId						int NOT NULL,
	SalesOrderDetailWarrantiesId	smallint NOT NULL,
	Amount dbo.BlueAmount			NOT NULL
)  
GO

ALTER TABLE Sales.SalesOrderReturnedWarranty ADD CONSTRAINT PK_SalesOrderReturnedWarranty PRIMARY KEY CLUSTERED 
(
	Id
) WITH
( 
	STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON
) 
GO

ALTER TABLE Sales.SalesOrderReturnedWarranty ADD CONSTRAINT FK_SalesOrderReturnedWarranty_SalesOrder FOREIGN KEY
(
	SaleOrderId
) REFERENCES Sales.SalesOrder
(
	Id
) 
	ON UPDATE  NO ACTION 
	ON DELETE  NO ACTION 
GO

ALTER TABLE Sales.SalesOrderReturnedWarranty ADD CONSTRAINT FK_SalesOrderReturnedWarranty_SalesOrderDetailWarranties FOREIGN KEY
(
	SalesOrderDetailWarrantiesId
) REFERENCES Sales.SalesOrderDetailWarranties
(
	Id
) 
	ON UPDATE  NO ACTION 
	ON DELETE  NO ACTION 
GO

CREATE TABLE Sales.SalesOrderReturnedItem
(
	Id					smallint NOT NULL IDENTITY (1, 1),
	SaleOrderId			int NOT NULL,
	SalesOrderItemId	int NOT NULL,
	Quantity			smallint NOT NULL
)
GO

ALTER TABLE Sales.SalesOrderReturnedItem ADD CONSTRAINT PK_SalesOrderReturnedItem PRIMARY KEY CLUSTERED 
(
	Id
) WITH
( 
	STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON
)
GO

ALTER TABLE Sales.SalesOrderReturnedItem ADD CONSTRAINT FK_SalesOrderReturnedItem_SalesOrder FOREIGN KEY
(
	SaleOrderId
) REFERENCES Sales.SalesOrder
(
	Id
) 
	ON UPDATE  NO ACTION 
	ON DELETE  NO ACTION 
GO

ALTER TABLE Sales.SalesOrderReturnedItem ADD CONSTRAINT FK_SalesOrderReturnedItem_SalesOrderItem FOREIGN KEY
(
	SalesOrderItemId
) REFERENCES Sales.SalesOrderItem
(
	Id
) 
	ON UPDATE  NO ACTION 
	ON DELETE  NO ACTION 
