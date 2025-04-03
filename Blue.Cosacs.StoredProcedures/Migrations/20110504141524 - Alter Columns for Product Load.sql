-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

alter TABLE RItemp_RawProductload alter column ItemPOSDescr VARCHAR(25)

alter TABLE RItemp_RawProductLoadRepo alter column ItemPOSDescr VARCHAR(25)

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'ItemPOSDescr'
               AND OBJECT_NAME(id) = 'StockInfo')
BEGIN
  ALTER TABLE StockInfo ADD ItemPOSDescr VARCHAR(25)
END

go