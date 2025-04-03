
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM SYSOBJECTS 
           WHERE NAME = 'CustomerGetIdByAcctno'
           AND xtype = 'P')
BEGIN 
DROP PROCEDURE CustomerGetIdByAcctno
END
GO

CREATE PROCEDURE CustomerGetIdByAcctno
@Acctno VARCHAR(12),
@return INT OUTPUT

AS
BEGIN
	SELECT custid 
	FROM custacct
	WHERE acctno = @acctno
	AND hldorjnt = 'H'
END

