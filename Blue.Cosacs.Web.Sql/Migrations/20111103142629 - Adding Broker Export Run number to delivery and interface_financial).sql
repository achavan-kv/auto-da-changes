-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE column_name = 'BrokerExRunNo' AND table_name ='delivery')
ALTER TABLE delivery ADD BrokerExRunNo SMALLINT NOT NULL DEFAULT 0 
GO 

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE column_name = 'BrokerExRunNo' AND table_name ='interface_financial')
ALTER TABLE interface_financial ADD BrokerExRunNo SMALLINT NOT NULL DEFAULT 0   

GO  
DECLARE @runno SMALLINT
SELECT @runno =ISNULL(MAX(runno),0)
			  FROM interfacecontrol 
			  WHERE interface = 'BROKERX' AND result = 'P'
UPDATE delivery SET BrokerExRunNo = @runno  WHERE brokerexrunno= 0 
UPDATE interface_financial SET BrokerExRunNo = @runno 
WHERE brokerexrunno= 0 