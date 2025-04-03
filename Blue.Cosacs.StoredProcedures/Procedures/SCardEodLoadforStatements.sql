IF EXISTS (SELECT * FROM sysobjects WHERE NAME ='SCardEodLoadforStatements' )
DROP PROCEDURE SCardEodLoadforStatements
GO
CREATE PROCEDURE SCardEodLoadforStatements 
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : SCardEodLoadforStatements.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : StoreCard - Create Statement 
-- Date         : ??
--
-- This procedure will Create Storecard Statements .
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 19/01/12  jec #8445 UAT32 Past due Amount - required on next Statement
-- 08/02/12  jec #9514 PaymentDueDate - required on Statement table
-- 10/02/12  jec #9513 CR9417 - new SC Arrears condition - set Minimum payment on StoreCardPaymentDetails??
-- 23/03/12  ip  #9812 - StoreCardPaymentDetails and StoreCardStatement reflected different Payment Due Dates.
-- 28/03/12  jec #9841 UAT142[UAT V6] - Incorrect Min pay when Payment Calculator used
-- 17/04/12  ip  #9908 - DateFrom and DateTo were incorrect. Subsequent statements the DateFrom will be the previous DateTo add a day. DateTo will be add a month to previous DateTo subtract a day.
--				 This also applies to DatePaymentDue
-- 18/04/12	 ip  #9941 - Only update StoreCardStatement.stmtminpayment for statements added in this run.
-- 15/05/12  jec #10120 UAT169[UAT V6] - Min payment > balance Outstanding
-- 18/05/12  jec #10081 UAT142[UAT V6] - Incorrect Min pay when Payment Calculator used
-- 07/09/2012 jec #11200 Interest due and the minimum payment amount is incorrectly exported in the statement csv
-- 13/02/2013 ip #12199 - CR11571
-- 21/02/2013 ip #12442 - Set StoreCardPaymentDetails.DateLastStatementPrinted to rundate - 1 day, when setting the FirstStatementDate
-- ================================================
	-- Add the parameters for the stored procedure here
@rundate SMALLDATETIME AS 
SET NOCOUNT ON 

DECLARE @runNo INT, @bacthCount INT --#12208

SELECT @RunNo = MAX(RunNo) FROM dbo.InterfaceControl WHERE interface = 'STStatements' AND DateFinish = '1900-01-01 00:00:00.000'		--#12208
SET @bacthCount = 0 --#12208

--SET @rundate = DATEADD(hour,6,@rundate) -- adding 6 hours onto the rundate so if run after six counts as a full day for the statement. 
Declare @MinPayment MONEY, @minPercent FLOAT
Declare @IntFreeDays INT		--#9514
select @IntFreeDays= ISNULL((select value from CountryMaintenance where codename='SCardInterestFreeDays'),0)
	
select @MinPayment=(select value from dbo.CountryMaintenance where codename='StoreCardMinPayment')
select @minPercent=(select value from dbo.CountryMaintenance where codename='StoreCardPaymentPercent')

--#12199 - Date for when first statement should be generated. For ones that do not have a statement.
UPDATE StoreCardPaymentDetails
SET FirstStatementDate = CASE StatementFrequency
							WHEN 'M' THEN DATEADD(MONTH, 1, CONVERT(SMALLDATETIME,CONVERT(VARCHAR(10),@rundate,105),105))
							WHEN 'Q' THEN DATEADD(MONTH, 3, CONVERT(SMALLDATETIME,CONVERT(VARCHAR(10),@rundate,105),105))
							WHEN 'B' THEN DATEADD(MONTH, 2, CONVERT(SMALLDATETIME,CONVERT(VARCHAR(10),@rundate,105),105))
							WHEN 'E' THEN DATEADD(MONTH, 6, CONVERT(SMALLDATETIME,CONVERT(VARCHAR(10),@rundate,105),105)) END,
	DateLastStatementPrinted = DATEADD(DAY,-1,@rundate)		--#12442
--SELECT p.acctno 
FROM StoreCardPaymentDetails p	
INNER JOIN acct a ON p.acctno = a.acctno					
WHERE FirstStatementDate IS NULL
AND NOT EXISTS(SELECT * FROM dbo.StoreCardStatement s
				WHERE s.Acctno = p.acctno)
