SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_AccountGetRFAgreementTotalSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_AccountGetRFAgreementTotalSP]
GO

CREATE PROCEDURE 	dbo.DN_AccountGetRFAgreementTotalSP
			@custid varchar(20),
			@agreementTotal money OUT,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT 	@agreementTotal = isnull(sum(A.agrmttotal), 0)
	FROM	 	acct A INNER JOIN custacct CA
	ON 		A.acctno = CA.acctno 
	WHERE 	CA.custid = @custid
	AND 		A.accttype = 'R'
	AND 		CA.hldorjnt = 'H'

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

