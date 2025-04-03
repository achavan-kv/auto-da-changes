-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

delete from Merchandising.SupplierType

alter table Merchandising.supplier
drop constraint FK_Supplier_SupplierType

alter table Merchandising.supplier
drop column suppliertypeid

drop table Merchandising.SupplierType

alter table Merchandising.Supplier
add SupplierType varchar(100)