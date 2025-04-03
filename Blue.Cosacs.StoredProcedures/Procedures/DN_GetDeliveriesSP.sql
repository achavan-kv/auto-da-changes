SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_GetDeliveriesSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_GetDeliveriesSP]
GO

CREATE PROCEDURE dbo.DN_GetDeliveriesSP
		@acctno varchar(12),
		@return int OUTPUT

AS

	SET @return = 0			--initialise return code

	SELECT 	L.AcctNo,
			L.AgrmtNo,
			--L.ItemNo,
			SI.IUPC as ItemNo,																--IP - 26/07/11 - RI
			L.StockLocn,
			L.Price,
			L.OrdVal,
			D.Quantity
	--FROM 		lineitem L, 
	--		delivery D
	FROM    lineitem L inner join delivery d on D.ItemID 	= L.ItemID						--IP - 26/07/11 - RI
	AND		D.StockLocn 	= L.StockLocn
	AND		D.AgrmtNo 	= L.AgrmtNo
	inner join stockinfo SI on L.ItemID = SI.ID												--IP - 26/07/11 - RI
	WHERE	D.AcctNo	= @acctno
	AND		D.AcctNo 	= L.AcctNo
	--AND		D.ItemNo 	= L.ItemNo
	--AND		D.StockLocn 	= L.StockLocn
	--AND		D.AgrmtNo 	= L.AgrmtNo

	SET @return = @@error
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

