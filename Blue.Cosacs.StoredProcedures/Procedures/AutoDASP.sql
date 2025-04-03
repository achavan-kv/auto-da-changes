
IF EXISTS (SELECT * FROM sysobjects 
		   WHERE NAME = 'AutoDASP'
		   AND xtype = 'p')
BEGIN
	DROP PROCEDURE AutoDASP
END
GO

CREATE PROCEDURE AutoDASP
@acctno VARCHAR(12),
@empeeno INT,
@return INT OUTPUT

AS

BEGIN

SET @RETURN = 0

--Auto DA for accounts CR1034
	IF  EXISTS (SELECT * FROM proposalflag
								 INNER JOIN acct a ON proposalflag.Acctno = a.acctno
								 WHERE a.acctno = @acctNo
								 AND a.accttype IN ('R','O')
								 AND checktype = 'DC'
								 AND datecleared IS NOT NULL)
	BEGIN
		exec DN_ProposalClearSP @acctno=@acctno,@empeeno=@empeeno,@source = 'Auto',@return=@return OUTPUT
	END

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
END
GO
