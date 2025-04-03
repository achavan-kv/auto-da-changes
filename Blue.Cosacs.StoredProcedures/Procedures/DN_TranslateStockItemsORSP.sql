SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_TranslateStockItemsORSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_TranslateStockItemsORSP]
GO



CREATE PROCEDURE DN_TranslateStockItemsORSP

--------------------------------------------------------------------------------
--
-- Project      : eCoSACS ? 2002 Strategic Thought Ltd.
-- File Name    : DN_TranslateStockItemsORSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Translate stock item descriptions
-- Author       : D Richardson
-- Date         : 17 Jan 2003
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
     @return int =0 output

AS 
  -- DECLARE
    -- Local variables

BEGIN

    SET NOCOUNT ON

    -- Update translations
    UPDATE StockItem 
    SET    itemdescr1 = t.descr1,
           itemdescr2 = t.descr2
    FROM   StockItemTrans t
    WHERE  StockItem.itemno = t.itemno
    AND    t.descr1 is not null

    
    -- Move rows to translation table
    -- StockItem sometimes has different descriptions for the same
    -- ItemNo at different Stock Locations. So SELECT DISTINCT on 
    -- ItemNo alone first.
    INSERT INTO StockItemTrans (itemno, descr1_en, descr1, descr2_en, descr2)
    SELECT DISTINCT ItemNo, null, null, null, null
    FROM   StockItem s
    WHERE  NOT EXISTS (SELECT ItemNo
                       FROM   StockItemTrans st
                       WHERE  st.ItemNo = s.ItemNo)
    

    -- Update descriptions
    -- (Arbitrarily choose one description if more than one for the same ItemNo)
    UPDATE StockItemTrans
    SET    Descr1_en = isnull(s.ItemDescr1,''),
           Descr2_en = isnull(s.ItemDescr2,'')
    FROM   StockItem s
    WHERE  StockItemTrans.ItemNo = s.ItemNo
    AND    StockItemTrans.Descr1_en IS NULL
    AND    StockItemTrans.Descr2_en IS NULL
    
    IF (@@error != 0)
    BEGIN
        SET @return = @@error
    END

END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

