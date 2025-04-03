

if not exists (select * from information_schema.tables where table_name = 'tallyweeklyexportlog')
begin
create table tallyweeklyexportlog
(acctno char(12) not null,
points int,
salesperson int,
runno smallint not null)

create clustered index ix_tallyweeklyexportlog on tallyweeklyexportlog(runno,acctno) 
end
GO

/****** Object:  StoredProcedure [dbo].[TallymanWeeklyExport]    Script Date: 07/04/2008 22:09:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF EXISTS (SELECT * FROM SYSOBJECTS WHERE NAME = 'TallymanWeeklyExport')
DROP PROCEDURE [TallymanWeeklyExport]
GO

CREATE   proc [dbo].[TallymanWeeklyExport]  
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
-- 13/05/08  jec UAT183 Replace NULL values with blanks.  
-- 16/05/08  jec UAT219 return 0 if termtype apr is ' '.  
-- 20/05/08  jec UAT183 remove white space  
-- 22/05/08  jec UAT183 add procedure to convert empty columns to null  
-- 04/07/08  AA change the export so that accounts with balance of 0 but with transactions   
-- 29/07/08  RDB lw69946 Tallyamend-outsbal < 0 not in TallyAmend, commented out  
-- within the period have their details exported to tallyman also improve performance for existing accounts   
-- for the totals in the #tables
-- 06/10/10  jec UAT1036 Ensure accounts are always sent to Tallyman if tallmanacct flag is set 
-- ================================================  
 -- Add the parameters for the stored procedure here  
AS  
  
declare @path varchar(200)  
declare @datestart datetime  
set @datestart = getdate()  
  
declare @pcentarrears float,@Tallymandays smallint  
select @pcentarrears = convert (float,value)   
 from countrymaintenance where name ='Tallyman arrears percentage'  
  
-- set up lineitems first  
  
TRUNCATE TABLE TM_Product  
declare @datelastproductupdate datetime  
select @datelastproductupdate = max(datefinish) from interfacecontrol where interface in ('TALLYDAILY','TALLYWEEK' )  
declare @delthres int  
select @delthres= value from countrymaintenance where name='Delivery percent for date first'  
--  
    -- NEW / UPDATE Products  
    --  
    UPDATE  TallymanProducts  
    SET     ExportType = ''  
  
    -- UPDATE Products that have changed since the last run  
    -- or belong to an account that has re-entered arrears  
    UPDATE  tp  
    SET     ExportType = 'U'  
    FROM    TallymanProducts  tp   
  inner join acct ta  
   on tp.AcctNo = ta.acctNo  
  inner join instalplan  
   on ta.acctno = instalplan.acctno  
  INNER JOIN vw_DelAmount  
   ON tp.AcctNo = vw_DelAmount.acctNo  
 WHERE  
  (tp.LastUpdate > @datelastproductupdate)  
  and  ta.acctno like '___0%' and  
   --outstbal >0 and  
   instalplan.datelast >'1-Jan-1910' and  
   --ta.arrears > (instalplan.instalamount * @pcentarrears /100) and ta.arrears >0.1 and   
   vw_DelAmount.DelAmount >= (ta.agrmttotal * @delthres/100) and  
   (ta.tallymanAcct is null or ta.tallyManAcct != 1)  
  
   -- SET @Return = @@ERROR IF @Return != 0 RETURN @Return  
  
    -- NEW Products for each NEW Account  
    -- Make sure the new products are present for a new account  
    -- New products are always sent with a new account  
 print 'inserting into TallymanProducts'  
 declare @EndDate datetime  
 set @EndDate = GetDate()  
  
    INSERT INTO TallymanProducts  
            (AcctNo, LastUpdate, ExportType)  
    SELECT  DISTINCT ta.acctno, @EndDate, 'N'  
    FROM    acct ta  
  inner join instalplan  
   on ta.acctno = instalplan.acctno  
  INNER JOIN vw_DelAmount  
   ON ta.AcctNo = vw_DelAmount.acctNo  
     
    WHERE   NOT EXISTS (SELECT AcctNo FROM TallymanProducts tp  
                    WHERE  tp.AcctNo = ta.AcctNo)  
   and ta.acctno like '___0%' and  
   --outstbal >0 and  
   instalplan.datelast >'1-Jan-1910' and  
   --ta.arrears > (instalplan.instalamount * @pcentarrears /100) and   
   --ta.arrears >0.1 and   
   vw_DelAmount.DelAmount >= (ta.agrmttotal * @delthres/100) and  
   (ta.tallymanAcct is null or ta.tallyManAcct != 1)  
  
    --SET @Return = @@ERROR IF @Return != 0 RETURN @Return  
  
    -- NEW Products for a NEW Account  
    UPDATE  tp  
    SET     ExportType = 'N'  
    FROM    TallymanProducts  tp   
  inner join acct ta  
   on tp.AcctNo = ta.acctNo  
  inner join instalplan  
   on ta.acctno = instalplan.acctno  
  INNER JOIN vw_DelAmount  
   ON tp.AcctNo = vw_DelAmount.acctNo      WHERE   ta.acctno like '___0%' and  
   --outstbal >0 and  
   instalplan.datelast >'1-Jan-1910' and  
   --ta.arrears > (instalplan.instalamount * @pcentarrears /100) and   
   --ta.arrears >0.1 and   
   vw_DelAmount.DelAmount >= (ta.agrmttotal * @delthres/100) and  
   (ta.tallymanAcct is null or ta.tallyManAcct != 1)  
  
    --SET @Return = @@ERROR IF @Return != 0 RETURN @Return  
  
  
    INSERT INTO TM_Product  
           (InsertUpdate,  
            Account_Number,  
            CustomerIC_Number)  
    SELECT  tp.ExportType,  
            tp.AcctNo,  
            substring(ltrim(ca.CustId),1,12)  
    FROM    TallymanProducts tp, CustAcct ca  
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
    -- AND     (s.ItemType = 'S' OR s.Category = 11 OR (s.Category >= 51 AND s.Category <= 59))  
    GROUP BY d.AcctNo, d.ItemNo, d.StockLocn, s.Category  
    HAVING SUM(d.TransValue) > 0 AND SUM(d.Quantity) > 0  
    ORDER BY d.AcctNo ASC, SUM(d.TransValue) DESC  
  
    UPDATE  #TMLineItem  
    SET     SeqNo = LineId + 1 - (SELECT MIN(t2.LineId) FROM #TMLineItem t2  
                                  WHERE  t2.Acctno = #TMLineItem.AcctNo)  
  
  
-- quantity overflow error 70345  
    UPDATE  #TMLineItem set quantity = 99 where quantity >99  
  
-- get min buffno for delivereditems by account  
select delivery.acctno, min(buffno) BuffNo   
 into    #TMBuffNo  
 from delivery  
 inner join #TMLineItem  
  on delivery.acctno = #TMLineItem.acctno  
  and delivery.itemno = #TMLineItem.itemno  
  where delorcoll = 'D'  
  group by delivery.acctno  
  
CREATE CLUSTERED INDEX ind_temp ON #TMBuffNo(acctno)  
  
  
  
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
    PRINT 'Product Category ... '  
    --  
  
    PRINT 'For RF accounts Product Category = Proposal.RFCategory ... '  
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
  
    PRINT 'Non RF accounts Product Category = Most expensive item ... '  
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
            Product_Code13_Description      = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(Product_Code13_Description,',',' '),'\t',' '),'\n',' '),'\r',' '),'\0',' '),  
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
  
  
  
  
  
  
  
  
  
  
  
  
-- get delivered total by account  
select f.acctno, sum(Transvalue) as Delivery_Amount  
 into #TMDelivery  
 from Fintrans f  
   JOIN acct a ON f.acctno = a.acctno  
 where f.transtypecode in ('DEL','GRT','ADD') AND a.tallymanacct ='1'  
 group by f.acctno  
  
  
-- get balance paid by account  
select f.acctno, sum(Transvalue) as Total  
 into #TMBalancePaid  
 from Fintrans f  
   JOIN acct a ON f.acctno = a.acctno  
 where f.transtypecode not in ('DEL','GRT','ADD')  
   AND a.tallymanacct='1'  
 group by f.acctno  
  
  
-- get total goods returned buy account  
select f.acctno, sum(TransValue) as Total  
 into #TMGoodsReturned  
 from FinTrans f   
   JOIN acct a ON f.acctno = a.acctno  
 where f.transtypecode = 'GRT'    AND a.tallymanacct='1'  
 group by f.acctno  
  
-- get date_last_movement  
select f.acctno, max(Datetrans) as Date  
 into #TMLastMovement  
 from FinTrans f  
   JOIN acct a ON f.acctno = a.acctno  
   WHERE a.tallymanacct='1'  
 group by f.acctno  
  
  
select a.acctno, min(datestatchge) as Date   
 into #TMMinStatusChanged  
 from status s   
   JOIN acct a ON s.acctno = a.acctno  
 where s.statuscode = '5' AND a.tallymanacct='1'  
 group by a.acctno  
  
select acctno, max(datestatchge) as Date   
 into #TMMaxStatusChanged  
 from status  
 where statuscode = 'S'  
 group by acctno  
  
select acctno , sum(TransValue) Total  
 into #TMInterest  
 from FinTrans  
 where FinTrans.transtypecode = 'INT'  
 group by acctno  
  
  
  
--******************** Begin TM_Account SetUp  ****************************************  
  
  
declare @Return  int  
DECLARE @DateNow DATETIME  
  
-- Back date by nine hours in case it is run at midnight  
SET @DateNow = DATEADD(Hour, -9, GETDATE())  
  
TRUNCATE TABLE TM_Account  
  
 INSERT INTO TM_Account  
           (InsertUpdate,  
            Account_Number,  
            Account_Open_Date,  
            Agreement_Account_Type,  
            Arrears,  
            Balance_OutStanding,  
            Branch_No,  
            Date_of_Last_Payment,  
            Highest_Status,  
            Percent_Paid,  
            Settled_Indicator,  
            Status,  
            Terms_Type,  
            Settled_Week,  
            Arrears_Flag)  
   SELECT  '',--ta.ExportType,  
            a.Acctno,  
            ISNULL(CONVERT(VARCHAR(10),a.DateAcctOpen,120),''),  
            ISNULL(a.AcctType,''),  
            ISNULL(CONVERT(DECIMAL(15,2),a.Arrears),'0.00'),  
            ISNULL(CONVERT(DECIMAL(15,2),a.OutStBal),'0.00'),  
            ISNULL(CONVERT(DECIMAL(3,0),a.BranchNo),'0'),  
            ISNULL(CONVERT(VARCHAR(10),a.DateLastPaid,120),''),  
            ISNULL(CONVERT(DECIMAL(2,0),CASE WHEN ISNUMERIC(a.HighstStatus) = 1 THEN a.HighstStatus ELSE '0' END),'0'),  
            ISNULL(CONVERT(DECIMAL(3,0),a.PaidPcent),'0'),  
            CASE WHEN ISNULL(a.CurrStatus,'') = 'S' THEN 'S' ELSE '' END,  
            ISNULL(CONVERT(DECIMAL(2,0),CASE WHEN ISNUMERIC(a.CurrStatus) = 1 THEN a.CurrStatus ELSE '0' END),'0'),  
            ISNULL(a.TermsType,''),  
        '',  
            'N' --CASE WHEN ISNULL(ta.Removed,'') = '' THEN 'Y' ELSE 'N' END  
    /*FROM    --TallymanAcctArrears ta,   
   Acct a  
    WHERE   ta.ExportType != ''  
    AND     a.AcctNo = ta.AcctNo*/  
  
    FROM acct a  
  inner join instalplan  
   on a.acctno = instalplan.acctno  
  INNER JOIN vw_DelAmount  
   ON a.AcctNo = vw_DelAmount.acctNo  
 WHERE  
   a.acctno like '___0%' and  
   --outstbal >0 and  
   instalplan.datelast >'1-Jan-1910' and  
   vw_DelAmount.DelAmount >= (a.agrmttotal * 75 /100) and  
   (a.tallymanAcct is null or a.tallyManAcct != 1)  
  
