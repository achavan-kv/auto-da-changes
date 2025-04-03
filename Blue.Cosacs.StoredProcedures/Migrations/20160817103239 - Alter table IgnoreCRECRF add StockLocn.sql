-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'IgnoreCRECRF' AND  Column_Name = 'StockLocn')
BEGIN
	ALTER TABLE IgnoreCRECRF Add StockLocn smallint not null default 0
END