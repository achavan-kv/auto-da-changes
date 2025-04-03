
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[CM_LockAccountSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CM_LockAccountSP]
GO
-- =============================================
-- Author:		Jez Hemans
-- Create date: 05/06/2007
-- Description:	Locks the account if no current lock of type 'T' exists for this account
-- =============================================
CREATE PROCEDURE CM_LockAccountSP 
	@acctno varchar(12),
    @user INT,
	@return int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SET 	@return = 0			--initialise return code

	IF NOT EXISTS (SELECT * FROM [AccountLocking] WHERE [acctno] = @acctno AND CurrentAction = 'T')
    BEGIN 
    INSERT INTO [AccountLocking]([acctno],lockedby,lockedat,lockcount,CurrentAction)
    VALUES(@acctno,@user,GETDATE(),1,'T')
    END

	SET	@return = @@error
END
GO
