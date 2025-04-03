-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

--Details
declare @taxrate float

select @taxrate = value from CountryMaintenance where CodeName = 'TaxRate'

--Details
insert into 
    NonStocks.NonStock
select 
    'discount', 
     si.SKU, 
     si.IUPC, 
     si.itemdescr1, 
     si.itemdescr2, 
     1, 
     @taxrate,
     null,
     null,
     null,
     null
from 
    StockInfo si
inner join 
    code c on CAST(si.category as VARCHAR(3)) = c.code 
where 
    c.category = 'PCDIS'
    and si.itemtype = 'N'
    and si.RepossessedItem = 0


select distinct n.id as NonStockId,
       null as Fascia, 
       null as BranchNumber, 
       sp.CostPrice as CostPrice,
       isnull(sp.CreditPrice,0) as RetailPrice,
       isnull(sp.CreditPrice,0) as TaxInclusivePrice,
        case 
            when sp.DateActivated is not null
                then cast(sp.DateActivated as date)
            else 
                cast(getdate() as date)
        end as EffectiveDate,
        null as EndDate,
        isnull(sp.CreditPrice,0) as DiscountValue
into #tempPrices
from 
    NonStocks.NonStock n
inner join 
    StockInfo si on n.SKU = si.SKU
    and n.IUPC = si.IUPC
inner join 
    code c on CAST(si.category as VARCHAR(3)) = c.code 
inner join 
    StockPrice sp on si.ID = sp.ID 
inner join 
    Branch b on sp.branchno = b.branchno
where
    n.[Type] = 'discount'
    and c.category= 'PCDIS'
    and si.itemtype = 'N'
    and si.RepossessedItem = 0

insert into 
    NonStocks.NonStockPrice
select 
    t.NonStockId, 
    t.Fascia, 
    t.BranchNumber, 
    t.CostPrice, 
    t.RetailPrice, 
    t.TaxInclusivePrice, 
    CAST(t.EffectiveDate AS VARCHAR(10)), 
    CAST(t.EndDate as varchar(10)),
    t.DiscountValue
from 
    #tempPrices t
where 
    EffectiveDate = (select min(t1.EffectiveDate) 
                     from #tempPrices t1
                     where t1.NonStockId = t.NonStockId)