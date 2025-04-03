IF EXISTS (SELECT * FROM sysobjects WHERE NAME = 'StoreCardRateDelete')
 DROP PROCEDURE StoreCardRateDelete
GO 
 CREATE  PROCEDURE StoreCardRateDelete 
	@Id int,
	@user VARCHAR(50) 	
AS 

	INSERT INTO StoreCardRateAudit
	        ( Id ,
	          [$Action] ,
	          [$CreatedOn] ,
	          [$CreatedBy] ,
	          Name ,
	          RateFixed ,
	          AppScorefrom ,
	          AppScoreTo ,
	          BehaveScoreFrom ,
	          BehaveScoreTo,
	          PurchaseInterestRate
	        )
	SELECT    @id , -- Id - int
	          'D' , -- $Action - char(1)
	          GETDATE() , -- $CreatedOn - datetime
	          @user , -- $CreatedBy - varchar(50)
	          Name , -- Name - varchar(50)
	          RateFixed , -- RateFixed - bit
	          AppScoreFrom , -- AppScorefrom - smallint
	          AppScoreTo , -- AppScoreTo - smallint
	          BehaveScoreFrom , -- BehaveScoreFrom - smallint
	          BehaveScoreTo , -- BehaveScoreTo - smallint
	          PurchaseInterestRate  -- CreditLimitPercent - decimal
	 FROM storecardrate 
	 INNER JOIN StoreCardRateDetails ON StoreCardRate.Id = StoreCardRateDetails.ParentID
	 WHERE storecardrate.Id = @id 
	        
	DELETE FROM storecardRate WHERE id = @id

GO  
