-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
alter table Merchandising.RetailPrice
drop column CreatedDate

alter table Merchandising.RetailPrice
drop column CreatedById

alter table Merchandising.RetailPrice
add LastUpdated DateTime not null