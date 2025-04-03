-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here -- Adding due date + next due date... 
-- Options either store just due day. Then calculate due date based on Last successful run date of end of day option
-- But how do you work it out???  
-- do we need a Payment due date and due day?? 
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.columns WHERE column_name = 'DatePaymentDue' AND table_name ='StoreCardPaymentDetails')
	ALTER TABLE StoreCardPaymentDetails ADD DatePaymentDue Datetime 
