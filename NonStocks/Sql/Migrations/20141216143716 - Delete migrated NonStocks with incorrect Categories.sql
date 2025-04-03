-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

select NonStockId 
into #NonStockToDelete
from NonStocks.NonStockHierarchy where len(levelKey) > 2
and [Level] = '2'

delete from NonStocks.LinkNonStock 
from NonStocks.LinkNonStock n inner join #NonStockToDelete nd on n.NonStockId = nd.NonStockId

delete from NonStocks.NonStockHierarchy 
from NonStocks.NonStockHierarchy n inner join #NonStockToDelete nd on n.NonStockId = nd.NonStockId

delete from NonStocks.NonStockPromotion 
from NonStocks.NonStockPromotion n inner join #NonStockToDelete nd on n.NonStockId = nd.NonStockId

delete from NonStocks.NonStockPrice 
from NonStocks.NonStockPrice n inner join #NonStockToDelete nd on n.NonStockId = nd.NonStockId

delete from NonStocks.NonStock 
from NonStocks.NonStock n inner join #NonStockToDelete nd on n.Id = nd.NonStockId