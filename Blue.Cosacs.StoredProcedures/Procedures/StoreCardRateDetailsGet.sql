IF  EXISTS(SELECT * FROM sysobjects WHERE NAME ='StoreCardRateDetailsGet')
DROP PROCEDURE  StoreCardRateDetailsGet
GO 
CREATE PROCEDURE StoreCardRateDetailsGet @id INT ,@points INT , @scorecard char(1),
@PurchaseInterestRate DECIMAL(6,3) OUT 
--, @variableRate DECIMAL(6,3) OUT
AS 

SELECT @PurchaseInterestRate=PurchaseInterestRate
FROM storecardRateDetails
WHERE (id = @id OR @id = 0)
AND (@points = 0 
OR (@scorecard = 'B' and @points >=BehaveScoreFrom AND @points <=BehaveScoreTo)
OR (@scorecard != 'B' and @points >=AppScoreFrom AND @points <=AppScoreTo))

GO 

-- What are we going to do about New Saves
-- When you save determine interest rate and band
-- then when load... 