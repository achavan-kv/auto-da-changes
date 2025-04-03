
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


IF EXISTS (SELECT * FROM SYSOBJECTS 
           WHERE NAME = 'LoyaltyCheckCustomer'
           AND xtype = 'P')
BEGIN 
DROP PROCEDURE LoyaltyCheckCustomer
END
GO

CREATE PROCEDURE [dbo].[LoyaltyCheckCustomer]
@custid VARCHAR(20),
@return INT OUTPUT

AS
BEGIN
	SELECT 1 
	FROM customer
	WHERE custid = @custid
END

GO