--    --SET @Return = @@ERROR IF @Return != 0 RETURN @Return  
  
  
    PRINT 'Agreement ... '  
    UPDATE  TM_Account  
    SET     Agreement_Account_Total = ISNULL(CONVERT(DECIMAL(15,2),ag.AgrmtTotal),'0.00'),  
            Agreement_Cash_Price    = ISNULL(CONVERT(DECIMAL(15,2),ag.CashPrice),'0.00'),  
            Deferred_Terms_Charge   = ISNULL(CONVERT(DECIMAL(15,2),ag.ServiceChg),'0.00'),  
            Delivery_Date           = ISNULL(CONVERT(VARCHAR(10),ag.DateDel,120),''),  
            Deposit_Amount          = ISNULL(CONVERT(DECIMAL(15,2),ag.Deposit),'0.00'),  
            Next_Due_Date           = ISNULL(CONVERT(VARCHAR(10),ag.DateNextDue,120),''),  
            Salesperson_Number      = ISNULL(CONVERT(DECIMAL(5,0),ag.Empeenosale),'0')  
    FROM    Agreement ag  
    WHERE   ag.AcctNo = TM_Account.Account_Number  
    AND     ag.AgrmtNo = 1  
  
--    --SET @Return = @@ERROR IF @Return != 0 RETURN @Return  
  
    PRINT 'InstalPlan ... '  
    UPDATE  TM_Account  
    SET     Due_Day             = ISNULL(CONVERT(DECIMAL(2,0),ip.DueDay),'0'),  
            Final_Instalment    = ISNULL(CONVERT(DECIMAL(15,2),ip.FinInstalAmt),'0.00'),  
            First_Due_Date      = ISNULL(CONVERT(VARCHAR(10),ip.DateFirst,120),''),  
            Instalment_Amount   = ISNULL(CONVERT(DECIMAL(15,2),ip.InstalAmount),'0.00'),  
            Last_Due_Date       = ISNULL(CONVERT(VARCHAR(10),ip.DateLast,120),''),  
            Number_Instalments  = ISNULL(CONVERT(DECIMAL(3,0),ip.InstalNo),'0')  
    FROM    InstalPlan ip  
    WHERE   ip.AcctNo = TM_Account.Account_Number  
  
    --SET @Return = @@ERROR IF @Return != 0 RETURN @Return  
  
    PRINT 'Proposal ... '  
    UPDATE  TM_Account  
    SET     Application_Date    = ISNULL(CONVERT(VARCHAR(10),p.DateProp,120),''),  
            CustomerIC_Number   = substring(ltrim(p.CustId),1,12)  
    FROM    Proposal p  
    WHERE   p.AcctNo = TM_Account.Account_Number  
    AND     p.DateProp = (SELECT MAX(p2.DateProp) FROM Proposal p2  
                          WHERE  p2.AcctNo = TM_Account.Account_Number)  
  
    --SET @Return = @@ERROR IF @Return != 0 RETURN @Return  
  
    PRINT 'LineItem ... '  
    UPDATE  TM_Account  
    SET     Location_Lookup1 = ISNULL((SELECT  CASE WHEN MAX(LEFT(l.DeliveryAddress,1)) = 'H'  
                                                    THEN 'H'  
                                                    ELSE 'O'  
                                               END  
                                       FROM    LineItem l  
                                       WHERE   l.AcctNo = TM_Account.Account_Number),'0')  
  
    --SET @Return = @@ERROR IF @Return != 0 RETURN @Return  
  
    PRINT 'Status Last Settled ... '  
    UPDATE  TM_Account  
    SET     Date_Last_Settled = ISNULL((SELECT  CONVERT(VARCHAR(10),MAX(s.DateStatChge),120)  
                                        FROM    Status s  
                                        WHERE   s.AcctNo = TM_Account.Account_Number  
                                        AND     s.StatusCode = 'S'),'')  
  
    --SET @Return = @@ERROR IF @Return != 0 RETURN @Return  
  
    PRINT 'Status to SC5 ... '  
    UPDATE  TM_Account  
    SET     Date_To_SC5 = ISNULL((SELECT  CONVERT(VARCHAR(10),MIN(s.DateStatChge),120)  
                                  FROM    Status s  
                                  WHERE   s.AcctNo = TM_Account.Account_Number  
                                AND     s.StatusCode = '5'),'')  
  
    --SET @Return = @@ERROR IF @Return != 0 RETURN @Return  
  
    PRINT 'Status Settled This Week ... '  
    UPDATE  TM_Account  
    SET     Settled_Week = ISNULL(  
            (SELECT  CASE WHEN MAX(s.DateStatChge)  
                               BETWEEN DATEADD(Day,-7,@DateNow) AND @DateNow  
                          THEN 'Y' ELSE 'N'  
                     END  
             FROM    Status s  
             WHERE   s.AcctNo = TM_Account.Account_Number  
             AND     s.StatusCode = 'S'),'')  
  
    --SET @Return = @@ERROR IF @Return != 0 RETURN @Return  
  
  
    --  
    PRINT 'Account CALCULATIONS ... '  
    --  
  
    PRINT 'Admin_Charge = SUM(ADM) transactions ... '  
    UPDATE  TM_Account  
    SET     Admin_Charge = ISNULL(CONVERT(DECIMAL(15,2),  
                           (SELECT  SUM(f.TransValue)  
                            FROM    FinTrans f  
                            WHERE   f.AcctNo = TM_Account.Account_Number  
                            AND     f.TransTypeCode = 'ADM')), '0.00')  
      PRINT 'Admin_Charge = SUM(ADM) transactions ... '  
  
    SELECT     acctno,  ISNULL(CONVERT(DECIMAL(15,2),SUM(f.TransValue)),0.00) as Admin_Charge
    into #tm_admin  
                            FROM    FinTrans f  
                            WHERE   f.TransTypeCode = 'ADM' 
                            group by acctno 
    --SET @Return = @@ERROR IF @Return != 0 RETURN @Return  
  
    PRINT 'Amount_Paid = SUM transaction list ... '  
    UPDATE  TM_Account  
    SET     Amount_Paid =  ISNULL(CONVERT(DECIMAL(15,2),  
                           (SELECT  SUM(f.TransValue)  
                            FROM    FinTrans f  
                            WHERE   f.AcctNo = TM_Account.Account_Number  
                            AND     f.TransTypeCode IN ('ADJ', 'ADX', 'COR', 'DDE', 'DD', 'DDR',  
                                                        'DPY', 'INS', 'JLX', 'OVE', 'PAY', 'PEX',  
                                                        'REB', 'REF', 'RET', 'SCX', 'SHO', 'XFR'))), '0.00')  
  
    --SET @Return = @@ERROR IF @Return != 0 RETURN @Return  
  
    PRINT 'Arrears_Interest = SUM(INT) transactions ... '  
    UPDATE  TM_Account  
    SET     Arrears_Interest = ISNULL(CONVERT(DECIMAL(15,2),  
                               (SELECT  SUM(f.TransValue)  
                                FROM    FinTrans f  
                                WHERE   f.AcctNo = TM_Account.Account_Number  
                                AND     f.TransTypeCode = 'INT')), '0.00')  
  
    --SET @Return = @@ERROR IF @Return != 0 RETURN @Return  
  
    PRINT 'Bailiff_Fees = SUM(FEE) transactions ... '  
    UPDATE  TM_Account  
    SET     Bailiff_Fees = ISNULL(CONVERT(DECIMAL(15,2),  
                           (SELECT  SUM(f.TransValue)  
                            FROM    FinTrans f  
                            WHERE   f.AcctNo = TM_Account.Account_Number  
                            AND     f.TransTypeCode = 'FEE')), '0.00')  
  
    --SET @Return = @@ERROR IF @Return != 0 RETURN @Return  
  
  
    PRINT 'Date_of_Last_Movement = Date of last transaction ... '  
    UPDATE  TM_Account  
    SET     Date_of_Last_Movement = ISNULL((SELECT  CONVERT(VARCHAR(10),MAX(f.DateTrans),120)  
                                            FROM    FinTrans f  
                                            WHERE   f.AcctNo = TM_Account.Account_Number),'')  
  
    --SET @Return = @@ERROR IF @Return != 0 RETURN @Return  
  
    PRINT 'Delivery_Amount = SUM(DEL, GRT, ADD) ... '  
    UPDATE  TM_Account  
    SET     Delivery_Amount = ISNULL(CONVERT(DECIMAL(15,2),  
                              (SELECT  SUM(f.TransValue)  
                               FROM    FinTrans f  
                               WHERE   f.AcctNo = TM_Account.Account_Number  
                               AND     f.TransTypeCode IN ('DEL', 'GRT', 'ADD'))), '0.00')  
  
    --SET @Return = @@ERROR IF @Return != 0 RETURN @Return  
  
    PRINT 'Goods_Returned = SUM(GRT) transactions ... '  
    UPDATE  TM_Account  
    SET     Goods_Returned = ISNULL(CONVERT(DECIMAL(15,2),  
                             (SELECT  SUM(f.TransValue)  
                              FROM    FinTrans f  
                              WHERE   f.AcctNo = TM_Account.Account_Number  
                              AND     f.TransTypeCode = 'GRT')), '0.00')  
  
    --SET @Return = @@ERROR IF @Return != 0 RETURN @Return  
  
    PRINT 'Trace_Fees = SUM(TRC) transactions ... '  
    UPDATE  TM_Account  
    SET     Trace_Fees = ISNULL(CONVERT(DECIMAL(15,2),  
                         (SELECT  SUM(f.TransValue)  
                          FROM    FinTrans f  
                          WHERE   f.AcctNo = TM_Account.Account_Number  
                          AND     f.TransTypeCode = 'TRC')), '0.00')  
  
    --SET @Return = @@ERROR IF @Return != 0 RETURN @Return  
  
    --  
    -- Balance Due  
    --  
    PRINT 'Balance_Due = Instalment * no months since Date First + Deposit ... '  
    UPDATE  TM_Account  
    SET     Balance_Due = ISNULL(CONVERT(DECIMAL(15,2),  
                          CASE WHEN DAY(ip.DateFirst) >= DAY(@DateNow)  
                               THEN DATEDIFF(Month,ip.DateFirst,@DateNow) * ip.InstalAmount + ag.Deposit  
                               ELSE (DATEDIFF(Month,ip.DateFirst,@DateNow) + 1) * ip.InstalAmount + ag.Deposit  
                          END  
                          ), '0.00')  
    FROM    Agreement ag, InstalPlan ip  
    WHERE   ag.AcctNo = TM_Account.Account_Number  
    AND     ip.AcctNo = TM_Account.Account_Number  
  
    --SET @Return = @@ERROR IF @Return != 0 RETURN @Return  
  
 --  
    -- Cursor updates  
    --  
    PRINT 'CALL Rebate and MPR function per account ... '  
    SET NOCOUNT ON -- improve performance   
    DECLARE @AcctNo             CHAR(12)  
    DECLARE @Rebate             MONEY  
    DECLARE @RebateWithin12Mths MONEY  
    DECLARE @RebateAfter12Mths  MONEY  
    DECLARE @MPR                FLOAT  
  
    DECLARE TM_AcctList_Csr CURSOR LOCAL  
    FOR  
        SELECT Account_Number FROM TM_Account  
  
    OPEN TM_AcctList_Csr  
  
    FETCH NEXT FROM TM_AcctList_Csr  
    INTO @AcctNo  
  
    --SET @Return = @@ERROR IF @Return != 0 RETURN @Return  
  
    WHILE @@FETCH_STATUS = 0  
    BEGIN  
  
        -- No rebate for a  Cash Account  
        IF SUBSTRING(@AcctNo, 4, 1) != '4'  
        BEGIN  
            -- Rebate Due  
            EXEC DN_RebateSP  
                @AcctNo,  
                @Rebate             OUTPUT,  
                @RebateWithin12Mths OUTPUT,  
                @RebateAfter12Mths  OUTPUT,  
                @Return             OUTPUT  
  
            IF @Return != 0 SET @Rebate = 0  
        END  
        ELSE SET @Rebate = 0  
  
        UPDATE  TM_Account SET Rebate_Due = ISNULL(CONVERT(DECIMAL(15,2),@Rebate), '0.00')  
        WHERE   Account_Number = @AcctNo  
  
        --SET @Return = @@ERROR IF @Return != 0 RETURN @Return  
  
        -- No MPR for a  Cash Account  
        IF SUBSTRING(@AcctNo, 4, 1) != '4'  
        BEGIN  
            -- MPR  
            EXEC TM_MPRCalcSP  
                @AcctNo,  
                @MPR                OUTPUT,  
                @Return             OUTPUT  
  
            --IF @Return != 0 RETURN @Return  
        END  
        ELSE SET @MPR = 0  
  
        UPDATE  TM_Account SET MPR = ISNULL(CONVERT(DECIMAL(15,2),@MPR), '0.00')  
        WHERE   Account_Number = @AcctNo  
  
        --SET @Return = @@ERROR IF @Return != 0 RETURN @Return  
  
        FETCH NEXT FROM TM_AcctList_Csr  
        INTO @AcctNo  
  
        --SET @Return = @@ERROR IF @Return != 0 RETURN @Return  
  
    END  
  
    CLOSE TM_AcctList_Csr  
    DEALLOCATE TM_AcctList_Csr  
  
  
  
  
  
