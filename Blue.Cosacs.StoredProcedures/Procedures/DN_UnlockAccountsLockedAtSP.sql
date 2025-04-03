SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_UnlockAccountsLockedAtSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_UnlockAccountsLockedAtSP]
GO


CREATE PROCEDURE [dbo].[DN_UnlockAccountsLockedAtSP] 
	@user INT, @TimeLocked DATETIME, @return INT OUTPUT 
AS
	DELETE FROM accountlocking
	WHERE lockedby = @user
	AND lockedat = @TimeLocked

	SELECT @return = @@error

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

