IF EXISTS (SELECT * FROM sysobjects WHERE NAME ='StoreCardRateDetailsLoadbyParent' )
DROP PROCEDURE StoreCardRateDetailsLoadbyParent
GO
CREATE PROCEDURE StoreCardRateDetailsLoadbyParent @ParentId INT 
AS 

SELECT Id,
	   ParentID,
	   AppScoreFrom,
	   AppScoreTo,
	   BehaveScoreFrom,
	   BehaveScoreTo
	   PurchaseInterestRate
 FROM dbo.StoreCardRateDetails
 WHERE  parentId = @ParentId
 
 GO 