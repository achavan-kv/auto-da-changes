SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[EODRun]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[EODRun]
GO

CREATE PROCEDURE EODRun
			
AS

	DECLARE	@status int,@statement varchar (800), @donextrun varchar (1)

	EXEC @status = dbgenerateaccts

	IF @status = 0
	BEGIN	
		EXEC @status = dbautomatedBDW
	END

	


	RETURN @status
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

