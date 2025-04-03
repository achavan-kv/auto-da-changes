--------------------------------------------------------------------------------
--
-- Project      : Securitisation Reports
-- File Name    : RP_Service_Report.sql
-- File Type    : MSSQL Server SQL Script for reporting services
-- Author       : A Ayscough - 
-- Date         : 12 Mar 2008
-- Comments     : This takes the servicer data and puts it into a table which is then loaded up to be displayed by reporting services
-- Change Control
-- --------------
-- Date      By     Description
-- ----      --     -----------
-- 12/03/08  AA Created for Malaysia 
IF EXISTS(SELECT * FROM sysobjects WHERE NAME = 'RP_Service_Report')
DROP PROCEDURE RP_Service_Report 
GO
CREATE PROCEDURE RP_Service_Report 
AS
DECLARE @ReportDate         DATETIME,
        @CalcDate           DATETIME,
        @CalcDate_Last      DATETIME,
        @CalcDate_Next      DATETIME,
        @SettleDate         DATETIME,
        @SettleDate_Last    DATETIME,
        @SettleDate_Next    DATETIME,
        @CollectDate        DATETIME,
        @CollectDate_Last   DATETIME,
        @CollectDate_Next   DATETIME,
        @ProgramLimit       MONEY,
        @ProgramAmount      MONEY,
        @DiscountRate1      MONEY,
        @DiscountRate2      MONEY


-------------------------------------------------------------------------------------------------
-- Load report results into variables
-------------------------------------------------------------------------------------------------

SELECT  @CalcDate           = ISNULL(CalcDate,''),
        @CalcDate_Last      = ISNULL(CalcDate_Last,''),
        @CalcDate_Next      = ISNULL(CalcDate_Next,''),
        @SettleDate         = ISNULL(SettleDate,''),
        @SettleDate_Last    = ISNULL(SettleDate_Last,''),
        @SettleDate_Next    = ISNULL(SettleDate_Next,''),
        @CollectDate        = ISNULL(CollectDate,''),
        @CollectDate_Last   = ISNULL(CollectDate_Last,''),
        @CollectDate_Next   = ISNULL(CollectDate_Next,''),
        @ProgramLimit       = ISNULL(ProgramLimit,0),
        @ProgramAmount      = ISNULL(ProgramAmount,0),
        @DiscountRate1      = ISNULL(DiscountRate1,0),
        @DiscountRate2      = ISNULL(DiscountRate2,0)
FROM    Service_Data


-- Different settle date for first report
IF DATEADD(Month,-1,@CalcDate) < CONVERT(DATETIME,'01 Jun 2004',106) 
	SET  @SettleDate_Last = DATEADD(Day,-DAY(@SettleDate_Last)+13,@SettleDate_Last)


IF NOT EXISTS(SELECT * FROM information_schema.tables WHERE table_name = 'temp_svcr')
CREATE TABLE temp_svcr (description VARCHAR(512),lastmonth VARCHAR(64),thismonth VARCHAR(64))

TRUNCATE TABLE temp_svcr

INSERT INTO temp_svcr (description,lastmonth,thismonth) VALUES(  '','','')
INSERT INTO temp_svcr (description,lastmonth,thismonth) VALUES(  'Servicer Certificate (Receivable Portfolio Details)','','')
INSERT INTO temp_svcr (description,lastmonth,thismonth) VALUES(  '','','')
INSERT INTO temp_svcr (description,lastmonth,thismonth) VALUES(  'Calculation Date of this Report' , ISNULL(CONVERT(VARCHAR,@CalcDate,106),''), '')
--INSERT INTO temp_svcr (description,lastmonth,thismonth) VALUES(  'Calculation Date of the Last Report' , ISNULL(CONVERT(VARCHAR,@CalcDate_Last,106),''),'')
INSERT INTO temp_svcr (description,lastmonth,thismonth)  VALUES( ' Collection Period for this Report' , 
ISNULL(CONVERT(VARCHAR,@CollectDate_Last,106),'') , ISNULL(CONVERT(VARCHAR,@CollectDate,106),''))

DECLARE  @openingbalance MONEY,@newreceivables MONEY,
@additionalcharges MONEY,@collections MONEY,@normalcollections MONEY, @insurance MONEY,
@prepaymentcols MONEY, @subtotalcols MONEY

