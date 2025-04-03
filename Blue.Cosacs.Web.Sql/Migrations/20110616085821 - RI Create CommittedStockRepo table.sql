-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID('RICommittedStockRepo') AND type in (N'U'))
BEGIN

	Select * into RICommittedStockRepo from RICommittedStock
	truncate TABLE RICommittedStockRepo

End
