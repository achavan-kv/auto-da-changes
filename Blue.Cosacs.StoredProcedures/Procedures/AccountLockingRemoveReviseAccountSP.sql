SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[AccountLockingRemoveReviseAccountSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[AccountLockingRemoveReviseAccountSP]
GO
-- =============================================
-- Author:		Jez Hemans
-- Create date: 31/03/2008
-- Description:	69650 Removes the CurrentAction of 'R' from AccountLocking when the Revise Account Screen is closed
-- =============================================
CREATE PROCEDURE AccountLockingRemoveReviseAccountSP
	@acctno VARCHAR(12),
	@user INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    IF EXISTS(SELECT * FROM AccountLocking WHERE acctno = @acctno AND lockedby = @user AND CurrentAction = 'R')
    BEGIN
        UPDATE AccountLocking SET CurrentAction = NULL WHERE acctno = @acctno AND lockedby = @user AND CurrentAction = 'R'
	END
END
GO