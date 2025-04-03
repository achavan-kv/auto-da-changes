SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_AccountIsCancelledSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_AccountIsCancelledSP]
GO

CREATE PROCEDURE 	dbo.DN_AccountIsCancelledSP
			@acctno varchar(12),
			@cancelled smallint OUT,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	@cancelled = count(*)
	FROM	cancellation c INNER JOIN acct a ON c.acctno = a.acctno
	WHERE	c.acctno = @acctno
	AND	a.currstatus = 'S'

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO