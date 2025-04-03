-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
--#2728 allow for non-stocks...
CREATE TABLE NonStockDeletionDates
(branchno SMALLINT NOT NULL,
itemno VARCHAR(8) NOT NULL,
DeletionDate DATETIME)

GO  
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.columns WHERE table_name ='StockPrice'
AND column_name = 'DateActivated')
ALTER TABLE StockPrice ADD DateActivated DATETIME 
GO 