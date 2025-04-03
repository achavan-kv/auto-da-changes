SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_StatusWriteSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_StatusWriteSP]
GO

CREATE PROCEDURE 	dbo.DN_StatusWriteSP
			@acctno varchar(12),
			@datechanged datetime,
			@empeeno int,
			@status char(1),
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

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

