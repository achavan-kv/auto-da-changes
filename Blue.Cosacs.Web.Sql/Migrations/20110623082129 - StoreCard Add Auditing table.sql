-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.columns WHERE column_name = 'LastUpdatedBy' AND table_name ='StoreCardPaymentDetails')
ALTER TABLE StoreCardPaymentDetails ADD LastUpdatedBy INT 
GO 
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.tables WHERE table_name ='StoreCardAudit')
CREATE TABLE StoreCardAudit ( Empeeno INT NOT NULL , DateChanged DATETIME NOT NULL , Field VARCHAR(32) NOT NULL , OldValue VARCHAR(500) NOT NULL , NewValue VARCHAR(500) NOT NULL , AcctNo CHAR(12) NOT NULL )
GO 
ALTER TABLE StoreCardAudit ADD CONSTRAINT pk_acctno PRIMARY KEY  (acctno,datechanged,field )

GO 
