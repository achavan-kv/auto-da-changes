-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'WarrantyRenewable'
               AND OBJECT_NAME(id) = 'RItemp_RawProductLoad')
BEGIN
  ALTER TABLE RItemp_RawProductLoad ADD WarrantyRenewable CHAR(1)
END

go

drop TABLE RItemp_RawProductLoadRepo
go

select * into RItemp_RawProductLoadRepo from RItemp_RawProductLoad
