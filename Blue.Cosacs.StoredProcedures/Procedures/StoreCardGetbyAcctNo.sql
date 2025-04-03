IF  EXISTS (SELECT * FROM sysobjects WHERE NAME LIKE 'StoreCardGetbyAcctNo')
DROP PROCEDURE StoreCardGetbyAcctNo 
GO 
CREATE PROCEDURE StoreCardGetbyAcctNo  
@acctno CHAR(12)  
AS 

SELECT
	   	   c.CardName,c.CardNumber,
	   c.IssueYear,	   c.IssueMonth,
	   c.ExpirationYear,	  c.ExpirationMonth,
	   --FixedRate,	   c.RateId,
	   c.AcctNo,	  
	   d.MonthlyAmount,
	   d.PaymentMethod, d.PaymentOption,
	   d.RateId,d.InterestRate,
	   d.RateFixed,d.StatementFrequency,d.Status,
	   d.NoStatements,d.ContactMethod,d.dateNotePrinted,
	   d.DateLastStatementPrinted,d.DatePaymentDue,
	   c.ExportRunNo 
	   FROM storecard c
	   LEFT JOIN dbo.StoreCardPaymentDetails d ON c.acctno= d.acctno 
	   WHERE c.acctno= @acctno 
	   order by  c.cardnumber 
	   -- just putting this order in as you want cards to be loaded which are not stolen
GO 
