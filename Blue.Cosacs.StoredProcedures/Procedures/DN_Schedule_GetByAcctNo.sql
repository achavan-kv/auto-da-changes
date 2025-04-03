SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_Schedule_GetByAcctNo]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_Schedule_GetByAcctNo]
GO

CREATE PROCEDURE DN_Schedule_GetByAcctNo
--------------------------------------------------------------------------------
--
-- Project      : eCoSACS r 2002 Strategic Thought Ltd.
-- File Name    : DN_Schedule_GetByAcctNo.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Load delivery notes for by Account number
-- Author       : D Richardson
-- Date         : 5 November 2002
--
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 18/05/11 jec  CR1212 RI Integration
-- 26/07/11  ip  CR1254 - RI - include ItemID in the select
--------------------------------------------------------------------------------
    -- Parameters
			@acctno varchar(12),
    		@return int OUTPUT

AS
BEGIN

    SET @return = 0;

    SELECT ISNULL(BuffBranchNo,0) AS BuffBranchNo,
           ISNULL(BuffNo,0) AS BuffNo,
           ISNULL(AgrmtNo,0) AS AgrmtNo,
           CASE ISNULL(DelOrColl,'') WHEN 'D' THEN 'Delivery' WHEN 'C' THEN 'Collection' ELSE '' END AS DelOrColl,
           --ISNULL(ItemNo,'') AS ItemNo,
           ISNULL(si.IUPC,'IUPCmissing') AS ItemNo,		-- RI
           ISNULL(Quantity,0) AS Quantity,
           ISNULL(StockLocn,0) AS StockLocn,
           ISNULL(DateDelPlan,'') AS DateDelPlan,
           ISNULL(RetStockLocn,0) AS RetStockLocn,
           --ISNULL(RetItemNo,'') AS RetItemNo,
           ISNULL(sr.IUPC,' ') AS RetItemNo,		-- RI
           ISNULL(RetVal,0) AS RetVal,
           ISNULL(VanNo,'') AS VanNo,
           ISNULL(LoadNo,0) AS LoadNo,
           s.ItemID									--IP - 26/07/11 - RI
    FROM   Schedule s
			INNER JOIN StockInfo si on s.ItemID=si.ID		-- RI
			LEFT OUTER JOIN StockInfo sr on s.RetItemID=sr.ID 
    WHERE  AcctNo       = @acctno

    SET @Return = @@ERROR
    
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
-- End End End End End End End End End End End End End End End End End End End End End End End End End End
