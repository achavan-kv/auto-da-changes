IF EXISTS (SELECT * FROM sysobjects
		   WHERE name = 'View_StoreCardValidationandLimits'
		   AND xtype = 'V')
BEGIN
	DROP VIEW View_StoreCardValidationandLimits
END
GO


CREATE VIEW View_StoreCardValidationandLimits
AS
	SELECT Storecard.Acctno, StoreCard.CardName,StoreCard.CardNumber, ExpirationMonth,ExpirationYear,StatusCode,c.StoreCardAvailable, c.StoreCardLimit,
		   c.creditblocked, ~c.StoreCardApproved AS suspended, a.outstbal AS Balance, SPD.InterestRate   -- Not the ~ on storecardapproved. This flips the bit.
	FROM StoreCard
	INNER JOIN StoreCardStatus s ON s.CardNumber = StoreCard.CardNumber
	INNER JOIN custacct ca ON StoreCard.AcctNo = ca.acctno
	INNER JOIN customer c ON ca.custid = c.custid
	INNER JOIN acct a ON a.acctno = storecard.AcctNo
	INNER JOIN StoreCardPaymentDetails SPD ON storecard.AcctNo = SPD.acctno
	WHERE ca.hldorjnt = 'H'
	AND DateChanged = (SELECT MAX(DateChanged) 
						 FROM StoreCardstatus s2
						 WHERE s2.CardNumber = s.CardNumber)

GO

