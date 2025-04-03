IF  EXISTS(SELECT * FROM sysobjects WHERE NAME ='StoreCardRateDetailsGetforPoints')
DROP PROCEDURE  StoreCardRateDetailsGetforPoints
GO 
CREATE PROCEDURE StoreCardRateDetailsGetforPoints @id INT out ,@points INT ,@scorecard char(1),
@PurchaseInterestRate DECIMAL(6,3) OUT , @rateFixed BIT OUT
AS 

SELECT @rateFixed=R.rateFixed,
@PurchaseInterestRate = S.PurchaseInterestRate,
@id = R.id
FROM storecardRateDetails S, StorecardRate R
WHERE s.parentid =r.id and r.[$isDeleted]=0
AND (@points = 0 
OR (@scorecard = 'B' and @points >=BehaveScoreFrom AND @points <=BehaveScoreTo)
OR (@scorecard != 'B' and @points >=AppScoreFrom AND @points <=AppScoreTo))
AND R.isDefaultRate = 1
GO 
