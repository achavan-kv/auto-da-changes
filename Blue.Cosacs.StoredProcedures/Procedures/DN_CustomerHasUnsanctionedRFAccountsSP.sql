SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CustomerHasUnsanctionedRFAccountsSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CustomerHasUnsanctionedRFAccountsSP]
GO


CREATE PROCEDURE 	dbo.DN_CustomerHasUnsanctionedRFAccountsSP
			@custid varchar(20),
			@hasAccounts tinyint OUT,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	IF EXISTS	(SELECT	1
			FROM		custacct CA INNER JOIN acct A
			ON		CA.acctno = A.acctno INNER JOIN accttype AT
			ON		A.accttype = AT.genaccttype
			WHERE	A.accttype = 'R'
			AND		CA.custid = @custid
			AND		CA.hldorjnt = 'H'
			AND		A.currstatus = '0')
	BEGIN
		SET	@hasAccounts = 1
	END
	ELSE
	BEGIN
		SET	@hasAccounts = 0
	END

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

