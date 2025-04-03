SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_AccountGetDueDay]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_AccountGetDueDay]
GO

CREATE PROCEDURE 	dbo.DN_AccountGetDueDay
			@custid varchar(20),
			@dueday smallint OUT,
			@return int OUTPUT
AS
	DECLARE @dateprop datetime

	SET 	@return = 0			--initialise return code
	SET	@dueday = 0

	SELECT	TOP 1 @dateprop = P.dateprop
	FROM	proposal P INNER JOIN
		acct A ON P.acctno = A.acctno	INNER JOIN
		accttype AT ON A.accttype = AT.accttype 
	WHERE	AT.genaccttype = 'R'
	AND	P.custid = @custid
	ORDER BY P.dateprop DESC

	SELECT	TOP 1 @dueday = ISNULL(I.dueday, 0)
	FROM	proposal P INNER JOIN acct A ON P.acctno = A.acctno 
			   	       INNER JOIN accttype AT ON A.accttype = AT.accttype 
			   	       LEFT OUTER JOIN instalplan I ON A.acctno = I.acctno 	
	WHERE	AT.genaccttype = 'R'
	AND	P.custid = @custid
	AND	P.dateprop != @dateprop
	ORDER BY 	P.dateprop DESC
	
	IF(@dueday = 0)
	BEGIN
		SELECT	TOP 1 @dueday = ISNULL(I.dueday, 0)
		FROM	proposal P INNER JOIN acct A ON P.acctno = A.acctno 
				           INNER JOIN accttype AT ON A.accttype = AT.genaccttype 
				           LEFT OUTER JOIN instalplan I ON A.acctno = I.acctno 	
		WHERE	AT.genaccttype = 'O'
		AND		P.custid = @custid
		ORDER BY 	P.dateprop DESC
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO