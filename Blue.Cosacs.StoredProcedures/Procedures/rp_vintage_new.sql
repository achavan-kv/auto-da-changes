SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO
--rp_vintage_new '20080101'

IF EXISTS (SELECT * FROM SysObjects
           WHERE id = object_id(N'[dbo].rp_vintage_new') and OBJECTPROPERTY(id, N'IsProcedure') = 1)

BEGIN
    DROP PROCEDURE rp_vintage_new
END
GO
CREATE procedure rp_vintage_new       
@startdate datetime      
as      
BEGIN          
    
         
declare @lastdate datetime            
select @lastdate=MAX(dateadd(month,0,dateadd(hour,12,datestart))) from interfacecontrol            
            
if (select dateadd(month,1,max(currentmonth)) from vintage2)>@lastdate            
begin            
return            
end          
/*********************************************** Testing Only *****************************************/            
            
--delete from vintage2 where Currentmonth>='2009-10-01'            
--delete from accountmonths2 where Currentmonth>='2009-10-01'           
--exec rp_vintage_new '2004-01-01'           
--drop table #accountmonths            
--drop table #vintage            
--drop table fintrans_backup            
--drop table accountmonths2            
--drop table vintage2            
            
IF NOT EXISTS (SELECT * FROM information_schema.tables WHERE table_name = 'accountmonths2')            
create table accountmonths2 (            
OriginatingMonth smalldatetime,  Currentmonth smalldatetime,  acctno varchar(12),            
outstbal money,      currstatus varchar(1),   balancepaid money,             
total_bal money,     balancedue money,    arrears money,             
instalamount money,     FeeTotal money,     agrmttotal money,             
servicechg money,     instalno int,     DelinquencyAmount money,             
intadminter money,     accountstatus varchar(1),  TERMSTYPE VARCHAR(2),            
DelinquencyNoMonths int,   DelinquencyNoDays float,  notfullydelivered varchar(1),             
datefirst datetime,     datelast datetime,    rate float,             
deposit money,      datedel datetime,    FullServiceChg money ,            
ServiceChg_after12mths money ,  FinInstalAmt money ,   Instalno_after12mths smallint ,            
InstalFreq char(1),     Rebatetotal money,    Rebatewithin12mths money,            
Rebateafter12mths money,   Agrmtmths smallint,    Deltot money,            
AgrmtDays smallint,     Totalmonths smallint,   Totalmonths_after12mths smallint,            
Dtnetfirstin varchar(1),   Agrmtyears float,    AgrmtyearsRemain smallint,  -- CR938 jec 01/04/08             
Insincluded smallint,    Inspcent float,     Insamount money,            
InsamountRebated money,    InsamountRebatedwithin12mths money,             
InsamountRebatedafter12mths money, FullRebateDays smallint,  Monthstogo smallint,            
Monthstogo_after12mths smallint, Monthstogofactor money,   Monthstogofactor_after12mths money,            
Totalmonthsfactor money,   Totalmonthsfactor_after12mths money,            
Rule78 char(4),      Calcdone char(1),    Rebatemonthsarrears smallint,            
Insplit smallint,     Insplit_after12mths smallint, instalpredel varchar(1),            
dateacctopen datetime)         
      
IF EXISTS (SELECT * FROM information_schema.tables WHERE table_name = 'accountmonths2')      
 if not exists      
  (select * from sys.columns a inner join sys.objects b      
   on  a.object_id=b.object_id      
  where a.name = 'Repoamt' and b.name='accountmonths2')      
        
  alter table accountmonths2 add repoamt money      
     
IF EXISTS (SELECT * FROM information_schema.tables WHERE table_name = 'accountmonths2')      
 if not exists      
  (select * from sys.columns a inner join sys.objects b      
   on  a.object_id=b.object_id      
  where a.name = 'intadm' and b.name='accountmonths2')      
        
  alter table accountmonths2 add intadm money      
            
IF NOT EXISTS (SELECT * FROM information_schema.tables WHERE table_name = 'vintage2')            
create table vintage2            
(OriginatingMonth smalldatetime,Currentmonth smalldatetime,            
openingbal money, outstandingreceivables money,RestructuredBalance MONEY, AddtoBalance MONEY, PrepaymentCollections MONEY , --AddtoPrepayments MONEY,            
noofaccounts int,instalmentsdue money,collections money,EarnedIntAdm MONEY,othermovements             
money,cr money,curr money,Arrears1to30 money,Arrears31to60 money,Arrears61to90 money,Arrears91to120 money,            
Arrears121to150 money,Arrears151to180 money,Arrears181to360 money,ArrearsSC6 money,ArrearsSC7 money,            
ArrearsSC8 money,settled money, intadminter money,dynamicterm float, dynamicint float,staticterm float, staticint float,           
settlementRebates money, addtorebates MONEY, rebates money,            
addtoprepayments money,BADDEBTWRITEOFF MONEY,Repossessions money ,Insurance money ,recoveries money,            
refunds money,Discounts money, AddTo money,Charges MONEY, Adjustments money,            
grts money,newreceivables money,adddr money,restruccoll money,dpr money, dpy money,Startrunno int, Endrunno int,            
rebcr money,rebcurr money,rebArrears1to30 money,rebArrears31to60 money,rebArrears61to90 money,rebArrears91to120 money,            
rebArrears121to150 money,rebArrears151to180 money,rebArrears181to360 money,rebArrearsSC6 money,rebArrearsSC7 money,            
rebArrearsSC8 money,rebsettled money, rebintadminter money)            
            
            
create table #vintage             
(OriginatingMonth smalldatetime,Currentmonth smalldatetime,            
openingbal money, outstandingreceivables money,RestructuredBalance MONEY, AddtoBalance MONEY, PrepaymentCollections MONEY , --AddtoPrepayments MONEY,            
noofaccounts int,instalmentsdue money,collections money,EarnedIntAdm MONEY,othermovements             
money,cr money,curr money,Arrears1to30 money,Arrears31to60 money,Arrears61to90 money,Arrears91to120 money,            
Arrears121to150 money,Arrears151to180 money,Arrears181to360 money,ArrearsSC6 money,ArrearsSC7 money,            
ArrearsSC8 money,settled money, intadminter money,dynamicterm float, dynamicint float,staticterm float, staticint float,            
settlementRebates money, addtorebates MONEY, rebates money,            
addtoprepayments money,BADDEBTWRITEOFF MONEY,Repossessions money ,Insurance money ,recoveries money,            
refunds money,Discounts money, AddTo money,Charges MONEY, Adjustments money,            
grts money,newreceivables money,adddr money,restruccoll money,dpr money, dpy money,Startrunno int, Endrunno int,            
rebcr money,rebcurr money,rebArrears1to30 money,rebArrears31to60 money,rebArrears61to90 money,rebArrears91to120 money,            
rebArrears121to150 money,rebArrears151to180 money,rebArrears181to360 money,rebArrearsSC6 money,rebArrearsSC7 money,            
rebArrearsSC8 money,rebsettled money, rebintadminter money)            
            
            
create table #accountmonths (            
OriginatingMonth smalldatetime,  Currentmonth smalldatetime,  acctno varchar(12),            
outstbal money,      currstatus varchar(1),   balancepaid money,             
total_bal money,     balancedue money,    arrears money,             
instalamount money,     FeeTotal money,     agrmttotal money,             
servicechg money,     instalno int,     DelinquencyAmount money,             
intadminter money,     accountstatus varchar(1),  TERMSTYPE VARCHAR(2),            
DelinquencyNoMonths int,   DelinquencyNoDays float,  notfullydelivered varchar(1),             
datefirst datetime,     datelast datetime,    rate float,             
deposit money,      datedel datetime,    FullServiceChg money ,            
ServiceChg_after12mths money ,  FinInstalAmt money ,   Instalno_after12mths smallint ,            
InstalFreq char(1),     Rebatetotal money,    Rebatewithin12mths money,            
Rebateafter12mths money,   Agrmtmths smallint,    Deltot money,            
AgrmtDays smallint,     Totalmonths smallint,   Totalmonths_after12mths smallint,            
Dtnetfirstin varchar(1),   Agrmtyears float,    AgrmtyearsRemain smallint,  -- CR938 jec 01/04/08             
Insincluded smallint,    Inspcent float,     Insamount money,            
InsamountRebated money,    InsamountRebatedwithin12mths money,             
InsamountRebatedafter12mths money, FullRebateDays smallint,  Monthstogo smallint,            
Monthstogo_after12mths smallint, Monthstogofactor money,   Monthstogofactor_after12mths money,            
Totalmonthsfactor money,   Totalmonthsfactor_after12mths money,            
Rule78 char(4),      Calcdone char(1),    Rebatemonthsarrears smallint,            
Insplit smallint,     Insplit_after12mths smallint, instalpredel varchar(1),            
dateacctopen datetime,    RepoAmt money, intadm money)           
            
IF EXISTS (SELECT * FROM information_schema.tables WHERE table_name = 'temp_AddtoReceivable')            
drop table temp_AddtoReceivable            
begin            
CREATE TABLE temp_AddtoReceivable (acctno CHAR(12), type CHAR(1), datetran datetime)            
            
end            
            
          
/************** Begin by identifying add to accounts **********************/          
            
INSERT INTO temp_AddtoReceivable (acctno)            
SELECT DISTINCT acctno FROM fintrans            
            
