SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_UndeliveredLineItemsGetForAccountSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_UndeliveredLineItemsGetForAccountSP]
GO

CREATE PROCEDURE 	dbo.DN_UndeliveredLineItemsGetForAccountSP
			@acctno varchar(12),
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	L.acctno,
		L.quantity as 'quantity',
		L.ordval as 'price',
		L.itemno as 'itemno',
		isnull(L.datereqdel,  '1/1/1900') as 'DateReqDel',
		L.qtydiff as 'Qtydiff',
		S.itemdescr1 as 'itemdescr1',
		S.taxrate as 'TaxRate',
		D.datedel as 'DateDel'
	FROM LINEITEM L
	LEFT JOIN DELIVERY D ON L.acctno = D.acctno
			     AND L.stocklocn = D.stocklocn
			     AND L.itemno = D.itemno
	LEFT JOIN STOCKITEM S ON L.ItemNo = S.ItemNo
			     AND L.StockLocn = S.StockLocn
	WHERE L.AcctNo    = @acctno
	AND   L.AgrmtNo   = 1
	AND   L.Quantity  > 0
	AND   D.datedel IS NULL

	SET	@return = @@error
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