--************************* End TM_Account Setup **************************  
  
PRINT 'Tallynew data ... '  
  
select   
  substring(ltrim(customer.custid),1,12) as CUSTOMERIC_NUMBER,   
  acct.acctno as ACCOUNT_NUMBER,  
  '0.00' as TransactionAmount,   
  CONVERT(CHAR(10), GetDate(), 23) as TransactionDate,  
  'MYR' as Currency,  
  'MS' as Language,  
  Acct.Branchno as Branch_No,  
  right(Left(right(acct.acctno,9),8),5) as Account4to11,  
  left(replace(Customer.Name,',',' '),50) as Customer_Name,  
  Customer.Title as Customer_Title,  
  Left(Customer.firstname,1) as Customer_Initials,  
  replace(custaddress.Cusaddr1,',',' ') as Customer_Address_Line1,  
  replace(custaddress.Cusaddr2,',',' ') as Customer_Address_Line2,  
  replace(custaddress.Cusaddr3,',',' ') as Customer_Address_Line3,  
  replace(custaddress.cuspocode,',',' ') as Customer_Postcode,  
  TraceInd.Code as Trace_indicator,  
  RefuseCredit.Code as Customer_Refuse_Credit,  
  CircIndicator.Code as Circ_indicator  
 into ##TempTallyNew  
 from acct  
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
  INNER JOIN vw_DelAmount  
   ON acct.AcctNo = vw_DelAmount.acctNo  
 where acct.acctno like '___0%' and  
  --outstbal >0 and  
  instalplan.datelast >'1-Jan-1910' and  
  --arrears > (instalplan.instalamount * @pcentarrears /100) and   
  --arrears >0.1 and   
  vw_DelAmount.DelAmount >= (acct.agrmttotal * @delthres /100) and  
  (acct.tallymanAcct is null or acct.tallyManAcct != 1)  
  
  
