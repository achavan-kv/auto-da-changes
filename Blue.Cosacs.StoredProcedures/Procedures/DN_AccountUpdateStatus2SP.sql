SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_AccountUpdateStatus2SP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_AccountUpdateStatus2SP]
GO




CREATE PROCEDURE 	dbo.DN_AccountUpdateStatus2SP
			@acctno varchar(12),
			@current char(1),
			@new char(1),
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	declare @stat char(1)

	SELECT	@stat = currstatus
	FROM		acct
	WHERE	acctno = @acctno

	IF(@@rowcount>0)
	BEGIN
		IF(@stat = @current)
		BEGIN
			UPDATE	acct
			SET		currstatus = @new
			WHERE	acctno = @acctno
		END
	END

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END



GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

