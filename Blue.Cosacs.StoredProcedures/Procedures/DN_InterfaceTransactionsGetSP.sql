SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_InterfaceTransactionsGetSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_InterfaceTransactionsGetSP]
GO

CREATE PROCEDURE 	dbo.DN_InterfaceTransactionsGetSP
			@runno int,
			@empeeno int,
			@code varchar(4),
			@interfaceacctno varchar(10),
			@branchno int,
			@return int OUTPUT

AS

	DECLARE @branch varchar(5)

	SET 	@return = 0
	SET	@branch = convert(varchar, @branchno) + '%'

	CREATE TABLE #accts(	transvalue money,
				codedescript varchar(128),
				reference varchar(60))

	INSERT INTO 	#accts
	SELECT	f.transvalue,
			c.codedescript,
			'' as reference
	FROM		fintrans f, code c, acct a
	WHERE	a.acctno = f.acctno
	AND		runno = @runno
	AND		empeeno = @empeeno
	AND		transtypecode = @code
	AND		f.paymethod = C.code 
	AND		c.category = 'FPM'
	AND		a.securitised != 'Y'
	AND		f.acctno like @branch
	AND EXISTS(	SELECT transtypecode
			FROM	transtype t
			WHERE t.interfaceaccount = right(@interfaceacctno, len(@interfaceacctno) - 3)
                        AND f.transtypecode = t.transtypecode)

	INSERT INTO 	#accts
	SELECT	f.transvalue,
			c.codedescript,
			'' as reference
	FROM		fintrans f, code c, acct a
	WHERE	a.acctno = f.acctno
	AND		runno = @runno
	AND		empeeno = @empeeno
	AND		transtypecode = @code
	AND		f.paymethod = C.code 
	AND		c.category = 'FPM'
	AND		a.securitised = 'Y'
	AND		f.acctno like @branch
	AND EXISTS(	SELECT transtypecode
			FROM	transtype t
			WHERE t.interfacesecaccount = right(@interfaceacctno, len(@interfaceacctno) - 3)
                        AND f.transtypecode = t.transtypecode)

	INSERT INTO 	#accts
	SELECT	f.transvalue,
			c.codedescript,
			'' as reference
	FROM		fintrans f, code c, acct a
	WHERE	a.acctno = f.acctno
	AND		runno = @runno
	AND		empeeno = @empeeno
	AND		transtypecode = @code
	AND		f.paymethod = C.code 
	AND		c.category = 'FPM'
	AND		a.securitised != 'Y'
	AND		f.branchno = @branchno
	AND EXISTS(	SELECT transtypecode
			FROM	transtype t
			WHERE t.interfacebalancing = right(@interfaceacctno, len(@interfaceacctno) - 3)
                        AND f.transtypecode = t.transtypecode)

	INSERT INTO 	#accts
	SELECT	f.transvalue,
			c.codedescript,
			'' as reference
	FROM		fintrans f, code c, acct a
	WHERE	a.acctno = f.acctno
	AND		runno = @runno
	AND		empeeno = @empeeno
	AND		transtypecode = @code
	AND		f.paymethod = C.code 
	AND		c.category = 'FPM'
	AND		a.securitised = 'Y'
	AND		f.branchno = @branchno
	AND EXISTS(	SELECT transtypecode
			FROM	transtype t
			WHERE t.interfacesecbalancing = right(@interfaceacctno, len(@interfaceacctno) - 3)
                        AND f.transtypecode = t.transtypecode)

	INSERT INTO 	#accts
	SELECT	cd.depositvalue,
			c.codedescript,
			cd.reference
	FROM		cashierdeposits cd, code c
	WHERE	runno = @runno
	AND		empeeno = @empeeno
	AND		cd.code = @code
	AND		cd.paymethod = C.code 
	AND		c.category = 'FPM'

	SELECT	reference,
			codedescript,
			transvalue
	FROM		#accts

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

