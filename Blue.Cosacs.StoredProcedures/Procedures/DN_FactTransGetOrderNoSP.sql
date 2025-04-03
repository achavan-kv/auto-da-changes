SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_FactTransGetOrderNoSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_FactTransGetOrderNoSP]
GO

CREATE PROCEDURE 	dbo.DN_FactTransGetOrderNoSP
			@acctno varchar(12),
			@agreementno int,
			@buffno int OUT,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT 	@buffno = buffno
	FROM	 	facttrans 
	WHERE 	acctno =@acctno 
	AND	 	agrmtno =@agreementno

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

