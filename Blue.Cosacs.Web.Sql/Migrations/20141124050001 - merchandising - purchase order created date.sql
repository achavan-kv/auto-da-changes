-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

alter table Merchandising.PurchaseOrder
add CreatedDate datetime null
GO

update Merchandising.PurchaseOrder
set CreatedDate = GETDATE()

alter table Merchandising.PurchaseOrder
alter column CreatedDate datetime
