--remove previous records from product hierarchy
TRUNCATE TABLE dbo.ProductHeirarchy
insert into productheirarchy
--Product Grouping
select '', '01', '8', 'Electrical', '', 'A' union
select '', '01', '7', 'Furniture', '', 'A' union
select '', '01', 'R', 'Work Station', '', 'A' union
select '', '01', '6', 'Other', '', 'A' union
--Category/Department
select distinct '', '02', convert(varchar, stockinfo.category), codedescript, '8', 'A' from stockinfo
inner join code on code.code = StockInfo.category and code.category in ('pce') union 
select distinct '', '02', convert(varchar,stockinfo.category), isnull(codedescript, ''), '7', 'A' from stockinfo
inner join code on code.code = StockInfo.category and code.category in ('pcf') union 
select distinct '', '02', convert(varchar,stockinfo.category), isnull(codedescript, ''),'R', 'A' from stockinfo
inner join code on code.code = StockInfo.category and code.category in ('pcw') union 
select distinct '', '02', convert(varchar,stockinfo.category), isnull(codedescript, ''), '6', 'A' from stockinfo
inner join code on code.code = StockInfo.category and code.category in ('pco') union 
--Class
select distinct '', '03', convert(varchar,class), isnull(max(codedescript), ''), convert(varchar,stockinfo.category), 'A' 
from stockinfo
inner join code on code.code = StockInfo.category and code.category in ('pce', 'pcf', 'pcw', 'pco')
where class is not null
group by class, stockinfo.category