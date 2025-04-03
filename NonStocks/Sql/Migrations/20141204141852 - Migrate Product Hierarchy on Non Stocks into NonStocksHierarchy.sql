-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


truncate table NonStocks.NonStockHierarchy

insert into NonStocks.NonStockHierarchy
select data.Id, data.[Level], data.PrimaryCode, data.CodeDescription from (
    select n.Id, 1 AS [Level], pDiv.PrimaryCode, pDiv.CodeDescription
    from NonStocks.NonStock n
    inner join StockInfo si on si.IUPC = n.IUPC and si.SKU = n.SKU
    inner join ProductHeirarchy pCat on si.category = pCat.PrimaryCode
    inner join ProductHeirarchy pDiv on pCat.ParentCode = pDiv.PrimaryCode
    where pCat.CatalogType = '02'
    and pDiv.CatalogType = '01'

    union

    select n.id, 2 AS [Level], pCat.PrimaryCode, pCat.CodeDescription
    from NonStocks.NonStock n
    inner join StockInfo si on si.IUPC = n.IUPC and si.SKU = n.SKU
    inner join ProductHeirarchy pCat on si.category = pCat.PrimaryCode
    where pCat.CatalogType = '02'

    union

    select n.Id, 3 AS [Level], pClass.PrimaryCode, pClass.CodeDescription
    from NonStocks.NonStock n
    inner join StockInfo si on si.IUPC = n.IUPC and si.SKU = n.SKU
    inner join ProductHeirarchy pClass on si.Class = pClass.PrimaryCode
    and si.category = pClass.ParentCode
    where pClass.CatalogType = '03'
) data
order by data.Id, data.PrimaryCode