DECLARE @openingbalanceLM MONEY,@newreceivablesLM MONEY,
@additionalchargesLM MONEY,@collectionsLM MONEY,@normalcollectionsLM MONEY, @insuranceLM MONEY,
@prepaymentcolsLM MONEY, @subtotalcolsLM MONEY 
SELECT @openingbalance= ISNULL(ResultValue,0) FROM Service_Report WHERE CalcDate =  @CalcDate  AND ResultId = 'OpeningBalance'
SELECT @openingbalanceLM= ISNULL(ResultValue,0) FROM Service_Report WHERE CalcDate = DATEADD(Month,-1,@CalcDate) AND ResultId = 'OpeningBalance'

SELECT @Newreceivables= ISNULL(ResultValue,0) FROM Service_Report WHERE CalcDate =  @CalcDate  AND ResultId = 'Newreceivables'
SELECT @newreceivablesLM= ISNULL(ResultValue,0) FROM Service_Report WHERE CalcDate = DATEADD(Month,-1,@CalcDate) AND ResultId = 'Newreceivables'

SELECT @additionalcharges= ISNULL(ResultValue,0) FROM Service_Report WHERE CalcDate =  @CalcDate  AND ResultId = 'additionalcharges'
SELECT @additionalchargesLM= ISNULL(ResultValue,0) FROM Service_Report WHERE CalcDate = DATEADD(Month,-1,@CalcDate) AND ResultId = 'additionalcharges'

SELECT @normalcollections= ISNULL(-ResultValue,0) FROM Service_Report WHERE CalcDate =  @CalcDate  AND ResultId = 'normalcollections'
SELECT @normalcollectionsLM= ISNULL(-ResultValue,0) FROM Service_Report WHERE CalcDate = DATEADD(Month,-1,@CalcDate) AND ResultId = 'normalcollections'

SELECT @insurance= ISNULL(-ResultValue,0) FROM Service_Report WHERE CalcDate =  @CalcDate  AND ResultId = 'insurance'
SELECT @insuranceLM= ISNULL(-ResultValue,0) FROM Service_Report WHERE CalcDate = DATEADD(Month,-1,@CalcDate) AND ResultId = 'insurance'

SELECT @prepaymentcols= ISNULL(-ResultValue,0) FROM Service_Report WHERE CalcDate =  @CalcDate  AND ResultId = 'prepaymentcols'
SELECT @prepaymentcolsLM= ISNULL(-ResultValue,0) FROM Service_Report WHERE CalcDate = DATEADD(Month,-1,@CalcDate) AND ResultId = 'prepaymentcols'

SELECT @additionalcharges= ISNULL(ResultValue,0) FROM Service_Report WHERE CalcDate =  @CalcDate  AND ResultId = 'additionalcharges'
SELECT @additionalchargesLM= ISNULL(ResultValue,0) FROM Service_Report WHERE CalcDate = DATEADD(Month,-1,@CalcDate) AND ResultId = 'additionalcharges'

SELECT @subtotalcols= ISNULL(-ResultValue,0) FROM Service_Report WHERE CalcDate =  @CalcDate  AND ResultId = 'subtotalcols'
SELECT @subtotalcolsLM= ISNULL(-ResultValue,0) FROM Service_Report WHERE CalcDate = DATEADD(Month,-1,@CalcDate) AND ResultId = 'subtotalcols'

INSERT INTO temp_svcr (description,lastmonth,thismonth) VALUES(  ' Receivable Activity Table (These balances would be based upon bad debts grossed up. i.e. if no receivables were written off)','','')
INSERT INTO temp_svcr (description,lastmonth,thismonth) VALUES(  '','','')
INSERT INTO temp_svcr (description,lastmonth,thismonth) VALUES(  '',' Calculations from last  Report','Calculations for this Report')
INSERT INTO temp_svcr (description,lastmonth,thismonth) VALUES(  '','','')
INSERT INTO temp_svcr (description,lastmonth,thismonth) VALUES(  ' Opening Balance of Securitised Receivables ' , ISNULL(CONVERT(VARCHAR,@openingbalanceLM),'')  , ISNULL(CONVERT(VARCHAR,@openingbalance),''))
INSERT INTO temp_svcr (description,lastmonth,thismonth) VALUES(  '','','')
INSERT INTO temp_svcr (description,lastmonth,thismonth) VALUES(  'New Receivables generated & securitised during the Collection Period ' , ISNULL(CONVERT(VARCHAR,@newreceivablesLM),'') ,  ISNULL(CONVERT(VARCHAR,@newreceivables),''))

