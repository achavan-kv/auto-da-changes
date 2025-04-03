
IF EXISTS (SELECT * FROM sysobjects WHERE NAME LIKE 'StoreCardRateDetailsSave')
DROP PROCEDURE StoreCardRateDetailsSave
GO 
CREATE  PROCEDURE StoreCardRateDetailsSave 
	@Id int,
	@ParentID INT ,
	@AppScoreFrom smallint,
	@AppScoreTo smallint,
	@BehaveScoreFrom smallint,
	@BehaveScoreTo smallint,
	@PurchaseInterestRate FLOAT,
	@user VARCHAR (50),
	@createdOn DATETIME, 
	@RateName VARCHAR(50)
AS 

	INSERT INTO dbo.StoreCardRateDetails (
		Id,
		ParentID,
		AppScoreFrom,
		AppScoreTo,
		BehaveScoreFrom,
		BehaveScoreTo,
		PurchaseInterestRate
		
	) VALUES ( 
		@Id,
		@ParentID,
		@AppScoreFrom,
		@AppScoreTo,
		@BehaveScoreFrom ,
		@BehaveScoreTo ,
		@PurchaseInterestRate
		) 	
GO 


