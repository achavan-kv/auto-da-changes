-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

select n.id as NonStockId
into #ToDelete from NonStocks.NonStock n
inner join stockinfo si on n.sku = si.itemno
where si.itemtype = 'S'

delete from NonStocks.LinkNonStock 
from NonStocks.LinkNonStock n inner join #ToDelete nd on n.NonStockId = nd.NonStockId

delete from NonStocks.NonStockHierarchy 
from NonStocks.NonStockHierarchy n inner join #ToDelete nd on n.NonStockId = nd.NonStockId

delete from NonStocks.NonStockPromotion 
from NonStocks.NonStockPromotion n inner join #ToDelete nd on n.NonStockId = nd.NonStockId

delete from NonStocks.NonStockPrice 
from NonStocks.NonStockPrice n inner join #ToDelete nd on n.NonStockId = nd.NonStockId

delete from NonStocks.NonStock 
from NonStocks.NonStock n inner join #ToDelete nd on n.Id = nd.NonStockId