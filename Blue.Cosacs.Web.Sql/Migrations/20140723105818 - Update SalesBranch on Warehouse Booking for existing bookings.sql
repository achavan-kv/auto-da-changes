-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


update 
	warehouse.booking
set 
	SalesBranch = l.SalesBrnNo
from 
	warehouse.booking wb
inner join 
	lineitem l on wb.acctno = l.acctno
	and wb.itemid = l.itemid
	and wb.stockbranch = l.stocklocn


	