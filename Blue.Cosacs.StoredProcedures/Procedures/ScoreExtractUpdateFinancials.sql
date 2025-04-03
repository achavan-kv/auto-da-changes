IF EXISTS (SELECT * FROM sysobjects WHERE NAME = 'ScoreExtractUpdateFinancials')
DROP PROCEDURE ScoreExtractUpdateFinancials
GO 
CREATE PROCEDURE ScoreExtractUpdateFinancials @rundate DATETIME 
AS 
SET NOCOUNT ON 
CREATE TABLE #Financials 
(acctno CHAR(12) ,Monthlypayment MONEY ,MthlyIntAdmcharges MONEY )

INSERT INTO #Financials (acctno)
SELECT acctno FROM delinquency 
GROUP BY acctno 

UPDATE #Financials SET monthlypayment = 
ISNULL((SELECT  SUM(transvalue ) FROM fintrans f 
WHERE f.acctno= #financials.acctno 
AND f.transtypecode IN ('PAY','COR','REF','PA1','DDN')
AND f.datetrans > DATEADD(MONTH,-1,@rundate) ),0)

UPDATE #FINANCIALS 
SET MthlyIntAdmcharges= 
ISNULL((SELECT  SUM(transvalue ) FROM fintrans f 
WHERE f.acctno= #financials.acctno 
AND f.transtypecode IN ('INT','ADM' )
AND f.datetrans > DATEADD(MONTH,-1,@rundate) ),0)

UPDATE D
SET Monthlypayment = F.Monthlypayment,
MthlyIntAdmcharges= F.MthlyIntAdmcharges
FROM  Delinquency D, #FINANCIALS F
WHERE d.acctno= f.acctno 
 DROP TABLE #Financials
GO 
--EXEC ScoreExtractUpdateFinancials @rundate ='1-mar-2010'