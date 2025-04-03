SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[CM_SaveWorklistRightsSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CM_SaveWorklistRightsSP]
GO

-- =============================================
-- Author:		Alex Ayscough
-- Create date: 08/01/2009
-- Description:	Saves worklist rights 
-- =============================================
CREATE PROCEDURE [dbo].[CM_SaveWorklistRightsSP] 
    @EmpeeNo INT,
	@WorkList VARCHAR(10),
	@Empeenochange INT,
	@return INT OUTPUT
AS
	-- SET NOCOUNT ON added to marginally improve performance 
	SET NOCOUNT ON;
    SET @return = 0
	
	UPDATE CMWorkListRights
	SET 
	Empeenochange =  @Empeenochange
	WHERE EmpeeNo =  @EmpeeNo AND 
	WorkList =  @WorkList

	IF @@ROWCOUNT = 0

    INSERT INTO CMWorkListRights(
		EmpeeNo,
		WorkList,
		Empeenochange
	) VALUES ( 
		@EmpeeNo,
		@WorkList,
		@Empeenochange
	 ) 

	    SET @return = @@ERROR

	


	RETURN @return

GO 
