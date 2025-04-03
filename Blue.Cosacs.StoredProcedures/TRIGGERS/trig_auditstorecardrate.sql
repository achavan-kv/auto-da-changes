IF EXISTS (SELECT * FROM sysobjects 
		   WHERE xtype = 'TR'
		   AND name = 'trig_StoreCardRateDetailsAudit')
BEGIN
DROP TRIGGER trig_StoreCardRateDetailsAudit
END
GO

CREATE TRIGGER trig_StoreCardRateDetailsAudit ON StoreCardRateDetails
FOR UPDATE, INSERT 
AS
BEGIN

DECLARE @now datetime

SET @now = GETDATE()

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
          BehaveScoreTo ,
          PurchaseInterestRate
        )
	SELECT    ParentID, -- Id - int
	          'D' , -- $Action - char(1)
	          @now , -- $CreatedOn - datetime
	          '' , -- $CreatedBy - varchar(50)
	          StoreCardrate.Name , -- Name - varchar(50)
	          StoreCardrate.RateFixed,
	          AppScorefrom , -- AppScorefrom - smallint
	          AppScoreTo , -- AppScoreTo - smallint
	          BehaveScoreFrom , -- BehaveScoreFrom - smallint
	          BehaveScoreTo , -- BehaveScoreTo - smallint
	          PurchaseInterestRate  -- DetailsId - int
	        FROM DELETED
	LEFT OUTER JOIN StoreCardrate ON ParentID = storecardrate.id
	
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
          BehaveScoreTo ,
          PurchaseInterestRate
        )
	SELECT    ParentID, -- Id - int
	          'I' , -- $Action - char(1)
	          @now , -- $CreatedOn - datetime
	          '' , -- $CreatedBy - varchar(50)
	          StoreCardrate.Name , -- Name - varchar(50)
	          StoreCardrate.RateFixed,
	          AppScorefrom , -- AppScorefrom - smallint
	          AppScoreTo , -- AppScoreTo - smallint
	          BehaveScoreFrom , -- BehaveScoreFrom - smallint
	          BehaveScoreTo , -- BehaveScoreTo - smallint
	          PurchaseInterestRate  -- DetailsId - int
	        FROM INSERTED
	LEFT OUTER JOIN StoreCardrate ON ParentID = storecardrate.id
END

GO