-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
alter table Merchandising.CintOrder
add Discount decimal not null default(0)
