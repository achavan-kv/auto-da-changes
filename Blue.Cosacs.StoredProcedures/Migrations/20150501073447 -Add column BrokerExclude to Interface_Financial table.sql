-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'interface_financial' AND Column_Name = 'BrokerExclude')
BEGIN
	ALTER TABLE interface_financial ADD BrokerExclude bit NULL
END
GO
