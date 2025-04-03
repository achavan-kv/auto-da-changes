SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_ValidLockSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_ValidLockSP]
GO


CREATE PROCEDURE 	dbo.DN_ValidLockSP
			@acctno varchar(12),
			@user int,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	UPDATE	AccountLocking
	SET		lockedat = getdate()
	WHERE	acctno = @acctno
	AND		lockedby = @user
	AND		(datediff(minute, lockedat, getdate()) <120)
	AND		lockcount > 0

	IF(@@rowcount = 0) and @acctno not like '___5%' -- UAT 76 3510 prevent error message when saving cashiertotals - locking only really applies to Cash and HP accounts anyway
	BEGIN
		RAISERROR ('Your lock on Account Number %s has expired. You must reopen the screen', 16, 1, @acctno)
	END

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
             return @return
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

