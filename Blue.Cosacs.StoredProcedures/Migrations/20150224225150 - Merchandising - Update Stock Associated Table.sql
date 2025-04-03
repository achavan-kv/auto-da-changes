-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


ALTER TABLE StockInfoAssociated
ALTER COLUMN Class varchar(5) NOT NULL

ALTER TABLE StockInfoAssociated
ALTER COLUMN [Source] varchar(20) NOT NULL


