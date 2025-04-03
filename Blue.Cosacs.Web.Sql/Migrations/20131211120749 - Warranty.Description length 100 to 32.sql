-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
update Warranty.Warranty
set [Description] = SUBSTRING([Description], 1, 32)
where len([Description]) > 32

go

alter table Warranty.Warranty
  alter column [Description] varchar(32) not null

go