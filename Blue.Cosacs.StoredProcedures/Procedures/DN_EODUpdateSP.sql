SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_EODUpdateSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_EODUpdateSP]
GO

CREATE PROCEDURE 	dbo.DN_EODUpdateSP
			@interface varchar(12),
			@donextrun char(1),
			@dodefault char(1),
			@return int OUTPUT

AS
	SET @return = 0		--initialise return code

	UPDATE	eodcontrol
	SET		donextrun = @donextrun,
			dodefault = @dodefault
	WHERE	interface = @interface

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

