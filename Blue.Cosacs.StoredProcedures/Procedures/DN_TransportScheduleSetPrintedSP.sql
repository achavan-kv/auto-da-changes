SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_TransportScheduleSetPrintedSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_TransportScheduleSetPrintedSP]
GO

CREATE PROCEDURE 	dbo.DN_TransportScheduleSetPrintedSP
			@loadno smallint, 
			@branchno smallint,
			@datedel datetime,
			@return int OUTPUT

AS
	SET 	@return = 0			--initialise return code

    	UPDATE  transptsched 
	SET	printed = 1
    	WHERE	loadno   = @loadno
	AND	branchno = @branchno
        AND	datedel  = @datedel

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO