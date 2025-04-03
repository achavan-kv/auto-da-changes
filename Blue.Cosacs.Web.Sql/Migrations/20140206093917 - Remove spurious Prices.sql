-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


select * into StockPrice_Spurious
from stockprice p
where not exists(select * from branch b where b.branchno=p.branchno)

if @@ROWCOUNT>0
	delete stockprice  where not exists(select * from branch b where b.branchno=stockprice.branchno)
else
	drop table StockPrice_Spurious

