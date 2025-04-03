-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
ALTER TABLE Sales.OrderItem
	DROP CONSTRAINT FK_OrderDetail_ItemType
GO

ALTER TABLE Sales.OrderItem
	DROP CONSTRAINT FK_OrderDetail_SalesOrder
GO

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
	TaxAmount dbo.BlueAmount NULL,
	WarrantyLengthMonths tinyint SPARSE  NULL,
	WarrantyEffectiveDate date SPARSE  NULL,
	WarrantyContractNo varchar(20) NULL
	)  ON [PRIMARY]
GO
ALTER TABLE Sales.Tmp_OrderItem SET (LOCK_ESCALATION = TABLE)
GO
SET IDENTITY_INSERT Sales.Tmp_OrderItem ON
GO
IF EXISTS(SELECT * FROM Sales.OrderItem)
	 EXEC('INSERT INTO Sales.Tmp_OrderItem (Id, ParentItemId, OrderId, ItemTypeId, ItermNo, Description, Quantity, Price, TaxRate, TaxAmount, WarrantyLengthMonths, WarrantyEffectiveDate, WarrantyContractNo)
		SELECT Id, ParentItemId, OrderId, ItemTypeId, ItermNo, Description, Quantity, Price, TaxRate, TaxAmount, WarrantyLengthMonths, WarrantyEffectiveDate, WarrantyContractNo FROM Sales.OrderItem WITH (HOLDLOCK TABLOCKX)')
GO
SET IDENTITY_INSERT Sales.Tmp_OrderItem OFF
GO
ALTER TABLE Sales.OrderItem
	DROP CONSTRAINT FK_OrderItem_OrderItem
GO
DROP TABLE Sales.OrderItem
GO
EXECUTE sp_rename N'Sales.Tmp_OrderItem', N'OrderItem', 'OBJECT' 
GO
ALTER TABLE Sales.OrderItem ADD CONSTRAINT
	PK_OrderDetail PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE Sales.OrderItem ADD CONSTRAINT
	FK_OrderDetail_SalesOrder FOREIGN KEY
	(
	OrderId
	) REFERENCES Sales.[Order]
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  CASCADE 
	
GO
ALTER TABLE Sales.OrderItem ADD CONSTRAINT
	FK_OrderDetail_ItemType FOREIGN KEY
	(
	ItemTypeId
	) REFERENCES Sales.ItemType
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE Sales.OrderItem ADD CONSTRAINT
	FK_OrderItem_OrderItem FOREIGN KEY
	(
	ParentItemId
	) REFERENCES Sales.OrderItem
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO