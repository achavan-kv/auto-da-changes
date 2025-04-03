/****** Object:  StoredProcedure [dbo].[DN_CheckRedeliveryAfterRepossessionSP]    Script Date: 06/23/2008 12:03:03 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DN_CheckRedeliveryAfterRepossessionSP]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DN_CheckRedeliveryAfterRepossessionSP]
GO

CREATE PROC DN_CheckRedeliveryAfterRepossessionSP
	@acctno VARCHAR(12),
	@stocklocn INT,
	@return INT out
AS

	SET  @return = 0

	SELECT DISTINCT	
				l.acctno as acctno,
				l.itemno,
				l.stocklocn,
				l.notes as 'itemnotes',
				l.price,
				l.deliveryaddress,
				s.quantity,
				b.itemdescr1,
				b.itemdescr2,
				s.datedelplan,
				l.datereqdel,
				l.timereqdel,
				l.ordval,
				0 as delqty,
				l.agrmtno,
				s.delorcoll,
				s.buffno,
				s.retItemNo,
				b.supplier,		-- CR1048
				l.printorder
	FROM lineitem l 
		INNER JOIN schedule s
			ON l.acctno = s.acctno 
				AND l.agrmtno = s.agrmtno
				AND l.itemno = s.itemno
				AND s.retstocklocn = l.stocklocn
		INNER JOIN STOCKITEM b 
			ON l.itemno = b.itemno 
				AND l.stocklocn = b.stocklocn
				
WHERE l.AcctNo = @acctno
		AND 	s.stocklocn  = @stocklocn
		AND s.delorcoll = 'R'
ORDER BY l.printorder


 IF @@ROWCOUNT > 0
	BEGIN
	UPDATE schedule SET dateprinted = GETDATE()
		WHERE acctno = @acctno AND @stocklocn = @stocklocn
	DELETE FROM lineitemosdelnotes WHERE acctno = @acctno AND @stocklocn = @stocklocn
	END
