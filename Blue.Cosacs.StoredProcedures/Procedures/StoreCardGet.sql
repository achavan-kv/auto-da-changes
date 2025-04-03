

IF EXISTS (SELECT * FROM sysobjects
               WHERE NAME = 'StoreCardGet'
               AND xtype = 'P')
BEGIN
	DROP PROCEDURE StoreCardGet
END
GO

-- ============================================================================================
-- Author:		Stephen Chong
-- Create date: ?
-- Description:	Procedure that returns Store Card details
-- Change Control
-- --------------

-- ============================================================================================

CREATE PROCEDURE StoreCardGet
@cardnumber bigint 
AS
BEGIN
	SELECT 
	    CardNumber ,
	    CardName ,
	    IssueYear ,
	    IssueMonth ,
	    ExpirationYear ,
	    ExpirationMonth ,
	    --InterestRate ,
	    --RateId ,
	    AcctNo
	FROM StoreCard
	WHERE Cardnumber = @cardnumber
END



