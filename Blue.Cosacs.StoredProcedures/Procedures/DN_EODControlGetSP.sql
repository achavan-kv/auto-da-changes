SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_EODControlGetSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_EODControlGetSP]
GO

CREATE PROCEDURE 	dbo.DN_EODControlGetSP
			@return int OUTPUT

AS
	SET @return = 0		--initialise return code

	SELECT	interface,
			description,
			donextrun,
			dodefault
	FROM		eodcontrol

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

