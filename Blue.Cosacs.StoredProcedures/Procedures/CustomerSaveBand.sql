

IF EXISTS (SELECT * FROM sysobjects
           WHERE name = 'CustomerSaveBand'
           AND xtype = 'P')
BEGIN
	DROP PROCEDURE CustomerSaveBand
END
GO

CREATE PROCEDURE CustomerSaveBand
@custid VARCHAR(20),
@band char(1),
@return INT out

AS

SET @return = 0

BEGIN
	UPDATE customer
	SET scoringband = @band
	WHERE custid = @custid
END

GO 