SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[CM_SaveWorkLists]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[CM_SaveWorkLists]
GO

-- =============================================
-- Author:		Mohamed Nasmi
-- Create date: 21/01/2009
-- Description:	Saves Worklists
-- =============================================
CREATE PROCEDURE [dbo].[CM_SaveWorkLists] 
    @WorkList VARCHAR(10),
	@Description NVARCHAR(30),
	@return INT OUTPUT

AS
	-- SET NOCOUNT ON added to marginally improve performance 
	SET NOCOUNT ON;
    SET @return = 0
	
	UPDATE CMWorkList
	SET 
	Description=  @Description
	WHERE WorkList =  @WorkList 

	IF @@ROWCOUNT = 0

    INSERT INTO CMWorkList(
		WorkList,
		EmpeeType,
		Description
	) VALUES ( 
		@WorkList,
		'',
		@Description
	 ) 

	    SET @return = @@ERROR

	RETURN @return


GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO