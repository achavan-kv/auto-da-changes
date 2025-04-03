SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[AccountLockingSelectReviseAccountSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[AccountLockingSelectReviseAccountSP]
GO
-- =============================================
-- Author:		Jez Hemans
-- Create date: 31/03/2008
-- Description:	69650 Checks for an entry in AccountLocking when the Goods Return screen loads an account
-- =============================================
CREATE PROCEDURE AccountLockingSelectReviseAccountSP
	@acctno VARCHAR(12),
	@user INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    IF EXISTS(SELECT * FROM AccountLocking WHERE acctno = @acctno AND lockedby = @user AND CurrentAction = 'R')
    BEGIN
		SELECT 1
	END
END
GO