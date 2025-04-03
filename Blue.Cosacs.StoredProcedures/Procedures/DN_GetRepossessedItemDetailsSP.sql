SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_GetRepossessedItemDetailsSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_GetRepossessedItemDetailsSP]
GO

CREATE PROCEDURE 	dbo.DN_GetRepossessedItemDetailsSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_GetRepossessedItemDetailsSP.PRC
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : 
-- Author       : ??
-- Date         : ??
-- 
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 26/05/11  IP  CR1212 - RI - #3636 - Redelivery after Repossession (Standardisation of Courts SKU - New product codes)
-- 07/03/12  IP  #8058 - LW73924 - Qty incorrect for multiple items with the same RetItemID. Included ItemID in grouping
--								 when calculating qty.
-- 16/03/12  IP  #9799 - Unable to reposess all items on account. The same Return Item No was given to two different items.
--				 Problem ocurred when marking only 1 of the items for delivery and then attempting to reload the account.
--				 Included join on ItemID
-- 12/06/12  IP  #10357 - Warehouse & Deliveries - Returning the Lineitem.ID
-- 14/06/12  IP  #10387 - Warehouse & Deliveries - Do not return an item that has been marked for re-delivery previously. Remove duplicate rows.
-- 18/06/12  IP/JC  #10409 - Warehouse & Deliveries 
-- 16/08/12  IP  #10718 - added missing join on acctno
-- 23/01/13 jec #10686 Repossessed item does not appear on screen.
-- 24/06/13  IP #13926 - Blank delivery area sent from re-delivery after repossession screen
-- 04/08/14  IP #19627 - Installations should not be displayed in re-delivery after repossession screen
-- ================================================
@acctno varchar(12),
		@return int OUTPUT

