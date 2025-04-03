
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM SYSOBJECTS 
           WHERE NAME = 'LoyaltyGetCharges'
            AND xtype = 'P')
BEGIN 
DROP PROCEDURE LoyaltyGetCharges
END
GO

CREATE PROCEDURE [dbo].[LoyaltyGetCharges]  
@Custid VARCHAR(20), 
@acctno VARCHAR(12) OUTPUT,
@amount MONEY OUTPUT,
@active BIT OUTPUT, 
@return INT OUTPUT   
AS  
  
BEGIN  
    SELECT @acctno = loyalty.Loyaltyacct, @amount = SUM(transvalue)
	FROM loyalty 
	LEFT OUTER JOIN fintrans ON fintrans.acctno = loyalty.Loyaltyacct
	WHERE loyalty.Custid = @Custid
	GROUP BY loyalty.Loyaltyacct
	
	IF EXISTS (SELECT *
			FROM Loyalty  
			WHERE loyalty.Custid = @custid  
			AND Loyalty.StatusAcct = 1 )
	BEGIN
		SET @active = 1 
	END
	ELSE
	BEGIN
		SET @active = 0
	END
	
	SET @return = 0
END  
GO