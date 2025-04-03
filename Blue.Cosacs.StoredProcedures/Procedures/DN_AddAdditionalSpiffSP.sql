
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_AddAdditionalSpiffSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_AddAdditionalSpiffSP]
GO

CREATE PROCEDURE 	dbo.DN_AddAdditionalSpiffSP
				@acctno varchar(12),
				@authorisedby int,
				@itemno varchar(10),
				@stocklocn smallInt,
				@amount money,
				@agrmtNo int,
				@empeeno int,
				@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	INSERT INTO	SalesCommissionExtraSpiffs(AcctNo, EmpeenoAuthorised, EmpeenoSpiff, ItemNo, StockLocn, AgrmtNo, SpiffAmount)
	VALUES(@acctno, @authorisedby, @empeeno, @itemNo, @stocklocn, @agrmtNo, @amount)

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

