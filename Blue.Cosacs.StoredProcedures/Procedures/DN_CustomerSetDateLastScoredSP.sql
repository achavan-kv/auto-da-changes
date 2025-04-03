SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CustomerSetDateLastScoredSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CustomerSetDateLastScoredSP]
GO



CREATE PROCEDURE 	dbo.DN_CustomerSetDateLastScoredSP
			@custid varchar(20),
			@lastScored datetime,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	UPDATE	customer
	SET		datelastscored = @lastScored
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

