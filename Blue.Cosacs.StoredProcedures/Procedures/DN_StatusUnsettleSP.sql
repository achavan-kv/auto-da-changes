SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_StatusUnsettleSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_StatusUnsettleSP]
GO

CREATE PROCEDURE 	dbo.DN_StatusUnsettleSP
			@acctno varchar(12),
			@datechanged datetime,
			@empeeno int, 	
			@status char(1) OUT,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SET	@status  = 1

	SELECT	TOP 1
			@status = statuscode
	FROM		status 
	WHERE	acctno = @acctno
	AND		statuscode != 'S'
	ORDER BY	datestatchge DESC

	INSERT	
	INTO		status
			(origbr, acctno, datestatchge, empeenostat, statuscode)
	VALUES	(0, @acctno, @datechanged, @empeeno, @status)

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

