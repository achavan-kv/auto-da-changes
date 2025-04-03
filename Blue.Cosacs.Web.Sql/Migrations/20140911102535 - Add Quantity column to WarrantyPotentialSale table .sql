-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS(select * from syscolumns where name = 'Quantity' and object_name(id) = 'WarrantyPotentialSale')
BEGIN
	alter table Warranty.WarrantyPotentialSale add Quantity int not null default 0
END