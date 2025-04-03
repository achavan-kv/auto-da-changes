-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

update 
    NonStocks.NonStockPrice
set 
    EffectiveDate = case 
                        when sp.DateActivated is null
                            then cast(getdate() as date)
                        else
                            cast(sp.DateActivated as date)
                    end 
from 
    NonStocks.NonStock n
inner join 
    NonStocks.NonStockPrice np on n.Id = np.NonStockId
inner join 
    StockInfo si on n.IUPC = si.IUPC and si.SKU = n.SKU
inner join 
    StockPrice sp on sp.ID = si.Id
where 
    si.category = 93
    and np.BranchNumber = sp.branchno
    and np.BranchNumber is not null


--Update entries where BranchNumber null
update 
    NonStocks.NonStockPrice
set 
    EffectiveDate = case 
                        when sp.DateActivated is null
                            then cast(getdate() as date)
                        else
                            cast(sp.DateActivated as date)
                    end 
from 
    NonStocks.NonStock n
inner join 
    NonStocks.NonStockPrice np on n.Id = np.NonStockId
inner join 
    StockInfo si on n.IUPC = si.IUPC and si.SKU = n.SKU
inner join 
    StockPrice sp on sp.ID = si.Id
where 
    si.category = 93
    and np.BranchNumber is null
    and np.RetailPrice = 0
    and sp.CreditPrice = 0
    and sp.ID = (select top 1 Id
                    from StockPrice sp1
                    where sp1.ID = sp.ID
                    and sp1.CreditPrice = sp.CreditPrice)
