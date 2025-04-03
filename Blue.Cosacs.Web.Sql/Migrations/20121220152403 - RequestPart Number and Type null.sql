-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
alter table "Service".RequestPart
  alter column PartNumber varchar(50) null
go

alter table "Service".RequestPart
  alter column  PartType varchar(30) null
go
