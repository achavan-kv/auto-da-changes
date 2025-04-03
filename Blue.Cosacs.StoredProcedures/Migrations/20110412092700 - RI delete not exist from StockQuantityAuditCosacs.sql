-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

delete A
from StockQuantityAuditCosacs A
where not exists( select itemno from stockitem s where a.itemno=s.itemno)

