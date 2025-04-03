-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

-- Fix Purchase dates that are 1900-01-01 for Courts SR's
UPDATE SR_ServiceRequest
set purchasedate =d.datedel
from SR_ServiceRequest r INNER JOIN delivery d 
	on r.acctno=d.acctno and r.ProductCode=d.itemno --and r.StockLocn=d.stocklocn
where d.delorcoll='D' and servicetype='C' and r.PurchaseDate='1900-01-01'
