if  exists (select * from sysobjects  where name =  'RP_HitRateMissed' )
drop procedure RP_HitRateMissed
go
create procedure RP_HitRateMissed     
    
-- ================================================      
-- Project      : CoSACS .NET      
-- File Name    : RP_HitRateMissed.sql      
-- File Type    : MSSQL Server Stored Procedure Script      
-- Title        : Warranty Hit Rate Missed report      
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
-- 19/07/11  ip  CR1254 - RI - #4015 - Warranty Reporting. RI Integration changes. Changed to join on ItemID.   
-- =================================================================================      
 -- Add the parameters for the stored procedure here      
-- Procedure to get a report of HI       
      
@warrantytype varchar(3), -- HR         
-- MS for missed sales,         
--or LC for lost warrantySales Commission        
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
@includeRep smallint,  
@includeCanc smallint,              
@return int OUTPUT        
        
--set @warrantytype ='MS'        
--set @branch = 'ALL'        
--set @salesperson='ALL'        
--set @categoryset =null        
--set @includecash =1        
--set @includecredit =1        
--set @includespecial =1        
--set @datefrom ='01-mar-2009'        
--set @dateto ='01-mar-2009'        
--set @accttypetotal =1        
--set @branchtotal =0        
--set @categorytotal =0        
--set @salespersontotal =0
--set @includeRep =0
--set @includeCanc =0         
----@return =0        
as        
declare @select sqltext,@from sqltext,@where sqltext,@groupby sqltext,@groupby2 sqltext,@nl varchar(32)        
set @nl = ''        
set @return = 0        
        
SET NOCOUNT ON     
  
