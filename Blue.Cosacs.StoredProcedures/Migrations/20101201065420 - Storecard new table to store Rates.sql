-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
IF NOT EXISTS
 (SELECT * FROM INFORMATION_SCHEMA.tables WHERE table_name ='StoreCardPaymentDetails')
CREATE TABLE StoreCardPaymentDetails
(	acctno CHAR(12) NOT NULL,
	MonthlyAmount decimal,
	PaymentMethod CHAR(4),
	PaymentOption CHAR(4),
	RateId INT NOT NULL,
	InterestRate FLOAT NOT NULL,
	RateFixed BIT)
GO 	
IF NOT EXISTS (SELECT * FROM sysobjects WHERE NAME = 'pk_StoreCardPaymentDetails')	
ALTER TABLE StoreCardPaymentDetails ADD CONSTRAINT 	pk_StoreCardPaymentDetails
PRIMARY KEY  (acctno)
GO 

ALTER TABLE storecard DROP  FK_StoreCard_StoreCardRate
GO 
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.columns WHERE table_name ='storecard' AND 
column_name = 'FixedRate')
BEGIN
	ALTER TABLE storecard DROP COLUMN  FixedRate
	ALTER TABLE storecard DROP COLUMN  RateId
	
END
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.columns WHERE table_name ='storecard' AND 
column_name = 'MonthlyAmount')
BEGIN	
	ALTER TABLE storecard DROP COLUMN  MonthlyAmount
	ALTER TABLE storecard DROP COLUMN  PaymentMethod
	ALTER TABLE storecard DROP COLUMN  PaymentOption
	
END

GO 
ALTER TABLE StoreCardPaymentDetails ADD CONSTRAINT FK_StoreCard_StoreCardRate
FOREIGN KEY (RateId)
 REFERENCES StoreCardRate (Id)


	