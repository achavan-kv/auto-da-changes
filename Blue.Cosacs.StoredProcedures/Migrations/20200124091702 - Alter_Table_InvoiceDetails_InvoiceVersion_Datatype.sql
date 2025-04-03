
-- Script Comment : Alter Table InvoiceDetails For Column InvoiceVersion datatype change from Varchar(15) to TinyInt	
-- Created By : Snehal D												
-- Created On : 23/01/2020	
--*************************************************************************************************************	


	ALTER TABLE [dbo].[invoiceDetails] ALTER COLUMN InvoiceVersion TinyInt NOT NULL
	

--*************************************************************************************************************	