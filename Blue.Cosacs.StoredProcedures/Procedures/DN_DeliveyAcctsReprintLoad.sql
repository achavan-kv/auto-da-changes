SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_DeliveyAcctsReprintLoad]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_DeliveyAcctsReprintLoad]
GO

CREATE procedure   dbo.DN_DeliveyAcctsReprintLoad
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : dbo.DN_DeliveyAcctsReprintLoad
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Load accounts for re-print delivery note
-- Author       : ??
-- Date         : ??
--
-- This procedure will load accounts that require their delivery note to be re-printed.
-- 
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 24/11/09  ip  UAT769 - Two lines were displayed in the Re-print Delivery Note screen
--			     for the same account and delivery note due to the account being joined to 
--				 a spouse. Only select from custacct for the holder of the account.
-- 19/02/10 jec -- 19/02/10 jec CR1072 Malaysia merge -LW71408
-- 07/04/10 jec UAT62 Reprint Delivery Notes screen - reinstate removed code.
-- 21/04/10 jec UAT102 Reprint Collection Note displaying the Stock location incorrectly
-- 06/06/11 ip  CR1212 - RI - Join using ItemID
-- =================================================================================
	-- Add the parameters for the stored procedure here

   			@acctno 	varchar(12),
			@stockLocn 	int,
			@bufffnofrom	int,
			@bufffnoto	int,
   			@return 	int OUTPUT
 
