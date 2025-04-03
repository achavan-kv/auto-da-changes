

IF EXISTS (SELECT * FROM sysobjects
           WHERE name = 'CustomerGetBand'
           AND xtype = 'P')
BEGIN
	DROP PROCEDURE CustomerGetBand
END
GO

CREATE PROCEDURE CustomerGetBand
@acctno varchar(12),
@return int OUTPUT 
AS

BEGIN
	SELECT scoringband FROM customer
	INNER JOIN custacct ON customer.custid = custacct.custid
	WHERE custacct.acctno = @acctno
END
