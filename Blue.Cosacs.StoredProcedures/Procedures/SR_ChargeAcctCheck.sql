IF EXISTS (SELECT *
		   FROM sysobjects
		   WHERE xtype = 'P'
		   AND name = 'SR_ChargeAcctCheck')
BEGIN
DROP PROCEDURE SR_ChargeAcctCheck
END
GO

CREATE PROCEDURE SR_ChargeAcctCheck
@acctno VARCHAR(12),
@return INT OUTPUT
AS
BEGIN

IF EXISTS (SELECT * 
		   FROM SR_ChargeAcct
		   WHERE acctno = @acctno)
	BEGIN
		SELECT 1
	END
	ELSE
	BEGIN
		SELECT 0
	END
END
GO
