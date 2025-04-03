-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS(select * from syscolumns
			where name = 'WarrantyContractNo'
			and object_name(id) = 'Request')
			
BEGIN
	alter TABLE service.Request add WarrantyContractNo VARCHAR(10)
END


