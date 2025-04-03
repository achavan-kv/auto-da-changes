SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_FintransGetRebateSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_FintransGetRebateSP]
GO

CREATE PROCEDURE 	dbo.DN_FintransGetRebateSP
			@acctno varchar(12),
			@rebate money OUT,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	@rebate = sum(transvalue)
	FROM		fintrans 
	WHERE	acctno = @acctno 
	AND		transtypecode = 'REB'

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

