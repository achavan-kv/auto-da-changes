
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM SYSOBJECTS 
           WHERE NAME = 'LoyaltyIsLinkAccount'
           AND xtype = 'P')
BEGIN 
DROP PROCEDURE LoyaltyIsLinkAccount
END
GO

CREATE PROCEDURE [dbo].LoyaltyIsLinkAccount
@acctno CHAR(12),
@custid VARCHAR(20),
@return INT output
AS
BEGIN

IF NOT EXISTS (SELECT * FROM custacct
               WHERE acctno = @acctno
               AND custid = @custid)
 BEGIN
	SELECT 1
 END
 ELSE
 BEGIN
	SELECT 0
 END
END


GO


