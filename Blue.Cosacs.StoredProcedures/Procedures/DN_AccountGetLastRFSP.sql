SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_AccountGetLastRFSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_AccountGetLastRFSP]
GO






CREATE PROCEDURE 	dbo.DN_AccountGetLastRFSP
			@custid varchar(20),
			@acctno varchar(12) OUT, 
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	TOP 1
			@acctno = A.acctno
	FROM		custacct CA INNER JOIN
			acct A ON CA.acctno = A.acctno
	WHERE	CA.custid = @custid
	AND		A.accttype = 'R'
	ORDER BY	A.acctno DESC

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END





GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

