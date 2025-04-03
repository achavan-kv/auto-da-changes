SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_ScheduleDeleteByAcctNoSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_ScheduleDeleteByAcctNoSP]
GO

CREATE PROCEDURE 	dbo.DN_ScheduleDeleteByAcctNoSP
			@acctNo varchar(12),
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	DELETE FROM Schedule
	WHERE	acctno 		= @acctNo
	AND 		quantity		> 0

	SET @return = @@error
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

