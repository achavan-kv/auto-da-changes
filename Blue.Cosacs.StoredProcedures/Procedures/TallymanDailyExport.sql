
-- Not required in 5.2 - jec 09/02/10 - Malaysia Merge
--IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.columns WHERE column_name ='ExportedToTallyman')
--ALTER TABLE fintrans ADD ExportedToTallyman BIT 
--go
IF EXISTS (SELECT * FROM SYSOBJECTS WHERE NAME = 'TallymanDailyExport')
DROP PROCEDURE TallymanDailyExport
GO
create PROCEDURE [dbo].[TallymanDailyExport]
-- ================================================  
-- Project      : CoSACS .NET  
-- File Name    : TallymanWeeklyExport.prc  
-- File Type    : MSSQL Server Stored Procedure Script  
-- Title        : Tallyman Weekly Export  
-- Author       : Richard Boyce  
-- Date         : March 2008  
--  
-- This procedure will export the Tallyman/Transact details  
--   
-- Change Control  
-- --------------  
-- Date      By  Description  
-- ----      --  -----------  
-- 15/05/08  jec UAT219 transaction codes in Tallypayment file to be two chars i.e 3 = '03' and   
--     output transaction values with 2 decimal places.  
-- 16/05/08  jec UAT219 debit and credit reason switched  
-- 20/05/08  jec UAT183 remove white space  
-- 22/05/08  jec UAT183 add procedure to convert empty columns to null  
-- 30/05/08  jec Remove value 0.00 from transactamendhdr  
-- 09/06/08  aa  adding extra column to fintrans called exportedtotransact  
-- 23/06/08  aa	 cant remember
-- 26/06/08  AA	 telephone number restricted to 12 digits of Tallyman rejects 
-- 29/07/08  RDB Added logic to update products for changed accounts
-- ================================================  
 -- Add the parameters for the stored procedure here  
as  
  
declare @err int  
declare @PassFail char(1)  
declare @tallyPayPassFail char(1)  
declare @tallyNewAmendPassFail char(1)  
declare @transactAmendPassFail char(1)  


select acctno, hldorjnt,min(custid) as delcust
into #tempx
from custacct
group by acctno, hldorjnt
having count(*)>1