INSERT INTO temp_svcr (description,lastmonth,thismonth) VALUES(  'Additional Charges (upon Collections) during the Collection Period ' 
, ISNULL(CONVERT(VARCHAR,@additionalchargesLM),'') ,  ISNULL(CONVERT(VARCHAR,@additionalcharges),''))

INSERT INTO temp_svcr (description,lastmonth,thismonth) VALUES(  'Collections During the Period (-)'  , '','')
INSERT INTO temp_svcr (description,lastmonth,thismonth) VALUES(  'Comprising of:','','')
INSERT INTO temp_svcr (description,lastmonth,thismonth) VALUES(  ' Normal Collections ' , ISNULL(CONVERT(VARCHAR,@normalcollectionsLM),'') ,  ISNULL(CONVERT(VARCHAR,@normalcollections),''))
INSERT INTO temp_svcr (description,lastmonth,thismonth) VALUES(  ' Insurance claims received ' , ISNULL(CONVERT(VARCHAR,@insuranceLM),'') ,  ISNULL(CONVERT(VARCHAR,@insurance),''))
INSERT INTO temp_svcr (description,lastmonth,thismonth) VALUES(  ' Prepayment Collections'        , ISNULL(CONVERT(VARCHAR,@prepaymentcolsLM),'') ,  ISNULL(CONVERT(VARCHAR,@prepaymentcols),''))
INSERT INTO temp_svcr (description,lastmonth,thismonth) VALUES(  ' Additional Charges Collected'  , ISNULL(CONVERT(VARCHAR,@additionalchargesLM),'') ,  ISNULL(CONVERT(VARCHAR,@additionalcharges),''))
--INSERT INTO temp_svcr (description,lastmonth,thismonth) VALUES(  ' Sub-total'                     , ISNULL(CONVERT(VARCHAR,@PR7A+@PR7B+@PR7C),'') , ISNULL(CONVERT(VARCHAR,@CR7A+@CR7B+@CR7C),''))
INSERT INTO temp_svcr (description,lastmonth,thismonth) VALUES(  ' Sub-total'                     , ISNULL(CONVERT(VARCHAR,@subtotalcolsLM),'') , ISNULL(CONVERT(VARCHAR,@subtotalcols),''))

DECLARE @returns MONEY,@allowances MONEY,@addtosettlements MONEY, @otherdilutions MONEY,
@writeoffs MONEY,@rebates MONEY,@dilutionssubtotal MONEY,@weightedaverage FLOAT
DECLARE @returnsLM MONEY,@allowancesLM MONEY,@addtosettlementsLM MONEY, @otherdilutionsLM MONEY,
@writeoffsLM MONEY,@rebatesLM MONEY,@dilutionssubtotalLM MONEY,@weightedaverageLM FLOAT

SELECT @weightedaverage= ISNULL(ResultValue,0) FROM Service_Report WHERE CalcDate =  @CalcDate  AND ResultId = 'weightedaverage'
SELECT @weightedaverageLM = ISNULL(ResultValue,0) FROM Service_Report WHERE CalcDate = DATEADD(Month,-1,@CalcDate) AND ResultId = 'weightedaverage'

SELECT @returns= ISNULL(-ResultValue,0) FROM Service_Report WHERE CalcDate =  @CalcDate  AND ResultId = 'returns'
SELECT @returnsLM= ISNULL(-ResultValue,0) FROM Service_Report WHERE CalcDate = DATEADD(Month,-1,@CalcDate) AND ResultId = 'returns'


SELECT @allowances= ISNULL(-ResultValue,0) FROM Service_Report WHERE CalcDate =  @CalcDate  AND ResultId = 'allowances'
SELECT @allowancesLM= ISNULL(-ResultValue,0) FROM Service_Report WHERE CalcDate = DATEADD(Month,-1,@CalcDate) AND ResultId = 'allowances'

SELECT @addtosettlements= ISNULL(-ResultValue,0) FROM Service_Report WHERE CalcDate =  @CalcDate  AND ResultId = 'addtosettlements'
SELECT @addtosettlementsLM= ISNULL(-ResultValue,0) FROM Service_Report WHERE CalcDate = DATEADD(Month,-1,@CalcDate) AND ResultId = 'addtosettlements'

