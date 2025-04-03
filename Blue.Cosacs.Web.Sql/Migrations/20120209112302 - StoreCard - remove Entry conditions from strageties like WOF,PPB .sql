-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

Delete CMStrategyCondition where (Strategy like '%WOF' or Strategy like '%PPB')
	and savedtype='N' and Condition in('StoreCard','CreditAcct')
	
