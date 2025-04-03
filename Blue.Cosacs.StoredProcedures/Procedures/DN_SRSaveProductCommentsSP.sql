SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

if exists (select * from dbo.sysobjects
where id = object_id('[dbo].[DN_SRSaveProductCommentsSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_SRSaveProductCommentsSP]
GO


CREATE PROCEDURE dbo.DN_SRSaveProductCommentsSP
-- =============================================
-- Author:		????
-- Create date: ??
-- Title:	DN_SRSaveProductCommentsSP
--
--	This procedure will save the Comments only.
-- 
-- Change Control
-----------------
-- 10/01/11 jec CR1030 - Update datechange if comments change
-- =============================================
	-- Add the parameters for the function here
    @ServiceBranchNo        SMALLINT,
    @ServiceUniqueId        INTEGER,
    @Comments               VARCHAR(5000),
    @DateReopened			datetime = '1900-01-01',
	@Return                 INTEGER OUTPUT

AS    
    SET NOCOUNT ON
    SET @Return = 0
    

    -- Save the SR
    UPDATE  SR_ServiceRequest
    SET     
			Comments        = @Comments,
			DateChange		= @DateReopened,	
			DateReopened	= @DateReopened
			
    WHERE   ServiceBranchNo = @ServiceBranchNo
    AND     ServiceRequestNo = @ServiceUniqueId


    SET @Return = @@error

    SET NOCOUNT OFF
    RETURN @Return
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End