delete from custacct
where exists
(select 'x' from #tempx
where custacct.acctno=#tempx.acctno
and custacct.hldorjnt=#tempx.hldorjnt
and custacct.custid=#tempx.delcust)

drop table #tempx
 
select acctno, custid, max(dateprop) as dateprop, max(propnotes) as propnotes
into #tempx3
from proposal
group by acctno, custid
having count(*)>1
 

delete from proposal
where exists
(select 'x' from #tempx3
where proposal.acctno=#tempx3.acctno
and proposal.custid=#tempx3.custid
and (proposal.dateprop<#tempx3.dateprop
or (proposal.dateprop=#tempx3.dateprop and proposal.propnotes<>#tempx3.propnotes)))

drop table #tempx3

select  custid, max(dateemployed) as dateemployed
into #tempx2
from employment
where dateleft is null
group by  custid
having count(*)>1
 

update employment
set dateleft=getdate()
where exists
(select 'x' from #tempx2
where employment.dateemployed<#tempx2.dateemployed
and employment.custid=#tempx2.custid)

drop table #tempx2

select  custid,addtype, max(datein) as datein
into #tempx1
from custaddress
where datemoved is null
group by  custid,addtype
having count(*)>1
 

update custaddress
set datemoved=getdate()
where exists
(select 'x' from #tempx1
where custaddress.datein<#tempx1.datein
and custaddress.custid=#tempx1.custid
and custaddress.addtype=#tempx1.addtype)

drop table #tempx1


UPDATE acct SET highststatus = isnull((select max(statuscode) 
from status where status.acctno=acct.acctno and status.statuscode not in ('S','U')),1)
WHERE highststatus =''  
--AA 31 Jul 2008
if exists (select * from information_schema.tables where table_name = 'tempTransactAmend')
drop table tempTransactAmend   
  
set @PassFail = 'P'  
  
declare @startDate datetime  
declare @endDate datetime  
declare @RunNo int  
  
if exists(select * from interfacecontrol where interface = 'TALLYDAILY')  
begin  
 select @startdate =  max(datefinish), @RunNo = max(RunNo) from interfacecontrol  
  where interface = 'TALLYDAILY'  
  
 if not exists( select * from interfacecontrol where runno = @runNo and result='P' and interface = 'TALLYDAILY')  
 begin  
  -- if last run was not succesful rerun and set start date to run start date  
   select @startdate =  datestart from interfacecontrol where  runno = @runNo and interface = 'TALLYDAILY'  
 end  
 else  
 begin  
  -- otherwise move to next run  
set @RunNo= @RunNo + 1  
 end  
end  
else  
begin  
  
   select @startdate = min(datefinish) from interfacecontrol  
    where interface = 'TALLYWEEK'  
   set @RunNo = 1  
    
end  
  
--REMOVE  
--set @startdate = dateadd(day,-1,getdate())  
  
  
set @enddate = getdate()  
  
-- dont let this run before a tallyweek run has done  
if not @startdate is null  
begin  

-- rdb 19/08/08 select all account that have been updated into a temporary table 
-- select from this table
PRINT 'importing updated accounts'
SELECT distinct acct.acctno
	INTO #updatedAccounts
FROM acct  
	inner join custacct  
		on acct.acctno = custacct.acctno and hldorjnt = 'H'  
	INNER JOIN customer   
		on custacct.custid = customer.custid  
	inner join custaddress  
	   on customer.custid = custAddress.custid and addtype='H' and datemoved is null  
  left outer  join custaddress workAddress  
   on customer.custid = workAddress.custid and workAddress.addtype='W' and workAddress.datemoved is null  
  left outer  join custaddress deliveryAddress  
   on customer.custid = deliveryAddress.custid and deliveryAddress.addtype='H' and deliveryAddress.datemoved is null  
  LEFT outer join custtel  
   on customer.custid = custtel.custid and custtel.tellocn = 'H' and datediscon is null  
  left outer join custtel mobileTel  
   on customer.custid = mobileTel.custid and mobileTel.tellocn = 'M' and mobileTel.datediscon is null  
  left outer join custtel employerTel  
on customer.custid = employerTel.custid and employerTel.tellocn = 'W' and employerTel.datediscon is null  
  inner join Proposal  
   on acct.acctno = proposal.acctno  
  inner join Agreement  
   on acct.acctno = Agreement.acctno  
  left outer join Employment  
   on customer.custid = employment.custid and dateleft is null   
  left outer join custacct otherApplicant  
   on acct.acctno = otherApplicant.acctno and otherApplicant.hldorjnt != 'H'  
  left outer join customer app2   
   on otherApplicant.custid  =app2.custid  
  left outer join custaddress app2Address  
   on otherApplicant.custid = app2Address.custid and app2Address.addtype='H' and app2Address.datemoved is null  
  left outer  join custaddress app2WorkAddress  
   on otherApplicant.custid = app2WorkAddress.custid and app2WorkAddress.addtype='W' and workAddress.datemoved is null  
  left outer join custtel app2homeTel  
   on otherApplicant.custid = app2homeTel.custid and app2homeTel.tellocn = 'H' and app2homeTel.datediscon is null  
  left outer join custtel app2WorkTel  
   on otherApplicant.custid = app2WorkTel.custid and app2WorkTel.tellocn = 'W' and app2WorkTel.datediscon is null  
  left outer join Employment app2Employment  
   on otherApplicant.custid = app2Employment.custid  
  left outer join ProposalRef Ref1  
   on acct.acctno = Ref1.acctno and refno = 1  
  left outer join ProposalRef Ref2  
   on acct.acctno = Ref2.acctno and Ref2.refno = 2  
  left outer join ProposalRef Ref3  
   on acct.acctno = Ref3.acctno and Ref3.refno = 3  
  left outer join ProposalRef Ref4  
   on acct.acctno = Ref4.acctno and Ref4.refno = 4  
  left outer join ProposalRef Ref5  
   on acct.acctno = Ref5.acctno and Ref5.refno = 5  
  left outer join scoretrak  
   on acct.acctno = scoretrak.acctno  
  inner join instalplan  
   on acct.acctno = instalplan.acctno  
  LEFT outer join TM_Product  
   on acct.acctno = TM_Product.Account_Number  
  left outer join bank  
   on proposal.bankcode = bank.bankcode  
  left outer join bankacct  
   on customer.custid = bankacct.custid  
  LEFT outer join termstype   
   on acct.termstype=termstype.TermsType  -- jec 20/05/08 UAT 183 link to get G,S,B code  
   
   left outer join custcatcode Traceind  
   on customer.custid = Traceind.custid and Traceind.code = 'T'  
  left outer join custcatcode RefuseCredit  
   on customer.custid = RefuseCredit.custid and RefuseCredit.code = 'R'  
  left outer join custcatcode CircIndicator  
   on customer.custid = CircIndicator.custid and CircIndicator.code = 'C'   
  where  (acct.tallymanAcct = 1
  --and (CurrStatus != 'S' 
	
  and  
 (  
  acct.dateacctopen > @startdate and acct.dateacctopen <= @enddate  
  or  
  acct.datelastpaid > @startdate and acct.datelastpaid <= @enddate  
  or  
  acct.dateintoarrears > @startdate and acct.dateintoarrears <= @enddate  
  or  
  customer.datechange > @startdate and customer.datechange <= @enddate  
  or   
  custaddress.datein > @startdate and custaddress.datein <= @enddate  
  or   
  custaddress.datemoved > @startdate and custaddress.datemoved <= @enddate  
  or  
  custaddress.datechange > @startdate and custaddress.datechange <= @enddate  
  or   
  workAddress.datein > @startdate and workAddress.datein <= @enddate  
  or   
  workAddress.datemoved > @startdate and workAddress.datemoved <= @enddate  
  or  
  workAddress.datechange > @startdate and workAddress.datechange <= @enddate  
  or   
  deliveryAddress.datein > @startdate and deliveryAddress.datein <= @enddate  
  or   
  deliveryAddress.datemoved > @startdate and deliveryAddress.datemoved <= @enddate  
  or  
  deliveryAddress.datechange > @startdate and deliveryAddress.datechange <= @enddate  
  or  
  custtel.dateteladd > @startdate and custtel.dateteladd <= @enddate  
  or  
  custtel.datediscon > @startdate and custtel.datediscon <= @enddate  
  or  
  custtel.datechange > @startdate and custtel.datechange <= @enddate  
  or  
  mobileTel.dateteladd > @startdate and mobileTel.dateteladd <= @enddate  
  or  
  mobileTel.datediscon > @startdate and mobileTel.datediscon <= @enddate  
  or  
  mobileTel.datechange > @startdate and mobileTel.datechange <= @enddate  
  or  
  employerTel.dateteladd > @startdate and employerTel.dateteladd <= @enddate  
  or  
  employerTel.datediscon > @startdate and employerTel.datediscon <= @enddate  
  or  
  employerTel.datechange > @startdate and employerTel.datechange <= @enddate  
  or  
  proposal.datechange > @startdate and proposal.datechange <= @enddate  
  or  
  agreement.datechange > @startdate and agreement.datechange <= @enddate  
  or  
  Employment.dateemployed > @startdate and Employment.dateemployed <= @enddate  
  or  
  app2.datechange > @startdate and app2.datechange <= @enddate  
  or   
  app2Address.datein > @startdate and app2Address.datein <= @enddate  
  or   
  app2Address.datemoved > @startdate and app2Address.datemoved <= @enddate  
  or  
  app2Address.datechange > @startdate and app2Address.datechange <= @enddate  
  or   
  app2WorkAddress.datein > @startdate and app2WorkAddress.datein <= @enddate  
  or   
  app2WorkAddress.datemoved > @startdate and app2WorkAddress.datemoved <= @enddate  
  or  
  app2WorkAddress.datechange > @startdate and app2WorkAddress.datechange <= @enddate  
  or  
  app2homeTel.dateteladd > @startdate and app2homeTel.dateteladd <= @enddate  
  or  
  app2homeTel.datediscon > @startdate and app2homeTel.datediscon <= @enddate  
  or  
  app2homeTel.datechange > @startdate and app2homeTel.datechange <= @enddate  
  or  
  app2Employment.dateemployed > @startdate and app2Employment.dateemployed <= @enddate  
  or  
  Ref1.datechange > @startdate and Ref1.datechange <= @enddate  
  or  
  Ref2.datechange > @startdate and Ref2.datechange <= @enddate  
  or  
  Ref3.datechange > @startdate and Ref3.datechange <= @enddate  
  or  
  Ref4.datechange > @startdate and Ref4.datechange <= @enddate  
  or  
  Ref5.datechange > @startdate and Ref5.datechange <= @enddate  
  or  
  scoretrak.dateprop > @startdate and scoretrak.dateprop <= @enddate  
  or  
  bankacct.dateopened > @startdate and bankacct.dateopened <= @enddate  
  
  
  or  
  Traceind.datecoded > @startdate and Traceind.datecoded <= @enddate  
  or  
  RefuseCredit.datecoded > @startdate and RefuseCredit.datecoded <= @enddate  
  or  
  CircIndicator.datecoded > @startdate and CircIndicator.datecoded <= @enddate  
 ) )
    or exists (select * from tallytoexport t 
               where t.acctno= acct.acctno)

  truncate table tallytoexport

  
-- set up lineitems first  
  
TRUNCATE TABLE TM_Product  
declare @datelastproductupdate datetime  
select @datelastproductupdate = max(datefinish) from interfacecontrol where interface in ('TALLYDAILY','TALLYWEEK' )  
  
  
--  
    -- NEW / UPDATE Products  
    --  
    UPDATE  TallymanProducts  
    SET     ExportType = ''  
  
    -- UPDATE Products that have changed since the last run  
    -- or belong to an account that has re-entered arrears  
/*  
    UPDATE  TallymanProducts  
    SET     ExportType = 'U'  
    FROM    TallymanAcctArrears ta  
    WHERE   ta.AcctNo = TallymanProducts.AcctNo  
    AND     ta.FirstSent IS NOT NULL  
    AND     ta.Removed IS NULL  
    AND     (TallymanProducts.LastUpdate > @datelastproductupdate OR ta.ExportType = 'R')  
*/  


-- uat69900 set export to 'N' on accounts that have changed
	-- to update their lineitems
UPDATE tp   
    SET ExportType = 'N'  
		FROM TallymanProducts tp
			INNER join #updatedaccounts ua
				ON tp.acctno = ua.acctno
			
-- END 69900

   -- SET @Return = @@ERROR IF @Return != 0 RETURN @Return  
  
    -- NEW Products for each NEW Account  
    -- Make sure the new products are present for a new account  
    -- New products are always sent with a new account  
 print 'inseting into TallymanProducts'  
  
    INSERT INTO TallymanProducts  
            (AcctNo, LastUpdate, ExportType)  
    SELECT  DISTINCT acctno, @EndDate, 'N'  
    FROM    acct ta  
    WHERE   ta.tallymanacct = 1  
    AND NOT EXISTS (SELECT AcctNo FROM TallymanProducts tp  
                    WHERE  tp.AcctNo = ta.AcctNo)  
  
    --SET @Return = @@ERROR IF @Return != 0 RETURN @Return  
  
    -- NEW Products for a NEW Account  
/*  
    UPDATE  TallymanProducts  
    SET     ExportType = 'N'  
    FROM    TallymanAcctArrears ta  
    WHERE   ta.AcctNo = TallymanProducts.AcctNo  
    AND     ta.ExportType = 'N'  
*/  
  
    --SET @Return = @@ERROR IF @Return != 0 RETURN @Return  
  
  
    INSERT INTO TM_Product  
           (InsertUpdate,  
			Account_Number,  
            CustomerIC_Number)  
    SELECT  tp.ExportType,  
            tp.AcctNo,  
            substring(ltrim(ca.CustId),1,12) as custid
    FROM     TallymanProducts tp, CustAcct ca  
    WHERE   tp.ExportType != ''  
    AND     ca.AcctNo = tp.AcctNo  
    AND     ca.HldOrJnt = 'H'  
  
  
-- set up lineitems first  
    SELECT  IDENTITY(INT,1,1) AS LineId,  
            CONVERT(INTEGER,0) AS SeqNo,  
            d.AcctNo,  
            d.ItemNo,  
            d.StockLocn,  
            SUM(d.TransValue) AS TransValue,  
            SUM(d.Quantity) AS Quantity,  
            s.Category  
    INTO    #TMLineItem  
    FROM    TM_Product tp, Delivery d, StockItem s  
    WHERE   d.AcctNo = tp.Account_Number  
    AND     s.ItemNo = d.ItemNo  
    AND     s.StockLocn = d.StockLocn 
    -- lw69900 Malaysia require non-stocks to be passed Tallyman, remove clause
    --AND     (s.ItemType = 'S' OR s.Cate ry = 11 OR (s.Cate ry >= 51 AND s.Cate ry <= 59))  
    GROUP BY d.AcctNo, d.ItemNo, d.StockLocn, s.Category  
    HAVING SUM(d.TransValue) > 0 AND SUM(d.Quantity) > 0  
    ORDER BY d.AcctNo ASC, SUM(d.TransValue) DESC  
  
    UPDATE  #TMLineItem  
    SET     SeqNo = LineId + 1 - (SELECT MIN(t2.LineId) FROM #TMLineItem t2  
                                  WHERE  t2.Acctno = #TMLineItem.AcctNo)  
  
  
    -- prevent large quantities causing errors
   update #TMLineItem set quantity = 99 where quantity >=100
   
-- get min buffno for delivereditems by account  
select delivery.acctno, min(buffno) BuffNo   
 into    #TMBuffNo  
 from delivery  
 inner join #TMLineItem  
  on delivery.acctno = #TMLineItem.acctno  
  and delivery.itemno = #TMLineItem.itemno  
  where delorcoll = 'D'  
  group by delivery.acctno  
  
  
  
PRINT 'Export the items for each account numbered 1 to 20 ... '  
    DECLARE @SeqNo   INTEGER  
    DECLARE @SQLStr  VARCHAR(1500)  
    SET @SeqNo = 1  
    WHILE (@SeqNo <= 20)  
    BEGIN  
        SET @SQLStr =  
         ' UPDATE  TM_Product ' +  
         ' SET     Product_Code' + CONVERT(VARCHAR,@SeqNo) + '               = ISNULL(l.ItemNo,''''), ' +  
         '         Product_Code' + CONVERT(VARCHAR,@SeqNo) + '_Amount        = ISNULL(CONVERT(DECIMAL(15,2),l.TransValue),''0.00''), ' +  
         '         Product_Code' + CONVERT(VARCHAR,@SeqNo) + '_Quantity      = ISNULL(CONVERT(DECIMAL(2,0),l.Quantity),''0''), ' +  
         '         Product_Code' + CONVERT(VARCHAR,@SeqNo) + '_Description   = ISNULL(s.ItemDescr1,''''), ' +  
         '         Product_Code' + CONVERT(VARCHAR,@SeqNo) + '_Description_2 = ISNULL(s.ItemDescr2,'''') ' +  
         ' FROM    #TMLineItem l, StockItem s ' +  
         ' WHERE   l.AcctNo = TM_Product.Account_Number ' +  
         ' AND     l.SeqNo = ' + CONVERT(VARCHAR,@SeqNo) +  
         ' AND     s.ItemNo = l.ItemNo ' +  
         ' AND     s.StockLocn = l.StockLocn '  
  
        EXECUTE (@SQLStr)  
  
        --SET @Return = @@ERROR IF @Return != 0 RETURN @Return  
  
        SET @SeqNo = @SeqNo + 1  
    END  
  
  
    --  
    PRINT 'Product Cate ry ... '  
    --  
  
    PRINT 'For RF accounts Product Cate ry = Proposal.RFCate ry ... '  
    UPDATE  TM_Product  
    SET     Product_Category =  
            ISNULL((SELECT  MAX(p.RFCategory)  
                    FROM    Proposal p  
                    WHERE   p.AcctNo   = TM_Product.Account_Number  
                    AND     p.DateProp = (SELECT MAX(p2.DateProp) FROM Proposal p2  
                                          WHERE  p2.AcctNo = TM_Product.Account_Number)),'')  
    FROM    Acct a  
    WHERE   a.AcctNo   = TM_Product.Account_Number  
    AND     a.AcctType = 'R'  
  
    --SET @Return = @@ERROR IF @Return != 0 RETURN @Return  
  
    PRINT 'Non RF accounts Product Cate ry = Most expensive item ... '  
    UPDATE  TM_Product  
    SET     Product_Category = ISNULL(t.Category,'')  
    FROM    Acct a, #TMLineItem t  
    WHERE   a.AcctNo    = TM_Product.Account_Number  
    AND     a.AcctType != 'R'  
    AND     t.AcctNo    = TM_Product.Account_Number  
    AND     t.SeqNo     = 1  
  
    --SET @Return = @@ERROR IF @Return != 0 RETURN @Return  
  
  
  
-- Product INSERT  
    UPDATE  TM_Product  
    SET     CustomerIC_Number               = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(CustomerIC_Number,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Account_Number                  = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Account_Number,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code1                   = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code1,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code1_Description       = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code1_Description,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code1_Quantity          = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code1_Quantity,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code1_Amount            = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code1_Amount,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code2                   = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code2,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code2_Description       = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code2_Description,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code2_Quantity          = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code2_Quantity,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code2_Amount            = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code2_Amount,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code3                   = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code3,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code3_Description       = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code3_Description,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code3_Quantity          = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code3_Quantity,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code3_Amount            = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code3_Amount,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code4                   = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code4,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code4_Description       = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code4_Description,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code4_Quantity          = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code4_Quantity,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code4_Amount            = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code4_Amount,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code5                   = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code5,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code5_Description       = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code5_Description,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code5_Quantity          = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code5_Quantity,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code5_Amount            = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code5_Amount,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code6                   = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code6,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code6_Description       = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code6_Description,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code6_Quantity          = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code6_Quantity,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code6_Amount            = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code6_Amount,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
         Product_Code7                   = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code7,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code7_Description       = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code7_Description,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code7_Quantity          = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code7_Quantity,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code7_Amount            = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code7_Amount,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code8                   = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code8,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code8_Description       = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code8_Description,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code8_Quantity          = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code8_Quantity,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code8_Amount            = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code8_Amount,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code9                   = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code9,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code9_Description       = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code9_Description,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code9_Quantity          = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code9_Quantity,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code9_Amount            = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code9_Amount,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code10                  = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code10,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code10_Description      = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code10_Description,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code10_Quantity         = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code10_Quantity,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code10_Amount           = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code10_Amount,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code11                  = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code11,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code11_Description      = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code11_Description,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code11_Quantity         = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code11_Quantity,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code11_Amount           = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code11_Amount,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code12                  = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code12,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code12_Description      = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code12_Description,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code12_Quantity         = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code12_Quantity,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code12_Amount           = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code12_Amount,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code13                  = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code13,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code13_Description    = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code13_Description,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code13_Quantity         = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code13_Quantity,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code13_Amount           = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code13_Amount,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code14                  = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code14,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code14_Description      = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code14_Description,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code14_Quantity         = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code14_Quantity,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code14_Amount           = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code14_Amount,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code15                  = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code15,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code15_Description      = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code15_Description,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code15_Quantity         = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code15_Quantity,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code15_Amount           = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code15_Amount,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code16                  = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code16,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code16_Description      = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code16_Description,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code16_Quantity         = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code16_Quantity,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code16_Amount           = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code16_Amount,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code17                  = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code17,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code17_Description      = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code17_Description,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code17_Quantity         = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code17_Quantity,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code17_Amount           = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code17_Amount,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code18                  = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code18,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code18_Description      = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code18_Description,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code18_Quantity         = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code18_Quantity,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code18_Amount           = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code18_Amount,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code19                  = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code19,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code19_Description      = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code19_Description,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code19_Quantity         = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code19_Quantity,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code19_Amount           = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code19_Amount,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code20                  = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code20,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code20_Description      = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code20_Description,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code20_Quantity         = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code20_Quantity,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code20_Amount           = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code20_Amount,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code1_Description_2     = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code1_Description_2,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code2_Description_2     = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code2_Description_2,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code3_Description_2     = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code3_Description_2,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code4_Description_2     = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code4_Description_2,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code5_Description_2     = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code5_Description_2,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code6_Description_2     = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code6_Description_2,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code7_Description_2     = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code7_Description_2,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code8_Description_2     = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code8_Description_2,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code9_Description_2     = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code9_Description_2,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code10_Description_2    = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code10_Description_2,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code11_Description_2    = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code11_Description_2,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code12_Description_2    = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code12_Description_2,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code13_Description_2    = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code13_Description_2,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code14_Description_2    = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code14_Description_2,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code15_Description_2    = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code15_Description_2,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code16_Description_2    = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code16_Description_2,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code17_Description_2    = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code17_Description_2,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code18_Description_2    = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code18_Description_2,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code19_Description_2    = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code19_Description_2,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Code20_Description_2    = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code20_Description_2,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
            Product_Category                = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Category,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' ')  
  
  
PRINT 'Tallypayment data ... '  
-- *************************** TallyPay ********************************  
-- rdb 09/05/08 all transaction too be shown as positive  
  
SELECT substring(ltrim(customer.custid),1,12) as CUSTOMERIC_NUMBER,   
  acct.acctno as ACCOUNT_NUMBER,  
  --Fintrans.Transvalue as TransactionAmount,  
  TransactionAmount = case when FinTrans.TransValue < 0 then -FinTrans.TransValue ELSE  FinTrans.TransValue end,  
  CONVERT(CHAR(10), Fintrans.datetrans, 23) as TransactionDate,  
  'MYR' as Currency,  
--  [Credit Reason] = case when FinTrans.TransValue <= 0 then Transtype.Tccodecr else null end,  
--  --[Credit Reason] = case when FinTrans.TransValue <= 0 THEN CASE WHEN LEN(Transtype.Tccodecr) = 1 THEN '0' + Transtype.Tccodecr ELSE Transtype.Tccodecr end else null end,  
--  [Debit Reason] =  case when FinTrans.TransValue > 0 then Transtype.Tccodedr else null end,  
 -- jec 15/05/08 UAT219  
  [Credit Reason] = case   
    when FinTrans.TransValue <= 0  then RIGHT( '00' + cast(Transtype.Tccodecr  as varCHAR(2)),2)       
    else null end,    
  [Debit Reason] =  case       
    when FinTrans.TransValue > 0 then RIGHT( '00' + cast(Transtype.Tccodedr  as varCHAR(2)),2)       
    else null end,  
  Fintrans.Branchno as Last_Payment_Branch  
 into ##TempTallyPay  
 from acct  
 inner join custacct  
  on acct.acctno = custacct.acctno and hldorjnt = 'H'  
 INNER JOIN customer   
  on custacct.custid = customer.custid  
 inner join Fintrans  
  on  acct.acctno = FinTrans.acctno  
 inner join TransType  
  on FinTrans.transtypecode = TransType.transtypecode  
where    
 --Fintrans.datetrans > @startdate   
 -- and Fintrans.datetrans <= @enddate and   
    ISNULL(fintrans.ExportedToTallyman,0) !=1  
    and acct.tallymanacct = 1 --and CurrStatus != 'S'  
     
     
     
  
  
declare @path varchar(400)  
-- jec 15/05/08 uat219 -- output transaction amount with 2 decimal places  
-- jec 16/05/08 uat219 -- debit and credit reason switched in header  
set @path = 'BCP " select CONVERT(CHAR(10), GetDate(), 23),Count(*), (select cast(sum(TransactionAmount) as decimal(12,2)) from ##TempTallyPay where NOT [credit reason] IS null), (select cast(sum(TransactionAmount) as decimal(12,2)) from ##TempTallyPay where NOT [debit reason] IS null) from ##tempTallyPay "  queryout ' +  
 'D:\tallyman\operations\interface\debt\Tallypaymenthdr.csv ' + '-c -t, -q -Usa -P'  
exec master.dbo.xp_cmdshell @path  
-- jec 15/05/08 uat219 - remove "RIGHT( '00' + " etc from BCP command  
set @path = 'BCP "SELECT CUSTOMERIC_NUMBER, ACCOUNT_NUMBER, cast(TransactionAmount as decimal(8,2)), TransactionDate, Currency,[Credit Reason] , [Debit Reason] , Last_Payment_Branch FROM ##temptallypay "  queryout ' +  
 'D:\tallyman\operations\interface\debt\Tallypayment.csv ' + '-c -t, -q -Usa -P'  
exec master.dbo.xp_cmdshell @path  
  
SET @err = @@ERROR   
  
set @tallyPayPassFail = 'P'  
IF @err != 0  
begin  
 set @PassFail = 'F'  
 set @tallyPayPassFail = 'F'  
end  
else  
begin  
 UPDATE fintrans SET exportedToTallyman = 1   
 FROM acct a ,##TempTallyPay t  
 WHERE a.acctno= fintrans.acctno AND a.tallymanacct = 1  
   AND t.ACCOUNT_NUMBER= a.acctno   
 AND ISNULL(exportedtoTallyman,0)=0    
end  
  
insert into interfacecontrol  
  (interface,runno,datestart,datefinish,result)  
 values  
  ('TALLYPAYME', @runno,@startdate ,@enddate, @tallyPayPassFail)  
  
drop table ##tempTallyPay  
  
  
  
-- ****************************** TallyNewAmend *********************************  
PRINT 'TallyNewAmend data ... '  
  
select  distinct substring(ltrim(customer.custid),1,12) as CUSTOMERIC_NUMBER,   
  acct.acctno as ACCOUNT_NUMBER,  
  --'0.00' as TransactionAmount,   
  --CONVERT(CHAR(10), GetDate(), 23) as TransactionDate,  
  --'MYR' as Currency,  
  --'MS' as Language,  
  convert(char(3),Acct.Branchno) as Branch_No,  --jec 16/05/08 change branch length from 5 to 3  
  right(Left(right(acct.acctno,9),8),5) as Account4to11,  
  left(replace(Customer.Name,',',' '),60) as Customer_Name,  
  replace(Customer.Title,',',' ') as Customer_Title,  
  Left(Customer.firstname,1) as Customer_Initials,  
  replace(custaddress.Cusaddr1,',',' ') as Customer_Address_Line1,  
  replace(custaddress.Cusaddr2,',',' ') as Customer_Address_Line2,  
  replace(custaddress.Cusaddr3,',',' ') as Customer_Address_Line3,  
  replace(ISNULL(custaddress.cuspocode,''),',',' ') as Customer_Postcode,  --jec 16/05/08 remove null  
  TraceInd.Code as Trace_indicator,  
  RefuseCredit.Code as Customer_Refuse_Credit,  
  CircIndicator.Code as Circ_indicator  
 into ##tempTallyNewAmend  
 from acct  
	INNER JOIN #updatedAccounts ua
		ON	acct.acctno = ua.acctno
  inner join custacct  
   on acct.acctno = custacct.acctno and hldorjnt = 'H'  
  INNER JOIN customer   
   on custacct.custid = customer.custid  
  inner join custaddress  
   on customer.custid = custAddress.custid and addtype='H' and datemoved is null  
  left outer join custcatcode Traceind  
   on customer.custid = Traceind.custid and Traceind.code = 'T'  
  left outer join custcatcode RefuseCredit  
   on customer.custid = RefuseCredit.custid and RefuseCredit.code = 'R'  
  left outer join custcatcode CircIndicator  
   on customer.custid = CircIndicator.custid and CircIndicator.code = 'C'  
  inner join instalplan  
   on acct.acctno = instalplan.acctno  

  
  
-- jec 16/05/08 remove 0.00 from tallynewamendhdr  
set @path = 'BCP "select CONVERT(CHAR(10), GetDate(), 23),Count(*) from ##tempTallyNewAmend "  queryout ' +  
 'D:\tallyman\operations\interface\debt\Tallynewamendhdr.csv ' + '-c -t, -q -Usa -P'  
exec master.dbo.xp_cmdshell @path  
set @path = 'BCP "select * from ##tempTallyNewAmend "  queryout ' +  
 'D:\tallyman\operations\interface\debt\Tallynewamend.csv ' + '-c -t, -q -Usa -P'  
exec master.dbo.xp_cmdshell @path  
  
SET @err = @@ERROR   
  
set @tallyNewAmendPassFail = 'P'  
IF @err != 0  
begin  
 set @PassFail = 'F'  
 set @tallyNewAmendPassFail = 'F'  
end  
  
insert into interfacecontrol  
  (interface,runno,datestart,datefinish,result)  
 values  
  ('TALLYNEWAM', @runno,@startdate ,@enddate, @tallyNewAmendPassFail)  
  
drop table ##tempTallyNewAmend  
  
  

  
-- ************************************** TransactAmend *******************************  
PRINT 'TransactAmend data ... '  
  
select distinct substring(ltrim(customer.custid),1,12) as CUSTOMERIC_NUMBER,   
  acct.acctno as ACCOUNT_NUMBER,  
  rtrim(Customer.IDType) as Customer_ID_Type,  
  ISNULL(RTRIM(Proposal.A2relation),'') as Application_Type,   -- jec 13/05/08 uat 183  
  CONVERT(CHAR(10), Proposal.dateprop, 23) as Application_Date,  
  REPLACE(CONVERT(varchar(5), Custaddress.Datein, 108), ':', '') as Customer_Time_Curr_Add_trans,  
  CONVERT(decimal(12,2),Custaddress.Mthlyrent) as Monthly_Mortgage_Rent,  
  RTRIM(Custaddress.resstatus) as Customer_Residential_Status,  
  RTRIM(Custaddress.Proptype) as Customer_Property_Type,  
  left(ltrim(replace(ISNULL(Custtel.Telno,''),'-','')),10) as Customer_Telephone_Number,  -- jec 13/05/08 uat 183  
  replace(ISNULL(Customer.Alias,''),',',' ') as Customer_Alias,    -- jec 13/05/08 uat 183  
  left(replace(ltrim(ISNULL(left(mobileTel.Telno,12),'')),'-',''),10) as Customer_Handphone_Number, -- jec 13/05/08 uat 183  
  CONVERT(CHAR(10), Customer.Dateborn, 23) as Customer_Date_of_Birth,  
  ISNULL(Proposal.Maritalstat,'') as Customer_Marital_Status,  --jec 20/05/08 UAT183  
   --null as Customer_Number_of_Dependents,  --Customer.Dependents as Customer_Number_of_Dependents   
  ISNULL(proposal.dependants,'') as Customer_Number_of_Dependents,  --jec 20/05/08 UAT183  
  RTRIM(Customer.Ethnicity) as Customer_Ethnicity_Code,  
  ISNULL(Employment.Empmtstatus,' ') as Employment_Status,  
  ISNULL(CONVERT(VARCHAR(4), Employment.DateEmployed, 12),' ') as Employment_Time_Current_trans,  
  ISNULL(Employment.Worktype,' ') as Occupation,  
  replace(LTRIM(RTRIM(ISNULL(Proposal.EmpName,''))),',',' ' ) as Employer_Name,  --jec 20/05/08 UAT183  
  null as Employer_Manager_Name, 
  replace(LTRIM(RTRIM(ISNULL(workAddress.cusaddr1,''))),',',' ') as Employer_Address_Line1,  -- rdb lw69888 referencing incorrect table for work address
  replace(LTRIM(RTRIM(ISNULL(workAddress.cusaddr2,''))),',',' ') as Employer_Address_Line2, 
  replace(LTRIM(RTRIM(ISNULL(workAddress.cusaddr3,''))),',',' ') as Employer_Address_Line3,  
  left(replace(LTRIM(RTRIM(ISNULL(Employment.Persdialcode,''))) + ltrim(rtrim(ISNULL(Employment.PersTel,''))),' ',''),10) as Employer_Telephone_Number,  -- jec 13/05/08 uat 183  
  ltrim(rtrim(ISNULL(employerTel.Extnno,''))) as Employer_Telephone_Number_Ext, -- jec 13/05/08 uat 183  
  replace(ISNULL(Proposal.Empdept,''),',','') as Employment_Department,   -- jec 13/05/08 uat 183  
  ISNULL(RTRIM(Employment.StaffNo),'') as Employment_Staff_Number, -- jec 13/05/08 uat 183  
  CONVERT(decimal(12,2),ISNULL(Proposal.Addincome,0)) as Additional_Income, -- jec 13/05/08 uat 183  
  CONVERT(decimal(12,2),ISNULL(Proposal.Mthlyincome,0)) as Total_monthly_income,  -- jec 13/05/08 uat 183  
  CONVERT(decimal(12,2),isnull(Proposal.commitments1,0) + isnull(Proposal.commitments2,0) + isnull(Proposal.commitments3,0) + isnull(Proposal.otherpmnts,0)) as Total_Monthly_Outgoings,  
  CONVERT(decimal(12,2),ISNULL(Proposal.Mthlyincome,0) - (isnull(Proposal.commitments1,0) + isnull(Proposal.commitments2,0) + isnull(Proposal.commitments3,0) + isnull(Proposal.otherpmnts,0))) as Total_Disposable_Income,  
  null as Payment_Frequency, --Employment.Payfreq  --jec 20/05/08 UAT183  
  ISNULL(Proposal.vehicleregistration,'') as Customer_Vehicle_Registration,  
  case when exists(select 1 from lineitem where acctno = acct.acctno and deliveryaddress not in ('H','',null) )then 'O' else 'H' end  as Location_of_goods, -- home or other  
  LTRIM(RTRIM(ISNULL(App2.Title,''))) as App2_Title,     --jec 20/05/08 UAT183  
  left(replace(LTRIM(RTRIM(ISNULL(App2.Name,''))),',',''),60) as App2_Customer_Name,   --jec 20/05/08 UAT183  
  substring(LTRIM(ISNULL(App2.Custid,'')),1,12) as App2_ICNumber,   --jec 20/05/08 UAT183  
  replace(LTRIM(RTRIM(ISNULL(app2Address.Cusaddr1,''))),',',' ') as APP2_Address_line1, --jec 20/05/08 UAT183  
  replace(LTRIM(RTRIM(ISNULL(app2Address.Cusaddr2,''))),',',' ') as APP2_Address_line2, --jec 20/05/08 UAT183  
  replace(LTRIM(RTRIM(ISNULL(app2Address.Cusaddr3,''))),',',' ') as APP2_Address_line3, --jec 20/05/08 UAT183  
  replace(LTRIM(RTRIM(ISNULL(app2Address.cuspocode,''))),',',' ') as App2_Postcode,  --jec 20/05/08 UAT183  
  left(lTRIM(replace(ISNULL(app2homeTel.telno,''),' ','')),10) as App2_Telephone_Number, --jec 20/05/08 UAT183  
  CONVERT(decimal(12,2),ISNULL(Proposal.A2MthlyIncome,0)) as App2_Additional_Income,  -- jec 13/05/08 uat 183  
  CONVERT(decimal(12,2),ISNULL(Proposal.A2addincome,0)) as App2_Net_monthly_income,   -- jec 13/05/08 uat 183  
  replace(ISNULL(case when Ref1.Name ='' then ref1.Surname else ref1.name end,''),',',' ') as Reference1_Name,   -- jec 13/05/08 uat 183  
  replace(ISNULL(Ref1.Address1,''),',',' ') as Address_Line1_Ref1,  
  replace(ISNULL(Ref1.Address2,''),',',' ') as Address_Line2_Ref1,  
  replace(ISNULL(Ref1.City,''),',',' ') as Address_Line3_Ref1,  
  left(ltrim(ISNULL(Ref1.Tel,'')),10) as Reference1_Telephone_Number,  
  ISNULL(RTRIM(Ref1.Relation),'') as Relationship_to_App1_Ref1,  
  replace(ISNULL(case when Ref2.Name ='' then ref2.Surname else ref2.name end,''),',',' ') as Reference2_Name,  
  replace(ISNULL(Ref2.Address1,''),',',' ') as Address_Line1_Ref2,  
  replace(ISNULL(Ref2.Address2,''),',',' ') as Address_Line2_Ref2,  
  replace(ISNULL(Ref2.City,''),',',' ') as Address_Line3_Ref2,  
  left(ltrim(ISNULL(Ref2.Tel,'')),10) as Reference2_Telephone_Number,  
  ISNULL(RTRIM(Ref2.Relation),'') as Relationship_to_App1_Ref2,  
  replace(ISNULL(case when Ref3.Name ='' then ref3.Surname else ref3.name end,''),',',' ') as Reference3_Name,  
  replace(ISNULL(Ref3.Address1,''),',',' ') as Address_Line1_Ref3,  
  replace(ISNULL(Ref3.Address2,''),',',' ') as Address_Line2_Ref3,  
  replace(ISNULL(Ref3.City,''),',',' ') as Address_Line3_Ref3,  
  left(ltrim(ISNULL(Ref3.Tel,'')),10) as Reference3_Telephone_Number,  
  ISNULL(RTRIM(Ref3.Relation),'') as Relationship_to_App1_Ref3,  
  replace(ISNULL(case when Ref4.Name ='' then ref4.Surname else ref4.name end,''),',',' ') as Reference4_Name,  
  replace(ISNULL(Ref4.Address1,''),',',' ') as Address_Line1_Ref4,  
  replace(ISNULL(Ref4.Address2,''),',',' ') as Address_Line2_Ref4,  
  replace(ISNULL(Ref4.City,''),',',' ') as Address_Line3_Ref4,  
  left(ltrim(ISNULL(Ref4.Tel,'')),10) as Reference4_Telephone_Number,  
  ISNULL(RTRIM(Ref4.Relation),'') as Relationship_to_App1_Ref4,  
  replace(ISNULL(case when Ref5.Name ='' then ref5.Surname else ref5.name end,''),',',' ') as Reference5_Name,  
  replace(ISNULL(Ref5.Address1,''),',',' ') as Address_Line1_Ref5,  
  replace(ISNULL(Ref5.Address2,''),',',' ') as Address_Line2_Ref5,  
  replace(ISNULL(Ref5.City,''),',',' ') as Address_Line3_Ref5,  
  left(ltrim(ISNULL(Ref5.Tel,'')),10) as Reference5_Telephone_Number,  
  ISNULL(RTRIM(Ref5.Relation),'') as Relationship_to_App1_Ref5,  -- jec 13/05/08 uat 183  
  replace(RTRIM(ISNULL(Custaddress.Email,'')),',','') as Customer_Email,  -- jec 13/05/08 uat 183 -- [Customer_E-mail],  
  replace(RTRIM(ISNULL(workAddress.Email,'')),',','') as Customer_Business_Email, -- jec 13/05/08 uat 183 -- [Customer_Business_E-mail]  
  replace(REPLACE(REPLACE(REPLACE(right(Proposal.Propnotes,250), CHAR(10), ''), CHAR(13), ''), CHAR(9), ''),',','') as Underwriters_Comments,  
  replace(deliveryAddress.Cusaddr1,',',' ') as Delivery_Address_line1,  
  replace(deliveryAddress.Cusaddr2,',',' ') as Delivery_Address_line2,  
  replace(deliveryAddress.Cusaddr3,',',' ') as Delivery_Address_line3,  
  null as Risk_Category,     
  Proposal.Points as Application_Score,  
  LTRIM(RTRIM(ISNULL(Proposal.Reason,''))) as Policy_Rule1,  --jec 20/05/08 UAT183  
  LTRIM(RTRIM(ISNULL(Proposal.Reason2,''))) as Policy_Rule2,  --jec 20/05/08 UAT183  
  LTRIM(RTRIM(ISNULL(Proposal.Reason3,''))) as Policy_Rule3,  --jec 20/05/08 UAT183  
  LTRIM(RTRIM(ISNULL(Proposal.Reason4,''))) as Policy_Rule4,  --jec 20/05/08 UAT183  
  LTRIM(RTRIM(ISNULL(Proposal.Reason5,''))) as Policy_Rule5,  --jec 20/05/08 UAT183  
  LTRIM(RTRIM(ISNULL(Proposal.Reason6,''))) as Policy_Rule6,  --jec 20/05/08 UAT183  
  null as Policy_Rule7,    
  null as Policy_Rule8,    
  null as Policy_Rule9,    
  null as Policy_Rule10,    
  null as OverRide_Reason,    
  null as Product_Category , --***scoretrak.prodcat   
  LTRIM(RTRIM(ISNULL(Product_Code1,''))) as Product_Code1,     --jec 20/05/08 UAT183 LTRIM(RTRIM(   
  LTRIM(RTRIM(ISNULL(Product_Code1_description,''))) as Product_Code1_description,  --jec 20/05/08 UAT183  
  LTRIM(RTRIM(ISNULL(Product_Code1_Quantity,''))) as Product_Code1_Quantity,  
  LTRIM(RTRIM(ISNULL(Product_Code1_Amount,''))) as Product_Code1_Amount,  
  LTRIM(RTRIM(ISNULL(Product_Code2,''))) as Product_Code2,  
  LTRIM(RTRIM(ISNULL(Product_Code2_description,''))) as Product_Code2_description,  
  LTRIM(RTRIM(ISNULL(Product_Code2_Quantity,''))) as Product_Code2_Quantity,  
  LTRIM(RTRIM(ISNULL(Product_Code2_Amount,''))) as Product_Code2_Amount,  
  LTRIM(RTRIM(ISNULL(Product_Code3,''))) as Product_Code3,  
  LTRIM(RTRIM(ISNULL(Product_Code3_description,''))) as Product_Code3_description,  
  LTRIM(RTRIM(ISNULL(Product_Code3_Quantity,''))) as Product_Code3_Quantity,  
  LTRIM(RTRIM(ISNULL(Product_Code3_Amount,''))) as Product_Code3_Amount,  
  LTRIM(RTRIM(ISNULL(Product_Code4,''))) as Product_Code4,  
  LTRIM(RTRIM(ISNULL(Product_Code4_description,''))) as Product_Code4_description,  
  LTRIM(RTRIM(ISNULL(Product_Code4_Quantity,''))) as Product_Code4_Quantity,  
  LTRIM(RTRIM(ISNULL(Product_Code4_Amount,''))) as Product_Code4_Amount,  
  LTRIM(RTRIM(ISNULL(Product_Code5,''))) as Product_Code5,  
  LTRIM(RTRIM(ISNULL(Product_Code5_description,''))) as Product_Code5_description,  
  LTRIM(RTRIM(ISNULL(Product_Code5_Quantity,''))) as Product_Code5_Quantity,  
  LTRIM(RTRIM(ISNULL(Product_Code5_Amount,''))) as Product_Code5_Amount, -- ordval  
  LTRIM(RTRIM(ISNULL(Product_Code6,''))) as Product_Code6,  
  LTRIM(RTRIM(ISNULL(Product_Code6_description,''))) as Product_Code6_description,  
  LTRIM(RTRIM(ISNULL(Product_Code6_Quantity,''))) as Product_Code6_Quantity,  
  LTRIM(RTRIM(ISNULL(Product_Code6_Amount,''))) as Product_Code6_Amount, -- ordval  
  LTRIM(RTRIM(ISNULL(Product_Code7,''))) as Product_Code7,  
  LTRIM(RTRIM(ISNULL(Product_Code7_description,''))) as Product_Code7_description,  
  LTRIM(RTRIM(ISNULL(Product_Code7_Quantity,''))) as Product_Code7_Quantity,  
  LTRIM(RTRIM(ISNULL(Product_Code7_Amount,''))) as Product_Code7_Amount, -- ordval  
  LTRIM(RTRIM(ISNULL(Product_Code8,''))) as Product_Code8,  
  LTRIM(RTRIM(ISNULL(Product_Code8_description,''))) as Product_Code8_description,  
  LTRIM(RTRIM(ISNULL(Product_Code8_Quantity,''))) as Product_Code8_Quantity,  
  LTRIM(RTRIM(ISNULL(Product_Code8_Amount,''))) as Product_Code8_Amount,   
  LTRIM(RTRIM(ISNULL(Product_Code9,''))) as Product_Code9,  
  LTRIM(RTRIM(ISNULL(Product_Code9_description,''))) as Product_Code9_description,  
  LTRIM(RTRIM(ISNULL(Product_Code9_Quantity,''))) as Product_Code9_Quantity,  
  LTRIM(RTRIM(ISNULL(Product_Code9_Amount,''))) as Product_Code9_Amount,   
  LTRIM(RTRIM(ISNULL(Product_Code10,''))) as Product_Code10,  
  LTRIM(RTRIM(ISNULL(Product_Code10_description,''))) as Product_Code10_description,  
  LTRIM(RTRIM(ISNULL(Product_Code10_Quantity,''))) as Product_Code10_Quantity,  
  LTRIM(RTRIM(ISNULL(Product_Code10_Amount,''))) as Product_Code10_Amount,   
  LTRIM(RTRIM(ISNULL(Product_Code11,''))) as Product_Code11,  
  LTRIM(RTRIM(ISNULL(Product_Code11_description,''))) as Product_Code11_description,  
  LTRIM(RTRIM(ISNULL(Product_Code11_Quantity,''))) as Product_Code11_Quantity,  
  LTRIM(RTRIM(ISNULL(Product_Code11_Amount,''))) as Product_Code11_Amount,   
  LTRIM(RTRIM(ISNULL(Product_Code12,''))) as Product_Code12,  
  LTRIM(RTRIM(ISNULL(Product_Code12_description,''))) as Product_Code12_description,  
  LTRIM(RTRIM(ISNULL(Product_Code12_Quantity,''))) as Product_Code12_Quantity,  
  LTRIM(RTRIM(ISNULL(Product_Code12_Amount,''))) as Product_Code12_Amount,   
  LTRIM(RTRIM(ISNULL(Product_Code13,''))) as Product_Code13,  
  LTRIM(RTRIM(ISNULL(Product_Code13_description,''))) as Product_Code13_description,  
  LTRIM(RTRIM(ISNULL(Product_Code13_Quantity,''))) as Product_Code13_Quantity,  
  LTRIM(RTRIM(ISNULL(Product_Code13_Amount,''))) as Product_Code13_Amount,   
  LTRIM(RTRIM(ISNULL(Product_Code14,''))) as Product_Code14,  
  LTRIM(RTRIM(ISNULL(Product_Code14_description,''))) as Product_Code14_description,  
  LTRIM(RTRIM(ISNULL(Product_Code14_Quantity,''))) as Product_Code14_Quantity,  
  LTRIM(RTRIM(ISNULL(Product_Code14_Amount,''))) as Product_Code14_Amount,   
  LTRIM(RTRIM(ISNULL(Product_Code15,''))) as Product_Code15,  
  LTRIM(RTRIM(ISNULL(Product_Code15_description,''))) as Product_Code15_description,  
  LTRIM(RTRIM(ISNULL(Product_Code15_Quantity,''))) as Product_Code15_Quantity,  
  LTRIM(RTRIM(ISNULL(Product_Code15_Amount,''))) as Product_Code15_Amount,   
  LTRIM(RTRIM(ISNULL(Product_Code16,''))) as Product_Code16,  
  LTRIM(RTRIM(ISNULL(Product_Code16_description,''))) as Product_Code16_description,  
  LTRIM(RTRIM(ISNULL(Product_Code16_Quantity,''))) as Product_Code16_Quantity,  
  LTRIM(RTRIM(ISNULL(Product_Code16_Amount,''))) as Product_Code16_Amount,   
  LTRIM(RTRIM(ISNULL(Product_Code17,''))) as Product_Code17,  
  LTRIM(RTRIM(ISNULL(Product_Code17_description,''))) as Product_Code17_description,  
  LTRIM(RTRIM(ISNULL(Product_Code17_Quantity,''))) as Product_Code17_Quantity,  
  LTRIM(RTRIM(ISNULL(Product_Code17_Amount,''))) as Product_Code17_Amount,   
  LTRIM(RTRIM(ISNULL(Product_Code18,''))) as Product_Code18,  
  LTRIM(RTRIM(ISNULL(Product_Code18_description,''))) as Product_Code18_description,  
  LTRIM(RTRIM(ISNULL(Product_Code18_Quantity,''))) as Product_Code18_Quantity,  
  LTRIM(RTRIM(ISNULL(Product_Code18_Amount,''))) as Product_Code18_Amount,   
  LTRIM(RTRIM(ISNULL(Product_Code19,''))) as Product_Code19,  
  LTRIM(RTRIM(ISNULL(Product_Code19_description,''))) as Product_Code19_description,  
  LTRIM(RTRIM(ISNULL(Product_Code19_Quantity,''))) as Product_Code19_Quantity,  
  LTRIM(RTRIM(ISNULL(Product_Code19_Amount,''))) as Product_Code19_Amount,   
  LTRIM(RTRIM(ISNULL(Product_Code20,''))) as Product_Code20,  
  LTRIM(RTRIM(ISNULL(Product_Code20_description,''))) as Product_Code20_description,  
  LTRIM(RTRIM(ISNULL(Product_Code20_Quantity,''))) as Product_Code20_Quantity,  --jec 20/05/08 UAT183  
  LTRIM(RTRIM(ISNULL(Product_Code20_Amount,''))) as Product_Code20_Amount,  --jec 20/05/08 UAT183  
  convert(decimal(12,2),Agreement.Agrmttotal - servicechg) as Product_Total_Value,  
  0 as Buff_Number, --***#TMBuffNo.buffNo   -- jec 13/05/08 uat 183  
  ISNULL(Proposal.ScoreCardNo,'') as Scorecard_Used,  -- jec 13/05/08 uat 183  
  ISNULL(Bank.BankCode,'') as Name_Bank, --Bank.BankName  -- jec 13/05/08 uat 183  
  -- following 3 field must be defined as char of correct size  
  ' ' as Type_of_bank, --   ***bankacct.Code   --jec 20/05/08 UAT183  
  '    ' as Time_bank, --  ***right(CONVERT(char(6), bankacct.Dateopened, 112),4)  --jec 20/05/08 UAT183  
  '               ' as Bank_Account_Number, --  ***bankacct.Bankacctno   --jec 20/05/08 UAT183  
  replace(ISNULL(app2WorkAddress.Cusaddr1,''),',',' ') as App2_Employer_Name,    -- jec 13/05/08 uat 183  
  replace(ISNULL(app2WorkAddress.Cusaddr2,''),',',' ') as App2_Employer_Address_Line1, -- jec 13/05/08 uat 183  
  replace(ISNULL(app2WorkAddress.Cusaddr3,''),',',' ') as App2_Employer_Address_Line2, -- jec 13/05/08 uat 183  
replace(ISNULL(app2WorkAddress.Cuspocode,''),',',' ') as App2_Employer_Address_Line3, -- jec 13/05/08 uat 183  
  left(replace(LTRIM(RTRIM(ISNULL(app2WorkTel.DialCode,'') + replace(ISNULL(app2WorkTel.TelNo,''),' ',''))),' ',0),10) as App2_Employer_Tel_No, -- jec 13/05/08 uat 183  
  null as App2_Employer_Tel_Ext,     
  replace(ISNULL(App2Employment.Department,''),',',' ') as App2_Department,  -- jec 13/05/08 uat 183  
  LTRIM(RTRIM(ISNULL(App2Employment.StaffNo,''))) as App2_Staff_Number,  -- jec 13/05/08 uat 183  
  ISNULL(App2Employment.WorkType,'') as App2_Occupation,  -- jec 13/05/08 uat 183  
  ISNULL(CONVERT(VARCHAR(4), App2Employment.Dateemployed, 12),'') as App2_Time_Current_Employment,  -- jec 13/05/08 uat 183  
  RTRIM(Proposal.A2Relation) as Relationship_App1,  
  Agreement.Empeenosale as Salesperson_number,  
  LTRIM(RTRIM(ISNULL(Product_Code1_Description_2,''))) as Product_Code1_Description_2,  --jec 20/05/08 UAT183 LTRIM(RTRIM(   
  LTRIM(RTRIM(ISNULL(Product_Code2_Description_2,''))) as Product_Code2_Description_2,  
  LTRIM(RTRIM(ISNULL(Product_Code3_Description_2,''))) as Product_Code3_Description_2,  
  LTRIM(RTRIM(ISNULL(Product_Code4_Description_2,''))) as Product_Code4_Description_2,  
  LTRIM(RTRIM(ISNULL(Product_Code5_Description_2,''))) as Product_Code5_Description_2,  
  LTRIM(RTRIM(ISNULL(Product_Code6_Description_2,''))) as Product_Code6_Description_2,  
  LTRIM(RTRIM(ISNULL(Product_Code7_Description_2,''))) as Product_Code7_Description_2,  
  LTRIM(RTRIM(ISNULL(Product_Code8_Description_2,''))) as Product_Code8_Description_2,  
  LTRIM(RTRIM(ISNULL(Product_Code9_Description_2,''))) as Product_Code9_Description_2,  
  LTRIM(RTRIM(ISNULL(Product_Code10_Description_2,''))) as Product_Code10_Description_2,  
  LTRIM(RTRIM(ISNULL(Product_Code11_Description_2,''))) as Product_Code11_Description_2,  
  LTRIM(RTRIM(ISNULL(Product_Code12_Description_2,''))) as Product_Code12_Description_2,  
  LTRIM(RTRIM(ISNULL(Product_Code13_Description_2,''))) as Product_Code13_Description_2,  
  LTRIM(RTRIM(ISNULL(Product_Code14_Description_2,''))) as Product_Code14_Description_2,  
  LTRIM(RTRIM(ISNULL(Product_Code15_Description_2,''))) as Product_Code15_Description_2,  
  LTRIM(RTRIM(ISNULL(Product_Code16_Description_2,''))) as Product_Code16_Description_2,  
  LTRIM(RTRIM(ISNULL(Product_Code17_Description_2,''))) as Product_Code17_Description_2,  
  LTRIM(RTRIM(ISNULL(Product_Code18_Description_2,''))) as Product_Code18_Description_2,  
  LTRIM(RTRIM(ISNULL(Product_Code19_Description_2,''))) as Product_Code19_Description_2,  
  LTRIM(RTRIM(ISNULL(Product_Code20_Description_2,''))) as Product_Code20_Description_2,  --jec 20/05/08 UAT183 LTRIM(RTRIM(   
  '01' as New_Due_Date,  
  convert(decimal(12,2),Customer.Rfcreditlimit) as RF_Recommended_Spend_Limit,  
   --null as Aricy_Courts_Option_Flag  
  case  -- jec 20/05/08 UAT 183 G,S,B code  
   when acct.termstype in('01','02','03') then LEFT(termstype.Description,1)   
  end as Aricy_Courts_Option_Flag     
  
 into tempTransactAmend-- jec 22/05/08  
 from acct  
	INNER JOIN #updatedAccounts ua
		ON	acct.acctno = ua.acctno
  inner join custacct  
   on acct.acctno = custacct.acctno and hldorjnt = 'H'  
    
  INNER JOIN customer   
   on custacct.custid = customer.custid  
  inner join custaddress  
   on customer.custid = custAddress.custid and addtype='H' and datemoved is null  
  left outer  join custaddress workAddress  
   on customer.custid = workAddress.custid and workAddress.addtype='W' and workAddress.datemoved is null  
  left outer  join custaddress deliveryAddress  
   on customer.custid = deliveryAddress.custid and deliveryAddress.addtype='H' and deliveryAddress.datemoved is null  
  LEFT outer join custtel  
   on customer.custid = custtel.custid and custtel.tellocn = 'H' and datediscon is null  
  left outer join custtel mobileTel  
   on customer.custid = mobileTel.custid and mobileTel.tellocn = 'M' and mobileTel.datediscon is null  
  left outer join custtel employerTel  
   on customer.custid = employerTel.custid and employerTel.tellocn = 'W' and employerTel.datediscon is null  
  inner join Proposal  
   on acct.acctno = proposal.acctno  and customer.custid=proposal.custid 
  inner join Agreement  
   on acct.acctno = Agreement.acctno  
  left outer join Employment  
   on customer.custid = employment.custid and employment.dateleft is null  --IP - 22/02/10 - CR1072 - LW 70691 - Tallyman Fixes from 4.3 - Merge 
  left outer join custacct otherApplicant  
   on acct.acctno = otherApplicant.acctno and otherApplicant.hldorjnt != 'H'  
  left outer join customer app2   
   on otherApplicant.custid  =app2.custid  
  left outer join custaddress app2Address  
   on otherApplicant.custid = app2Address.custid and app2Address.addtype='H' and app2Address.datemoved is null  
  left outer  join custaddress app2WorkAddress  
   on otherApplicant.custid = app2WorkAddress.custid and app2WorkAddress.addtype='W' and app2workAddress.datemoved is null  
  left outer join custtel app2homeTel  
   on otherApplicant.custid = app2homeTel.custid and app2homeTel.tellocn = 'H' and app2homeTel.datediscon is null  
  left outer join custtel app2WorkTel  
   -- jec 15/05/08 correct table name on join  
   --on otherApplicant.custid = app2homeTel.custid and app2homeTel.tellocn = 'W' and app2homeTel.datediscon is null  
   on otherApplicant.custid = app2WorkTel.custid and app2WorkTel.tellocn = 'W' and app2WorkTel.datediscon is null  
  left outer join Employment app2Employment  
   on otherApplicant.custid = app2Employment.custid and app2Employment.dateleft is null 
  
  left outer join ProposalRef Ref1  
   on acct.acctno = Ref1.acctno and refno = 1  
  left outer join ProposalRef Ref2  
   on acct.acctno = Ref2.acctno and Ref2.refno = 2  
  left outer join ProposalRef Ref3  
   on acct.acctno = Ref3.acctno and Ref3.refno = 3  
  left outer join ProposalRef Ref4  
   on acct.acctno = Ref4.acctno and Ref4.refno = 4  
  left outer join ProposalRef Ref5  
   on acct.acctno = Ref5.acctno and Ref5.refno = 5  
  left outer join scoretrak  
   on acct.acctno = scoretrak.acctno  
  inner join instalplan  
   on acct.acctno = instalplan.acctno  
  LEFT outer join TM_Product  
   on acct.acctno = TM_Product.Account_Number  
  left outer join bank  
   on proposal.bankcode = bank.bankcode  
  left outer join bankacct  
   on customer.custid = bankacct.custid  
  LEFT outer join termstype   
   on acct.termstype=termstype.TermsType  -- jec 20/05/08 UAT 183 link to get G,S,B code  


-- convert empty columns to null prior to BCP  -- jec 22/05/08  
exec empty_to_null tempTransactAmend  
  
truncate table tallytoexport
  
set @path = 'BCP "select CONVERT(CHAR(10), GetDate(), 23),Count(*) from  ' + DB_NAME() + '.dbo.tempTransactAmend "  queryout ' +  
 'D:\tallyman\operations\interface\debt\temp\Transactamendhdr.csv ' + '-c -t, -q -Usa -P'  
exec master.dbo.xp_cmdshell @path  
  
set @path = 'BCP "select * from  ' + DB_NAME() + '.dbo.tempTransactAmend "  queryout ' +  
 'D:\tallyman\operations\interface\debt\temp\TransactamendBdy.csv ' + '-c -t, -q -Usa -P'  
exec master.dbo.xp_cmdshell @path  
  
-- merge to produce final file  
declare @statement sqltext -- varchar(6000)  
set @statement ='copy D:\tallyman\operations\interface\debt\temp\Transactamendhdr.csv + D:\tallyman\operations\interface\debt\temp\TransactamendBdy.csv D:\tallyman\operations\interface\debt\Transactamend.csv '  
exec master.dbo.xp_cmdshell @statement   
  
SET @err = @@ERROR   
  
set @transactAmendPassFail = 'P'  
IF @err != 0  
begin  
 set @PassFail = 'F'  
 set @transactAmendPassFail = 'F'  
end  
  
insert into interfacecontrol  
  (interface,runno,datestart,datefinish,result)  
 values  
  ('TRANSACTAM', @runno,@startdate ,@enddate, @transactAmendPassFail)  
  
  
drop table tempTransactAmend  
  
  
  
insert into interfacecontrol  
(interface,runno,datestart,datefinish,result)  
values  
('TALLYDAILY', @runno,@startdate ,@enddate,'P')  
end  


GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End