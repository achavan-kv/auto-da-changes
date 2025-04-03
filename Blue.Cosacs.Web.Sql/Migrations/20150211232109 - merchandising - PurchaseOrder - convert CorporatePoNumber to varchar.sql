update [Merchandising].[PurchaseOrder]
set CorporatePoNumber = null

alter table [Merchandising].[PurchaseOrder]
alter column CorporatePoNumber varchar(20) null