set @path = 'BCP "select CONVERT(CHAR(10), GetDate(), 23),Count(*), ''0.00'' from ##TempTallyNew "  queryout ' +  
 'D:\tallyman\operations\interface\debt\Tallynewhdr.csv ' + '-c -t, -q -Usa -P'  
exec master.dbo.xp_cmdshell @path  
set @path = 'BCP "select * from ##TempTallyNew "  queryout ' +  
 'D:\tallyman\operations\interface\debt\Tallynew.csv  ' + '-c -t, -q -Usa -P'  
exec master.dbo.xp_cmdshell @path  
  
drop table ##TempTallyNew  
  
--**********************************transact***************************************  
  
  
PRINT 'Transact data ... '  
  
SELECT    
  substring(customer.custid,1,12) as CUSTOMERIC_NUMBER,   
  acct.acctno as ACCOUNT_NUMBER,  
  rtrim(Customer.IDType) as Customer_ID_Type,  
  ISNULL(RTRIM(Proposal.A2relation),'') as Application_Type,   -- jec 13/05/08 uat 183  
  CONVERT(CHAR(10), Proposal.dateprop, 23) as Application_Date,  
  REPLACE(CONVERT(varchar(5), Custaddress.Datein, 108), ':', '') as Customer_Time_Curr_Add_trans,  
  CONVERT(decimal(12,2),Custaddress.Mthlyrent) as Monthly_Mortgage_Rent,  
  RTRIM(Custaddress.resstatus) as Customer_Residential_Status,  
  RTRIM(Custaddress.Proptype) as Customer_Property_Type,  
  ISNULL(left(ltrim(rtrim(Custtel.Telno)),10),'') as Customer_Telephone_Number,  -- jec 13/05/08 uat 183  
  ISNULL(Customer.Alias,'') as Customer_Alias,    -- jec 13/05/08 uat 183  
  ISNULL(left(rtrim(replace(mobileTel.Telno,'-','')),10),'') as Customer_Handphone_Number, -- jec 13/05/08 uat 183  
  CONVERT(CHAR(10), Customer.Dateborn, 23) as Customer_Date_of_Birth,  
  ISNULL(Proposal.Maritalstat,'') as Customer_Marital_Status,  --jec 20/05/08 UAT183  
   --null as Customer_Number_of_Dependents,  --Customer.Dependents as Customer_Number_of_Dependents   
  ISNULL(proposal.dependants,'') as Customer_Number_of_Dependents,  --jec 20/05/08 UAT183  
  RTRIM(Customer.Ethnicity) as Customer_Ethnicity_Code,  
  ISNULL(Employment.Empmtstatus,' ') as Employment_Status,  
  ISNULL(CONVERT(VARCHAR(4), Employment.DateEmployed, 12),' ') as Employment_Time_Current_trans,  
  ISNULL(Employment.Worktype,' ') as Occupation,  
  replace(LTRIM(RTRIM(ISNULL(Proposal.EmpName,''))),',',' ') as Employer_Name,  --jec 20/05/08 UAT183  
  null as Employer_Manager_Name,     
  replace(LTRIM(RTRIM(ISNULL(workAddress.cusaddr1,''))),',',' ') as Employer_Address_Line1, -- rdb lw69888 referencing incorrect table for work address  
  replace(LTRIM(RTRIM(ISNULL(workAddress.cusaddr2,''))),',',' ') as Employer_Address_Line2,   
  replace(LTRIM(RTRIM(ISNULL(workAddress.cusaddr3,''))),',',' ') as Employer_Address_Line3,    
  left(replace(Employment.Persdialcode + Employment.PersTel,' ',''),10) as Employer_Telephone_Number,  -- jec 13/05/08 uat 183  
  left(ltrim(ISNULL(employerTel.Extnno,'')),5) as Employer_Telephone_Number_Ext, -- jec 13/05/08 uat 183  
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
  replace(LTRIM(RTRIM(ISNULL(App2.Name,''))),',',' ') as App2_Customer_Name,   --jec 20/05/08 UAT183  
  replace(LTRIM(RTRIM(ISNULL(App2.Custid,''))),',',' ') as App2_ICNumber,   --jec 20/05/08 UAT183  
  replace(LTRIM(RTRIM(ISNULL(app2Address.Cusaddr1,''))),',',' ') as APP2_Address_line1, --jec 20/05/08 UAT183  
  replace(LTRIM(RTRIM(ISNULL(app2Address.Cusaddr2,''))),',',' ') as APP2_Address_line2, --jec 20/05/08 UAT183  
  replace(LTRIM(RTRIM(ISNULL(app2Address.Cusaddr3,''))),',',' ') as APP2_Address_line3, --jec 20/05/08 UAT183  
  replace(LTRIM(RTRIM(ISNULL(app2Address.cuspocode,''))),',',' ') as App2_Postcode,  --jec 20/05/08 UAT183  
  left(LTRIM(RTRIM(ISNULL(app2homeTel.telno,''))),10) as App2_Telephone_Number, --jec 20/05/08 UAT183  
  CONVERT(decimal(12,2),ISNULL(Proposal.A2MthlyIncome,0)) as App2_Additional_Income,  -- jec 13/05/08 uat 183  
  CONVERT(decimal(12,2),ISNULL(Proposal.A2addincome,0)) as App2_Net_monthly_income,   -- jec 13/05/08 uat 183  
  replace(ISNULL(case when Ref1.Name ='' then ref1.Surname else ref1.name end,''),',',' ') as Reference1_Name,   -- jec 13/05/08 uat 183  
  replace(ISNULL(Ref1.Address1,''),',',' ') as Address_Line1_Ref1,  
  replace(ISNULL(Ref1.Address2,''),',',' ') as Address_Line2_Ref1,  
  replace(ISNULL(Ref1.City,''),',',' ') as Address_Line3_Ref1,  
  left(ltrim(rtrim(ISNULL(Ref1.Tel,''))),10) as Reference1_Telephone_Number,  
  ISNULL(RTRIM(Ref1.Relation),'') as Relationship_to_App1_Ref1,  
  replace(ISNULL(case when Ref2.Name ='' then ref2.Surname else ref2.name end,''),',',' ') as Reference2_Name,  
  replace(ISNULL(Ref2.Address1,''),',',' ') as Address_Line1_Ref2,  
  replace(ISNULL(Ref2.Address2,''),',',' ') as Address_Line2_Ref2,  
  replace(ISNULL(Ref2.City,''),',',' ') as Address_Line3_Ref2,  
  left(ltrim(rtrim(ISNULL(Ref2.Tel,''))),10) as Reference2_Telephone_Number,  
  ISNULL(RTRIM(Ref2.Relation),'') as Relationship_to_App1_Ref2,  
  replace(ISNULL(case when Ref3.Name ='' then ref3.Surname else ref3.name end,''),',',' ') as Reference3_Name,  
  replace(ISNULL(Ref3.Address1,''),',',' ') as Address_Line1_Ref3,  
  replace(ISNULL(Ref3.Address2,''),',',' ') as Address_Line2_Ref3,  
  replace(ISNULL(Ref3.City,''),',',' ') as Address_Line3_Ref3,  
  left(ltrim(rtrim(ISNULL(Ref3.Tel,''))),10) as Reference3_Telephone_Number,  
  ISNULL(RTRIM(Ref3.Relation),'') as Relationship_to_App1_Ref3,  
  replace(ISNULL(case when Ref4.Name ='' then ref4.Surname else ref4.name end,''),',',' ') as Reference4_Name,  
  replace(ISNULL(Ref4.Address1,''),',',' ') as Address_Line1_Ref4,  
  replace(ISNULL(Ref4.Address2,''),',',' ') as Address_Line2_Ref4,  
  replace(ISNULL(Ref4.City,''),',',' ') as Address_Line3_Ref4,  
  left(ltrim(rtrim(ISNULL(Ref4.Tel,''))),10) as Reference4_Telephone_Number,  
  ISNULL(RTRIM(Ref4.Relation),'') as Relationship_to_App1_Ref4,  
  replace(ISNULL(Ref5.Name,''),',',' ') as Reference5_Name,  
  replace(ISNULL(Ref5.Address1,''),',',' ') as Address_Line1_Ref5,  
  replace(ISNULL(Ref5.Address2,''),',',' ') as Address_Line2_Ref5,  
  replace(ISNULL(Ref5.City,''),',',' ') as Address_Line3_Ref5,  
  left(ltrim(rtrim(ISNULL(Ref5.Tel,''))),10) as Reference5_Telephone_Number,  
  ISNULL(RTRIM(Ref5.Relation),'') as Relationship_to_App1_Ref5,  -- jec 13/05/08 uat 183  
  RTRIM(ISNULL(Custaddress.Email,'')) as Customer_Email,  -- jec 13/05/08 uat 183 -- [Customer_E-mail],  
  RTRIM(ISNULL(workAddress.Email,'')) as Customer_Business_Email, -- jec 13/05/08 uat 183 -- [Customer_Business_E-mail]  
  replace(REPLACE(REPLACE(REPLACE(right(Proposal.Propnotes,250), CHAR(10), ''), CHAR(13), ''), CHAR(9), ''),',','') as Underwriters_Comments,  
  replace(deliveryAddress.Cusaddr1,',',' ') as Delivery_Address_line1,  
  replace(deliveryAddress.Cusaddr2,',',' ') as Delivery_Address_line2,  
  replace(deliveryAddress.Cusaddr3 ,',',' ')as Delivery_Address_line3,  
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
  left(replace(LTRIM(RTRIM(ISNULL(app2WorkTel.DialCode,'') + ltrim(rtrim(ISNULL(app2WorkTel.TelNo,''))))),' ',''),10) as App2_Employer_Tel_No, -- jec 13/05/08 uat 183  
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
    
 INTO TempTransact  -- jec 22/05/08   
 from acct  
  
  inner join custacct  
   on acct.acctno = custacct.acctno and hldorjnt = 'H'   
  inner JOIN customer   
   on custacct.custid = customer.custid  
  LEFT outer join custaddress  
   on customer.custid = custAddress.custid and addtype='H' and datemoved is null  
  left outer  join custaddress workAddress  
   on customer.custid = workAddress.custid and workAddress.addtype='W' and workAddress.datemoved is null  
  left outer  join custaddress deliveryAddress  
   on customer.custid = deliveryAddress.custid and deliveryAddress.addtype='H' and deliveryAddress.datemoved is null  
  left outer join custtel  
   on customer.custid = custtel.custid and custtel.tellocn = 'H' and datediscon is null  
  left outer join custtel mobileTel  
   on customer.custid = mobileTel.custid and mobileTel.tellocn = 'M' and mobileTel.datediscon is null  
  left outer join custtel employerTel  
   on customer.custid = employerTel.custid and employerTel.tellocn = 'W' and employerTel.datediscon is null  
  left outer join Proposal  
   on acct.acctno = proposal.acctno  
  inner join Agreement  
   on acct.acctno = Agreement.acctno  
  left outer join Employment  
   on customer.custid = employment.custid and Employment.dateleft is null   
  left outer join custacct otherApplicant  
   on acct.acctno = otherApplicant.acctno and otherApplicant.hldorjnt != 'H'  
  left outer join customer app2   
   on otherApplicant.custid  =app2.custid  
  left outer join custaddress app2Address  
   on otherApplicant.custid = app2Address.custid and app2Address.addtype='H' and app2Address.datemoved is null  
  left outer  join custaddress app2WorkAddress  
   on otherApplicant.custid = app2WorkAddress.custid and app2WorkAddress.addtype='W' and app2WorkAddress.datemoved is null  
  left outer join custtel app2homeTel  
   on otherApplicant.custid = app2homeTel.custid and app2homeTel.tellocn = 'H' and app2homeTel.datediscon is null  
  left outer join custtel app2WorkTel  
   --on otherApplicant.custid = app2homeTel.custid and app2homeTel.tellocn = 'W' and app2homeTel.datediscon is null   
   on otherApplicant.custid = app2WorkTel.custid and app2WorkTel.tellocn = 'W' and app2WorkTel.datediscon is null  -- jec 13/05/08 uat 183  
  left outer join Employment app2Employment  
   on otherApplicant.custid = app2Employment.custid AND app2Employment.dateleft IS null  
  left outer join ProposalRef Ref1  
   on acct.acctno = Ref1.acctno and Ref1.refno = 1  
  left outer join ProposalRef Ref2  
   on acct.acctno = Ref2.acctno and Ref2.refno = 2  
  left outer join ProposalRef Ref3  
   on acct.acctno = Ref3.acctno and Ref3.refno = 3  
  left outer join ProposalRef Ref4  
   on acct.acctno = Ref4.acctno and Ref4.refno = 4  
  left outer join ProposalRef Ref5  
   on acct.acctno = Ref5.acctno and Ref5.refno = 5  
  --left outer join scoretrak  
  -- on acct.acctno = scoretrak.acctno    
  LEFT outer join TM_Product  
   on acct.acctno = TM_Product.Account_Number  
  --left outer join #TMBuffNo  
  -- on acct.acctno = #TMBuffNo.acctno  
  left outer join bank  
   on proposal.bankcode = bank.bankcode  
  --left outer join bankacct  
  -- on customer.custid = bankacct.custid  
  LEFT outer join termstype    
   on acct.termstype=termstype.TermsType  -- jec 20/05/08 UAT 183 link to get G,S,B code  
  left outer join instalplan  
   on acct.acctno = instalplan.acctno  
  left outer JOIN vw_DelAmount  
   ON acct.AcctNo = vw_DelAmount.acctNo  
 where acct.acctno like '___0%' and  
  --outstbal >0 and  
  instalplan.datelast >'1-Jan-1910' and  
  --arrears > (instalplan.instalamount * @pcentarrears /100) and   
  --arrears >0.1 and   
  vw_DelAmount.DelAmount >= (acct.agrmttotal * @delthres /100) and  
  (acct.tallymanAcct is null or acct.tallyManAcct != 1)  
  AND custacct.custid != ''   
  AND proposal.dateprop = (SELECT MAX(dateprop)FROM proposal p WHERE p.acctno = acct.acctno)  
