SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_UnlockItemSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_UnlockItemSP]
GO


CREATE PROCEDURE 	dbo.DN_UnlockItemSP
			@user int,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	IF EXISTS(SELECT * 
		  FROM   ItemLocking 
		  WHERE	 lockedby = @user)
	BEGIN
		DELETE	
		FROM	ItemLocking
	    	WHERE	lockedby = @user
	END

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
             return @return

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

