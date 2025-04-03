-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

delete from StoreCardPaymentDetails
delete from storecard
GO
IF EXISTS (SELECT * FROM sysobjects WHERE NAME  ='FK_StoreCard_StoreCardRate')
ALTER TABLE dbo.StoreCardPaymentDetails DROP CONSTRAINT FK_StoreCard_StoreCardRate

GO 
go 
ALTER TABLE dbo.StoreCardPaymentDetails ADD CONSTRAINT
	FK_StoreCard_StoreCardRate FOREIGN KEY
	(
	RateId
	) REFERENCES dbo.StoreCardRateDetails 
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
GO 
