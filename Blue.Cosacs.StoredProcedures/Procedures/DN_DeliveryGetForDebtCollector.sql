SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_DeliveryGetForDebtCollector]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_DeliveryGetForDebtCollector]
GO

CREATE PROCEDURE dbo.DN_DeliveryGetForDebtCollector
-- ===================================================================
-- Author:		?
-- Create date: ?
-- Description:	
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 22/09/11  IP  RI - #8229 - CR8201 - Debt Collection Action Sheet print out - description needs to be: descr+brand+vendor style long
-- ==================================================================
		 @acctno varchar(12),
		 @return int OUTPUT

AS

	SET @return = 0			--initialise return code

	SELECT	G.AcctNo,
			G.AgrmtNo,
			--S.ItemNo,
			isnull(S.IUPC,'') as ItemNo,														--IP - 22/09/11 - RI - #8229 - CR8201
			L.Quantity,
			L.Price,
			L.Ordval,
			L.datereqdel,
			L.dateplandel,
			S.ItemDescr1,
			S.ItemDescr2,
			S.Stocklocn,
			S.ItemType,
			0 as delqty,
			S.ID as ItemID,
			rtrim(ltrim(isnull(S.VendorLongStyle,''))) as Style,								--IP - 22/09/11 - RI - #8229 - CR8201	
			rtrim(ltrim(isnull(S.Brand,''))) as Brand											--IP - 22/09/11 - RI - #8229 - CR8201						
	INTO		#tmpaccts
	FROM		LINEITEM L,
			STOCKITEM S,
			AGREEMENT G
	WHERE 	G.acctno 	= @acctno
	AND 		L.acctno 	= G.acctno
	--AND 		S.Itemno 	= L.ItemNo
	AND 		S.ID 	= L.ItemID																--IP - 22/09/11 - RI - #8229 - CR8201				
	AND 		S.Stocklocn	= L.Stocklocn
	AND 		L.quantity 	!= 0

	--SELECT	isnull(sum(d.quantity),0) as delquantity, d.acctno, d.itemno, d.stocklocn
	SELECT	isnull(sum(d.quantity),0) as delquantity, d.acctno, d.ItemID, d.stocklocn			--IP - 22/09/11 - RI - #8229 - CR8201	
	INTO		#tmpqty
	FROM		DELIVERY d, #tmpaccts t
	WHERE	t.acctno = d.acctno
	--AND 		t.itemno = d.itemno
	AND 		t.ItemID = d.ItemID																--IP - 22/09/11 - RI - #8229 - CR8201																
	AND 		t.stocklocn = d.stocklocn
	--GROUP BY 	d.acctno, d.itemno, d.stocklocn
	GROUP BY 	d.acctno, d.ItemID, d.stocklocn													--IP - 22/09/11 - RI - #8229 - CR8201	

	IF (@@error != 0)
	BEGIN
		DROP TABLE #tmpaccts
		SET @return = @@error
	END


	UPDATE	#tmpaccts
	SET		delqty = delquantity
	FROM		#tmpqty 
	WHERE	#tmpaccts.acctno = #tmpqty.acctno
	--AND 		#tmpaccts.itemno = #tmpqty.itemno
	AND 		#tmpaccts.ItemID = #tmpqty.ItemID												--IP - 22/09/11 - RI - #8229 - CR8201	
	AND 		#tmpaccts.stocklocn = #tmpqty.stocklocn

	IF (@@error != 0)
	BEGIN
		DROP TABLE #tmpaccts
		DROP TABLE #tmpqty
		SET @return = @@error
	END

	DELETE
	FROM #tmpaccts
	WHERE delqty <= 0 AND ItemNo <> 'LOAN' --CR906 Loan Item to appear on bailiff action sheet


	IF (@@error != 0)
	BEGIN
		DROP TABLE #tmpaccts
		DROP TABLE #tmpqty
		SET @return = @@error
	END

	DELETE
	FROM #tmpaccts
	WHERE ItemType = 'N' AND ItemNo <> 'LOAN' --CR906 Loan Item to appear on bailiff action sheet


	IF (@@error != 0)
	BEGIN
		DROP TABLE #tmpaccts
		DROP TABLE #tmpqty
		SET @return = @@error
	END

	SELECT *
	FROM #tmpaccts

	DROP TABLE #tmpaccts
	DROP TABLE #tmpqty

	SET @return = @@error
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