-- sl - need to get max date by account and not customer  
  
  
--CREATE NONCLUSTERED INDEX ind_oacid ON ##TempTransact(OtherApplicantCustId)  
CREATE CLUSTERED INDEX ind_acctno ON TempTransact(ACCOUNT_NUMBER)  
CREATE NONCLUSTERED INDEX ind_custid ON TempTransact(CUSTOMERIC_NUMBER)  
  
  
UPDATE tt  
SET Product_Category = scoretrak.prodcat  
FROM TempTransact tt  
 INNER JOIN scoretrak  
  ON tt.ACCOUNT_NUMBER = scoretrak.acctno  
  
  
UPDATE tt  
SET Buff_Number = #TMBuffNo.buffNo  
FROM TempTransact tt  
 INNER JOIN #TMBuffNo  
  ON tt.ACCOUNT_NUMBER = #TMBuffNo.acctno  
  
UPDATE tt  
 SET Type_of_bank = bankacct.Code,  
  Time_bank = right(CONVERT(char(6), bankacct.Dateopened, 112),4),  
  Bank_Account_Number = bankacct.Bankacctno  
FROM TempTransact tt  
 inner join bankacct  
  on tt.CUSTOMERIC_NUMBER = substring(bankacct.custid,1,12)  
  
  
--UPDATE tt  
-- SET App2_Department = App2Employment.Department,  
--  App2_Staff_Number = App2Employment.StaffNo,  
--  App2_Occupation = App2Employment.WorkType,  
--  App2_Time_Current_Employment = App2Employment.Dateemployed   
--FROM ##temptransact tt  
-- INNER JOIN Employment App2Employment  
--  on tt.OtherApplicantCustId = app2Employment.custid   
--where app2Employment.dateleft IS null  
  