UPDATE temp_AddtoReceivable             
SET [TYPE] = 'A' , datetran=datetrans            
 FROM fintrans f , lineitem l             
WHERE f.acctno= temp_AddtoReceivable.acctno             
AND l.acctno =f.acctno AND f.transtypecode ='ADD'            
AND l.itemno = 'ADDDR' AND F.transvalue >0             
and substring(f.acctno,4,1) not in ('4','5')            
AND EXISTS             
(SELECT * FROM lineitem LA ,STOCKITEM S             
WHERE LA.ACCTNO= L.ACCTNO AND LA.itemid= S.itemid AND LA.STOCKLOCN = S.STOCKLOCN AND             
S.itemtype = 'S' )            
            
            
UPDATE temp_AddtoReceivable SET TYPE = 'F' , datetran=datetrans            
FROM fintrans f , lineitem l             
WHERE f.acctno= temp_AddtoReceivable.acctno             
AND l.acctno =f.acctno AND f.transtypecode ='ADD'            
AND l.itemno = 'ADDCR' AND F.transvalue <0             
and substring(f.acctno,4,1) not in ('4','5')            
AND type is null            
and EXISTS             
(SELECT * FROM lineitem LA ,STOCKITEM S             
WHERE LA.ACCTNO= L.ACCTNO AND LA.itemid= S.itemid AND LA.STOCKLOCN = S.STOCKLOCN AND             
S.itemtype = 'S')            
            
            
DELETE FROM temp_AddtoReceivable WHERE TYPE IS null            
            
/***************************** Get Months ********************/            
             
declare @currentmonth datetime,            
  @nextmonth datetime,             
  @startrunno int,             
  @endrunno int            
            
if ((select count(*) from vintage2)=0)              
set @currentmonth='1900-01-01'            
else             
begin            
select @currentmonth= MAX(currentmonth) from vintage2            
if (@currentmonth>'1900-01-01')              
 set @currentmonth=DATEADD(m,1,@currentmonth)            
else             
select @currentmonth ='2004-01-01'            
end            
            
declare @starttime datetime            
set @starttime=getdate()            
            
set @nextmonth = DATEADD(mm,1,@currentmonth)       
      
         
                
/**************************For Each Current Month ********************/            
while @nextmonth<=@lastdate             
begin            
if @starttime<=dateadd(hh,-1,getdate())       
return            
      
-- Next Month            
            
if (@currentmonth='1900-01-01')            
begin            
 set @nextmonth='2004-01-01'            
 set @startrunno=1            
               
 SELECT @endrunno = MAX(runno)             
 FROM interfacecontrol WHERE interface =             
 'updsmry' AND datestart <dateadd(hour,6,@nextmonth)            
end            
            
else             
begin            
 set @nextmonth=dateadd(m,1,@currentmonth)            
            
 SELECT @startrunno = MAX(runno) +1            
   FROM interfacecontrol WHERE interface = 'updsmry'             
   AND datestart <dateadd(hour,6,@currentmonth)            
               
 SELECT @endrunno = MAX(runno)             
 FROM interfacecontrol WHERE interface =             
 'updsmry' AND datestart <dateadd(hour,6,@nextmonth)            
              
end            
            
           
            
-- get fintrans records            
IF  EXISTS (SELECT * FROM information_schema.tables WHERE table_name = 'fintrans_backup')            
drop table fintrans_backup            
            
            
select f.acctno, f.transtypecode, @Currentmonth as currentmonth, getdate() as originatingmonth ,             
sum(transvalue) as transvalue, datetrans as datetrans, runno as runno            
into fintrans_backup            
from fintrans f            
where (f.runno<0 and datetrans between @currentmonth and @nextmonth             
  and SUBSTRING(acctno,4,1) not in ('4','5'))            
  or (runno between @startrunno and @endrunno            
  and SUBSTRING(acctno,4,1) not in ('4','5'))     
group by f.acctno, f.transtypecode,  datetrans, runno            
            
             
create clustered index ix_fintrans_backup on fintrans_backup(acctno,OriginatingMonth,Currentmonth)            
             
            
print 'update months'            
             
            
update fintrans_backup            
set originatingmonth=            
isnull((select cast(convert(varchar,dateadd(month,0,dateadd(day,-day(x.dateacctopen)            
+1, x.dateacctopen)),102) as datetime) from acct x --where acctno='871000315601'            
where x.acctno=fintrans_backup.acctno            
and dateacctopen>='2004-01-01'),'1900-01-01')            
            
-- Set up vintage months            
insert into #vintage (currentmonth, originatingmonth)             
(select distinct @currentmonth, originatingmonth from vintage2            
where originatingmonth!=@currentmonth            
union all            
select @currentmonth,@currentmonth)            
            
insert into #vintage  (currentmonth, originatingmonth)            
select distinct @currentmonth, originatingmonth            
from fintrans_backup f            
where not exists            
(select 'x' from #vintage v where v.Currentmonth=@currentmonth            
and v.OriginatingMonth=f.originatingmonth)            
            
            
-- opening balance = last months closing balance            
if @currentmonth='1900-01-01'             
update #vintage set openingbal=0            
else            
begin            
if @currentmonth='2004-01-01'            
            
update #vintage            
set openingbal=isnull((select sum(vintage2.outstandingreceivables)            
from vintage2            
where #vintage.OriginatingMonth=vintage2.OriginatingMonth            
and vintage2.Currentmonth='1900-01-01'),0.00)            
else            
update #vintage            
set openingbal=isnull((select sum(vintage2.outstandingreceivables)            
from vintage2            
where #vintage.OriginatingMonth=vintage2.OriginatingMonth    and vintage2.Currentmonth=DATEADD(m,-1,@currentmonth)),0.00)            
end            
            
            
            
print 'GRT'            
            
 update #vintage  set  GRTs= isnull ((select sum(f.transvalue)            
 from fintrans_backup f             
 where f.originatingmonth = #vintage.originatingmonth            
 and f.Currentmonth =  #vintage.Currentmonth            
and  transtypecode  ='GRT'  ),0)            
            
print 'bdw'            
             
            
update #vintage  set   BADDEBTWRITEOFF = isnull ((select sum(isnull(f.transvalue,0))            
 from fintrans_backup f             
 where f.originatingmonth = #vintage.originatingmonth            
 and f.Currentmonth =  #vintage.Currentmonth            
  and transtypecode  ='BDW'),0)            
            
            
print 'recoveries'            
             
            
update #vintage  set   recoveries =             
isnull ((select sum(f.transvalue)            
 from fintrans_backup f             
 where f.originatingmonth = #vintage.originatingmonth            
 and f.Currentmonth =  #vintage.Currentmonth            
and transtypecode in ('DPY','DPR')),0)            
            
            
print 'refunds'            
             
            
update #vintage  set   refunds = isnull ((select sum(f.transvalue)            
 from fintrans_backup f             
 where f.originatingmonth = #vintage.originatingmonth            
 and f.Currentmonth =  #vintage.Currentmonth            
  and transtypecode  ='REF'),0)            
            
            
print '4.discounts'            
             
            
update #vintage  set   discounts = isnull ((select sum(f.transvalue)            
 from fintrans_backup f             
 where f.originatingmonth = #vintage.originatingmonth            
 and f.Currentmonth =  #vintage.Currentmonth            
 and transtypecode in ('DDH','DDC')),0)            
            
print '5.repo'            
             
            
 update #vintage  set   Repossessions = isnull ((select sum(f.transvalue)            
 from fintrans_backup f             
 where f.originatingmonth = #vintage.originatingmonth            
 and f.Currentmonth =  #vintage.Currentmonth            
 and transtypecode in ('REP','RDL')),0)            
            
            
print '6.prepaycoll'            
              
            
UPDATE  #vintage SET PrepaymentCollections =isnull((select sum(f.transvalue)            
from fintrans_backup f             
 where f.originatingmonth = #vintage.originatingmonth            
 and f.Currentmonth =  #vintage.Currentmonth              
and transtypecode  IN ('PAY', 'RET','COR','DDE','DDN','DDR')            
 AND EXISTS (SELECT * FROM fintrans_backup fb WHERE  fb.transtypecode ='REB'            
 AND fb.acctno=f.acctno            
 and fb.Currentmonth =  #vintage.Currentmonth AND fb.transvalue <0)            
 ),0)            
            
            
print '8.collections'            
             
            
update #vintage  set  collections = isnull ((select sum(f.transvalue)            
from fintrans_backup f             
 where f.originatingmonth = #vintage.originatingmonth            
 and f.Currentmonth =  #vintage.Currentmonth              
  and transtypecode in ('PAY', 'RET','COR','DDE','DDN','DDR')),0)            
             
            
print '11.charges'            
             
            
 update #vintage  set   Charges = isnull ((select sum(f.transvalue)            
from fintrans_backup f             
 where f.originatingmonth = #vintage.originatingmonth            
 and f.Currentmonth =  #vintage.Currentmonth             
 and transtypecode in ('FEE','INT','ADM','BNK','DDF')),0)            
             
            
            
update #vintage  set   Insurance = isnull ((select sum(f.transvalue)            
from fintrans_backup f             
 where f.originatingmonth = #vintage.originatingmonth            
 and f.Currentmonth =  #vintage.Currentmonth             
   and transtypecode  ='INS'),0)            
             
            
