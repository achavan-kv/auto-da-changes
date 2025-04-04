-----------------*******************************************************-----------------
--CR2018-013 This trigger will insert in Lineitemaudit table
--for the warranty which is returned along with stock item in case of
--GRTCancel or GRTExchange.
-----------------*******************************************************-----------------

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM SYSOBJECTS 
           WHERE NAME = 'trig_WarrantyRemoval'
           AND xtype = 'TR')
BEGIN 
DROP trigger trig_WarrantyRemoval
END
GO

CREATE TRIGGER trig_WarrantyRemoval on [dbo].[lineitem]
FOR UPDATE 
AS 
   
	SELECT  origbr, acctno, agrmtno, itemno, itemsupptext, quantity, delqty, stocklocn, price, ordval, datereqdel, 
				timereqdel, dateplandel, delnotebranch, qtydiff, itemtype, notes, taxamt, isKit, deliveryaddress, parentitemno, parentlocation, 
				contractno, expectedreturndate, deliveryprocess, deliveryarea, DeliveryPrinted, assemblyrequired, damaged, OrderNo, Orderlineno, 
				PrintOrder, taxrate, ItemID, ParentItemID, SalesBrnNo,Express, WarrantyGroupId	--#15639 -- #10229
	INTO #insert FROM inserted            
	          
	CREATE CLUSTERED INDEX ix_insert_lineitem2324 ON #insert (acctno,itemno,agrmtno, stocklocn,contractno )          
	          
	CREATE INDEX ix_insertStock ON #insert(itemno,stocklocn, itemtype , qtydiff)          	          	

	 SELECT origbr, acctno, agrmtno, itemno, itemsupptext, quantity, delqty, stocklocn, price, ordval, datereqdel, 
				timereqdel, dateplandel, delnotebranch, qtydiff, itemtype, notes, taxamt, isKit, deliveryaddress, parentitemno, parentlocation, 
				contractno, expectedreturndate, deliveryprocess, deliveryarea, DeliveryPrinted, assemblyrequired, damaged, OrderNo, Orderlineno, 
				PrintOrder, taxrate, ItemID, ParentItemID, SalesBrnNo, Express, WarrantyGroupId	
	INTO #delete FROM deleted   
	       
	CREATE CLUSTERED INDEX ix_delete_lineitem2324 ON #delete (acctno,itemno,agrmtno, stocklocn,contractno )  

		select i.acctno as iacctno, d.acctno as dacctno, i.agrmtno as iagrmtno, d.agrmtno as dagrmtno,
		   i.contractno as icontractno, d.contractno as dcontractno ,i.quantity as iquantity, d.quantity as dquantity,
		  d.ordval as dordval, i.ordval as iordval,d.taxamt as dtaxamt,i.taxamt as itaxamt
		       into #insertdelete from   #insert i  FULL OUTER JOIN #delete  d 
		   ON d.ItemID = i.ItemID						         
		   and d.stocklocn = i.stocklocn          
		   and d.contractno = i.contractno          
		   AND d.agrmtno= i.agrmtno
		   WHERE d.contractno != ''    --will allow for warranties alone

		   
		IF  NOT EXISTS 
		(SELECT * FROM LineitemAudit la  
		INNER JOIN lineitem l ON la.acctno = l.acctno
		INNER JOIN #insertdelete id
		ON l.acctno = COALESCE(id.iacctno, id.dacctno)
		AND l.agrmtno = COALESCE(id.iagrmtno, id.dagrmtno)
		AND l.contractno = COALESCE(id.icontractno, id.dcontractno)
		WHERE la.acctno= l.acctno		
				  AND la.ItemID= l.ItemID
				  AND la.agrmtno = l.agrmtno
				  AND la.contractno = l.contractno 
				  AND la.Datechange > DATEADD(second,-15,la.datechange)
				  AND la.QuantityAfter = id.iquantity  )
		INSERT
		INTO		LineItemAudit
				(acctno, agrmtno, empeenochange, itemno, stocklocn, 
				quantitybefore, quantityafter, valuebefore, valueafter,
				taxamtbefore,taxamtafter,contractno, datechange, source, 
				ParentItemno, ParentLocation, DelNoteBranch,RunNo,ItemID,ParentItemID, SalesBrnNo)
		select  l.acctno, l.agrmtno, '-77777', l.itemno, l.stocklocn, 
				id.dquantity, id.iquantity, id.dordval, id.iordval,
				id.dtaxamt,id.itaxamt,l.contractno, getdate(), 'GRTCancl/Exchng', 
				l.ParentItemno, l.ParentLocation, l.DelNoteBranch,0,l.ItemID,l.ParentItemID, l.SalesBrnNo
			    FROM
				lineitem l INNER JOIN #insertdelete id
				ON l.acctno = COALESCE(id.iacctno, id.dacctno)
		       AND l.agrmtno = COALESCE(id.iagrmtno, id.dagrmtno)
		       AND l.contractno = COALESCE(id.icontractno, id.dcontractno)
			


