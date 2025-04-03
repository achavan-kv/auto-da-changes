SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_LineItemsGetForAccountSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_LineItemsGetForAccountSP]
GO


CREATE PROCEDURE 	dbo.DN_LineItemsGetForAccountSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_LineItemsGetForAccountSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Get Lineitems details for an Account
-- Author       : ??
-- Date         : ??
--
-- This procedure will retreive the Lineitem details.
-- 
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 21/05/09  jec UAT599 return ParentItemNo. 
-- 26/07/11  ip  RI - System Integration changes.
-- =================================================================================
	-- Add the parameters for the stored procedure here
			@acctno varchar(12),
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	L.acctno,
			L.quantity as 'quantity',
			L.ordval as 'price',
			--L.itemno as 'itemno',
			s.iupc as 'itemno',												--IP - 26/07/11 - RI
			isnull(L.datereqdel,  '1/1/1900') as 'DateReqDel',
			L.qtydiff as 'Qtydiff',
			S.itemdescr1 as 'itemdescr1',
			S.taxrate as 'TaxRate',
			S.ItemType,	
			L.StockLocn,
			L.TaxAmt,
			L.DelQty,
			L.ContractNo,
			L.Agrmtno,
			L.DeliveryArea,
			L.DeliveryProcess,
			--L.parentitemno,				-- UAT599  jec 21/05/09
			isnull(SI.IUPC,'') as parentitemno,	    --IP - 26/07/11 - RI				-- UAT599  jec 21/05/09
			L.ItemId,
			L.ParentItemID
	--FROM    	LINEITEM L, STOCKITEM S
	FROM    	LINEITEM L INNER JOIN STOCKITEM S ON L.ItemId    = S.ItemId	--IP - 26/07/11 - RI
	AND			L.StockLocn = S.StockLocn
	LEFT JOIN   STOCKINFO SI ON L.ParentItemID = SI.ID						--IP - 26/07/11 - RI
	WHERE		L.AcctNo    = @acctno
	AND    		L.AgrmtNo   = 1
	AND			L.Quantity  > 0
	--AND    		L.ItemId    = S.ItemId
	--AND    		L.StockLocn = S.StockLocn

	SET	@return = @@error
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End

