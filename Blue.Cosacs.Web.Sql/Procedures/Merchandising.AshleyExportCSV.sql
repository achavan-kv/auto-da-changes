 if exists ( select *  from  dbo.sysobjects   where  id = object_id('[Merchandising].[AshleyExportCSV]')    and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure  [Merchandising].[AshleyExportCSV]
GO  
create PROCEDURE  [Merchandising].[AshleyExportCSV]
as 
Begin
SELECT DISTINCT
  P.SKU,
  P.LongDescription,
  P.POSDescription,
  p.CountryofOrigin,
  t.StockAvailable,
  x.SupplierCost,
  t.LocationId,
  r.CashPrice AS RetailCashprice,
  x.SupplierCurrency
FROM merchandising.LocationStockLevelView t with(Nolock)
INNER JOIN [Merchandising].[ProductAttributes] o with(Nolock)
ON t.ProductId = o.ProductId
INNER JOIN [Merchandising].[Product] p with(Nolock)
ON t.ProductId = p.id
INNER JOIN [Merchandising].[RetailPrice] r with(Nolock)
ON t.ProductId = r.ProductId
INNER JOIN [Merchandising].[CostPrice] x with(Nolock)
ON t.ProductId = x.ProductId
WHERE  IsAshleyProduct=1 and  
(convert(date,p.LastStatusChangeDate,102) = convert(date,getdate()-1 ,102) 
or 
 convert(date, X.LastLandedCostUpdated, 102)=convert(date,getdate()-1 ,102) 
or
 convert(date,r.LastUpdated ,102) = convert(date,getdate()-1 ,102) 
or
 convert(date, X.AverageWeightedCostUpdated, 102)=convert(date,getdate()-1 ,102)  )
 and t.LocationId='11'
end  