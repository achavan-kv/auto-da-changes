SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_ScheduleGetItemSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_ScheduleGetItemSP]
GO


CREATE PROCEDURE dbo.DN_ScheduleGetItemSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_ScheduleGetItemSP.PRC
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : 
-- Date         : ??
--
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 07/06/11  IP  CR1212 - RI - use ItemID
-- ================================================
            @buffbranchno smallint,
            @buffno int,
            @acctno varchar(12),
            @agrmtno int,
            --@itemno varchar(8),
            @itemID int,								--IP - 07/06/11 - CR1212 - RI						
            @stocklocn smallint,
            @return int OUTPUT

AS
    SET @return = 0

    SELECT  *
    FROM    Schedule
    WHERE   BuffNo       = @buffNo
    AND     AcctNo       = @acctNo
    AND     AgrmtNo      = @agrmtno
    --AND     ItemNo       = @itemno
    AND		ItemID		 = @itemID						--IP - 07/06/11 - CR1212 - RI	
    AND     StockLocn    = @stocklocn

    SET @return = @@error
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

