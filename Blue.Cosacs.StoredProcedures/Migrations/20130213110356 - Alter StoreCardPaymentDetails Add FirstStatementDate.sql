-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
-- Related to issue: #12199 - CR11571

IF NOT EXISTS(select * from syscolumns where name = 'FirstStatementDate' and object_name(id) = 'StoreCardPaymentDetails')
BEGIN
	ALTER TABLE StoreCardPaymentDetails ADD FirstStatementDate SMALLDATETIME
END	
GO