AS
 	SET  @return = 0 --initialise return code

	IF(@acctno = '000000000000')
	BEGIN
		SELECT DISTINCT	l.AcctNo,
					l.datereqdel,
					l.timereqdel,
					s.buffno,
					s.buffbranchno,  --71408 must return both buff branch no and stock locn --IP - 22/02/10 - Undone 71408 - reinstate later -- reinstated jec					
					a.empeenosale,
					c.custid,
					l.deliveryaddress,    -- FR67773 Use the delivery address from NSO
					--s.stockLocn,	--UAT102 jec
					CASE WHEN ISNULL(s.retstocklocn,0) = 0 THEN s.stocklocn ELSE s.retstocklocn END as stocklocn,	--UAT102 jec
					convert(bit,0) as released
		INTO #reprint
		FROM	LINEITEM l
		JOIN SCHEDULE s ON l.acctno = s.acctno 
		AND l.agrmtno   = s.agrmtno
		--AND l.itemno    = s.itemno
		AND l.ItemID = s.ItemID						--IP - 06/06/11 - CR1212 - RI
		AND l.stocklocn = s.stocklocn
		AND s.dateprinted IS NOT NULL
		--JOIN STOCKITEM b ON l.itemno = b.itemno 
		JOIN StockInfo si ON l.ItemID = si.ID		--IP - 06/06/11 - CR1212 - RI
		JOIN StockQuantity sq ON l.ItemID = sq.ID	--IP - 06/06/11 - CR1212 - RI 
		--AND l.stocklocn = b.stocklocn
		AND l.stocklocn = sq.stocklocn				--IP - 06/06/11 - CR1212 - RI
		JOIN custacct c ON l.acctno = c.acctno
		JOIN agreement a ON l.acctno = a.acctno
		WHERE	s.BuffNo BETWEEN @bufffnofrom AND @bufffnoto
		AND 	 	s.stockLocn  = @stockLocn
		AND 	 	l.Iskit	= 0
		AND 		c.hldorjnt = 'H'
        AND NOT EXISTS (SELECT c.AcctNo FROM Cancellation c WHERE c.AcctNo = l.AcctNo)
        and s.delorcoll<>'X' --IP - 18/02/10 - CR1072 - LW 70615 - General Fixes from 4.3 - Merge

		-- FR67773 But default to the home address if the customer delivery address is not set up
		UPDATE #reprint
		SET    deliveryaddress = 'H'
		WHERE  NOT EXISTS (SELECT * FROM CustAddress ca
		                   WHERE ca.CustId = #reprint.CustId
		                   AND   ca.AddType = #reprint.DeliveryAddress
		                   AND   ISNULL(ca.DateMoved,'1-January-1900') = '1-January-1900')

		select @return = @@error
		
		IF @@error = 0	
		BEGIN
			-- Return account and address details for display
			SELECT DISTINCT SUBSTRING(l.acctno,1,3)+N'-'+SUBSTRING(l.acctno,4,4)+N'-'+SUBSTRING(l.acctno,8,4)+N'-'+SUBSTRING(l.acctno,12,1) as acctno,
					l.buffno,  l.buffbranchno,		-- UAT62 jec 07/04/10
					c.firstname, c.name, c.title, cad.cusaddr1, cad.cusaddr2, cad.cusaddr3, cad.cuspocode, l.empeenosale, l.datereqdel, l.timereqdel,
					l.deliveryaddress, l.stocklocn, l.released, c.custid, c.alias
			FROM #reprint l
			JOIN customer c ON l.custid = c.custid
			JOIN custaddress cad ON l.custid = cad.custid AND l.deliveryaddress = cad.addtype
			WHERE ISNULL(cad.datemoved,'1-January-1900') = '1-January-1900'
			ORDER BY c.name

			select @return = @@error
		END
	END
	ELSE	
	BEGIN
		SELECT DISTINCT	l.AcctNo,
					l.datereqdel,
					l.timereqdel,
					s.buffno,
					s.buffbranchno,  --71408 must return both buff branch no and stock locn --IP - 22/02/10 - Undone 71408 - reinstate later -- reinstated jec
					a.empeenosale,
					c.custid,
					l.deliveryaddress,    -- FR67773 Use the delivery address from NSO
					--s.stocklocn,	--UAT102 jec
					CASE WHEN ISNULL(s.retstocklocn,0) = 0 THEN s.stocklocn ELSE s.retstocklocn END as stocklocn,	--UAT102 jec
					convert(bit,0) as released
		INTO #reprint1
		FROM	LINEITEM l
		JOIN SCHEDULE s ON l.acctno = s.acctno 
		AND l.agrmtno   = s.agrmtno
		--AND l.itemno    = s.itemno
		AND l.ItemID = s.ItemID						--IP - 06/06/11 - CR1212 - RI
		AND l.stocklocn = s.stocklocn
		AND s.dateprinted IS NOT NULL
		--JOIN STOCKITEM b ON l.itemno = b.itemno 
		JOIN StockInfo si ON l.ItemID = si.ID		--IP - 06/06/11 - CR1212 - RI
		JOIN StockQuantity sq ON l.ItemID = sq.ID	--IP - 06/06/11 - CR1212 - RI
		AND l.stocklocn = sq.stocklocn				--IP - 06/06/11 - CR1212 - RI
		--AND l.stocklocn = b.stocklocn
		JOIN custacct c ON l.acctno = c.acctno
		JOIN agreement a ON l.acctno = a.acctno
		WHERE	l.AcctNo    = @acctno
				AND l.Iskit	= 0
				AND c.hldorjnt = 'H' --IP - 24/11/09 - UAT5.2 (769)

		-- FR67773 But default to the home address if the customer delivery address is not set up
		UPDATE #reprint1
		SET    deliveryaddress = 'H'
		WHERE  NOT EXISTS (SELECT * FROM CustAddress ca
		                   WHERE ca.CustId = #reprint1.CustId
		                   AND   ca.AddType = #reprint1.DeliveryAddress
		                   AND   ISNULL(ca.DateMoved,'1-January-1900') = '1-January-1900')

		select @return = @@error
		
		IF @@error = 0	
		BEGIN
			-- Return account and address details for display
			SELECT DISTINCT SUBSTRING(l.acctno,1,3)+N'-'+SUBSTRING(l.acctno,4,4)+N'-'+SUBSTRING(l.acctno,8,4)+N'-'+SUBSTRING(l.acctno,12,1) as acctno,
					 l.buffno, l.buffbranchno,		-- UAT62 jec 07/04/10
					 c.firstname, c.name, c.title, cad.cusaddr1, cad.cusaddr2, cad.cusaddr3, cad.cuspocode, l.empeenosale, l.datereqdel, l.timereqdel,
					l.deliveryaddress, l.stocklocn, l.released, c.custid, c.alias
			FROM #reprint1 l
			JOIN customer c ON l.custid = c.custid
			JOIN custaddress cad ON l.custid = cad.custid AND l.deliveryaddress = cad.addtype
			WHERE ISNULL(cad.datemoved,'1-January-1900') = '1-January-1900'
			ORDER BY c.name

			select @return = @@error
		END
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End

