IF EXISTS (SELECT * 
		   FROM sysobjects 
		   WHERE name = 'View_StoreCardWithPayments'
		   AND xtype = 'V')
BEGIN
	DROP VIEW View_StoreCardWithPayments
END
GO

CREATE VIEW View_StoreCardWithPayments
-- **********************************************************************
-- Title: View_StoreCardWithPayments.sql
-- Developer: Alex
-- Date: 6/01/2011
-- Purpose: view

-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 22/05/12 jec #10142 The status of the active store card is displayed as TBI
-- 17/05/13 jec #13449 Store Card Limit
-- **********************************************************************      
AS      

SELECT s.CardNumber ,
       s.CardName ,
        IssueYear ,
        IssueMonth ,
        ExpirationYear ,
        ExpirationMonth ,
        s.Acctno ,
        ExportRunNo ,
        ProofAddress ,
        ProofAddNotes ,
        ProofID ,
        ProofIDNotes ,
        SecurityQ ,
        SecurityA ,
        MonthlyAmount ,
        PaymentMethod ,
        PaymentOption ,
        RateId ,
        InterestRate ,
        RateFixed ,
        StatementFrequency ,
        DateLastStatementPrinted ,
        DatePaymentDue ,
        NoStatements ,
        ContactMethod ,
        DateNotePrinted,
        c.StoreCardLimit,
        --StoreCardAvailable = CASE WHEN c.AvailableSpend - C.StoreCardAvailable >0  THEN
		StoreCardAvailable = CASE WHEN c.AvailableSpend - isnull(C.StoreCardAvailable,0) >0  THEN		-- #13449
				CASE WHEN  C.StoreCardAvailable >0 THEN  C.StoreCardAvailable ELSE 0 END ELSE  
				CASE WHEN  C.AvailableSpend >0 THEN c.AvailableSpend ELSE 0 END END ,
        s.custid,
        sd.Status AS SPDStatus,
        ss.StatusCode AS CardStatus,						-- #10142 revert Jec
        --sd.[Status] AS CardStatus,							--IP/JC - 13/04/12 - #9908
		ss.DateChanged AS CardStatusDateChanged,
		ss.EmpeeNo AS CardStatusEmpeenoChanged,
		ss.Notes AS CardStatusNotes,
		Holder = CASE  s.custid WHEN ca.custid THEN 'H' ELSE 'J' END  ,
		Cancelled = CASE ss.statusCode WHEN 'C' THEN 1 ELSE 0  END ,
		 sd.LastInterestDate ,
		 sd.MinimumPayment      
FROM StoreCard s
JOIN custacct ca ON ca.acctno = s.acctno
JOIN customer c ON ca.custid = c.custid
LEFT OUTER JOIN StoreCardStatus ss ON ss.CardNumber = s.CardNumber
LEFT OUTER JOIN StoreCardPaymentDetails sd ON s.AcctNo = sd.AcctNo
WHERE ca.hldorjnt = 'H'
AND ss.DateChanged = (SELECT MAX(ss2.datechanged) 
					  FROM StoreCardStatus ss2 
					  WHERE ss2.cardnumber = ss.cardNumber)
