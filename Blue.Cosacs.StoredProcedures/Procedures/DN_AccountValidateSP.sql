SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_AccountValidateSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_AccountValidateSP]
GO






CREATE PROCEDURE 	dbo.DN_AccountValidateSP
			@acctno char(12),
			@acctExists int OUTPUT,
			@custid varchar(20) OUTPUT,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT 	@acctno = acctno
	FROM		acct
	WHERE	acctno = @acctno

	IF(@@rowcount > 0)
	BEGIN
		SET @acctExists = 1
	END
	ELSE
	BEGIN
		SET @acctExists = 0
	END

	IF(@acctExists = 1)
	BEGIN
		SELECT 	@custid = custid
		FROM		custacct
		WHERE	acctno = @acctno
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

