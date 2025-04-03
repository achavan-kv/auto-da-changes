-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

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
where 
    si.category = 93
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