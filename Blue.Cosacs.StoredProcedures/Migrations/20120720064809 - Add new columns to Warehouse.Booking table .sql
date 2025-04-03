-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


IF NOT EXISTS(select * from syscolumns where name = 'BookedBy' and object_name(id) = 'warehouse.booking')
BEGIN
	alter table warehouse.booking add BookedBy int 
END

IF NOT EXISTS(select * from syscolumns where name = 'Fascia' and object_name(id) = 'warehouse.booking')
BEGIN
	alter table warehouse.booking add Fascia char(1) 
END