AND (p.Status = 'A' OR a.outstbal > 0)

--#12199 - Select dates activated
SELECT sp.acctno, CONVERT(SMALLDATETIME,CONVERT(VARCHAR(10),MIN(ss.datechanged),105),105)AS ActivatedDate
INTO #ActivatedDates
FROM dbo.StoreCardStatus ss
INNER JOIN dbo.StoreCard sc ON ss.CardNumber = sc.CardNumber
INNER JOIN dbo.StoreCardPaymentDetails sp ON sc.AcctNo = sp.acctno
WHERE ss.StatusCode = 'A'
GROUP BY sp.acctno

--IP - 23/03/12 - #9812 - Select the previous StoreCardStatement.DateTo
;WITH PrevSCardStat AS
(
	--select acctno,MAX(Dateto) as DateTo
	select acctno,Dateadd(day, 1,MAX(Dateto)) as DateTo	 --IP - 17/04/12 - #9908 - add a day to be used to set DateTo
	from dbo.StoreCardStatement s
	group by acctno
)

INSERT INTO StoreCardStatement (
	Custid,
	Acctno,
	DateFrom,
	DateTo,
	DatePrinted,
	DateLastReprinted,
	ReprintedBy,
	StmtMinPayment,			-- #8445
	PrevStmtMinPayment,		-- #8445
	DatePaymentDue,			-- #9514
	RunNo,					-- #12208
	BatchNo					-- #12208
) 
SELECT ca.custid,s.acctno,
--ISNULL(s.DateLastStatementPrinted,a.dateacctopen),
CONVERT(SMALLDATETIME,CONVERT(VARCHAR(10),ISNULL(ss.DateTo,ad.ActivatedDate),105),105),		--#12199							--IP - 22/03/12 - #9812
 DateTo=CASE  --StatementFrequency --#12199
