-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

declare @taxrate float, @stockTaxType char(1)

select @taxrate = value from CountryMaintenance where CodeName = 'TaxRate'
select @stockTaxType = value from CountryMaintenance where CodeName = 'taxtype'

insert into 
    NonStocks.NonStock
select 
    'inst', 
     si.SKU, 
     si.IUPC, 
     si.itemdescr1, 
     si.itemdescr2, 
     1, 
     @taxrate
from 
    StockInfo si
where 
    si.category = 93      --Installation Items

insert into 
    NonStocks.NonStockPrice
select n.id,
       b.StoreType, 
       sp.branchno, 
       sp.CostPrice,
       case 
            when @stockTaxType = 'I' 
                then isnull(sp.CreditPrice,0) - (isnull(sp.CreditPrice,0) * @taxrate) / (100 + @taxrate)
                else 
                    isnull(sp.CreditPrice,0)
            end,
        isnull(sp.CreditPrice,0),
        cast(getdate() as date)

from 
    NonStocks.NonStock n
inner join 
    StockInfo si on n.SKU = si.SKU
    and n.IUPC = si.IUPC
inner join 
    StockPrice sp on si.ID = sp.ID 
inner join 
    Branch b on sp.branchno = b.branchno
where
    sp.CreditPrice != 0

  
insert into 
    NonStocks.NonStockPrice
select distinct n.id,
       null, 
       null, 
       sp.CostPrice,
       isnull(sp.CreditPrice,0),
       isnull(sp.CreditPrice,0),
       cast(getdate() as date)
from 
    NonStocks.NonStock n
inner join 
    StockInfo si on n.SKU = si.SKU
    and n.IUPC = si.IUPC
inner join 
    StockPrice sp on si.ID = sp.ID 
inner join 
    Branch b on sp.branchno = b.branchno
where
    sp.CreditPrice = 0