SELECT @dilutionssubtotal= ISNULL(-ResultValue,0) FROM Service_Report WHERE CalcDate =  @CalcDate  AND ResultId = 'dilutionssubtotal'
SELECT @dilutionssubtotalLM= ISNULL(-ResultValue,0) FROM Service_Report WHERE CalcDate = DATEADD(Month,-1,@CalcDate) AND ResultId = 'dilutionssubtotal'

SELECT @rebates= ISNULL(-ResultValue,0) FROM Service_Report WHERE CalcDate =  @CalcDate  AND ResultId = 'rebates'
SELECT @rebatesLM= ISNULL(-ResultValue,0) FROM Service_Report WHERE CalcDate = DATEADD(Month,-1,@CalcDate) AND ResultId = 'rebates'

INSERT INTO temp_svcr (description,lastmonth,thismonth) VALUES(  '','','')
INSERT INTO temp_svcr (description,lastmonth,thismonth) VALUES(  'Dilutions during the Period (-)'  , '','')
INSERT INTO temp_svcr (description,lastmonth,thismonth) VALUES(  '  Returns '    
 , ISNULL(CONVERT(VARCHAR,@returnsLM),'') ,  ISNULL(CONVERT(VARCHAR,@returns),''))
INSERT INTO temp_svcr (description,lastmonth,thismonth) VALUES(  '  Allowances '    
 , ISNULL(CONVERT(VARCHAR,@allowancesLM),'') ,  ISNULL(CONVERT(VARCHAR,@allowances),''))

INSERT INTO temp_svcr (description,lastmonth,thismonth) VALUES(  '  Add-to Settelments'    
 , ISNULL(CONVERT(VARCHAR,@addtosettlementsLM),'') ,  ISNULL(CONVERT(VARCHAR,@addtosettlements),''))
--TODO Other
INSERT INTO temp_svcr (description,lastmonth,thismonth) VALUES(  '  Sub-total'    
 , ISNULL(CONVERT(VARCHAR,@dilutionssubtotalLM),'') ,  ISNULL(CONVERT(VARCHAR,@dilutionssubtotal),''))

DECLARE 
@closing_balance MONEY,@totalwriteoffs MONEY,@losses MONEY,@0monthsarrs MONEY,
@1monthsarrs MONEY, @2monthsarrs MONEY,@3monthsarrs MONEY,@4monthsarrs MONEY,
@5monthsarrs MONEY,@6monthsarrs MONEY,@7monthsarrs MONEY,@8monthsarrs MONEY,
@9monthsarrs MONEY,@10monthsarrs MONEY,@11monthsarrs MONEY,@12monthsarrs MONEY,
@gr12monthsarrs MONEY

DECLARE 
@closing_balanceLM MONEY,@totalwriteoffsLM MONEY,@lossesLM MONEY,@0monthsarrsLM MONEY,
@1monthsarrsLM MONEY, @2monthsarrsLM MONEY,@3monthsarrsLM MONEY,@4monthsarrsLM MONEY,
@5monthsarrsLM MONEY,@6monthsarrsLM MONEY,@7monthsarrsLM MONEY,@8monthsarrsLM MONEY,
@9monthsarrsLM MONEY,@10monthsarrsLM MONEY,@11monthsarrsLM MONEY,@12monthsarrsLM MONEY,
@gr12monthsarrsLM MONEY


--Monthsarrears0
SELECT @closing_balance =ISNULL(ResultValue,0) FROM Service_Report WHERE CalcDate =  @CalcDate  AND ResultId = 'closing_balance'
SELECT @closing_balanceLM= ISNULL(ResultValue,0) FROM Service_Report WHERE CalcDate = DATEADD(Month,-1,@CalcDate) AND ResultId = 'closing_balance'
--totalwriteoffs
SELECT @totalwriteoffs =ISNULL(ResultValue,0) FROM Service_Report WHERE CalcDate =  @CalcDate  AND ResultId = 'totalwriteoffs'
SELECT @totalwriteoffsLM= ISNULL(ResultValue,0) FROM Service_Report WHERE CalcDate = DATEADD(Month,-1,@CalcDate) AND ResultId = 'totalwriteoffs'

