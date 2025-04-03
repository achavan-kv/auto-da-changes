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
    'rassist', 
     si.SKU, 
     si.IUPC, 
     si.itemdescr1, 
     si.itemdescr2, 
     1, 
     @taxrate,
     c.reference
from 
    StockInfo si
inner join 
    code c on c.code = si.IUPC
where 
    si.category = 11
    and c.category = 'RDYAST'

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
        case 
            when sp.DateActivated != null
                then cast(sp.DateActivated as date)
            else 
                cast(getdate() as date)
        end
from 
    NonStocks.NonStock n
inner join 
    StockInfo si on n.SKU = si.SKU
    and n.IUPC = si.IUPC
inner join 
    Code c on c.code = si.IUPC
inner join 
    StockPrice sp on si.ID = sp.ID 
inner join 
    Branch b on sp.branchno = b.branchno
where
    sp.CreditPrice != 0
    and c.category = 'RDYAST'
    and n.[Type] = 'rassist'
    and not exists(select * from NonStocks.NonStockPrice p
                        where p.NonStockId = n.Id)

  