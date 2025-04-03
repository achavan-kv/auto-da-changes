SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CustomerSetPotentialCalcDateSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CustomerSetPotentialCalcDateSP]
GO



CREATE PROCEDURE 	dbo.DN_CustomerSetPotentialCalcDateSP
			@custid varchar(20),
			@calcDate datetime,
			@score int,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	UPDATE	customer_potential_spend
	SET	    calc_date = @calcDate,
			score = @score
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

