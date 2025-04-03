
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[DN_AccountUpdateRepoStatusSP]') AND type in ('P', 'PC'))
DROP PROCEDURE [dbo].[DN_AccountUpdateRepoStatusSP]
GO
-- =============================================
-- Author:		Jez Hemans
-- Create date: 23/07/2007
-- Description:	This will update the status of an account for a repossession to 6
-- =============================================
CREATE PROCEDURE DN_AccountUpdateRepoStatusSP
	@acctno     varchar(12),
    @return     int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SET @Return = 0;
            UPDATE acct
            SET    currstatus = '6' 
            WHERE  acctno = @acctno
    SET @Return = @@ERROR
END
GO
