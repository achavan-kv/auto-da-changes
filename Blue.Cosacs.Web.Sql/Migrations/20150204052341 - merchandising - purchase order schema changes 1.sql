SET ANSI_WARNINGS OFF

alter table Merchandising.PurchaseOrder
alter column PaymentTerms	varchar(60) null

alter table Merchandising.PurchaseOrder
alter column [Status]		varchar(3)	null

alter table Merchandising.PurchaseOrder
alter column [Currency]		varchar(3)	null

SET ANSI_WARNINGS ON