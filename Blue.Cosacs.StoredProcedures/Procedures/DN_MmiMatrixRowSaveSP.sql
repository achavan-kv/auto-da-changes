IF EXISTS (	SELECT	1 
			FROM	dbo.sysobjects
			WHERE	id = object_id('[dbo].[DN_MmiMatrixRowSaveSP]') 
					AND OBJECTPROPERTY(id, 'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[DN_MmiMatrixRowSaveSP]
END
GO


SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

-- ================================================================================
-- Project      : CoSACS.NET
-- File Name    : DN_MmiMatrixRowSaveSP
-- Author       : Amit Vernekar
-- Create Date	: 03 July 2020
-- Description	: This procedure is used to save MMI matrix record.
-- 
-- Change Control
-- --------------
-- Date				By			Description
-- ----				--			-----------
-- 			
-- ================================================================================


CREATE PROCEDURE [dbo].[DN_MmiMatrixRowSaveSP]
    @Label			VARCHAR(50),
    @FromScore		SMALLINT,
    @ToScore		SMALLINT,
    @MmiPercentage  FLOAT,
    @ConfiguredDate DATETIME,
    @ConfiguredBy   INTEGER,
    @Return         INTEGER OUTPUT

AS
BEGIN

	SET @Return = 0
    SET NOCOUNT ON

    --DELETE
    --FROM   [dbo].[MmiMatrix]
    --WHERE  Label = @Label

    INSERT INTO [dbo].[MmiMatrix]
			(Label,
			 FromScore,
			 ToScore,
			 MmiPercentage,
			 ConfiguredDate,
			 ConfiguredBy)
    VALUES (
			@Label,
			@FromScore,
			@ToScore,
			@MmiPercentage,
			@ConfiguredDate,
			@ConfiguredBy)

    SET @Return = @@error
    
    SET NOCOUNT OFF
    RETURN @Return

END
GO