-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON

ALTER TABLE Sales.OrderPayment ADD CONSTRAINT
	CK_OrderPayment_CardNo CHECK (CardNo IS NULL OR (CardNo < 10000 AND CardNo >= 0))

ALTER TABLE Sales.OrderPayment ADD CONSTRAINT
	CK_OrderPayment_Amount CHECK (Amount > 0)

CREATE TABLE Sales.OrderReturnPayment
	(
	Id int NOT NULL,
	OrderReturnId int NOT NULL,
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

ALTER TABLE Sales.OrderReturnPayment ADD CONSTRAINT
	CK_OrderReturnPayment_CardNo CHECK (CardNo IS NULL OR (CardNo < 10000 AND CardNo >= 0))

ALTER TABLE Sales.OrderReturnPayment ADD CONSTRAINT
	CK_OrderReturnPayment_Amount CHECK (Amount > 0)

ALTER TABLE Sales.OrderReturnPayment ADD CONSTRAINT
	PK_OrderReturnPayment PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

ALTER TABLE Sales.OrderReturnPayment ADD CONSTRAINT
	FK_OrderReturnPayment_OrderReturn FOREIGN KEY
	(
	OrderReturnId
	) REFERENCES Sales.OrderReturn
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  CASCADE 
	
ALTER TABLE Sales.OrderReturnPayment ADD CONSTRAINT
	FK_OrderReturnPayment_PaymentMethod FOREIGN KEY
	(
	PaymentMethodId
	) REFERENCES Sales.PaymentMethod
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 