-- convert empty columns to null prior to BCP  -- jec 22/05/08  
exec empty_to_null TempTransact  
  
  
--seperate files not required here but we need to create them   
--and produce one file from both  
set @path = 'BCP "select CONVERT(CHAR(10), GetDate(), 23),Count(*) from ' + DB_NAME() + '.dbo.TempTransact "  queryout ' +  
 'D:\tallyman\operations\interface\debt\temp\Transacthdr.csv ' + '-c -t, -q -Usa -P'  
exec master.dbo.xp_cmdshell @path  
  
set @path = 'BCP "select * from ' + DB_NAME() + '.dbo.TempTransact "  queryout ' +  
 'D:\tallyman\operations\interface\debt\temp\Transactbdy.csv ' + '-c -t, -q -Usa -P'  
exec master.dbo.xp_cmdshell @path  
  
-- merge to produce final file  
declare @statement sqltext -- varchar(6000)  
set @statement ='copy D:\tallyman\operations\interface\debt\temp\Transacthdr.csv + D:\tallyman\operations\interface\debt\temp\Transactbdy.csv D:\tallyman\operations\interface\debt\Transact.csv '  
exec master.dbo.xp_cmdshell @statement   
  
  
-- insert record into interface control  
declare @runno int  
select @runno =  max(runno) from interfacecontrol where interface = 'TALLYWEEK'  
if @runno is null  
 set @runno = 1  
