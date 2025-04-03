SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_GetInstalAcctsSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_GetInstalAcctsSP]
GO

CREATE PROCEDURE 	dbo.DN_GetInstalAcctsSP
			@acctno varchar(12),
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	DECLARE @custid varchar(20)

	CREATE TABLE #accts(acctno varchar(12),
				datefirst datetime,
				status char(1),
				type char(1),
				customername varchar(60),
				dateagrmt datetime,
				outstbal money,
				datedel datetime)
	
	SELECT	@custid = custid
	FROM		custacct
	WHERE	acctno = @acctno
	AND		hldorjnt = 'H'
	
	INSERT INTO #accts
	SELECT	CA.acctno,
			'',
			'',
			'',
			c.firstname + ' ' + c.name,
			'',
			0,
			''
	FROM    	customer c 
	INNER JOIN 	custacct ca ON ca.custid = c.custid
	WHERE	c.custid = @custid
	AND		ca.hldorjnt = 'H'
	
	UPDATE	#accts
	SET		status = acct.currstatus,
			type = acct.accttype,
			outstbal = acct.outstbal
	FROM		acct
	WHERE	acct.acctno = #accts.acctno
	
	UPDATE	#accts
	SET		dateagrmt = agreement.dateagrmt,
			datedel = agreement.datedel
	FROM		agreement
	WHERE	agreement.acctno = #accts.acctno

	UPDATE	#accts
	SET		datefirst = instalplan.datefirst
	FROM		instalplan
	WHERE	instalplan.acctno = #accts.acctno

	DELETE
	FROM		#accts
	WHERE	isnull(datedel, '1/1/1900') = '1/1/1900'

	SELECT	acctno,
			datefirst,
			outstbal,
			type,
			dateagrmt,
			customername,
			datedel
	FROM		#accts
	WHERE	status != 'S'
	AND		type NOT IN('C', 'S')

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

