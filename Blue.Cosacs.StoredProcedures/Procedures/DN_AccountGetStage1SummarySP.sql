SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_AccountGetStage1SummarySP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_AccountGetStage1SummarySP]
GO






CREATE PROCEDURE 	dbo.DN_AccountGetStage1SummarySP
			@custid varchar(20),
			@currentAccounts int OUT,
			@settledAccounts int OUT,
			@return int OUTPUT

AS
 SET NOCOUNT ON
	SET 	@return = 0			--initialise return code
	SET	@currentAccounts = 0
	SET	@settledAccounts = 0

	SELECT	@currentAccounts = count(CA.acctno)
	FROM		custacct CA INNER JOIN
			acct A ON CA.acctno = A.acctno
	WHERE	CA.custid = @custid 
	AND		CA.hldorjnt = 'H'
	AND		A.currstatus != 'S'

	SELECT	@settledAccounts = count(CA.acctno)
	FROM		custacct CA INNER JOIN
			acct A ON CA.acctno = A.acctno
	WHERE	CA.custid = @custid 
	AND		CA.hldorjnt = 'H'
	AND		A.currstatus = 'S'

	SELECT	A.acctno as 'Account No.',
			A.outstbal as 'Outstanding Bal',
			A.arrears as 'Arrears',
			IP.instalamount as 'Instal Amount',
			A.currstatus as 'Status',			
			AG.datedel as 'Date Delivered' 
	FROM		custacct CA 
			INNER JOIN acct A ON CA.acctno = A.acctno 
			INNER JOIN agreement AG ON A.acctno = AG.acctno
			LEFT OUTER JOIN instalplan IP ON A.acctno = IP.acctno
	WHERE	CA.custid = @custid
	AND		CA.hldorjnt = 'H'
			

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