if @warrantytype != 'HR'  
BEGIN  
 create table #hitrate (accttype char(1),branchno smallint,BranchName varchar (30),         
  category varchar(20),empeeno integer,salesperson varchar(99),TotalSales decimal(12,2),        
  WarrantyCost  decimal(12,2),MaxWarrantableSales decimal(12,2),        
  ActualWarrantySales decimal(12,2),NoofWarrantySales integer,NoofWarrantableSales integer,        
  --HitRate decimal(12,2),itemno varchar (10),stocklocn smallint,acctno char(12)not null,  
  HitRate decimal(12,2),itemno varchar (18),stocklocn smallint,acctno char(12)not null,									--IP - 14/07/11 - CR1254 - RI       
  refcode varchar(10),quantity int,datedel datetime,        
  lostsalesvalue decimal(12,2),lostunits float, LostCommission decimal(12,2),        
  --Warrantydescription varchar(77),warrantyno varchar(10), contractno varchar(15), buffno int, agrmtno int not null)   
  Warrantydescription varchar(77),warrantyno varchar(18), contractno varchar(15), buffno int, agrmtno int not null,		--IP - 14/07/11 - CR1254 - RI	
  ItemID int, WarrantyID int)																							--IP - 14/07/11 - CR1254 - RI														           
         
 create clustered index ix_gdsdrtre on #hitrate (acctno,agrmtno)        
         
 set @select = ' insert into #hitrate         
       (branchno, branchname, category, empeeno, acctno,        
     salesperson, totalsales, warrantycost, ' +         
     + ' MaxWarrantableSales, actualwarrantysales, noofwarrantysales, noofwarrantablesales , '         
    +        
   --+ ' hitrate,itemno,stocklocn,refcode,quantity,datedel, buffno, agrmtno ) ' +  @nl    
      + ' hitrate,itemno,stocklocn,refcode,quantity,datedel, buffno, agrmtno, ItemID ) ' +  @nl		--IP - 14/07/11 - CR1254 - RI      
     + ' select convert(smallint,left(g.acctno,3)),'''',convert(varchar,s.category), g.empeenosale,g.acctno, '+ @nl +        
      '  '''',ds.transvalue,0,  ' + @nl +        
      --' 0,0,0,0, 0,ds.itemno,ds.stocklocn,s.refcode,ds.quantity,ds.datedel,ds.buffno, ds.agrmtno '    
      ' 0,0,0,0, 0,s.iupc as itemno,ds.stocklocn,s.refcode,ds.quantity,ds.datedel,ds.buffno, ds.agrmtno, ds.ItemID '  --IP - 14/07/11 - CR1254 - RI      
 set @from =' from delivery ds '        
 --set @from = @from + ' join stockitem s on ds.itemno =s.itemno and ds.stocklocn =s.stocklocn ' + @nl   
 set @from = @from + ' join stockitem s on ds.ItemID =s.ID and ds.stocklocn =s.stocklocn ' + @nl		--IP - 14/07/11 - CR1254 - RI        
 set @from = @from +  ' join agreement g on g.acctno = ds.acctno and g.agrmtno =ds.agrmtno '         
                         
 set @where= ' where ds.datedel between ' + '''' + convert (varchar,@datefrom) + '''' + ' and ' + '''' +  convert (varchar,@dateTO        
 ) + '''' + ' and ds.quantity > 0 and ds.delorcoll = ''D'' and s.itemtype !=''N'' '        
 -- + ' and ds.acctno = ''720007717181''  '        
         
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
 --print @statement         
 exec sp_executesql @statement        
 --print @statement        
         
             
 update #hitrate set NoofWarrantableSales = quantity from #hitrate 
 where exists (        
	 select 1 from stockitem s  join warrantyband wb on wb.refcode =s.refcode and 
	 (#hitrate.totalsales/ #hitrate.quantity) 
	 between wb.minprice and wb.maxprice        
	 --where #hitrate.itemno=s.itemno and #hitrate.stocklocn=s.stocklocn   
	 where #hitrate.ItemID=s.ID and #hitrate.stocklocn=s.stocklocn							--IP - 14/07/11 - CR1254 - RI    
 AND #hitrate.quantity <> 0)
  
                
 update #hitrate set accttype = a.accttype from acct a where a.acctno = #hitrate.acctno        
          
 update #hitrate set salesperson = convert(varchar,#hitrate.empeeno) + ' ' + FullName from Admin.[User] c where #hitrate.empeeno = c.Id  
         
         
 -- replaced code jec 02/10/09        
      
 --update #hitrate set warrantyno = ws.itemno ,         
 --warrantydescription = (ws.itemdescr1 + ' ' + ws.itemdescr2)        
 --from warrantyband wb,stockitem ws ,stockitem s         
 --where wb.refcode = #hitrate.refcode         
 --AND s.itemno =#hitrate.itemno AND s.stocklocn = #hitrate.stocklocn        
 --AND s.unitpricecash BETWEEN wb.minprice AND wb.maxprice        
 --and ws.itemno = wb.waritemno   
 
 --IP - 14/07/11 - CR1254 - RI - Replaces the above code
  update #hitrate set warrantyno = ws.iupc ,         
 warrantydescription = (ws.itemdescr1 + ' ' + ws.itemdescr2),
 WarrantyID = ws.ID        
 from warrantyband wb,stockitem ws ,stockitem s         
 where wb.refcode = #hitrate.refcode         
 AND s.ID =#hitrate.ItemID AND s.stocklocn = #hitrate.stocklocn        
 AND s.unitpricecash BETWEEN wb.minprice AND wb.maxprice        
 and ws.ItemID = wb.ItemID        
              
         
         
 update #hitrate set contractno = D.contractno    
 from delivery d, lineitem l    
 --where d.parentitemno = #hitrate.itemno  -- now using delivery parent item no in case of identical exchanges. 
 where d.ParentItemID = #hitrate.ItemID  -- now using delivery parent item no in case of identical exchanges.	--IP - 14/07/11 - CR1254 - RI
 and d.acctno = #hitrate.acctno      
 and D.delorcoll ='D'    
 --and D.itemno = #hitrate.warrantyno   
 and D.ItemID = #hitrate.WarrantyID								--IP - 14/07/11 - CR1254 - RI     
 and D.buffno = #hitrate.buffno    
 and l.acctno = d.acctno  
 --and l.itemno = d.itemno  
 and l.ItemID = d.ItemID										--IP - 14/07/11 - CR1254 - RI  
 and l.agrmtno = d.agrmtno  
 and l.stocklocn = d.stocklocn  
 and l.contractno = d.contractno  
     
 update #hitrate set contractno = d.contractno    
 from delivery d, lineitem l    
 --where d.parentitemno = #hitrate.itemno  
 where d.ParentItemID = #hitrate.ItemID							--IP - 14/07/11 - CR1254 - RI     
 and d.acctno = #hitrate.acctno      
 and D.delorcoll ='D'    
 --and D.itemno = #hitrate.warrantyno 
 and D.ItemID = #hitrate.WarrantyID								--IP - 14/07/11 - CR1254 - RI	      
 and D.buffno = #hitrate.buffno    
 and l.acctno = d.acctno  
 --and l.itemno = d.itemno 
 and l.ItemID = d.ItemID										--IP - 14/07/11 - CR1254 - RI   
 and l.agrmtno = d.agrmtno  
 and l.stocklocn = d.stocklocn  
 and l.contractno = d.contractno  
 and #hitrate.contractno is null    
 --and not exists (select * from #hitrate where itemno = d.itemno and contractno = d.contractno) 
 and not exists (select * from #hitrate where ItemID = d.ItemID and contractno = d.contractno)		--IP - 14/07/11 - CR1254 - RI   
 and #hitrate.buffno = (select top 1 h.buffno from #hitrate h, lineitem l  
 --where d.parentitemno = h.itemno    
 where d.ParentItemID = h.ItemID								--IP - 14/07/11 - CR1254 - RI      
 --and D.itemno = h.warrantyno   
 and D.ItemID = h.WarrantyID									--IP - 14/07/11 - CR1254 - RI     
 and D.delorcoll ='D'    
 and d.acctno = h.acctno    
 and h.contractno is null   
 and l.acctno = d.acctno  
 --and l.itemno = d.itemno 
 and l.ItemID = d.ItemID										--IP - 14/07/11 - CR1254 - RI   
 and l.agrmtno = d.agrmtno  
 and l.stocklocn = d.stocklocn  
 and l.contractno = d.contractno  
  )  
       
 update #hitrate set warrantycost =  s.costprice  from        
 --stockitem s where s.itemno = #hitrate.warrantyno and s.stocklocn = #hitrate.branchno  
 stockitem s where s.ID = #hitrate.WarrantyID and s.stocklocn = #hitrate.branchno			--IP - 14/07/11 - CR1254 - RI       
    
   --select #hitrate.acctno,#hitrate.itemno,
  select #hitrate.acctno,#hitrate.itemno,#hitrate.ItemID,		--IP - 14/07/11 - CR1254 - RI    
  CASE sum(d.quantity)-count(distinct d.contractno)  
   WHEN 0 THEN ISNULL(sum(d.transvalue)/(CASE sum(d.quantity)
						WHEN 0 THEN 1
						ELSE sum(d.quantity)
						END), 0)  
   ELSE ISNULL(sum(d.transvalue)/(CASE (sum(d.quantity)-count(distinct d.contractno))
						WHEN 0 THEN 1
						ELSE (sum(d.quantity)-count(distinct d.contractno))
						END),0)   
  END  
  as warrantysales   
 into #actualwarrantysales        
 from  #hitrate, delivery D        
    --join  lineitem l on l.acctno =D.acctno and L.itemno =D.itemno 
   join  lineitem l on l.acctno =D.acctno and L.ItemID =D.ItemID				--IP - 14/07/11 - CR1254 - RI           
   and L.stocklocn =D.stocklocn   
   --and L.contractNO=d.contractno        
   --join  stockitem s ON s.itemno =l.itemno and S.stocklocn =L.stocklocn 
   join  stockitem s ON s.ID =l.ItemID and S.stocklocn =L.stocklocn				--IP - 14/07/11 - CR1254 - RI	        
   where s.category in (select distinct code from code where category = 'WAR') and d.delorcoll ='D'          
     --and L.acctno=  #hitrate.acctno and d.parentitemno =#hitrate.itemno AND l.agrmtno = #hitrate.agrmtno        -- using delivery
     and L.acctno=  #hitrate.acctno and d.ParentItemID =#hitrate.ItemID AND l.agrmtno = #hitrate.agrmtno        -- using delivery		--IP - 14/07/11 - CR1254 - RI
     and l.parentlocation = #hitrate.stocklocn     
     --and d.contractno = #hitrate.contractno       
      group by #hitrate.acctno,#hitrate.itemno        
      
 update #hitrate         
 set actualwarrantysales = warrantysales        
 from #actualwarrantysales        
 --where #hitrate.acctno=#actualwarrantysales.acctno and #hitrate.itemno=#actualwarrantysales.itemno  
 where #hitrate.acctno=#actualwarrantysales.acctno and #hitrate.ItemID=#actualwarrantysales.ItemID		--IP - 14/07/11 - CR1254 - RI          
  
 -- number of warranty sales      
     
      
  --select #hitrate.acctno,#hitrate.itemno, #hitrate.stocklocn,ISNULL(count(distinct d.contractno),0) as nowarrantysales   
 select #hitrate.acctno,#hitrate.itemno, #hitrate.stocklocn,ISNULL(count(distinct d.contractno),0) as nowarrantysales, #hitrate.ItemID	--IP - 14/07/11 - CR1254 - RI         
 into #nowarrantysales        
 from #hitrate, delivery D        
   --join  lineitem l on l.acctno =D.acctno and L.itemno =D.itemno 
   join  lineitem l on l.acctno =D.acctno and L.ItemID =D.ItemID										--IP - 14/07/11 - CR1254 - RI          
   and L.stocklocn =D.stocklocn-- and l.parentitemno = d.ParentItemNo  
   and L.contractNO=d.contractno        
   --join  stockitem s ON s.itemno =l.itemno and S.stocklocn =L.stocklocn 
   join  stockitem s ON s.ID =l.ItemID and S.stocklocn =L.stocklocn										--IP - 14/07/11 - CR1254 - RI        
   where s.category in (select distinct code from code where category = 'WAR')         
   and d.delorcoll ='D'   and d.quantity >0       
   --and d.contractno = #hitrate.contractno     
   --and L.acctno=  #hitrate.acctno and l.parentitemno =#hitrate.itemno and l.agrmtno= #hitrate.agrmtno  
   and L.acctno=  #hitrate.acctno and l.ParentItemID =#hitrate.ItemID and l.agrmtno= #hitrate.agrmtno	--IP - 14/07/11 - CR1254 - RI      
   and L.parentlocation = #hitrate.stocklocn and #hitrate.datedel = d.datedel       
   group by #hitrate.acctno,#hitrate.itemno, #hitrate.stocklocn        
         
 update #hitrate         
 set NoOfwarrantysales=nowarrantysales        
 from #nowarrantysales         
 where #hitrate.acctno=#nowarrantysales.acctno        
 --and #hitrate.itemno=#nowarrantysales.itemno 
 and #hitrate.ItemID=#nowarrantysales.ItemID															--IP - 14/07/11 - CR1254 - RI       
 and #hitrate.stocklocn  = #nowarrantysales.stocklocn      
 -- Warrantable Sales        
 --select #hitrate.itemno,max(ws.unitpricehp) as unitprice
  
 select #hitrate.itemno,max(ws.unitpricehp) as unitprice, #hitrate.ItemID								--IP - 14/07/11 - CR1254 - RI              
 into #MaxWarrantableSales        
 from stockitem ws,warrantyband wb,stockitem s,#hitrate        
  --where wb.waritemno = ws.itemno AND  wb.refcode = #hitrate.refcode and ws.stocklocn = #hitrate.stocklocn 
 where wb.ItemID = ws.ID AND  wb.refcode = #hitrate.refcode and ws.stocklocn = #hitrate.stocklocn		--IP - 14/07/11 - CR1254 - RI        
 --AND s.itemno=#hitrate.itemno AND s.stocklocn=#hitrate.stocklocn  
 AND s.ID=#hitrate.ItemID AND s.stocklocn=#hitrate.stocklocn											--IP - 14/07/11 - CR1254 - RI          
 AND s.unitpricecash BETWEEN wb.minprice AND wb.maxprice        
 group by #hitrate.itemno        
         
 update #hitrate         
 set MaxWarrantableSales=quantity * unitprice        
 from #MaxWarrantableSales        
 --where #hitrate.itemno=#MaxWarrantableSales.itemno  
 where #hitrate.ItemID=#MaxWarrantableSales.ItemID														--IP - 14/07/11 - CR1254 - RI          
         
 -- end of replaced code        
         
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
         
        
         
    if @warrantytype ='LC'        
    begin      
           
     --PRINT 'HERE 1'        
     declare @TargetSalespercent float        
    select @TargetSalespercent= convert(float,value) from countrymaintenance where name ='Target Warranty Hit Rate %'       
          
        
        
     update #hitrate set lostunits =NoofWarrantySales-(Maxwarrantablesales*ROUND( CONVERT(VARCHAR,@TargetSalespercent) /100.0,2)),         
     lostsalesvalue =actualwarrantysales  -(Maxwarrantablesales*ROUND(CONVERT(VARCHAR,@TargetSalespercent) /100.0,2))      
            
     --update #hitrate set LostCommission = lostsalesvalue  * ISNULL(percentage,0)/100.00        
     --from SalesCommissionRatesAudit s where  #hitrate.warrantyno like s.itemtext + '%'         
     --and #hitrate.datedel between s.datefrom and s.dateto and commissiontype NOT IN ('SP','PC')      
     --AND s.Percentage >0 AND s.ActionType !='D'   
     
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
        AND((select category from stockinfo where ID = #hitrate.WarrantyID) in (select code from code where category = 'WAR'))	--IP - 14/07/11 - CR1254 - RI        
        AND s.Percentage >0 AND s.ActionType !='D'        
            
            
    --SELECT * FROM #hitrate        
    --SELECT * FROM SalesCommissionRatesAudit        
                
 end        
 else -- for hit rate getting totals in new table        
 begin        
         
    select  CONVERT(VARCHAR(20),'') AS Total,     -- jec 70817 06/02/09        
    accttype,branchno,BranchName,Cast(category as varchar(50)) as category,  -- jec 70817 06/02/09        
    empeeno,salesperson ,sum(TotalSales) as TotalSales,sum(NoofWarrantySales) as NoofWarrantySales,        
    sum(WarrantyCost) as WarrantyCost,sum(MaxWarrantableSales) as MaxWarrantableSales,        
    sum(ActualWarrantySales) as ActualWarrantySales ,        
    sum(NoofWarrantableSales) as NoofWarrantableSales,HitRate        
    into #hrtotals        
    from  #hitrate        
    group by  accttype,branchno,BranchName,category,empeeno,salesperson ,HitRate        
           
    update #hrtotals set hitrate = convert(float,NoofWarrantySales) / convert(float,NoofWarrantableSales) * 100.0000         
           where NoofWarrantableSales >0        
         
 end        
         
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
  WarrantyNo varchar(18),Warrantydescription varchar(65),warrantyprice MONEY,Branchname varchar(65),			--IP - 14/07/11 - CR1254 - RI      
   --accttype char(1), branchno smallint, category varchar(32), salesperson varchar(99)) -- these    
  accttype char(1), branchno smallint, category varchar(32), salesperson varchar(99), WarrantyID int) -- these	--IP - 14/07/11 - CR1254 - RI	         
                   
         
  create table #hr(TotalSales money, NoofWarrantySales int ,        
    WarrantyCost money, MaxWarrantableSales money ,ActualWarrantySales money,        
  NoofWarrantableSales int, HitRate float,         
   accttype char(1), branchno smallint, category varchar(32), salesperson varchar(99) )        
 if @warrantytype ='LC' -- missed sales         
  set @salespersontotal = 1        
 if @accttypetotal =1 or @branchtotal =1 or @categorytotal = 1 or @salespersontotal =1        
 begin        
         
  if @warrantytype = 'LC'-- missed sales          
  begin        
      set @statement = ' insert into #missedtotals  ' +        
   ' select sum(TotalSales)  , sum(NoofWarrantySales) , ' +        
   ' sum(WarrantyCost)   , '  +        
   ' sum(MaxWarrantableSales) , sum(ActualWarrantySales)  ,' +        
   '  sum(NoofWarrantableSales)  ,' +        
   ' sum(HitRate), ' +        
   ' sum(lostsalesvalue) , sum(lostunits)  , sum(LostCommission),warrantyno,Warrantydescription,0,'''' '          
   end        
 else -- hitrate         
   set @statement = ' insert into #hr ' +        
    ' select sum(TotalSales)  , sum(NoofWarrantySales) , ' +        
    ' sum(WarrantyCost)   , '  +        
    ' sum(MaxWarrantableSales) , sum(ActualWarrantySales)  ,' +        
    '  sum(NoofWarrantableSales)  ,' +        
    ' sum(HitRate) '  -- jec 70817 06/02/09  ,warrantyno        
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
 
 --IP - 14/07/11 - CR1254 - RI
 if(@warrantytype = 'LC')
 begin
	
	set @statement = @statement + ' ,WarrantyID '
	
 end
         
   if @warrantytype in ('MS','LC') -- missed sales         
    SET @groupby2=@groupby         
   set @groupby =  @groupby +  @comma + ' Warrantyno,Warrantydescription '        
     
   set @statement = @statement + '  from #hitrate group by ' + @groupby        
   print @statement        
   exec sp_executesql @statement        
 end        
         
 if @warrantytype in ('MS','LC') -- missed sales        
 begin        
 -- reinserting totals into main table         
   update #missedtotals set hitrate = convert(float,NoofWarrantySales)/convert(float, NoofWarrantableSales) * 100 where NoofWarrantableSales >0        
          
   update #missedtotals set lostunits = NoofWarrantySales -( NoofWarrantableSales* @TargetSalespercent/100.0)           
   update #missedtotals set lostunits = 0 where lostunits >0        
   update #missedtotals set lostcommission = 0 where lostcommission>0        
   --UPDATE #missedtotals SET Lostcommission = Lostcommission * lostunits     
    --delete from #missedtotals where maxwarrantablesales=0 
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
                 
      set @statement = @statement + ' Warrantyno as Warranty,Warrantydescription as Description, ' + @nl +        
           ' maxwarrantablesales as ''Max Warranty Sales Value'', ' + @nl +         
           ' ActualWarrantySales as ''Actual Warranty Sales Value'', ' + @NL +        
        ' NoofWarrantySales as ''Warranties Sold'', ' + @NL +        
           ' NoofWarrantableSales as ''Max No of Warrantable Sales'',  ' +        
           ' HitRate as ''Attachment %'',  ' + @NL +        
      'convert(money,  round('+CONVERT(VARCHAR, @TargetSalespercent)+' /100.00* maxwarrantablesales, 2)) as ''Required Sales Value'', '  + @nl +        
        + 'Case when ActualWarrantySales -ROUND(' + CONVERT(VARCHAR,@TargetSalespercent) + '.0/100.0* maxwarrantablesales,2)>=0 then 0 else ActualWarrantySales -convert(money,  round('+CONVERT(VARCHAR, @TargetSalespercent)+  
        ' /100.00* maxwarrantablesales, 2))  end as ''Lost Sales     
     Value'', '  + @nl +         
        ' LostUnits as ''Lost Units'', ' +         
           ' LostCommission as ''Lost Commission'' '  + @NL + -- jec 70816        
      ' FROM #missedtotals where maxwarrantablesales != 0'+        
      ' union all ' +              
     @statement + ' '''',''Total'' , Sum(maxwarrantablesales),       
     Sum(ActualWarrantySales),       
     Sum(noofWarrantySales),      
     sum(NoofWarrantableSales),      
     convert(float, Sum(noofWarrantySales))/convert(float, sum(NoofWarrantableSales))*100,' +@nl+        
     'sum( convert(money,  round('+CONVERT(VARCHAR, @TargetSalespercent)+' /100.00* maxwarrantablesales, 2)) ) as ''Required Sales Value'',' + @nl +      
     '      
     Sum(Case when ActualWarrantySales -ROUND(' + CONVERT(VARCHAR,@TargetSalespercent) + '.0/100.0* maxwarrantablesales,2)>=0 then 0 else ActualWarrantySales -convert(money,  round('+CONVERT(VARCHAR, @TargetSalespercent)+' /100.00* maxwarrantablesales, 2)
)    
     end ) as ''Lost Sales Value'','    
     +@nl+         
     'sum(lostUnits),       
     sum(LostCommission) '+        
      ' FROM #missedtotals where maxwarrantablesales != 0 '+ 'GROUP BY'  + @groupby2  +         
      ' HAVING sum(NoofWarrantableSales) > 0 ORDER BY ' + @groupby        
  -- PRINT @STATEMENT        
           
   exec sp_executesql @statement        
 end        
 else -- hit rate        
 begin        
    declare @totalcolumn varchar(22)        
    set @totalcolumn=''        
   if @branchtotal !=1        
   BEGIN        
    set @groupby = @groupby + @comma + ' BranchName'        
    set @orderby = @orderby + @comma + ' BranchName'        
   END        
   IF @branchtotal=1 OR @categorytotal=1 OR @salespersontotal=1 OR @accttypetotal=1        
   BEGIN        
          
  -- Branch Totals   -- jec 09/02/09  70817        
     INSERT INTO #hrtotals (total,accttype,branchno,category,salesperson,        
            warrantycost,maxwarrantablesales,actualwarrantysales, hitrate,        
             TotalSales,NoofWarrantySales,NoofWarrantableSales)        
       SELECT case        
     when @branchtotal=1 then 'Branch'        
     end ,'',branchno,'' ,'',          
            sum(warrantycost),sum(maxwarrantablesales),sum(actualwarrantysales), sum(hitrate),        
             sum(TotalSales),sum(NoofWarrantySales),sum(NoofWarrantableSales)        
       FROM #hr        
       where branchno is not null        
       group by branchno        
       order by branchno        
      -- Category Totals  -- jec 09/02/09  70817        
       INSERT INTO #hrtotals (total,accttype,branchno,category,salesperson,        
            warrantycost,maxwarrantablesales,actualwarrantysales, hitrate,        
             TotalSales,NoofWarrantySales,NoofWarrantableSales)        
       SELECT case            
      when @categorytotal=1 then ' Category'        
     end ,'','', category ,'',          
            sum(warrantycost),sum(maxwarrantablesales),sum(actualwarrantysales), sum(hitrate),        
             sum(TotalSales),sum(NoofWarrantySales),sum(NoofWarrantableSales)        
       FROM #hr        
       where category is not null        
       group by category        
       order by category         
       -- Salesperson Totals  -- jec 09/02/09  70817        
       INSERT INTO #hrtotals (total,accttype,branchno,category,salesperson,        
            warrantycost,maxwarrantablesales,actualwarrantysales, hitrate,        
             TotalSales,NoofWarrantySales,NoofWarrantableSales)        
       SELECT case           
      when  @salespersontotal=1 then ' SalesPerson'        
     end ,'','','',salesperson,          
            sum(warrantycost),sum(maxwarrantablesales),sum(actualwarrantysales), sum(hitrate),        
             sum(TotalSales),sum(NoofWarrantySales),sum(NoofWarrantableSales)        
       FROM #hr        
       where salesperson !='Total'        
       group by salesperson        
       order by salesperson        
       -- AccountType Totals  -- jec 09/02/09  70817        
       INSERT INTO #hrtotals (total,accttype,branchno,category,salesperson,        
            warrantycost,maxwarrantablesales,actualwarrantysales, hitrate,        
             TotalSales,NoofWarrantySales,NoofWarrantableSales)        
       SELECT case        
      when  @accttypetotal=1 then ' AcctType'        
     end ,accttype,'','','',          
        sum(warrantycost),sum(maxwarrantablesales),sum(actualwarrantysales), sum(hitrate),        
           sum(TotalSales),sum(NoofWarrantySales),sum(NoofWarrantableSales)        
       FROM #hr        
       where accttype !=' '        
       group by accttype        
       order by accttype        
       set @totalcolumn='Total,'        
       SET @groupby = @groupby + ',Total'        
       --SET @orderby = @orderby + ',Total '        
       -- jec 09/02/09 70817  order by groupings        
       SET @orderby = 'Total'        
       if @accttypetotal=1 set @orderby =@orderby + ',accttype'           
       if @categorytotal=1 set @orderby =@orderby + ',category'        
       if @salespersontotal=1 set @orderby =@orderby + ',salesperson'        
       --IP - 25/02/09 - (70817) Grouping by 'Branch' from 'Reports' screen was causing error, as columns 'branchno,branchname' were specified twice        
       --if @branchtotal=1 set @orderby =@orderby + ',branchno,branchname'         
               
       if @accttypetotal=1 and @branchtotal=1 set @orderby = 'accttype,total'           
       if @categorytotal=1 and @branchtotal=1 set @orderby ='category,total'        
       if @salespersontotal=1 and @branchtotal=1 set @orderby ='salesperson,total'        
       if @branchtotal=1 set @orderby =@orderby + ',branchno,branchname'        
       --SET @orderby = @orderby + ',Total '        
     END        
             
     update #hrtotals set branchname = b.branchname from branch b where b.branchno =  #hrtotals.branchno        
          
     update #hrtotals set hitrate =  convert(float,NoofWarrantySales) / convert(float,NoofWarrantableSales) * 100.0000         
          where NoofWarrantableSales >0        
                                          
     select @statement = 'SELECT ' + @Totalcolumn + '   case  when accttype = ''S'' then ''Cash & Go'' when accttype = ''C'' then ''Cash'' when accttype = ''R'' then ''Ready Finance'' when accttype = ''O'' then ''Hire Purchase'' end  as ''Account Type'', 
    
      
       
      case when branchno =0 then cast('' '' as char(3)) else cast(branchno as char(3)) end as ''Branch Number'',BranchName as ''Branch Name'', ' +        
           ' category as ''Product Category'', ' +        
           ' salesperson as ''Sales Person'',    TotalSales as ''Total Sales'', NoofWarrantySales as ''No Of Warranty Sales'', ' +        
           ' WarrantyCost as ''Warranty Cost'',ActualWarrantySales as ''Actual Warranty Sales'',  ' +         
        ' NoofWarrantableSales as ''Max No of Warrantable Sales'', MaxWarrantableSales as ''Max Value of Warrantable Sales'',HitRate' +        
           ' FROM #hrtotals ' +         
     'ORDER BY ' + @orderby        
             
   --PRINT @statement        
   exec sp_executesql @statement    
      
   --set @groupby = @groupby + @comma + ' acctno'        
            
 END        
