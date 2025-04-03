SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO


if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].WeeklyTradingDataSP') 
            and OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
    DROP PROCEDURE dbo.WeeklyTradingDataSP
END
GO
  
CREATE PROCEDURE dbo.WeeklyTradingDataSP  
  
-- ================================================  
-- Project      : CoSACS .NET  
-- File Name    : WeeklyTradingDataSP.sql  
-- File Type    : MSSQL Server Stored Procedure Script  
-- Title        : Trading Data  
-- Author       : John Croft  
-- Date         : 28 October 2008 (initial coding - never live)  
--  
-- This procedure will get Trading data for the dates submitted.  
--  
-- Change Control  
-- --------------  
-- Date      By  Description  
-- ----      --  -----------  
-- 12/08/11  jec CR975 changes  
-- 31/08/11  jec Use account no. branch not Delivery branch & include Dept/Category code  
-- 05/09/11  jec Exclude repossessions - delorcoll='R'  
-- 06/09/11  jec Use Cost price from Audit table  
-- 08/09/11  jec Add Class/Class Description  
-- 26/09/11  jec Sales Type description  - differentiate between ProductSales 
-- 19/04/12  jec #9888 CR9430 Additional transactions/columns
-- 30/04/12  jec #10022 No records for Warranty Item
-- ================================================  
 -- Add the parameters for the stored procedure here  
 @return INT output  
   
As   
   
 set @return=0  
   
