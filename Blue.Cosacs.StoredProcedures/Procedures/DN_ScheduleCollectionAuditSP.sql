SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

if exists (select * from dbo.sysobjects
where id = object_id('[dbo].[DN_ScheduleCollectionAuditSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_ScheduleCollectionAuditSP]
GO

CREATE PROCEDURE dbo.DN_ScheduleCollectionAuditSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_ScheduleCollectionAuditSP.PRC
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : 
-- Author       : ??
-- Date         : ??
--
-- This procedure will retrieve the value before collection.
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 18/05/11  IP/NM 	RI Integration changes - CR1212 - #3627 - use itemID
-- ================================================
    @AcctNo             VARCHAR(12),
    @AgrmtNo            INT,
    --@ItemNo             VARCHAR(8),			
    @itemID				INT,				--IP/NM - 18/05/11 -CR1212 - #3627 - use itemID rather than itemNo
    @CurRetStockLocn    SMALLINT,
    @RetStockLocn       SMALLINT,
    @CurQuantity        FLOAT,
    @Quantity           FLOAT,
    @ChangedBy          INT,
    @return             INT OUTPUT

AS

    SET @Return = 0

    INSERT INTO CollectionNoteChange
        (Acctno, AgrmtNo, Itemno, ItemID,OldRetStockLocn, NewRetStockLocn, OldQuantity, NewQuantity, DateChanged, ChangedBy) --IP/NM - 18/05/11 -CR1212 - #3627 - use itemID rather than itemNo
    VALUES
        (@AcctNo, @AgrmtNo, '',@itemID, @CurRetStockLocn, @RetStockLocn, @CurQuantity, @Quantity, GETDATE(), @ChangedBy)

    SET @Return = @@ERROR

GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO
