-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

CREATE NONCLUSTERED INDEX [ix_SalesCommission_RunDate]
ON [dbo].[SalesCommission] ([RunDate])
INCLUDE ([Employee],[AcctNo],[AgrmtNo],[StockLocn],[CommissionAmount],[ItemId])

