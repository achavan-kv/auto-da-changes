-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
INSERT INTO Cron.Endpoint
	(Id, Name, Url, Module)
Values
	(251, 'Refactor data from Stock Receive Report', '/Cosacs/Merchandising/GoodsReceipt/Recreate', 'Merchandising')