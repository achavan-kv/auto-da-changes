SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CreditBureauGetLastRequestSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CreditBureauGetLastRequestSP]
GO

CREATE PROCEDURE 	dbo.DN_CreditBureauGetLastRequestSP
			@custid varchar(20),
			@datelast datetime OUT,
			@source CHAR(1),
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	@datelast = scoredate
	FROM		CreditBureau
	WHERE	custid = @custid AND Source = @source
	ORDER BY 	scoredate DESC		--just in case there's more than one

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

