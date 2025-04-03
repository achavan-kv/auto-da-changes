SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_LineItem_GetRealDiscount]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_LineItem_GetRealDiscount]
GO


CREATE PROCEDURE DN_LineItem_GetRealDiscount

--------------------------------------------------------------------------------
--
-- Project      : eCoSACS r 2002 Strategic Thought Ltd.
-- File Name    : DN_LineItem_GetRealDiscount.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Discount as sum of the negative order values from the Line Items
-- Author       : D Richardson
-- Date         : 11 November 2002
--
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
--
--
--------------------------------------------------------------------------------

    -- Parameters
    @piAcctNo		VARCHAR(12),

    @Return         INTEGER OUTPUT

AS
BEGIN

    SET NOCOUNT ON

    SET @Return = 0;

    -- Sum the negative order values
    
    SELECT ISNULL(SUM(OrdVal),0) AS RealDiscount
    FROM   LineItem
    WHERE  AcctNo = @piAcctNo
    AND    OrdVal < 0
    
    SET @Return = @@ERROR
    
END



GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