update #vintage  set  newreceivables = isnull ((select sum(isnull(f.transvalue,0))            
from fintrans_backup f             
 where f.originatingmonth = #vintage.originatingmonth            
 and f.Currentmonth =  #vintage.Currentmonth             
    --and transtypecode  ='DEL'),0) 
    and transtypecode  in('DEL','CLD')),0)      -- #10138      
            
update #vintage  set   Adjustments = isnull ((select sum(f.transvalue)            
from fintrans_backup f             
 where f.originatingmonth = #vintage.originatingmonth            
 and f.Currentmonth =  #vintage.Currentmonth             
    and transtypecode  IN ('ADJ','DAD')),0)            
             
             
update #vintage  set   EarnedIntAdm = isnull ((select sum(f.transvalue)            
from fintrans_backup f             
 where f.originatingmonth = #vintage.originatingmonth            
 and f.Currentmonth =  #vintage.Currentmonth             
    and transtypecode  IN ('INT','ADM')),0)            
            
update #vintage  set   Adddr = isnull ((select sum(f.transvalue)            
from fintrans_backup f             
 where f.originatingmonth = #vintage.originatingmonth            
 and f.Currentmonth =  #vintage.Currentmonth             
    and transtypecode  = 'ADD'),0)            
            
update #vintage  set   rebates = isnull ((select sum(isnull(f.transvalue,0))            
from fintrans_backup f             
 where f.originatingmonth = #vintage.originatingmonth            
 and f.Currentmonth =  #vintage.Currentmonth             
    and transtypecode  = 'REB'),0)            
            
            
UPDATE  #vintage SET AddtoPrepayments =isnull((select sum(f.transvalue)            
from fintrans_backup f             
 where f.originatingmonth = #vintage.originatingmonth            
 and f.Currentmonth =  #vintage.Currentmonth              
  and transtypecode  IN ('ADD')            
AND EXISTS (SELECT * FROM temp_AddtoReceivable fb WHERE  fb.acctno = f.acctno             
AND fb.type='F')),0)            
            
            
 update #vintage  set   SettlementRebates = isnull ((select sum(f.transvalue)            
from fintrans_backup f             
 where f.originatingmonth = #vintage.originatingmonth            
 and f.Currentmonth =  #vintage.Currentmonth             
 and transtypecode  ='REB'             
 AND NOT EXISTS (SELECT * FROM temp_AddtoReceivable t             
WHERE t.acctno = f.acctno and type='F') ),0)            
            
            
update #vintage  set   AddTo = isnull ((select sum(f.transvalue)            
from fintrans_backup f             
 where f.originatingmonth = #vintage.originatingmonth            
 and f.Currentmonth =  #vintage.Currentmonth             
  and transtypecode = ('DEL')            
 AND EXISTS (SELECT * FROM temp_AddtoReceivable t WHERE t.acctno = f.acctno             
and type='A')),0)            
            
            
 update #vintage  set   AddtoRebates = isnull ((select sum(f.transvalue)            
from fintrans_backup f             
 where f.originatingmonth = #vintage.originatingmonth            
 and f.Currentmonth =  #vintage.Currentmonth             
and transtypecode  ='REB'            
 AND EXISTS (SELECT * FROM temp_AddtoReceivable t             
WHERE t.acctno = f.acctno and type='F')),0)            
            
            
  update #vintage  set  AddtoBalance = isnull(( select sum(f.transvalue)            
 from fintrans f            
  where  (runno<0 and datetrans < @nextmonth             
  and SUBSTRING(acctno,4,1) not in ('4','5'))            
   or (runno between 1 and @endrunno            
  and SUBSTRING(acctno,4,1) not in ('4','5')            
 AND EXISTS (SELECT * FROM temp_AddtoReceivable t WHERE f.acctno = t.acctno             
 AND ISNULL(t.TYPE,'')= 'A'))            
 having sum(f.transvalue) >0),0)            
            
update #vintage  set  OTHERMOVEMENTS = isnull ((select sum(f.transvalue)            
from fintrans_backup f             
 where f.originatingmonth = #vintage.originatingmonth            
 and f.Currentmonth =  #vintage.Currentmonth             
  and transtypecode  not in ('BNK','DDF','DDE','DDN','DDR','RDL','DPR','DDH','DDC','ADJ',            
  'DAD','DPY','REP','INS','DAD','FEE','PAY', 'REF', 'RET','COR','INT','ADM','DEL','GRT','BDW','REB','ADD','CLD')),0)         -- #10138   
            
            
/***************** create accountmonths data ********************/            
INSERT INTO #accountmonths            
    (Currentmonth, originatingmonth,acctno,outstbal,currstatus, instalamount,balancedue, [datefirst],             
    datelast ,agrmttotal, servicechg, ServiceChg_after12mths,FullServiceChg ,            
    instalno, Instalno_after12mths , InstalFreq,FinInstalAmt, rate,deposit, datedel, Rule78, dateacctopen,termstype )            
SELECT  @currentmonth,  cast(convert(varchar,dateadd(month,0,dateadd(day,-day(a.dateacctopen)            
+1, a.dateacctopen)),102) as datetime) as originatingmonth,a.acctno,SUM(f.transvalue ),a.currstatus,             
i.instalamount,0, datefirst,datelast, ag.agrmttotal, ag.servicechg, ag.servicechg,ag.servicechg,            
instalno, instalno,instalfreq,i.fininstalamt,0, ag.deposit, datedel,         convert (varchar(4),'78-1'), a.dateacctopen ,a.termstype           
FROM    acct a left outer join instalplan i            
on a.acctno = i.acctno inner join fintrans f  on f.acctno=a.acctno             
left outer join agreement ag on  ag.acctno=a.acctno and ag.agrmtno=1            
where              
 SUBSTRING(a.acctno,4,1) not in ('4','5')            
AND     a.acctno != '000000000000'             
and dateacctopen<@nextmonth            
and ((f.runno <=@endrunno              
and runno<>0         
AND   f.datetrans < @nextmonth) OR            
(f.transtypecode in ('INT','ADM')            
and (runno>@endrunno or runno=0)           
AND   f.datetrans < @nextmonth))            
GROUP BY   cast(convert(varchar,dateadd(month,0,dateadd(day,-day(a.dateacctopen)            
+1, a.dateacctopen)),102) as datetime) ,a.acctno,a.currstatus,             
i.instalamount,datefirst,datelast, ag.agrmttotal, ag.servicechg,             
instalno,instalfreq,i.fininstalamt, ag.deposit, datedel, a.dateacctopen, a.termstype            
            
            
delete from #accountmonths where outstbal between -0.009999 and 0.00999999         
        
            
            
/****************** Update historic data ***********************/            
update #accountmonths set termstype=            
isnull((select max(termstype)            
 from termstypeaudit a            
where @nextmonth >= a.datefrom and             
( a.dateto > @nextmonth or a.dateto='1900-01-01')            
 AND #accountmonths.acctno=a.acctno),termstype)            
             
            
  update #accountmonths set instalpredel=0            
              
  update #accountmonths set instalpredel=1            
 where exists            
 (select instalpredel from termstype             
 where termstype.termstype=#accountmonths.termstype            
 and instalpredel='Y')            
             
             
update #accountmonths set agrmttotal=newagreementtotal,            
servicechg=newservicecharge,            
FullServiceChg=NewServiceCharge,            
ServiceChg_after12mths=NewServiceCharge,            
deposit=newdeposit            
from agreementAudit a            
where a.datechange  = (select MAX(datechange) from agreementAudit x where             
datechange<@nextmonth and x.acctno=a.acctno)            
and a.acctno=#accountmonths.acctno            
and exists (select * from agreementAudit z where datechange<@nextmonth and z.acctno=#accountmonths.acctno)            
            
            
update #accountmonths set agrmttotal=oldagreementtotal,            
servicechg=oldservicecharge,            
FullServiceChg=NewServiceCharge,            
ServiceChg_after12mths=NewServiceCharge,            
deposit=olddeposit            
from agreementAudit a            
where a.datechange  = (select MIN(datechange) from agreementAudit x where             
datechange>=@nextmonth and x.acctno=a.acctno)            
and a.acctno=#accountmonths.acctno            
and exists (select * from agreementAudit z where datechange>=@nextmonth and z.acctno=#accountmonths.acctno)            
and not exists (select * from agreementAudit z2 where datechange<@nextmonth and z2.acctno=#accountmonths.acctno)            
            
            
update #accountmonths set instalno=oldinstalno,Instalno_after12mths=oldinstalno,            
instalamount=oldinstalment,            
fininstalamt=case when (agrmttotal-(oldinstalment*(oldinstalno-1))-deposit)<=0             
then oldinstalment-(agrmttotal-(oldinstalment*(oldinstalno-1))-deposit)            
else (agrmttotal-(oldinstalment*(oldinstalno-1))-deposit)  end,            
datelast= case when newinstalno!=oldinstalno then dateadd(mm,oldinstalno-1+instalpredel,datefirst) else datelast end            
from instalplanaudit a            
where a.datechange  = (select MIN(datechange) from instalplanaudit x where             
datechange>@nextmonth and x.acctno=a.acctno)            
and a.acctno=#accountmonths.acctno            
and exists (select * from instalplanaudit z where datechange>=@nextmonth and z.acctno=#accountmonths.acctno)            
and not exists (select * from instalplanaudit z2 where datechange<@nextmonth and z2.acctno=#accountmonths.acctno)            
            
            
update #accountmonths set instalno=newinstalno,Instalno_after12mths=newinstalno,            
instalamount=newinstalment,            
fininstalamt=case when (agrmttotal-(newinstalment*(newinstalno-1))-deposit)<=0             
then newinstalment-(agrmttotal-(newinstalment*(newinstalno-1))-deposit)            
else (agrmttotal-(newinstalment*(newinstalno-1))-deposit)  end,            
datelast= case when newinstalno!=instalno then dateadd(mm,newinstalno-1+instalpredel,datefirst) else datelast end            
from instalplanaudit a            
where a.datechange  = (select MAX(datechange) from instalplanaudit x where             
datechange<@nextmonth and x.acctno=a.acctno)            
and a.acctno=#accountmonths.acctno            
and exists (select * from instalplanaudit z where datechange<@nextmonth and z.acctno=#accountmonths.acctno)            
and (newinstalment!=instalamount            
or newinstalno!=instalno)            
            
print 'calculating termstype'            
            
            
            
            
update #accountmonths set rate=            
isnull((select max(intrate) from acct a, intratehistory t, proposal p            
where a.acctno=p.acctno and p.ScoringBand=t.band and #accountmonths.acctno=p.acctno            
and #accountmonths.termstype=t.termstype and a.dateacctopen between t.datefrom and ISNULL(t.dateto,getdate())),0)            
            
            
            
update #accountmonths set rate=            
isnull((select min(intrate) from  intratehistory t            
where  #accountmonths.termstype=t.termstype and #accountmonths.acctno=#accountmonths.acctno ),0)            
where rate=0            
            
            
            
--update #accountmonths set [datefirst]=newdatefirst,            
--datelast=isnull(dateadd(mm,instalno-1+instalpredel,newdatefirst),datelast)            
--from instalplan_dateaudit a            
--where a.datechanged  = (select MAX(datechanged) from instalplan_dateaudit x where             
--datechanged<@nextmonth and x.acctno=a.acctno and newdatefirst!='1900-01-01' )            
--and a.acctno=#accountmonths.acctno            
--and exists (select * from instalplan_dateaudit z             
--where datechanged<@nextmonth and z.acctno=#accountmonths.acctno and newdatefirst!='1900-01-01')            
--and newdatefirst!=#accountmonths.datefirst             
            
            
update #accountmonths set [datefirst]=olddatefirst,            
datelast=dateadd(mm,instalno-1+instalpredel,olddatefirst)            
from instalplan_dateaudit a            
where a.datechanged  = (select MIN(datechanged) from instalplan_dateaudit x where             
datechanged>=@nextmonth and x.acctno=a.acctno            
and olddatefirst!='1900-01-01')            
and a.acctno=#accountmonths.acctno            
and exists (select * from instalplan_dateaudit z where datechanged>@nextmonth             
and z.acctno=#accountmonths.acctno            
and olddatefirst!='1900-01-01')            
and not exists (select * from instalplan_dateaudit z2 where datechanged<@nextmonth             
and z2.acctno=#accountmonths.acctno and olddatefirst!='1900-01-01'              
and newdatefirst!='1900-01-01')             
            
/*********************** Arrears **************/             
            
            
UPDATE  #accountmonths            
SET     balancepaid =            
         ISNULL((SELECT sum(f.transvalue)            
                 FROM   Fintrans f            
                 WHERE  f.acctno = #accountmonths.acctno            
     AND f.runno <= @EndRunno --AND f.runno<>0            
     and    f.datetrans < @nextmonth             
                 AND    f.transtypecode NOT IN            
                        ('DEL', 'GRT','ADD','REP','RPO','RDL','CLD')),0)			-- #10138    
            
UPDATE #accountmonths            
SET    deltot = ISNULL((            
SELECT sum(transvalue)            
   from fintrans              
   where acctno = #accountmonths.acctno               
        and transtypecode in (N'DEL', N'GRT', N'ADD', N'CLD')				-- #10138   
        and datetrans<@nextmonth             
        and runno>0            
        and runno<=@endrunno) ,0)            
            
            
UPDATE #accountmonths            
SET    Total_bal = ISNULL((SELECT sum(f.transvalue)            
                           FROM   fintrans  f             
            WHERE  f.acctno = #accountmonths.acctno            
      AND    f.runno <= @endrunno and f.runno!=0 and f.datetrans<=@nextmonth),0)            
            
            
update #accountmonths            
set datedel=dbo.deldate(acctno,@nextmonth,agrmttotal)            
where datedel>@currentmonth            
and agrmttotal*.75<=            
isnull((select sum(transvalue)            
from fintrans f            
where f.acctno=#accountmonths.acctno            
and f.transtypecode in ('DEL','GRT','ADD','CLD')            -- #10138
and datetrans<@nextmonth),0)           
and agrmttotal!=0           
            
update #accountmonths            
set datedel=datefirst, datelast='1900-01-01'            
where isnull(datedel,'1900-01-01')='1900-01-01'            
or datefirst='1900-01-01'            
            
--update #accountmonths  set datedel='1900-01-01', datefirst='1900-01-01', datelast='1900-01-01'            
--where   --total_bal>0 and             
--datefirst!='1900-01-01' and deltot<agrmttotal*.75             
        
          
update #accountmonths            
set datedel=DATEADD(month,-1,datefirst)            
where isnull(datedel,'1900-01-01')='1900-01-01'            
and isnull(datefirst,'1900-01-01')!='1900-01-01'            
            
update #accountmonths            
set notfullydelivered = 'Y'            
where (isnull(datedel,'1900-01-01') = '1900-01-01'           
or datedel>=@nextmonth)          
      
update #accountmonths            
set notfullydelivered = 'N'            
where notfullydelivered is null           
          
       
          
               
UPDATE  #accountmonths            
SET     balancedue = Total_bal - balancepaid            
WHERE   balancedue + balancepaid > Total_bal            
            
UPDATE  #accountmonths            
SET     balancedue = (balancedue + isnull(#accountmonths.deposit,0))            
WHERE   datedel < @nextmonth      -- DSR Changed to end of month            
AND     #accountmonths.deposit > 0            
            
UPDATE  #accountmonths            
SET     balancedue = isnull(balancedue,0) + isnull(#accountmonths.instalamount,0)            
FROM    termstype t            
WHERE    t.termstype = #accountmonths.termstype            
AND     t.instalpredel = 'Y'            
AND     datedel < @nextmonth      -- DSR Changed to end of month            
AND     #accountmonths.deposit <= 0            
            select @nextmonth
UPDATE  #accountmonths            
SET     balancedue =            
            ISNULL(balancedue,0) +            
            ISNULL(instalamount * (1+DATEDIFF(month, datefirst, dateadd(hh,-1,@nextmonth))),0)            
            where datefirst < =@nextmonth  -- DSR Changed to end of month            
            
UPDATE  #accountmonths            
SET     balancedue =            
            ISNULL(balancedue,0) +  ISNULL((SELECT  i.fininstalamt             
                    FROM    instalplan i            
                    WHERE   i.acctno = #accountmonths.acctno),0)            
WHERE #accountmonths.datelast<=dateadd(d,-1,@nextmonth)            
            
                                                    
UPDATE  #accountmonths            
SET     balancedue = agrmttotal            
WHERE  agrmttotal < #accountmonths.balancedue            
and agrmttotal!=0            
            
            
            
UPDATE  #accountmonths            
SET     Arrears = case when isnull(datedel,'1900-01-01')='1900-01-01' or datedel>=@nextmonth   then 0
when datelast<@nextmonth then outstbal
else (ISNULL(BalanceDue,0) + ISNULL(BalancePaid,0)) end -- DSR replaces above line            
            
            
UPDATE  #accountmonths            
SET     FeeTotal = ISNULL((SELECT sum(transvalue)            
                           FROM   fintrans f            
                           WHERE  f.acctno = #accountmonths.acctno            
      AND    f.datetrans < @nextmonth            
                           AND    f.transtypecode = 'FEE'),0)            
            
       
UPDATE  #accountmonths      
SET     RepoAmt = ISNULL((SELECT sum(transvalue)      
    FROM   fintrans f      
                           WHERE  f.acctno = #accountmonths.acctno      
      AND    f.datetrans < @nextmonth      
                           AND    f.transtypecode in ( 'REP','RDL','RPO')),0)      
            
 UPDATE  #accountmonths            
 SET     AccountStatus =            
             ISNULL((SELECT  min(s1.StatusCode)            
                     FROM    Status s1            
                     WHERE   s1.AcctNo = #accountmonths.acctno            
       AND     s1.DateStatChge < @nextmonth            
                     AND     s1.DateStatChge = (SELECT MAX(s2.DateStatChge)            
                                                FROM   Status s2            
                                                WHERE  s2.AcctNo = s1.AcctNo            
                                                AND    s2.DateStatChge < @nextmonth)),0)            
                                                          
                                                update #accountmonths set accountstatus='1'            
                                                where balancepaid>deposit and accountstatus='U'            
            
        
             
/****************** remove int adm */            
            
update #accountmonths set intadminter =             
isnull((select sum(transvalue) from fintrans where transtypecode in ('INT','ADM') and runno<=@endrunno and runno!=0            
and fintrans.acctno=#accountmonths.acctno),0) 

           
update #accountmonths set intadm =             
isnull((select sum(transvalue) from fintrans where transtypecode in ('INT','ADM') and datetrans<=@nextmonth and runno=0            
and fintrans.acctno=#accountmonths.acctno),0)            
            
    
update #accountmonths set total_bal = ISNULL(total_bal,0)            
            
            
            
UPDATE  #accountmonths            
SET     DelinquencyAmount = ISNULL(arrears,0) --+ ISNULL(FeeTotal,0)            
WHERE   #accountmonths.AccountStatus != 'S'    -- DSR            
            
DECLARE @threshold FLOAT            
SELECT @threshold = globdelpcent FROM country            
            
            
UPDATE #accountmonths               
SET DelinquencyAmount = outstbal  where @nextmonth>datelast               
and ISNULL(datelast,'1900-01-01')!='1900-01-01'              
              
UPDATE #accountmonths               
SET DelinquencyAmount = outstbal where DelinquencyAmount > outstbal                   

UPDATE #accountmonths               
SET arrears = outstbal where arrears > outstbal       
            
UPDATE #accountmonths             
SET DelinquencyAmount = 0 WHERE notfullydelivered = 'Y'            
            
update #accountmonths            
set DelinquencyNoDays =0, DelinquencyNoMonths=0            
            
UPDATE  #accountmonths      
SET     DelinquencyNoMonths = isnull(ceiling(((arrears -RepoAmt -intadm-intadminter)/ instalamount)),0), -- DSR Need to round DOWN not to NEAREST      
  DelinquencyNoDays = isnull(cast(((arrears -RepoAmt - intadm-intadminter)/ instalamount)* 30  as int),0)  --  Days in arrears      
WHERE   #accountmonths.AccountStatus != 'S'    -- DSR      
AND     instalamount > 0.01          
            
            
delete from #accountmonths where  total_bal between -0.0099999 and 0.0099999     and DelinquencyAmount+intadminter!=0            
            
/******************** Rebates **************************/            
declare @poRebate     money  ,            
    @poRebateWithin12Mths money   ,            
    @poRebateAfter12Mths  money  ,            
    @return     int     ,            
    @FromDate    datetime ,            
    @FromThresDate   datetime ,            
    @UntilThresDate   datetime ,            
    @RuleChangeDate   datetime ,            
    @RebateDate    datetime ,            
            
 @delpcent  money,            
 @rebpcent  money,            
 @CountryCode  char(1),            
    @Nearest   smallint,            
 @NoCents  smallint,            
 @RebateDate_after12mths datetime,            
 @sqlstr   sqltext,            
    @query_text varchar (1000),            
 @rebatetable  varchar(12),            
    @rule           smallint, --CR888 added            
 @StLuciaRuleChange datetime -- CR938 jec 01/04/08            
            
if (select countrycode from country) = 'L' -- only for St.Lucia            
BEGIN            
if (select value from CountryMaintenance where CodeName='SLRebateCalculationRuleDate') < @StLuciaRuleChange            
 and (select value from CountryMaintenance where CodeName='SLRebateCalculationRuleDate') >= '2007-03-31'            
 set @StLuciaRuleChange=(select value from CountryMaintenance where CodeName='SLRebateCalculationRuleDate')            
else-- set parameter to hard coded date            
 update CountryMaintenance set value='2008-03-31' where codename='SLRebateCalculationRuleDate'            
End            
            
 SELECT @RebPCent    = rebpcent,            
  @CountryCode = countrycode,            
  @NoCents     = nocents,            
  @delpcent    = globdelpcent,            
  @return      = 0            
 FROM country            
            
    select @rule = value from countrymaintenance where codename = 'RebateCalculationRule'            
            
 if (select isnull(@UntilThresDate,'01-jan-1900')) = '01-jan-1900'            
  select @UntilThresDate = convert(datetime,getdate(),111)            
            
 if (select isnull(@RebateDate,'01-jan-1900')) = '01-jan-1900'            
  select @RebateDate = convert(datetime,getdate(),111)            
            
            
--------------------------------            
--Ensure dates have no time part            
--------------------------------            
 select @UntilThresDate = convert(datetime,convert(varchar(11),@UntilThresDate))            
            
             
-------------------------------            
--SET CALCULATION DATE + 7 days            
-------------------------------            
 --set date to run rebate at, rebate run date 7 days after period end, truncated so no time part            
 select @RebateDate = CONVERT(DATETIME,CONVERT(CHAR(11),dateadd(day,-1,@nextmonth),111),111)            
            
 --set date for rebate after 12 months - date in 1 year            
 select @RebateDate_after12mths = CONVERT(DATETIME,CONVERT(CHAR(11),dateadd(year,1,@RebateDate),111),111)            
            
--------------------------------            
update #accountmonths            
set Insplit=0,Insplit_after12mths=0,Totalmonths=0,Totalmonths_after12mths=0,Totalmonthsfactor=0,            
Totalmonthsfactor_after12mths=0,Monthstogo=0,Monthstogo_after12mths=0, Monthstogofactor=0,            
Monthstogofactor_after12mths=0, Inspcent=0, Insamount=0,InsamountRebated=0,InsamountRebatedafter12mths=0,             
InsamountRebatedwithin12mths=0,AgrmtDays=0,Rebateafter12mths=0,Rebatemonthsarrears=0, Rebatetotal=0,             
Rebatewithin12mths=0, Calcdone='N'            
            
            
            
  update r            
  set Insplit = 1            
  from instalmentvariable i,#accountmonths r            
  where i.acctno = r.acctno            
  and @rebatedate <= dateto            
  and instalorder = 1            
            
  update r            
  set Insplit = 2            
  from instalmentvariable i,#accountmonths r            
  where i.acctno = r.acctno            
  and Insplit = 0            
            
  update r            
  set Datefirst = Datefrom,            
   Datelast = Dateto,            
   Instalno = instalmentnumber,            
   Instalamount = instalment,            
   Servicechg = servicecharge            
  from instalmentvariable i,#accountmonths r            
  where i.acctno = r.acctno            
  and Insplit = instalorder            
            
  update r            
  set Insplit_after12mths = 1            
  from instalmentvariable i,#accountmonths r            
  where i.acctno = r.acctno            
  and @RebateDate_after12mths <= dateto            
  and instalorder = 1            
            
  update r            
  set Insplit_after12mths = 2            
  from instalmentvariable i,#accountmonths r            
  where i.acctno = r.acctno            
  and Insplit_after12mths = 0            
            
  update r            
  set Instalno_after12mths = instalmentnumber,            
   Servicechg_after12mths = servicecharge            
  from instalmentvariable i,#accountmonths r            
  where i.acctno = r.acctno            
  and Insplit_after12mths = instalorder            
            
         SELECT @return = @@error            
            
            
   if (select CountryCode from country) = 'M'            
   begin            
           UPDATE #accountmonths            
           SET    Rule78 = '78-2'            
           WHERE  termstype in ('04','05','06')            
    AND    Datedel > @rulechangedate            
             
   end            
   else            
   begin              
       if (select CountryCode from country) in ('N','B','Z','D','G','A','J','K','L','V','T')            
                 begin            
           UPDATE #accountmonths             
           SET    Rule78 = '78+1' --CR900 changed to use rule 78+1 for the Caribbean            
            
           SELECT @return = @@error            
                end            
                else            
                begin            
           UPDATE #accountmonths            
           SET    Rule78 = '78-2'            
           WHERE  DateDel NOT BETWEEN '01-Jan-1900' AND @RuleChangeDate            
          end            
               
                
   end            
            
             
         update #accountmonths            
  set AgrmtMths = InstalNo - 1            
  where InstalFreq = N'M'            
            
            
  update #accountmonths            
  set AgrmtDays = (InstalNo - 1) * 14            
  where InstalFreq = N'F'            
            
      update #accountmonths            
  set AgrmtDays = (InstalNo - 1) * 7  where InstalFreq = N'W'            
            
             
          update #accountmonths            
  set AgrmtMths = (InstalNo - 1) * 6  where InstalFreq = N'B'            
            
         SELECT @return = @@error     
             
  update #accountmonths            
         set DateLast = DATEADD(mm, AgrmtMths, DateFirst)            
         where AgrmtMths != 0            
  and isnull(datelast,'01-jan-1900') <= '01-jan-1910'            
  and isnull(datefirst,'01-jan-1900') > '01-jan-1910' --only set if datefirst is set            
            
              
     update  #accountmonths            
        set  DateLast = DATEADD(dd, AgrmtDays, DateFirst)            
     where  AgrmtMths = 0            
  and  AgrmtDays != 0            
            
            
  update r            
  set    DtNetFirstIn = t.dtnetfirstin,            
         FullRebateDays = t.FullRebateDays            
  FROM   termstypetable t,#accountmonths r            
  WHERE  r.termstype = t.TermsType            
            
 update r            
         set    InsPcent    = I.inspcent,            
                Insincluded = I.insincluded            
--                servpcent   = I.intrate,            
--              servpcent2  = I.intrate2            
         FROM   IntRateHistory I, #accountmonths r            
         WHERE  r.termstype = I.TermsType            
         AND    dateacctopen >= datefrom            
            AND    band in ('','A')             
         AND    (dateacctopen <= I.dateto OR I.dateto = '01-jan-1900')            
            
      DECLARE @intratehistory TABLE (termstype VARCHAR(2),earliest SMALLDATETIME)            
      INSERT INTO @intratehistory(TermsType,earliest)            
  select  termstype, min(datefrom) --as earliest            
  --into #intratehistory            
  from intratehistory            
        where   band in ('','A') --CR806 all bands will have same inspcent. If bands not implemented then will band will be blank. Date from and to of blank bands will not overlap with A,B,C,D bands.            
  group by termstype            
                  
            
      declare @intratehistory2 TABLE(TermsType VARCHAR(2),inspcent FLOAT,intrate FLOAT,intrate2 FLOAT,insincluded SMALLINT)            
      INSERT INTO @intratehistory2 (TermsType,inspcent,intrate,intrate2,insincluded)            
  select  i2.termstype, inspcent, intrate, intrate2, insincluded            
  from @intratehistory i2, intratehistory i            
  where i2.termstype = i.termstype            
  and datefrom = earliest            
        AND    band in ('','A') --CR806 all bands will have same inspcent. If bands not implemented then will band will be blank. Date from and to of blank bands will not overlap with A,B,C,D bands.            
            
         update  r            
         set     InsPcent    = I.inspcent,            
                 Insincluded = I.insincluded            
         FROM    @intratehistory2 I,#accountmonths r            
         WHERE   r.termstype = I.TermsType            
  and (r.InsPcent = -99            
  or r.Insincluded = -99)            
            
  update #accountmonths            
  set    TotalMonths = ISNULL((AgrmtTotal-Deposit-FinInstalAmt)/InstalAmount,0)+1.9,            
         TotalMonths_after12mths = ISNULL((AgrmtTotal-Deposit-FinInstalAmt)/InstalAmount,0)+1.9 --set the same if not split interest            
  where  InstalAmount != 0            
  and    Insplit = 0            
            
            
  --update total months for accounts with split interest rate - use instalno as can't calculate as don't have split agrmttotal            
  update #accountmonths            
  set    TotalMonths = Instalno            
  where  Insplit <> 0            
            
   update #accountmonths            
  set    TotalMonths_after12mths = Instalno_after12mths            
  where  Insplit_after12mths <> 0            
            
  update #accountmonths            
  set    TotalMonths = ((TotalMonths-1)*6)            
  where  InstalFreq = N'B'            
  and    Insplit = 0            
            
            
  update reb             
  set    TotalMonths = TotalMonths + ISNULL(DATEDIFF(mm, DateFirst, DateAcctOpen),0)            
  from  #accountmonths reb where   DateAcctOpen < DateFirst            
  and    InstalFreq = N'B'            
  and    Insplit = 0            
            
  update #accountmonths            
  set    TotalMonths = TotalMonths/2            
  where  InstalFreq = N'F'            
  and    Insplit = 0            
            
             
  update #accountmonths            
  set    TotalMonths = TotalMonths/4            
  where  InstalFreq = N'W'            
  and    Insplit = 0            
            
            
  --update totalmonths again if dtnetfirstin set            
  update #accountmonths            
  set    TotalMonths = TotalMonths - 1,            
         TotalMonths_after12mths = TotalMonths_after12mths - 1            
  where  DtNetFirstIn = N'Y'            
            
  update  #accountmonths            
  set AgrmtYears = CONVERT(FLOAT,TotalMonths)/12,             
   -- This code will give AgrmtYearsRemain = zero when @RebateDate > date of last instalment            
   AgrmtYearsRemain = (DATEDIFF(dd,@RebateDate,DateLast) / 365 +            
        ABS(DATEDIFF(dd,@RebateDate,DateLast) / 365)) / 2 -- CR938 jec 01/04/08            
  where insplit = 0            
            
  update  r            
  set AgrmtYears = CONVERT(FLOAT,Instalno)/12             
  from #accountmonths r            
  where insplit <> 0            
              
            
             
             
  update  #accountmonths            
  set     InsAmount = round((convert(float,AgrmtTotal) * convert(float,AgrmtYears) * (convert(float,InsPCent) / 100)) ,2)            
  where insincluded = 1            
            
             
   update #accountmonths            
       set    ServiceChg = (ServiceChg - InsAmount)            
            
  update #accountmonths            
  set rebatetotal = fullservicechg,            
   calcdone = 'Y'            
  where FullRebateDays > 0            
  and DATEDIFF(dd,ISNULL(datedel,'01-jan-1900'),dateadd(day,-7,@RebateDate)) <= FullRebateDays            
   and isnull(DateDel,'01-jan-1900') >= '01-jan-1910'            
             
  UPDATE #accountmonths            
 SET    MonthsToGo = (select case            
         when   day(@RebateDate) <= day(Datelast)            
         then   datediff(month,@RebateDate,Datelast)            
         else   datediff(month,@RebateDate,Datelast) - 1            
         end)            
            
            
          update  #accountmonths            
         set  rebatetotal = 0,            
    rebatewithin12mths = 0,            
    rebateafter12mths = 0,            
    calcdone = 'Y'            
   where MonthsToGo <= 0            
   and     calcdone = 'N'            
   and Insplit <> 1 --don't make rebate zero if in split 1 as rebate=servicechg until in 2nd split            
            
             
   --update MonthsToGo again if bigger than totalmonths            
   IF (@CountryCode = N'S' OR @CountryCode= N'H' OR @CountryCode= N'I' OR @CountryCode= N'M' OR @CountryCode= N'C')            
  begin            
      update  #accountmonths            
      set     MonthsToGo = TotalMonths - 1            
      where   MonthsToGo > TotalMonths --remove equals sign            
      and     calcdone = 'N'            
        end            
  else            
  begin            
      update  #accountmonths            
      set     MonthsToGo = TotalMonths - 1            
      where   MonthsToGo >= TotalMonths            
      and     calcdone = 'N'            
        end            
            
  UPDATE  #accountmonths            
  SET     MonthsToGo_after12mths = MonthsToGo - 12            
  WHERE MonthsToGo > 12            
            
             
  UPDATE r            
  SET    MonthsToGo_after12mths = (select case            
         when   day(@RebateDate_after12mths) <= day(Dateto)            
         then   datediff(month,@RebateDate_after12mths,Dateto)            
         else   datediff(month,@RebateDate_after12mths,Dateto) - 1            
         end)            
  from instalmentvariable i,#accountmonths r            
  where i.acctno = r.acctno            
  and insplit = 1            
  and insplit_after12mths = 2            
  and instalorder = 2 --datelast for 2nd split            
            
            
  UPDATE  #accountmonths            
  SET     MonthsToGo_after12mths = 0            
  where MonthsToGo_after12mths < 0            
            
            
  update  #accountmonths            
  set  MonthsToGoFactor = (MonthsToGo - ((left(right(rule78,2),2)*-1)-1)) * (MonthsToGo - (left(right(rule78,2),2)*-1))--, --CR900            
  where   calcdone = 'N'            
   and rule78 !='Term'  -- Update all rebates where rule78 not 'Term' CR938 jec 01/04/08            
            
            
             
  update  #accountmonths            
--  set  MonthsToGoFactor_after12mths = (MonthsToGo_after12mths - ((left(right(rule78,2),1))-1)) * (MonthsToGo_after12mths - (left(right(rule78,2),1)))            
  set  MonthsToGoFactor_after12mths = (MonthsToGo_after12mths - ((left(right(rule78,2),2)*-1)-1)) * (MonthsToGo_after12mths - (left(right(rule78,2),2)*-1))--, --CR900            
  where   calcdone = 'N'            
  and     MonthsToGo_after12mths > 0 --needed to avoid incorrect results            
  and rule78 !='Term'  -- Update all rebates where rule78 not 'Term' CR938 jec 01/04/08            
            
  update #accountmonths            
  set  TotalMonthsFactor = TotalMonths * (TotalMonths + 1),            
   TotalMonthsFactor_after12mths = TotalMonths_after12mths * (TotalMonths_after12mths + 1)            
            
             
  update #accountmonths            
  set rebatetotal = ((ServiceChg * MonthsToGoFactor / TotalMonthsFactor) * @RebPCent / 100) + InsamountRebated,  --CR938 jec 01/04/08            
   calcdone = 'P' --may need to redo for split accounts            
  where totalmonthsfactor > 0            
  and calcdone = 'N'            
            
             
  update #accountmonths            
  set rebateafter12mths = ((ServiceChg_After12mths * MonthsToGoFactor_after12mths / TotalMonthsFactor_after12mths)             
       * @RebPCent / 100) + InsamountRebatedafter12mths  --CR938 jec 01/04/08            
  where totalmonthsfactor_after12mths > 0            
  and calcdone = 'P' --only update accts that were set above            
            
            
  --find accts still in first split and calc rebate < servicecharge for 2nd split            
      DECLARE @rebates_service TABLE(acctno CHAR(12) PRIMARY KEY,servicechgfor2ndsplit MONEY)            
      INSERT INTO @rebates_service(acctno,servicechgfor2ndsplit)            
  select r.acctno, servicecharge --as servicechgfor2ndsplit            
  --into #rebates_service            
  from  instalmentvariable i, #accountmonths r            
  where  Insplit = 1            
  and i.acctno = r.acctno        
  and rebatetotal < servicecharge            
  and Instalorder = 2            
  and calcdone = 'P' --don't want accounts that have already been set as full service charge or set to zero as invalid for rebate            
            
             
  --update rebate            
  update ra            
  set rebatetotal = servicechgfor2ndsplit,            
   calcdone = 'S' --may need to update after12tmhs still            
  from @rebates_service r,#accountmonths ra            
  where   ra.acctno = r.acctno            
  and calcdone = 'P'            
            
      DECLARE @rebates_service_after12mths TABLE (acctno CHAR(12) PRIMARY KEY,servicechgfor2ndsplit MONEY)            
      INSERT INTO @rebates_service_after12mths(acctno,servicechgfor2ndsplit)            
  select r.acctno, servicecharge as servicechgfor2ndsplit            
  --into #rebates_service_after12mths            
  from  instalmentvariable i, #accountmonths r            
  where  Insplit_after12mths = 1            
  and i.acctno = r.acctno            
  and r.rebateafter12mths < servicecharge            
  and Instalorder = 2            
  and calcdone = 'S'             
            
            
  update reb            
  set rebateafter12mths = servicechgfor2ndsplit,            
   calcdone = 'Y'            
  from @rebates_service_after12mths r,#accountmonths reb            
  where   reb.acctno = r.acctno            
            
  SELECT @return = @@error            
            
            
            
  IF (@CountryCode = N'I' OR @CountryCode= N'C')            
  begin            
   SELECT @Nearest = 100            
            
  end            
  ELSE IF @NoCents = 1            
  begin            
   SELECT @Nearest = 1            
            
  end            
        
             
     IF (@Nearest = 1) --no cents is set            
  BEGIN            
         UPDATE  #accountmonths            
    SET  rebatetotal = FLOOR(rebatetotal), --Round down to nearest 1            
    rebateafter12mths = FLOOR(rebateafter12mths),            
    InsamountRebated = FLOOR(InsamountRebated),            
    InsamountRebatedafter12mths = FLOOR(InsamountRebatedafter12mths) -- CR938 jec 02/04/08            
            
  END            
     ELSE IF (@Nearest = 100) --Large currency, Ind or Mad            
     BEGIN            
         UPDATE  #accountmonths            
    SET  rebatetotal = ROUND(floor(rebatetotal),-2,1), -- Floor then round to nearest 100            
    rebateafter12mths = ROUND(floor(rebateafter12mths),-2,1),            
    InsamountRebated = ROUND(FLOOR(InsamountRebated),-2,1),            
    InsamountRebatedafter12mths = ROUND(FLOOR(InsamountRebatedafter12mths),-2,1) -- CR938 jec 02/04/08            
            
            
        END            
  else --Always round to two decimal places max precision at least            
  begin            
         UPDATE  #accountmonths            
    SET  rebatetotal = ROUND(rebatetotal,2), --2 decimal places            
    rebateafter12mths = ROUND(rebateafter12mths,2),            
    InsamountRebated = ROUND(InsamountRebated,2),            
    InsamountRebatedafter12mths = ROUND(InsamountRebatedafter12mths,2) -- CR938 jec 02/04/08            
            
            
  end            
            
            
        UPDATE  #accountmonths        
  SET  rebatetotal = FullServiceChg, Calcdone='F'            
     WHERE   rebatetotal > FullServiceChg            
        and    FullServiceChg > 0            
            
     SELECT @return = @@error            
            
            
  update #accountmonths            
  set rebateafter12mths = 0            
  where rebateafter12mths < 0            
            
  update #accountmonths            
  set rebatewithin12mths = rebatetotal - rebateafter12mths,            
   InsamountRebatedWithin12mths = InsamountRebated - InsamountRebatedafter12mths -- CR938 jec 01/04/08            
            
             
  update #accountmonths            
  set Rebatemonthsarrears = isnull(cast(0.5+(Arrears/InstalAmount) as integer),0)            
  WHERE  cast(0.5+(Arrears/InstalAmount) as float) < 1000            
  AND  InstalAmount > 1            
            
 update #accountmonths set Rebateafter12mths=0, Rebatewithin12mths=0, Rebatetotal=0, Calcdone='Z'            
 where outstbal<=1 or accountstatus='S'            
            
         TRUNCATE TABLE [del_thres_reached_before]              
                     
INSERT INTO [del_thres_reached_before] (acctno,deltotal,DelThresDate)              
   select  b.acctno, sum(transvalue) as deltotal, max(b.datetrans)               
--   into    dbo.del_thres_reached_before              
   from    fintrans b, country c, #accountmonths a              
   where   b.acctno = a.acctno              
   and  b.transtypecode in ('DEL','GRT','REP','ADD','RDL','RPO','CLD')           -- #10138   
   and datetrans<@nextmonth            
   and DateAcctOpen < @nextmonth             
   and     substring(b.acctno,4,1) in ('0','1','2','3','6','7','8','9')               
   group by b.acctno, a.agrmttotal, c.globdelpcent              
   having ((sum(transvalue)) >= (a.agrmttotal * (c.globdelpcent/100)))              
               
   update #accountmonths            
   set rebatetotal=0 where not exists            
   (select acctno from [del_thres_reached_before]            
   where #accountmonths.acctno=[del_thres_reached_before].acctno)            
            
insert into #vintage  (currentmonth, originatingmonth)            
select distinct @currentmonth, originatingmonth            
from #accountmonths f            
where not exists            
(select 'x' from #vintage v where v.Currentmonth=@currentmonth            
and v.OriginatingMonth=f.originatingmonth)            
            
            
update #vintage            
set outstandingreceivables=isnull(openingbal,0)+newreceivables+            
collections+Discounts+recoveries+Repossessions+BADDEBTWRITEOFF+            
Insurance+refunds+charges+grts+Adjustments+othermovements+rebates+adddr            
            
if @currentmonth='1900-01-01'            
set @currentmonth='2004-01-01'            
else            
begin      
 set @currentmonth=DATEADD(m,1,@currentmonth)            
 set @nextmonth = DATEADD(m, 1, @currentmonth)      
END      
            
update #accountmonths            
set delinquencynodays=0 where AccountStatus in ('0') or instalamount<=1            
            
            
            
update #vintage            
set intadminter =isnull(( select sum(intadminter)            
from #accountmonths f            
where  f.Currentmonth =  #vintage.Currentmonth            
and f.originatingmonth =  #vintage.originatingmonth ),0)            
            
update #vintage            
set Arrears1to30 =isnull(( select sum(Total_bal-intadminter)            
from #accountmonths f            
where  f.Currentmonth =  #vintage.Currentmonth            
and f.originatingmonth =  #vintage.originatingmonth            
and f.accountstatus!='S'            
and accountstatus in ('1','2','3','4','5','6','7','8','9')            
and floor(f.delinquencynodays) between 1 and 30 and total_bal>0              
and total_bal-intadminter>0 and notfullydelivered='N'),0)            
             
update #vintage            
set rebArrears1to30 =isnull(( select sum(Rebatetotal)            
from #accountmonths f            
where f.Currentmonth =  #vintage.Currentmonth            
and f.originatingmonth =  #vintage.originatingmonth            
and f.accountstatus!='S'            
and accountstatus in ('1','2','3','4','5','6','7','8','9')            
and floor(f.delinquencynodays) between 1 and 30 and (total_bal)>0              
and total_bal-intadminter>0 and notfullydelivered='N'),0)            
            
update #vintage            
set Arrears31to60 =isnull(( select sum(Total_bal-intadminter)            
from #accountmonths f            
where  f.Currentmonth =  #vintage.Currentmonth            
and f.originatingmonth =  #vintage.originatingmonth            
and f.accountstatus!='S'            
and accountstatus in ('1','2','3','4','5','6','7','8','9')            
and floor(f.delinquencynodays) between 31 and 60 and total_bal>0               
and total_bal-intadminter>0 and notfullydelivered='N'),0)            
             
             
update #vintage            
set RebArrears31to60 =isnull(( select sum(Rebatetotal)            
from #accountmonths f            
where  f.Currentmonth =  #vintage.Currentmonth            
and f.originatingmonth =  #vintage.originatingmonth            
and f.accountstatus!='S'            
and accountstatus in ('1','2','3','4','5','6','7','8','9')            
and floor(f.delinquencynodays) between 31 and 60 and total_bal>0              
and total_bal-intadminter>0 and notfullydelivered='N' ),0)            
            
   update #vintage            
   set Arrears61to90 =isnull(( select sum(Total_bal-intadminter)            
from #accountmonths f            
where  f.Currentmonth =  #vintage.Currentmonth            
and f.originatingmonth =  #vintage.originatingmonth            
and f.accountstatus!='S'            
and accountstatus in ('1','2','3','4','5','6','7','8','9')            
and floor(f.delinquencynodays) between 61 and 90 and total_bal>0               
and total_bal-intadminter>0 and notfullydelivered='N'),0)            
            
    update #vintage            
   set rebArrears61to90 =isnull(( select sum(rebatetotal)            
from #accountmonths f            
where  f.Currentmonth =  #vintage.Currentmonth            
and f.originatingmonth =  #vintage.originatingmonth            
and f.accountstatus!='S'            
and accountstatus in ('1','2','3','4','5','6','7','8','9')            
and floor(f.delinquencynodays) between 61 and 90 and total_bal>0              
and total_bal-intadminter>0 and notfullydelivered='N' ),0)            
            
   update #vintage            
   set Arrears91to120 =isnull(( select sum(Total_bal-intadminter)            
from #accountmonths f            
where  f.Currentmonth =  #vintage.Currentmonth            
and f.originatingmonth =  #vintage.originatingmonth            
and f.accountstatus!='S'            
and accountstatus in ('1','2','3','4','5','6','7','8','9')            
and floor(f.delinquencynodays) between 91 and 120 and total_bal>0              
and total_bal-intadminter>0 and notfullydelivered='N' ),0)            
             
            
   update #vintage            
   set rebArrears91to120 =isnull(( select sum(rebatetotal)            
from #accountmonths f            
where  f.Currentmonth =  #vintage.Currentmonth            
and f.originatingmonth =  #vintage.originatingmonth            
and f.accountstatus!='S'            
and accountstatus in ('1','2','3','4','5','6','7','8','9')            
and floor(f.delinquencynodays) between 91 and 120 and total_bal>0              
and total_bal-intadminter>0 and notfullydelivered='N'),0)            
             
   update #vintage            
   set Arrears121to150 =isnull(( select sum(Total_bal-intadminter)            
from #accountmonths f            
where  f.Currentmonth =  #vintage.Currentmonth            
and f.originatingmonth =  #vintage.originatingmonth            
and f.accountstatus!='S'            
and accountstatus in ('1','2','3','4','5','6','7','8','9')            
and floor(f.delinquencynodays) between 121 and 150 and total_bal>0              
and total_bal-intadminter>0 and notfullydelivered='N'),0)            
             
             
   update #vintage            
   set rebArrears121to150 =isnull(( select sum(Rebatetotal)            
from #accountmonths f            
where  f.Currentmonth =  #vintage.Currentmonth            
and f.originatingmonth =  #vintage.originatingmonth            
and f.accountstatus!='S'            
and accountstatus in ('1','2','3','4','5','6','7','8','9')            
and floor(f.delinquencynodays) between 121 and 150 and total_bal>0              
and total_bal-intadminter>0 and notfullydelivered='N' ),0)            
             
   update #vintage            
   set Arrears151to180=isnull(( select sum(Total_bal-intadminter)            
from #accountmonths f            
where  f.Currentmonth =  #vintage.Currentmonth            
and f.originatingmonth =  #vintage.originatingmonth            
and f.accountstatus!='S'            
and accountstatus in ('1','2','3','4','5','6','7','8','9')            
and floor(f.delinquencynodays) between 151 and 180 and total_bal>0             
and total_bal-intadminter>0  and notfullydelivered='N') ,0)            
             
    update #vintage            
   set rebArrears151to180=isnull(( select sum(rebatetotal)            
from #accountmonths f            
where  f.Currentmonth =  #vintage.Currentmonth         
and f.originatingmonth =  #vintage.originatingmonth            
and f.accountstatus!='S'            
and accountstatus in ('1','2','3','4','5','6','7','8','9')            
and floor(f.delinquencynodays) between 151 and 180 and total_bal>0             
and total_bal-intadminter>0  and notfullydelivered='N') ,0)            
              
   update #vintage            
   set Arrears181to360 =isnull(( select sum(Total_bal-intadminter)            
from #accountmonths f            
where  f.Currentmonth =  #vintage.Currentmonth            
and f.originatingmonth =  #vintage.originatingmonth            
and f.accountstatus!='S'            
and accountstatus in ('1','2','3','4','5','6','7','8','9')            
and floor(f.delinquencynodays) >= 181 and total_bal>0             
and total_bal-intadminter>0  and notfullydelivered='N' ),0)            
             
   update #vintage            
   set rebArrears181to360 =isnull(( select sum(rebatetotal)            
from #accountmonths f            
where  f.Currentmonth =  #vintage.Currentmonth            
and f.originatingmonth =  #vintage.originatingmonth            
and f.accountstatus!='S'            
and accountstatus in ('1','2','3','4','5','6','7','8','9')            
and floor(f.delinquencynodays) >= 181  and total_bal>0             
and total_bal-intadminter>0  and notfullydelivered='N'),0)            
             
            
update #vintage            
set settled = isnull(( select sum(Total_bal-intadminter)            
from #accountmonths f            
where  f.Currentmonth =  #vintage.Currentmonth            
and f.originatingmonth =  #vintage.originatingmonth            
 and ((accountstatus ='S' AND outstbal != 0  ) or  -- settled          
( accountstatus !='S' AND notfullydelivered = 'Y' ) or  -- non delivered          
((accountstatus in ('','U','0') or currstatus is null)   -- sc          
      AND (total_bal < 0 or total_bal-intadminter<0)  AND notfullydelivered = 'N' ) or  -- credit          
( accountstatus !='S' AND ( total_bal <= 0  or total_bal-intadminter<0) ) )            
and outstbal not between -0.0099999 and 0.00999999),0)            
            
update #vintage            
set curr = isnull(( select sum(Total_bal-intadminter)            
from #accountmonths f            
where  f.Currentmonth =  #vintage.Currentmonth            
and f.originatingmonth =  #vintage.originatingmonth            
and f.accountstatus!='S'            
and notfullydelivered='N'            
and total_bal>0            
and total_bal-intadminter>0             
and accountstatus in ('1','2','3','4','5','6','7','8','9')            
and (floor(f.delinquencynodays)<=0)),0)            
            
update #vintage            
set rebcurr = isnull(( select sum(rebatetotal)            
from #accountmonths f            
where  f.Currentmonth =  #vintage.Currentmonth            
and f.originatingmonth =  #vintage.originatingmonth            
and f.accountstatus!='S'            
and notfullydelivered='N'            
and total_bal>0            
and total_bal-intadminter>0             
and accountstatus in ('1','2','3','4','5','6','7','8','9')            
and (floor(f.delinquencynodays)<=0)) ,0)            
            
update #vintage            
set dynamicint=            
(select sum(isnull(rate,0)*isnull(total_bal,0))            
/sum(isnull(total_bal,0))            
from #accountmonths            
where isnull(total_bal,0)>0)            
            
update #vintage            
set staticint=            
(select sum(isnull(rate,0)*isnull(total_bal,0))            
/sum(isnull(total_bal,0))            
from #accountmonths            
where isnull(total_bal,0)>0             
and #vintage.OriginatingMonth=#accountmonths.originatingmonth)            
            
            
update #vintage            
set dynamicterm=            
(select sum(DATEDIFF(month,@currentmonth,datelast)*total_bal)/sum(total_bal)            
from #accountmonths            
where total_bal>0)            
            
update #vintage            
set staticterm=            
(select sum(DATEDIFF(month,@currentmonth,datelast)*total_bal)/sum(total_bal)            
from #accountmonths            
where isnull(total_bal,0)>0 and #vintage.OriginatingMonth=#accountmonths.originatingmonth)            
             
insert into vintage2 select * from #vintage            
truncate table #vintage            
            
insert into accountmonths2             
select * from #accountmonths            
            
truncate table #accountmonths            
            
--if @currentmonth='2006-01-01'            
--break            
            
end            
            
/*            
            
select currentmonth as currentmonth,              
dynamicterm as term,dynamicint as intr,               
sum(openingbal) as opening_balance,              
sum(newreceivables) as newbus,              
sum(prepaymentcollections) as ppcoll,              
sum(collections-prepaymentcollections) as ncoll,              
sum(collections) as tcoll,            
sum(baddebtwriteoff) as bdwo,              
sum(insurance) as ins,              
sum(recoveries) as rec,              
sum(baddebtwriteoff+insurance+recoveries) as netWO,              
sum(discounts) as disc,              
sum(refunds)as ref,              
sum(adjustments) as adj,              
sum(grts) as grts,              
sum(discounts+refunds+adjustments+grts) as dil,              
sum(repossessions) as repo,              
sum(charges) as chg,              
sum(othermovements) as other,              
sum(settlementrebates) as setreb,              
sum(addtorebates) as addtoreb,              
sum(rebates) as reb,              
sum(addtoprepayments) as addtopp,              
sum(addto) as addcr,              
sum(adddr) as adddr,              
sum(adddr-addtoprepayments) as addtonew,              
sum(addto+addtoprepayments+addtorebates) as addtototal,              
sum(outstandingreceivables) as closingbalance,     
sum(addtobalance) as addtobal,              
sum(curr) as curradv,              
sum(arrears1to30) as arr1to30,              
sum(arrears31to60)as arr31to60,              
sum(arrears61to90) as arrs61to90,              
sum(arrears91to120)as arr91to120,              
sum(arrears121to150) as arr121to150,              
sum(arrears151to180) as arr151to180,              
sum(Arrears181to360) as Arr181to360,              
sum(settled) as settint,              
sum(intadminter) as intreopen,              
sum(rebcurr) as rebcurradv,              
sum(rebarrears1to30) as rebarr1to30,              
sum(rebarrears31to60)as rebarr31to60,              
sum(rebarrears61to90) as rebarr61to90,              
sum(rebarrears91to120)as rebarr91to120,              
sum(rebarrears121to150) as rebarr121to150,              
sum(rebarrears151to180) as rebarr151to180,              
sum(rebArrears181to360) as rebArr181to360,              
sum(curr-rebcurr) as netcurradv,              
sum(arrears1to30-rebarrears1to30) as net1to30,              
sum(arrears31to60-rebarrears31to60)as net31to60,              
sum(arrears61to90-rebarrears61to90) as net61to90,              
sum(arrears91to120-rebarrears91to120)as net91to120,              
sum(arrears121to150-rebarrears121to150) as net121to150,              
sum(arrears151to180-rebarrears151to180) as net151to180,              
sum(Arrears181to360-rebArrears181to360) as net181to360              
from vintage2             
where currentmonth>=@startdate            
group by currentmonth, dynamicterm, dynamicint              
order by currentmonth desc            
*/            
END         
go