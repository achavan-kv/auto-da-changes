
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[AccountLockingInsertReviseAccountSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[AccountLockingInsertReviseAccountSP]
GO
-- =============================================
-- Author:		Jez Hemans
-- Create date: 31/03/2008
-- Description:	69650 Adds an entry into AccountLocking when an account is loaded into the Revise Account Screen
-- =============================================
CREATE PROCEDURE AccountLockingInsertReviseAccountSP
	@acctno VARCHAR(12),
	@user INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE	@lockedby int
	DECLARE	@name varchar(20)

	--Otherwise we need to check if anyone else has a current lock
	SELECT	@name = u.Fullname,
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
		IF NOT EXISTS(SELECT * FROM AccountLocking WHERE acctno = @acctno AND lockedby = @user)
		BEGIN
			INSERT INTO AccountLocking (
				acctno,
				lockedby,
				lockedat,
				lockcount,
				CurrentAction
			) VALUES ( 
				@acctno,
				@user,
				GETDATE(),
				1,
				'R' ) 
		END
		ELSE
		BEGIN
			UPDATE AccountLocking 
			SET CurrentAction = 'R'
			WHERE acctno = @acctno AND lockedby = @user
		END
	END 
END
GO