Declare @week1 datetime,  
  @week1Week int,  
  @CurrYrWeek int,  
  @CurrTradWeek int,  
  @CurrDateDay int,  
  @TradDateStart datetime,  
  @TradDateEnd datetime,  
  @YearStartDate datetime,  
  @TradDateStartLY datetime,  
  @TradDateEndLY datetime,  
  @YearStartDateLY datetime,  
  @TradDateStartCY datetime,  
  @TradDateEndCY datetime,  
  @YearStartDateCY datetime,
  @MonthStartDate datetime,			-- #9888
  @MonthStartDateLY datetime,			-- #9888
  @MonthStartDateCY datetime,			-- #9888  
  @ActualDate datetime,  
  @Times INT,  
  @DayNum3103 INT,  
  @ReportName VARCHAR(30),  
  @ReportNo TINYINT=0,  
  @Active BIT,  
  @sql NVARCHAR(200),  
  @BCPpath varchar(200),  
        @path varchar(200)  
   
     select @BCPpath=value from CountryMaintenance where codename='BCPpath'  
  -- #9888 ensure description is same for warranty category in all departments - otherwise causes Primary Key error
  declare @codedescript VARCHAR(50)
  select @codedescript=MAX(codedescript) from code where code='864' and category in('pce','pcf','war')
  UPDATE code set codedescript=@codedescript where code='864' and category in('pce','pcf','war') and codedescript!=@codedescript 
 
 set @ActualDate=Convert(datetime,CAST(GETDATE() as CHAR(12)),103)   
    
 select * into #TradingSummaryCY  from TradingSummary   -- Current year  
 select * into #TradingSummaryLY  from TradingSummary   -- Previous year  
 
  
   
 -- get dates into rows  
 declare @wtrdates TABLE (id INT identity,DtStartCY datetime,DtEndCY datetime,DtStartLY datetime,DtEndLY datetime,DtActive bit,DtFileName VARCHAR(30))  
  
 insert into @wtrdates  
 select DtStartCY1,DtEndCY1,DtStartLY1,DtEndLY1,DtActive1,DtFileName1  
 from wtrdates  
 insert into @wtrdates  
 select DtStartCY2,DtEndCY2,DtStartLY2,DtEndLY2,DtActive2,DtFileName2  
 from wtrdates  
 insert into @wtrdates  
 select DtStartCY3,DtEndCY3,DtStartLY3,DtEndLY3,DtActive3,DtFileName3  
 from wtrdates  
 insert into @wtrdates  
 select DtStartCY4,DtEndCY4,DtStartLY4,DtEndLY4,DtActive4,DtFileName4  
 from wtrdates  
 insert into @wtrdates  
 select DtStartCY5,DtEndCY5,DtStartLY5,DtEndLY5,DtActive5,DtFileName5  
 from wtrdates  
  
 -- loop through Report dates  
 While @ReportNo<5  
   
 Begin  
   
 set @ReportNo=@ReportNo+1  
   
 truncate TABLE #TradingSummaryCY  
 truncate TABLE #TradingSummaryLY  
 truncate TABLE WTRReport  
 truncate TABLE WTRReportDates  
  
 -- Set selection dates  
 select @TradDateStartCY=DtStartCY from @wtrdates where id=@ReportNo  
 select @TradDateEndCY=DATEADD(mi,-1,DATEADD(d,1,DtEndCY)) from @wtrdates where id=@ReportNo  -- 23:59:00  
 select @TradDateStartLY=DtStartLY from @wtrdates where id=@ReportNo  
 select @TradDateEndLY=DATEADD(mi,-1,DATEADD(d,1,DtEndLY)) from @wtrdates where id=@ReportNo  -- 23:59:00
 select @MonthStartDateCY=DATEADD(dd, DATEDIFF(dd,0,DATEADD(d,-DAY(@TradDateEndCY)+1,@TradDateEndCY)), 0)			-- #9888 month start without time
 select @MonthStartDateLY=DATEADD(dd, DATEDIFF(dd,0,DATEADD(d,-DAY(@TradDateEndLY)+1,@TradDateEndLY)), 0)			-- #9888 month start without time  
 select @ReportName=DtFileName from @wtrdates where id=@ReportNo  
 select @Active=DtActive from @wtrdates where id=@ReportNo  
   
 ----select @TradDateEndCY=DtEndCY from @wtrdates where id=@ReportNo  -- !!!Testing only  
 ----select @TradDateEndLY=DtEndLY from @wtrdates where id=@ReportNo  -- !!!Testing only  
   
 If @Active=1  -- Report date active  
 Begin   
   
 set @YearStartDateCY=CAST(case when Month(@TradDateStartCY)>3 then YEAR(@TradDateStartCY) else (YEAR(@TradDateStartCY)-1) end as CHAR(4)) + '-04-01'  
 set @YearStartDateLY=CAST(case when Month(@TradDateStartLY)>3 then YEAR(@TradDateStartLY) else (YEAR(@TradDateStartLY)-1) end as CHAR(4)) + '-04-01'  
   
 ----set @YearStartDateCY=@TradDateStartCY  -- !!!Testing only  
 ----set @YearStartDateLY=@TradDateStartLY  -- !!!Testing only  
   
 -- drop report table  
 --set @sql='IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(' + '''' + @ReportName + '''' + ') AND type in (N''U'')) Drop TABLE ' + @ReportName  
 --execute sp_executesql  @sql   
 --set @sql='IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(' + '''' + @ReportName + 'Dates' + '''' + ') AND type in (N''U'')) Drop TABLE ' + @ReportName +'Dates'  
 --execute sp_executesql  @sql  
        
 set @times=0  
 While @Times<2  
   
 BEGIN  
 set @Times=@Times+1  
 select @TradDateStart=case when @Times=1 then @TradDateStartCY else @TradDateStartLY end  
 select @TradDateEnd=case when @Times=1 then @TradDateEndCY else @TradDateEndLY end  
 select @YearStartDate=case when @Times=1 then @YearStartDateCY else @YearStartDateLY end
 select @MonthStartDate=case when @Times=1 then @MonthStartDateCY else @MonthStartDateLY end		-- #9888  
    
 -- Merchandise Sales  
 select  'M' as SalesCode,Category=case  
   when c.category='PCE' then 'Electrical          '  
   when c.category='PCF' then 'Furniture'  
   when c.category='PCW' then 'Workstation'  
   --when c.category='PCDIS' then 'Discounts'    
   when c.category='PCO' then 'Spare Parts' end ,  
  codedescript=case --when c.category='PCDIS' then 'Discounts'  
      when c.category!='PCO' or codedescript='Spare Parts' then codedescript   
      else codedescript end,convert(varchar(3), s.category) as DepartmentCode,  
      case when len(convert(varchar(3), s.category)) > 2      -- 14/10/11 Use fact category if len < 2  
      then ISNULL(s.Class,convert(varchar(3), s.category))  
      else convert(varchar(3), s.category) end as Class,   
      --else codedescript end,convert(varchar(3), s.category) as DepartmentCode,ISNULL(s.Class,'') as Class,     -- 08/09/11  
  SUBSTRING(acctno,1,3) as Branchno, Type=case when SUBSTRING(acctno,4,1)<4 then 'Credit' else 'Cash' end,  
  SUM(quantity) as TotQty,SUM(transvalue) as TotValue,CAST(0 as float) as ActualPct,SUM(transvalue)-SUM(pa.CostPrice*quantity) as ActualGP,  -- Audit cost price  
  CAST(0 as float) as ActualGPPct,CAST(0 as float) as YTDSales,CAST(0 as float) as YTDSalesPct,  
  CAST(0 as float) as YTDGP,CAST(0 as float) as YTDGPPct,
  SUM(pa.CostPrice*quantity) as ActualCost, CAST(0 as MONEY) as YTDCost, CAST(0 as MONEY) as ActualValueMTD,CAST(0 as MONEY) as ActualCostMTD		-- #9888
 into #TradingData  
 from delivery d   
  INNER JOIN stockitem s on d.ItemId = s.ItemId and d.stocklocn=s.stocklocn  
  INNER JOIN stockPriceAudit pa on d.itemid = pa.id and d.stocklocn=pa.branchno    -- for CostPrice  
  --INNER JOIN code c on convert(varchar(3), s.category) = c.code and c.category in('PCE','PCF','PCW','PCO')  
  INNER JOIN code c on convert(varchar(3), s.category) = c.code 
					and (c.category  in('PCE','PCF','PCW')		-- categorised product or .... Other and not Spare Part or Affinity and has both cash price and cost price
								or (c.category  in('PCO') and codedescript not in('Spare Parts','Affinity') and s.CostPrice!=0 and s.unitpricecash!=0))    -- #9888  
 where datetrans between @TradDateStart and @TradDateEnd  
  --and delorcoll!='R'   -- not a repossession				-- #9888
  and convert(varchar(3), s.category) not in (select code from code where category = 'WAR')  
  and pa.datechange=(select MAX(datechange) from stockPriceAudit p2 where d.itemid=p2.id and d.stocklocn=p2.branchno and p2.datechange<=d.datetrans) -- Audit cost price  
   Group by c.category,codedescript,convert(varchar(3), s.category),s.Class,   -- 08/09/11  
   SUBSTRING(acctno,1,3),case when SUBSTRING(acctno,4,1)<4 then 'Credit' else 'Cash' end  
   order by c.category,codedescript,SUBSTRING(acctno,1,3),case when SUBSTRING(acctno,4,1)<4 then 'Credit' else 'Cash' end  
   
 --Add Warranties  
 insert into #TradingData  
 select  'M' as SalesCode,Category=case  
   when cp.category='PCF' then 'Furniture'  
   ELSE 'Electrical' end ,  
  codedescript=case --when c.category='PCDIS' then 'Discounts'   
      when c.category!='PCO' or c.codedescript='Spare Parts' then c.codedescript  
      else c.codedescript end,convert(varchar(3), s.category) as DepartmentCode,  
      case when len(convert(varchar(3), s.category)) > 2     -- 14/10/11 Use fact category if len < 2  
      then ISNULL(s.Class,convert(varchar(3), s.category))  
      else convert(varchar(3), s.category) end as Class,   
      --else codedescript end,convert(varchar(3), s.category) as DepartmentCode,ISNULL(s.Class,'') as Class,     -- 08/09/11  
  SUBSTRING(acctno,1,3) as Branchno, Type=case when SUBSTRING(acctno,4,1)<4 then 'Credit' else 'Cash' end,  
  SUM(quantity) as TotQty,SUM(transvalue) as TotValue,CAST(0 as float) as ActualPct,SUM(transvalue)-SUM(pa.CostPrice*quantity) as ActualGP,  -- Audit cost price  
  CAST(0 as float) as ActualGPPct,CAST(0 as float) as YTDSales,CAST(0 as float) as YTDSalesPct,  
  CAST(0 as float) as YTDGP,CAST(0 as float) as YTDGPPct,
  SUM(pa.CostPrice*quantity) as ActualCost, CAST(0 as MONEY) as YTDCost, CAST(0 as MONEY) as ActualValueMTD,CAST(0 as MONEY) as ActualCostMTD		-- #9888  
   
 from delivery d   
  INNER JOIN stockitem s on d.ItemId = s.ItemId and d.stocklocn=s.stocklocn  
  INNER JOIN stockPriceAudit pa on d.itemid = pa.id and d.stocklocn=pa.branchno    -- for CostPrice  
  --INNER JOIN code c on convert(varchar(3), s.category) = c.code and c.category in('PCE','PCF','PCW','PCO')  
  INNER JOIN code c on convert(varchar(3), s.category) = c.code and (c.category  in('WAR'))  
  INNER JOIN stockinfo p on p.id = d.parentitemid   
  INNER JOIN code cp on convert(varchar(3), p.category) = cp.code and cp.category in('PCE','PCF','PCW')  
 where datetrans between @TradDateStart and @TradDateEnd  
  --and delorcoll!='R'   -- not a repossession				-- #9888
  --and convert(varchar(3), s.category) not in (select code from code where category = 'WAR')		-- #10022
  and pa.datechange=(select MAX(datechange) from stockPriceAudit p2 where d.itemid=p2.id and d.stocklocn=p2.branchno and p2.datechange<=d.datetrans) -- Audit cost price  
   Group by c.category,cp.category,cp.codedescript,c.codedescript,convert(varchar(3), s.category),s.Class,     -- 08/09/11  
   SUBSTRING(acctno,1,3),case when SUBSTRING(acctno,4,1)<4 then 'Credit' else 'Cash' end  
 order by c.category,cp.codedescript,SUBSTRING(acctno,1,3),case when SUBSTRING(acctno,4,1)<4 then 'Credit' else 'Cash' end  
    
  
 -- Add in Discounts  
 Insert into #TradingData  
 select  'D' as SalesCode,'Discounts','Discounts',0 as DepartmentCode,' ' as Class,     -- 08/09/11  
  SUBSTRING(acctno,1,3), Type=case when SUBSTRING(acctno,4,1)<4 then 'Credit' else 'Cash' end,  
  SUM(quantity) as TotQty,SUM(transvalue) as TotValue,CAST(0 as float) as ActualPct,SUM(transvalue)-SUM(CostPrice) as ActualGP,  
  CAST(0 as float) as ActualGPPct,CAST(0 as float) as YTDSales,CAST(0 as float) as YTDSalesPct,  
  CAST(0 as float) as YTDGP,CAST(0 as float) as YTDGPPct,
  CAST(0 as MONEY) as ActualCost, CAST(0 as MONEY) as YTDCost, CAST(0 as MONEY) as ActualValueMTD,CAST(0 as MONEY) as ActualCostMTD		-- #9888  
   
 from delivery d INNER JOIN stockitem s on d.ItemId = s.ItemId and d.stocklocn=s.stocklocn  
     INNER JOIN code c on convert(varchar(3), s.category) = c.code and c.category in('PCDIS')   
 where datetrans between @TradDateStart and @TradDateEnd  
  --and convert(varchar(3), s.category) in ( 36, 37, 38, 46, 47, 48, 86, 87, 88)  
 Group by SUBSTRING(acctno,1,3),case when SUBSTRING(acctno,4,1)<4 then 'Credit' else 'Cash' end  
 order by SUBSTRING(acctno,1,3),case when SUBSTRING(acctno,4,1)<4 then 'Credit' else 'Cash' end  
     
 -- Add in Service Charge  
 Insert into #TradingData  
 select  'S' as SalesCode,'Service Charge','Service Charge',0 as DepartmentCode,' ' as Class,     -- 08/09/11  
  SUBSTRING(acctno,1,3), Type=case when SUBSTRING(acctno,4,1)<4 then 'Credit' else 'Cash' end,  
  SUM(quantity) as TotQty,SUM(transvalue) as TotValue,CAST(0 as float) as ActualPct,SUM(transvalue)-SUM(CostPrice) as ActualGP,  
  CAST(0 as float) as ActualGPPct,CAST(0 as float) as YTDSales,CAST(0 as float) as YTDSalesPct,  
  CAST(0 as float) as YTDGP,CAST(0 as float) as YTDGPPct,
  CAST(0 as MONEY) as ActualCost, CAST(0 as MONEY) as YTDCost, CAST(0 as MONEY) as ActualValueMTD,CAST(0 as MONEY) as ActualCostMTD		-- #9888  
   
 from delivery d INNER JOIN stockitem s on d.ItemId = s.ItemId and d.stocklocn=s.stocklocn  
    
 where datetrans between @TradDateStart and @TradDateEnd  
  and s.IUPC='DT'  
 Group by SUBSTRING(acctno,1,3),case when SUBSTRING(acctno,4,1)<4 then 'Credit' else 'Cash' end  
 order by SUBSTRING(acctno,1,3),case when SUBSTRING(acctno,4,1)<4 then 'Credit' else 'Cash' end  
   
 -- Add in Credit/Cash diff  
 Insert into #TradingData  
 select  'F' as SalesCode,'Credit/Cash diff','Credit/Cash diff',0 as DepartmentCode,' ' as Class,     -- 08/09/11  
  SUBSTRING(acctno,1,3), Type=case when SUBSTRING(acctno,4,1)<4 then 'Credit' else 'Cash' end,  
  SUM(quantity) as TotQty,  
  case when SUM(quantity)=0 then 0 else SUM(quantity)/ABS(SUM(quantity)) end * SUM(unitpricehp-unitpricecash) as TotValue,CAST(0 as float) as ActualPct,CAST(0 as float) as ActualGP,  
  CAST(0 as float) as ActualGPPct,CAST(0 as float) as YTDSales,CAST(0 as float) as YTDSalesPct,  
  CAST(0 as float) as YTDGP,CAST(0 as float) as YTDGPPct,
  CAST(0 as MONEY) as ActualCost, CAST(0 as MONEY) as YTDCost, CAST(0 as MONEY) as ActualValueMTD,CAST(0 as MONEY) as ActualCostMTD		-- #9888  
   
 from delivery d INNER JOIN stockitem s on d.ItemId = s.ItemId and d.stocklocn=s.stocklocn  
     INNER JOIN code c on convert(varchar(3), s.category) = c.code and c.category like('PC%')  
    
 where datetrans between @TradDateStart and @TradDateEnd  
    and SUBSTRING(acctno,4,1)<4    -- only Credit accounts  
 Group by SUBSTRING(acctno,1,3),case when SUBSTRING(acctno,4,1)<4 then 'Credit' else 'Cash' end  
 order by SUBSTRING(acctno,1,3),case when SUBSTRING(acctno,4,1)<4 then 'Credit' else 'Cash' end  
     
 -- Add in Affinity Products		#9888
 Insert into #TradingData  
 select  'W' as SalesCode,'Affinity','Affinity',convert(varchar(3), s.category) as DepartmentCode,  
      case when len(convert(varchar(3), s.category)) > 2      
      then ISNULL(s.Class,convert(varchar(3), s.category))  
      else convert(varchar(3), s.category) end as Class,
  SUBSTRING(acctno,1,3) as Branchno, Type=case when SUBSTRING(acctno,4,1)<4 then 'Credit' else 'Cash' end,  
  SUM(quantity) as TotQty,SUM(transvalue) as TotValue,CAST(0 as float) as ActualPct,SUM(transvalue)-SUM(pa.CostPrice*quantity) as ActualGP,  
  CAST(0 as float) as ActualGPPct,CAST(0 as float) as YTDSales,CAST(0 as float) as YTDSalesPct,  
  CAST(0 as float) as YTDGP,CAST(0 as float) as YTDGPPct,
  SUM(pa.CostPrice*quantity) as ActualCost, CAST(0 as MONEY) as YTDCost, CAST(0 as MONEY) as ActualValueMTD,CAST(0 as MONEY) as ActualCostMTD		-- #9888    
   
 from delivery d INNER JOIN stockitem s on d.ItemId = s.ItemId and d.stocklocn=s.stocklocn
				INNER JOIN stockPriceAudit pa on d.itemid = pa.id and d.stocklocn=pa.branchno    -- for CostPrice 
				INNER JOIN code c on convert(varchar(3), s.category) = c.code and c.category like('PC%') and c.codedescript='Affinity'
 where datetrans between @TradDateStart and @TradDateEnd
		and pa.datechange=(select MAX(datechange) from stockPriceAudit p2 where d.itemid=p2.id and d.stocklocn=p2.branchno and p2.datechange<=d.datetrans) -- Audit cost price   
  --and convert(varchar(3), s.category) in ( 11, 51, 52, 53, 54, 55, 56, 57, 58, 59)  
 Group by convert(varchar(3), s.category),s.Class,SUBSTRING(acctno,1,3),case when SUBSTRING(acctno,4,1)<4 then 'Credit' else 'Cash' end  
 order by convert(varchar(3), s.category),s.Class,SUBSTRING(acctno,1,3),case when SUBSTRING(acctno,4,1)<4 then 'Credit' else 'Cash' end  
   
   
 -- Add in Rebates  
 Insert into #TradingData  
 select  'R' as SalesCode,'Rebates','Rebates',0 as DepartmentCode,' ' as Class,     -- 08/09/11  
  SUBSTRING(acctno,1,3), Type=case when SUBSTRING(acctno,4,1)<4 then 'Credit' else 'Cash' end,  
  SUM(quantity) as TotQty,SUM(transvalue) as TotValue,CAST(0 as float) as ActualPct,SUM(transvalue)-SUM(CostPrice) as ActualGP,  
  CAST(0 as float) as ActualGPPct,CAST(0 as float) as YTDSales,CAST(0 as float) as YTDSalesPct,  
  CAST(0 as float) as YTDGP,CAST(0 as float) as YTDGPPct,
  CAST(0 as MONEY) as ActualCost, CAST(0 as MONEY) as YTDCost, CAST(0 as MONEY) as ActualValueMTD,CAST(0 as MONEY) as ActualCostMTD		-- #9888  
   
 from delivery d INNER JOIN stockitem s on d.ItemId = s.ItemId and d.stocklocn=s.stocklocn  
  and s.IUPC='RB'    
 where datetrans between @TradDateStart and @TradDateEnd    
 Group by SUBSTRING(acctno,1,3),case when SUBSTRING(acctno,4,1)<4 then 'Credit' else 'Cash' end  
 order by SUBSTRING(acctno,1,3),case when SUBSTRING(acctno,4,1)<4 then 'Credit' else 'Cash' end     
  
 -- Year to Date Values  
   
 select  'M' as SalesCode,Category=case  
   when c.category='PCE' then 'Electrical          '  
   when c.category='PCF' then 'Furniture'  
   when c.category='PCW' then 'Workstation'  
   --when c.category='PCDIS' then 'Discounts'    
   when c.category='PCO' then 'Spare Parts' end ,  
  codedescript=case --when c.category='PCDIS' then 'Discounts'   
      when c.category!='PCO' or codedescript='Spare Parts' then codedescript  
      else codedescript end,convert(varchar(3), s.category) as DepartmentCode,  
      case when len(convert(varchar(3), s.category)) > 2     -- 14/10/11 Use fact category if len < 2  
      then ISNULL(s.Class,convert(varchar(3), s.category))  
      else convert(varchar(3), s.category) end as Class,     -- 08/09/11  
      --else codedescript end,convert(varchar(3), s.category) as DepartmentCode,ISNULL(s.Class,'') as Class,     -- 08/09/11  
  SUBSTRING(acctno,1,3) as BranchNo, Type=case when SUBSTRING(acctno,4,1)<4 then 'Credit' else 'Cash' end,  
  CAST(0 as int) as TotQty,CAST(0 as float) as TotValue,CAST(0 as float) as ActualPct,CAST(0 as float) as ActualGP,  
  CAST(0 as float) as ActualGPPct,SUM(quantity) as YTDTotQty,SUM(transvalue) as YTDSales,CAST(0 as float) as YTDSalesPct,  
  SUM(transvalue)-SUM(pa.CostPrice*quantity) as YTDGP,CAST(0 as float) as YTDGPPct,   -- Audit cost price 
  CAST(0 as MONEY) as ActualCost,SUM(pa.CostPrice*quantity) as YTDCost, CAST(0 as MONEY) as ActualValueMTD,CAST(0 as MONEY) as ActualCostMTD		-- #9888 
 into #TradingDataYTD  
 from delivery d INNER JOIN stockitem s on d.ItemId = s.ItemId and d.stocklocn=s.stocklocn  
   INNER JOIN stockPriceAudit pa on d.itemid = pa.id and d.stocklocn=pa.branchno    -- for CostPrice  
   --INNER JOIN code c on convert(varchar(3), s.category) = c.code and c.category in('PCE','PCF','PCW','PCO')  
   INNER JOIN code c on convert(varchar(3), s.category) = c.code 
					and (c.category  in('PCE','PCF','PCW')		-- categorised product or .... Other and not Spare Part or Affinity and has both cash price and cost price
								or (c.category  in('PCO') and codedescript not in('Spare Parts','Affinity') and s.CostPrice!=0 and s.unitpricecash!=0))    -- #9888  
 where datetrans between @YearStartDate and @TradDateEnd  
  --and delorcoll!='R'   -- not a repossession				-- #9888
  and convert(varchar(3), s.category) not in (select code from code where category = 'WAR')  
  and pa.datechange=(select MAX(datechange) from stockPriceAudit p2 where d.itemid=p2.id and d.stocklocn=p2.branchno and p2.datechange<=d.datetrans) -- Audit cost price  
 Group by c.category,c.codedescript,convert(varchar(3), s.category),s.Class,     -- 08/09/11  
   SUBSTRING(acctno,1,3),case when SUBSTRING(acctno,4,1)<4 then 'Credit' else 'Cash' end  
 order by c.category,codedescript,SUBSTRING(acctno,1,3),case when SUBSTRING(acctno,4,1)<4 then 'Credit' else 'Cash' end  
   
  
 -- Add in Warranties  
 insert into #TradingDataYTD  
 select  'M' as SalesCode,Category=case  
   when cp.category='PCF' then 'Furniture'  
   ELSE 'Electrical' end ,  
  codedescript=case --when c.category='PCDIS' then 'Discounts'   
      when c.category!='PCO' or c.codedescript='Spare Parts' then c.codedescript  
      else c.codedescript end,convert(varchar(3), s.category) as DepartmentCode,  
      case when len(convert(varchar(3), s.category)) > 2     -- 14/10/11 Use fact category if len < 2  
      then ISNULL(s.Class,convert(varchar(3), s.category))  
      else convert(varchar(3), s.category) end as Class,     -- 08/09/11  
      --else codedescript end,convert(varchar(3), s.category) as DepartmentCode,ISNULL(s.Class,'') as Class,     -- 08/09/11  
  SUBSTRING(acctno,1,3) as BranchNo, Type=case when SUBSTRING(acctno,4,1)<4 then 'Credit' else 'Cash' end,  
  CAST(0 as int) as TotQty,CAST(0 as float) as TotValue,CAST(0 as float) as ActualPct,CAST(0 as float) as ActualGP,  
  CAST(0 as float) as ActualGPPct,SUM(quantity) as YTDTotQty,SUM(transvalue) as YTDSales,CAST(0 as float) as YTDSalesPct,  
  SUM(transvalue)-SUM(pa.CostPrice*quantity) as YTDGP,CAST(0 as float) as YTDGPPct,   -- Audit cost price
  CAST(0 as MONEY) as ActualCost,SUM(pa.CostPrice*quantity) as YTDCost, CAST(0 as MONEY) as ActualValueMTD,CAST(0 as MONEY) as ActualCostMTD		-- #9888   
   
 from delivery d INNER JOIN stockitem s on d.ItemId = s.ItemId and d.stocklocn=s.stocklocn  
   INNER JOIN stockPriceAudit pa on d.itemid = pa.id and d.stocklocn=pa.branchno    -- for CostPrice  
   --INNER JOIN code c on convert(varchar(3), s.category) = c.code and c.category in('PCE','PCF','PCW','PCO')  
   INNER JOIN code c on convert(varchar(3), s.category) = c.code and (c.category  in('WAR') ) --or c.category  in('PCO') and codedescript='Spare Parts')    --!!! change  
   INNER JOIN stockinfo p on p.id = d.parentitemid   
   INNER JOIN code cp on convert(varchar(3), p.category) = cp.code and cp.category in('PCE','PCF','PCW')  
 where datetrans between @YearStartDate and @TradDateEnd  
  --and delorcoll!='R'   -- not a repossession				-- #9888
  and pa.datechange=(select MAX(datechange) from stockPriceAudit p2 where d.itemid=p2.id and d.stocklocn=p2.branchno and p2.datechange<=d.datetrans) -- Audit cost price  
 Group by c.category,cp.category,cp.codedescript,c.codedescript,convert(varchar(3), s.category),s.Class,     -- 08/09/11  
   SUBSTRING(acctno,1,3),case when SUBSTRING(acctno,4,1)<4 then 'Credit' else 'Cash' end  
 order by c.category,cp.codedescript,SUBSTRING(acctno,1,3),case when SUBSTRING(acctno,4,1)<4 then 'Credit' else 'Cash' end  
  
 -- Add in Discounts  
 Insert into #TradingDataYTD  
 select  'D' as SalesCode,'Discounts','Discounts',0 as DepartmentCode,' ' as Class,     -- 08/09/11  
  SUBSTRING(acctno,1,3),   
  Type=case when SUBSTRING(acctno,4,1)<4 then 'Credit' else 'Cash' end,  
  CAST(0 as int) as TotQty,CAST(0 as float) as TotValue,CAST(0 as float) as ActualPct,CAST(0 as float) as ActualGP,  
  CAST(0 as float) as ActualGPPct,SUM(quantity) as YTDTotQty,SUM(transvalue) as YTDSales,CAST(0 as float) as YTDSalesPct,  
  SUM(transvalue)-SUM(CostPrice) as YTDGP,CAST(0 as float) as YTDGPPct,
  CAST(0 as MONEY) as ActualCost,CAST(0 as MONEY) as YTDCost, CAST(0 as MONEY) as ActualValueMTD,CAST(0 as MONEY) as ActualCostMTD		-- #9888     
   
 from delivery d INNER JOIN stockitem s on d.ItemId = s.ItemId and d.stocklocn=s.stocklocn  
     INNER JOIN code c on convert(varchar(3), s.category) = c.code and c.category in('PCDIS')   
 where datetrans between @YearStartDate and @TradDateEnd  
  --and convert(varchar(3), s.category) in ( 36, 37, 38, 46, 47, 48, 86, 87, 88)  -- Discounts  
 Group by SUBSTRING(acctno,1,3),case when SUBSTRING(acctno,4,1)<4 then 'Credit' else 'Cash' end  
 order by SUBSTRING(acctno,1,3),case when SUBSTRING(acctno,4,1)<4 then 'Credit' else 'Cash' end  
     
 -- Add in Service Charge  
 Insert into #TradingDataYTD  
 select  'S' as SalesCode,'Service Charge','Service Charge',0 as DepartmentCode,' ' as Class,     -- 08/09/11  
  SUBSTRING(acctno,1,3), Type=case when SUBSTRING(acctno,4,1)<4 then 'Credit' else 'Cash' end,  
  CAST(0 as int) as TotQty,CAST(0 as float) as TotValue,CAST(0 as float) as ActualPct,CAST(0 as float) as ActualGP,  
  CAST(0 as float) as ActualGPPct,SUM(quantity) as YTDTotQty,SUM(transvalue) as YTDSales,CAST(0 as float) as YTDSalesPct,  
  SUM(transvalue)-SUM(CostPrice) as YTDGP,CAST(0 as float) as YTDGPPct,
  CAST(0 as MONEY) as ActualCost,CAST(0 as MONEY) as YTDCost, CAST(0 as MONEY) as ActualValueMTD,CAST(0 as MONEY) as ActualCostMTD		-- #9888   
   
 from delivery d INNER JOIN stockitem s on d.ItemId = s.ItemId and d.stocklocn=s.stocklocn  
    
 where datetrans between @YearStartDate and @TradDateEnd  
  and s.IUPC='DT'  
 Group by SUBSTRING(acctno,1,3),case when SUBSTRING(acctno,4,1)<4 then 'Credit' else 'Cash' end  
 order by SUBSTRING(acctno,1,3),case when SUBSTRING(acctno,4,1)<4 then 'Credit' else 'Cash' end  
   
 -- Add in Credit/Cash diff  
 Insert into #TradingDataYTD  
 select  'F' as SalesCode,'Credit/Cash diff','Credit/Cash diff',0 as DepartmentCode,' ' as Class,     -- 08/09/11  
  SUBSTRING(acctno,1,3), Type=case when SUBSTRING(acctno,4,1)<4 then 'Credit' else 'Cash' end,  
  CAST(0 as int) as TotQty,CAST(0 as float) as TotValue,CAST(0 as float) as ActualPct,CAST(0 as float) as ActualGP,  
  CAST(0 as float) as ActualGPPct,SUM(quantity) as YTDTotQty,  
  case when SUM(quantity)=0 then 0 else SUM(quantity)/ABS(SUM(quantity)) end *SUM(unitpricehp-unitpricecash) as YTDSales,CAST(0 as float) as YTDSalesPct,  
  CAST(0 as float) as YTDGP,CAST(0 as float) as YTDGPPct,
  CAST(0 as MONEY) as ActualCost,CAST(0 as MONEY) as YTDCost, CAST(0 as MONEY) as ActualValueMTD,CAST(0 as MONEY) as ActualCostMTD		-- #9888   
   
 from delivery d INNER JOIN stockitem s on d.ItemId = s.ItemId and d.stocklocn=s.stocklocn  
     INNER JOIN code c on convert(varchar(3), s.category) = c.code and c.category like('PC%')  
 where datetrans between @YearStartDate and @TradDateEnd  
    and SUBSTRING(acctno,4,1)<4    -- only Credit accounts    
 Group by SUBSTRING(acctno,1,3),case when SUBSTRING(acctno,4,1)<4 then 'Credit' else 'Cash' end  
 order by SUBSTRING(acctno,1,3),case when SUBSTRING(acctno,4,1)<4 then 'Credit' else 'Cash' end  
     
 -- Add in Affinity Products		#9888
 Insert into #TradingDataYTD  
 select  'W' as SalesCode,'Affinity','Affinity',convert(varchar(3), s.category) as DepartmentCode,  
      case when len(convert(varchar(3), s.category)) > 2      
      then ISNULL(s.Class,convert(varchar(3), s.category))  
      else convert(varchar(3), s.category) end as Class,
  SUBSTRING(acctno,1,3) as Branchno, Type=case when SUBSTRING(acctno,4,1)<4 then 'Credit' else 'Cash' end,  
  CAST(0 as int) as TotQty,CAST(0 as float) as TotValue,CAST(0 as float) as ActualPct,CAST(0 as float) as ActualGP,  
  CAST(0 as float) as ActualGPPct,SUM(quantity) as YTDTotQty,SUM(transvalue) as YTDSales,CAST(0 as float) as YTDSalesPct,  
  SUM(transvalue)-SUM(pa.CostPrice*quantity) as YTDGP,CAST(0 as float) as YTDGPPct,
  CAST(0 as MONEY) as ActualCost, SUM(pa.CostPrice*quantity) as YTDCost, CAST(0 as MONEY) as ActualValueMTD,CAST(0 as MONEY) as ActualCostMTD		-- #9888    
   
 from delivery d INNER JOIN stockitem s on d.ItemId = s.ItemId and d.stocklocn=s.stocklocn
				INNER JOIN stockPriceAudit pa on d.itemid = pa.id and d.stocklocn=pa.branchno    -- for CostPrice 
				INNER JOIN code c on convert(varchar(3), s.category) = c.code and c.category like('PC%') and c.codedescript='Affinity'    
 where datetrans between @YearStartDate and @TradDateEnd
		and pa.datechange=(select MAX(datechange) from stockPriceAudit p2 where d.itemid=p2.id and d.stocklocn=p2.branchno and p2.datechange<=d.datetrans) -- Audit cost price   
  --and convert(varchar(3), s.category) in ( 11, 51, 52, 53, 54, 55, 56, 57, 58, 59)  
 Group by convert(varchar(3), s.category),s.Class,SUBSTRING(acctno,1,3),case when SUBSTRING(acctno,4,1)<4 then 'Credit' else 'Cash' end  
 order by convert(varchar(3), s.category),s.Class,SUBSTRING(acctno,1,3),case when SUBSTRING(acctno,4,1)<4 then 'Credit' else 'Cash' end    
     
   
 -- Add in Rebates  
 Insert into #TradingDataYTD  
 select  'R' as SalesCode,'Rebates','Rebates',0 as DepartmentCode,' ' as Class,     -- 08/09/11  
  SUBSTRING(acctno,1,3), Type=case when SUBSTRING(acctno,4,1)<4 then 'Credit' else 'Cash' end,  
  CAST(0 as int) as TotQty,CAST(0 as float) as TotValue,CAST(0 as float) as ActualPct,CAST(0 as float) as ActualGP,  
  CAST(0 as float) as ActualGPPct,SUM(quantity) as YTDTotQty,SUM(transvalue) as YTDSales,CAST(0 as float) as YTDSalesPct,  
  SUM(transvalue)-SUM(CostPrice) as YTDGP,CAST(0 as float) as YTDGPPct,
  CAST(0 as MONEY) as ActualCost,CAST(0 as MONEY) as YTDCost, CAST(0 as MONEY) as ActualValueMTD,CAST(0 as MONEY) as ActualCostMTD		-- #9888   
   
 from delivery d INNER JOIN stockitem s on d.ItemId = s.ItemId and d.stocklocn=s.stocklocn  
  and s.IUPC='RB'    
 where datetrans between @YearStartDate and @TradDateEnd    
 Group by SUBSTRING(acctno,1,3),case when SUBSTRING(acctno,4,1)<4 then 'Credit' else 'Cash' end  
 order by SUBSTRING(acctno,1,3),case when SUBSTRING(acctno,4,1)<4 then 'Credit' else 'Cash' end
  
 
 -- Month to Date Values		#9888    
 select  'M' as SalesCode,Category=case  
   when c.category='PCE' then 'Electrical          '  
   when c.category='PCF' then 'Furniture'  
   when c.category='PCW' then 'Workstation'  
   --when c.category='PCDIS' then 'Discounts'    
   when c.category='PCO' then 'Spare Parts' end ,  
  codedescript=case --when c.category='PCDIS' then 'Discounts'   
      when c.category!='PCO' or codedescript='Spare Parts' then codedescript  
      else codedescript end,convert(varchar(3), s.category) as DepartmentCode,  
      case when len(convert(varchar(3), s.category)) > 2     -- 14/10/11 Use fact category if len < 2  
      then ISNULL(s.Class,convert(varchar(3), s.category))  
      else convert(varchar(3), s.category) end as Class,     -- 08/09/11  
      --else codedescript end,convert(varchar(3), s.category) as DepartmentCode,ISNULL(s.Class,'') as Class,     -- 08/09/11  
  SUBSTRING(acctno,1,3) as BranchNo, Type=case when SUBSTRING(acctno,4,1)<4 then 'Credit' else 'Cash' end,  
  CAST(0 as int) as TotQty,CAST(0 as float) as TotValue,CAST(0 as float) as ActualPct,CAST(0 as float) as ActualGP,  
  CAST(0 as float) as ActualGPPct,SUM(quantity) as YTDTotQty,CAST(0 as MONEY) as YTDSales,CAST(0 as float) as YTDSalesPct,			-- #9888 
  CAST(0 as float) as YTDGP,CAST(0 as float) as YTDGPPct,   -- Audit cost price 
  CAST(0 as MONEY) as ActualCost,CAST(0 as MONEY) as YTDCost, SUM(transvalue) as ActualValueMTD,SUM(pa.CostPrice*quantity) as ActualCostMTD		-- #9888 
 into #TradingDataMTD  
 from delivery d INNER JOIN stockitem s on d.ItemId = s.ItemId and d.stocklocn=s.stocklocn  
   INNER JOIN stockPriceAudit pa on d.itemid = pa.id and d.stocklocn=pa.branchno    -- for CostPrice  
   --INNER JOIN code c on convert(varchar(3), s.category) = c.code and c.category in('PCE','PCF','PCW','PCO')  
   INNER JOIN code c on convert(varchar(3), s.category) = c.code 
					and (c.category  in('PCE','PCF','PCW')		-- categorised product or .... Other and not Spare Part or Affinity and has both cash price and cost price
								or (c.category  in('PCO') and codedescript not in('Spare Parts','Affinity') and s.CostPrice!=0 and s.unitpricecash!=0))    -- #9888 
 where datetrans between @MonthStartDate and @TradDateEnd  
  --and delorcoll!='R'   -- not a repossession				-- #9888
  and convert(varchar(3), s.category) not in (select code from code where category = 'WAR')  
  and pa.datechange=(select MAX(datechange) from stockPriceAudit p2 where d.itemid=p2.id and d.stocklocn=p2.branchno and p2.datechange<=d.datetrans) -- Audit cost price  
 Group by c.category,c.codedescript,convert(varchar(3), s.category),s.Class,     -- 08/09/11  
   SUBSTRING(acctno,1,3),case when SUBSTRING(acctno,4,1)<4 then 'Credit' else 'Cash' end  
 order by c.category,codedescript,SUBSTRING(acctno,1,3),case when SUBSTRING(acctno,4,1)<4 then 'Credit' else 'Cash' end  
   
  
 -- Add in Warranties  
 insert into #TradingDataMTD  
 select  'M' as SalesCode,Category=case  
   when cp.category='PCF' then 'Furniture'  
   ELSE 'Electrical' end ,  
  codedescript=case --when c.category='PCDIS' then 'Discounts'   
      when c.category!='PCO' or c.codedescript='Spare Parts' then c.codedescript  
      else c.codedescript end,convert(varchar(3), s.category) as DepartmentCode,  
      case when len(convert(varchar(3), s.category)) > 2     -- 14/10/11 Use fact category if len < 2  
      then ISNULL(s.Class,convert(varchar(3), s.category))  
      else convert(varchar(3), s.category) end as Class,     -- 08/09/11  
      --else codedescript end,convert(varchar(3), s.category) as DepartmentCode,ISNULL(s.Class,'') as Class,     -- 08/09/11  
  SUBSTRING(acctno,1,3) as BranchNo, Type=case when SUBSTRING(acctno,4,1)<4 then 'Credit' else 'Cash' end,  
  CAST(0 as int) as TotQty,CAST(0 as float) as TotValue,CAST(0 as float) as ActualPct,CAST(0 as float) as ActualGP,  
  CAST(0 as float) as ActualGPPct,SUM(quantity) as YTDTotQty,CAST(0 as MONEY) as YTDSales,CAST(0 as float) as YTDSalesPct,			-- #9888 
  CAST(0 as float) as YTDGP,CAST(0 as float) as YTDGPPct,   -- Audit cost price
  CAST(0 as MONEY) as ActualCost,CAST(0 as MONEY) as YTDCost, SUM(transvalue) as ActualValueMTD,SUM(pa.CostPrice*quantity) as ActualCostMTD		-- #9888   
   
 from delivery d INNER JOIN stockitem s on d.ItemId = s.ItemId and d.stocklocn=s.stocklocn  
   INNER JOIN stockPriceAudit pa on d.itemid = pa.id and d.stocklocn=pa.branchno    -- for CostPrice  
   --INNER JOIN code c on convert(varchar(3), s.category) = c.code and c.category in('PCE','PCF','PCW','PCO')  
   INNER JOIN code c on convert(varchar(3), s.category) = c.code and (c.category  in('WAR') ) --or c.category  in('PCO') and codedescript='Spare Parts')    --!!! change  
   INNER JOIN stockinfo p on p.id = d.parentitemid   
   INNER JOIN code cp on convert(varchar(3), p.category) = cp.code and cp.category in('PCE','PCF','PCW')  
 where datetrans between @MonthStartDate and @TradDateEnd  
  --and delorcoll!='R'   -- not a repossession				-- #9888
  and pa.datechange=(select MAX(datechange) from stockPriceAudit p2 where d.itemid=p2.id and d.stocklocn=p2.branchno and p2.datechange<=d.datetrans) -- Audit cost price  
 Group by c.category,cp.category,cp.codedescript,c.codedescript,convert(varchar(3), s.category),s.Class,     -- 08/09/11  
   SUBSTRING(acctno,1,3),case when SUBSTRING(acctno,4,1)<4 then 'Credit' else 'Cash' end  
 order by c.category,cp.codedescript,SUBSTRING(acctno,1,3),case when SUBSTRING(acctno,4,1)<4 then 'Credit' else 'Cash' end  
  
 -- Add in Discounts  
 Insert into #TradingDataMTD  
 select  'D' as SalesCode,'Discounts','Discounts',0 as DepartmentCode,' ' as Class,     -- 08/09/11  
  SUBSTRING(acctno,1,3),   
  Type=case when SUBSTRING(acctno,4,1)<4 then 'Credit' else 'Cash' end,  
  CAST(0 as int) as TotQty,CAST(0 as float) as TotValue,CAST(0 as float) as ActualPct,CAST(0 as float) as ActualGP,  
  CAST(0 as float) as ActualGPPct,SUM(quantity) as YTDTotQty,CAST(0 as MONEY) as YTDSales,CAST(0 as float) as YTDSalesPct,				-- #9888 
  CAST(0 as float) as YTDGP,CAST(0 as float) as YTDGPPct,
  CAST(0 as MONEY) as ActualCost,CAST(0 as MONEY) as YTDCost, SUM(transvalue) as ActualValueMTD,CAST(0 as MONEY) as ActualCostMTD		-- #9888     
   
 from delivery d INNER JOIN stockitem s on d.ItemId = s.ItemId and d.stocklocn=s.stocklocn  
     INNER JOIN code c on convert(varchar(3), s.category) = c.code and c.category in('PCDIS')   
 where datetrans between @MonthStartDate and @TradDateEnd  
  --and convert(varchar(3), s.category) in ( 36, 37, 38, 46, 47, 48, 86, 87, 88)  -- Discounts  
 Group by SUBSTRING(acctno,1,3),case when SUBSTRING(acctno,4,1)<4 then 'Credit' else 'Cash' end  
 order by SUBSTRING(acctno,1,3),case when SUBSTRING(acctno,4,1)<4 then 'Credit' else 'Cash' end  
     
 -- Add in Service Charge  
 Insert into #TradingDataMTD  
 select  'S' as SalesCode,'Service Charge','Service Charge',0 as DepartmentCode,' ' as Class,     -- 08/09/11  
  SUBSTRING(acctno,1,3), Type=case when SUBSTRING(acctno,4,1)<4 then 'Credit' else 'Cash' end,  
  CAST(0 as int) as TotQty,CAST(0 as float) as TotValue,CAST(0 as float) as ActualPct,CAST(0 as float) as ActualGP,  
  CAST(0 as float) as ActualGPPct,SUM(quantity) as YTDTotQty,CAST(0 as MONEY) as YTDSales,CAST(0 as float) as YTDSalesPct,			-- #9888 
  CAST(0 as float) as YTDGP,CAST(0 as float) as YTDGPPct,
  CAST(0 as MONEY) as ActualCost,CAST(0 as MONEY) as YTDCost, SUM(transvalue) as ActualValueMTD,CAST(0 as MONEY) as ActualCostMTD		-- #9888   
   
 from delivery d INNER JOIN stockitem s on d.ItemId = s.ItemId and d.stocklocn=s.stocklocn  
    
 where datetrans between @MonthStartDate and @TradDateEnd  
  and s.IUPC='DT'  
 Group by SUBSTRING(acctno,1,3),case when SUBSTRING(acctno,4,1)<4 then 'Credit' else 'Cash' end  
 order by SUBSTRING(acctno,1,3),case when SUBSTRING(acctno,4,1)<4 then 'Credit' else 'Cash' end  
   
 -- Add in Credit/Cash diff  
 Insert into #TradingDataMTD  
 select  'F' as SalesCode,'Credit/Cash diff','Credit/Cash diff',0 as DepartmentCode,' ' as Class,     -- 08/09/11  
  SUBSTRING(acctno,1,3), Type=case when SUBSTRING(acctno,4,1)<4 then 'Credit' else 'Cash' end,  
  CAST(0 as int) as TotQty,CAST(0 as float) as TotValue,CAST(0 as float) as ActualPct,CAST(0 as float) as ActualGP,  
  CAST(0 as float) as ActualGPPct,SUM(quantity) as YTDTotQty,  
  CAST(0 as MONEY) as YTDSales,CAST(0 as float) as YTDSalesPct,			-- #9888 
  CAST(0 as float) as YTDGP,CAST(0 as float) as YTDGPPct,
  CAST(0 as MONEY) as ActualCost,CAST(0 as MONEY) as YTDCost, 
  case when SUM(quantity)=0 then 0 else SUM(quantity)/ABS(SUM(quantity)) end *SUM(unitpricehp-unitpricecash) as ActualValueMTD,CAST(0 as MONEY) as ActualCostMTD		-- #9888   
   
 from delivery d INNER JOIN stockitem s on d.ItemId = s.ItemId and d.stocklocn=s.stocklocn  
     INNER JOIN code c on convert(varchar(3), s.category) = c.code and c.category like('PC%')  
 where datetrans between @MonthStartDate and @TradDateEnd  
    and SUBSTRING(acctno,4,1)<4    -- only Credit accounts    
 Group by SUBSTRING(acctno,1,3),case when SUBSTRING(acctno,4,1)<4 then 'Credit' else 'Cash' end  
 order by SUBSTRING(acctno,1,3),case when SUBSTRING(acctno,4,1)<4 then 'Credit' else 'Cash' end  
     
 -- Add in Affinity Products				#9888
 Insert into #TradingDataMTD  
 select  'W' as SalesCode,'Affinity','Affinity',convert(varchar(3), s.category) as DepartmentCode,  
      case when len(convert(varchar(3), s.category)) > 2      
      then ISNULL(s.Class,convert(varchar(3), s.category))  
      else convert(varchar(3), s.category) end as Class,
  SUBSTRING(acctno,1,3) as Branchno, Type=case when SUBSTRING(acctno,4,1)<4 then 'Credit' else 'Cash' end,  
  CAST(0 as int) as TotQty,CAST(0 as float) as TotValue,CAST(0 as float) as ActualPct,CAST(0 as float) as ActualGP,  
  CAST(0 as float) as ActualGPPct,SUM(quantity) as YTDTotQty,CAST(0 as MONEY) as YTDSales,CAST(0 as float) as YTDSalesPct,  
  CAST(0 as float) as YTDGP,CAST(0 as float) as YTDGPPct,
  CAST(0 as MONEY) as ActualCost, CAST(0 as MONEY) as YTDCost, SUM(transvalue) as ActualValueMTD,SUM(pa.CostPrice*quantity) as ActualCostMTD		-- #9888    
   
 from delivery d INNER JOIN stockitem s on d.ItemId = s.ItemId and d.stocklocn=s.stocklocn
				INNER JOIN stockPriceAudit pa on d.itemid = pa.id and d.stocklocn=pa.branchno    -- for CostPrice 
				INNER JOIN code c on convert(varchar(3), s.category) = c.code and c.category like('PC%') and c.codedescript='Affinity'    
 where datetrans between @MonthStartDate and @TradDateEnd
		and pa.datechange=(select MAX(datechange) from stockPriceAudit p2 where d.itemid=p2.id and d.stocklocn=p2.branchno and p2.datechange<=d.datetrans) -- Audit cost price   
  --and convert(varchar(3), s.category) in ( 11, 51, 52, 53, 54, 55, 56, 57, 58, 59)  
 Group by convert(varchar(3), s.category),s.Class,SUBSTRING(acctno,1,3),case when SUBSTRING(acctno,4,1)<4 then 'Credit' else 'Cash' end  
 order by convert(varchar(3), s.category),s.Class,SUBSTRING(acctno,1,3),case when SUBSTRING(acctno,4,1)<4 then 'Credit' else 'Cash' end    
      
   
 -- Add in Rebates  
 Insert into #TradingDataMTD  
 select  'R' as SalesCode,'Rebates','Rebates',0 as DepartmentCode,' ' as Class,     -- 08/09/11  
  SUBSTRING(acctno,1,3), Type=case when SUBSTRING(acctno,4,1)<4 then 'Credit' else 'Cash' end,  
  CAST(0 as int) as TotQty,CAST(0 as float) as TotValue,CAST(0 as float) as ActualPct,CAST(0 as float) as ActualGP,  
  CAST(0 as float) as ActualGPPct,SUM(quantity) as YTDTotQty,CAST(0 as MONEY) as YTDSales,CAST(0 as float) as YTDSalesPct,		-- #9888 
  CAST(0 as float) as YTDGP,CAST(0 as float) as YTDGPPct,
  CAST(0 as MONEY) as ActualCost,CAST(0 as MONEY) as YTDCost, SUM(transvalue) as ActualValueMTD,CAST(0 as MONEY) as ActualCostMTD		-- #9888   
   
 from delivery d INNER JOIN stockitem s on d.ItemId = s.ItemId and d.stocklocn=s.stocklocn  
  and s.IUPC='RB'    
 where datetrans between @MonthStartDate and @TradDateEnd    
 Group by SUBSTRING(acctno,1,3),case when SUBSTRING(acctno,4,1)<4 then 'Credit' else 'Cash' end  
 order by SUBSTRING(acctno,1,3),case when SUBSTRING(acctno,4,1)<4 then 'Credit' else 'Cash' end  
 
  
   
 -- Calc Total sales - excl Service Charge & Discounts  
 select branchno,SUM(Totvalue) as TotalSales, SUM(ActualCost) as TotalCost				-- #9888  
 into #TotalSales from #TradingData   
 where SalesCode in('M','W')  
 Group by branchno   
   
 insert into #TotalSales   -- Company Total  
  select 0,SUM(TotalSales),SUM(TotalCost)			-- #9888 
  from #TotalSales  
   
 select branchno,SUM(YTDSales) as TotalSales, SUM(YTDCost) as TotalCost				-- #9888 
 into #TotalSalesYTD from #TradingDataYTD   
 where SalesCode in('M','W')  
 Group by branchno  
   
 insert into #TotalSalesYTD  -- Company Total  
  select 0,SUM(TotalSales),SUM(TotalCost)			-- #9888 
  from #TotalSalesYTD  
    
 -- Calc Total sales - inc Service Charge & Discounts  
 select branchno,SUM(Totvalue) as TotalSales,SUM(ActualCost) as TotalCost			-- #9888 
 into #TotalSalesSD from #TradingData    
 Group by branchno   
   
 insert into #TotalSalesSD  -- Company Total  
  select 0,SUM(TotalSales),SUM(TotalCost)			-- #9888 
  from #TotalSalesSD  
   
 select branchno,SUM(YTDSales) as TotalSales, SUM(YTDCost) as TotalCost				-- #9888 
 into #TotalSalesYTDSD from #TradingDataYTD   
 Group by branchno  
   
 insert into #TotalSalesYTDSD -- Company Total    
  select 0,SUM(TotalSales),SUM(TotalCost)			-- #9888  
  from #TotalSalesYTD  
  
   
 -- Total Sales by Category  
 select 'A' as SortOrder,SalesCode,Category,Codedescript,DepartmentCode,Class,  -- 08/09/11  
  0 as Branchno,CAST('ProductSales' as varchar(15)) as SalesType,  
  SUM(TotValue) as TotValue,CAST(0 as float) as ActualPct,SUM(ActualGP) as ActualGP,  
  CAST(0 as float) as ActualGPPct,SUM(YTDSales) as YTDSales,CAST(0 as float) as YTDSalesPct,  
  SUM(YTDGP) as YTDGP,CAST(0 as float) as YTDGPPct,
  SUM(ActualCost) as ActualCost, CAST(0 as MONEY) as YTDCost, CAST(0 as MONEY) as ActualValueMTD,CAST(0 as MONEY) as ActualCostMTD		-- #9888  
 Into #CatTradingData  
 From #TradingData  
 Where SalesCode in('M','W') -- not Service Charge or Discount  
 Group by SalesCode,Category,Codedescript,DepartmentCode,Class  -- 08/09/11  
   
 select 'A' as SortOrder,SalesCode,Category,Codedescript,DepartmentCode,Class,  -- 08/09/11  
  0 as Branchno,CAST('ProductSales' as varchar(15)) as SalesType,  
  SUM(TotValue) as TotValue,CAST(0 as float) as ActualPct,SUM(ActualGP) as ActualGP,  
  CAST(0 as float) as ActualGPPct,SUM(YTDSales) as YTDSales,CAST(0 as float) as YTDSalesPct,  
  SUM(YTDGP) as YTDGP,CAST(0 as float) as YTDGPPct,
  SUM(ActualCost) as ActualCost, SUM(YTDCost) as YTDCost, CAST(0 as MONEY) as ActualValueMTD,CAST(0 as MONEY) as ActualCostMTD		-- #9888  
 Into #CatTradingDataYTD   
 From #TradingDataYTD  
 Where SalesCode in('M','W') -- not Service Charge or Discount or Rebate  
 Group by SalesCode,Category,Codedescript,DepartmentCode,Class  -- 08/09/11
 
  select 'A' as SortOrder,SalesCode,Category,Codedescript,DepartmentCode,Class,					-- #9888 Monthly 
  0 as Branchno,CAST('ProductSales' as varchar(15)) as SalesType,  
  CAST(0 as MONEY) as TotValue,CAST(0 as float) as ActualPct,SUM(ActualGP) as ActualGP,								-- #9888 
  CAST(0 as float) as ActualGPPct,CAST(0 as MONEY) as YTDSales,CAST(0 as float) as YTDSalesPct,						-- #9888 
  SUM(YTDGP) as YTDGP,CAST(0 as float) as YTDGPPct,
  SUM(ActualCost) as ActualCost, SUM(YTDCost) as YTDCost, SUM(ActualValueMTD) as ActualValueMTD,SUM(ActualCostMTD) as ActualCostMTD		-- #9888  
 Into #CatTradingDataMTD		-- #9888 
 From #TradingDataMTD			-- #9888 
 Where SalesCode in('M','W') -- not Service Charge or Discount or Rebate  
 Group by SalesCode,Category,Codedescript,DepartmentCode,Class  -- 08/09/11    
   
 -- Service Charge and Discount  
 Insert Into #CatTradingData  
 --select SortOrder= case when salesCode!='R' then 'C' else 'G' end,  
 select salesCode as SortOrder,  
  SalesCode,Category,Codedescript,DepartmentCode,Class,  -- 08/09/11  
  0 as Branchno,case when SalesCode='S' then CAST('Service Charge' as varchar(15))  -- 26/09/11  
       when SalesCode='D' then CAST('Discounts' as varchar(15))   
       when SalesCode='R' then CAST('Rebates' as varchar(15)) end as SalesType,  
  SUM(TotValue) as TotValue,CAST(0 as float) as ActualPct,SUM(ActualGP) as ActualGP,  
  CAST(0 as float) as ActualGPPct,SUM(YTDSales) as YTDSales,CAST(0 as float) as YTDSalesPct,  
  SUM(YTDGP) as YTDGP,CAST(0 as float) as YTDGPPct,
  SUM(ActualCost) as ActualCost, CAST(0 as MONEY) as YTDCost, CAST(0 as MONEY) as ActualValueMTD,CAST(0 as MONEY) as ActualCostMTD		-- #9888     
 From #TradingData  
 Where SalesCode in('S','D','R') -- Service Charge or Discount or Rebate  
 Group by SalesCode,Category,Codedescript,DepartmentCode,Class  -- 08/09/11  
   
 Insert Into #CatTradingDataYTD  
 --select SortOrder= case when salesCode!='R' then 'C' else 'G' end,  
 select salesCode as SortOrder,  
  SalesCode,Category,Codedescript,DepartmentCode,Class,  -- 08/09/11  
  0 as Branchno,case when SalesCode='S' then CAST('Service Charge' as varchar(15))  -- 26/09/11   
       when SalesCode='D' then CAST('Discounts' as varchar(15))   
       when SalesCode='R' then CAST('Rebates' as varchar(15)) end as SalesType,  
  SUM(TotValue) as TotValue,CAST(0 as float) as ActualPct,SUM(ActualGP) as ActualGP,  
  CAST(0 as float) as ActualGPPct,SUM(YTDSales) as YTDSales,CAST(0 as float) as YTDSalesPct,  
  SUM(YTDGP) as YTDGP,CAST(0 as float) as YTDGPPct,
  SUM(ActualCost) as ActualCost, SUM(YTDCost) as YTDCost, CAST(0 as MONEY) as ActualValueMTD,CAST(0 as MONEY) as ActualCostMTD		-- #9888    
 From #TradingDataYTD  
 Where SalesCode in('S','D','R') -- Service Charge or Discount  
 Group by SalesCode,Category,Codedescript,DepartmentCode,Class  -- 08/09/11
 
 Insert Into #CatTradingDataMTD								-- #9888 Monthly
 --select SortOrder= case when salesCode!='R' then 'C' else 'G' end,  
 select salesCode as SortOrder,  
  SalesCode,Category,Codedescript,DepartmentCode,Class,  -- 08/09/11  
  0 as Branchno,case when SalesCode='S' then CAST('Service Charge' as varchar(15))  -- 26/09/11   
       when SalesCode='D' then CAST('Discounts' as varchar(15))   
       when SalesCode='R' then CAST('Rebates' as varchar(15)) end as SalesType,  
  SUM(TotValue) as TotValue,CAST(0 as float) as ActualPct,SUM(ActualGP) as ActualGP,  
  CAST(0 as float) as ActualGPPct,SUM(YTDSales) as YTDSales,CAST(0 as float) as YTDSalesPct,  
  SUM(YTDGP) as YTDGP,CAST(0 as float) as YTDGPPct,
  SUM(ActualCost) as ActualCost, SUM(YTDCost) as YTDCost, SUM(ActualValueMTD) as ActualValueMTD,SUM(ActualCostMTD) as ActualCostMTD		-- #9888    
 From #TradingDataMTD			-- #9888 
 Where SalesCode in('S','D','R') -- Service Charge or Discount  
 Group by SalesCode,Category,Codedescript,DepartmentCode,Class  -- 08/09/11  
   
    
 -- Update Percentage GP - excl Service Charge & Discounts  
 update #CatTradingData   
  set ActualPct=TotValue/Totalsales*100,ActualGPPct=ActualGP/TotValue*100
 from #CatTradingData c INNER JOIN  #TotalSales t on c.Branchno=t.branchno  
 where SalesCode in('M','W') and TotValue!=0  
   
 update #CatTradingDataYTD   
  set YTDSalesPct=YTDSales/Totalsales*100,YTDGPPct=YTDGP/YTDSales*100
 from #CatTradingDataYTD c INNER JOIN  #TotalSalesYTD t on c.Branchno=t.branchno  
 where SalesCode in('M','W') and YTDSales!=0  
   
 -- Update Percentage GP for Service Charge & Discounts - Inc Service Charge & Discounts  
 update #CatTradingData   
 set ActualPct=TotValue/Totalsales*100,ActualGPPct=ActualGP/TotValue*100
 from #CatTradingData c INNER JOIN  #TotalSalesSD t on c.Branchno=t.branchno  
 where SalesCode in ('S','D','R') and TotValue!=0  
   
 update #CatTradingDataYTD   
  set YTDSalesPct=YTDSales/Totalsales*100,YTDGPPct=YTDGP/YTDSales*100
 from #CatTradingDataYTD c INNER JOIN  #TotalSalesYTDSD t on c.Branchno=t.branchno  
 where SalesCode in ('S','D','R') and YTDSales!=0  
  
  
 -- Total Sales by Branch  
 select 'B' as SortOrder,SalesCode,Category,Codedescript,DepartmentCode,Class,  -- 08/09/11  
  Branchno,CAST('ProductSales' as varchar(15)) as SalesType,  
  SUM(TotValue) as TotValue,CAST(0 as float) as ActualPct,SUM(ActualGP) as ActualGP,  
  CAST(0 as float) as ActualGPPct,SUM(YTDSales) as YTDSales,CAST(0 as float) as YTDSalesPct,  
  SUM(YTDGP) as YTDGP,CAST(0 as float) as YTDGPPct,
  SUM(ActualCost) as ActualCost, SUM(YTDCost) as YTDCost, CAST(0 as MONEY) as ActualValueMTD,CAST(0 as MONEY) as ActualCostMTD		-- #9888       
 Into #BrnTradingData  
 From #TradingData  
 Where SalesCode in('M','W') -- not Service Charge or Discount  
 Group by SalesCode,Category,Codedescript,DepartmentCode,Class,  -- 08/09/11  
     Branchno     
   
 select 'B' as SortOrder,SalesCode,Category,Codedescript,DepartmentCode,Class,  -- 08/09/11  
  Branchno,CAST('ProductSales' as varchar(15)) as SalesType,  
  SUM(TotValue) as TotValue,CAST(0 as float) as ActualPct,SUM(ActualGP) as ActualGP,  
  CAST(0 as float) as ActualGPPct,SUM(YTDSales) as YTDSales,CAST(0 as float) as YTDSalesPct,  
  SUM(YTDGP) as YTDGP,CAST(0 as float) as YTDGPPct,
  SUM(ActualCost) as ActualCost, SUM(YTDCost) as YTDCost, CAST(0 as MONEY) as ActualValueMTD,CAST(0 as MONEY) as ActualCostMTD		-- #9888     
 Into #BrnTradingDataYTD  
 From #TradingDataYTD  
 Where SalesCode in('M','W') -- not Service Charge or Discount  
 Group by SalesCode,Category,Codedescript,DepartmentCode,Class,  -- 08/09/11  
     Branchno 
     
 select 'B' as SortOrder,SalesCode,Category,Codedescript,DepartmentCode,Class,				-- #9888 Monthly 
  Branchno,CAST('ProductSales' as varchar(15)) as SalesType,  
  SUM(TotValue) as TotValue,CAST(0 as float) as ActualPct,SUM(ActualGP) as ActualGP,  
  CAST(0 as float) as ActualGPPct,CAST(0 as MONEY) as YTDSales,CAST(0 as float) as YTDSalesPct,  
  SUM(YTDGP) as YTDGP,CAST(0 as float) as YTDGPPct,
  CAST(0 as MONEY) as ActualCost, SUM(YTDCost) as YTDCost, SUM(ActualValueMTD) as ActualValueMTD,SUM(ActualCostMTD) as ActualCostMTD		-- #9888     
 Into #BrnTradingDataMTD			-- #9888 
 From #TradingDataMTD				-- #9888 
 Where SalesCode in('M','W') -- not Service Charge or Discount  
 Group by SalesCode,Category,Codedescript,DepartmentCode,Class,  -- 08/09/11  
     Branchno  
   
   
 -- Service Charge or Discount  
 Insert Into #BrnTradingData  
 --select SortOrder= case when salesCode!='R' then 'C' else 'G' end,    !!! change  
 select salesCode as SortOrder,  
  SalesCode,Category,Codedescript,DepartmentCode,Class,  -- 08/09/11  
  Branchno,case when SalesCode='S' then CAST('Service Charge' as varchar(15))  -- 26/09/11   
       when SalesCode='D' then CAST('Discounts' as varchar(15))   
       when SalesCode='R' then CAST('Rebates' as varchar(15)) end as SalesType,  
  SUM(TotValue) as TotValue,CAST(0 as float) as ActualPct,SUM(ActualGP) as ActualGP,  
  CAST(0 as float) as ActualGPPct,SUM(YTDSales) as YTDSales,CAST(0 as float) as YTDSalesPct,  
  SUM(YTDGP) as YTDGP,CAST(0 as float) as YTDGPPct,
  SUM(ActualCost) as ActualCost, SUM(YTDCost) as YTDCost, CAST(0 as MONEY) as ActualValueMTD,CAST(0 as MONEY) as ActualCostMTD		-- #9888      
 From #TradingData  
 Where SalesCode in('S','D','R') -- Service Charge or Discount  
 Group by SalesCode,Category,Codedescript,DepartmentCode,Class,  -- 08/09/11  
     Branchno  
   
 Insert Into #BrnTradingDataYTD  
 --select SortOrder= case when salesCode!='R' then 'C' else 'G' end,   !!! change  
 select salesCode as SortOrder,  
  SalesCode,Category,Codedescript,DepartmentCode,Class,  -- 08/09/11  
  Branchno,case when SalesCode='S' then CAST('Service Charge' as varchar(15))  -- 26/09/11   
       when SalesCode='D' then CAST('Discounts' as varchar(15))   
       when SalesCode='R' then CAST('Rebates' as varchar(15)) end as SalesType,  
  SUM(TotValue) as TotValue,CAST(0 as float) as ActualPct,SUM(ActualGP) as ActualGP,  
  CAST(0 as float) as ActualGPPct,SUM(YTDSales) as YTDSales,CAST(0 as float) as YTDSalesPct,  
  SUM(YTDGP) as YTDGP,CAST(0 as float) as YTDGPPct,
  SUM(ActualCost) as ActualCost, SUM(YTDCost) as YTDCost, CAST(0 as MONEY) as ActualValueMTD,CAST(0 as MONEY) as ActualCostMTD		-- #9888      
 From #TradingDataYTD  
 Where SalesCode in('S','D','R') --  Service Charge or Discount  
 Group by SalesCode,Category,Codedescript,DepartmentCode,Class,  -- 08/09/11  
     Branchno 
     
 Insert Into #BrnTradingDataMTD							-- #9888 Monthly 
 --select SortOrder= case when salesCode!='R' then 'C' else 'G' end,   !!! change  
 select salesCode as SortOrder,  
  SalesCode,Category,Codedescript,DepartmentCode,Class,  -- 08/09/11  
  Branchno,case when SalesCode='S' then CAST('Service Charge' as varchar(15))  -- 26/09/11   
       when SalesCode='D' then CAST('Discounts' as varchar(15))   
       when SalesCode='R' then CAST('Rebates' as varchar(15)) end as SalesType,  
  SUM(TotValue) as TotValue,CAST(0 as float) as ActualPct,SUM(ActualGP) as ActualGP,  
  CAST(0 as float) as ActualGPPct,CAST(0 as MONEY) as YTDSales,CAST(0 as float) as YTDSalesPct,  
  SUM(YTDGP) as YTDGP,CAST(0 as float) as YTDGPPct,
  CAST(0 as MONEY) as ActualCost, SUM(YTDCost) as YTDCost, SUM(ActualValueMTD) as ActualValueMTD,SUM(ActualCostMTD) as ActualCostMTD		-- #9888      
 From #TradingDataMTD  
 Where SalesCode in('S','D','R') --  Service Charge or Discount  
 Group by SalesCode,Category,Codedescript,DepartmentCode,Class,  -- 08/09/11  
     Branchno  
   
 -- Update Percentage GP - excl Service Charge & Discounts  
 update #BrnTradingData   
  set ActualPct=TotValue/Totalsales*100,ActualGPPct=ActualGP/TotValue*100  
 from #BrnTradingData c INNER JOIN  #TotalSales t on c.Branchno=t.branchno  
 where SalesCode in('M','W') and TotValue!=0  
   
 update #BrnTradingDataYTD   
  set YTDSalesPct=YTDSales/Totalsales*100,YTDGPPct=YTDGP/YTDSales*100  
 from #BrnTradingDataYTD c INNER JOIN  #TotalSalesYTD t on c.Branchno=t.branchno  
 where SalesCode in('M','W') and YTDSales!=0  
   
 -- Update Percentage GP Service Charge & Discounts - incl Service Charge & Discounts  
 update #BrnTradingData   
  set ActualPct=TotValue/Totalsales*100,ActualGPPct=ActualGP/TotValue*100  
 from #BrnTradingData c INNER JOIN  #TotalSalesSD t on c.Branchno=t.branchno  
 where SalesCode in ('S','D','R') and TotValue!=0  
   
 update #BrnTradingDataYTD   
  set YTDSalesPct=YTDSales/Totalsales*100,YTDGPPct=YTDGP/YTDSales*100  
 from #BrnTradingDataYTD c INNER JOIN  #TotalSalesYTDSD t on c.Branchno=t.branchno  
 where SalesCode in ('S','D','R') and YTDSales!=0  
   
 -- Total Sales by Sales Type (Credit/Cash)  
 select 'F' as SortOrder,SalesCode,CAST('Furniture ' as CHAR(10)) as Category,'TotalSales' as Codedescript, 0  as Branchno,Type as SalesType,  
  SUM(TotValue) as TotValue,CAST(0 as float) as ActualPct,SUM(ActualGP) as ActualGP,  
  CAST(0 as float) as ActualGPPct,SUM(YTDSales) as YTDSales,CAST(0 as float) as YTDSalesPct,  
  SUM(YTDGP) as YTDGP,CAST(0 as float) as YTDGPPct,
  SUM(ActualCost) as ActualCost, SUM(YTDCost) as YTDCost, CAST(0 as MONEY) as ActualValueMTD,CAST(0 as MONEY) as ActualCostMTD		-- #9888     
 Into #TypTradingData  
 From #TradingData   
 where SalesCode in('M','W') and Category='Furniture'  
 Group by SalesCode,Type  
   
 Insert Into #TypTradingData  
 select 'F' as SortOrder,SalesCode,CAST('Electrical' as CHAR(10)) as Category,'TotalSales' as Codedescript, 0  as Branchno,Type as SalesType,  
  SUM(TotValue) as TotValue,CAST(0 as float) as ActualPct,SUM(ActualGP) as ActualGP,  
  CAST(0 as float) as ActualGPPct,SUM(YTDSales) as YTDSales,CAST(0 as float) as YTDSalesPct,  
  SUM(YTDGP) as YTDGP,CAST(0 as float) as YTDGPPct,
  SUM(ActualCost) as ActualCost, SUM(YTDCost) as YTDCost, CAST(0 as MONEY) as ActualValueMTD,CAST(0 as MONEY) as ActualCostMTD		-- #9888      
 From #TradingData  
 where SalesCode in('M','W') and Category in('Electrical','Workstation','Spare Parts') -- Workstation classed as electrical  
 Group by SalesCode,Type  
   
   
 select 'F' as SortOrder,SalesCode,CAST('Furniture' as CHAR(10)) as Category,'TotalSales' as Codedescript,0  as Branchno,Type as SalesType,  
  SUM(TotValue) as TotValue,CAST(0 as float) as ActualPct,SUM(ActualGP) as ActualGP,  
  CAST(0 as float) as ActualGPPct,SUM(YTDSales) as YTDSales,CAST(0 as float) as YTDSalesPct,  
  SUM(YTDGP) as YTDGP,CAST(0 as float) as YTDGPPct,
  SUM(ActualCost) as ActualCost, SUM(YTDCost) as YTDCost, CAST(0 as MONEY) as ActualValueMTD,CAST(0 as MONEY) as ActualCostMTD		-- #9888     
 Into #TypTradingDataYTD    
 From #TradingDataYTD  
 where SalesCode in('M','W') and Category='Furniture'  
 Group by SalesCode,Type  
    
 Insert Into #TypTradingDataYTD   
 select 'F' as SortOrder,SalesCode,CAST('Electrical' as CHAR(10)) as Category,'TotalSales' as Codedescript,0  as Branchno,Type as SalesType,  
  SUM(TotValue) as TotValue,CAST(0 as float) as ActualPct,SUM(ActualGP) as ActualGP,  
  CAST(0 as float) as ActualGPPct,SUM(YTDSales) as YTDSales,CAST(0 as float) as YTDSalesPct,  
  SUM(YTDGP) as YTDGP,CAST(0 as float) as YTDGPPct,
  SUM(ActualCost) as ActualCost, SUM(YTDCost) as YTDCost, CAST(0 as MONEY) as ActualValueMTD,CAST(0 as MONEY) as ActualCostMTD		-- #9888      
 From #TradingDataYTD  
 where SalesCode in('M','W') and Category in('Electrical','Workstation','Spare Parts') -- Workstation classed as electrical  
 Group by SalesCode,Type 
 
 -- #9888 Monthly
 select 'F' as SortOrder,SalesCode,CAST('Furniture' as CHAR(10)) as Category,'TotalSales' as Codedescript,0  as Branchno,Type as SalesType,  
  SUM(TotValue) as TotValue,CAST(0 as float) as ActualPct,SUM(ActualGP) as ActualGP,  
  CAST(0 as float) as ActualGPPct,SUM(YTDSales) as YTDSales,CAST(0 as float) as YTDSalesPct,  
  SUM(YTDGP) as YTDGP,CAST(0 as float) as YTDGPPct,
  SUM(ActualCost) as ActualCost, SUM(YTDCost) as YTDCost, SUM(ActualValueMTD) as ActualValueMTD,SUM(ActualCostMTD) as ActualCostMTD		-- #9888     
 Into #TypTradingDataMTD    
 From #TradingDataMTD  
 where SalesCode in('M','W') and Category='Furniture'  
 Group by SalesCode,Type  
  -- #9888 Monthly  
 Insert Into #TypTradingDataMTD   
 select 'F' as SortOrder,SalesCode,CAST('Electrical' as CHAR(10)) as Category,'TotalSales' as Codedescript,0  as Branchno,Type as SalesType,  
  SUM(TotValue) as TotValue,CAST(0 as float) as ActualPct,SUM(ActualGP) as ActualGP,  
  CAST(0 as float) as ActualGPPct,SUM(YTDSales) as YTDSales,CAST(0 as float) as YTDSalesPct,  
  SUM(YTDGP) as YTDGP,CAST(0 as float) as YTDGPPct,
  CAST(0 as MONEY) as ActualCost, SUM(YTDCost) as YTDCost, SUM(ActualValueMTD) as ActualValueMTD,SUM(ActualCostMTD) as ActualCostMTD		-- #9888      
 From #TradingDataMTD  
 where SalesCode in('M','W') and Category in('Electrical','Workstation','Spare Parts') -- Workstation classed as electrical  
 Group by SalesCode,Type
  
   
 -- Total Sales by Sales Type (Credit/Cash) , Branch   
 select 'F' as SortOrder,SalesCode,CAST('Furniture ' as CHAR(10)) as Category,'TotalSales' as Codedescript,Branchno,Type as SalesType,  
  SUM(TotValue) as TotValue,CAST(0 as float) as ActualPct,SUM(ActualGP) as ActualGP,  
  CAST(0 as float) as ActualGPPct,SUM(YTDSales) as YTDSales,CAST(0 as float) as YTDSalesPct,  
  SUM(YTDGP) as YTDGP,CAST(0 as float) as YTDGPPct,
  SUM(ActualCost) as ActualCost, SUM(YTDCost) as YTDCost, CAST(0 as MONEY) as ActualValueMTD,CAST(0 as MONEY) as ActualCostMTD		-- #9888     
 Into #TypBrnTradingData  
 From #TradingData   
 where SalesCode in('M','W') and Category='Furniture'  
 Group by SalesCode,Type,BranchNo  
   
 Insert Into #TypBrnTradingData  
 select 'F' as SortOrder,SalesCode,CAST('Electrical' as CHAR(10)) as Category,'TotalSales' as Codedescript,Branchno,Type as SalesType,  
  SUM(TotValue) as TotValue,CAST(0 as float) as ActualPct,SUM(ActualGP) as ActualGP,  
  CAST(0 as float) as ActualGPPct,SUM(YTDSales) as YTDSales,CAST(0 as float) as YTDSalesPct,  
  SUM(YTDGP) as YTDGP,CAST(0 as float) as YTDGPPct,
  SUM(ActualCost) as ActualCost, SUM(YTDCost) as YTDCost, CAST(0 as MONEY) as ActualValueMTD,CAST(0 as MONEY) as ActualCostMTD		-- #9888      
 From #TradingData  
 where SalesCode in('M','W') and Category in('Electrical','Workstation','Spare Parts') -- Workstation classed as electrical  
 Group by SalesCode,Type,BranchNo  
   
   
 select 'F' as SortOrder,SalesCode,CAST('Furniture' as CHAR(10)) as Category,'TotalSales' as Codedescript,Branchno,Type as SalesType,  
  SUM(TotValue) as TotValue,CAST(0 as float) as ActualPct,SUM(ActualGP) as ActualGP,  
  CAST(0 as float) as ActualGPPct,SUM(YTDSales) as YTDSales,CAST(0 as float) as YTDSalesPct,  
  SUM(YTDGP) as YTDGP,CAST(0 as float) as YTDGPPct,
  SUM(ActualCost) as ActualCost, SUM(YTDCost) as YTDCost, CAST(0 as MONEY) as ActualValueMTD,CAST(0 as MONEY) as ActualCostMTD		-- #9888     
 Into #TypBrnTradingDataYTD    
 From #TradingDataYTD  
 where SalesCode in('M','W') and Category='Furniture'  
 Group by SalesCode,Type,BranchNo  
    
 Insert Into #TypBrnTradingDataYTD   
 select 'F' as SortOrder,SalesCode,CAST('Electrical' as CHAR(10)) as Category,'TotalSales' as Codedescript,Branchno,Type as SalesType,  
  SUM(TotValue) as TotValue,CAST(0 as float) as ActualPct,SUM(ActualGP) as ActualGP,  
  CAST(0 as float) as ActualGPPct,SUM(YTDSales) as YTDSales,CAST(0 as float) as YTDSalesPct,  
  SUM(YTDGP) as YTDGP,CAST(0 as float) as YTDGPPct,
  SUM(ActualCost) as ActualCost, SUM(YTDCost) as YTDCost, CAST(0 as MONEY) as ActualValueMTD,CAST(0 as MONEY) as ActualCostMTD		-- #9888      
 From #TradingDataYTD  
 where SalesCode in('M','W') and Category in('Electrical','Workstation','Spare Parts') -- Workstation classed as electrical  
 Group by SalesCode,Type,BranchNo 
 
 -- #9888 Monthly
 select 'F' as SortOrder,SalesCode,CAST('Furniture' as CHAR(10)) as Category,'TotalSales' as Codedescript,Branchno,Type as SalesType,  
  SUM(TotValue) as TotValue,CAST(0 as float) as ActualPct,SUM(ActualGP) as ActualGP,  
  CAST(0 as float) as ActualGPPct,CAST(0 as MONEY) as YTDSales,CAST(0 as float) as YTDSalesPct,  
  SUM(YTDGP) as YTDGP,CAST(0 as float) as YTDGPPct,
  CAST(0 as MONEY) as ActualCost, SUM(YTDCost) as YTDCost, SUM(ActualValueMTD) as ActualValueMTD,SUM(ActualCostMTD) as ActualCostMTD		-- #9888     
 Into #TypBrnTradingDataMTD    
 From #TradingDataMTD  
 where SalesCode in('M','W') and Category='Furniture'  
 Group by SalesCode,Type,BranchNo  
 -- #9888 Monthly   
 Insert Into #TypBrnTradingDataMTD   
 select 'F' as SortOrder,SalesCode,CAST('Electrical' as CHAR(10)) as Category,'TotalSales' as Codedescript,Branchno,Type as SalesType,  
  SUM(TotValue) as TotValue,CAST(0 as float) as ActualPct,SUM(ActualGP) as ActualGP,  
  CAST(0 as float) as ActualGPPct,CAST(0 as MONEY) as YTDSales,CAST(0 as float) as YTDSalesPct,  
  SUM(YTDGP) as YTDGP,CAST(0 as float) as YTDGPPct,
  CAST(0 as MONEY) as ActualCost, SUM(YTDCost) as YTDCost, SUM(ActualValueMTD) as ActualValueMTD,SUM(ActualCostMTD) as ActualCostMTD		-- #9888      
 From #TradingDataMTD  
 where SalesCode in('M','W') and Category in('Electrical','Workstation','Spare Parts') -- Workstation classed as electrical  
 Group by SalesCode,Type,BranchNo  
    
   
 -- Total Credit/Cash Diff by Sales Type (Credit/Cash) , Branch   
 select 'G' as SortOrder,SalesCode,CAST('Furniture ' as CHAR(10)) as Category,'Credit/Cash diff' as Codedescript,Branchno,Type as SalesType,  
  SUM(TotValue) as TotValue,CAST(0 as float) as ActualPct,SUM(ActualGP) as ActualGP,  
  CAST(0 as float) as ActualGPPct,SUM(YTDSales) as YTDSales,CAST(0 as float) as YTDSalesPct,  
  SUM(YTDGP) as YTDGP,CAST(0 as float) as YTDGPPct,
  SUM(ActualCost) as ActualCost, SUM(YTDCost) as YTDCost, CAST(0 as MONEY) as ActualValueMTD,CAST(0 as MONEY) as ActualCostMTD		-- #9888     
 Into #DiffTradingData  
 From #TradingData   
 where SalesCode in('F') and Category='Furniture'  
 Group by SalesCode,Type,BranchNo  
   
 Insert Into #DiffTradingData  
 select 'G' as SortOrder,SalesCode,CAST('Electrical' as CHAR(10)) as Category,'Credit/Cash diff' as Codedescript,Branchno,Type as SalesType,  
  SUM(TotValue) as TotValue,CAST(0 as float) as ActualPct,SUM(ActualGP) as ActualGP,  
  CAST(0 as float) as ActualGPPct,SUM(YTDSales) as YTDSales,CAST(0 as float) as YTDSalesPct,  
  SUM(YTDGP) as YTDGP,CAST(0 as float) as YTDGPPct,
  SUM(ActualCost) as ActualCost, SUM(YTDCost) as YTDCost, CAST(0 as MONEY) as ActualValueMTD,CAST(0 as MONEY) as ActualCostMTD		-- #9888      
 From #TradingData  
 where SalesCode in('F') and Category in('Electrical','Workstation') -- Workstation classed as electrical  
 Group by SalesCode,Type,BranchNo  
   
   
 select 'G' as SortOrder,SalesCode,CAST('Furniture' as CHAR(10)) as Category,'Credit/Cash diff' as Codedescript,Branchno,Type as SalesType,  
  SUM(TotValue) as TotValue,CAST(0 as float) as ActualPct,SUM(ActualGP) as ActualGP,  
  CAST(0 as float) as ActualGPPct,SUM(YTDSales) as YTDSales,CAST(0 as float) as YTDSalesPct,  
  SUM(YTDGP) as YTDGP,CAST(0 as float) as YTDGPPct,
  SUM(ActualCost) as ActualCost, SUM(YTDCost) as YTDCost, CAST(0 as MONEY) as ActualValueMTD,CAST(0 as MONEY) as ActualCostMTD		-- #9888     
 Into #DiffTradingDataYTD    
 From #TradingDataYTD  
 where SalesCode in('F') and Category='Furniture'  
 Group by SalesCode,Type,BranchNo  
    
 Insert Into #DiffTradingDataYTD   
 select 'G' as SortOrder,SalesCode,CAST('Electrical' as CHAR(10)) as Category,'Credit/Cash diff' as Codedescript,Branchno,Type as SalesType,  
  SUM(TotValue) as TotValue,CAST(0 as float) as ActualPct,SUM(ActualGP) as ActualGP,  
  CAST(0 as float) as ActualGPPct,SUM(YTDSales) as YTDSales,CAST(0 as float) as YTDSalesPct,  
  SUM(YTDGP) as YTDGP,CAST(0 as float) as YTDGPPct,
  SUM(ActualCost) as ActualCost, SUM(YTDCost) as YTDCost, CAST(0 as MONEY) as ActualValueMTD,CAST(0 as MONEY) as ActualCostMTD		-- #9888      
 From #TradingDataYTD  
 where SalesCode in('F') and Category in('Electrical','Workstation') -- Workstation classed as electrical  
 Group by SalesCode,Type,BranchNo
 
 -- Monthly #9888
 select 'G' as SortOrder,SalesCode,CAST('Furniture' as CHAR(10)) as Category,'Credit/Cash diff' as Codedescript,Branchno,Type as SalesType,  
  SUM(TotValue) as TotValue,CAST(0 as float) as ActualPct,SUM(ActualGP) as ActualGP,  
  CAST(0 as float) as ActualGPPct,CAST(0 as MONEY) as YTDSales,CAST(0 as float) as YTDSalesPct,			-- #9888 
  SUM(YTDGP) as YTDGP,CAST(0 as float) as YTDGPPct,
  CAST(0 as MONEY) as ActualCost, SUM(YTDCost) as YTDCost, SUM(ActualValueMTD) as ActualValueMTD,SUM(ActualCostMTD) as ActualCostMTD		-- #9888     
 Into #DiffTradingDataMTD    
 From #TradingDataMTD  
 where SalesCode in('F') and Category='Furniture'  
 Group by SalesCode,Type,BranchNo  
 -- Monthly #9888   
 Insert Into #DiffTradingDataMTD   
 select 'G' as SortOrder,SalesCode,CAST('Electrical' as CHAR(10)) as Category,'Credit/Cash diff' as Codedescript,Branchno,Type as SalesType,  
  SUM(TotValue) as TotValue,CAST(0 as float) as ActualPct,SUM(ActualGP) as ActualGP,  
  CAST(0 as float) as ActualGPPct,CAST(0 as MONEY) as YTDSales,CAST(0 as float) as YTDSalesPct,		-- #9888 
  SUM(YTDGP) as YTDGP,CAST(0 as float) as YTDGPPct,
  CAST(0 as MONEY) as ActualCost, SUM(YTDCost) as YTDCost, SUM(ActualValueMTD) as ActualValueMTD,SUM(ActualCostMTD) as ActualCostMTD		-- #9888      
 From #TradingDataMTD  
 where SalesCode in('F') and Category in('Electrical','Workstation') -- Workstation classed as electrical  
 Group by SalesCode,Type,BranchNo
   
    
 -- Total Sales by Category/Sales Type  
   
 select 'Furniture ' as Category,Type,  
  SUM(Totvalue) as TotalSales,SUM(ActualCost) as TotalCost				-- #9888 
  into #TypTotalSales from #TradingData   
 where SalesCode in('M','W') and Category='Furniture'  
 Group by Type  
   
 insert into #TypTotalSales  
 select 'Electrical' as Category,Type,  
  SUM(Totvalue) as TotalSales,SUM(ActualCost) as TotalCost				-- #9888 
  from #TradingData   
 where SalesCode in('M','W') and Category in('Electrical','Workstation') -- Workstation classed as electrical  
 Group by Type  
   
 insert into #TypTotalSales  
  select Category,'Total',SUM(TotalSales),SUM(TotalCost)				-- #9888  
  from #TypTotalSales Group by Category  
   
 select 'Furniture ' as Category,Type,  
  SUM(YTDSales) as TotalSales,SUM(YTDCost) as TotalCost				-- #9888  
  into #TypTotalSalesYTD from #TradingDataYTD   
 where SalesCode in('M','W') and Category='Furniture'  
 Group by Type  
   
 insert into #TypTotalSalesYTD  
 select 'Electrical' as Category,Type,  
  SUM(YTDSales) as TotalSales,SUM(YTDCost) as TotalCost				-- #9888  
  from #TradingDataYTD   
 where SalesCode in('M','W') and Category in('Electrical','Workstation') -- Workstation classed as electrical  
 Group by Type  
   
 insert into #TypTotalSalesYTD  
  select Category,'Total',SUM(TotalSales),SUM(TotalCost)			-- #9888 
  from #TypTotalSalesYTD Group by Category
  
 -- Monthly		#9888 
  select 'Furniture ' as Category,Type,  
  SUM(ActualValueMTD) as TotalSales,SUM(ActualCostMTD) as TotalCost				-- #9888 
  into #TypTotalSalesMTD from #TradingDataMTD   
 where SalesCode in('M','W') and Category='Furniture'  
 Group by Type  
   
 insert into #TypTotalSalesMTD  
 select 'Electrical' as Category,Type,  
  SUM(ActualValueMTD) as TotalSales,SUM(ActualCostMTD) as TotalCost				-- #9888   
  from #TradingDataMTD   
 where SalesCode in('M','W') and Category in('Electrical','Workstation') -- Workstation classed as electrical  
 Group by Type  
   
 insert into #TypTotalSalesMTD  
  select Category,'Total',SUM(TotalSales),SUM(TotalCost)			-- #9888
  from #TypTotalSalesMTD Group by Category  
    
    
 -- Total Sales by Category/Sales Type/Branch  
    
 select 'Furniture ' as Category,Type,BranchNo,  
  SUM(Totvalue) as TotalSales,SUM(ActualCost) as TotalCost					-- #9888  
  into #TypBrnTotalSales from #TradingData   
 where SalesCode in('M','W') and Category='Furniture'  
 Group by Type,BranchNo  
   
 insert into #TypBrnTotalSales  
 select 'Electrical' as Category,Type,BranchNo,  
  SUM(Totvalue) as TotalSales,SUM(ActualCost) as TotalCost					-- #9888   
  from #TradingData   
 where SalesCode in('M','W') and Category in('Electrical','Workstation') -- Workstation classed as electrical  
 Group by Type,BranchNo  
   
 insert into #TypBrnTotalSales  
  select Category,'Total',BranchNo,SUM(TotalSales),SUM(TotalCost)			-- #9888  
  from #TypBrnTotalSales Group by Category,BranchNo  
   
 select 'Furniture ' as Category,Type,BranchNo,  
  SUM(YTDSales) as TotalSales,SUM(YTDCost) as TotalCost						-- #9888 
  into #TypBrnTotalSalesYTD from #TradingDataYTD   
 where SalesCode in('M','W') and Category='Furniture'  
 Group by Type,BranchNo  
   
 insert into #TypBrnTotalSalesYTD  
 select 'Electrical' as Category,Type,BranchNo,  
  SUM(YTDSales) as TotalSales,SUM(YTDCost) as TotalCost						-- #9888 
  from #TradingDataYTD   
 where SalesCode in('M','W') and Category in('Electrical','Workstation') -- Workstation classed as electrical  
 Group by Type,BranchNo  
   
 insert into #TypBrnTotalSalesYTD  
  select Category,'Total',BranchNo,SUM(TotalSales),SUM(TotalCost)			-- #9888  
  from #TypBrnTotalSalesYTD Group by Category,BranchNo  
  
 -- Monthly						#9888 
  select 'Furniture ' as Category,Type,BranchNo,  
  SUM(ActualValueMTD) as TotalSales,SUM(ActualCostMTD) as TotalCost				-- #9888 
  into #TypBrnTotalSalesMTD from #TradingDataMTD   
 where SalesCode in('M','W') and Category='Furniture'  
 Group by Type,BranchNo  
   
 insert into #TypBrnTotalSalesMTD  
 select 'Electrical' as Category,Type,BranchNo,  
  SUM(ActualValueMTD) as TotalSales,SUM(ActualCostMTD) as TotalCost				-- #9888 
  from #TradingDataMTD   
 where SalesCode in('M','W') and Category in('Electrical','Workstation') -- Workstation classed as electrical  
 Group by Type,BranchNo  
   
 insert into #TypBrnTotalSalesMTD  
  select Category,'Total',BranchNo,SUM(TotalSales),SUM(TotalCost)			-- #9888 
  from #TypBrnTotalSalesMTD Group by Category,BranchNo 
  
    
 -- Update Percentage GP - excl Service Charge & Discounts  
   
 ------update #TypTradingData   
 ------ set ActualPct=ROUND(TotValue/Totalsales,2)*100.000,ActualGPPct=ROUND(ActualGP/TotValue,2)*100.00  
 ------from #TypTradingData c INNER JOIN  #TypTotalSales t on c.category=t.Category and Type='Total'  
 ------where SalesCode in('M','W') and TotValue!=0  
   
 update #TypTradingData   
  set ActualPct=ROUND(TotValue/Totalsales*100.000,2),ActualGPPct=ROUND(ActualGP/TotValue*100.00,2)  
 from #TypTradingData c INNER JOIN  #TotalSales t on c.branchno=t.branchno   
 where SalesCode in('M','W') and TotValue!=0  
   
 update #TypTradingDataYTD   
  set YTDSalesPct=YTDSales/Totalsales*100.000,YTDGPPct=YTDGP/YTDSales*100.00  
 from #TypTradingDataYTD c INNER JOIN  #TotalSalesYTD t on c.branchno=t.branchno   
 where SalesCode in('M','W') and YTDSales!=0   
   
 -- Branch  
 update #TypBrnTradingData   
  set ActualPct=ROUND(TotValue/Totalsales*100.000,2),ActualGPPct=ROUND(ActualGP/TotValue*100.00,2)  
 from #TypBrnTradingData c INNER JOIN  #TotalSales t on c.branchno=t.branchno   
 where SalesCode in('M','W') and TotValue!=0  
   
 update #TypBrnTradingDataYTD   
  set YTDSalesPct=YTDSales/Totalsales*100.000,YTDGPPct=YTDGP/YTDSales*100.00  
 from #TypBrnTradingDataYTD c INNER JOIN  #TotalSalesYTD t on c.branchno=t.branchno   
 where SalesCode in('M','W') and YTDSales!=0   
   
 ------update #TypTradingDataYTD   
 ------ set YTDSalesPct=YTDSales/Totalsales*100.000,YTDGPPct=YTDGP/YTDSales*100.00  
 ------from #TypTradingDataYTD c INNER JOIN  #TypTotalSalesYTD t on c.category=t.Category and Type='Total'  
 ------where SalesCode in('M','W') and YTDSales!=0   
  
 -- Update YTD tables with Current data  
   
 update #CatTradingDataYTD  
  set Totvalue=cw.TotValue, ActualPct=cw.ActualPct,ActualGp=cw.ActualGP,ActualGPPct=cw.ActualGPPct,
		ActualCost=cw.ActualCost,ActualValueMTD=cm.ActualValueMTD,ActualCostMTD=cm.ActualCostMTD				--#9888  
 From #CatTradingDataYTD y LEFT OUTER JOIN #CatTradingData cw on y.Category=cw.Category and y.Codedescript=cw.Codedescript   
    and y.DepartmentCode=cw.DepartmentCode and y.Class=cw.Class -- 08/09/11
		LEFT OUTER JOIN #CatTradingDataMTD cm on y.Category=cm.Category and y.Codedescript=cm.Codedescript and y.DepartmentCode=cm.DepartmentCode and y.Class=cm.Class --#9888 Monthly      
   
 update #BrnTradingDataYTD  
  set Totvalue=cw.TotValue, ActualPct=cw.ActualPct,ActualGp=cw.ActualGP,ActualGPPct=cw.ActualGPPct,
		ActualCost=cw.ActualCost,ActualValueMTD=cm.ActualValueMTD,ActualCostMTD=cm.ActualCostMTD				--#9888  
 From #BrnTradingDataYTD y LEFT OUTER JOIN #BrnTradingData cw   
   on y.Category=cw.Category and y.Codedescript=cw.Codedescript and y.Branchno=cw.Branchno   
    and y.DepartmentCode=cw.DepartmentCode and y.Class=cw.Class -- 08/09/11
		LEFT OUTER JOIN #BrnTradingDataMTD cm on y.Category=cm.Category and y.Codedescript=cm.Codedescript and y.Branchno=cm.Branchno 
			and y.DepartmentCode=cm.DepartmentCode and y.Class=cm.Class		--#9888 Monthly     
   
 update #TypTradingDataYTD  
  set Totvalue=cw.TotValue, ActualPct=cw.ActualPct,ActualGp=cw.ActualGP,ActualGPPct=cw.ActualGPPct,
		ActualCost=cw.ActualCost,ActualValueMTD=cm.ActualValueMTD,ActualCostMTD=cm.ActualCostMTD				--#9888  
 From #TypTradingDataYTD y LEFT OUTER JOIN #TypTradingData cw   
   on y.Category=cw.Category and y.Codedescript=cw.Codedescript and y.SalesType=cw.SalesType
		LEFT OUTER JOIN #TypTradingDataMTD cm on y.Category=cm.Category and y.Codedescript=cm.Codedescript and y.SalesType=cm.SalesType   --#9888 Monthly   
     
 update #TypBrnTradingDataYTD  
  set Totvalue=cw.TotValue, ActualPct=cw.ActualPct,ActualGp=cw.ActualGP,ActualGPPct=cw.ActualGPPct,
		ActualCost=cw.ActualCost,ActualValueMTD=cm.ActualValueMTD,ActualCostMTD=cm.ActualCostMTD				--#9888  
 From #TypBrnTradingDataYTD y LEFT OUTER JOIN #TypBrnTradingData cw   
   on y.Category=cw.Category and y.Codedescript=cw.Codedescript and y.SalesType=cw.SalesType and y.Branchno=cw.Branchno 
		LEFT OUTER JOIN #TypBrnTradingDataMTD cm on y.Category=cm.Category and y.Codedescript=cm.Codedescript and y.SalesType=cm.SalesType and y.Branchno=cm.Branchno  --#9888 Monthly  
   
 update #DiffTradingDataYTD  
  set Totvalue=cw.TotValue, ActualPct=cw.ActualPct,ActualGp=cw.ActualGP,ActualGPPct=cw.ActualGPPct,
		ActualCost=cw.ActualCost,ActualValueMTD=cm.ActualValueMTD,ActualCostMTD=cm.ActualCostMTD				--#9888  
 From #DiffTradingDataYTD y LEFT OUTER JOIN #DiffTradingData cw   
   on y.Category=cw.Category and y.Codedescript=cw.Codedescript and y.SalesType=cw.SalesType and y.Branchno=cw.Branchno
		LEFT OUTER JOIN #DiffTradingDataMTD cm on y.Category=cm.Category and y.Codedescript=cm.Codedescript and y.SalesType=cm.SalesType and y.Branchno=cm.Branchno		--#9888 Monthly    
     
 -- Stock Value - Only for Current year date   
 if @times=1  
 Begin   
  select 'V' as SalesCode,Category=case  
    when c.category='PCE' then 'Electrical          '  
    when c.category='PCF' then 'Furniture'  
    when c.category='PCW' then 'Workstation'   
    when c.category='PCO' then 'Spare Parts' end ,codedescript,convert(varchar(3), i.category) as DepartmentCode,  
    case when len(convert(varchar(3), i.category)) > 2      -- 14/10/11 Use fact category if len < 2  
      then ISNULL(i.Class,convert(varchar(3), i.category))  
      else convert(varchar(3), i.category) end as Class,   -- 08/09/11  
    Stocklocn,SUM(CAST(ISNULL(q.Stock,0) as float)*ISNULL(p.CostPrice,0)) as ActualValue,CAST(0 as money) as ActualPct  
  into #StockValueBrn  
  from StockInfo i INNER JOIN StockQuantity q on i.ID=q.ID  
      INNER JOIN StockPrice p on i.ID = p.ID and q.StockLocn=p.BranchNo  
      INNER JOIN code c on convert(varchar(3), i.category) = c.code and (c.category  in('PCE','PCF','PCW') ) --or c.category  in('PCO') and codedescript='Spare Parts')    --!!! change  
  Where i.itemtype='S'  
  Group by c.Category,codedescript,convert(varchar(3), i.category),case when len(convert(varchar(3), i.category)) > 2      -- 14/10/11 Use fact category if len < 2  
      then ISNULL(i.Class,convert(varchar(3), i.category))  
      else convert(varchar(3), i.category) end,  -- 08/09/11  
     Stocklocn  
  order by Stocklocn  
    
  select 'V' as SalesCode,Stocklocn,SUM(CAST(ISNULL(q.Stock,0) as float)*p.CostPrice) as ActualValue,CAST(0 as FLOAT) as ActualPct  
  into #StockValueTot  
  from StockInfo i INNER JOIN StockQuantity q on i.ID=q.ID  
      INNER JOIN StockPrice p on i.ID = p.ID and q.StockLocn=p.BranchNo  
      INNER JOIN code c on convert(varchar(3), i.category) = c.code and (c.category  in('PCE','PCF','PCW')) -- or c.category  in('PCO') and codedescript='Spare Parts')    --!!! change   
      --INNER JOIN code c on convert(varchar(3), i.category) = c.code and (c.category  in('PCE','PCF','PCW') or c.category  in('PCO') and codedescript='Spare Parts')    --!!! change   
  Where i.itemtype='S'   
  Group by Stocklocn  
    
  -- Stock Value - Company  
  insert into #StockValueBrn  
  select 'V' as SalesCode,Category,codedescript,DepartmentCode,Class,  -- 08/09/11  
   0,SUM(ActualValue) as ActualValue,CAST(0 as money) as ActualPct  
  from #StockValueBrn  
  Group by Category,codedescript,DepartmentCode,Class  -- 08/09/11  
  order by Category,codedescript  
    
  insert into #StockValueTot  
  select 'V' as SalesCode,0,SUM(ActualValue),CAST(0 as FLOAT) as ActualPct  
  from #StockValueTot  
     
  -- Update Percentages   
  UPDATE  #StockValueBrn  
   set ActualPct=b.ActualValue/t.ActualValue*100  
  From #StockValueBrn b INNER JOIN #StockValueTot t on b.Stocklocn=t.Stocklocn  
  Where t.ActualValue!=0  
    
  select SUM(ActualValue) as StockValue into #CompanyStockValue from #StockValueBrn where stocklocn!=0  
  UPDATE #StockValueTot   
   set ActualPct=ISNULL(ActualValue,0)/c.StockValue*100  
  from #CompanyStockValue c   
    
 End  
 -- merge data from temp tables  
 Truncate table TradingSummary  
   
 Insert into TradingSummary (SortOrder, WeekNo, WeekEndDate, Category, [Product Category], DepartmentCode,Class, --ClassDescr,    -- 08/09/11  
       Branchno, SalesType, ActualValue,   
       ActualPct, ActualGP, ActualGPPct, YTDSales, YTDSalesPct, YTDGP, YTDGPPct,
       ActualCost,YTDCost,ActualValueMTD,ActualCostMTD)			-- #9888		
 select SortOrder,@CurrTradWeek as WeekNo,@TradDateEnd as WeekEndDate,Category,Codedescript as 'Product Category', DepartmentCode,Class,  -- 08/09/11  
   Branchno,SalesType,TotValue as ActualValue,  
   ActualPct,ActualGP,ActualGPPct,YTDSales,YTDSalesPct,YTDGP,YTDGPPct,
   ActualCost,YTDCost,ActualValueMTD,ActualCostMTD			-- #9888		    
 From #CatTradingDataYTD  
 union  
 select SortOrder,@CurrTradWeek as WeekNo,@TradDateEnd as WeekEndDate,Category,Codedescript as 'Product Category', DepartmentCode,Class,  -- 08/09/11  
   Branchno,SalesType,TotValue as ActualValue,  
   ActualPct,ActualGP,ActualGPPct,YTDSales,YTDSalesPct,YTDGP,YTDGPPct,
   ActualCost,YTDCost,ActualValueMTD,ActualCostMTD			-- #9888		   
 From #BrnTradingDataYTD  
 union  
 select SortOrder,@CurrTradWeek as WeekNo,@TradDateEnd as WeekEndDate,Category,Codedescript as 'Product Category', 0,' ',  -- 08/09/11  
   Branchno,SalesType,TotValue as ActualValue,  
   ActualPct,ActualGP,ActualGPPct,YTDSales,YTDSalesPct,YTDGP,YTDGPPct,
   ActualCost,YTDCost,ActualValueMTD,ActualCostMTD			-- #9888		   
 From #TypTradingDataYTD  
 union  
 select SortOrder,@CurrTradWeek as WeekNo,@TradDateEnd as WeekEndDate,Category,Codedescript as 'Product Category', 0,' ',  -- 08/09/11  
   Branchno,SalesType,TotValue as ActualValue,  
   ActualPct,ActualGP,ActualGPPct,YTDSales,YTDSalesPct,YTDGP,YTDGPPct,
   ActualCost,YTDCost,ActualValueMTD,ActualCostMTD			-- #9888		   
 From #TypBrnTradingDataYTD  
 union  
 select SortOrder,@CurrTradWeek as WeekNo,@TradDateEnd as WeekEndDate,Category,Codedescript as 'Product Category', 0,' ',  -- 08/09/11  
   Branchno,SalesType,TotValue as ActualValue,  
   ActualPct,ActualGP,ActualGPPct,YTDSales,YTDSalesPct,YTDGP,YTDGPPct,
   ActualCost,YTDCost,ActualValueMTD,ActualCostMTD			-- #9888		   
 From #DiffTradingDataYTD  
 Order by SortOrder  
   
 -- Insert missing categories for each branch & Company to ensure same number of rows (currently 36)  
 insert into TradingSummary (SortOrder, WeekNo, WeekEndDate, Category, [Product Category], DepartmentCode,Class, --ClassDescr,    -- 08/09/11  
       Branchno, SalesType, ActualValue,   
       ActualPct, ActualGP, ActualGPPct, YTDSales, YTDSalesPct, YTDGP, YTDGPPct,
       ActualCost,YTDCost,ActualValueMTD,ActualCostMTD)			-- #9888	  
   
 select Distinct 'A',null,@TradDateEnd,ProductGroupDescr,DeptDescr,Department as DepartmentCode,Class,--ClassDescr,  -- 08/09/11   
   0 as branchno,'ProductSales',0,0,0,0,0,0,0,0,
   0,0,0,0							-- #9888
 from branch b,code c inner JOIN HierarchyVw h on c.code=h.Department    
 where not exists (select * from TradingSummary where SortOrder='A' and Branchno=0 and h.Department=DepartmentCode   
   and h.Class=Class and Category=ProductGroupDescr)  
 and (c.category  in('PCE','PCF','PCW') ) --or c.category  in('PCO') and codedescript in('Spare Parts'))  -- !!Change  
   
 --and exists(select * from TradingSummary where [Product Category]=c.Codedescript   
 --  and Category=case when c.category='PCE' then 'Electrical'  
 --  when c.category='PCF' then 'Furniture'  
 --  when c.category='PCO' then c.codedescript  
 --  when c.category='PCW' then 'Workstation' end)  
 union  
 --select Distinct 'B',null,@TradDateEnd,ISNULL(ProductGroupDescr,c.codedescript),ISNULL(DeptDescr,''),ISNULL(Department,c.code) as DepartmentCode,ISNULL(Class,''),  -- 12/09/11   
 select Distinct 'B',null,@TradDateEnd,ProductGroupDescr,DeptDescr,Department as DepartmentCode,Class,  -- 12/09/11  
   b.branchno,'ProductSales',0,0,0,0,0,0,0,0,
   0,0,0,0							-- #9888  
 --from branch b,code c Left Outer JOIN HierarchyVw h on c.code=h.Department     -- Category/Dept may not be on Hierarchy  12/09/11  
 from branch b,code c Inner JOIN HierarchyVw h on c.code=h.Department    
 where not exists (select * from TradingSummary where SortOrder='B' and Branchno=b.branchno and h.Department=DepartmentCode   
   and h.Class=Class and Category=ProductGroupDescr)  
 and (c.category  in('PCE','PCF','PCW') ) --or c.category  in('PCO') and codedescript in('Spare Parts'))  -- !!Change  
   
 --and exists(select * from TradingSummary where [Product Category]=c.Codedescript   
 --  and Category=case when c.category='PCE' then 'Electrical'  
 --  when c.category='PCF' then 'Furniture'  
 --  when c.category='PCO' then c.codedescript  
 --  when c.category='PCW' then 'Workstation' end)   
 union  
 select 'F',null,@TradDateEnd,case when c.category='PCE' then 'Electrical'  
   when c.category='PCF' then 'Furniture' end,'TotalSales',0 as DepartmentCode,' ' as Class,  -- 08/09/11   
   b.branchno,'Cash',0,0,0,0,0,0,0,0,
   0,0,0,0							-- #9888 
 from branch b,code c  
 where not exists (select * from TradingSummary where SortOrder='F' and Branchno=b.branchno and [Product Category]='TotalSales' and SalesType='Cash'  
   and Category=case when c.category='PCE' then 'Electrical'  
   when c.category='PCF' then 'Furniture' end)  
 and c.category  in('PCE','PCF')  
 union  
 select 'F',null,@TradDateEnd,case when c.category='PCE' then 'Electrical'  
   when c.category='PCF' then 'Furniture' end,'TotalSales',0 as DepartmentCode,' ' as Class,  -- 08/09/11  
   b.branchno,'Credit',0,0,0,0,0,0,0,0,
   0,0,0,0							-- #9888  
 from branch b,code c  
 where not exists (select * from TradingSummary where SortOrder='F' and Branchno=b.branchno and [Product Category]='TotalSales' and SalesType='Credit'  
   and Category=case when c.category='PCE' then 'Electrical'  
   when c.category='PCF' then 'Furniture' end)  
 and c.category  in('PCE','PCF')  
 union  
 select 'F',null,@TradDateEnd,case when c.category='PCE' then 'Electrical'  
   when c.category='PCF' then 'Furniture' end,'TotalSales',0 as DepartmentCode,' ' as Class,  -- 08/09/11  
   0 as BranchNo,'Cash',0,0,0,0,0,0,0,0,
   0,0,0,0							-- #9888  
 from branch b,code c  
 where not exists (select * from TradingSummary where SortOrder='F' and Branchno=0 and [Product Category]='TotalSales' and SalesType='Cash'  
   and Category=case when c.category='PCE' then 'Electrical'  
   when c.category='PCF' then 'Furniture' end)  
 and c.category  in('PCE','PCF')  
 union  
 select 'F',null,@TradDateEnd,case when c.category='PCE' then 'Electrical'  
   when c.category='PCF' then 'Furniture' end,'TotalSales',0 as DepartmentCode,' ' as Class,  -- 08/09/11  
   0 as BranchNo,'Credit',0,0,0,0,0,0,0,0,
   0,0,0,0							-- #9888 
 from branch b,code c  
 where not exists (select * from TradingSummary where SortOrder='F' and Branchno=0 and [Product Category]='TotalSales' and SalesType='Credit'  
   and Category=case when c.category='PCE' then 'Electrical'  
   when c.category='PCF' then 'Furniture' end)  
 and c.category  in('PCE','PCF')  
 union  
 select 'S',null,@TradDateEnd,'Service Charge','Service Charge',0 as DepartmentCode,' ' as Class,  -- 08/09/11  
   b.branchno,'Service Charge',0,0,0,0,0,0,0,0,
   0,0,0,0							-- #9888  
 from branch b  
 where not exists (select * from TradingSummary where SortOrder='S' and Branchno=b.branchno and [Product Category]='Service Charge')  
 union  
 select 'S',null,@TradDateEnd,'Service Charge','Service Charge',0 as DepartmentCode,' ' as Class,  -- 08/09/11  
   0 as BranchNo,'Service Charge',0,0,0,0,0,0,0,0,
   0,0,0,0							-- #9888  
 where not exists (select * from TradingSummary where SortOrder='S' and Branchno=0 and [Product Category]='Service Charge')  
 union  
 select 'R',null,@TradDateEnd,'Rebates','Rebates',0 as DepartmentCode,' ' as Class,  -- 08/09/11  
   b.branchno,'Rebates',0,0,0,0,0,0,0,0,
   0,0,0,0							-- #9888  
 from branch b  
 where not exists (select * from TradingSummary where SortOrder='R' and Branchno=b.branchno and [Product Category]='Rebates')  
 union  
 select 'R',null,@TradDateEnd,'Rebates','Rebates',0 as DepartmentCode,' ' as Class,  -- 08/09/11  
   0 as BranchNo,'Rebates',0,0,0,0,0,0,0,0,
   0,0,0,0							-- #9888  
 where not exists (select * from TradingSummary where SortOrder='R' and Branchno=0 and [Product Category]='Rebates')  
 union  
 select 'D',null,@TradDateEnd,'Discounts','Discounts',0 as DepartmentCode,' ' as Class,  -- 08/09/11  
   b.branchno,'Discounts',0,0,0,0,0,0,0,0,
   0,0,0,0							-- #9888 
 from branch b  
 where not exists (select * from TradingSummary where SortOrder='D' and Branchno=b.branchno and [Product Category]='Discounts')  
 union  
 select 'D',null,@TradDateEnd,'Discounts','Discounts',0 as DepartmentCode,' ' as Class,  -- 08/09/11  
   0 as BranchNo,'Discounts',0,0,0,0,0,0,0,0,
   0,0,0,0							-- #9888   
 where not exists (select * from TradingSummary where SortOrder='D' and Branchno=0 and [Product Category]='Discounts')  
 union  
 select 'G',null,@TradDateEnd,'Credit/Cash diff','Credit/Cash diff',0 as DepartmentCode,' ' as Class,  -- 08/09/11  
   b.branchno,'Credit/CashDiff',0,0,0,0,0,0,0,0,
   0,0,0,0							-- #9888  
 from branch b  
 where not exists (select * from TradingSummary where SortOrder='G' and Branchno=b.branchno and [Product Category]='Credit/Cash diff')  
 union  
 select 'G',null,@TradDateEnd,'Credit/Cash diff','Credit/Cash diff',0 as DepartmentCode,' ' as Class,  -- 08/09/11  
   0 as BranchNo,'Credit/CashDiff',0,0,0,0,0,0,0,0,			-- diff ???
   0,0,0,0							-- #9888 
 where not exists (select * from TradingSummary where SortOrder='G' and Branchno=0 and [Product Category]='Credit/Cash diff')  
   
 --order by b.branchno  
   
   
 If @Times=1  insert into #TradingSummaryCY select * from TradingSummary  -- Current Year  
  else insert into #TradingSummaryLY select * from TradingSummary   -- Previous Year  
    
 -- drop temp tables for each @Times   
 drop table #TradingData    
 drop table #TotalSales    
 drop table #CatTradingData   
 drop table #BrnTradingData   
 drop table #TypTotalSales   
 drop table #TradingDataYTD   
 drop table #TotalSalesYTD   
 drop table #CatTradingDataYTD   
 drop table #BrnTradingDataYTD   
 drop table #TypTotalSalesYTD   
 drop table #TotalSalesSD    
 drop table #TotalSalesYTDSD    
 drop table #TypTradingData    
 drop table #TypTradingDataYTD   
 drop table #TypBrnTotalSales   
 drop table #TypBrnTotalSalesYTD   
 drop table #TypBrnTradingData   
 drop table #TypBrnTradingDataYTD   
 drop table #DiffTradingData   
 drop table #DiffTradingDataYTD
 drop table #TradingDataMTD					-- #9888
 drop table #CatTradingDataMTD				-- #9888
 drop table #BrnTradingDataMTD				-- #9888
 drop table #TypTotalSalesMTD				-- #9888
 drop table #TypTradingDataMTD				-- #9888
 drop table #DiffTradingDataMTD				-- #9888 
 drop table #TypBrnTotalSalesMTD			-- #9888
 drop table #TypBrnTradingDataMTD			-- #9888    
 --drop table #StockValueBrn   
 --drop table #StockValueTot   
 --drop table #CompanyStockValue  
     
 End    
 -- Select CY and LY & Stock data  for @ReportNo into holding table  
   
 --insert into WTRReport (SortOrder, Category, [Product Category],DepartmentCode,Class,ClassDescr,  -- 08/09/11  
 --    Branchno, BranchName, SalesType, ActualValue, ActualPct, ActualValueLY, Variance, ActualGP,   
 --    ActualGPPct, ActualGPPctLY, YTDSales, YTDSalesPct, YTDSalesLY, YTDVariance, YTDGP, YTDGPPct, YTDGPLY, YTDGPPctLY)    
 insert into WTRReport (SortOrder, Category,   
     [Product Category],DepartmentCode,  
     Class,ClassDescr,  -- 08/09/11  
     Branchno, BranchName,   
     SalesType,   
     ActualValue, ActualValueLY, Variance,   
     ActualGP, ActualGPLY, VarianceGP,  
     YTDSales, YTDSalesLY, YTDVariance,   
     YTDGP, YTDGPLY, YTDGPVariance,
     ActualCost,ActualCostLY,YTDCost,YTDCostLY,ActualValueMTD,ActualCostMTD,MTDSalesLY,MTDCostLY)			-- #9888  
 select ISNULL(cy.SortOrder,ly.SortOrder) as SortOrder,ISNULL(cy.Category,ly.Category) as Category,  
     ISNULL(cy.[Product Category],ly.[Product Category]) as [Product Category],ISNULL(cy.DepartmentCode,ly.DepartmentCode) as DepartmentCode,  
     ISNULL(cy.Class,ly.Class) as Class,ISNULL(h.ClassDescr,'') as ClassDescr,  -- 08/09/11    
     ISNULL(cy.Branchno,ly.Branchno) as BranchNo,case when ISNULL(cy.Branchno,ly.Branchno)=0 then 'Company' else ISNULL(b.BranchName,b2.BranchName) end as BranchName,  
     ISNULL(cy.SalesType,ly.SalesType) as SalesType,  
     ISNULL(cy.ActualValue,0) ,ISNULL(ly.ActualValue,0) as ActualValueLY,ISNULL(cy.ActualValue,0)-ISNULL(ly.ActualValue,0) as Variance,  
     ISNULL(cy.ActualGP,0),ISNULL(ly.ActualGP,0) as ActualGPLY,ISNULL(cy.ActualGP,0)-ISNULL(ly.ActualGP,0) as VarianceGP,  
     ISNULL(cy.YTDSales,0),ISNULL(ly.YTDSales,0) as YTDSalesLY,ISNULL(cy.YTDSales,0)-ISNULL(ly.YTDSales,0) as YTDVariance,  
     ISNULL(cy.YTDGP,0),ISNULL(ly.YTDGP,0) as YTDGPLY,ISNULL(cy.YTDGP,0)-ISNULL(ly.YTDGP,0) as YTDGPVariance,
     ISNULL(cy.ActualCost,0),ISNULL(ly.ActualCost,0)as ActualCostLY,ISNULL(cy.YTDCost,0),ISNULL(ly.YTDCost,0) as YTDCostLY,			-- #9888
     ISNULL(cy.ActualValueMTD,0),ISNULL(cy.ActualCostMTD,0),ISNULL(ly.ActualValueMTD,0) as MTDSalesLY,ISNULL(ly.ActualCostMTD,0) as MTDCostLY		-- #9888
       
 --into WTRReport  
 from #TradingSummaryCY cy Full OUTER JOIN #TradingSummaryLY ly on cy.SortOrder=ly.SortOrder and cy.Branchno=ly.Branchno   
     and cy.Category=ly.Category and cy.[Product Category]=ly.[Product Category] and cy.SalesType=ly.SalesType   
     and cy.DepartmentCode=ly.DepartmentCode and cy.Class=ly.Class  -- 08/09/11  
     LEFT OUTER JOIN Branch b on cy.BranchNo=b.Branchno  
     LEFT OUTER JOIN Branch b2 on ly.BranchNo=b2.Branchno   -- used if no current year data  
     LEFT OUTER JOIN HierarchyVw h on h.Department=cy.DepartmentCode and h.Class=cy.Class   -- 08/09/11  
       
 union -- Stock values - Category  
 select 'V',Category,codedescript as [Product Category],DepartmentCode,vb.Class,ISNULL(h.ClassDescr,'') as ClassDescr,  -- 08/09/11       
    StockLocn as BranchNo,case when StockLocn=0 then 'Company' else BranchName end as BranchName,'Stock Value' as SalesType,  
    ActualValue,0,0,0,0,0,0,0,0,0,0,0,
    0,0,0,0,0,0,0,0				-- #9888
 from #StockValueBrn vb LEFT OUTER JOIN Branch b on StockLocn=b.Branchno  
     LEFT OUTER JOIN HierarchyVw h on h.Department=DepartmentCode and h.Class=vb.Class   -- 08/09/11   
 union -- Stock values - Branch  
 select 'T','Branch' as Category,'Total' as [Product Category],0 as DepartmentCode,' ' as Class,' ' as ClassDescr,  -- 08/09/11  
    StockLocn as BranchNo,case when StockLocn=0 then 'Company' else BranchName end as BranchName,'Stock Value' as SalesType,  
    ActualValue,0,0,0,0,0,0,0,0,0,0,0,
    0,0,0,0,0,0,0,0				-- #9888
 from #StockValueTot LEFT OUTER JOIN Branch b on StockLocn=b.Branchno   
       
 order by cy.Branchno,cy.SortOrder,cy.SalesType,cy.Category,cy.[Product Category],cy.DepartmentCode  
  
 -- set sql for saving report to @ReportName  
 --set @sql='Select * into [' + @ReportName + '] from WTRReport'  
 --execute sp_executesql  @sql  
   
 insert into WTRReportDates  
 Select 'Report Dates:',CONVERT(CHAR(12),@TradDateStartCY),CONVERT(CHAR(12),@TradDateEndCY),CONVERT(CHAR(12),@TradDateStartLY),CONVERT(CHAR(12),@TradDateEndLY)   
  
 -- save Report dates for csv file output  
 --set @sql='Select * into [' + @ReportName + 'Dates] from WTRReportDates'   
 --execute sp_executesql  @sql  
 --   
   
 drop table #StockValueBrn   
 drop table #StockValueTot  
 drop table #CompanyStockValue   
    
 select @ReportName=DtFileName from @wtrdates where id=@ReportNo  
  set @path = '"' +@BCPpath+'\BCP' + '" ' +db_name()+'..WTRReport out ' +  
  'd:\users\default\TSDta' + @ReportName + '.csv ' + '-c -t, -q -Usa -P'  
  print @path
  exec master.dbo.xp_cmdshell @path  
  -- report dates  
  set @path = '"' +@BCPpath+'\BCP' + '" ' +db_name()+'..WTRReportDates' + ' out ' +  
  'd:\users\default\TSDta' + @ReportName+'Dates' + '.csv ' + '-c -t, -q -Usa -P'  
   
  exec master.dbo.xp_cmdshell @path    
    
     -- Export Headings to csv  
  set @path = '"' +@BCPpath+'\BCP' + '" ' + DB_NAME() + '.dbo.TradingSummaryHdr  out ' +    
  'D:\users\default\TSHdr.csv ' + '-c -t, -q -Usa -P'  
   
  exec master.dbo.xp_cmdshell @path   
    
  set @path = '"' +@BCPpath+'\BCP' + '" ' + DB_NAME() + '.dbo.TradingSummaryDatesHdr  out ' +    
  'D:\users\default\TSdatesHdr.csv ' + '-c -t, -q -Usa -P'  
   
  exec master.dbo.xp_cmdshell @path     
     
  -- merge to produce final file    
  declare @statement sqltext  -- varchar(6000)    
  set @statement ='copy D:\users\default\TSdatesHdr.csv + D:\users\default\TSDta' + @ReportName +'Dates.csv + ' +  -- 26/09/11  
   'D:\users\default\TSHdr.csv + D:\users\default\TSDta' + @ReportName +   
   '.csv D:\users\default\WTR' + @ReportName + '.csv '    
  exec master.dbo.xp_cmdshell @statement   
    End  -- if Active  
   
 End -- end of Report loop  

     
 -- set dates to inactive  
 UPDATE wtrdates  
  set dtActive1=0,dtActive2=0,dtActive3=0,dtActive4=0,dtActive5=0  
   
    truncate TABLE WTRReport  
 truncate TABLE WTRReportDates  
  
 return @return  
 
 Go
 
 -- End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End
   