--WHEN 'M' THEN DATEADD(DAY,-1,DATEADD(MONTH,1,isnull(ss.DateTo,CONVERT(SMALLDATETIME,CONVERT(VARCHAR(10),DateAcctOpen,105),105))))	 --IP - 17/04/12 - #9908 - minus a day --IP - 22/03/12 - #9812
--WHEN 'Q' THEN DATEADD(DAY,-1,DATEADD(MONTH,3,isnull(ss.DateTo,CONVERT(SMALLDATETIME,CONVERT(VARCHAR(10),DateAcctOpen,105),105))))
--WHEN 'B' THEN DATEADD(DAY,-1,DATEADD(MONTH,2,isnull(ss.DateTo,CONVERT(SMALLDATETIME,CONVERT(VARCHAR(10),DateAcctOpen,105),105))))
--WHEN 'E' THEN DATEADD(DAY,-1,DATEADD(MONTH,6,isnull(ss.DateTo,CONVERT(SMALLDATETIME,CONVERT(VARCHAR(10),DateAcctOpen,105),105)))) END ,
WHEN ss.DateTo IS NULL THEN DATEADD(DAY,-1,s.FirstStatementDate)	--#12199
WHEN s.StatementFrequency = 'M' THEN DATEADD(DAY,-1,DATEADD(MONTH,1,isnull(ss.DateTo,CONVERT(SMALLDATETIME,CONVERT(VARCHAR(10),@rundate,105),105))))	 --IP - 17/04/12 - #9908 - minus a day --IP - 22/03/12 - #9812
WHEN s.StatementFrequency = 'Q' THEN DATEADD(DAY,-1,DATEADD(MONTH,3,isnull(ss.DateTo,CONVERT(SMALLDATETIME,CONVERT(VARCHAR(10),@rundate,105),105))))
WHEN s.StatementFrequency = 'B' THEN DATEADD(DAY,-1,DATEADD(MONTH,2,isnull(ss.DateTo,CONVERT(SMALLDATETIME,CONVERT(VARCHAR(10),@rundate,105),105))))
WHEN s.StatementFrequency = 'E' THEN DATEADD(DAY,-1,DATEADD(MONTH,6,isnull(ss.DateTo,CONVERT(SMALLDATETIME,CONVERT(VARCHAR(10),@rundate,105),105)))) END ,
@rundate,
NULL,
null,
MinimumPayment,				-- #8445
0,						-- #8445
DatePaymentDue=CASE  --StatementFrequency		--#12199		-- #9514
--WHEN 'M' THEN DATEADD(DAY,-1,DATEADD(DAY,@IntFreeDays,DATEADD(MONTH,1,isnull(ss.DateTo,CONVERT(SMALLDATETIME,CONVERT(VARCHAR(10),DateAcctOpen,105),105)))))	 --IP - 17/04/12 - #9908 - minus a day 			--IP - 22/03/12 - #9812
--WHEN 'Q' THEN DATEADD(DAY,-1,DATEADD(DAY,@IntFreeDays,DATEADD(MONTH,3,isnull(ss.DateTo,CONVERT(SMALLDATETIME,CONVERT(VARCHAR(10),DateAcctOpen,105),105)))))
--WHEN 'B' THEN DATEADD(DAY,-1,DATEADD(DAY,@IntFreeDays,DATEADD(MONTH,2,isnull(ss.DateTo,CONVERT(SMALLDATETIME,CONVERT(VARCHAR(10),DateAcctOpen,105),105)))))
--WHEN 'E' THEN DATEADD(DAY,-1,DATEADD(DAY,@IntFreeDays,DATEADD(MONTH,6,isnull(ss.DateTo,CONVERT(SMALLDATETIME,CONVERT(VARCHAR(10),DateAcctOpen,105),105))))) END 
WHEN ss.DateTo IS NULL THEN DATEADD(DAY,-1,DATEADD(DAY, @IntFreeDays, s.FirstStatementDate))	--#12199
WHEN s.StatementFrequency = 'M' THEN DATEADD(DAY,-1,DATEADD(DAY,@IntFreeDays,DATEADD(MONTH,1,isnull(ss.DateTo,CONVERT(SMALLDATETIME,CONVERT(VARCHAR(10),@rundate,105),105)))))	 --IP - 17/04/12 - #9908 - minus a day 			--IP - 22/03/12 - #9812
WHEN s.StatementFrequency = 'Q' THEN DATEADD(DAY,-1,DATEADD(DAY,@IntFreeDays,DATEADD(MONTH,3,isnull(ss.DateTo,CONVERT(SMALLDATETIME,CONVERT(VARCHAR(10),@rundate,105),105)))))
WHEN s.StatementFrequency = 'B' THEN DATEADD(DAY,-1,DATEADD(DAY,@IntFreeDays,DATEADD(MONTH,2,isnull(ss.DateTo,CONVERT(SMALLDATETIME,CONVERT(VARCHAR(10),@rundate,105),105)))))
WHEN s.StatementFrequency = 'E' THEN DATEADD(DAY,-1,DATEADD(DAY,@IntFreeDays,DATEADD(MONTH,6,isnull(ss.DateTo,CONVERT(SMALLDATETIME,CONVERT(VARCHAR(10),@rundate,105),105))))) END,
@RunNo,										--#12208
ROW_NUMBER() OVER(ORDER BY a.acctno)		--#12208	
FROM StoreCardPaymentDetails s
LEFT JOIN PrevSCardStat ss on ss.acctno = s.acctno										--IP - 23/03/12 - #9812
JOIN  acct a ON s.acctno = a.acctno
JOIN custacct ca ON a.acctno = ca.acctno
JOIN #ActivatedDates ad ON s.acctno = ad.acctno				--#12199
WHERE
( s.status = 'A' OR a.outstbal >0) AND  -- must have active card or balance >0
( (isnull(ss.DateTo,ad.ActivatedDate) < DATEADD(MONTH,-3,@rundate) AND StatementFrequency = 'Q') -- quarterly			--#12199 --IP - 22/03/12 - #9812
OR (isnull(ss.DateTo,ad.ActivatedDate) < DATEADD(MONTH,-1,@rundate) AND StatementFrequency = 'M') -- monthly			--#12199
OR (isnull(ss.DateTo,ad.ActivatedDate) < DATEADD(MONTH,-2,@rundate) AND StatementFrequency = 'B') -- bimonthly			--#12199
OR (isnull(ss.DateTo,ad.ActivatedDate) < DATEADD(MONTH,-6,@rundate) AND StatementFrequency = 'E') -- every 6 months		--#12199
)
AND ISNULL(nostatements,0) !=1
AND ca.hldorjnt = 'H'
AND (@rundate >= s.FirstStatementDate OR FirstStatementDate IS NULL)			--#12199 - if first statement then can generate

