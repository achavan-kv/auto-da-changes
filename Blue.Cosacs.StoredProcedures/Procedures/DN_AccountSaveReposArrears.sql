SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_AccountSaveReposArrears]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_AccountSaveReposArrears]
GO



CREATE PROCEDURE 	dbo.DN_AccountSaveReposArrears
			@acctNo varchar(12),
			@reposarrears money,
			@reposvalue money,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	UPDATE	acct 
	SET		RepossArrears   = @reposarrears,
        			RepossValue     = @reposvalue
	WHERE 	AcctNo	             = @acctNo

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

