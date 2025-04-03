-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

--Re-create Win Non Stock Hierarchy
select top 0 * into #WinNonStockHierarchy from NonStocks.NonStockHierarchy

insert into #WinNonStockHierarchy
select data.Id, 
       data.[Level], 
       data.PrimaryCode, 
       data.CodeDescription 
from (
    select n.Id, 
           1 AS [Level], 
           pDiv.PrimaryCode, 
           pDiv.CodeDescription
    from NonStocks.NonStock n
    inner join StockInfo si on si.IUPC = n.SKU and si.SKU = n.SKU
    inner join ProductHeirarchy pCat on si.category = pCat.PrimaryCode
    inner join ProductHeirarchy pDiv on pCat.ParentCode = pDiv.PrimaryCode
    where pCat.CatalogType = '02'
    and pDiv.CatalogType = '01'

    union

    select n.id, 
           2 AS [Level], 
           pCat.PrimaryCode, 
           pCat.CodeDescription
    from NonStocks.NonStock n
    inner join StockInfo si on si.IUPC = n.SKU and si.SKU = n.SKU
    inner join ProductHeirarchy pCat on si.category = pCat.PrimaryCode
    where pCat.CatalogType = '02'

    union

    select n.Id, 
           3 AS [Level], 
           pClass.PrimaryCode, 
           pClass.CodeDescription
    from NonStocks.NonStock n
    inner join StockInfo si on si.IUPC = n.SKU and si.SKU = n.SKU
    inner join ProductHeirarchy pClass on si.Class = pClass.PrimaryCode
    and si.category = pClass.ParentCode
    where pClass.CatalogType = '03'
) data
order by data.Id, data.PrimaryCode

--Add Non Stock Hierarchy to Merchandising
--Division
declare @sequence int 

select @sequence = NextHi
from HiLo
where sequence = 'Merchandising.HierarchyTagCode'

select level AS LevelId, LevelName AS Name, ROW_NUMBER() OVER(ORDER BY level) as rowNo
into #division
from #WinNonStockHierarchy s
where not exists (select 'a' 
                  from Merchandising.HierarchyTag 
                  where LevelId = 1
                    and UPPER(LTRIM(RTRIM(Name))) = UPPER(LTRIM(RTRIM(s.LevelName)))
                  )
    and Level = 1
group by level, LevelName

insert into Merchandising.HierarchyTag
(LevelId,Name,Code)
select LevelId , Name, @sequence + rowNo
from #division

IF EXISTS (select 1 from #division)
begin
    update HiLo
    set NextHi = (select MAX(rowNo) from #division)
    where sequence = 'Merchandising.HierarchyTagCode'
end

--Department 

select @sequence = NextHi
from HiLo
where sequence = 'Merchandising.HierarchyTagCode'

select level AS LevelId, LevelName AS Name, ROW_NUMBER() OVER(ORDER BY level) as rowNo
into #department
from #WinNonStockHierarchy s
where not exists (select 'a' 
                  from Merchandising.HierarchyTag 
                  where LevelId = 2
                    and UPPER(LTRIM(RTRIM(Name))) = UPPER(LTRIM(RTRIM(s.LevelName)))
                  )
    and Level = 2
group by level, LevelName

insert into Merchandising.HierarchyTag
(LevelId,Name,Code)
select LevelId , Name, @sequence + rowNo
from #department

IF EXISTS (select 1 from #department)
begin
    update HiLo
    set NextHi = (select MAX(rowNo) from #department)
    where sequence = 'Merchandising.HierarchyTagCode'
end

--Class

select @sequence = NextHi
from HiLo
where sequence = 'Merchandising.HierarchyTagCode'

select level AS LevelId, LevelName AS Name, ROW_NUMBER() OVER(ORDER BY level) as rowNo
into #class
from #WinNonStockHierarchy s
where not exists (select 'a' 
                  from Merchandising.HierarchyTag 
                  where LevelId = 3
                    and UPPER(LTRIM(RTRIM(Name))) = UPPER(LTRIM(RTRIM(s.LevelName)))
                  )
    and Level = 3
group by level, LevelName

insert into Merchandising.HierarchyTag
(LevelId,Name,Code)
select LevelId , Name, @sequence + rowNo
from #class

IF EXISTS (select 1 from #class)
begin
    update HiLo
    set NextHi = (select MAX(rowNo) from #class) + 1 
    where sequence = 'Merchandising.HierarchyTagCode'
end

-- Refresh Non Stock Hierarchy
DELETE FROM NonStocks.NonStockHierarchy

-- Refresh Non Stock Hierarchy
INSERT INTO NonStocks.NonStockHierarchy(NonStockId, [Level], LevelKey, LevelName)
select w.NonStockId,
       mp.LevelId,
       mp.Id,
       mp.Name
from #WinNonStockHierarchy w
inner join Merchandising.HierarchyTag mp
    on UPPER(LTRIM(RTRIM(w.LevelName))) = UPPER(LTRIM(RTRIM(mp.Name)))
    and mp.LevelId = w.Level
