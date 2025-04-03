SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_DeliveryGetSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_DeliveryGetSP]
GO


CREATE PROCEDURE dbo.DN_DeliveryGetSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_DeliveryGetSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Get Delivery and Schedule details  
-- Author       : ??
-- Date         : ??
--
-- This procedure will retrieve the Lineitem Details
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 21/04/10 jec UAT114 Include DHL interface date
-- 24/05/10 ip  UAT192 UAT5.2.1.0 Log - Only select where DHLPickingDate is not null, as previously returned duplicate records.
-- 25/05/10 ip  UAT187 UAT5.2.1.0 Log - Removed join on ItemType as not required.
-- 25/05/10 ip  UAT224 UAT5.2.1.0 Log - Added join on buffno as previously returned duplicates.
-- 17/05/11 ip  RI Integration changes - CR1212 - #3627 - Changed joins to use ItemID and ParentItemID rather than ItemNo and ParentItemNo. Returning IUPC for item numbers
-- ================================================
	-- Add the parameters for the stored procedure here
		 @acctno varchar(12),
		 @agreementno int,
		 @return int OUTPUT

AS

	SET @return = 0			--initialise return code
	-- #12863 - Schedule table no longer used for scheduling - remove 
	--SELECT 	--SC.itemno as itemno,
	--		S.IUPC as itemno,								--IP - 17/05/11 - CR1212 - #3627
	--		SC.ItemID as ItemID,							--IP - 17/05/11 - CR1212 - #3627
	--		S.itemdescr1, 
	--		SC.stocklocn,
	--		SC.retstocklocn,  
	--		SC.quantity,
	--		null as datedel,
	--		CONVERT(VARCHAR,SC.datedelplan,103) as ScheduledDate,	-- 68440 RD
	--		L.price * SC.quantity as value,
	--		SC.buffno ,
	--		i.datefinish as 'DHLInterfaceDate',	--UAT114 jec 21/04/10
	--		SC.OrigBuffno,						--IP - 20/04/10 - UAT(107) UAT5.2		
	--		SC.LoadNo,							-- 68440 RD
	--		t.Truckid,
	--		SC.delorcoll,
	--		SC.contractno,
	--		--SC.retitemno,					
	--		SR.IUPC as retitemno,				--IP - 17/05/11 - CR1212 - #3627 
	--		SC.retItemID,						--IP - 17/05/11 - CR1212 - #3627 
	--		--SC.retstocklocn, 
	--		'' AS NotifiedBy,
	--		L.ordval,                            -- 69474 OrdVal field expected in the business layer JH
 --           --L.parentitemno,
 --           SRP.IUPC as parentitemno,			--IP - 17/05/11 - CR1212 - #3627 
 --           L.ParentItemID						--IP - 17/05/11 - CR1212 - #3627
	--FROM	schedule SC 
	--		INNER JOIN stockitem S	--ON SC.itemno = S.itemno
	--								ON SC.ItemID = S.ItemID				--IP - 16/05/11 - CR1212 - #3627
	--								AND SC.stocklocn = S.stocklocn 
	--		LEFT JOIN stockinfo SR	ON SC.retItemID = SR.ID			--IP - 17/05/11 - CR1212 - #3627
	--		INNER JOIN lineitem L	ON SC.acctno = L.acctno
	--								--AND SC.itemno = L.itemno
	--								AND	SC.ItemID = l.ItemID			--IP - 16/05/11 - CR1212 - #3627
	--								AND SC.stocklocn = L.stocklocn
	--								AND SC.contractno = L.contractno
	--		LEFT JOIN stockinfo SRP ON L.ParentItemID = SRP.ID		--IP - 17/05/11 - CR1212 - #3627
	--		LEFT JOIN deliveryload dl ON sc.buffno = dl.buffno
	--								   AND sc.stocklocn = dl.buffbranchno
	--								   AND sc.loadno = dl.loadno
	--		LEFT JOIN transptsched t ON dl.branchno = t.branchno
	--								  AND dl.datedel = t.datedel
	--								  AND dl.loadno	= t.loadno
	--		LEFT OUTER JOIN interfacecontrol i on sc.runNo=i.runno and i.interface = 'LOGEXPORT'	--UAT114 jec 21/04/10
	--WHERE	SC.acctno = @acctno
	--UNION
	SELECT 	--D.itemno as itemno,
			S.IUPC as itemno,						--IP - 17/05/11 - CR1212 - #3627
			D.ItemID as ItemID,						--IP - 17/05/11 - CR1212 - #3627
			S.itemdescr1,
			D.stocklocn,
			D.retstocklocn,
			D.quantity,
			D.datedel as datedel ,
			CONVERT(VARCHAR,null) as ScheduledDate,			-- 68440
			D.transvalue as value,
			D.buffno,
			i.datefinish as 'DHLInterfaceDate',	--UAT114 jec 21/04/10				
			sa.OrigBuffno,					--IP - 20/04/10 - UAT(107) UAT5.2	
			0 as LoadNo,					-- 68440
			'' as Truckid,				-- 68440
			D.delorcoll,
			D.contractno,
			--D.retitemno,
			SR.IUPC as retitemno,			--IP - 17/05/11 - CR1212 - #3627
			D.retItemID,					--IP - 17/05/11 - CR1212 - #3627
			--D.retstocklocn, 
			D.NotifiedBy,
			L.ordval,                          -- 69474 OrdVal field expected in the business layer JH
            --L.parentitemno,
            SRP.IUPC as parentitemno,		--IP - 17/05/11 - CR1212 - #3627
            L.ParentItemID					--IP - 17/05/11 - CR1212 - #3627
	FROM	delivery D 
				inner JOIN stockitem S 
					--on D.itemno = S.itemno and D.stocklocn = S.stocklocn
					on D.ItemID = S.ItemID and D.stocklocn = S.stocklocn									--IP - 16/05/11 - CR1212 - #3627
				INNER JOIN lineitem L 
					--ON S.itemno = L.itemno AND S.itemtype = L.itemtype AND S.stocklocn = L.stocklocn
					--ON S.itemno = L.itemno AND S.stocklocn = L.stocklocn -- IP - 25/05/10 - UAT(187) UAT5.2.1.0 Log - Removed join on ItemType as not required.
					ON S.ItemID = L.ItemID AND S.stocklocn = L.stocklocn -- IP - 25/05/10 - UAT(187) UAT5.2.1.0 Log - Removed join on ItemType as not required.  --IP - 16/05/11 - #3626 - Use ItemID and ParentItemID
					and l.acctno = @acctno and l.agrmtno = @agreementno --69474 - IP - 14/04/08 - Join was incorrect, needed join onto acctno and agrmtno on Lineitem table.
					and l.contractno = d.contractno  -- IP - 21/04/08 - UAT (410, V.5.1) procedure was incorrectly returning a warranty that I had removed and replaced with a different warranty.
					--AND (d.parentItemNo = l.parentItemNo OR l.parentItemNo = '') --IP - 27/05/09 - (69662)
					--AND (d.ParentItemID = l.ParentItemID OR l.ParentItemID = 0) --IP - 27/05/09 - (69662)	--IP - 16/05/11 - CR1212 - #3627 - use ItemID and ParentItemID
				LEFT JOIN stockitem SR ON D.RetItemID = SR.ItemID and D.retstocklocn = SR.Stocklocn			--IP - 17/05/11 - CR1212 - #3627
				LEFT JOIN stockitem SRP ON L.ParentItemID = SRP.ItemID and L.Parentlocation = SRP.stocklocn	--IP - 17/05/11 - CR1212 - #3627
				LEFT JOIN dbo.ScheduleAudit sa --IP - 20/04/10 - UAT(107) UAT5.2	
					ON sa.acctno = d.acctno
					--AND sa.itemno = d.itemno
					AND sa.ItemID = d.ItemID		--IP - 16/05/11 - CR1212 - #3627
					AND sa.stocklocn = d.stocklocn
					and sa.delorcoll=d.delorcoll			--UAT114 jec 21/04/10
					AND sa.DHLPickingDate IS NOT NULL		--IP - 24/05/10 - UAT(192) 
					AND sa.buffno = d.buffno				--IP - 25/05/10 - UAT(224) UAT5.2.1.0 Log
				LEFT OUTER JOIN interfacecontrol i on sa.runNo=i.runno and i.interface = 'LOGEXPORT'	--UAT114 jec 21/04/10
									
				
	WHERE	D.acctno = @acctno
	AND		D.agrmtno = @agreementno
	--ORDER BY	itemno
	ORDER BY ItemID			--IP - 17/05/11 - CR1212 - #3627

	SET @return = @@error
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End
