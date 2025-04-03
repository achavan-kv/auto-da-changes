-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
update Merchandising.Product
set ProductType = 'RegularStock'

alter table merchandising.product
alter column ProductType varchar(100) not null