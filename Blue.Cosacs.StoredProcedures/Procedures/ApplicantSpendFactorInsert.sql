

GO
IF EXISTS(	
			SELECT	1
			FROM	dbo.sysobjects
			WHERE	id = OBJECT_ID('[dbo].[ApplicantSpendFactorInsert]')
					AND OBJECTPROPERTY(id, 'IsProcedure') = 1
		 )
BEGIN
    DROP PROCEDURE [dbo].[ApplicantSpendFactorInsert]
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =======================================================================================
-- Project			: CoSaCS.NET
-- Procedure Name   : ApplicantSpendFactorInsert
-- Author			: SHUBHAM GAIKWAD
-- Create Date		: 14 July 2020
-- Description		: This procedure is used to insert apllicant spend factor ratio/percentage in a configured table.

-- Change Control
-- --------------
-- Date			By			Description
-- ----			--			-----------
-- 
-- =======================================================================================


CREATE PROCEDURE [dbo].[ApplicantSpendFactorInsert] 
	@ApplicantSpendFactorType ApplicantSpendFactorType Readonly
	,@return INT OUT
AS
BEGIN
	SET @return = 0;

	SET NOCOUNT ON;  

	-- maintaining input data to a temp table.
	SELECT	* 
	INTO	#ApplicantSpendFactorTypeTemp
	FROM	@ApplicantSpendFactorType

	--making a note if some value is marked as inactive or turned off
	UPDATE	A 
	SET		A.IsActive = 0
			, A.DateDeactivated = GETDATE()
	FROM	ApplicantSpendFactor A
			INNER JOIN #ApplicantSpendFactorTypeTemp AT1 
			ON A.Id = AT1.Id 
	WHERE	AT1.[MinimumIncome] = '0' 
			AND AT1.[MaximumIncome] = '0'

	DELETE FROM #ApplicantSpendFactorTypeTemp WHERE [MinimumIncome] ='0' and [MaximumIncome] = '0'


	--deleting duplicate values before insert on main table
	
	DELETE AT1 from #ApplicantSpendFactorTypeTemp AT1
	INNER JOIN ApplicantSpendFactor A ON 
	AT1.Id = A.Id
	WHERE AT1.[MinimumIncome] = A.[MinimumIncome] AND AT1.[MaximumIncome] = A.[MaximumIncome] 
												  AND AT1.[ApplicantSpendFactorInPercent] = A.[ApplicantSpendFactorInPercent]
	
    --UPDATE A  SET A.ISACTIVE=0 FROM	ApplicantSpendFactor A 
	--INNER JOIN #ApplicantSpendFactorTypeTemp AT1 
	--ON A.Id = AT1.Id WHERE 
	--A.[MinimumIncome] = AT1.[MinimumIncome] AND A.[MaximumIncome] = AT1.[MaximumIncome] 
	--											 AND A.[ApplicantSpendFactorInPercent] <> AT1.[ApplicantSpendFactorInPercent]


	UPDATE A  SET A.ISACTIVE=0 , A.DateDeactivated = GETDATE() FROM	ApplicantSpendFactor A 
	INNER JOIN #ApplicantSpendFactorTypeTemp AT1 
	ON A.Id = AT1.Id WHERE 
	A.[MinimumIncome] <> AT1.[MinimumIncome] OR A.[MaximumIncome] <> AT1.[MaximumIncome] 
												 OR A.[ApplicantSpendFactorInPercent] <> AT1.[ApplicantSpendFactorInPercent]

	
	---inserting values to the table		
	INSERT INTO ApplicantSpendFactor ([MinimumIncome], [MaximumIncome], [ApplicantSpendFactorInPercent],[CreatedDate],[OrgMinimumIncome])
							   Select [MinimumIncome], [MaximumIncome], [ApplicantSpendFactorInPercent], GETDATE(),
							   CASE WHEN [MinimumIncome] like '%>%'
								THEN
								SUBSTRING([MinimumIncome],2,10) + 1  
								ELSE
								[MinimumIncome]
								END AS [OrgMinimumIncome] 						
			FROM #ApplicantSpendFactorTypeTemp

	UPDATE ApplicantSpendFactor SET IsBaseOfNext = 0 
	UPDATE ApplicantSpendFactor SET IsBaseOfNext = 1 WHERE [MinimumIncome] like '%>%'
			

	IF OBJECT_ID('tempdb..#ApplicantSpendFactorTypeTemp') IS NOT NULL
	BEGIN
			DROP TABLE #ApplicantSpendFactorTypeTemp
	END


END

