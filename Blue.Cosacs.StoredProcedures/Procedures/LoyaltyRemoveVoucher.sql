

IF EXISTS (SELECT * FROM sysobjects
           WHERE NAME = 'LoyaltyRemoveVoucher'
           AND xtype = 'p')
BEGIN
	DROP PROCEDURE LoyaltyRemoveVoucher
END
GO

CREATE PROCEDURE LoyaltyRemoveVoucher
@acctno VARCHAR(12),
@return INT output
AS
BEGIN

UPDATE loyaltyvoucher
SET AcctnoRedeem = '', VoucherRedeemed = 0
WHERE AcctnoRedeem = @acctno

END

