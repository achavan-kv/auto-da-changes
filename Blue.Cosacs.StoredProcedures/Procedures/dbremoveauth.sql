SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[dbremoveauth]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[dbremoveauth]
GO


CREATE PROCEDURE dbremoveauth	@acctno	char(12)
AS
DECLARE
	@status	integer

	delete 
	from	delauthorise
	where	acctno = @acctno; 

	set @status = @@error;
	if @status != 0 
	BEGIN
		/* raise error 73 'delauthorise delete failed';	*/
		/* Pass message, add Severity=1, state=msg-number)	*/
		raiserror ('Error 73: delauthorise delete failed',1,73);
	END
RETURN @status;
/* -------------------------------------------------- Incomplete */


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

