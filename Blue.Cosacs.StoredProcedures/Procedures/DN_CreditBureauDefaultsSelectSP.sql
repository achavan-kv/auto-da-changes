SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CreditBureauDefaultsSelectSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CreditBureauDefaultsSelectSP]
GO

CREATE PROCEDURE 	dbo.DN_CreditBureauDefaultsSelectSP
			@custid varchar(20),
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	C.codedescript,
			CBD.defaultsbalance,
			CBD.defaults, 
			CBD.defaultsexmotorbalance,
			CBD.defaultsexmotor		
	FROM		CreditBureauDefaults CBD
	INNER JOIN	code C 
	ON		CBD.status = C.code
	AND		C.category = 'PDC'
	WHERE	custid = @custid 

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

