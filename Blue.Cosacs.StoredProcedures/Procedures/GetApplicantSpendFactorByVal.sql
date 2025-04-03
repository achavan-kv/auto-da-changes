
GO

IF EXISTS (
			SELECT	1
			FROM	sys.procedures WITH (NOLOCK)
			WHERE	NAME = 'GetApplicantSpendFactorByVal'
					AND type = 'P'
		  )
BEGIN
	DROP PROCEDURE [dbo].[GetApplicantSpendFactorByVal]
END

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =======================================================================================
-- Project			: CoSaCS.NET
-- Procedure Name   : GetApplicantSpendFactorByVal
-- Author			: SHUBHAM GAIKWAD
-- Create Date		: 28 July 2020
-- Description		: This procedure used to get Applicant Spend Factor By value. 
-- Change Control
-- --------------
-- Date			By			Description
-- ----			--			-----------
-- 
-- =======================================================================================

CREATE PROCEDURE [dbo].[GetApplicantSpendFactorByVal]
	@income VARCHAR(10)
	,@spendFactor VARCHAR(10) OUT
	,@return INT OUT
AS
BEGIN
	SET @return = 0;
	SET NOCOUNT ON;

	DECLARE @OrgbaseValue VARCHAR(10)
	DECLARE @baseValue VARCHAR(10)

	CREATE TABLE #ApplicantSpendFactorInPercentTemp 
	(
		Id INT
		,MinimumIncome VARCHAR(10)
		,MaximumIncome VARCHAR(10)
		,ApplicantSpendFactorInPercent NUMERIC(9, 2)
	)

	INSERT INTO #ApplicantSpendFactorInPercentTemp
	EXEC GetApplicantSpendFactor 0

	SELECT	@OrgbaseValue = MinimumIncome
	FROM	#ApplicantSpendFactorInPercentTemp
	WHERE	MinimumIncome LIKE '%>%'

	SET @basevalue = SUBSTRING(@OrgbaseValue, 2, 10);
	SET @spendFactor = (	SELECT TOP 1 ApplicantSpendFactorInPercent --AS SpendFactor
							FROM #ApplicantSpendFactorInPercentTemp
							WHERE CAST(MinimumIncome AS FLOAT) <= CAST(@income AS FLOAT) AND CAST(MaximumIncome AS FLOAT) >= CAST(@income AS FLOAT)
						)

	IF (	(@spendFactor IS NULL OR @spendFactor = '')
			AND @income > @baseValue
		)
	BEGIN
		SET @spendFactor = (
				SELECT TOP 1 ApplicantSpendFactorInPercent AS SpendFactor
				FROM #ApplicantSpendFactorInPercentTemp
				WHERE MinimumIncome = @OrgbaseValue
				)
	END

	IF (@spendFactor IS NULL OR @spendFactor = '')
	BEGIN
		SET @spendFactor = '0';
	END

	IF OBJECT_ID('tempdb..#ApplicantSpendFactorInPercentTemp') IS NOT NULL
		DROP TABLE #ApplicantSpendFactorInPercentTemp
END

GO