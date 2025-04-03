-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.columns WHERE table_name ='StoreCardPaymentDetails'
AND column_name = 'NoStatements')
ALTER TABLE StoreCardPaymentDetails ADD NoStatements BIT NOT NULL DEFAULT 0 
 
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.columns WHERE table_name ='StoreCardPaymentDetails'
AND column_name = 'ContactMethod')
ALTER TABLE StoreCardPaymentDetails ADD ContactMethod VARCHAR(6)  

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.columns WHERE table_name ='StoreCardPaymentDetails'
AND column_name = 'DateNotePrinted')
ALTER TABLE StoreCardPaymentDetails ADD DateNotePrinted SMALLDATETIME  


