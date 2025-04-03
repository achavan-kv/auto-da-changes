
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM SYSOBJECTS 
           WHERE NAME = 'LoyaltyGetCashAccount'
          AND xtype = 'P')
BEGIN 
DROP PROCEDURE LoyaltyGetCashAccount
END
GO

-- ============================================================================================
-- Author:		Stephen Chong
-- Create date: 27/08/09
-- Description:	Retrieves the Home Club Cash account number for a customer.
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 28/05/10  IP  UAT(193) UAT5.2.1.0 Log - If a new membership was created after a previous had been cancelled
--				 and the memberno was < than the previous memberno, then the procedure would select a null
--				 for the account number due to the default asc order by. The account number is only updated later
--				 in procedure: LoyaltyAddFee.sql. Therefore select where LoyaltyAcct is not null.
-- ============================================================================================

CREATE PROCEDURE [dbo].[LoyaltyGetCashAccount]
@custid VARCHAR(20),
@return INT output
AS
BEGIN
	SELECT LoyaltyAcct
	FROM Loyalty
	WHERE custid = @custid
	AND LoyaltyAcct IS NOT NULL			--IP - 28/05/10 - UAT(193) UAT5.2.1.0 Log
END 

GO


