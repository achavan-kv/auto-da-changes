SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_DeliveryLoad_DeleteByBuffNo]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_DeliveryLoad_DeleteByBuffNo]
GO


CREATE PROCEDURE DN_DeliveryLoad_DeleteByBuffNo

--------------------------------------------------------------------------------
--
-- Project      : eCoSACS r 2002 Strategic Thought Ltd.
-- File Name    : DN_DeliveryLoad_DeleteByBuffNo.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Load delivery notes for immediate delivery
-- Author       : D Richardson
-- Date         : 8 November 2002
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
    @piStockLocn	INTEGER,
    @piBuffNo		INTEGER,

    @Return         INTEGER OUTPUT

AS DECLARE
    -- Local variables
    @BranchNo       SMALLINT, 
    @DateDel        DATETIME,
    @LoadNo         SMALLINT,
    @pickListNumber INT,	-- 68184 SC 26-7-07
    @pickListBranchNumber SMALLINT
           
BEGIN

    SET NOCOUNT ON

    SET @Return = 0;

    -- Delete the Transport Load Schedule for a delivery
    
    -- First record the Transport Schedule being delivered
    SELECT @BranchNo = BranchNo, 
           @DateDel  = DateDel,
           @LoadNo   = LoadNo
    FROM   DeliveryLoad
    WHERE  BuffNo       = @piBuffNo
    AND    BuffBranchNo = @piStockLocn

    SELECT @pickListNumber = picklistnumber,
           @pickListBranchNumber = picklistbranchNumber
    FROM   schedule 
    WHERE  BuffNo       = @piBuffNo
    AND    BuffBranchNo = @piStockLocn
    AND    LoadNo = @LoadNo
    AND    DateDelPlan = @DateDel
    AND    stocklocn = @BranchNo
    
    -- Delete the Delivery Note
    DELETE
    FROM   DeliveryLoad
    WHERE  BuffNo       = @piBuffNo
    AND    BuffBranchNo = @piStockLocn
    
    -- Update the Transport Schedule status to 'D' but
    -- only if all the Delivery Notes have been delivered
    UPDATE TransptSched
    SET    DeliveryStatus = 'D'
    WHERE  BranchNo = @BranchNo
    AND    DateDel  = @DateDel
    AND    LoadNo   = @LoadNo
    AND NOT EXISTS (SELECT * FROM DeliveryLoad
                    WHERE  BranchNo = @BranchNo
                    AND    DateDel  = @DateDel
                    AND    LoadNo   = @LoadNo)
                    
    UPDATE  picklist 
       SET  datedel = @DateDel
     WHERE  BranchNo = @pickListBranchNumber 
       AND  pickListNumber   = @pickListNumber
       AND	ordertransport = 'O'

    SET @Return = @@ERROR
        
END



GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

