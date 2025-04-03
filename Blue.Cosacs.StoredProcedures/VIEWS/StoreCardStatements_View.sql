IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.tables WHERE table_name ='StoreCardStatements_View')
DROP VIEW StoreCardStatements_View
GO 
CREATE VIEW StoreCardStatements_View 
-- **********************************************************************
-- Title: StoreCardStatements_View.sql
-- Developer: Alex
-- Date: ??
-- Purpose: Store Card Statements

-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 30/01/12  jec #8445 UAT32[UAT V6] - Past due Amount - required on next Statement
-- 08/02/12  jec #9514 PaymentDueDate - required on Statement table
-- 26/09/12  jec #14235 Only one row per account required on statement file 
-- **********************************************************************
AS 

WITH UniqueCards AS			-- #14235  Select only one card for each acccount
(
	select max(CardNumber) as CardNumber,acctno from Storecard sc
	where not exists(select * from storecardstatus ss2 where ss2.CardNumber=sc.CardNumber and ss2.StatusCode='C')		-- Not a cancelled card
	group by acctno
	union
	select max(CardNumber),acctno from Storecard sc
	where exists(select * from storecardstatus ss2 where ss2.CardNumber=sc.CardNumber and ss2.StatusCode='C')			-- cancelled card where no active card
	and not exists(select acctno from Storecard sc2 where not exists(select * from storecardstatus ss2 where ss2.CardNumber=sc2.CardNumber and ss2.StatusCode='C')
	and sc2.acctno=sc.acctno)
	group by acctno
)
 

 SELECT s.Id, s.DateFrom,s.DateTo, s.DatePrinted,--ft.branchno, b.branchname, 
 s.Acctno,CA.custid,C.title,C.firstname,C.[name], c.StoreCardLimit,c.StoreCardAvailable, 
 CAD.cusaddr1,  CAD.cusaddr2,CAD.cusaddr3, CAD.cuspocode,
 d.InterestRate , --d.DatePaymentDue,	-- #9514
 s.PrevStmtMinPayment,			-- #8445 30/01/12
 s.DatePaymentDue,  -- #9514
 sc.CardNumber				-- #8894 jec 20/04/12
 FROM StoreCardStatement s 
 JOIN storecardpaymentdetails d ON s.Acctno = d.acctno
 join storecard sc on s.Acctno = sc.acctno					-- #8894 jec 20/04/12
 join UniqueCards u on  sc.CardNumber=u.CardNumber 			-- #14235 	
 JOIN custacct ca ON ca.acctno = s.Acctno
 JOIN customer c ON ca.custid = c.custid
 JOIN custaddress cad ON c.custid = cad.custid
 WHERE ca.hldorjnt = 'H' AND cad.datemoved IS NULL AND cad.addtype = 'H'

GO 

-- End End End End End End End End End End End End End End End End End End End End End End End End End End