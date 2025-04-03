IF EXISTS (SELECT * 
		   FROM sysobjects 
		   WHERE name = 'View_StoreCardAll'
		   AND xtype = 'V')
BEGIN
	DROP VIEW View_StoreCardAll
END
GO

CREATE VIEW View_StoreCardAll      
AS      

SELECT  s.CardNumber ,
		CardName ,
		IssueYear ,
		IssueMonth ,
		ExpirationYear ,
		ExpirationMonth ,
		s.AcctNo AS AcctNo,
		ExportRunNo ,
		MonthlyAmount ,
		PaymentMethod ,
		PaymentOption ,
		RateId ,
		InterestRate ,
		RateFixed ,
		StatementFrequency ,
		DateLastStatementPrinted ,
		DatePaymentDue ,
		ss.StatusCode ,
		NoStatements ,
		ContactMethod ,
		DateNotePrinted ,
		custacct.custid AS CustID,
		otherid ,
		branchnohdle ,
		name ,
		firstname ,
		title ,
		alias ,
		addrsort ,
		namesort ,
		dateborn ,
		sex ,
		ethnicity ,
		morerewardsno ,
		effectivedate ,
		IdNumber ,
		IdType ,
		creditblocked ,
		RFCreditLimit ,
		RFCardSeqNo ,
		RFCardPrinted ,
		datelastscored ,
		RFDateReminded ,
		empeenochange ,
		datechange ,
		maidenname ,
		OldRFCreditLimit ,
		LimitType ,
		AvailableSpend ,
		ScoringBand ,
		InstantCredit ,
		StoreType ,
		LoanQualified ,
		dependants ,
		maritalstat ,
		Nationality ,
		recurringarrears ,
		age ,
		ScoreCardType ,
		StoreCardApproved ,
		StoreCardLimit ,
		StoreCardAvailable ,
		SCardApprovedDate 
FROM StoreCard s
INNER JOIN StoreCardPaymentDetails pd ON s.AcctNo = pd.acctno
INNER JOIN custacct ON pd.acctno = custacct.acctno
INNER JOIN customer ON custacct.custid = customer.custid
JOIN StoreCardStatus SS ON ss.cardNumber = s.cardNumber
WHERE hldorjnt = 'H' AND ss.datechanged = (SELECT MAX(DateChanged) FROM StoreCardStatus sh 
WHERE sh.CardNumber = ss.CardNumber)
GO 

