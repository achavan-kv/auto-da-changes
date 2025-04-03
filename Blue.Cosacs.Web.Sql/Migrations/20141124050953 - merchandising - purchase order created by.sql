-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

alter table Merchandising.PurchaseOrder
add CreatedById int null
GO

alter table Merchandising.PurchaseOrder
add CreatedBy varchar(max) null
GO

update Merchandising.PurchaseOrder
set CreatedById = 99999,
	CreatedBy = 'System Administrator'

alter table Merchandising.PurchaseOrder
alter column CreatedById int

