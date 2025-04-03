
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[CM_UnlockAccountSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CM_UnlockAccountSP]
GO
-- =============================================
-- Author:		Jez Hemans
-- Create date: 05/06/2007
-- Description:	Unlocks the account if a current lock of type 'T' exists for this account
-- =============================================
CREATE PROCEDURE CM_UnlockAccountSP 
	@acctno varchar(12),
    @user INT,
	@return int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SET 	@return = 0			--initialise return code

	IF EXISTS (SELECT * FROM [AccountLocking] WHERE [acctno] = @acctno AND [lockedby] = @user AND CurrentAction = 'T')
    BEGIN 
    DELETE FROM [AccountLocking]
    WHERE [acctno] = @acctno AND [lockedby] = @user AND CurrentAction = 'T'
    END

	SET	@return = @@error
END
GO
