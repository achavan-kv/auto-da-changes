
GO

IF EXISTS (
			SELECT 1
			FROM sys.procedures WITH (NOLOCK)
			WHERE NAME = 'GetApplicantSpendFactor'
				AND type = 'P'
		)
	DROP PROCEDURE [dbo].[GetApplicantSpendFactor]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =======================================================================================
-- Project			: CoSaCS.NET
-- PROCEDURE Name   : GetApplicantSpendFactor
-- Author			: SHUBHAM GAIKWAD
-- Create Date		: 28 July 2020
-- Description		: This procedure is used to get applicant spend factor.
-- Version			: 001 
-- Change Control
-- --------------
-- Date			By			Description
-- ----			--			-----------
-- 
-- =======================================================================================

CREATE PROCEDURE [dbo].[GetApplicantSpendFactor] 
	@return INT OUT
AS
BEGIN
	SET @return = 0;
	SET NOCOUNT ON;

	SELECT DISTINCT Id, MinimumIncome, MaximumIncome
	INTO ##ApplicantSpendTemp 
	FROM [ApplicantSpendFactor] 
	WHERE IsActive=1 
	ORDER BY id DESC

	ALTER TABLE ##ApplicantSpendTemp ADD ISBASEOFNEXT BIT

	ALTER TABLE ##ApplicantSpendTemp ADD OrgMinimumIncome VARCHAR(10)

	ALTER TABLE ##ApplicantSpendTemp ADD ApplicantSpendFactorInPercent NUMERIC(18, 2)

	UPDATE T
	SET T.ISBASEOFNEXT = A.ISBASEOFNEXT
		,T.OrgMinimumIncome = A.OrgMinimumIncome
		,T.ApplicantSpendFactorInPercent = A.ApplicantSpendFactorInPercent
	FROM ##ApplicantSpendTemp T
	INNER JOIN [ApplicantSpendFactor] A ON T.Id = A.id

	SELECT Id
		,MinimumIncome
		,MaximumIncome
		,ApplicantSpendFactorInPercent
	FROM ##ApplicantSpendTemp
	WHERE ISBASEOFNEXT = 0
	
	UNION
	
	SELECT Id
		,MinimumIncome
		,MaximumIncome
		,ApplicantSpendFactorInPercent
	FROM ##ApplicantSpendTemp
	WHERE OrgMinimumIncome IN (
			SELECT Min(DISTINCT OrgMinimumIncome)
			FROM [ApplicantSpendFactor]
			WHERE IsBAseofNExt = 1
				AND ISACTIVE = 1
			)

	IF OBJECT_ID('tempdb..##ApplicantSpendTemp') IS NOT NULL
		DROP TABLE ##ApplicantSpendTemp
END
GO