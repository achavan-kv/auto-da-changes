 if  exists (select * from sysobjects  where name =  'RP_HitRateLostSales' )
drop procedure RP_HitRateLostSales
go

  
create procedure RP_HitRateLostSales  
  
    
-- ================================================      
-- Project      : CoSACS .NET      
-- File Name    : RP_HitRateLostSales.sql      
-- File Type    : MSSQL Server Stored Procedure Script      
-- Title        : Warranty Lost Sales Report     
-- Author       : ??      
-- Date         : ??      
--      
-- This procedure will load details for the Hit Rate Missed Sales in the Warranty Reporting screen.      
--       
-- Change Control      
-- --------------      
-- Date      By  Description      
-- ----      --  -----------      
-- 02/10/08  jec Procedure timing out. Was this ever tested!!!       
--     Correct procedure.      
-- 06/02/09  jec 70816 column formating & change money types to decimal(12,2)      
-- 06/02/09  jec 70817 Change grouping totals   
-- 02/12/09  RKM UAT5.1 9 removed from rp_hitratemissed and adjusted to fix for kit codes 
-- 19/10/10  jec UAT219 correct lost units calc  
-- 13/07/11  ip  CR1254 - RI - #4017 - Warranty Reporting - Lost Warranty Sales Commission. Display the IUPC and Courts Code
--				 for warranties
-- 15/05/12  jec #8063 LW73918 - Cannot Run Warranty reports - lost warranty
-- =================================================================================      
 -- Add the parameters for the stored procedure here      
-- Procedure to get a report of HI       
             
@branch  varchar(6), -- branchno or 'ALL'        
@salesperson varchar(10), -- empeeno or 'ALL'        
@categoryset varchar(32),        
@includecash smallint,        
@includecredit smallint,        
@includespecial smallint,        
@datefrom datetime,        
@dateto datetime,        
@accttypetotal smallint,        
@branchtotal smallint,        
@categorytotal smallint,        
@salespersontotal smallint,            
@return int OUTPUT        
as        
declare @select sqltext,@from sqltext,@where sqltext,@groupby sqltext,@groupby2 sqltext,@nl varchar(32)        
set @nl = ''        
set @return = 0        
        
