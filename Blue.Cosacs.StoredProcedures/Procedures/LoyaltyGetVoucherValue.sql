
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM SYSOBJECTS 
           WHERE NAME = 'LoyaltyGetVoucherValue'
           AND xtype = 'P')
BEGIN 
DROP PROCEDURE LoyaltyGetVoucherValue
END
GO

CREATE PROCEDURE dbo.LoyaltyGetVoucherValue
@acctno VARCHAR(12),
@return INT output
AS
BEGIN

	SET @return = 0

	SELECT VoucherValue * -1 AS VoucherValue  
	FROM LoyaltyVoucher  
	WHERE AcctnoRedeem = @acctno 
	AND VoucherRedeemed = 1    
	
END
GO