SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_LockAccountSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_LockAccountSP]
GO

CREATE PROCEDURE 	dbo.DN_LockAccountSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_LockAccountSP.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Lock Account
-- Date         : ??
--
-- This procedure will lock an account
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 02/06/11  IP  #3743 - LW73641 - Credit Customers multiple accounts. Lock the customer when locking an account
-- 07/06/11  IP  #3809 - Only insert into Customer locking and account locking if the account is linked to a customer.
-- 08/06/11  IP  #3743 - If a lock had expired and a user was attempting to gain the lock on the same account an entry was not being 
--				 posted to CustomerLocking. Entry should now be posted to CustomerLocking.
-- ================================================
			@acctno varchar(12),
			@user int,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code
	DECLARE	@lockedby int
	DECLARE	@lockedat datetime
	DECLARE	@name varchar(20)
	DECLARE @custid varchar(20)		--IP - 02/06/11 - #3743 - LW73641
	
	--IP - 02/06/11 - #3743 - LW73641 - Select the customer for the account so that we can lock the customer.
	select @custid = custid
	from custacct ca
	where ca.hldorjnt = 'H' 
	and ca.acctno = @acctno

	--attempt to refresh our own lock first to obtain an update lock
	UPDATE	AccountLocking
	SET		lockedat = getdate(),
			lockcount = lockcount + 1
	WHERE	acctno = @acctno
	AND		lockedby = @user
	AND		lockcount > 0

	--if this update was successful that's all we need to do. 
	--Otherwise we need to check if anyone else has a current lock
	IF(@@rowcount = 0)
	BEGIN		
		SELECT	@name = u.FullName,
				@lockedby = AL.lockedby
		FROM		AccountLocking AL 
		INNER JOIN Admin.[User] u ON AL.lockedby = u.Id
		WHERE	AL.acctno = @acctno
				AND AL.lockedby != @user
				AND AL.lockcount > 0
				AND (datediff(minute, AL.lockedat, getdate()) < 60)
		
		IF(@@rowcount > 0)	--if someone else has a current lock
		BEGIN
			--we cannot lock this record throw an exception
			RAISERROR ('Cannot obtain an update lock on Account Number %s Account currently locked by %s', 16, 1, @acctno, @name)
		END
		ELSE
		BEGIN
			--try to update any lock there is (which must be expired) to be our lock
			UPDATE	AccountLocking
			SET		lockedby = @user,
					lockedat = getdate(),
					lockcount = 1
			WHERE	acctno = @acctno

			--if there were no locks to update, insert one
			IF(@@rowcount = 0)
			BEGIN
				
					INSERT INTO AccountLocking	(acctno, lockedby, lockedat, lockcount)
					VALUES (@acctno, @user, getdate(), 1)
				
				--IP - 02/06/11 - #3743 - LW73641 - If the customer is not currently locked, lock the  customer
				IF NOT EXISTS(select * from CustomerLocking where CustID = @custid 
								and LockCount > 0) and @custid is not null -- IP - 07/06/11 -  #3809 - Added check to ensure that account is linked to a customer
				BEGIN
					INSERT INTO CustomerLocking(CustID, LockedBy, LockedAt, LockCount, CurrentAction)
					VALUES (@custid, @user, getdate(), 1, null)
				
				END	
				
				
			END
			ELSE	--IP - 08/06/11 - If we are able to update an expired lock then insert into Customer Locking
			BEGIN
					INSERT INTO CustomerLocking(CustID, LockedBy, LockedAt, LockCount, CurrentAction)
					VALUES (@custid, @user, getdate(), 1, null)
			END
		END
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