SET NOCOUNT ON  
 
 create table #hitrate (
  accttype char(1),
  branchno smallint,
  BranchName varchar (30),         
  category varchar(20),
  empeeno integer,
  salesperson varchar(99),
  TotalSales decimal(12,2),        
  WarrantyCost  decimal(12,2),
  MaxWarrantableSales decimal(12,2),        
  ActualWarrantySales decimal(12,2),
  NoofWarrantySales integer,
  NoofWarrantableSales integer,        
  HitRate decimal(12,2),
  --itemno varchar (10),
  itemno varchar (18),									--IP - 12/07/11 - CR1254 - RI 
  stocklocn smallint,
  acctno char(12)not null,        
  refcode varchar(10),
  quantity int,
  datedel datetime,        
  lostsalesvalue decimal(12,2),
  lostunits float, 
  LostCommission decimal(12,2),        
  Warrantydescription varchar(77),
  --warrantyno varchar(10), 
  warrantyno varchar(18),								--IP - 12/07/11 - CR1254 - RI 
  WarrantyCourtsCode varchar(18),						--IP - 12/07/11 - CR1254 - RI 
  contractno varchar(15), 
  buffno int, 
  agrmtno int not null,
  ItemID int,											--IP - 12/07/11 - CR1254 - RI 
  WarrantyID int										--IP - 12/07/11 - CR1254 - RI 
  )        
         
 create clustered index ix_gdsdrtre on #hitrate (acctno,agrmtno)        
         
 set @select = ' insert into #hitrate '   
     + '(branchno, branchname, category, empeeno, acctno, '       
     + 'salesperson, totalsales, warrantycost, ' +         
     + ' MaxWarrantableSales, actualwarrantysales, noofwarrantysales, noofwarrantablesales , '                 
     --+ ' hitrate,itemno,stocklocn,refcode,quantity,datedel, buffno, agrmtno ) '
     + ' hitrate,itemno,stocklocn,refcode,quantity,datedel, buffno, agrmtno, ItemID ) '		--IP - 12/07/11 - CR1254 - RI 
     +  @nl        
     + ' select convert(smallint,left(g.acctno,3)),'''',convert(varchar,s.category), g.empeenosale,g.acctno, '
     + @nl 
     + '  '''',ds.transvalue,0,  ' 
     + @nl 
     + ' 0,0,0,0, 0,'
     --+ ' case ds.parentitemno '
     --+ ' WHEN '''' then ds.itemno '
     --+ ' WHEN NULL then ds.itemno '
     --+ ' ELSE ds.parentitemno END, '
	 + ' s.iupc, '																						--IP - 12/07/11 - CR1254 - RI 
     + ' ds.stocklocn,s.refcode,ds.quantity,ds.datedel,ds.buffno, ds.agrmtno, ' 
     + ' s.ID'																							--IP - 12/07/11 - CR1254 - RI 
             
 set @from =' from delivery ds '        
 --set @from = @from + ' join stockitem s on ds.itemno =s.itemno and ds.stocklocn =s.stocklocn ' + @nl   
 set @from = @from + ' join stockitem s on ds.ItemID =s.ID and ds.stocklocn =s.stocklocn ' + @nl		--IP - 12/07/11 - CR1254 - RI   
 set @from = @from +  ' join agreement g on g.acctno = ds.acctno and g.agrmtno =ds.agrmtno '         
                         
 set @where= ' where ds.datedel between ' 
           + '''' 
           + convert (varchar,@datefrom) 
           + '''' 
           + ' and ' 
           + '''' 
           +  convert (varchar,@dateTO) 
           + '''' 
           + ' and ds.quantity > 0 and ds.delorcoll = ''D'' and s.itemtype !=''N'' '        
        
         
 if @salesperson !='0' and @salesperson !='ALL'        
    set @where = @where + ' and g.empeenosale = ' + @salesperson        
                                    
   declare @acctlike varchar(6),@not varchar(5)        
   set @acctlike =''         
   SET @not =''        
   -- here were are building a string to restrict to account type. If restricting to type and branch        
   -- then query will be even faster. e.g p.acctno like '9104%'        
   -- however if doing a not restriction then need to do and acctno like branchno and acctno not like '___4%'        
                 
   if @includecash = 1 and @includecredit = 0 and @includespecial = 0        
    set @acctlike = @acctlike + '4'        
         
   if @includecash = 0 and @includecredit = 1 and @includespecial = 0        
    set @acctlike = @acctlike + '0'        
         
   if @includecash = 0 and @includecredit = 1 and @includespecial = 1        
   begin        
    set @not = 'not '        
    set @acctlike = @acctlike + '4'        
   end        
         
   if @includecash = 1 and @includecredit = 1 and @includespecial = 0        
   begin        
    set @not = 'not '        
    set @acctlike = @acctlike + '5'        
   end        
         
   if @includecash = 1 and @includecredit = 0 and @includespecial = 1        
   begin        
    set @not = 'not '        
    set @acctlike = @acctlike + '0'        
   end        
         
   if @includecash = 0 and @includecredit = 0 and @includespecial = 1        
    set @acctlike = @acctlike + '5'        
         
         
   if @not !='not ' -- can restrict by branch and accttype        
   begin        
    if @branch !='ALL' -- restrict by branch        
   set @acctlike = @branch + @acctlike + '%'        
  else        
   set @acctlike = '___' + @acctlike + '%'        
  set @where = @where + ' and ds.acctno like ' + '''' +  @acctlike + ''''        
   end        
           
          
 -- here want e.g. and p.acctno like '910%' and p.acctno not like '___4%'        
   if @not = 'not '        
   begin        
   set @acctlike = @acctlike + '%'        
             
   if @branch !='ALL' -- restrict by branch        
   BEGIN        
    set @where = @where + ' and ds.acctno like ' + '''' + @branch  + '%' + ''''        
    set @where = @where + ' and ds.acctno not like ' + '''' + @branch  + @acctlike  + ''''        
   END        
   ELSE        
   BEGIN        
   set @where = @where + ' and ds.acctno not like ' + '''' + '___' + @acctlike + ''''        
   END        
           
   end        
 --PRINT @acctlike        
 --PRINT @where        
   if @categoryset !='' and @categoryset !='ALL'        
   begin        
  set @where = @where + ' and s.category in ( '         
   declare @counter int,@setdata varchar(36)        
   set @counter = 0        
         
  DECLARE set_cursor CURSOR         
    FOR SELECT data        
   from setdetails              
   where setname = @categoryset        
    OPEN set_cursor        
    FETCH NEXT FROM set_cursor INTO @setdata        
         
    WHILE (@@fetch_status <> -1)        
    BEGIN        
     IF (@@fetch_status <> -2)        
     BEGIN        
    set @counter = @counter + 1        
    if @counter > 1        
     set @where =@where + ','        
         
    set @where =@where + ''''        
    set @where =@where + @setdata        
    set @where =@where + ''''        
     END        
     FETCH NEXT FROM set_cursor INTO @setdata        
    END        
    CLOSE set_cursor        
    DEALLOCATE set_cursor        
    set @where = @where + ')'         
   end       
         
         
 declare @statement sqltext        
 set @statement = @select + @from + @where        
 print @statement         
 exec sp_executesql @statement        
 --print @statement        
         
 
--add identity
alter table #hitrate add id int identity
--de-duplicate

DELETE FROM #hitrate
WHERE EXISTS (
	SELECT * FROM #hitrate b
	WHERE #hitrate.acctno = b.acctno
	AND #hitrate.agrmtno = b.agrmtno
	--AND #hitrate.itemno = b.itemno
	AND #hitrate.ItemID = b.ItemID						--IP - 12/07/11 - CR1254 - RI
	AND #hitrate.stocklocn = b.stocklocn
	AND #hitrate.id < b.id
	)
	
--update qty and value from lineitems

UPDATE #hitrate 
SET quantity = l.quantity, totalsales = ordval
FROM lineitem l
--WHERE #hitrate.itemno = l.itemno
WHERE #hitrate.ItemID = l.ItemID						--IP - 12/07/11 - CR1254 - RI			
AND #hitrate.acctno = l.acctno
AND #hitrate.agrmtno = l.agrmtno
AND #hitrate.stocklocn = l.stocklocn
 
             
update #hitrate set NoofWarrantableSales = quantity   
from #hitrate   
where exists   
(        
 select 1 from stockitem s   
 --join warrantyband wb on wb.refcode =s.refcode and (#hitrate.totalsales/#hitrate.quantity) between wb.minprice and wb.maxprice  
  join warrantyband wb on wb.refcode =s.refcode and (#hitrate.totalsales/case when #hitrate.quantity = 0 then 1 else #hitrate.quantity  end) between wb.minprice and wb.maxprice   --IP - 12/07/11 - CR1254 - RI        
 --where #hitrate.itemno=s.itemno and #hitrate.stocklocn=s.stocklocn   
 where #hitrate.ItemID=s.ID and #hitrate.stocklocn=s.stocklocn			--IP - 12/07/11 - CR1254 - RI			
)
  
         
         
 update #hitrate set accttype = a.accttype from acct a where a.acctno = #hitrate.acctno        
          
 update #hitrate set salesperson = convert(varchar,#hitrate.empeeno) + ' ' + FullName from Admin.[User] c where #hitrate.empeeno = c.Id  
         
 --PRINT 's2'        
 -- code replaced jec 02/10/09        
 --update #hitrate         
 --set actualwarrantysales=isnull((select sum(d.transvalue) from delivery D    --   join  lineitem l on l.acctno =D.acctno and L.itemno =D.itemno         
 --     and L.stocklocn =D.stocklocn and L.contractNO=d.contractno        
 --                          join  stockitem s ON s.itemno =l.itemno and S.stocklocn =L.stocklocn        
 --     where s.category in (12, 82) and d.delorcoll !='R'          
 --                          and L.acctno=  #hitrate.acctno and l.parentitemno =#hitrate.itemno AND l.agrmtno = #hitrate.agrmtno        
 --  and L.parentlocation = #hitrate.stocklocn),0)        
 --        
 --        
 --update #hitrate         
 --set NoOfwarrantysales=isnull((select sum(d.quantity) from delivery D        
 --   join  lineitem l on l.acctno =D.acctno and L.itemno =D.itemno         
 --     and L.stocklocn =D.stocklocn and L.contractNO=d.contractno        
 --                          join  stockitem s ON s.itemno =l.itemno and S.stocklocn =L.stocklocn        
 --     where s.category in (12, 82)         
 --     and d.delorcoll !='R'  -- and d.quantity >0        
 --                          and L.acctno=  #hitrate.acctno and l.parentitemno =#hitrate.itemno and l.agrmtno= #hitrate.agrmtno        
 --     and L.parentlocation = #hitrate.stocklocn),0)        
 --        
 ----        
 --update #hitrate         
 --set MaxWarrantableSales=quantity *         
 --isnull((select max(ws.unitpricehp)         
 --from stockitem ws,warrantyband wb,stockitem s        
 -- where         
 --wb.waritemno = ws.itemno AND  wb.refcode = #hitrate.refcode and ws.stocklocn = #hitrate.stocklocn        
 --AND s.itemno=#hitrate.itemno AND s.stocklocn=#hitrate.stocklocn         
 --AND s.unitpricecash BETWEEN wb.minprice AND wb.maxprice ),0)        
         
 -- replaced code jec 02/10/09   
 
 --drop table #allwarranties
  
create table #allwarranties(  
acctno char(12),  
agrmtno int,  
branchno smallint,  
contractno varchar(10),  
delorcoll char(1),  
--witemno varchar(8),  
witemno varchar(18),							--IP - 12/07/11 - CR1254 - RI
wCourtsCode varchar(18),						--IP - 13/07/11 - CR1254 - RI
wdesc VARCHAR (70),
quantity float,  
stocklocn smallint,  
transvalue money,  
--itemno varchar(8),  
itemno varchar(18),								--IP - 12/07/11 - CR1254 - RI
costprice money,  
commission money,
ItemID int,										--IP - 12/07/11 - CR1254 - RI
WarrantyID int									--IP - 12/07/11 - CR1254 - RI				  
)  
  
  
--set @statement =   
--'  
--insert INTO #allwarranties  
--SELECT distinct d.acctno, d.agrmtno, d.branchno, d.contractno,  
--d.delorcoll, d.itemno as witemno,  s.itemdescr1+s.itemdescr2 as wdesc, 
--d.quantity , d.stocklocn, d.transvalue, h.itemno, (s.costprice * d.quantity),  
--isnull(c.commissionamount, 0) as commission
--FROM delivery d   
--INNER JOIN lineitem l  
-- ON l.itemno = d.itemno  
-- AND l.stocklocn = d.stocklocn  
-- AND l.acctno = d.acctno  
-- AND l.contractno = d.contractno  
-- AND l.agrmtno = d.agrmtno  
--inner join agreement a on a.agrmtno = d.agrmtno  
-- and a.acctno = d.acctno  
--left outer join salescommission c on c.employee = a.empeenosale  
-- and c.itemno = d.itemno  
-- and c.AcctNo = d.acctno  
-- and (c.CommissionAmount/ABS(c.commissionamount)) = (d.quantity/ABS(d.quantity))  
-- and c.agrmtno = d.agrmtno  
--inner join stockitem s   
-- on s.itemno = l.itemno  
-- and s.stocklocn = l.stocklocn  
--INNER JOIN #hitrate h  
-- ON h.acctno = d.acctno  
-- AND h.itemno = l.parentitemno  
-- AND h.agrmtno = d.agrmtno  
-- AND h.stocklocn = l.parentlocation  
--WHERE (d.itemno like ''19%''  
--OR d.itemno like ''XW%'') 
--and d.datedel between '   
--   + ''''   
--   + convert (varchar,@datefrom)   
--   + ''''   
--   + ' and '   
--   + ''''   
--   +  convert (varchar,@dateTO)   
--   + ''' 
        
--and (d.delorcoll = ''D'' ) group by d.acctno, d.agrmtno, d.branchno, d.contractno,  
--d.delorcoll, s.itemdescr1, s.itemdescr2, d.itemno, d.quantity , c.commissionamount, d.stocklocn, d.transvalue, h.itemno, s.costprice'  

set @statement =   
'  
insert INTO #allwarranties  
SELECT distinct d.acctno, d.agrmtno, d.branchno, d.contractno,  
d.delorcoll, s.iupc as witemno, s.itemno as wCourtsCode,s.itemdescr1+s.itemdescr2 as wdesc, 
d.quantity , d.stocklocn, d.transvalue, h.itemno, (s.costprice * d.quantity),  
isnull(c.commissionamount, 0) as commission, h.ItemID, d.ItemID as WarrantyID
FROM delivery d   
INNER JOIN lineitem l  
 ON l.ItemID = d.ItemID  
 AND l.stocklocn = d.stocklocn  
 AND l.acctno = d.acctno  
 AND l.contractno = d.contractno  
 AND l.agrmtno = d.agrmtno  
inner join agreement a on a.agrmtno = d.agrmtno  
 and a.acctno = d.acctno  
left outer join salescommission c on c.employee = a.empeenosale  
 and c.ItemID = d.ItemID  
 and c.AcctNo = d.acctno  
 and (c.CommissionAmount/ABS(case when c.commissionamount=0 then 1 else c.commissionamount end)) = (d.quantity/ABS(case when d.quantity =0 then 1 else d.quantity end))  
 and c.agrmtno = d.agrmtno  
inner join stockitem s   
 on s.ID = l.ItemID  
 and s.stocklocn = l.stocklocn  
INNER JOIN #hitrate h  
 ON h.acctno = d.acctno  
 AND h.ItemID = l.ParentItemID  
 AND h.agrmtno = d.agrmtno  
 AND h.stocklocn = l.parentlocation  
 WHERE s.category in (select distinct code from code where category = ''WAR'') 
 and d.datedel between '    
   + ''''   
   + convert (varchar,@datefrom)   
   + ''''   
   + ' and '   
   + ''''   
   +  convert (varchar,@dateTO)   
   + ''' 
        
and (d.delorcoll = ''D'' ) group by d.acctno, d.agrmtno, d.branchno, d.contractno,  
d.delorcoll, s.itemdescr1, s.itemdescr2, s.iupc, s.itemno,d.quantity , c.commissionamount, d.stocklocn, d.transvalue, h.itemno, s.costprice, h.ItemID, d.ItemID' 
  
  print @statement
exec sp_executesql @statement  

delete from #allwarranties 
where (#allwarranties.commission < 0
		and #allwarranties.quantity > 0)
OR  (#allwarranties.commission > 0
		and #allwarranties.quantity < 0)
 
 --drop table #warranties
   
select acctno, agrmtno, branchno, contractno,  
--delorcoll, witemno, wdesc, sum(quantity) as quantity ,  
delorcoll, witemno, wCourtsCode, wdesc, sum(quantity) as quantity ,						--IP - 13/07/11 - CR1254 - RI  
stocklocn, sum(transvalue) as transvalue, itemno, sum(costprice) as costprice,  
sum(commission) as commission,
ItemID, WarrantyID																		--IP - 12/07/11 - CR1254 - RI 

into #warranties  
from #allwarranties  
GROUP BY acctno, agrmtno, branchno, contractno,  
--delorcoll, witemno, wdesc, stocklocn, itemno 
delorcoll, witemno,wCourtsCode, wdesc, stocklocn, itemno, ItemID, WarrantyID			--IP - 12/07/11 - CR1254 - RI					 
  
--drop table #warrantytotals  
  
--get total value and qty of warranties sold on an item  
--select h.acctno, h.itemno, h.stocklocn, witemno, wdesc, sum(w.transvalue) as totalValue, 
select h.acctno, h.itemno, h.stocklocn, witemno, wCourtsCode, wdesc, sum(w.transvalue) as totalValue,	--IP - 13/07/11 - CR1254 - RI   
 sum(w.quantity) as totalQuantity, sum(costprice) as totalCost,  
 sum(commission) as commission, 0 as delExists,
 h.ItemID, w.WarrantyID																	--IP - 12/07/11 - CR1254 - RI													 
into #warrantyTotals  
from #warranties w, #hitrate h  
where h.acctno = w.acctno  
--AND h.itemno = w.itemno 
AND h.ItemID = w.ItemID																	--IP - 12/07/11 - CR1254 - RI   
and h.stocklocn = w.stocklocn  
--group by h.acctno, h.itemno, h.stocklocn, witemno, wdesc
group by h.acctno, h.itemno, h.stocklocn, witemno, wCourtsCode, wdesc, h.ItemID, w.WarrantyID		--IP - 12/07/11 - CR1254 - RI
    
update #hitrate   
set ActualWarrantySales = t.totalValue,  
NoofWarrantySales = t.totalQuantity,  
warrantyCost = t.totalCost,   
warrantyno = witemno,
WarrantyCourtsCode = wCourtsCode,														--IP - 13/07/11 - CR1254 - RI
warrantydescription = wdesc
from #warrantytotals t  
where #hitrate.acctno = t.acctno  
--AND #hitrate.itemno = t.itemno  
AND #hitrate.ItemID = t.ItemID															--IP - 12/07/11 - CR1254 - RI					  
and #hitrate.stocklocn = t.stocklocn 
 
      
          
 --update #hitrate set warrantyno = ws.itemno ,         
 --warrantydescription = (ws.itemdescr1 + ' ' + ws.itemdescr2)        
 --from warrantyband wb,stockitem ws ,stockitem s         
 --where wb.refcode = #hitrate.refcode         
 --AND s.itemno =#hitrate.itemno AND s.stocklocn = #hitrate.stocklocn        
 --AND s.unitpricecash BETWEEN wb.minprice AND wb.maxprice        
 --and ws.itemno = wb.waritemno        
 
 update #hitrate set warrantyno = ws.iupc ,												--IP - 12/07/11 - CR1254 - RI - Replaces above code
 WarrantyCourtsCode = ws.itemno,
 WarrantyID = ws.ID,        
 warrantydescription = (ws.itemdescr1 + ' ' + ws.itemdescr2)        
 from warrantyband wb,stockitem ws ,stockitem s         
 where wb.refcode = #hitrate.refcode         
 AND s.ID =#hitrate.ItemID AND s.stocklocn = #hitrate.stocklocn        
 AND s.unitpricecash BETWEEN wb.minprice AND wb.maxprice        
 and ws.ID = wb.ItemID     
         
         
 --update #hitrate set contractno = D.contractno    
 --from delivery d, lineitem l    
 --where l.parentitemno = #hitrate.itemno     
 --and d.acctno = #hitrate.acctno         
 --and D.itemno = #hitrate.warrantyno       
 --and l.acctno = d.acctno  
 --and l.itemno = d.itemno  
 --and l.agrmtno = d.agrmtno  
 --and l.stocklocn = d.stocklocn  
 --and l.contractno = d.contractno  
     
 --update #hitrate set contractno = d.contractno    
 --from delivery d, lineitem l    
 --where l.parentitemno = #hitrate.itemno     
 --and d.acctno = #hitrate.acctno     
 --and D.itemno = #hitrate.warrantyno    
 --and D.buffno = #hitrate.buffno    
 --and l.acctno = d.acctno  
 --and l.itemno = d.itemno  
 --and l.agrmtno = d.agrmtno  
 --and l.stocklocn = d.stocklocn  
 --and l.contractno = d.contractno  
 --and #hitrate.contractno is null    
 --and not exists (select * from #hitrate where itemno = d.itemno and contractno = d.contractno)   
 --and #hitrate.buffno = (select top 1 h.buffno from #hitrate h, lineitem l  
 --where l.parentitemno = h.itemno       
 --and D.itemno = h.warrantyno        
 --and d.acctno = h.acctno    
 --and h.contractno is null   
 --and l.acctno = d.acctno  
 --and l.itemno = d.itemno  
 --and l.agrmtno = d.agrmtno  
 --and l.stocklocn = d.stocklocn  
 --and l.contractno = d.contractno  
 -- )  
       
 update #hitrate set warrantycost =  s.costprice  from        
 --stockitem s where s.itemno = #hitrate.warrantyno and s.stocklocn = #hitrate.branchno    
  stockitem s where s.ID = #hitrate.WarrantyID and s.stocklocn = #hitrate.branchno				--IP - 12/07/11 - CR1254 - RI      
    
   select #hitrate.acctno,#hitrate.itemno,  
  CASE sum(d.quantity)-count(distinct d.contractno)  
   WHEN 0 THEN ISNULL(sum(d.transvalue)/sum(d.quantity), 0)  
   ELSE ISNULL(sum(d.transvalue)/(sum(d.quantity)-count(distinct d.contractno)),0)   
  END  
  as warrantysales,#hitrate.ItemID																 --IP - 12/07/11 - CR1254 - RI 
 into #actualwarrantysales        
 from  #hitrate, delivery D        
    --join  lineitem l on l.acctno =D.acctno and L.itemno =D.itemno   
   join  lineitem l on l.acctno =D.acctno and L.ItemID =D.ItemID        
   and L.stocklocn =D.stocklocn           
   --join  stockitem s ON s.itemno =l.itemno and S.stocklocn =L.stocklocn   
   join  stockitem s ON s.ID =l.ItemID and S.stocklocn =L.stocklocn								 --IP - 12/07/11 - CR1254 - RI      
   where s.category in (select distinct code from code where category = 'WAR')         
     --and L.acctno=  #hitrate.acctno and l.parentitemno =#hitrate.itemno AND l.agrmtno = #hitrate.agrmtno    
     and L.acctno=  #hitrate.acctno and l.ParentItemID =#hitrate.ItemID AND l.agrmtno = #hitrate.agrmtno         
     and L.parentlocation = #hitrate.stocklocn           
      --group by #hitrate.acctno,#hitrate.itemno 
     group by #hitrate.acctno,#hitrate.itemno, #hitrate.ItemID									--IP - 12/07/11 - CR1254 - RI            
         
 update #hitrate         
 set actualwarrantysales = warrantysales        
 from #actualwarrantysales        
 --where #hitrate.acctno=#actualwarrantysales.acctno and #hitrate.itemno=#actualwarrantysales.itemno   
 where #hitrate.acctno=#actualwarrantysales.acctno and #hitrate.ItemID=#actualwarrantysales.ItemID	--IP - 12/07/11 - CR1254 - RI          
  
 -- number of warranty sales      
     
      
  --select #hitrate.acctno,#hitrate.itemno, #hitrate.stocklocn,ISNULL(count(distinct d.contractno),0) as nowarrantysales  
 select #hitrate.acctno,#hitrate.itemno, #hitrate.stocklocn,ISNULL(count(distinct d.contractno),0) as nowarrantysales, #hitrate.ItemID	--IP - 12/07/11 - CR1254 - RI         
 into #nowarrantysales        
 from #hitrate, delivery D        
    --join  lineitem l on l.acctno =D.acctno and L.itemno =D.itemno   
   join  lineitem l on l.acctno =D.acctno and L.ItemID =D.ItemID											--IP - 12/07/11 - CR1254 - RI         
   and L.stocklocn =D.stocklocn
   and L.contractNO=d.contractno        
   --join  stockitem s ON s.itemno =l.itemno and S.stocklocn =L.stocklocn 
   join  stockitem s ON s.ID =l.ItemID and S.stocklocn =L.stocklocn											--IP - 12/07/11 - CR1254 - RI         
   where s.category in (select distinct code from code where category = 'WAR')        and d.quantity >0           
   --and L.acctno=  #hitrate.acctno and l.parentitemno =#hitrate.itemno and l.agrmtno= #hitrate.agrmtno  
   and L.acctno=  #hitrate.acctno and l.ParentItemID =#hitrate.ItemID and l.agrmtno= #hitrate.agrmtno		--IP - 12/07/11 - CR1254 - RI       
   and L.parentlocation = #hitrate.stocklocn and #hitrate.datedel = d.datedel       
   --group by #hitrate.acctno,#hitrate.itemno, #hitrate.stocklocn    
   group by #hitrate.acctno,#hitrate.itemno, #hitrate.stocklocn, #hitrate.ItemID							--IP - 12/07/11 - CR1254 - RI    
         
 update #hitrate         
 set NoOfwarrantysales=nowarrantysales        
 from #nowarrantysales         
 where #hitrate.acctno=#nowarrantysales.acctno        
 --and #hitrate.itemno=#nowarrantysales.itemno  
 and #hitrate.ItemID=#nowarrantysales.ItemID																--IP - 12/07/11 - CR1254 - RI       
 and #hitrate.stocklocn  = #nowarrantysales.stocklocn 
      
 -- Warrantable Sales        
 --select #hitrate.itemno,max(ws.unitpricehp) as unitprice        
 --into #MaxWarrantableSales        
 --from stockitem ws,warrantyband wb,stockitem s,#hitrate        
 -- where wb.waritemno = ws.itemno AND  wb.refcode = #hitrate.refcode and ws.stocklocn = #hitrate.stocklocn        
 --AND s.itemno=#hitrate.itemno AND s.stocklocn=#hitrate.stocklocn         
 --AND s.unitpricecash BETWEEN wb.minprice AND wb.maxprice        
 --group by #hitrate.itemno  
 
  -- Warrantable Sales        
 select #hitrate.itemno,max(ws.unitpricehp) as unitprice, #hitrate.ItemID									--12/07/11 - CR1254 - RI - Replaces above code      
 into #MaxWarrantableSales        
 from stockitem ws,warrantyband wb,stockitem s,#hitrate        
 where wb.ItemID = ws.ID AND  wb.refcode = #hitrate.refcode and ws.stocklocn = #hitrate.stocklocn        
 AND s.ID=#hitrate.ItemID AND s.stocklocn=#hitrate.stocklocn         
 AND s.unitpricecash BETWEEN wb.minprice AND wb.maxprice        
 group by #hitrate.itemno, #hitrate.ItemID           
         
 update #hitrate         
 set MaxWarrantableSales=quantity * unitprice        
 from #MaxWarrantableSales        
 --where #hitrate.itemno=#MaxWarrantableSales.itemno   
 where #hitrate.ItemID=#MaxWarrantableSales.ItemID															--12/07/11 - CR1254 - RI        
         
 -- edn of replaced code        
         
 -- this is for price falls        
 update #hitrate set maxwarrantablesales =actualwarrantysales where actualwarrantysales>maxwarrantablesales        
         
 -- this is to update the value to be equal where there have been price changes as the above code only stores current prices        
 update #hitrate set maxwarrantablesales =actualwarrantysales where NoofWarrantySales=NoofWarrantableSales        
          
 update #hitrate set HitRate = convert(float,NoofWarrantySales)/convert(float, NoofWarrantableSales) * 100  
 where NoofWarrantableSales >0  
         
 update #hitrate  set category =LEFT((code + '-' + codedescript),20)         
 from code        
 where code.code = #hitrate.category and         
 code.category in ('PCE','PCF','PCW')        
         
      
           
     --PRINT 'HERE 1'        
     declare @TargetSalespercent float        
    select @TargetSalespercent= convert(float,value) from countrymaintenance where name ='Target Warranty Hit Rate %'       
          
        
        
     update #hitrate set lostunits =NoofWarrantySales-(NoOfwarrantablesales*ROUND( CONVERT(VARCHAR,@TargetSalespercent) /100.0,2)),    --UAT219 jec 19/10/10     
     lostsalesvalue =actualwarrantysales  -(Maxwarrantablesales*ROUND(CONVERT(VARCHAR,@TargetSalespercent) /100.0,2))      
            
     --update #hitrate set LostCommission = lostsalesvalue  * ISNULL(percentage,0)/100.00        
     --from SalesCommissionRatesAudit s where  #hitrate.warrantyno like s.itemtext + '%'         
     --and #hitrate.datedel between s.datefrom and s.dateto and commissiontype NOT IN ('SP','PC')        
     --  AND s.Percentage >0 AND s.ActionType !='D'  
       
     --IP - 14/07/11 - CR1254 - RI - Replaces the above code.
     update #hitrate set LostCommission = lostsalesvalue  * ISNULL(percentage,0)/100.00        
     from SalesCommissionRatesAudit s, #hitrate h inner join stockinfo si on h.WarrantyID = si.ID
     where ((commissiontype = 'P' and si.iupc = s.itemtext)--Product
			or(commissiontype = 'SK' and si.sku = s.itemtext) --SKU
			or(commissiontype = 'PS' and si.subclass = s.itemtext) --Subclass
			or(commissiontype = 'PL' and si.Class = s.itemtext)) --Class     
     and h.datedel between s.datefrom and s.dateto    
     AND s.Percentage >0 AND s.ActionType !='D'   
             
            
     update #hitrate set LostCommission = CONVERT(FLOAT,lostsalesvalue) *percentage/100.00        
     from SalesCommissionRatesAudit s        
     where ABS(ISNULL(LostCommission,0)) <1        
       and #hitrate.datedel between s.datefrom and s.dateto        
       and S.commissiontype ='PC'  
       and S.ItemText in (select code from code where category = 'WAR')      
     --  AND ((LEFT(warrantyNO,2) ='19' AND S.ItemText ='12') OR        
     --(LEFT(warrantyNO,2) ='XW' and S.ItemText ='82'))  
       AND((select category from stockinfo where ID = #hitrate.WarrantyID) in (select code from code where category = 'WAR'))	--IP - 12/07/11 - CR1254 - RI        
       AND s.Percentage >0 AND s.ActionType !='D'        
            
    --SELECT * FROM #hitrate        
    --SELECT * FROM SalesCommissionRatesAudit        
   
         
 declare  @comma varchar(9), @finalorder sqltext, @orderby varchar(222)        
   set @groupby =' '        
   set @groupby2 =' '        
   set @comma = ' '        
   SET @orderby =' '        
         
 -- here we are getting totals.         
 -- if we are to have totals within the body of the results then we will need to insert these back into the         
 -- #hitrate table. Then we would need to retrieve in that order.        
 -- for instan        
   create table #missedtotals  (TotalSales money,NoofWarrantySales integer,        
  WarrantyCost  money,MaxWarrantableSales money,ActualWarrantySales money,        
  NoofWarrantableSales integer,HitRate float,        
  lostsalesvalue money, lostunits float, LostCommission money,        
  --WarrantyNo varchar(10),Warrantydescription varchar(65),warrantyprice MONEY,Branchname varchar(65), 
   WarrantyNo varchar(18), WarrantyCourtsCode varchar(18),Warrantydescription varchar(65),warrantyprice MONEY,Branchname varchar(65),	--IP - 12/07/11 - CR1254 - RI        
   accttype char(1), branchno smallint, category varchar(32), salesperson varchar(99)) -- these          
                   
         
  create table #hr(TotalSales money, NoofWarrantySales int ,        
    WarrantyCost money, MaxWarrantableSales money ,ActualWarrantySales money,        
  NoofWarrantableSales int, HitRate float,         
   accttype char(1), branchno smallint, category varchar(32), salesperson varchar(99) )        
         
  set @salespersontotal = 1        
 if @accttypetotal =1 or @branchtotal =1 or @categorytotal = 1 or @salespersontotal =1        
 begin        
         
         
      set @statement = ' insert into #missedtotals  ' +        
   ' select sum(TotalSales)  , sum(NoofWarrantySales) , ' +        
   ' sum(WarrantyCost)   , '  +        
   ' sum(MaxWarrantableSales) , sum(ActualWarrantySales)  ,' +        
   '  sum(NoofWarrantableSales)  ,' +        
   ' sum(HitRate), ' +        
   --' sum(lostsalesvalue) , sum(lostunits)  , sum(LostCommission),warrantyno,Warrantydescription,0,'''' '      
   ' sum(lostsalesvalue) , sum(lostunits)  , isnull(sum(LostCommission),0),warrantyno, WarrantyCourtsCode,Warrantydescription,0,'''' '  --IP - 12/07/11 - CR1254 - RI      
         
    if @accttypetotal =1        
 begin        
   set @statement = @statement + ' ,accttype '        
   set @groupby = @groupby + ' accttype  '        
   set @orderby = @orderby + ' accttype desc '        
      set @comma = ' , '        
         
 end        
 else        
    set @statement = @statement + ', convert(varchar(1),'''')   '        
         
 if @branchtotal =1        
 begin        
    set @statement = @statement + ' ,branchno '        
    set @groupby = @groupby + @comma +  ' branchno,branchname  '        
    set @orderby = @orderby + @comma +  ' branchno,branchname  '        
    set @comma = ' , '        
 end        
 else        
    set @statement = @statement + ', convert(smallint,NULL)   '        
         
 if @categorytotal =1        
 begin        
    set @statement = @statement + ' ,category '        
    set @groupby = @groupby +  @comma + ' category '        
    set @orderby = @orderby  +  @comma + ' category desc '        
    set @comma = ' , '        
 end        
 else        
    set @statement = @statement + ', convert(smallint,NULL)  '        
         
         
 if @salespersontotal =1        
 begin        
    set @statement = @statement + ' ,salesperson '        
    set @groupby = @groupby +  @comma + ' salesperson '        
    set @orderby = @orderby +  @comma + ' salesperson '        
    set @comma = ' , '        
 end        
 else        
    set @statement = @statement + ', convert( varchar(8),''Total'')  '        
         
    --set @statement = @statement + ', WarrantyID '										--IP - 12/07/11 - CR1254 - RI 
       
   SET @groupby2=@groupby       
   --SET @groupby2 = @groupby2 + ', WarrantyCourtsCode '									--IP - 12/07/11 - CR1254 - RI    
        
   set @groupby =  @groupby +  @comma + ' Warrantyno,Warrantydescription, WarrantyCourtsCode '  
     --set @groupby =  @groupby +  @comma + ' Warrantyno, Warrantydescription'	--IP - 12/07/11 - CR1254 - RI      
           
   set @statement = @statement + '  from #hitrate group by ' + @groupby      
 			  
   --print @statement        
   exec sp_executesql @statement        
 end        
         
       
 -- reinserting totals into main table         
   update #missedtotals set hitrate = convert(float,NoofWarrantySales)/convert(float, NoofWarrantableSales) * 100 where NoofWarrantableSales >0        
          
   update #missedtotals set lostunits = NoofWarrantySales -( NoofWarrantableSales* @TargetSalespercent/100.0)           
   update #missedtotals set lostunits = 0 where lostunits >0        
   update #missedtotals set lostcommission = 0 where lostcommission>0        
   --UPDATE #missedtotals SET Lostcommission = Lostcommission * lostunits     
       
   --UPDATE #missedtotals SET warrantyprice=s.unitpricecash         
   --FROM stockitem s, #hitrate    
   --WHERE s.itemno = #hitrate.Warrantyno AND s.stocklocn = #hitrate.branchno     
   --and #hitrate.warrantyno = #missedtotals.warrantyno       
         
       
   --UPDATE #missedtotals SET TotalSales =NoofWarrantableSales * warrantyprice        
   --UPDATE  #missedtotals SET totalSales = warrantyprice * NoofWarrantySales        
           
     update #missedtotals set branchname = b.branchname from branch b where b.branchno =  #missedtotals.branchno        
      set @statement = 'SELECT '         
      IF @accttypetotal=1        
      BEGIN        
     set @statement = @statement + 'accttype as ''Account Type'',   ' + @nl          
      END        
      IF @branchtotal =1        
      BEGIN        
     SET @statement = @statement +        
       'branchno as ''Branch Number'', BranchName as ''Branch Name'', ' + @NL         
      END           
      IF @categorytotal=1        
      BEGIN        
     SET @statement= @statement +  ' category as ''Product Category'', ' + @NL         
                 
      END        
      if @salespersontotal =1        
      BEGIN        
     set @statement=@statement +    ' salesperson as ''Sales Person'',  '         
      END        
                 
      --set @statement = @statement + ' Warrantyno as Warranty,Warrantydescription as Description, ' + @nl +   
      set @statement = @statement + ' Warrantyno as Warranty, WarrantyCourtsCode as''Warranty Courts Code'', Warrantydescription as Description, ' + @nl +  --IP - 12/07/11 - CR1254 - RI     
           ' maxwarrantablesales as ''Max Warranty Sales Value'', ' + @nl +         
           ' ActualWarrantySales as ''Actual Warranty Sales Value'', ' + @NL +        
        ' NoofWarrantySales as ''Warranties Sold'', ' + @NL +        
           ' NoofWarrantableSales as ''Max No of Warrantable Sales'',  ' +        
           ' HitRate as ''Attachment %'',  ' + @NL +        
      'convert(money,  round('+CONVERT(VARCHAR, @TargetSalespercent)+' /100.00* maxwarrantablesales, 2)) as ''Required Sales Value'', '  + @nl +        
        + 'Case when ActualWarrantySales -ROUND(' + CONVERT(VARCHAR,@TargetSalespercent) + '.0/100.0* maxwarrantablesales,2)>=0 then 0 else ActualWarrantySales -convert(money,  round('+CONVERT(VARCHAR, @TargetSalespercent)+  
        ' /100.00* maxwarrantablesales, 2))  end as ''Lost Sales Value'', '  + @nl +         
        ' LostUnits as ''Lost Units'', ' +         
           ' LostCommission as ''Lost Commission'' '  + @NL + -- jec 70816        
      ' FROM #missedtotals '+        
      ' union all ' +              
     --@statement + ' '''',''Total'' , Sum(maxwarrantablesales),   --IP - 13/07/11 - CR1254 - RI - replaced with below  
     @statement + ' '''', '''', ''Total'' , Sum(maxwarrantablesales),       
     Sum(ActualWarrantySales),       
     Sum(noofWarrantySales),      
     sum(NoofWarrantableSales),' +@nl+      
     --convert(float, Sum(noofWarrantySales))/convert(float, sum(NoofWarrantableSales))*100,' +@nl+ --IP - 13/07/11 - CR1254 - RI - Replaced by below    
     'convert(float, Sum(noofWarrantySales))/convert(float, case when sum(NoofWarrantableSales) = 0 then 1 else sum(NoofWarrantableSales) end)*100,' +@nl+       
     'sum( convert(money,  round('+CONVERT(VARCHAR, @TargetSalespercent)+' /100.00* maxwarrantablesales, 2)) ) as ''Required Sales Value'',' + @nl +      
     '      
     Sum(Case when ActualWarrantySales -ROUND(' + CONVERT(VARCHAR,@TargetSalespercent) + '.0/100.0* maxwarrantablesales,2)>=0 then 0 else ActualWarrantySales -convert(money,  round('+CONVERT(VARCHAR, @TargetSalespercent)+' /100.00* maxwarrantablesales, 2)
)    
     end ) as ''Lost Sales Value'','    
     +@nl+         
     'sum(lostUnits),       
     sum(LostCommission) '+        
      ' FROM #missedtotals '+ 'GROUP BY'  + @groupby2  +         
      'ORDER BY ' + @groupby         
  -- PRINT @STATEMENT        
           
   exec sp_executesql @statement        
                                          
              
 
GO