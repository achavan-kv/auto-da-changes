IF EXISTS (SELECT * 
		   FROM sysobjects 
		   WHERE name = 'View_StoreCardRateDetailsGetforPoints'
		   AND xtype = 'V')
BEGIN
	DROP VIEW View_StoreCardRateDetailsGetforPoints
END
GO

CREATE VIEW View_StoreCardRateDetailsGetforPoints 
AS

WITH prop AS
(
	SELECT custid, MAX(ISNULL(points,0)) points, MAX(ISNULL(scorecard,'A')) scorecard 
	FROM proposal p1
	WHERE dateprop = (SELECT MAX(dateprop)
				    FROM proposal p2
				    WHERE p2.custid = p1.custid
				    AND p2.points > 0)
	GROUP BY custid
)
SELECT R.RateFixed,
S.PurchaseInterestRate,
S.Id,
R.Name,
BehaveScoreFrom,
BehaveScoreTo,
AppScoreFrom,
AppScoreTo,
prop.Custid,
prop.Points,
prop.Scorecard,
r.IsDefaultRate
FROM prop, storecardRateDetails s
INNER JOIN StorecardRate r ON s.parentid = r.id 
WHERE r.[$isDeleted] = 0