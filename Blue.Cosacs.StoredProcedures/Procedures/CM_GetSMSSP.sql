SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[CM_GetSMSSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CM_GetSMSSP]
GO
-- =============================================
-- Author:		Jez Hemans
-- Create date: 19/03/2007
-- Description:	Selects all the SMS descriptions from the Collections SMS table.
-- =============================================
CREATE PROCEDURE [dbo].[CM_GetSMSSP] 
	@return int output
AS
BEGIN
    SET @return = 0    --initialise return code

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

-- FA 18/11/09 - UAT 896 Added extra field to be used in combo box in Strategy configuration
   SELECT SMSName,SMSText, DESCRIPTION, 
   (smsName + SPACE(1) + ISNULL(DESCRIPTION,'')) AS descr1
    FROM CMSMS
    
    IF (@@error <> 0)
    BEGIN
        SET @return = @@error
    END

END
GO