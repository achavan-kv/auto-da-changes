/*

	Who		When		What
	------- ----------- -----------------------------------------------------------------
	GAJ		10/08/2005  Need to allow for null values when retrieving DeliveryArea and
	                    DeliveryProcess.
*/
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_ItemGetSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_ItemGetSP]
GO


CREATE PROCEDURE 	dbo.DN_ItemGetSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_ItemGetSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Get Lineitem details  
-- Author       : ??
-- Date         : ??
--
-- This procedure will retrieve the Lineitem details.
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 18/05/10 jec UAT160 EW lineitem not updated after cancel reversed#
-- 17/05/11 ip  RI Integration changes - CR1212 - #3627 - Changed join between lineitem, stockitem table to now use ItemID and ParentItemID
--				 rather than itemno and parentitemno. 
-- 15/10/12 ip  #10609 - Join on ParentItemId required.
-- ================================================
	-- Add the parameters for the stored procedure here
			@acctno varchar(12),
			--@itemNo varchar(8),
			@itemID int,						--IP - 17/05/11 - CR1212 - #3627
			@locn smallint,
			@agrmtno int,
			@contractno varchar(10),
			--@parentItemNo VARCHAR(8),
			@parentItemID int,					--IP - 17/05/11 - CR1212 - #3627
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code
	SELECT	L.AcctNo,
			L.AgrmtNo,
			--L.ItemNo,
			S.IUPC as ItemNo,					--IP - 17/05/11 - CR1212 - #3627
			L.ItemID,							--IP - 17/05/11 - CR1212 - #3627
			L.Quantity,
			L.DelQty,
			L.StockLocn,
			L.ItemSuppText,
			L.ItemSuppText,
			L.Price,
			L.TaxAmt,
			L.OrdVal,
			L.DateReqDel,
			L.TimeReqDel,
			ISNULL(L.DatePlanDel, '1/1/1900') AS DatePlanDel, --IP - 12/04/10 - UAT(66) UAT5.2
			isnull(L.delnotebranch,L.stocklocn),
			S.ItemType,    
			L.Qtydiff,
			S.Category,
			L.ContractNo,
			isnull(L.expectedreturndate, '1/1/1900') as expectedreturndate,
			L.ParentItemNo,
			L.ParentItemID,						--IP - 17/05/11 - CR1212 - #3627
			L.ParentLocation,
			isnull(L.DeliveryArea,'') as DeliveryArea,
			isnull(L.DeliveryProcess,'') as DeliveryProcess
	FROM    LINEITEM L, STOCKITEM S
	WHERE	L.AcctNo    = @acctno
	AND    	L.AgrmtNo   = @agrmtno
	--AND    	L.ItemNo    = @itemNo
	AND		L.ItemID    = @itemID				--IP - 17/05/11 - CR1212 - #3627
	AND    	L.stocklocn = @locn
	--AND    	L.ItemNo    = S.ItemNo
	AND    	L.ItemID    = S.ItemID				--IP - 17/05/11 - CR1212 - #3627
	AND    	L.StockLocn = S.StockLocn
	AND		L.contractno = @contractno
	--AND		L.parentitemno = @parentItemNo		-- UAT160 do not need parentitem
	AND l.ParentItemID = @parentItemID				-- #10609

	SET	@return = @@error

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End
