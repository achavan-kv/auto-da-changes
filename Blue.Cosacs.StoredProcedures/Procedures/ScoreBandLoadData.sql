IF EXISTS (SELECT * FROM sysobjects WHERE NAME = 'ScoreBandLoadData')
DROP PROCEDURE ScoreBandLoadData
GO 
 
-- AA Procedure to load data for scoring which we are now doing within the application
CREATE PROCEDURE ScoreBandLoadData
@custid VARCHAR(20),
@acctno CHAR(12),
@return INT output
AS 
SET NOCOUNT ON 
SET @return = 0 
SELECT ISNULL(P.points,0) AS points ,
ISNULL(P.scoringband,'') AS scoringband,
ISNULL(i.scoringband,'') AS IPBand, 
ISNULL(c.scoringband,'') AS CustomerBand,ISNULL(h.band,'') AS ManualBand,
ISNULL(h.datechange,'1-Jan-1900') AS DateLastOverride, 
ISNULL(C.datelastscored,'1-Jan-1900') AS datelastscored, 
ISNULL(p.ScoreCard,'A') AS ScoreCard,
ISNULL(g.DateAuth,'1-jan-1900') AS DateAuth,
ISNULL(g.DateDel,'1-jan-1900') AS DateDel ,
p.dateprop AS DateProp, 
ISNULL(g.empeenoauth,0) AS EmployeeAuthorised
FROM dbo.proposal p 
LEFT JOIN instalplan i ON p.acctno = i.acctno 
LEFT JOIN dbo.agreement g ON i.acctno = g.acctno 
JOIN dbo.customer c ON c.custid = p.custid
LEFT JOIN CustomerScorehist h  ON h.custid = p.custid AND h.applied = 1--AND h.acctno = p.acctno Just use latest score for this customer in case override previous account
AND h.datechange = (SELECT MAX(X.datechange) FROM dbo.CustomerScoreHist X 
WHERE x.custid = h.custid AND x.applied = 1)-- AND x.acctno= h.acctno)
WHERE p.acctno = @acctno AND p.custid = @custid 
AND p.dateprop = (SELECT MAX(dateprop) FROM dbo.proposal WHERE acctno= @acctno AND custid = @custid)
--IF @@ROWCOUNT = 0
--	RAISERROR('NO data aaaarggh!',16,1)
SELECT CountryCode,
	   Band,
	   PointsFrom,
	   PointsTo,
	   ServiceCharge,
	   DateImported,
	   ImportedBy,
	   StartDate,
	   ScoreType
	   FROM TermstypeBand

SELECT MIN(datefrom) AS startdate FROM intratehistory WHERE Band <> ''
    
GO 
--SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED 
--EXEC ScoreBandLoadData
--@custid ='AD010264', @acctno = '700011395261',@return =0 
 