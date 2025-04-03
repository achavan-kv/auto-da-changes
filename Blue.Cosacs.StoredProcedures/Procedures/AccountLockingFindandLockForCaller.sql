IF EXISTS (SELECT * FROM sysobjects WHERE NAME = 'AccountLockingFindandLockForCaller')
DROP PROCEDURE AccountLockingFindandLockForCaller
GO 
CREATE PROCEDURE AccountLockingFindandLockForCaller 
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : AccountLockingFindandLockForCaller.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Lock Account
-- Date         : ??
--
-- This procedure will lock an account for a Telephone caller
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 02/06/11  IP  #3743 - LW73641 - Credit Customers multiple accounts. Lock the customer when locking an account
-- 29/06/11  jec #4120 Telephone Actions Screen - duplicate key Customer Locking error message 
-- 23/12/11  IP  LW73287 - Multiple user showing same account for action. Merged from 5.13.5.D
-- ================================================
@acctnolist VARCHAR(1000),@user INT,@acctno CHAR(12) OUT,@rundate DATETIME , @return INT OUT
AS 
SET @return = 0

DECLARE @DaysSinceAction int,
	    @Supervisor bit,
        @rowcount bit,
        @accountLocked bit,
        @customerLocked bit,
        @custid varchar(20)

SELECT @DaysSinceAction = VALUE FROM countrymaintenance WHERE codename = 'NoOfDaysSinceAction'

--get supervisor permission as he can see all accounts/customers
SET @Supervisor = Admin.CheckPermission(@user,326)

declare acct_cursor CURSOR FAST_FORWARD READ_ONLY FOR
SELECT items FROM  
SplitFN(@acctnolist,',')
OPEN acct_cursor
FETCH NEXT FROM acct_cursor INTO @acctno
WHILE @@FETCH_STATUS = 0
BEGIN
    
    select @custid = custid 
    from custacct 
    where acctno = @acctno 
        and hldorjnt = 'H'

    set @accountLocked = 0
    set @customerLocked = 0
    set @rowcount = 0

    --get write locks on the locking tables 

    UPDATE AccountLocking  with (serializable) 
    SET lockedat = @rundate
	WHERE acctno = @acctno 
        AND lockedby = @user 

    UPDATE CustomerLocking  with (serializable) 
    SET lockedat = @rundate
	WHERE CustID = @custid 
        AND lockedby = @user 

    -- Insert where not locked by someone in last hour
    select @accountLocked = 1
    from AccountLocking
    where acctno = @acctno
        and lockedby != @user
        and DATEDIFF(MINUTE, lockedat, @rundate) < 60

    -- Insert where not locked by someone in last hour
    select @customerLocked = 1
    from CustomerLocking
    where CustID = @custid
        and lockedby != @user
        and DATEDIFF(MINUTE, lockedat, @rundate) < 60

    IF(@accountLocked = 0 AND @customerLocked = 0)
    BEGIN 

        delete from AccountLocking where acctno = @acctno
        delete from CustomerLocking where CustID = @custid

        INSERT INTO AccountLocking (
			acctno,
			lockedby,
			lockedat,
			lockcount,
			CurrentAction
		) 
		SELECT @acctno ,
		@user,
		@rundate,
		1,
		'T'                                                        
		WHERE NOT EXISTS (SELECT 'a'                               -- No action made recently by another user.
                          FROM bailactionmaxaction 
		                  WHERE  acctno=@acctno
		                    AND  dateadded > DATEADD(DAY,-@DaysSinceAction,@rundate) 
                            AND @Supervisor = 0)

		SET @rowcount = @@ROWCOUNT	
		
		IF @ROWCOUNT > 0 --IP - 02/06/11 - #3743 - LW73641 - If the account has been locked, then proceed to lock the customer.
		BEGIN
		
			INSERT INTO CustomerLocking(CustID, LockedBy, LockedAt, LockCount, CurrentAction)
			VALUES (@custid, @user, getdate(), 1, 'T')
		END				
        
        SET @rowcount = @@ROWCOUNT

        IF (@rowcount != 0)
            BREAK --exit the cursor		
	END

FETCH NEXT FROM acct_cursor INTO @acctno

END

CLOSE acct_cursor
DEALLOCATE acct_cursor

IF @rowcount = 0 
BEGIN
	SET @acctno = ''
END
GO  

