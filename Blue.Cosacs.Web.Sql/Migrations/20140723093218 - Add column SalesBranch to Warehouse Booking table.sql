-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


IF NOT EXISTS(select * from syscolumns where name = 'SalesBranch' and object_name(id) = 'booking')
BEGIN
	alter table warehouse.booking add SalesBranch int null
END