SELECT @0monthsarrs= ISNULL(ResultValue,0) FROM Service_Report WHERE CalcDate =  @CalcDate  AND ResultId = 'Monthsarrears0'
SELECT @0monthsarrsLM= ISNULL(ResultValue,0) FROM Service_Report WHERE CalcDate = DATEADD(Month,-1,@CalcDate) AND ResultId = 'Monthsarrears0'

SELECT @0monthsarrs= ISNULL(ResultValue,0) FROM Service_Report 
WHERE CalcDate =  @CalcDate  AND ResultId = 'Monthsarrears1'
SELECT @0monthsarrsLM= ISNULL(ResultValue,0) FROM Service_Report 
WHERE CalcDate = DATEADD(Month,-1,@CalcDate) AND ResultId = 'Monthsarrears1'


SELECT @1monthsarrs =ISNULL(ResultValue,0) FROM Service_Report 
WHERE CalcDate =  @CalcDate  AND ResultId = 'Monthsarrears1'
SELECT @1monthsarrsLM= ISNULL(ResultValue,0) FROM Service_Report 
WHERE CalcDate = DATEADD(Month,-1,@CalcDate) AND ResultId = 'Monthsarrears1'

SELECT @2monthsarrs =ISNULL(ResultValue,0) FROM Service_Report 
WHERE CalcDate =  @CalcDate  AND ResultId = 'Monthsarrears2'
SELECT @2monthsarrsLM= ISNULL(ResultValue,0) FROM Service_Report 
WHERE CalcDate = DATEADD(Month,-2,@CalcDate) AND ResultId = 'Monthsarrears2'

SELECT @3monthsarrs =ISNULL(ResultValue,0) FROM Service_Report 
WHERE CalcDate =  @CalcDate  AND ResultId = 'Monthsarrears3'
SELECT @3monthsarrsLM= ISNULL(ResultValue,0) FROM Service_Report 
WHERE CalcDate = DATEADD(Month,-3,@CalcDate) AND ResultId = 'Monthsarrears3'

SELECT @4monthsarrs =ISNULL(ResultValue,0) FROM Service_Report 
WHERE CalcDate =  @CalcDate  AND ResultId = 'Monthsarrears4'
SELECT @4monthsarrsLM= ISNULL(ResultValue,0) FROM Service_Report 
WHERE CalcDate = DATEADD(Month,-4,@CalcDate) AND ResultId = 'Monthsarrears4'

SELECT @5monthsarrs =ISNULL(ResultValue,0) FROM Service_Report WHERE CalcDate =  @CalcDate  AND ResultId = 'Monthsarrears5'
SELECT @5monthsarrsLM= ISNULL(ResultValue,0) FROM Service_Report WHERE CalcDate = DATEADD(Month,-5,@CalcDate) AND ResultId = 'Monthsarrears5'

SELECT @6monthsarrs= ISNULL(ResultValue,0) FROM Service_Report WHERE CalcDate =  @CalcDate  AND ResultId = 'Monthsarrears6'
SELECT @6monthsarrsLM= ISNULL(ResultValue,0) FROM Service_Report WHERE CalcDate = DATEADD(Month,-6,@CalcDate) AND ResultId = 'Monthsarrears6'

SELECT @7monthsarrs =ISNULL(ResultValue,0) FROM Service_Report WHERE CalcDate =  @CalcDate  AND ResultId = 'Monthsarrears7'
SELECT @7monthsarrsLM= ISNULL(ResultValue,0) FROM Service_Report WHERE CalcDate = DATEADD(Month,-7,@CalcDate) AND ResultId = 'Monthsarrears7'

SELECT @8monthsarrs =ISNULL(ResultValue,0) FROM Service_Report WHERE CalcDate =  @CalcDate  AND ResultId = 'Monthsarrears8'
SELECT @8monthsarrsLM= ISNULL(ResultValue,0) FROM Service_Report WHERE CalcDate = DATEADD(Month,-8,@CalcDate) AND ResultId = 'Monthsarrears8'

SELECT @9monthsarrs= ISNULL(ResultValue,0) FROM Service_Report WHERE CalcDate =  @CalcDate  AND ResultId = 'Monthsarrears9'
SELECT @9monthsarrsLM= ISNULL(ResultValue,0) FROM Service_Report WHERE CalcDate = DATEADD(Month,-9,@CalcDate) AND ResultId = 'Monthsarrears9'

