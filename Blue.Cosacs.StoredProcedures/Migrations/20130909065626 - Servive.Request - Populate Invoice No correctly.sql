-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


update Service.Request
	set InvoiceNumber= case when InvoiceNumber like 'Invoice:%' then replace(InvoiceNumber,'Invoice: ','')
				when InvoiceNumber like 'Account:%' then ''
				end,
	Account = case when InvoiceNumber like 'Invoice:%' then '' else Account end
 
Update Service.Request
set Account=null where Account=''
 
Update Service.Request
set InvoiceNumber=null where InvoiceNumber=''
 
