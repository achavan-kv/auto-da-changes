IF EXISTS (SELECT * FROM sysobjects
               WHERE NAME = 'CustomerGet'
               AND xtype = 'P')
BEGIN
	DROP PROCEDURE CustomerGet
END
GO

CREATE PROCEDURE CustomerGet  
@custid VARCHAR(20)  
AS  
BEGIN  
 SELECT  
	custid,otherid,branchnohdle,
	name,firstname,title,
	alias,addrsort,namesort,
	dateborn,sex,ethnicity,
	morerewardsno,effectivedate,IdNumber,
	IdType,creditblocked,RFCreditLimit,
	RFCardSeqNo,RFCardPrinted,datelastscored,
	RFDateReminded,empeenochange,datechange,
	maidenname,OldRFCreditLimit,LimitType,
	AvailableSpend = Case when AvailableSpend >0 then AvailableSpend  else 0 end,ScoringBand,InstantCredit,
	StoreType,LoanQualified,dependants,
	maritalstat,Nationality,recurringarrears,
	age,ScoreCardType
 FROM customer
 WHERE custid = @custid  
END  
