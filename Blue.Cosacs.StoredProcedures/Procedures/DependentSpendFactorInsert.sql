
GO

IF EXISTS (	SELECT	1 
			FROM	sys.procedures WITH (NOLOCK)
			WHERE NAME = 'DependentSpendFactorInsert' AND type = 'P')
BEGIN
			DROP PROCEDURE [dbo].[DependentSpendFactorInsert]
END
GO


-- =======================================================================================
-- Project			: CoSaCS.NET
-- PROCEDURE Name   : DependentSpendFactorInsert
-- Author			: SHUBHAM GAIKWAD
-- Create Date		: 28 July 2020
-- Description		: This procedure is used to save Dependent Spend Factor.
-- Version			: 001

-- Change Control
-- --------------
-- Date			By			Description
-- ----			--			-----------
-- 
-- =======================================================================================

CREATE PROCEDURE [dbo].[DependentSpendFactorInsert] 
	@DependentSpendFactorType DependentSpendFactorType READONLY
	,@return INT OUT
AS
BEGIN

	SET @return = 0;
	SET NOCOUNT ON;

	SELECT *
	INTO #DependentSpendFactorTypeTemp
	FROM @DependentSpendFactorType

	UPDATE D
	SET D.IsActive = 0
		,D.DateDeactivated = GETDATE()
	FROM DependentSpendFactor D
	INNER JOIN #DependentSpendFactorTypeTemp DT ON D.Id = DT.Id
	WHERE DT.[NumOfDependents] = '0'

	DELETE
	FROM #DependentSpendFactorTypeTemp
	WHERE [NumOfDependents] = '0'

	IF EXISTS (
			SELECT 1
			FROM #DependentSpendFactorTypeTemp
			WHERE [NumOfDependents] LIKE '%>%'
			)
	BEGIN
		UPDATE DependentSpendFactor
		SET IsActive = 0
		WHERE [NumOfDependents] LIKE '%>%'
	END

	DELETE DT
	FROM #DependentSpendFactorTypeTemp DT
	INNER JOIN DependentSpendFactor D ON DT.Id = D.Id
	WHERE DT.[NumOfDependents] = D.[NumOfDependents]
		AND DT.[DependnetSpendFactorInPercent] = D.[DependnetSpendFactorInPercent]

	INSERT INTO DEPENDENTSPENDFACTOR (
		[NumOfDependents]
		,[DependnetSpendFactorInPercent]
		,[CreatedDate]
		,[OrgNumOfDep]
		)
	SELECT [NumOfDependents]
		,[DependnetSpendFactorInPercent]
		,GETDATE()
		,CASE 
			WHEN [NumOfDependents] LIKE '%>%'
				THEN SUBSTRING([NumOfDependents], 2, 3) + 1
			ELSE [NumOfDependents]
			END AS [OrgNumOfDep]
	FROM #DependentSpendFactorTypeTemp

	UPDATE DEPENDENTSPENDFACTOR
	SET IsBaseOfNext = 0

	UPDATE DEPENDENTSPENDFACTOR
	SET IsBaseOfNext = 1
	WHERE [NumOfDependents] LIKE '%>%'

	IF OBJECT_ID('tempdb..#DependentSpendFactorTypeTemp') IS NOT NULL
		DROP TABLE #DependentSpendFactorTypeTemp
END
GO