else  
 set @runno = @runno + 1  
  
  
INSERT INTO tallyweeklyexportlog  
(acctno,points,salesperson,runno)  
  
SELECT ACCOUNT_NUMBER,Application_Score,Salesperson_number,@runno  
 FROM TempTransact  
  
  
drop table TempTransact -- jec 22/05/08   
  
update acct  
 set tallyManAcct = 1  
  from acct   
   inner join instalplan  
    on acct.acctno = instalplan.acctno  
  INNER JOIN vw_DelAmount  
   ON acct.AcctNo = vw_DelAmount.acctNo  
  where acct.acctno like '___0%' and  
   --outstbal >0 and  
   instalplan.datelast >'1-Jan-1910' and  
   --acct.arrears > (instalplan.instalamount * @pcentarrears /100) and acct.arrears >0.1  
   vw_DelAmount.DelAmount >= (acct.agrmttotal * @delthres /100) and  
   (acct.tallymanAcct is null or acct.tallyManAcct != 1)  
  
  
  
DECLARE @lastrundate DATETIME  
SELECT @lastrundate=MAX(datestart) FROM interfacecontrol WHERE interface='TALLYWEEK'  
PRINT 'Tallyamend data ... '  
  
-- ************************* TallyAmend ***********************************  
  
select    
  substring(customer.custid,1,12) as CUSTOMERIC_NUMBER,   
  acct.acctno as ACCOUNT_NUMBER,  
  convert(decimal(12,2),acct.arrears) as Arrears_Value,  
  ISNULL(CONVERT(CHAR(10), Agreement.Datedel, 23),'1900-01-01') as Delivery_Date, --UAT1036 jec 
  CONVERT(CHAR(10), Agreement.dateNextdue, 23) as Next_due_date,  
  convert(decimal(8,2),Acct.outstbal) as Balance_Outstanding,  
  convert(decimal(8,2),Agreement.Agrmttotal) as Agreement_Total,  
  convert(decimal(8,2),#TMDelivery.Delivery_Amount) as Delivery_Amount,  
  Acct.Accttype as Agreement_Type,  
  convert(decimal(8,2),instalplan.Instalamount) as Instalment_Amount,  
  convert(decimal(8,2),Agreement.Deposit) as Deposit_Amount,  
  convert(decimal(8,2),Agreement.Servicechg) as Deferred_Terms,  
  convert(decimal(8,2),acct.Arrears + isnull(#TMBalancePaid.Total,0)) as Balance_Due,  
  Status_Code = case when acct.currstatus in ('1','2','3','4','5','6','7','8','9','0') then acct.currstatus else null end,  
  Settled_Ind = case when acct.currstatus = 'S' then 'S' else null end,  
  convert(decimal(8,2),#TMGoodsReturned.Total )as Goods_Returned,  
  CONVERT(CHAR(10), #TMLastMovement.Date, 23) as Date_Last_Movement,  
  CONVERT(CHAR(10), #TMMinStatusChanged.Date, 23) as Date_SC5,  
  CONVERT(CHAR(10), #TMMaxStatusChanged.Date, 23) as Date_Last_Settled,  
  case when highststatus in ('1','2','3','4','5','6','7','8','9','0') then highststatus else null end as Highest_Status,  
  CONVERT(decimal(8,2), rebates.Rebate) as Rebate_Due,  
  tm_Account.Settled_week,  
  CONVERT(decimal(12,2), #tm_admin.Admin_Charge) as Admin_Charge,  
  CONVERT(decimal(12,2), tm_Account.Trace_Fees) as Trace_Fees,  
  CONVERT(decimal(12,2), tm_Account.Bailiff_Fees) as Bailiff_Fees,  
  CONVERT(decimal(5,2), tm_Account.MPR) as MPR,  
  case when termstypetable.apr = ' ' then 0 else termstypetable.apr end as apr, -- jec 16/05/08 return 0 for termstype if ' '   
  -- termstypetable.apr as apr, --CONVERT(decimal(5,2), termstypetable.apr) as apr,  
  CONVERT(CHAR(10), Acct.Datelastpaid,23) as Date_Last_Payment,  
  Acct.Branchno as Branch_No,   
  Left(right(Acct.acctno,9),8) as Account4to11,  
  CONVERT(decimal(12,2),#TMInterest.Total) as Arrears_Interest,  
  null as Aricy_Courts_Option_Flag  
 into ##TempTallyAmend  
 from acct  
  inner join custacct  
   on acct.acctno = custacct.acctno and hldorjnt = 'H'  
  INNER JOIN customer   
   on custacct.custid = customer.custid  
  inner join agreement  
   on acct.acctno = agreement.acctno  
  left outer join #TMDelivery  
   on acct.acctno = #TMDelivery.acctno  
  left outer join #TMBalancePaid  
   on acct.acctno = #TMBalancePaid.acctno  
  left outer join #TMGoodsReturned  
   on acct.acctno = #TMGoodsReturned.acctno  
  left outer join #TMLastMovement  
   on acct.acctno = #TMLastMovement.acctno  
  left outer join #TMMinStatusChanged  
   on acct.acctno = #TMMinStatusChanged.acctno  
  left outer join  #TMMaxStatusChanged  
   on acct.acctno = #TMMaxStatusChanged.acctno  
  left outer join #tm_admin
	on acct.acctno = #tm_admin.acctno  
  left outer join tm_Account  
   on acct.acctno = tm_Account.Account_Number  
  left outer join termstypetable  
   on acct.termstype = termstypetable.termstype  
  left outer join #TMInterest  
   on acct.acctno = #TMInterest.acctno  
  left outer join rebates  
   on acct.acctno=rebates.acctno  
  inner join instalplan  
   on acct.acctno = instalplan.acctno  
  INNER JOIN vw_DelAmount  
   ON acct.AcctNo = vw_DelAmount.acctNo  
 where acct.acctno like '___0%' and  
  (currstatus<>'S' or   
   exists (select * from bdw b where b.acctno= acct.acctno)  
      --OR #TMLastMovement.Date>@lastrundate SL...select everything  
      ) and  
  (instalplan.datelast >'1-Jan-1910' or acct.tallyManAcct = '1')	--UAT1036 jec
   --acct.arrears > (instalplan.instalamount * @pcentarrears /100) and   
  --acct.arrears >0.1 and   
  --vw_DelAmount.DelAmount >= (acct.agrmttotal * @pcentarrears /100) and 
  
update ##TempTallyAmend   
set Highest_Status=  
isnull((select max(statuscode) from status where statuscode not in ('U','S')  
and ##TempTallyAmend.ACCOUNT_NUMBER = status.acctno),1)  
where Highest_Status='U'  
  
  
set @path = 'BCP "select CONVERT(CHAR(10), GetDate(), 23),Count(*), ''0.00'' from ##TempTallyAmend "  queryout ' +  
 'D:\tallyman\operations\interface\debt\Tallyamendhdr.csv ' + '-c -t, -q -Usa -P'  
exec master.dbo.xp_cmdshell @path  
set @path = 'BCP "select * from ##TempTallyAmend "  queryout ' +  
 'D:\tallyman\operations\interface\debt\Tallyamend.csv ' + '-c -t, -q -Usa -P'  
exec master.dbo.xp_cmdshell @path  
  
drop table ##tempTallyAmend  
  
  
  
select distinct catdescript, cc.category, code, codedescript, statusflag   
into ##tallymancodes  
from code c, codecat cc  
where cc.category=c.category  
set @path = 'BCP "select * from ##tallymancodes "  queryout ' +  
 'D:\tallyman\operations\interface\debt\Tallyamendlookup.csv ' + '-c -t, -q -Usa -P'  
exec master.dbo.xp_cmdshell @path  
  
drop table ##tallymancodes  
  
truncate table bdw  
  
insert into interfacecontrol  
(interface,runno,datestart,datefinish,result)  
values  
('TALLYWEEK', @runno,@datestart,getdate(),'P')  
  
  go
 
-- End End End End End End End End End End End End End End End End End End End End End End End 
