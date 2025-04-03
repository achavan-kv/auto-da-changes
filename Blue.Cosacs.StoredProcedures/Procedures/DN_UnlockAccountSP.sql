SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_UnlockAccountSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_UnlockAccountSP]
GO


CREATE PROCEDURE 	dbo.DN_UnlockAccountSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_UnlockAccountSP.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Lock Account
-- Date         : ??
--
-- This procedure will unlock an account
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 02/06/11  IP  #3743 - LW73641 - Credit Customers multiple accounts. Unlock the customer when unlocking an account
-- ================================================
			@acctno varchar(12),
			@user int,
			@return int OUTPUT

AS
	DECLARE @custid varchar(20)		--IP - 02/06/11 - #3743 - LW73641
	
	--IP - 02/06/11 - #3743 - LW73641 - Select the customer for the account so that we can lock the customer.
	select @custid = custid 
	from custacct ca
	where ca.hldorjnt = 'H' 
	and ca.acctno = @acctno
	
	SET 	@return = 0			--initialise return code

	IF(@acctno = '')
	BEGIN
		if exists (select * from AccountLocking WHERE	lockedby = @user)
			DELETE	FROM		AccountLocking
    		WHERE	lockedby = @user
	END
	ELSE
	BEGIN
		UPDATE	AccountLocking
		SET		lockcount = lockcount - 1
		--WHERE	acctno LIKE rtrim(@acctno) + '%'	/* don't know why this is like it is but it may be affecting locking */
		WHERE	acctno = @acctno
		AND		lockedby = @user
		AND     lockcount > 0
		
		UPDATE CustomerLocking					--IP - 02/06/11 - #3743 - LW73641
		SET LockCount = LockCount - 1
		WHERE CustID = @custid
		AND   LockedBy = @user
		and LockCount > 0
		
		DELETE FROM accountlocking WHERE acctno= @acctno AND lockedby = @user AND lockcount <=0
		DELETE FROM CustomerLocking WHERE custid = @custid AND LockedBy = @user and LockCount <=0 --IP - 02/06/11 - #3743 - LW73641
		
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

