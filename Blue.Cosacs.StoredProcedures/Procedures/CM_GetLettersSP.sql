
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[CM_GetLettersSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CM_GetLettersSP]
GO
-- =============================================
-- Author:		Jez Hemans
-- Create date: 19/03/2007
-- Description:	Selects all the letter descriptions from the Code table for the Collections Module.
-- =============================================
CREATE PROCEDURE [dbo].[CM_GetLettersSP] 
	@return int output
AS
BEGIN
    SET @return = 0    --initialise return code

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT code + ' ' + codedescript AS 'codedescript' FROM code WHERE category = 'LTA'

    IF (@@error <> 0)
    BEGIN
        SET @return = @@error
    END

END
GO