SELECT @10monthsarrs= ISNULL(ResultValue,0) FROM Service_Report WHERE CalcDate =  @CalcDate  AND ResultId = 'Monthsarrears10'
SELECT @10monthsarrsLM= ISNULL(ResultValue,0) FROM Service_Report WHERE CalcDate = DATEADD(Month,-10,@CalcDate) AND ResultId = 'Monthsarrears10'

SELECT @11monthsarrs= ISNULL(ResultValue,0) FROM Service_Report WHERE CalcDate =  @CalcDate  AND ResultId = 'Monthsarrears11'
SELECT @11monthsarrsLM= ISNULL(ResultValue,0) FROM Service_Report WHERE CalcDate = DATEADD(Month,-11,@CalcDate) AND ResultId = 'Monthsarrears11'

SELECT @12monthsarrs =ISNULL(ResultValue,0) FROM Service_Report WHERE CalcDate =  @CalcDate  AND ResultId = 'Monthsarrears12'
SELECT @12monthsarrsLM= ISNULL(ResultValue,0) FROM Service_Report WHERE CalcDate = DATEADD(Month,-12,@CalcDate) AND ResultId = 'Monthsarrears12'

SELECT @gr12monthsarrs= ISNULL(ResultValue,0) FROM Service_Report WHERE CalcDate =  @CalcDate  AND ResultId = 'MonthsarrearsGR12'
SELECT @gr12monthsarrs= ISNULL(ResultValue,0) FROM Service_Report WHERE CalcDate = DATEADD(Month,-12,@CalcDate) AND ResultId = 'MonthsarrearsGR12'

INSERT INTO temp_svcr (description,lastmonth,thismonth) VALUES(  '  Total Write Offs'    
 , ISNULL(CONVERT(VARCHAR,@writeoffslm),'') ,  ISNULL(CONVERT(VARCHAR,@writeoffs),''))

