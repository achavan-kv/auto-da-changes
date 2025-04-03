-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF EXISTS(select * from syscolumns
			where name = 'PurchaseInterestRate'
			and object_name(id) = 'StoreCardRateAudit')
BEGIN

	alter table StoreCardRateAudit alter column PurchaseInterestRate decimal(5,3) not null
	
END