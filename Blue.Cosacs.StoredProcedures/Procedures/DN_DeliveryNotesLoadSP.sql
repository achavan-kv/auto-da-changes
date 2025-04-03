SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_DeliveryNotesLoadSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_DeliveryNotesLoadSP]
GO


CREATE PROCEDURE [dbo].[DN_DeliveryNotesLoadSP] 
-- ================================================  
-- Project      : CoSACS .NET  
-- File Name    : DN_DeliveryNotesLoadSP.prc  
-- File Type    : MSSQL Server Stored Procedure Script  
-- Title        : Delivery Notes Load  
-- Author       : ??  
-- Date         : ??  
--  
-- This procedure will process Load the Delivery notes   
--  
-- Change Control  
-- --------------  
-- Date      By  Description  
-- ----      --  -----------  
-- 09/02/10  IP  CR1048 (Ref:3.1.2) Merged - Malaysia Enhancements (CR1072)
-- 04/11/2008 AA 69770 Allowing redeliveries after repos to be printed. --IP - 19/02/10 - CR1072 - LW 69770 - Printing Fixes from 4.3 - Merge
-- 17/05/11  jec CR1212 RI Integration
-- ================================================  
	@acctno CHAR(12), 
	@user INT, 
	@branch INT, 
	@addr1 VARCHAR(50), 
	@datereqdel DATETIME,
	@addtype CHAR(2),
	@timereqdel VARCHAR(12), 
	@locn INT,
	@buffno INT OUTPUT, 
	@return INT OUTPUT 
AS
	
	SELECT @return = 0

	SET ROWCOUNT 250

	-- Need an account number
	IF (LEN(@acctno) != 12)
		SELECT @return = -1

	-- Need addr1
	IF (LEN(ISNULL(@addr1,'')) = 0)
		SELECT @return = -1

	IF (@return = 0)
	BEGIN	
		-- Fetch address type from lineitem table
		--SELECT DISTINCT al.acctno, ca.custid, l.itemno, l.quantity,
		SELECT DISTINCT al.acctno, ca.custid, s.IUPC as ItemNo, l.quantity,			-- RI
		       l.delqty, l.notes as itemnotes, l.stocklocn, l.price, l.ordval,
		       l.datereqdel, l.timereqdel, l.dateplandel, s.itemdescr1, s.itemdescr2, l.deliveryaddress, l.agrmtno,
		       s.supplier, l.printorder,		--IP - 10/02/10 - CR1048 (Ref:3.1.3) UAT(108) Merged - Malaysia Enhancements (CR1072)
		       '        ' AS RetItemno, --IP - 19/02/10 - CR1072 - LW 69770 - Printing Fixes from 4.3 - Merge
		       l.ItemID, 0 as RetItemID		-- RI
		INTO #lines
		FROM accountlocking al
		JOIN lineitem l ON al.acctno = l.acctno 
		JOIN custacct ca ON l.acctno = ca.acctno
		--JOIN stockitem s ON l.itemno = s.itemno AND l.stocklocn = s.stocklocn
		JOIN stockitem s ON l.itemID = s.itemID AND l.stocklocn = s.stocklocn		-- RI
		WHERE al.acctno = @acctno
		AND al.lockedby = @user
		AND al.lockcount > 0
		AND l.quantity > 0
		AND s.itemtype != 'N'
		AND l.iskit = 0
		AND l.qtydiff = 'Y'
		AND l.datereqdel = @datereqdel	
		AND l.deliveryaddress = @addtype
		AND l.timereqdel = @timereqdel	
		AND l.DelNoteBranch = @branch 
		AND l.StockLocn = @locn
		AND l.quantity !=l.delqty --IP - 19/02/10 - CR1072 - LW 69770 - Printing Fixes from 4.3 - Merge
		SELECT @return = @@error
	END
	
	--IP - 19/02/10 - CR1072 - LW 69770 - Printing Fixes from 4.3 - Merge
	IF @return = 0
	BEGIN
		UPDATE #lines SET delorcoll = s.delorcoll , --retitemno = s.retitemno
				RetItemID=S.ItemID
		FROM schedule s WHERE s.acctno = #lines.acctno AND 
		--s.itemno= #lines.itemno AND 
		s.itemid=#lines.ItemID and		-- RI
		(s.stocklocn = #lines.stocklocn OR s.retstocklocn=#lines.stocklocn)
	END
	
	IF @return = 0	
	BEGIN
		-- Return non-display details required for printing
		SELECT 
		DISTINCT 	l.acctno as acctno, 
				l.itemno as itemno, 
				l.quantity as quantity, 
				l.delqty as delqty, 
				l.itemnotes as itemnotes, 
				l.stocklocn as stocklocn, 
				l.price as price, 
				l.ordval as ordval, 
				l.datereqdel as datereqdel, 
				l.timereqdel as timereqdel, 
				l.dateplandel as dateplandel,
				l.itemdescr1 as itemdescr1, 
				l.itemdescr2 as itemdescr2,
				l.deliveryaddress as deliveryaddress,
				l.agrmtno,
				l.supplier,		-- CR1048  
				l.printorder,  --IP - 10/02/10 - CR1048 (Ref:3.1.3) UAT(108) Merged - Malaysia Enhancements (CR1072)
				l.RetItemno --IP - 19/02/10 - CR1072 - LW 69770 - Printing Fixes from 4.3 - Merge
		FROM #lines l
		JOIN customer c ON l.custid = c.custid
		JOIN custaddress cad ON l.custid = cad.custid -- AND l.addtype = cad.addtype
		WHERE cad.cusaddr1 = @addr1
		AND l.quantity !=l.delqty	--IP - 19/02/10 - CR1072 - LW 69770 - Printing Fixes from 4.3 - Merge
		ORDER BY l.acctno, l.printorder --IP - 10/02/10 - CR1048 (Ref:3.1.3) UAT(108) Merged - Malaysia Enhancements (CR1072)
		SELECT @return = @@error
	END

	--IF @return = 0
	IF @return = 0 AND @buffno = 0 --IP - 19/02/10 - CR1072 - LW 69770 - Printing Fixes from 4.3 - Merge
	BEGIN
		EXEC DN_BranchGetBuffNoSP @branch, @buffno OUTPUT, @return OUTPUT
		--PRINT 'got buffno '
	END

	-- To be added when DN_ScheduleAddDeliverySP has been tested
	--IF @return = 0
	--	EXEC DN_ScheduleAddDeliverySP ....... @return OUTPUT
	--	except you can't run it from here because it needs to run for each record in the recordset that this returns. - JJ

	SET ROWCOUNT 0
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