END  
--====================================================  
--END OF LOST SALES  
   
--START OF HIT RATE  
ELSE  
BEGIN  
  
  
  
--drop table #hr_hitrate     
create table #hr_hitrate (  
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
 ValueWarrantyCancel decimal(12,2),        
 ValueWarrantyRepossess decimal(12,2),  
 NoofWarrantySales integer,  
 NoofWarrantableSales integer,    
 NoOfWarrantyCancel integer,  
 NoOfWarrantyRepossess integer,   
 WarrantyCommissions decimal(12,2),     
 HitRate decimal(12,2),  
 --itemno varchar (10), 
 itemno varchar (18),																		--IP - 14/07/11 - CR1254 - RI  
 stocklocn smallint,
 retstocklocn smallint,  
 acctno char(12)not null,        
 refcode varchar(10),  
 delorcoll char(1),  
 quantity int,    
 agrmtno int not null,
 ItemID int																					--IP - 14/07/11 - CR1254 - RI
 )        
        
create clustered index ix_gdsdrtre on #hr_hitrate (acctno,agrmtno)        
        
set @select = ' insert into #hr_hitrate         
               (branchno, branchname, category, empeeno, acctno, salesperson, totalsales, warrantycost, '  
                + ' MaxWarrantableSales, actualwarrantysales, noofwarrantysales, noofwarrantablesales , '       
    + ' hitrate,itemno,stocklocn,retstocklocn,refcode,quantity,agrmtno, delorcoll, NoOfWarrantyRepossess, '  
    --+ ' NoOfWarrantyCancel, ValueWarrantyCancel, ValueWarrantyRepossess, warrantycommissions) '   
    + ' NoOfWarrantyCancel, ValueWarrantyCancel, ValueWarrantyRepossess, warrantycommissions, ItemID) '		--IP - 14/07/11 - CR1254 - RI 
    +  @nl        
    + ' select convert(smallint,left(g.acctno,3)),'''',convert(varchar,s.category), g.empeenosale,g.acctno, '  
    + @nl   
    +'  '''',sum(ds.transvalue),0,  '   
    + @nl   
    --+' 0,0,0,0, 0,ds.itemno,ds.stocklocn,ds.retstocklocn,s.refcode,sum(ds.quantity), ds.agrmtno, ds.delorcoll, 0, 0, 0, 0,0 '  
    +' 0,0,0,0, 0,s.iupc as itemno,ds.stocklocn,ds.retstocklocn,s.refcode,sum(ds.quantity), ds.agrmtno, ds.delorcoll, 0, 0, 0, 0,0, s.ID '		--IP - 14/07/11 - CR1254 - RI
             
set @from =' from delivery ds '        
--set @from = @from + ' join stockitem s on ds.itemno =s.itemno and ds.stocklocn =s.stocklocn '  
set @from = @from + ' join stockitem s on ds.ItemID =s.ID and ds.stocklocn =s.stocklocn '   
      + @nl         
set @from = @from +  ' join agreement g on g.acctno = ds.acctno and g.agrmtno =ds.agrmtno '         
                        
set @where= ' where ds.datedel between '   
   + ''''   
   + convert (varchar,@datefrom)   
   + ''''   
   + ' and '   
   + ''''   
   +  convert (varchar,@dateTO)   
   + ''''   
   + ' and s.itemtype !=''N'' '     
        
       
        
if @salesperson !='0' and @salesperson !='ALL'        
   set @where = @where + ' and g.empeenosale = ' + @salesperson        
                                   
  --declare @acctlike varchar(6),@not varchar(5)        
  set @acctlike =''         
  SET @not =''   
  -- NOTES FROM VERSION 1.0       
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
    
  set @where = @where + ' and ( ds.delorcoll = ''D'' '  
    
  if @includeCanc = 1  
  begin  
 set @where = @where + ' or ds.delorcoll = ''C'''  
  end  
    
  if @includeRep = 1  
  begin  
 set @where = @where + ' or ds.delorcoll = ''R'''  
  end  
    
    
  set @where = @where + ' )'  
       
--PRINT @acctlike        
--PRINT @where        
  
  if @categoryset !='' and @categoryset !='ALL'        
  begin        
    set @where = @where + ' and s.category in ( '         
    
  --declare @counter int,  
  --declare @setdata varchar(36)        
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
          
   set @where = @where + ')'  + @nl       
  end       
      
--set @groupby = ' group by g.empeenosale,g.acctno, ds.itemno,ds.stocklocn,ds.retstocklocn,s.refcode,ds.datedel,ds.buffno, ds.agrmtno, ds.delorcoll, s.category'     
set @groupby = ' group by g.empeenosale,g.acctno, s.iupc,ds.stocklocn,ds.retstocklocn,s.refcode,ds.datedel,ds.buffno, ds.agrmtno, ds.delorcoll, s.category, s.ID'			--IP - 14/07/11 - CR1254 - RI      
--declare @statement sqltext        
set @statement = @select + @from + @where  +@groupby       
print @statement        
exec sp_executesql @statement   
  
--update number of warrantable sales  
update #hr_hitrate set NoofWarrantableSales = quantity   
from #hr_hitrate   
where exists   
(        
 select 1 from stockitem s   
 join warrantyband wb on wb.refcode =s.refcode and (#hr_hitrate.totalsales/#hr_hitrate.quantity) between wb.minprice and wb.maxprice        
 --where #hr_hitrate.itemno=s.itemno and #hr_hitrate.stocklocn=s.stocklocn   and #hr_hitrate.quantity != 0
 where #hr_hitrate.ItemID=s.ID and #hr_hitrate.stocklocn=s.stocklocn   and #hr_hitrate.quantity != 0		--IP - 14/07/11 - CR1254 - RI
)
and (((#hr_hitrate.delorcoll = 'D' and (retstocklocn = 0 or retstocklocn = '' or retstocklocn is null))
or #hr_hitrate.delorcoll != 'D') or (@includeCanc =1 and retstocklocn > 0) ) 

  
-- Get the name of the sales person from courtsperson table         
update #hr_hitrate   
set salesperson = fullname   
from Admin.[User] c   
where #hr_hitrate.empeeno = c.Id    
  
--Update the acct type from account table     
update #hr_hitrate set accttype = a.accttype from acct a where a.acctno = #hr_hitrate.acctno        
  
--have to select disctinct then group  
--drop table #hr_allwarranties  
  
create table #hr_allwarranties(  
acctno char(12),  
agrmtno int,  
branchno smallint,  
contractno varchar(10),  
delorcoll char(1),  
--witemno varchar(8),  
witemno varchar(18),													--IP - 14/07/11 - CR1254 - RI												 
quantity float,  
stocklocn smallint,  
transvalue money,  
--itemno varchar(8),  
itemno varchar(18),														--IP - 14/07/11 - CR1254 - RI
costprice money,  
commission money,
ItemID int,																--IP - 14/07/11 - CR1254 - RI  
WItemID int																--IP - 14/07/11 - CR1254 - RI
)  
  
  
--set @statement =   
--'  
--insert INTO #hr_allwarranties  
--SELECT distinct d.acctno, d.agrmtno, d.branchno, d.contractno,  
--d.delorcoll, d.itemno as witemno,   
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
-- and (c.CommissionAmount/ABS((CASE c.commissionamount
--						WHEN 0 THEN	1
--						ELSE
--							c.commissionamount
--						END
--						))) = (d.quantity/ABS((CASE d.quantity
--						WHEN 0 THEN	1
--						ELSE
--							d.quantity
--						END
--						))) 
-- and c.agrmtno = d.agrmtno  
--inner join stockitem s   
-- on s.itemno = l.itemno  
-- and s.stocklocn = l.stocklocn  
--INNER JOIN #hr_hitrate h  
-- ON h.acctno = d.acctno  
-- AND h.itemno = d.parentitemno  
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
        
--and (d.delorcoll = ''D'''  

set @statement =   
'  
insert INTO #hr_allwarranties  
SELECT distinct d.acctno, d.agrmtno, d.branchno, d.contractno,  
d.delorcoll, s.iupc as witemno,   
d.quantity , d.stocklocn, d.transvalue, h.itemno, (s.costprice * d.quantity),  
isnull(c.commissionamount, 0) as commission, h.ItemID, d.ItemID as WItemID
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
 and (c.CommissionAmount/ABS((CASE c.commissionamount
						WHEN 0 THEN	1
						ELSE
							c.commissionamount
						END
						))) = (d.quantity/ABS((CASE d.quantity
						WHEN 0 THEN	1
						ELSE
							d.quantity
						END
						))) 
 and c.agrmtno = d.agrmtno  
inner join stockitem s   
 on s.ID = l.ItemID  
 and s.stocklocn = l.stocklocn  
INNER JOIN #hr_hitrate h  
 ON h.acctno = d.acctno  
 AND h.ItemID = d.ParentItemID  
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
        
and (d.delorcoll = ''D'''  
  
  
if @includeCanc = 1  
begin  
 set @statement = @statement + ' or d.delorcoll = ''C'' '  
end  
  
if @includeRep = 1  
begin   
 set @statement = @statement + ' or d.delorcoll = ''R'' '  
end  
  
--set @statement = @statement + ' ) group by d.acctno, d.agrmtno, d.branchno, d.contractno,  
--d.delorcoll, d.itemno, d.quantity , c.commissionamount, d.stocklocn, d.transvalue, h.itemno, s.costprice'  

--IP - 14/07/11 - CR1254 - RI - Replaces the above
set @statement = @statement + ' ) group by d.acctno, d.agrmtno, d.branchno, d.contractno,  
d.delorcoll, s.iupc, d.quantity , c.commissionamount, d.stocklocn, d.transvalue, h.itemno, s.costprice, h.ItemID, d.ItemID'  


  
  print @statement
exec sp_executesql @statement  

delete from #hr_allwarranties 
where (#hr_allwarranties.commission < 0
		and #hr_allwarranties.quantity > 0)
OR  (#hr_allwarranties.commission > 0
		and #hr_allwarranties.quantity < 0)

--drop table #hr_warrantyRepValue  
  
create table #hr_warrantyrepvalue(  
acctno char(12),  
total money,  
count int,  
amountpw money)  
  
insert into #hr_warrantyrepvalue  
select f.acctno, sum(f.transvalue) as total, count(*) as [count], 0.0 as amountpw  
from fintrans f  
where f.transtypecode = 'CRE'  
or f.transtypecode = 'CRF'  
group by f.acctno  
  
update #hr_warrantyrepvalue  
set amountpw = total/[count]  
where [count] != 0
  
update #hr_allwarranties  
set transvalue = f.amountpw  
from #hr_warrantyrepvalue f  
where #hr_allwarranties.acctno = f.acctno  
and #hr_allwarranties.transvalue = 0  
  
--drop table #hr_warranties  
  
select acctno, agrmtno, branchno, contractno,  
delorcoll, witemno, sum(quantity) as quantity ,   
stocklocn, sum(transvalue) as transvalue, itemno, sum(costprice) as costprice,  
--sum(commission) as commission	 
sum(commission) as commission, ItemID, WItemID						--IP - 14/07/11 - CR1254 - RI	 
into #hr_warranties  
from #hr_allwarranties  
--GROUP BY acctno, agrmtno, branchno, contractno,  
GROUP BY acctno, agrmtno, branchno, contractno,  ItemID, WItemID,	--IP - 19/07/11 - CR1254 - RI	
delorcoll, witemno, stocklocn, itemno  
  
--drop table #hr_warrantytotals  
  
--get total value and qty of warranties sold on an item  
select h.acctno, h.itemno, h.stocklocn, sum(w.transvalue) as totalValue,   
 sum(w.quantity) as totalQuantity, sum(costprice) as totalCost,  
 --sum(commission) as commission, 0 as delExists 
sum(commission) as commission, 0 as delExists, h.ItemID		--IP - 14/07/11 - CR1254 - RI 
into #hr_warrantyTotals  
from #hr_warranties w, #hr_hitrate h  
where h.acctno = w.acctno  
--AND h.itemno = w.itemno  
and h.ItemID = w.ItemID										--IP - 14/07/11 - CR1254 - RI  
and h.stocklocn = w.stocklocn  
and h.delorcoll = w.delorcoll
and ((h.delorcoll = 'D' and (retstocklocn = 0 or retstocklocn = '' or retstocklocn is null))
or h.delorcoll != 'D')
--group by h.acctno, h.itemno, h.stocklocn
group by h.acctno, h.itemno, h.stocklocn, h.ItemID			--IP - 19/07/11 - CR1254 - RI

update #hr_warrantyTotals
set delExists = 1
from #hr_hitrate h
where  h.acctno = #hr_warrantyTotals.acctno  
--AND h.itemno = #hr_warrantyTotals.itemno 
AND h.ItemID = #hr_warrantyTotals.ItemID					--IP - 14/07/11 - CR1254 - RI   
and h.stocklocn = #hr_warrantyTotals.stocklocn
and h.delorcoll = 'D'
    
update #hr_hitrate   
set ActualWarrantySales = t.totalValue,  
NoofWarrantySales = t.totalQuantity,  
warrantyCost = t.totalCost,  
warrantycommissions = t.commission  
from #hr_warrantytotals t  
where #hr_hitrate.acctno = t.acctno  
--AND #hr_hitrate.itemno = t.itemno  
AND #hr_hitrate.ItemID = t.ItemID							--IP - 14/07/11 - CR1254 - RI  
and #hr_hitrate.stocklocn = t.stocklocn 
and ((#hr_hitrate.delorcoll = 'D' and t.delExists = 1) or (t.delExists = 0))
and ((#hr_hitrate.delorcoll = 'D' and (retstocklocn = 0 or retstocklocn = '' or retstocklocn is null))
or #hr_hitrate.delorcoll != 'D')
  
--calculate warranty cancellations  
  
--drop table #hr_warrantyCancel  
  
select h.acctno, h.itemno, h.stocklocn, sum(w.transvalue) as totalValue, 
--sum(w.quantity) as totalQuantity, 0 as delExists  
sum(w.quantity) as totalQuantity, 0 as delExists, h.ItemID	--IP - 14/07/11 - CR1254 - RI
into #hr_warrantyCancel  
from #hr_warranties w, #hr_hitrate h  
where h.acctno = w.acctno  
--AND h.itemno = w.itemno  
AND h.ItemID = w.ItemID										--IP - 14/07/11 - CR1254 - RI  
and h.stocklocn = w.stocklocn  
and w.delorcoll = 'C'  
and h.delorcoll = w.delorcoll
--group by h.acctno, h.itemno, h.stocklocn  
group by h.acctno, h.itemno, h.stocklocn, h.ItemID			--IP - 19/07/11 - CR1254 - RI

update #hr_warrantyCancel
set delExists = 1 from 
#hr_hitrate h  
where h.acctno = #hr_warrantyCancel.acctno  
--AND h.itemno = #hr_warrantyCancel.itemno 
AND h.ItemID = #hr_warrantyCancel.ItemID					--IP - 14/07/11 - CR1254 - RI   
and h.stocklocn = #hr_warrantyCancel.stocklocn
and h.delorcoll = 'D'
  
update #hr_hitrate   
set ValueWarrantyCancel = t.totalValue,  
NoofWarrantyCancel = t.totalQuantity  
from #hr_warrantycancel t  
where #hr_hitrate.acctno = t.acctno  
--AND #hr_hitrate.itemno = t.itemno
AND #hr_hitrate.ItemID = t.ItemID							--IP - 14/07/11 - CR1254 - RI  
and #hr_hitrate.stocklocn = t.stocklocn  
and ((#hr_hitrate.delorcoll = 'D' and t.delExists = 1) or (t.delExists = 0))
  
  
  
--calculate warranty repossessions  
  
--drop table #hr_warrantyRepossess  
  
select h.acctno, h.itemno, h.stocklocn, sum(w.transvalue) as totalValue, 
--sum(w.quantity) as totalQuantity, 0 as delExists
sum(w.quantity) as totalQuantity, 0 as delExists, h.ItemID	--IP - 14/07/11 - CR1254 - RI
into #hr_warrantyRepossess  
from #hr_warranties w, #hr_hitrate h  
where h.acctno = w.acctno  
--AND h.itemno = w.itemno  
and h.ItemID = w.ItemID										--IP - 14/07/11 - CR1254 - RI  
and h.stocklocn = w.stocklocn  
and w.delorcoll = 'R'   
and h.delorcoll = w.delorcoll 
--group by h.acctno, h.itemno, h.stocklocn
group by h.acctno, h.itemno, h.stocklocn, h.ItemID			--IP - 19/07/11 - CR1254 - RI   

update #hr_warrantyRepossess
set delExists = 1 from 
#hr_hitrate h  
where h.acctno = #hr_warrantyRepossess.acctno  
--AND h.itemno = #hr_warrantyRepossess.itemno 
AND h.ItemID = #hr_warrantyRepossess.ItemID					--IP - 19/07/11 - CR1254 - RI   
and h.stocklocn = #hr_warrantyRepossess.stocklocn
and h.delorcoll = 'D'
  
  
update #hr_hitrate   
set ValueWarrantyRepossess = t.totalValue,  
NoOfWarrantyRepossess = t.totalQuantity  
from #hr_warrantyRepossess t  
where #hr_hitrate.acctno = t.acctno  
--AND #hr_hitrate.itemno = t.itemno 
AND #hr_hitrate.ItemID = t.ItemID							--IP - 19/07/11 - CR1254 - RI   
and #hr_hitrate.stocklocn = t.stocklocn  
and ((#hr_hitrate.delorcoll = 'D' and t.delExists = 1) or (t.delExists = 0))
  
--calculate warranty commission  
  
-- update  Warrantable Sales     
  
--drop table #hr_maxwarrantablesales  
     
 select #hr_hitrate.itemno,max(ws.unitpricehp)  as unitprice,
 #hr_hitrate.delorcoll, #hr_hitrate.acctno,
 #hr_hitrate.ItemID											--IP - 19/07/11 - CR1254 - RI
 into #hr_MaxWarrantableSales        
 from stockitem ws,warrantyband wb,stockitem s,#hr_hitrate        
  --where wb.waritemno = ws.itemno AND  wb.refcode = #hr_hitrate.refcode and ws.stocklocn = #hr_hitrate.stocklocn    
 where wb.ItemID = ws.ID AND  wb.refcode = #hr_hitrate.refcode and ws.stocklocn = #hr_hitrate.stocklocn	--IP - 19/07/11 - CR1254 - RI       
 --AND s.itemno=#hr_hitrate.itemno AND s.stocklocn=#hr_hitrate.stocklocn  
 AND s.ID=#hr_hitrate.ItemID AND s.stocklocn=#hr_hitrate.stocklocn										--IP - 19/07/11 - CR1254 - RI										         
 AND s.unitpricecash BETWEEN wb.minprice AND wb.maxprice        
 --group by #hr_hitrate.itemno,  #hr_hitrate.delorcoll, #hr_hitrate.acctno    
 group by #hr_hitrate.itemno,  #hr_hitrate.delorcoll, #hr_hitrate.acctno, #hr_hitrate.ItemID			--IP - 19/07/11 - CR1254 - RI         
         
          
 update #hr_hitrate         
 set MaxWarrantableSales=#hr_hitrate.quantity * #hr_MaxWarrantableSales.unitprice        
 from #hr_MaxWarrantableSales        
 --where #hr_hitrate.itemno=#hr_MaxWarrantableSales.itemno
  where #hr_hitrate.ItemID=#hr_MaxWarrantableSales.ItemID												--IP - 19/07/11 - CR1254 - RI
  and   #hr_hitrate.acctno=#hr_MaxWarrantableSales.acctno
  and #hr_hitrate.delorcoll=#hr_MaxWarrantableSales.delorcoll        
         
 -- this is for price falls        
 update #hr_hitrate set maxwarrantablesales =actualwarrantysales where actualwarrantysales>maxwarrantablesales    
 and MaxWarrantableSales > 0
  
-- this is to update the value to be equal where there have been price changes as the above code only stores current prices        
 update #hr_hitrate set maxwarrantablesales =actualwarrantysales where NoofWarrantySales=NoofWarrantableSales        
      
  --calculate hitrate      
 update #hr_hitrate set HitRate = convert(float,NoofWarrantySales)/convert(float, NoofWarrantableSales) * 100  
 where NoofWarrantableSales >0  
         
 --update category description        
 update #hr_hitrate  set category =LEFT((code + '-' + codedescript),20)         
 from code        
 where code.code = #hr_hitrate.category and         
 code.category in ('PCE','PCF','PCW')    
  
   
 --drop table #hr_hrtotals      
   
   
 --create totals tables  
create table #hr_hrtotals(
  total varchar(20),accttype varchar(1), branchno smallint,BranchName varchar (30),         
  category varchar(50),empeeno integer,salesperson varchar(99),TotalSales decimal(12,2),        
  NoofWarrantySales integer, WarrantyCost  decimal(12,2),MaxWarrantableSales decimal(12,2),        
  ActualWarrantySales decimal(12,2),NoofWarrantableSales integer,        
  NoOfWarrantyRepossess integer,NoOfWarrantyCancel integer,
  ValueWarrantyCancel decimal(12,2),        
 ValueWarrantyRepossess decimal(12,2),warrantyCommissions decimal(12,2),        
  HitRate decimal(12,2)
  )

set @statement =   
 'insert into #hr_hrtotals select  CONVERT(VARCHAR(20),'''') AS Total,     -- jec 70817 06/02/09        
  accttype,branchno,BranchName,Cast(category as varchar(50)) as category,  -- jec 70817 06/02/09        
  empeeno,salesperson ,sum(TotalSales) as TotalSales,sum(NoofWarrantySales) as NoofWarrantySales,        
  sum(WarrantyCost) as WarrantyCost,sum(MaxWarrantableSales) as MaxWarrantableSales,        
  sum(ActualWarrantySales) as ActualWarrantySales ,        
  sum(NoofWarrantableSales) as NoofWarrantableSales,  
  sum(NoOfWarrantyRepossess) as NoOfWarrantyRepossess,   
  sum(NoOfWarrantyCancel) AS NoOfWarrantyCancel,   
  sum(ValueWarrantyCancel) as ValueWarrantyCancel,   
  sum(ValueWarrantyRepossess) as ValueWarrantyRepossess,   
  sum(warrantycommissions) as warrantycommissions,    
 sum(HitRate) as hitrate
  from  #hr_hitrate    
  where (#hr_hitrate.delorcoll = ''D'' '

if @includeRep = 1  
begin   
 set @statement = @statement + ' or #hr_hitrate.delorcoll = ''R'' '  
end
  
if @includeCanc = 1  
begin  
 set @statement = @statement + ' or #hr_hitrate.delorcoll = ''C'' )'  
end
else
begin
--set @statement = @statement + ' )'
	set @statement = @statement + ' ) and (#hr_hitrate.delorcoll != ''D'' or 
	(#hr_hitrate.delorcoll = ''D'' and (retstocklocn = 0 or retstocklocn = '''' or retstocklocn is null))) '
end  
   
set @statement = @statement + ' group by  accttype,branchno,BranchName,category,empeeno,salesperson'  
  
exec sp_executesql @statement   
         
  update #hr_hrtotals set hitrate = convert(float,NoofWarrantySales) / convert(float,NoofWarrantableSales) * 100.0000         
         where NoofWarrantableSales >0        
           
         
 --declare  @comma varchar(9), @finalorder sqltext, @orderby varchar(222)        
   set @groupby =' '        
   set @groupby2 =' '        
   set @comma = ' '        
   SET @orderby =' '        
         
 -- here we are getting totals.         
 -- if we are to have totals within the body of the results then we will need to insert these back into the         
 -- #hr_hitrate table. Then we would need to retrieve in that order.        
 -- for instan     
 --drop table #hr_missedtotals  
      
   create table #hr_missedtotals  (TotalSales money,NoofWarrantySales integer,        
  WarrantyCost  money,MaxWarrantableSales money,ActualWarrantySales money,        
  NoofWarrantableSales integer,HitRate float,        
  lostsalesvalue money, lostunits float, LostCommission money,        
  --WarrantyNo varchar(10),Warrantydescription varchar(65),warrantyprice MONEY,Branchname varchar(65),   
   WarrantyNo varchar(18),Warrantydescription varchar(65),warrantyprice MONEY,Branchname varchar(65),	--IP - 19/07/11 - CR1254 - RI         
   accttype char(1), branchno smallint, category varchar(32), salesperson varchar(99)) -- these         
                   
         
  --drop table #hr_HR  
         
  create table #hr_hr(TotalSales money, NoofWarrantySales int ,        
    WarrantyCost money, MaxWarrantableSales money ,ActualWarrantySales money,        
  NoofWarrantableSales int, ValueWarrantyCancel decimal(12,2),        
 ValueWarrantyRepossess decimal(12,2),   
 NoOfWarrantyCancel integer,  
 NoOfWarrantyRepossess integer,   
 WarrantyCommissions decimal(12,2) ,  HitRate float,         
   accttype char(1), branchno smallint, category varchar(32), empeeno int, salesperson varchar(99) )             
   if @accttypetotal =1 or @branchtotal =1 or @categorytotal = 1 or @salespersontotal =1        
   begin            
   set @statement = ' insert into #hr_hr ' +        
    ' select sum(TotalSales)  , sum(NoofWarrantySales) , ' +        
    ' sum(WarrantyCost)   , '  +        
    ' sum(MaxWarrantableSales) , sum(ActualWarrantySales)  ,' +        
    ' sum(NoofWarrantableSales)  ,' +   
    ' sum(ValueWarrantyCancel), '+  
    ' sum(ValueWarrantyRepossess), '+  
    ' sum(NoOfWarrantyCancel), '+  
    ' sum(NoOfWarrantyRepossess), '+        
    ' sum(warrantycommissions), '+       
    ' sum(HitRate) '  -- jec 70817 06/02/09  ,warrantyno        
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
     set @statement = @statement + ' ,empeeno,salesperson '        
     set @groupby = @groupby +  @comma + ' empeeno,salesperson '        
     set @orderby = @orderby +  @comma + ' empeeno,salesperson '        
     set @comma = ' , '        
  end        
  else        
  begin  
     set @statement = @statement + ','''', convert( varchar(8),''Total'')  '            
    
  end  
    
  set @statement = @statement + '  from #hr_hitrate where (delorcoll = ''D'' '  
    
  if @includeRep = 1  
  begin  
    set @statement = @statement + 'or delorcoll = ''R'' '  
  end 
   
if @includeCanc = 1  
begin  
 set @statement = @statement + ' or #hr_hitrate.delorcoll = ''C'' )'  
end
else
begin
--set @statement = @statement + ' )'
	set @statement = @statement + ' ) and (#hr_hitrate.delorcoll != ''D'' or 
	(#hr_hitrate.delorcoll = ''D'' and (retstocklocn = 0 or retstocklocn = '''' or retstocklocn is null))) '
end

  set @statement = @statement +'group by ' + @groupby        
    --print @statement        
    exec sp_executesql @statement        
    
  --declare @totalcolumn varchar(22)        
    set @totalcolumn=''        
   if @branchtotal !=1        
   BEGIN        
    set @groupby = @groupby + @comma + ' BranchName'        
    set @orderby = @orderby + @comma + ' BranchName'        
   END        
   IF @branchtotal=1 OR @categorytotal=1 OR @salespersontotal=1 OR @accttypetotal=1        
   BEGIN        
          
  -- Branch Totals   -- jec 09/02/09  70817        
     INSERT INTO #hr_hrtotals (total,accttype,branchno,category,salesperson,        
            warrantycost,maxwarrantablesales,actualwarrantysales, hitrate,        
             TotalSales,NoofWarrantySales,NoofWarrantableSales, NoOfWarrantyRepossess,   
        NoOfWarrantyCancel, ValueWarrantyCancel, ValueWarrantyRepossess, warrantycommissions )        
       SELECT case        
     when @branchtotal=1 then 'Branch'        
     end ,'',branchno,'' ,'',          
            sum(warrantycost),sum(maxwarrantablesales),sum(actualwarrantysales), sum(hitrate),        
             sum(TotalSales),sum(NoofWarrantySales),sum(NoofWarrantableSales), sum(NoOfWarrantyRepossess),  
             sum(NoOfWarrantyCancel), sum(ValueWarrantyCancel), sum(ValueWarrantyRepossess), sum(warrantycommissions)        
       FROM #hr_hr        
       where branchno is not null        
       group by branchno        
       order by branchno        
      -- Category Totals  -- jec 09/02/09  70817        
       INSERT INTO #hr_hrtotals (total,accttype,branchno,category,salesperson,        
            warrantycost,maxwarrantablesales,actualwarrantysales, hitrate,        
             TotalSales,NoofWarrantySales,NoofWarrantableSales, NoOfWarrantyRepossess,   
        NoOfWarrantyCancel, ValueWarrantyCancel, ValueWarrantyRepossess, warrantycommissions )        
       SELECT case            
      when @categorytotal=1 then ' Category'        
     end ,'','', category ,'',          
            sum(warrantycost),sum(maxwarrantablesales),sum(actualwarrantysales), sum(hitrate),        
             sum(TotalSales),sum(NoofWarrantySales),sum(NoofWarrantableSales), sum(NoOfWarrantyRepossess),  
             sum(NoOfWarrantyCancel), sum(ValueWarrantyCancel), sum(ValueWarrantyRepossess), sum(warrantycommissions)         
       FROM #hr_hr        
       where category is not null        
       group by category        
       order by category         
       -- Salesperson Totals  -- jec 09/02/09  70817        
       INSERT INTO #hr_hrtotals (total,accttype,branchno,category,empeeno, salesperson,        
            warrantycost,maxwarrantablesales,actualwarrantysales, hitrate,        
             TotalSales,NoofWarrantySales,NoofWarrantableSales, NoOfWarrantyRepossess,   
        NoOfWarrantyCancel, ValueWarrantyCancel, ValueWarrantyRepossess, warrantycommissions )        
       SELECT case           
      when  @salespersontotal=1 then ' SalesPerson'        
     end ,'','','', empeeno,   
     salesperson,          
            sum(warrantycost),sum(maxwarrantablesales),sum(actualwarrantysales), sum(hitrate),        
             sum(TotalSales),sum(NoofWarrantySales),sum(NoofWarrantableSales), sum(NoOfWarrantyRepossess),  
             sum(NoOfWarrantyCancel), sum(ValueWarrantyCancel), sum(ValueWarrantyRepossess), sum(warrantycommissions)         
       FROM #hr_hr        
       where salesperson !='Total'        
       group by empeeno, salesperson        
       order by salesperson        
       -- AccountType Totals  -- jec 09/02/09  70817        
       INSERT INTO #hr_hrtotals (total,accttype,branchno,category,salesperson,        
            warrantycost,maxwarrantablesales,actualwarrantysales, hitrate,        
             TotalSales,NoofWarrantySales,NoofWarrantableSales, NoOfWarrantyRepossess,   
        NoOfWarrantyCancel, ValueWarrantyCancel, ValueWarrantyRepossess, warrantycommissions )        
       SELECT case        
      when  @accttypetotal=1 then ' AcctType'        
     end ,accttype,'','','',          
        sum(warrantycost),sum(maxwarrantablesales),sum(actualwarrantysales), sum(hitrate),        
             sum(TotalSales),sum(NoofWarrantySales),sum(NoofWarrantableSales), sum(NoOfWarrantyRepossess),  
             sum(NoOfWarrantyCancel), sum(ValueWarrantyCancel), sum(ValueWarrantyRepossess), sum(warrantycommissions)        
       FROM #hr_hr        
       where accttype !=' '        
       group by accttype        
       order by accttype        
       set @totalcolumn='Total,'        
       SET @groupby = @groupby + ',Total'        
       --SET @orderby = @orderby + ',Total '        
       -- jec 09/02/09 70817  order by groupings        
       SET @orderby = 'Total'        
       if @accttypetotal=1 set @orderby =@orderby + ',accttype'           
       if @categorytotal=1 set @orderby =@orderby + ',category'        
       if @salespersontotal=1 set @orderby =@orderby + ',salesperson'        
       --IP - 25/02/09 - (70817) Grouping by 'Branch' from 'Reports' screen was causing error, as columns 'branchno,branchname' were specified twice        
       --if @branchtotal=1 set @orderby =@orderby + ',branchno,branchname'         
               
       if @accttypetotal=1 and @branchtotal=1 set @orderby = 'accttype,total'           
       if @categorytotal=1 and @branchtotal=1 set @orderby ='category,total'        
       if @salespersontotal=1 and @branchtotal=1 set @orderby ='salesperson,total'        
       if @branchtotal=1 set @orderby =@orderby + ',branchno,branchname'        
       --SET @orderby = @orderby + ',Total '        
     END        
             
     update #hr_hrtotals set branchname = b.branchname from branch b where b.branchno =  #hr_hrtotals.branchno        
          
     update #hr_hrtotals set hitrate =  convert(float,NoofWarrantySales) / convert(float,NoofWarrantableSales) * 100.0000         
          where NoofWarrantableSales >0        
                                          
     select @statement = 'SELECT ' + @Totalcolumn + '   case  when accttype = ''S'' then ''Cash & Go'' when accttype = ''C'' then ''Cash'' when accttype = ''R'' then ''Ready Finance'' when accttype = ''O'' then ''Hire Purchase'' end  as ''Account Type'',
     
      
       
      case when branchno =0 then cast('' '' as char(3)) else cast(branchno as char(3)) end as ''Branch Number'',BranchName as ''Branch Name'', ' +        
           ' category as ''Product Category'', ' +        
           ' empeeno as ''Sales Person Number'', salesperson as ''Sales Person Name'',    TotalSales as ''Total Sales'', NoofWarrantySales as ''No Of Warranty Sales'', ' +        
           ' WarrantyCost as ''Warranty Cost'',ActualWarrantySales as ''Actual Warranty Sales'',  ' +         
        ' NoofWarrantableSales as ''Max No of Warrantable Sales'', MaxWarrantableSales as ''Max Value of Warrantable Sales'','+  
        ' NoOfWarrantyCancel as ''No Of Warranties Cancelled'', ValueWarrantyCancel as ''Value of Warranties Cancelled'', '+  
        'NoOfWarrantyRepossess as ''No Of Warranties Repossessed'', '+  
        'ValueWarrantyRepossess as ''Value Of Warranties Repossessed'', warrantycommissions as ''Warranty Commissions'', HitRate' +        
           ' FROM #hr_hrtotals ' +         
     'ORDER BY ' + @orderby        
             
   --PRINT @statement        
   exec sp_executesql @statement    
   end  
      
   --set @groupby = @groupby + @comma + ' acctno'        
            
  
  
END  
       
SET @return = @@error        
            
         
GO   
