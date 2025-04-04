IF EXISTS (	SELECT	1 
			FROM	dbo.sysobjects
			WHERE	id = object_id('[dbo].[DN_MmiMatrixGetSP]') 
					AND OBJECTPROPERTY(id, 'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[DN_MmiMatrixGetSP]
END
GO


SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

-- ================================================================================
-- Project      : CoSACS.NET
-- File Name    : DN_MmiMatrixGetSP
-- Author       : Amit Vernekar
-- Create Date	: 03 July 2020
-- Description	: This procedure is used to populate the MMI matrix.
-- 
-- Change Control
-- --------------
-- Date				By			Description
-- ----				--			-----------
-- 			
-- ================================================================================


CREATE PROCEDURE [dbo].[DN_MmiMatrixGetSP]
	@Return INTEGER OUTPUT

AS
BEGIN

	SET NOCOUNT ON
    SET @Return = 0

    SELECT		Label
				, FromScore
				, ToScore
				, MmiPercentage
	FROM		[dbo].[MmiMatrix] WITH(NOLOCK)
	ORDER BY	FromScore DESC

    SET @Return = @@error

	SET NOCOUNT OFF
	RETURN @Return

END
GO