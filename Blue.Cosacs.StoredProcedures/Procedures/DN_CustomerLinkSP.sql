SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CustomerLinkSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CustomerLinkSP]
GO





CREATE PROCEDURE 	dbo.DN_CustomerLinkSP
			@holder varchar(20),
			@linked varchar(20),
			@relationship varchar(1),
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	IF(@relationship != 'H')
	BEGIN
		UPDATE	customerlinks
		SET		holder	= 	@holder,
				linked 	=	@linked,
				relationship	=	@relationship
		WHERE	holder = @holder
		AND		relationship = @relationship		

		IF(@@rowcount=0)
		BEGIN
			INSERT	
			INTO		customerlinks 
					(holder, linked, relationship)
			VALUES	(@holder, @linked, @relationship)
		END
	END
			

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END




GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

