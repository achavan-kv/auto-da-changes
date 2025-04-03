-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
alter table merchandising.supplier
alter column AddressLine2 varchar(100) null