SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_InterfaceSetDateFinishSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_InterfaceSetDateFinishSP]
GO

CREATE PROCEDURE 	dbo.DN_InterfaceSetDateFinishSP
			@interface varchar(10),
			@runno int,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	UPDATE	interfacecontrol
	SET     		datefinish = GETDATE()
	WHERE   	interface = UPPER(@interface)
	AND     		runno = @runno

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

