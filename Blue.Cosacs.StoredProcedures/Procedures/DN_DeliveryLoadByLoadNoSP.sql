SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE Name = 'DN_DeliveryLoadByLoadNoSP' AND Type = 'P')
BEGIN
    DROP PROCEDURE DN_DeliveryLoadByLoadNoSP
END
GO


CREATE PROCEDURE DN_DeliveryLoadByLoadNoSP

--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_DeliveryLoadByLoadNoSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Load Delivery Notes for a Load No
-- Author       : D Richardson
-- Date         : 20 July 2005
--
-- For Delivery Notification. The set of data for a given Branch, Load No
-- and date is returned made up of three tables.
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 17/05/11  ip  RI Integration changes - CR1212 - #3627 - Changed joins to use ItemID rather than ItemNo. Selecting ItemID and ParentItemID columns.
--------------------------------------------------------------------------------

    -- Parameters
    @piBranchNo         INTEGER,
    @piLoadNo           INTEGER,
    @piDateDel          DATETIME,
    @Return             INTEGER OUT

AS --DECLARE
    -- Local variables
BEGIN
   
    /* Load the Delivery Notes for this Branch, Load No and date */
    SELECT  BranchNo,
            DateDel,
            LoadNo,
            BuffBranchNo AS StockLocn,
            BuffNo
    INTO    #tmpDelNotes
    FROM    DeliveryLoad
    WHERE   BranchNo    = @piBranchNo
    AND     LoadNo      = @piLoadNo
    AND     DateDel     = @piDateDel


    /* Load the Line Items for each Delivery Note */
	--IP - 24/02/09 - CR929 & 974 - Need to also return 'ParentItemNo' and 'ContractNo'
    SELECT  DISTINCT
			sc.Buffno,
            (CASE WHEN ISNULL(sc.retstocklocn,0) = 0 THEN sc.stocklocn ELSE sc.retstocklocn END) AS StockLocn,
            sc.Quantity,
            l.AcctNo,
            l.AgrmtNo,
            --l.ItemNo,				
            s.IUPC as ItemNo,		--IP - 17/05/11 - CR1212 - #3627
            l.ItemID,				--IP - 17/05/11 - CR1212 - #3627
			ISNULL(sip.IUPC, '') AS ParentItemNo, 
			l.ParentItemID,			--IP - 17/05/11 - CR1212 - #3627
			l.ContractNo
    INTO    #tmpSchedule
    FROM    #tmpDelNotes t, Schedule sc, LineItem l		--IP - 17/05/11 - CR1212 - #3627 - add join to Stockitem
	LEFT JOIN StockInfo sip ON l.ParentItemID = sip.ID,
	StockInfo s
    WHERE   sc.BuffNo       = t.BuffNo
    AND     t.StockLocn     = (CASE WHEN ISNULL(sc.retstocklocn,0) = 0 THEN sc.stocklocn ELSE sc.retstocklocn END)
    AND     sc.LoadNo       = t.LoadNo
    AND     l.AcctNo        = sc.AcctNo
    AND     l.AgrmtNo       = sc.AgrmtNo
    --AND     l.ItemNo        = sc.ItemNo
    AND     l.ItemID        = sc.ItemID		--IP - 17/05/11 - CR1212 - #3627
    AND     l.StockLocn     = sc.StockLocn
    AND     l.Iskit         = 0
    AND		l.ItemID		= s.ID		--IP - 17/05/11 - CR1212 - #3627
    AND     ((l.Quantity != 0) OR (l.Quantity = 0 AND sc.DelOrColl = 'C'))


    /* Load the Stock Items for each Account */
    SELECT  DISTINCT
			l.AcctNo,
            l.AgrmtNo,
            --l.ItemNo,
            s.IUPC as ItemNo,				--IP - 17/05/11 - CR1212 - #3627
            l.ItemID,						--IP - 17/05/11 - CR1212 - #3627
            l.ContractNo,
            l.Quantity,
            l.Price,
            l.OrdVal,
            l.StockLocn,
            l.DelQty,
            ISNULL(l.DateReqDel, CONVERT(DATETIME,'1 Jan 1900',106)) AS DateReqDel,
            ISNULL(l.TimeReqDel, '') AS TimeReqDel
    INTO    #tmpAccounts
    FROM    #tmpSchedule t, LineItem l, StockItem s
    WHERE   l.AcctNo    = t.AcctNo 
    --AND     t.ItemNo    = l.ItemNo
    AND     t.ItemID	 = l.ItemID			--IP - 17/05/11 - CR1212 - #3627
    AND     t.StockLocn = l.StockLocn
    AND     l.IsKit     = 0
    --AND     s.ItemNo    = l.ItemNo
    AND     s.ItemID    = l.ItemID			--IP - 17/05/11 - CR1212 - #3627	
    AND     s.StockLocn = l.StockLocn
    AND     s.ItemType  = 'S'


	/* DelQty is not maintained anymore, so calculate it */
	UPDATE  #tmpAccounts
	SET		DelQty =
	          ISNULL((SELECT SUM(Quantity)
					  FROM	 Delivery
					  WHERE	 AcctNo     = #tmpAccounts.AcctNo
					  AND	 AgrmtNo    = #tmpAccounts.AgrmtNo
					  --AND	 ItemNo     = #tmpAccounts.ItemNo
					  AND	 ItemID     = #tmpAccounts.ItemID		--IP - 17/05/11 - CR1212 - #3627			
					  AND	 Stocklocn  = #tmpAccounts.StockLocn
					  AND	 ContractNo = #tmpAccounts.Contractno),0)


    /* Return the Delivery Notes */
    SELECT  BranchNo,
            DateDel,
            LoadNo,
            StockLocn,
            BuffNo
    FROM    #tmpDelNotes
    ORDER BY StockLocn, BuffNo


    /* Return the Line Items for these Delivery Notes */
    SELECT  StockLocn,
            Buffno,
            Quantity,
            AcctNo,
            AgrmtNo,
            ItemNo,
            ItemID,		  --IP - 17/05/11 - CR1212 - #3627	
			ParentItemNo, --IP - 24/02/09 - CR929 & 974
			ParentItemID, --IP - 17/05/11 - CR1212 - #3627		
			ContractNo	--IP - 24/02/09 - CR929 & 974
    FROM    #tmpSchedule
    ORDER BY StockLocn, BuffNo, AcctNo, AgrmtNo, ItemNo


    /* Return the undelivered Stock Items for these Accounts */
    SELECT  AcctNo,
            AgrmtNo,
            ItemNo,
            ItemID,			--IP - 17/05/11 - CR1212 - #3627
            ContractNo,
            Quantity - DelQty AS Quantity,
            Price,
            (Price * (Quantity - DelQty)) AS OrdVal,
            StockLocn,
            DelQty,
            DateReqDel,
            TimeReqDel
    FROM    #tmpAccounts
    WHERE   Quantity != DelQty
    ORDER BY AcctNo, AgrmtNo, ItemNo, StockLocn


    /* Return the earliest delivery date as the latest date account open */
    SELECT  MAX(a.DateAcctOpen) AS MinDeliveryDate
	FROM    #tmpSchedule ts, Acct a
	WHERE   a.AcctNo = ts.Acctno


    SET @Return = @@ERROR
    RETURN @Return
END

GO
GRANT EXECUTE ON DN_DeliveryLoadByLoadNoSP TO PUBLIC
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

