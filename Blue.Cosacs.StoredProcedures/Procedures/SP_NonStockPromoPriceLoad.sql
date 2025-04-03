SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[SP_NonStockPromoPriceLoad]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[SP_NonStockPromoPriceLoad]
GO


CREATE PROCEDURE SP_NonStockPromoPriceLoad

--------------------------------------------------------------------------------
--
-- Project      : CoSACS dotNET ? 2003 Strategic Thought Ltd.
-- File Name    : SP_NonStockPromoPriceLoad.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Remove duplicates and overlaps for Promo Price load
-- Author       : Ilyas Parker
-- Date         : 15 December 2014
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


AS -- DECLARE
    -- Local variables

BEGIN
    SET NOCOUNT ON

    ----------------------------------------------------------------
    -- Delete rubbish data from existing Promo records
    ----------------------------------------------------------------

    -- The start date should not be after the end date
    DELETE FROM PromoPrice
    WHERE FromDate > ToDate

    -- There should not be another record completely overlapping
    -- the same date range
    DELETE FROM PromoPrice
    WHERE EXISTS
        (SELECT pp.ItemNo
         FROM   PromoPrice pp
         WHERE  pp.ItemNo    = PromoPrice.ItemNo
         AND    pp.StockLocn = PromoPrice.StockLocn
         AND    pp.HPorCash  = PromoPrice.HPorCash
         AND    pp.FromDate <= PromoPrice.FromDate
         AND    pp.ToDate   >= PromoPrice.ToDate
         AND   (pp.FromDate != PromoPrice.FromDate OR pp.ToDate != PromoPrice.ToDate))

    -- Make sure the existing record set has no overlaps
    -- All end dates must end before the next start date
    UPDATE PromoPrice
    SET ToDate = 
        (SELECT DATEADD(Day,-1,MIN(pp.FromDate))
         FROM   PromoPrice pp
         WHERE  pp.ItemNo    = PromoPrice.ItemNo
         AND    pp.StockLocn = PromoPrice.StockLocn
         AND    pp.HPorCash  = PromoPrice.HPorCash
         AND    pp.FromDate  > PromoPrice.FromDate
         AND    pp.FromDate <= PromoPrice.ToDate)
    WHERE EXISTS
        (SELECT pp.ItemNo
         FROM   PromoPrice pp
         WHERE  pp.ItemNo    = PromoPrice.ItemNo
         AND    pp.StockLocn = PromoPrice.StockLocn
         AND    pp.HPorCash  = PromoPrice.HPorCash
         AND    pp.FromDate  > PromoPrice.FromDate
         AND    pp.FromDate <= PromoPrice.ToDate)


    ----------------------------------------------------------------
    -- Delete rubbish data from the new Promo records
    ----------------------------------------------------------------

    -- The start date should not be after the end date
    DELETE FROM temp_nonstockpromoload
    WHERE FromDate > ToDate

    -- There should not be another record completely overlapping
    -- the same date range
    DELETE FROM temp_nonstockpromoload
    WHERE EXISTS
        (SELECT ItemNo
         FROM   temp_nonstockpromoload tl
         WHERE  tl.ItemNo    = temp_nonstockpromoload.ItemNo
         AND    tl.StockLocn = temp_nonstockpromoload.StockLocn
         AND    tl.HPorCash  = temp_nonstockpromoload.HPorCash
         AND    tl.FromDate <= temp_nonstockpromoload.FromDate
         AND    tl.ToDate   >= temp_nonstockpromoload.ToDate
         AND   (tl.FromDate != temp_nonstockpromoload.FromDate OR tl.ToDate != temp_nonstockpromoload.ToDate))

    -- Make sure the new record set has no overlaps
    -- All end dates must end before the next start date
    UPDATE temp_nonstockpromoload
    SET ToDate = 
        (SELECT DATEADD(Day,-1,MIN(tl.FromDate))
         FROM   temp_nonstockpromoload tl
         WHERE  tl.ItemNo    = temp_nonstockpromoload.ItemNo
         AND    tl.StockLocn = temp_nonstockpromoload.StockLocn
         AND    tl.HPorCash  = temp_nonstockpromoload.HPorCash
         AND    tl.FromDate  > temp_nonstockpromoload.FromDate
         AND    tl.FromDate <= temp_nonstockpromoload.ToDate)
    WHERE EXISTS
        (SELECT ItemNo
         FROM   temp_nonstockpromoload tl
         WHERE  tl.ItemNo    = temp_nonstockpromoload.ItemNo
         AND    tl.StockLocn = temp_nonstockpromoload.StockLocn
         AND    tl.HPorCash  = temp_nonstockpromoload.HPorCash
         AND    tl.FromDate  > temp_nonstockpromoload.FromDate
         AND    tl.FromDate <= temp_nonstockpromoload.ToDate)


    ----------------------------------------------------------------
    -- Remove conflicts between old and new promo records
    ----------------------------------------------------------------

    -- Delete old promo records completely overlapped by a new record
    DELETE FROM PromoPrice
    WHERE EXISTS
        (SELECT ItemNo
         FROM   temp_nonstockpromoload tl
         WHERE  tl.ItemNo    = PromoPrice.ItemNo
         AND    tl.StockLocn = PromoPrice.StockLocn
         AND    tl.HPorCash  = PromoPrice.HPorCash
         AND    tl.FromDate <= PromoPrice.FromDate
         AND    tl.ToDate   >= PromoPrice.ToDate)

    -- Terminate an old record overlapping the start of a new record
    -- a day before the new record
    UPDATE PromoPrice
    SET ToDate = 
        (SELECT DATEADD(Day,-1,MIN(tl.FromDate))
         FROM   temp_nonstockpromoload tl
         WHERE  tl.ItemNo    = PromoPrice.ItemNo
         AND    tl.StockLocn = PromoPrice.StockLocn
         AND    tl.HPorCash  = PromoPrice.HPorCash
         AND    tl.FromDate  > PromoPrice.FromDate
         AND    tl.FromDate <= PromoPrice.ToDate)
    WHERE EXISTS
        (SELECT ItemNo
         FROM   temp_nonstockpromoload tl
         WHERE  tl.ItemNo    = PromoPrice.ItemNo
         AND    tl.StockLocn = PromoPrice.StockLocn
         AND    tl.HPorCash  = PromoPrice.HPorCash
         AND    tl.FromDate  > PromoPrice.FromDate
         AND    tl.FromDate <= PromoPrice.ToDate)

    -- Start an old record overlapping the end of a new record
    -- a day after the new record
    UPDATE PromoPrice
    SET FromDate = 
        (SELECT DATEADD(Day,+1,MAX(tl.ToDate))
         FROM   temp_nonstockpromoload tl
         WHERE  tl.ItemNo    = PromoPrice.ItemNo
         AND    tl.StockLocn = PromoPrice.StockLocn
         AND    tl.HPorCash  = PromoPrice.HPorCash
         AND    tl.FromDate  < PromoPrice.FromDate
         AND    tl.ToDate   >= PromoPrice.FromDate)
    WHERE EXISTS
        (SELECT ItemNo
         FROM   temp_nonstockpromoload tl
         WHERE  tl.ItemNo    = PromoPrice.ItemNo
         AND    tl.StockLocn = PromoPrice.StockLocn
         AND    tl.HPorCash  = PromoPrice.HPorCash
         AND    tl.FromDate  < PromoPrice.FromDate
         AND    tl.ToDate   >= PromoPrice.FromDate)

    -- The above may have (intentionally) set some start dates
    -- earlier than their end dates because new records start or
    -- finish on the same day as old records.
    DELETE FROM PromoPrice
    WHERE FromDate > ToDate
    -- dates of '000000'  that the promo prices should be removed....
    Delete from PromoPrice
    where exists 
    (select *   FROM    temp_rawnonstockpromoload t
	    WHERE   datefromcash1 = '000000'
	    AND     datetocash1 = '000000'
	    and t.itemno = PromoPrice.itemno and convert(int,t.branchno) =PromoPrice.stocklocn)
    ----------------------------------------------------------------
    -- Make sure time parts start at 00:00:00 and end at 23:59:59
    ----------------------------------------------------------------
    UPDATE temp_nonstockpromoload
    SET    FromDate = CONVERT(DATETIME, CONVERT(VARCHAR(10),FromDate,121) + ' 00:00:00.000', 121),
           ToDate   = CONVERT(DATETIME, CONVERT(VARCHAR(10),ToDate,121)   + ' 23:59:59.997', 121)


    RETURN @@ERROR
END


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

