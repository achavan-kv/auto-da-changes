SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_AccountGetNoOfAgreementsSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_AccountGetNoOfAgreementsSP]
GO






CREATE PROCEDURE 	dbo.DN_AccountGetNoOfAgreementsSP
			@acctno varchar(12),
			@num smallint OUT,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT 	@num = MAX(agrmtno)
	FROM		lineitem
	WHERE	acctno = @acctno

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END





GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

