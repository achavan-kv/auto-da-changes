-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


update 
	Warranty.WarrantySale
set 
	ItemPrice = l.price
from 
	Warranty.WarrantySale ws
inner join 
	lineitem l on ws.CustomerAccount = l.acctno
and ws.AgreementNumber = l.agrmtno
and ws.ItemId = l.ItemID
and ws.StockLocation = l.stocklocn
inner join 
	stockinfo si on si.id = l.ItemID
where ws.ItemPrice != l.price
and si.itemtype = 'S'
