
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM SYSOBJECTS 
           WHERE NAME = 'LoyaltySaveVouchers'
           AND xtype = 'P')
BEGIN 
DROP PROCEDURE LoyaltySaveVouchers
END
GO

CREATE PROCEDURE [dbo].[LoyaltySaveVouchers]  
@Voucher int,
@add BIT , 
@AcctnoRedeem VARCHAR(12),
@return INT OUTPUT  
AS  
  
BEGIN  

IF (@Voucher = 0) 
BEGIN
	UPDATE LoyaltyVoucher  
		SET VoucherRedeemed = @add,
    AcctnoRedeem = @AcctnoRedeem  
	WHERE AcctnoRedeem = @AcctnoRedeem  
END
ELSE
BEGIN
	UPDATE LoyaltyVoucher  
		SET VoucherRedeemed = @add,
    AcctnoRedeem = @AcctnoRedeem  
	WHERE voucherref = @Voucher  
END
       
END
GO
