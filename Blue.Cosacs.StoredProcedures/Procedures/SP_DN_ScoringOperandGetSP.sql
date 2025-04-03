-- Add Score card Operand
-- 

IF OBJECT_ID('dbo.DN_ScoringOperandGetSP') IS NOT NULL
	DROP PROCEDURE dbo.DN_ScoringOperandGetSP
GO
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO
Create PROCEDURE 	[dbo].[DN_ScoringOperandGetSP] 
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	--Declare @ScoreCardType char(1)
	--select @ScoreCardType = value from CountryMaintenance where Name='Behavioural Scorecard'

	--IF @ScoreCardType='A' OR @ScoreCardType ='B' OR  @ScoreCardType='P' OR @ScoreCardType ='S'
	--BEGIN
	SELECT distinct OperandID,
			OperandName,
			OperandType,
			OperandOptions,
			DropDownName
	FROM		ScoringOperand
	ORDER BY	OperandName
	--END
	--ELSE
	--BEGIN
	
	--END

	SELECT	OperandType,
			Operator
	FROM		ScoringOperator

	SELECT distinct OperandID,
			OperandName,
			OperandType,
			OperandOptions,
			DropDownName
	FROM		Equifax_ScoringOperand
	ORDER BY	OperandName

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END