--INSERT INTO temp_svcr (description,lastmonth,thismonth) VALUES(  ' (Deemed Collections) (Other than Rebates on Prepayments)','','')
--INSERT INTO temp_svcr (description,lastmonth,thismonth) VALUES(  ' 9Balance Dilutions (i.e. from Unseasoned & Additional Receivables)'                        , ISNULL(CONVERT(VARCHAR,@PR9),'') , '' , ISNULL(CONVERT(VARCHAR,@CR9),'')
INSERT INTO temp_svcr (description,lastmonth,thismonth) VALUES(  ' Rebates on Prepayments (balance) ' , 
ISNULL(CONVERT(VARCHAR,@rebatesLM),'') ,  ISNULL(CONVERT(VARCHAR,@rebates),''))
INSERT INTO temp_svcr (description,lastmonth,thismonth) VALUES(  ' Closing Balance of Securitised Receivables ' , ISNULL(CONVERT(VARCHAR,@closing_balanceLM),'') ,  ISNULL(CONVERT(VARCHAR,@closing_balance),''))
--INSERT INTO temp_svcr (description,lastmonth,thismonth) VALUES(  ' 17Total' , ISNULL(CONVERT(VARCHAR,@PR13,@PR14),'') , '' , ISNULL(CONVERT(VARCHAR,@CR13,@CR14),'')
INSERT INTO temp_svcr (description,lastmonth,thismonth) VALUES(  '','','')
INSERT INTO temp_svcr (description,lastmonth,thismonth) VALUES(  ' Weighted Average Portfolio Life (in Months)' , ISNULL(CONVERT(VARCHAR,@weightedaverageLM),'') , ISNULL(CONVERT(VARCHAR,@weightedaverage),''))
--INSERT INTO temp_svcr (description,lastmonth,thismonth) VALUES(  '','','')
--INSERT INTO temp_svcr (description,lastmonth,thismonth) VALUES(  ' Note 1 Details of Collections (other than Deemed Collections) Amount ','','')
INSERT INTO temp_svcr (description,lastmonth,thismonth) VALUES(  '','','')
--INSERT INTO temp_svcr (description,lastmonth,thismonth) VALUES(  'Collections from opening balance of Seasoned Receivables (other than insurance)' , ISNULL(CONVERT(VARCHAR,@PR19A),'') ,  ISNULL(CONVERT(VARCHAR,@CR19A),''))
--INSERT INTO temp_svcr (description,lastmonth,thismonth) VALUES(   'Balance Collections (other than insurance)'                                      , ISNULL(CONVERT(VARCHAR,@PR19B),'') ,  ISNULL(CONVERT(VARCHAR,@CR19B),''))
--INSERT INTO temp_svcr (description,lastmonth,thismonth) VALUES(  'Insurance Claim Receipts from opening balance of Seasoned Receivables'           , ISNULL(CONVERT(VARCHAR,@PR19C),'') ,  ISNULL(CONVERT(VARCHAR,@CR19C),''))
--INSERT INTO temp_svcr (description,lastmonth,thismonth) VALUES(  'Balance Insurance Claim Receipts'                                                , ISNULL(CONVERT(VARCHAR,@PR19D),'') ,  ISNULL(CONVERT(VARCHAR,@CR19D),''))

--INSERT INTO temp_svcr (description,lastmonth,thismonth) VALUES(  ' Note Details of Write-off balances (net of recoveries) Amount ','','')

--I--NSERT INTO temp_svcr (description,lastmonth,thismonth) VALUES(  ' Amount of opening Balance of Seasoned Receivables Written Off'
   --                      , ISNULL(CONVERT(VARCHAR,@PR20A),'') , ISNULL(CONVERT(VARCHAR,@CR20A),''))
/*INSERT INTO temp_svcr (description,lastmonth,thismonth) VALUES(  ' Amount of balance Purchased Receivables Written Off' 
                                  , ISNULL(CONVERT(VARCHAR,@PR20B),'') ,  ISNULL(CONVERT(VARCHAR,@CR20B),''))
INSERT INTO temp_svcr (description,lastmonth,thismonth) VALUES(  ' Amount of opening Balance of Seasoned Receivables Written Off and purchased by Seller' ,
 ISNULL(CONVERT(VARCHAR,@PR20C),'') ,  ISNULL(CONVERT(VARCHAR,@CR20C),''))
INSERT INTO temp_svcr (description,lastmonth,thismonth) VALUES(  ' Amount of balance Purchased Receivables Written Off and purchased by Seller'  
         , ISNULL(CONVERT(VARCHAR,@PR20D),'') ,  ISNULL(CONVERT(VARCHAR,@CR20D),''))*/
INSERT INTO temp_svcr (description,lastmonth,thismonth) VALUES(  ' ','','')
INSERT INTO temp_svcr (description,lastmonth,thismonth) VALUES(  ' Receivable Ageing Table (Net of bad debts & no Additional Charges)  ','','')
--INSERT INTO temp_svcr (description,lastmonth,thismonth) VALUES(  '  requirement  1 Month  2 Month ','','')
--INSERT INTO temp_svcr (description,lastmonth,thismonth) VALUES(  ' As of the Calculation Date of this Report','','')
INSERT INTO temp_svcr (description,lastmonth,thismonth) VALUES(  ' ','','')

-- Months in arrears
INSERT INTO temp_svcr (description,lastmonth,thismonth) VALUES(  ' Month in Arrears','Calculations from last Report','Calculations for this Report')


INSERT INTO temp_svcr (description,lastmonth,thismonth) VALUES(  ' 0'    ,ISNULL(CONVERT(VARCHAR,@0monthsarrsLM),''),
 ISNULL(CONVERT(VARCHAR,@0monthsarrs),''))

--SELECT @PR21D1M0A AS one ,@PR21D1M0B AS two,@PR21D1M0C AS three
INSERT INTO temp_svcr (description,lastmonth,thismonth) VALUES(  ' 1'    , ISNULL(CONVERT(VARCHAR,@1monthsarrsLM),''),
 ISNULL(CONVERT(VARCHAR,@1monthsarrs),''))
INSERT INTO temp_svcr (description,lastmonth,thismonth) VALUES(  ' 2'    ,ISNULL(CONVERT(VARCHAR,@2monthsarrsLM),''),
 ISNULL(CONVERT(VARCHAR,@2monthsarrs),''))
INSERT INTO temp_svcr (description,lastmonth,thismonth) VALUES(  ' 3'    , ISNULL(CONVERT(VARCHAR,@3monthsarrsLM),''),
ISNULL(CONVERT(VARCHAR,@3monthsarrs),''))
INSERT INTO temp_svcr (description,lastmonth,thismonth) VALUES(  ' 4'    ,ISNULL(CONVERT(VARCHAR,@4monthsarrsLM),''), ISNULL(CONVERT(VARCHAR,@4monthsarrs),''))
INSERT INTO temp_svcr (description,lastmonth,thismonth) VALUES(  ' 5'    ,ISNULL(CONVERT(VARCHAR,@5monthsarrsLM),''), ISNULL(CONVERT(VARCHAR,@5monthsarrs),''))
INSERT INTO temp_svcr (description,lastmonth,thismonth) VALUES(  ' 6'    ,ISNULL(CONVERT(VARCHAR,@6monthsarrsLM),''), ISNULL(CONVERT(VARCHAR,@6monthsarrs),''))
INSERT INTO temp_svcr (description,lastmonth,thismonth) VALUES(  ' 7'    , ISNULL(CONVERT(VARCHAR,@7monthsarrsLM),''), ISNULL(CONVERT(VARCHAR,@7monthsarrs),''))
INSERT INTO temp_svcr (description,lastmonth,thismonth) VALUES(  ' 8'    ,ISNULL(CONVERT(VARCHAR,@8monthsarrsLM),''),  ISNULL(CONVERT(VARCHAR,@8monthsarrs),''))
INSERT INTO temp_svcr (description,lastmonth,thismonth) VALUES(  ' 9'    , ISNULL(CONVERT(VARCHAR,@9monthsarrsLM),''), ISNULL(CONVERT(VARCHAR,@9monthsarrs),''))
INSERT INTO temp_svcr (description,lastmonth,thismonth) VALUES(  ' 10'   , ISNULL(CONVERT(VARCHAR,@10monthsarrsLM),''),ISNULL(CONVERT(VARCHAR,@10monthsarrs),''))
INSERT INTO temp_svcr (description,lastmonth,thismonth) VALUES(  ' 11'   ,ISNULL(CONVERT(VARCHAR,@11monthsarrsLM),''), ISNULL(CONVERT(VARCHAR,@11monthsarrs),''))
INSERT INTO temp_svcr (description,lastmonth,thismonth) VALUES(  ' 12'   , ISNULL(CONVERT(VARCHAR,@12monthsarrsLM),''), ISNULL(CONVERT(VARCHAR,@12monthsarrs),''))
INSERT INTO temp_svcr (description,lastmonth,thismonth) VALUES(  ' Balance' ,ISNULL(CONVERT(VARCHAR,@gr12monthsarrsLM),''), ISNULL(CONVERT(VARCHAR,@gr12monthsarrs),''))

INSERT INTO temp_svcr (description,lastmonth,thismonth) VALUES(  ' Total Receivables' , 
/*Last month */             ISNULL(CONVERT(VARCHAR,@1monthsarrslm + @2monthsarrslm + @3monthsarrsLM + @4monthsarrsLM + @5monthsarrsLM + 
@6monthsarrsLM + @7monthsarrsLM + @8monthsarrsLM + @9monthsarrsLM + @10monthsarrsLM + @11monthsarrsLM + @12monthsarrsLM + @gr12monthsarrsLM),''),
/*This month */ ISNULL(CONVERT(VARCHAR,@1monthsarrs+ @2monthsarrs+ @3monthsarrs+@4monthsarrs+ @5monthsarrs+@6monthsarrs
+@7monthsarrs+@8monthsarrs+@9monthsarrs+@10monthsarrs+@11monthsarrs+@12monthsarrs+@gr12monthsarrs),''))


INSERT INTO temp_svcr (description,lastmonth,thismonth) VALUES(  ' Losses' , ISNULL(CONVERT(VARCHAR, 
 @7monthsarrsLM + @8monthsarrsLM + @9monthsarrsLM + @10monthsarrsLM + @11monthsarrsLM + @12monthsarrsLM + @gr12monthsarrsLM),''),
ISNULL(CONVERT(VARCHAR, @7monthsarrs+@8monthsarrs+@9monthsarrs+@10monthsarrs+@11monthsarrs+@12monthsarrs+@gr12monthsarrs),''))
UPDATE temp_svcr SET thismonth = CONVERT(VARCHAR,(CONVERT(MONEY,thismonth)),1)  WHERE  ISNUMERIC(thismonth) = 1
UPDATE temp_svcr SET lastmonth = CONVERT(VARCHAR,(CONVERT(MONEY,lastmonth)),1)  WHERE  ISNUMERIC(lastmonth) = 1
SELECT * FROM temp_svcr
GO
--EXEC Service_ReportMalaysia
--EXEC RP_Service_Report
GO
