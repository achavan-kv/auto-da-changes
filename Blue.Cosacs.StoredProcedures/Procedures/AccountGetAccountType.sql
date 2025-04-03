
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM SYSOBJECTS 
           WHERE NAME = 'AccountGetAccountType'
           AND xtype = 'P')
BEGIN 
DROP PROCEDURE AccountGetAccountType
END
GO

CREATE PROCEDURE AccountGetAccountType
@Acctno VARCHAR(12),
@return INT OUTPUT

AS
BEGIN
	SELECT accttype 
	FROM acct
	WHERE acctno = @acctno
END

