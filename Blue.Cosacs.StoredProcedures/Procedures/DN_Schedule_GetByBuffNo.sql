SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_Schedule_GetByBuffNo]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_Schedule_GetByBuffNo]
GO

CREATE PROCEDURE DN_Schedule_GetByBuffNo

--------------------------------------------------------------------------------
--
-- Project      : eCoSACS r 2002 Strategic Thought Ltd.
-- File Name    : DN_Schedule_GetByBuffNo.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Load delivery notes for immediate delivery
-- Author       : D Richardson
-- Date         : 5 November 2002
--
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 06/04/10 jec  UAT61 DHL deliveries should not be loaded
-- 21/05/10 jec  UAT160 copy code from 5.1.8.4
-- 18/05/11 jec  CR1212 RI Integration
--------------------------------------------------------------------------------

    -- Parameters
 @piBranchNo		INTEGER,
    @piBuffNo		INTEGER,

    @Return         INTEGER OUTPUT

AS
BEGIN

    SET NOCOUNT ON

    SET @Return = 0;

    -- Load Schedules (Delivery Notes)
    
        SELECT ISNULL(s.BuffBranchNo,0) AS BuffBranchNo,  
           ISNULL(l.DelNoteBranch,0) AS DelNoteBranch,  
           ISNULL(s.BuffNo,0) AS BuffNo,  
           ISNULL(s.AcctNo,N'') AS AcctNo,  
           ISNULL(s.AgrmtNo,0) AS AgrmtNo,  
           CASE ISNULL(s.DelOrColl,N'') WHEN N'D' THEN N'Delivery' WHEN N'C' THEN N'Collection' WHEN N'R' THEN N'Collection'ELSE N'' END AS DelOrColl,  
           --ISNULL(s.ItemNo,N'') AS ItemNo, 
           ISNULL(si.IUPC,'IUPCmissing') AS ItemNo,		-- RI
		   ISNULL(sip.IUPC,'')  AS ParentItemNo,	--RI
           ISNULL(s.Quantity,0) AS Quantity,  
           ISNULL(s.StockLocn,0) AS StockLocn,  
           ISNULL(s.DateDelPlan,N'') AS DateDelPlan,  
           CASE WHEN s.DelorColl = 'C' THEN ISNULL(s.RetStockLocn,s.stocklocn) ELSE ISNULL(s.RetStockLocn,'') END  AS RetStockLocn,    
           --CASE WHEN s.DelorColl = 'C' AND s.retitemno != '' THEN ISNULL(s.RetItemNo,s.itemno)   
           --     WHEN s.DelorColl = 'C' AND s.retitemno = '' THEN s.itemno   
           --     ELSE ISNULL(s.RetItemNo,'') END AS RetItemNo,
           CASE WHEN s.DelorColl = 'C' AND s.RetItemID != 0 THEN ISNULL(sr.IUPC,'IUPCmissing')   -- RI
                WHEN s.DelorColl = 'C' AND s.RetItemID = 0 THEN ISNULL(si.IUPC,'IUPCmissing')   
                ELSE ISNULL(sr.IUPC,'') END AS RetItemNo,  
           CASE WHEN s.DelorColl = 'C' THEN ISNULL(s.RetVal,l.price * s.quantity) ELSE ISNULL(s.RetVal,0)END AS RetVal,  
           ISNULL(s.VanNo,N'') AS VanNo,  
           ISNULL(s.LoadNo,0) AS LoadNo,  
           ISNULL(l.itemtype,N'') AS ItemType,  
           ISNULL(s.DelOrColl,N'') AS DelType,  
        ISNULL(s.picklistnumber,N'')AS PickListNumber,  
        ISNULL(l.contractno,'') AS contractno,  
     s.dateprinted as DateDNPrinted,
     s.ItemID,l.ParentItemID,s.RetItemID	-- RI
    FROM   Schedule s 
    INNER JOIN Stockinfo si on s.ItemID=si.ID		-- RI
    INNER JOIN lineitem l ON l.acctno = s.acctno  
	LEFT OUTER JOIN Stockinfo sr on s.RetItemID=sr.ID	-- RI
	LEFT OUTER JOIN Stockinfo sip on l.ParentItemID=sip.ID		-- RI
    WHERE  s.BuffNo    = @piBuffNo  
    -- UAT 219 --AND    ISNULL(s.retstocklocn, s.stocklocn) = @piBranchNo  
    AND    @piBranchNo = (CASE WHEN ISNULL(s.retstocklocn,0) = 0 THEN s.stocklocn ELSE s.retstocklocn END)  
    AND    l.itemID = s.itemID  
    AND    l.stocklocn = s.stocklocn  
    AND	   l.ParentItemID = s.ParentItemID
    AND    ((l.quantity != 0) OR (l.quantity = 0 AND s.delorcoll = N'C'))  
    and not exists(select * from branch where l.delnotebranch = branchno and ThirdPartyWarehouse='Y')	-- UAT61 jec
 

    SET @Return = @@ERROR
    
END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End
