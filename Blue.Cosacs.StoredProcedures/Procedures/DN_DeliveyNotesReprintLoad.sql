SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_DeliveyNotesReprintLoad]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_DeliveyNotesReprintLoad]
GO

CREATE procedure   dbo.DN_DeliveyNotesReprintLoad
   			@acctno 	varchar(12),
			@stocklocn 	int,
			@buffno	int,
   			@return 	int OUTPUT
 
AS
 	SET  @return = 0 --initialise return codeSELECT	A.AcctNo,

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
				ISNULL(s.retitemno, '') as retitemno,
				b.supplier	-- IP - 09/02/10 - CR1048 (Ref:3.1.2) Merged - Malaysia Enhancements (CR1072) 
	FROM	LINEITEM l
	JOIN SCHEDULE s ON l.acctno = s.acctno 
	AND l.agrmtno   = s.agrmtno
	AND l.itemno    = s.itemno
	AND l.stocklocn = s.stocklocn
	JOIN STOCKITEM b ON l.itemno = b.itemno 
	AND l.stocklocn = b.stocklocn
	JOIN custacct c ON l.acctno = c.acctno
	JOIN agreement a ON l.acctno = a.acctno
	WHERE	l.AcctNo = @acctno
	AND	s.BuffNo = @buffno
	AND 	s.stocklocn  = @stocklocn
	AND 	l.Iskit = 0
        AND NOT EXISTS (SELECT c.AcctNo FROM Cancellation c WHERE c.AcctNo = l.AcctNo)

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

