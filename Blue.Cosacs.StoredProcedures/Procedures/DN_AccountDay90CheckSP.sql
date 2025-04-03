SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_AccountDay90CheckSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_AccountDay90CheckSP]
GO

CREATE PROCEDURE 	dbo.DN_AccountDay90CheckSP
			@acctno varchar(12),
			@rebate money OUTPUT,
			@return int OUTPUT

AS
 SET NOCOUNT ON
	SET 	@return = 0			--initialise return code
	SET	@rebate = 0

	SELECT 	@rebate = AG.servicechg
	FROM		acct A INNER JOIN 
			agreement AG ON A.acctno = AG.acctno INNER JOIN
			termstype TT ON A.termstype = TT.termstype
	WHERE	A.acctno = @acctno	
	AND		TT.description like '90%'
	AND		A.outstbal > 0
	AND		A.outstbal <= AG.servicechg
	AND		isnull(AG.dateagrmt, '1/1/1900') != '1/1/1900'
	AND		isnull(AG.datedel, '1/1/1900') != '1/1/1900'
	AND		datediff(dd, AG.datedel, getdate()) <= 90

	IF(@@rowcount = 0)
		SET	@return = -1

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

