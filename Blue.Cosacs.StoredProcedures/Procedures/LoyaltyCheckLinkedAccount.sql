

IF EXISTS (SELECT * FROM sysobjects
           WHERE NAME = 'LoyaltyCheckLinkedAccount'
           AND xtype = 'p')
BEGIN
	DROP PROCEDURE LoyaltyCheckLinkedAccount
END
GO

CREATE PROCEDURE LoyaltyCheckLinkedAccount
@acctno VARCHAR(12),
@return INT output
AS
BEGIN

IF EXISTS (SELECT * FROM Loyalty
           WHERE LoyaltyAcct = @acctno)
BEGIN
	SELECT 1
END
ELSE
BEGIN
	SELECT 0
END

END