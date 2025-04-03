SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_FintransGetSundryTotalSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_FintransGetSundryTotalSP]
GO

CREATE PROCEDURE 	dbo.DN_FintransGetSundryTotalSP
			@branchno smallint,
			@before datetime,
			@total money OUT,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	DECLARE	@acctno varchar(12)

	EXEC		DN_AccountGetSundryCreditSP @branchno, @acctno OUT, @return OUT

	SELECT	@total = sum(-transvalue)
	FROM		fintrans 
	WHERE	acctno = @acctno
	AND		datetrans <= DATEADD(s,-1, DATEADD(d,1,@before))
	AND		transtypecode in ('PAY', 'COR', 'DDE', 'DDG', 'DDN', 'DDR', 'REF', 'RET', 'XFR', 'SCX')	
	
	SELECT	@total = @total + isnull(sum(-transvalue),0)
	FROM		fintrans
	WHERE	acctno = @acctno
	AND		transtypecode = 'JLX'

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

