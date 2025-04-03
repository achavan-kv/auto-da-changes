-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

declare @taxrate float, @stockTaxType char(1)

select @taxrate = value from CountryMaintenance where CodeName = 'TaxRate'
select @stockTaxType = value from CountryMaintenance where CodeName = 'taxtype'

--Details
insert into 
    NonStocks.NonStock
select 
    'generic', 
     si.SKU, 
     si.IUPC, 
     si.itemdescr1, 
     si.itemdescr2, 
     1, 
     @taxrate,
     NULL
from 
    StockInfo si
inner join 
    code c on CAST(si.category as VARCHAR(3)) = c.code 
    and c.category in('PCE','PCF','PCO','PCW','PCDIS')
where 
    si.category != 93   --not Installation 
    and si.IUPC not in (select code from code c1 where c1.Category = 'RDYAST') -- Not Ready Assist
    and si.category not in (select code from code c2 where c2.category = 'PCDIS') --Not Discount
    and si.category not in (select code from code c3 where c3.category='WAR')
    and si.IUPC not like '19%'		-- like warranty but not in warranty category
    and si.itemtype = 'N'
    and si.RepossessedItem = 0

--Prices
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
            when sp.DateActivated is not null
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
    code c on CAST(si.category as VARCHAR(3)) = c.code 
    and c.category in('PCE','PCF','PCO','PCW','PCDIS')
inner join 
    StockPrice sp on si.ID = sp.ID 
inner join 
    Branch b on sp.branchno = b.branchno
where
    sp.CreditPrice != 0
    and n.[Type] = 'generic'
    and si.category != 93   --not Installation 
    and si.IUPC not in (select code from code c1 where c1.Category = 'RDYAST') -- Not Ready Assist
    and si.category not in (select code from code c2 where c2.category = 'PCDIS') --Not Discount
    and si.category not in (select code from code c3 where c3.category='WAR')
    and si.IUPC not like '19%'		-- like warranty but not in warranty category
    and si.itemtype = 'N'
    and si.RepossessedItem = 0
 
insert into 
    NonStocks.NonStockPrice
select distinct n.id,
       null, 
       null, 
       sp.CostPrice,
       case 
            when @stockTaxType = 'I' 
                then isnull(sp.CreditPrice,0) - (isnull(sp.CreditPrice,0) * @taxrate) / (100 + @taxrate)
                else 
                    isnull(sp.CreditPrice,0)
            end,
        isnull(sp.CreditPrice,0),
        case 
            when sp.DateActivated is not null
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
    code c on CAST(si.category as VARCHAR(3)) = c.code 
    and c.category in('PCE','PCF','PCO','PCW','PCDIS')
inner join 
    StockPrice sp on si.ID = sp.ID 
inner join 
    Branch b on sp.branchno = b.branchno
where
    sp.CreditPrice = 0
    and n.[Type] = 'generic'
    and si.category != 93   --not Installation 
    and si.IUPC not in (select code from code c1 where c1.Category = 'RDYAST') -- Not Ready Assist
    and si.category not in (select code from code c2 where c2.category = 'PCDIS') --Not Discount
    and si.category not in (select code from code c3 where c3.category='WAR')
    and si.IUPC not like '19%'		-- like warranty but not in warranty category
    and si.itemtype = 'N'
    and si.RepossessedItem = 0

--Associations
select 
    sia.ProductGroup, 
    sia.Category, 
    sia.Class, 
    sia.SubClass, 
    sia.AssocItemId, 
    'Mig_' + cast(si.SKU as varchar(8)) as LinkName, 
    si.SKU, 
    si.IUPC
into 
    #AssociationDetails
from 
    StockInfoAssociated sia
inner join 
    StockInfo si on si.id = sia.AssocItemId
inner join 
    code c on CAST(si.category as VARCHAR(3)) = c.code 
    and c.category in('PCE','PCF','PCO','PCW','PCDIS')
where 
    si.category != 93   --not Installation 
    and si.IUPC not in (select code from code c1 where c1.Category = 'RDYAST') -- Not Ready Assist
    and si.category not in (select code from code c2 where c2.category = 'PCDIS') --Not Discount
    and si.category not in (select code from code c3 where c3.category='WAR')
    and si.IUPC not like '19%'		-- like warranty but not in warranty category
    and si.itemtype = 'N'
    and si.RepossessedItem = 0
order by 
    sia.AssocItemId asc

insert into  
    NonStocks.Link
select distinct 
    LinkName, 
    getdate()
from #AssociationDetails

select 
    a.ProductGroup, 
    a.Category, 
    a.Class, 
    a.SubClass, 
    a.AssocItemId, 
    a.LinkName, 
    a.SKU, 
    a.IUPC, 
    l.id as LinkId, 
    ns.id as NonStockId  
into 
    #AssociationDetailsLink
from 
    #AssociationDetails a
inner join 
    NonStocks.Link l on a.LinkName = l.Name
inner join 
    NonStocks.NonStock ns on a.iupc = ns.iupc and a.sku = ns.sku
where ns.Type = 'generic'

insert into 
    NonStocks.LinkProduct
select 
    al.LinkId, 
    case 
        when al.ProductGroup = 'Any'
        then null
        else al.ProductGroup 
    end, 
     al.Category, 
    case 
        when al.Class = 'Any'
        then null
        else al.Class
    end, 
    null, 
    null, 
    0
from #AssociationDetailsLink al

insert into 
    NonStocks.LinkNonStock
select distinct 
    al.LinkId,
    al.NonStockId,
    0
from #AssociationDetailsLink al

--Product Hierarchy

insert into NonStocks.NonStockHierarchy
select data.Id, data.[Level], data.PrimaryCode, data.CodeDescription from (
    select n.Id, 1 AS [Level], pDiv.PrimaryCode, pDiv.CodeDescription
    from NonStocks.NonStock n
    inner join StockInfo si on si.IUPC = n.IUPC and si.SKU = n.SKU
    inner join ProductHeirarchy pCat on si.category = pCat.PrimaryCode
    inner join ProductHeirarchy pDiv on pCat.ParentCode = pDiv.PrimaryCode
    where pCat.CatalogType = '02'
    and pDiv.CatalogType = '01'
    and n.[Type] = 'generic'

    union

    select n.id, 2 AS [Level], pCat.PrimaryCode, pCat.CodeDescription
    from NonStocks.NonStock n
    inner join StockInfo si on si.IUPC = n.IUPC and si.SKU = n.SKU
    inner join ProductHeirarchy pCat on si.category = pCat.PrimaryCode
    where pCat.CatalogType = '02'
    and n.[Type] = 'generic'

    union

    select n.Id, 3 AS [Level], pClass.PrimaryCode, pClass.CodeDescription
    from NonStocks.NonStock n
    inner join StockInfo si on si.IUPC = n.IUPC and si.SKU = n.SKU
    inner join ProductHeirarchy pClass on si.Class = pClass.PrimaryCode
    and si.category = pClass.ParentCode
    where pClass.CatalogType = '03'
    and n.[Type] = 'generic'
) data
order by data.Id, data.[Level]

