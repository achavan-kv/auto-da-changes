SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_InterfaceSetResultSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_InterfaceSetResultSP]
GO

CREATE PROCEDURE 	dbo.DN_InterfaceSetResultSP
			@result varchar(1),
			@interface varchar(10),
			@runno int,
			@return int OUTPUT    
AS
	
	SET 	@return = 0	

	UPDATE	interfacecontrol
	SET    	result = @result,
	        datefinish = GETDATE()
	WHERE  	interface = @interface
	AND    	runno = @runno

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

