SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE Name = 'DN_LineItemsGetForCanxAccountSP' AND Type = 'P')
BEGIN
    DROP PROCEDURE DN_LineItemsGetForCanxAccountSP
END
GO

CREATE PROCEDURE DN_LineItemsGetForCanxAccountSP

--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_LineItemsGetForCanxAccountSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Cancel Account
-- Author       : Rupal Desai
-- Date         : 22 June 2006
--
-- Change Control
-- --------------
-- Date      	By  Description
-- ----      	--  -----------
-- 27/06/06	    RD  Initial creation in order to post cancellation record to fact 68181
-- 15/01/10     IP  UAT(965) - Return the Parentitemno
-- 27/05/11     IP  CR1212 - RI - #3756 - Return ItemID and ParentItemID
--------------------------------------------------------------------------------

    -- Parameters
    			@acctno varchar(12),
				@return int OUTPUT
AS
BEGIN

	SET 	@return = 0			--initialise return code

	SELECT	L.acctno,
			L.quantity as 'quantity',
			L.ordval as 'price',
			SI.IUPC as 'itemno',									--IP - 27/05/11 - CR1212 - RI - #3756		
			L.ItemID,												--IP - 27/05/11 - CR1212 - RI - #3756		
			isnull(L.datereqdel,  '1/1/1900') as 'DateReqDel',
			L.qtydiff as 'Qtydiff',
			SI.itemdescr1 as 'itemdescr1',							--IP - 27/05/11 - CR1212 - RI - #3756
			SI.taxrate as 'TaxRate',								--IP - 27/05/11 - CR1212 - RI - #3756
			SI.ItemType,											--IP - 27/05/11 - CR1212 - RI - #3756							
			L.StockLocn,
			L.TaxAmt,
			L.DelQty,
			L.ContractNo,
			L.Agrmtno,
			L.DeliveryArea,
			L.DeliveryProcess,
			SI1.IUPC as 'Parentitemno', --IP - 15/01/10 - UAT(965)
			L.ParentItemID											--IP - 27/05/11 - CR1212 - RI - #3756	
	FROM    Lineitem L inner join StockInfo SI on L.ItemID = SI.ID
					   inner join StockQuantity SQ on L.ItemID = SQ.ID and L.StockLocn = SQ.StockLocn
					   left join  StockInfo SI1 on L.ParentItemID = SI1.ID
	WHERE	L.AcctNo    = @acctno
	AND    	L.AgrmtNo   = 1
	--AND    	L.ItemNo    = S.ItemNo
	--AND    	L.StockLocn = S.StockLocn;

	SET	@return = @@error

END
GO

GRANT EXECUTE ON DN_LineItemsGetForCanxAccountSP TO PUBLIC
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
