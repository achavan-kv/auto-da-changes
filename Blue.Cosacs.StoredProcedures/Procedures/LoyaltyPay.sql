
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM SYSOBJECTS 
           WHERE NAME = 'LoyaltyPay'
           AND xtype = 'P')
BEGIN 
DROP PROCEDURE LoyaltyPay
END
GO

CREATE PROCEDURE [dbo].[LoyaltyPay]
@acctno CHAR(12),
@return INT OUTPUT
AS

BEGIN

	DECLARE @loyaltyperiod INT
	SET @loyaltyperiod = (SELECT value 
							 FROM countryMaintenance
							 WHERE CodeName = 'LoyaltyMembershipPeriod')
		                     
	IF (SELECT SUM(transvalue) FROM fintrans
		WHERE acctno = @acctno) = 0 

	BEGIN  
		UPDATE Loyalty  
		SET StatusAcct = 1  
		WHERE Custid = (SELECT MIN(Custid) FROM custacct  
						WHERE acctno = @acctno  
						AND hldorjnt = 'H')   
		AND StatusAcct = 4 
		
		UPDATE acct --uat 139
		SET currstatus = 'S'
		WHERE acctno = @acctno AND outstbal = 0
	END   
END  

GO