-- Update MinPayment for this statement #8445
-- Get Current statement
select AcctNo,MAX(DateFrom) as DateFrom,MAX(DateTo) as DateTo into #currStmt
from StoreCardStatement
where DatePrinted = @rundate						--IP - 18/04/12	-- #9941
Group by Acctno
-- Get Previous statement
select s.AcctNo,MAX(s.DateFrom) as DateFrom ,MAX(s.DateTo) as DateTo into #prevStmt
from StoreCardStatement s INNER JOIN #currStmt c on s.Acctno=c.AcctNo
where s.DateFrom =(select MAX(DateFrom) from StoreCardStatement s1 where s.acctno=s1.acctno and s1.datefrom <c.DateFrom)
Group by s.Acctno
-- Insert dummy if first statement
insert into #prevStmt
select distinct s.acctno,'1900-01-01','1900-01-01'
From StoreCardStatement s
where not exists (select * from #prevStmt p where s.acctno=p.acctno)
-- Get Balance at current statement date
select f.acctno,c.DateFrom,SUM(transvalue) as stmtBal,CAST(0 as MONEY) as Payments into #currTxn
from fintrans f INNER JOIN #currStmt c on f.acctno=c.AcctNo
				LEFT OUTER JOIN #prevStmt p on p.acctno=f.acctno
--where datetrans > c.datefrom
--	and datetrans<=c.dateto 
where datetrans<=c.dateto		-- #11200 
Group by f.acctno,c.DateFrom
-- Get Payments made since Current statement from date
UPDATE #currTxn
	set Payments= ISNULL((Select sum(Transvalue) from fintrans f 
			where f.acctno=c.acctno and transtypecode='PAY' and datetrans>c.datefrom and datetrans <= @rundate),0)		-- jec 27/01/12
from #currTxn c

-- Update minimum payment for Current statement
UPDATE s
	set StmtMinPayment=case
		when t.stmtBal<=0 then 0	-- jec 27/01/12
		when t.stmtBal<@MinPayment then t.stmtBal		-- jec 27/01/12
		when t.stmtbal<pd.MonthlyAmount then t.stmtbal
		-- #10120 - minpay + previous minpay > outstanding balance
		-- #10081 moved code
		when pd.MonthlyAmount> 0  and pd.MonthlyAmount>(t.stmtBal * @minPercent/100) then pd.MonthlyAmount	+ 
				case when ISNULL(sp.StmtMinPayment,0)+t.Payments <0 then 0				-- overpayment
					when pd.MonthlyAmount + ISNULL(sp.StmtMinPayment,0)+t.Payments > t.stmtBal then t.stmtBal		-- fixed pay > outstanding balance
					else ISNULL(sp.StmtMinPayment,0)+t.Payments end
					
		when t.stmtBal* @minPercent/100 < @MinPayment and @MinPayment +				
				case when ISNULL(sp.StmtMinPayment,0)+t.Payments <0 then 0				-- overpayment					
					when @MinPayment + ISNULL(sp.StmtMinPayment,0)+t.Payments > t.stmtBal then t.stmtBal		-- minpay > outstanding balance
					else ISNULL(sp.StmtMinPayment,0)+t.Payments end > t.stmtBal then t.stmtBal
		
		when t.stmtBal* @minPercent/100 < @MinPayment then @MinPayment + 
				case when ISNULL(sp.StmtMinPayment,0)+t.Payments <0 then 0				-- overpayment					
					when @MinPayment + ISNULL(sp.StmtMinPayment,0)+t.Payments > t.stmtBal then t.stmtBal		-- minpay > outstanding balance
					else ISNULL(sp.StmtMinPayment,0)+t.Payments end
		-- 9841 -Fixed Payment
		--when t.stmtbal<pd.MonthlyAmount then t.stmtbal
		-- #10081 moved code	
		--when pd.MonthlyAmount> 0  and pd.MonthlyAmount>(t.stmtBal * @minPercent/100) then pd.MonthlyAmount	+ 
		--		case when ISNULL(sp.StmtMinPayment,0)+t.Payments <0 then 0				-- overpayment
		--			when pd.MonthlyAmount + ISNULL(sp.StmtMinPayment,0)+t.Payments > t.stmtBal then t.stmtBal		-- fixed pay > outstanding balance
		--			else ISNULL(sp.StmtMinPayment,0)+t.Payments end
		--when pd.MonthlyAmount> t.stmtBal then pd.MonthlyAmount
					
		else Cast(t.stmtBal * @minPercent/100 + 
		
				case when ISNULL(sp.StmtMinPayment,0)+t.Payments <0 then 0				-- overpayment
					else ISNULL(sp.StmtMinPayment,0)+t.Payments end	as DECIMAL(11,2))			-- Curr MinPayment + any previous payment not paid
		end
from StoreCardStatement s INNER JOIN #currStmt c on s.Acctno=c.AcctNo and s.DateFrom=c.DateFrom
		INNER JOIN #currTxn t on s.Acctno=t.acctno
		INNER JOIN #prevStmt p on s.Acctno=p.acctno
		LEFT Outer JOIN StoreCardStatement sp on sp.Acctno=p.AcctNo and sp.DateFrom=p.DateFrom
		INNER JOIN StoreCardPaymentDetails pd on pd.Acctno=s.AcctNo

-- Update min payment from Previous Statement	(makes code easier when reprinting statement in Cosacs)
UPDATE s
	set PrevStmtMinPayment=ISNULL(sp.StmtMinPayment,0)
from StoreCardStatement s INNER JOIN #currStmt c on s.Acctno=c.AcctNo and s.DateFrom=c.DateFrom		
		INNER JOIN #prevStmt p on s.Acctno=p.acctno
		LEFT Outer JOIN StoreCardStatement sp on sp.Acctno=p.AcctNo and sp.DateFrom=p.DateFrom
		
			
UPDATE StoreCardPaymentDetails SET DateLastStatementPrinted = s.DateTo,
	MinimumPayment=case		-- #9513 jec 10/02/12  set minimum payment on Paymentdetails (excl previous unpaid amount)
		when ISNULL(t.stmtBal,0) <=0 then 0	
		when ISNULL(t.stmtBal,0) <@MinPayment then t.stmtBal
		when t.stmtbal<MonthlyAmount then t.stmtbal		
		when ISNULL(t.stmtBal,0) * @minPercent/100 < @MinPayment then @MinPayment
		-- 9841 -Fixed Payment
		--when t.stmtbal<MonthlyAmount then t.stmtbal	
		when MonthlyAmount> 0  and MonthlyAmount>(t.stmtBal * @minPercent/100) then MonthlyAmount
		when MonthlyAmount> t.stmtBal then MonthlyAmount	
		else CAST(ISNULL(t.stmtBal,0) * @minPercent/100 as DECIMAL(11,2))
		end,
		DatePaymentDue = s.DatePaymentDue											--IP - 22/03/12 - #9812
 FROM  StoreCardStatement s LEFT outer JOIN #currTxn t on s.Acctno=t.acctno		-- jec 10/02/12
 WHERE s.Acctno = StoreCardPaymentDetails.acctno
 AND s.DatePrinted = @rundate

 SELECT f.branchno, s.Acctno,CA.custid,C.title,C.firstname,C.[name],CAD.cusaddr1, CAD.cusaddr2,CAD.cusaddr3, CAD.cuspocode,F.Value,F.DateTrans,F.CardNumber,F.Description ,
 InvoiceNumber = CASE WHEN f.acctno LIKE '___5%' THEN CONVERT(VARCHAR,f.agrmtno )
 ELSE s.Acctno 
 END 
 FROM StoreCardStatement s 
 LEFT JOIN view_FintranswithTransfers f ON s.acctno = f.acctno AND F.DateTrans BETWEEN S.DateFrom AND S.DateTo
 LEFT JOIN branch b ON b.branchno= f.branchno 
  JOIN custacct ca ON ca.acctno = s.Acctno
 JOIN customer c ON ca.custid = c.custid
 JOIN custaddress cad ON c.custid = cad.custid
 WHERE ca.hldorjnt = 'H' AND cad.datemoved IS NULL AND cad.addtype = 'H'
 --AND s.Acctno='700900007431'
 AND S.DatePrinted = @rundate
GO 

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End