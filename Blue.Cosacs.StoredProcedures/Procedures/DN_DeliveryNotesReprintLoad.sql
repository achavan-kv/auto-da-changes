SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_DeliveryNotesReprintLoad]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_DeliveryNotesReprintLoad]
GO
-- =============================================
-- Author:		Jez Hemans
-- Create date: 31/10/2007
-- Description:	Replaces the stored procedure DN_DeliveyNotesReprintLoad and includes the retval field for UAT 365
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 19/02/10 jec CR1072 Malaysia merge -LW71408
-- 22/04/10 jec UAT102 
-- 07/06/11 ip  CR1212 - RI - use ItemID
-- 21/09/11 ip  RI - #8225 - CR8201 - Reprint Delivery Note printout - description needs to be: descr+brand+vendor style long
-- =============================================
CREATE procedure   dbo.DN_DeliveryNotesReprintLoad
   			@acctno 	varchar(12),
			@stocklocn 	int,
			--@buffbranchno int,		--CR1072 Malaysia merge -LW71408 --IP - 22/02/10 - Undone 71408 - reinstate later
			@buffno	int,
   			@return 	int OUTPUT
 
AS
 	SET  @return = 0 --initialise return codeSELECT	A.AcctNo,

	SELECT DISTINCT	
				l.acctno as acctno,
				--l.itemno,														--IP - CR1212 - RI 
				b.IUPC as itemno,												--IP - CR1212 - RI 
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
				--ISNULL(s.retitemno, '') as retitemno,
				si.IUPC as retitemno,
				b.supplier,		-- IP - 09/02/10 - CR1048 (Ref:3.1.3) Merged - Malaysia Enhancements (CR1072)
				l.PrintOrder,	-- IP - 09/02/10 - CR1048 (Ref:3.1.3) Merged - Malaysia Enhancements (CR1072)
				ISNULL(s.Grtnotes,'') AS grtnotes, --UAT 158 V4.4 )
                s.retval, -- UAT 365  JH 31/10/2007
                s.buffbranchno AS origBuffBranchNo, -- UAT(5.2) - 568	
                b.ID as ItemID,													--IP - CR1212 - RI 
                si.ID as RetItemID,												--IP - CR1212 - RI 
                rtrim(ltrim(isnull(b.VendorLongStyle,''))) as Style,			--IP - RI - #8225 - CR8201
                rtrim(ltrim(isnull(b.Brand,''))) as Brand						--IP - RI - #8225 - CR8201				
	FROM	LINEITEM l
	JOIN SCHEDULE s ON l.acctno = s.acctno 
	AND l.agrmtno   = s.agrmtno
	--AND l.itemno    = s.itemno
	AND l.ItemID = s.ItemID														--IP - CR1212 - RI 
	AND l.stocklocn = s.stocklocn
	--JOIN STOCKITEM b ON l.itemno = b.itemno 
	--AND l.stocklocn = b.stocklocn
	JOIN StockInfo b ON l.ItemID = b.ID											--IP - CR1212 - RI 
	JOIN StockQuantity sq ON l.ItemID = sq.ID AND l.stocklocn = sq.stocklocn	--IP - CR1212 - RI 
	LEFT JOIN StockInfo si ON s.RetItemID = si.ID								--IP - CR1212 - RI							
	JOIN custacct c ON l.acctno = c.acctno
	JOIN agreement a ON l.acctno = a.acctno
	WHERE	l.AcctNo = @acctno
	AND	    s.BuffNo = @buffno
	AND 	case when ISNULL(s.retstocklocn,0)=0 then s.stocklocn else s.retstocklocn end = @stocklocn	-- UAT102
	--AND		s.buffbranchno = @buffbranchno	--CR1072 Malaysia merge -LW71408 --IP - 22/02/10 - Undone 71408 - reinstate later
	AND 	l.Iskit = 0
    AND NOT EXISTS (SELECT c.AcctNo FROM Cancellation c WHERE c.AcctNo = l.AcctNo)
    ORDER BY l.PrintOrder	-- IP - 09/02/10 - CR1048 (Ref:3.1.3) Merged - Malaysia Enhancements (CR1072)


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End