AS

	DECLARE	@custid varchar(20)
	SELECT	@custid = custid 
	FROM		custacct 
	WHERE	acctno = @acctno
	AND		hldorjnt = 'H'

	SET 	@return = 0			--initialise return code

	CREATE TABLE  #repossesseditems ( acctno varchar(12),
					  agrmtno int,
					  datedel datetime, 
					  delorcoll	char, 
					  --itemno varchar (8), 
					  itemno varchar(18),				--IP - 26/05/11 - CR1212 - RI - #3636
					  quantity int, 
					  --retitemno	varchar (8), 
					  retitemno varchar(18),			--IP - 26/05/11 - CR1212 - RI - #3636
					  retItemID int,					--IP - 26/05/11 - CR1212 - RI - #3636		
					  retstocklocn smallint, 
					  stocklocn	smallint, 
					  retval money, 
					  buffno int, 
					  buffbranchno smallint, 
					  itemdescr1 varchar(32), 
					  itemdescr2 varchar(40), 
					  hpvalue money, 
					  cashvalue	money, 
					  datereqdel datetime, 
					  timereqdel varchar (12),
					  contractno varchar(10),
					  --parentitemno varchar(8)
					  parentitemno varchar(18),			--IP - 26/05/11 - CR1212 - RI - #3636
					  itemID int,						--IP - 26/05/11 - CR1212 - RI - #3636
					  parentItemID int,					--IP - 26/05/11 - CR1212 - RI - #3636
					  lineItemId int,					--IP - 12/06/12 - #10357	
					  DeliveryArea varchar(8),							--#13926
					  DeliveryAddress Char(1)							--#14927
					  ) --IP - 20/10/08 - UAT(5.2) - UAT(549)
	
	insert into #repossesseditems ( acctno ,
					  agrmtno ,
					  datedel, 
					  delorcoll, 
					  itemno, 
					  quantity, 
					  retitemno, 
					  retItemID,						--IP - 26/05/11 - CR1212 - RI - #3636	
					  retstocklocn, 
					  stocklocn, 
					  retval, 
					  buffno, 
					  buffbranchno, 
					  itemdescr1, 
					  itemdescr2, 
					  hpvalue, 
					  cashvalue, 
					  datereqdel, 
					  timereqdel,
					  contractno,
					  parentitemno,						--IP - 20/10/08 - UAT(5.2) - UAT(549)
					  itemID,							--IP - 26/05/11 - CR1212 - RI - #3636	
					  parentItemID,						--IP - 26/05/11 - CR1212 - RI - #3636	
					  lineItemId,						--IP - 12/06/12 - #10357
					  DeliveryArea,								--#13926
					  DeliveryAddress)							--#14927
	
	SELECT  D.AcctNo    acctno, 
		    D.AgrmtNo   agrmtno, 
		    D.DateDel   datedel, 
		    D.DelOrColl delorcoll, 
		    --D.ItemNo    itemno,						
		    S.IUPC		itemno,							--IP - 26/05/11 - CR1212 - RI - #3636	
		    D.Quantity  quantity, 
		    --D.RetItemNo retitemno, 
		    isnull(S1.IUPC,'')	retitemno,				--IP - 26/05/11 - CR1212 - RI - #3636
		    D.RetItemID	retItemID,						--IP - 26/05/11 - CR1212 - RI - #3636					
		    D.RetStockLocn retstocklocn, 
		    D.stocklocn stocklocn, 
		    D.RetVal    retval, 
		    D.BuffNo    buffno, 
		    D.BuffBranchNo buffbranchno, 
		    S.ItemDescr1   itemdescr1, 
		    S.ItemDescr2   itemdescr2, 
		    --S.UnitPriceHP  unitpricehp, 
		    SP.CreditPrice	 unitpricehp,		--IP - 26/05/11 - CR1212 - RI - #3636	
		    --S.UnitPriceCash  unitpricecash,
		    SP.CashPrice   unitpricecash,		--IP - 26/05/11 - CR1212 - RI - #3636	
		    ISNULL(L.DateReqDel, '') datereqdel, 
		    L.TimeReqDel   timereqdel, 
		    L.Contractno   contractno,
			--L.Parentitemno parentitemno		--IP - 20/10/08 - UAT(5.2) - UAT(549)
			isnull(S2.IUPC,'')	parentitemno,			--IP - 26/05/11 - CR1212 - RI - #3636	
			D.ItemID	itemID,					--IP - 26/05/11 - CR1212 - RI - #3636	
			D.ParentItemID	parentItemID,		--IP - 26/05/11 - CR1212 - RI - #3636
			L.ID lineItemId,					--IP - 12/06/12 - #10357 	
			L.deliveryarea,						--#13926
			L.deliveryaddress					--#14927
	 	 
		FROM Delivery D INNER JOIN Lineitem L ON D.ItemID = L.ItemID
		AND D.Acctno=L.acctno
		AND D.ParentItemID = L.ParentItemID
		AND D.StockLocn=L.StockLocn
		INNER JOIN StockInfo S ON D.ItemID = S.ID
		INNER JOIN StockQuantity SQ ON D.ItemID = SQ.ID AND D.StockLocn = SQ.StockLocn
		INNER JOIN StockPrice SP ON D.ItemID = SP.ID AND D.StockLocn = SP.BranchNo
		LEFT JOIN StockInfo S1 ON D.RetItemID = S1.ID		--Join to get the retitemno
		LEFT JOIN StockInfo S2 ON L.ParentItemID = S2.ID    --Join to get the parentitemno
		WHERE D.AcctNo = @acctno
		AND D.DelorColl='R'
		AND CONVERT(VARCHAR, S.category) NOT IN (SELECT code FROM dbo.Code WHERE category = 'WAR')
		AND s.iupc NOT IN (SELECT code FROM Code WHERE category = 'INST')							-- #19627
		and d.datetrans=(select MAX(datetrans) from delivery d2 where D2.Acctno=L.acctno			-- #10686 jec
				and D2.ItemID = L.ItemID AND D2.ParentItemID = L.ParentItemID AND D2.StockLocn=L.StockLocn and d2.DelorColl='R')
	 
	 --IP - 18/06/12 - #10409 - Update quantity to include those that have been booked.
	-- UPDATE #repossesseditems
	-- SET quantity = r.quantity + (isnull((select sum(lb.quantity) from LineItemBooking lb			
	--					where lb.LineItemId = ls.LineItemId
	--					and ls.BookingId = lb.ID),0))					
	--FROM #repossesseditems r
	--LEFT JOIN LineItemBookingSchedule ls on r.LineItemId = ls.LineItemId and ls.delorcoll = 'R'	
	
	-- #10686 jec - Update quantity to include those that have been scheduled.
	UPDATE #repossesseditems						
	 SET quantity = r.quantity + (isnull((select sum(ls.quantity) from LineItemBookingSchedule ls			
						where ls.LineItemId = r.LineItemId
						),0))					
	FROM #repossesseditems r
	LEFT JOIN LineItemBookingSchedule ls on r.LineItemId = ls.LineItemId and ls.delorcoll = 'R'	

	DELETE  #repossesseditems 
	  --FROM (SELECT S.ItemID, S.RetStockLocn, S.RetItemID, sum(S.Quantity) quantity			--IP - 16/03/12 - #9799 - added ItemID --IP - 26/05/11 - CR1212 - RI - #3636
	  FROM (SELECT S.ItemID, S.StockLocn,  sum(S.Quantity) quantity		-- #10686 jec
	          FROM schedule S, #repossesseditems R
             WHERE S.AcctNo		= R.acctno
    		   and S.AgrmtNo	= R.AgrmtNo
    		   and s.ItemID		= R.ItemID													--IP - 16/03/12 - #9799 - added ItemID
    	   	   --and S.RetItemNo	= R.RetItemNo
    	   	   and S.RetItemID  = R.RetItemID			--IP - 26/05/11 - CR1212 - RI - #3636
		   and S.RetStockLocn 	= R.RetStockLocn
	 --     GROUP BY S.ItemID, S.RetItemID,S.RetStockLocn ) AS tmp , #repossesseditems R1		--IP - 16/03/12 - #9799 - added ItemID	--IP - 26/05/11 - CR1212 - RI - #3636
	 --WHERE (R1.RetStockLocn = tmp.RetStockLocn and R1.RetItemID = tmp.RetItemID and R1.ItemID = tmp.ItemID) --IP - 16/03/12 - #9799 - added ItemID	--IP - 26/05/11 - CR1212 - RI - #3636 
	GROUP BY S.ItemID, S.StockLocn ) AS tmp , #repossesseditems R1		-- #10686 jec
	 WHERE (R1.StockLocn = tmp.StockLocn and R1.ItemID = tmp.ItemID)	-- #10686 jec


	--IP - 15/06/12 - #10387 - Delete duplicate rows.
	----DELETE FROM #repossesseditems		-- #10686 jec not required now
	----FROM #repossesseditems r 
	------where r.buffno != (select max(d2.buffno) from delivery d2
	----where r.datedel != (select max(d2.datedel) from delivery d2
	----					where d2.ItemID = r.ItemID
	----					and d2.StockLocn = r.StockLocn
	----					AND d2.acctno = r.acctno)		--# 10718
	
		--IP/JC - 18/06/12 - #10387
	--Delete an item which has been booked more than repossessed indicating it is not awaiting redelivery
	--DELETE #repossesseditems		-- #10686 jec not required now
	--FROM  #repossesseditems r
	--LEFT JOIN LineItemBookingSchedule ls on r.LineItemId = ls.LineItemId and ls.delorcoll = 'R'
	--WHERE (
	--			isnull((select sum(d1.quantity) from delivery d1		--Redeliveries
	--			join #repossesseditems r2 on 
	--					d1.acctno = r2.acctno
	--					and d1.agrmtno = r2.agrmtno
	--					and d1.ItemID = r2.ItemID 
	--					and d1.stocklocn = r2.stocklocn
	--					--and d1.buffno = r2.buffno
	--					where isnull(d1.quantity,0) > 0
	--					and isnull(d1.delorcoll,'R') = 'R'
	--					),0)
	--			 + 
				
	--			isnull((select sum(d2.quantity) from delivery d2		--Reposession
	--			join #repossesseditems r3 on 
	--					d2.acctno = r3.acctno
	--					and d2.agrmtno = r3.agrmtno
	--					and d2.ItemID = r3.ItemID 
	--					and d2.stocklocn = r3.stocklocn
	--					--and d2.buffno = r3.buffno
	--					where isnull(d2.quantity,0) < 0
	--					and isnull(d2.delorcoll,'R') = 'R'
	--					),0)
	--			+
				
	--			isnull((select sum(lb.quantity) from LineItemBooking lb		-- Rebooked		
	--					where lb.LineItemId = ls.LineItemId
	--					--and ls.BookingId = lb.ID
	--					),0)
						  
	--	   ) >= 0

		   
	DELETE FROM  #repossesseditems WHERE #repossesseditems.Quantity >= 0

	SELECT  acctno ,
		    itemno, 
		    retitemno,
		    retItemID,		--IP - 26/05/11 - CR1212 - RI - #3636	 
		    retstocklocn,
		    datedel, 
      		quantity, 
		    itemdescr1, 
		    itemdescr2,
		    agrmtno , 
		    delorcoll,  
		    stocklocn, 
		    retval, 
		    buffno, 
		    buffbranchno, 
		    hpvalue, 
		    cashvalue, 
		    datereqdel, 
		    timereqdel,
		    contractno,
			parentitemno, --IP - 20/10/08 - UAT(5.2) - UAT(549)
			itemID,		  --IP - 26/05/11 - CR1212 - RI - #3636
			parentItemID, --IP - 26/05/11 - CR1212 - RI - #3636
			lineItemId,	  --IP - 12/06/12 - #10357
			DeliveryArea,		  --#13926
			DeliveryAddress							--#14927
	  FROM #repossesseditems

	-- get address types
	  select Addtype from custaddress ca 
			where custid=@custid
				and datemoved is null


	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END




GO

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End
