SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_ScoringOperandGetSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_ScoringOperandGetSP]
GO




CREATE PROCEDURE 	dbo.DN_ScoringOperandGetSP
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT distinct OperandID,
			OperandName,
			OperandType,
			OperandOptions,
			DropDownName
	FROM		ScoringOperand
	ORDER BY	OperandName

	SELECT	OperandType,
			Operator
	FROM		ScoringOperator

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END



GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

