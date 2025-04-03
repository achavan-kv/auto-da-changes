
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[GetLineItemsForAWorklistAccountSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetLineItemsForAWorklistAccountSP]
GO

CREATE PROCEDURE GetLineItemsForAWorklistAccountSP
-- =============================================
-- Author:		Jez Hemans
-- Create date: 01/06/2007
-- Description:	Returns all the line items for a particular account
--
-- Change Control  
-- --------------
-- 29/07/11 jec  CR1212 RI changes
-- =============================================
	@acctno varchar(12),
	@return int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SET 	@return = 0			--initialise return code

	SELECT	L.acctno AS 'AcctNo',
			--L.itemno as 'ItemNo',
			S.IUPC as 'ItemNo',				-- RI
            S.itemdescr1 + ' ' + S.[itemdescr2] as 'Description',
	    CONVERT(DATETIME,CONVERT(VARCHAR(10),datedel,103),103) AS 'Date Delivered',				
            L.quantity as 'quantity',
			L.ordval as 'Value'
	FROM    	LINEITEM L, STOCKITEM S, [delivery] D
	WHERE	L.AcctNo    = @acctno
	AND    		L.AgrmtNo   = 1
	AND		L.Quantity  > 0
	--AND    		L.ItemNo    = S.ItemNo
	AND    		L.ItemId    = S.ItemID					-- RI
	AND    		L.StockLocn = S.StockLocn
    AND  L.[acctno] = D.[acctno]
    --AND  L.[itemno] = D.[itemno]
    AND  L.ItemId = D.ItemId							-- RI
    AND  L.[stocklocn] = D.[stocklocn]
	ORDER BY datedel DESC

	SET	@return = @@error
END
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End