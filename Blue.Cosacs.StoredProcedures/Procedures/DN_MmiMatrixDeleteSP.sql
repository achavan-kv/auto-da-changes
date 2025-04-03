IF EXISTS (	SELECT	1 
			FROM	dbo.sysobjects
			WHERE	id = object_id('[dbo].[DN_MmiMatrixDeleteSP]') 
					AND OBJECTPROPERTY(id, 'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[DN_MmiMatrixDeleteSP]
END
GO


SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

-- ================================================================================
-- Project      : CoSACS.NET
-- File Name    : DN_MmiMatrixDeleteSP
-- Author       : Amit Vernekar
-- Create Date	: 03 July 2020
-- Description	: This procedure is used to delete existing MMI matrix.
-- 
-- Change Control
-- --------------
-- Date				By			Description
-- ----				--			-----------
-- 			
-- ================================================================================


CREATE PROCEDURE [dbo].[DN_MmiMatrixDeleteSP]
    @Return INTEGER OUTPUT

AS 

BEGIN
    SET NOCOUNT ON
    SET @return = 0

    DELETE FROM [dbo].[MmiMatrix]

    SET @Return = @@error
    
    SET NOCOUNT OFF
    RETURN @Return
END

GO


