
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM SYSOBJECTS 
           WHERE NAME = 'LoyaltyGRTCancelFreeDelAdjust'
           AND xtype = 'P')
BEGIN 
DROP PROCEDURE LoyaltyGRTCancelFreeDelAdjust
END
GO

CREATE PROCEDURE LoyaltyGRTCancelFreeDelAdjust
-- ======================================================      
-- Project      : CoSACS .NET      
-- File Name    : LoyaltyGRTCancelFreeDelAdjust.sql      
-- File Type    : MSSQL Server Stored Procedure Script      
-- Title        :   
-- Author       : ??      
-- Date         : ??        
--       
-- Change Control      
-- --------------      
-- Date      By  Description      
-- ----      --  -----------      
-- 01/08/11  IP  RI System Integration
-- ======================================================
@Acctno VARCHAR(12),
@user INT,
@return INT OUTPUT 

AS
BEGIN

	DECLARE @itemno VARCHAR(20),
			@itemID INTEGER																						--IP - 27/07/11 - RI
	
	SET @itemno = (SELECT code FROM code  
						      WHERE reference = 4
							  AND category = 'HCI')
							  
	SET @itemID = isnull((SELECT ID FROM StockInfo WHERE IUPC = @itemno AND repossesseditem = 0),0)				--IP - 27/07/11 - RI
	
	IF (SELECT value FROM CountryMaintenance
	    WHERE codename = 'LoyaltyScheme') = 'True'
	AND EXISTS (SELECT * FROM lineitem
				WHERE acctno = @acctno
				--AND itemno = @itemno
				AND ItemID = @itemID																			--IP - 01/08/11 - RI
				AND quantity = 0
				AND delqty = 0)
							  
							  
	BEGIN
		IF (SELECT SUM(ordval) FROM lineitem
		    WHERE lineitem.acctno = @acctno
		    --AND itemno != @itemno) > (SELECT value FROM CountryMaintenance
		      AND ItemID != @itemID) > (SELECT value FROM CountryMaintenance									--IP - 01/08/11 - RI			
		                                                 WHERE codename = 'LoyaltyCashThreshold')
		    AND EXISTS (SELECT * FROM Loyalty
		                INNER JOIN custacct ON custacct.custid = Loyalty.Custid
		                WHERE loyalty.StatusAcct = '1'
		                AND custacct.acctno = @Acctno
		                AND hldorjnt = 'H')
		                                               
		 BEGIN
		 	UPDATE l
		 	SET qtydiff = 'N',
		 	    delqty = 1,
		 	    quantity = 1,
		 	    ordval = l2.ordval * -1
		 	FROM lineitem l
		 	INNER JOIN lineitem l2 ON l.acctno = l2.acctno
		 	WHERE l.acctno = @Acctno
		 	--AND l.itemno = @itemno
		 	AND l.ItemID = @itemID																				--IP - 01/08/11 - RI
			AND l2.itemno = 'DEL'
			AND l2.stocklocn = l.stocklocn
			
			                  
			
			
			INSERT INTO delivery
				(origbr, acctno, agrmtno, datedel, delorcoll, itemno, stocklocn,
				quantity, retitemno, retstocklocn, retval, buffno, buffbranchno,
				datetrans, branchno, transrefno, transvalue, runno, contractno, ReplacementMarker,
				--notifiedby, ftnotes)	
				notifiedby, ftnotes, ItemID)																	--IP - 01/08/11 - RI																
				
			SELECT  DISTINCT lineitem.stocklocn, @acctno, 1, GETDATE(), 'D',@itemno,lineitem.stocklocn,
				      1,'' ,'','',hibuffno + 1,lineitem.stocklocn,
				      GETDATE(), lineitem.stocklocn,hirefno + 1,lineitem.ordval,0,'','',
				      --@user,'HC' 
				      @user,'HC', @itemID																		--IP - 01/08/11 - RI 
		    FROM delivery 
			INNER JOIN lineitem ON  lineitem.acctno = delivery.acctno
			INNER JOIN branch ON lineitem.stocklocn = branch.branchno
			WHERE delivery.acctno = @acctno
			--AND delivery.itemno = @itemno
			AND delivery.ItemID = @itemID																		--IP - 01/08/11 - RI
			AND delivery.stocklocn = lineitem.stocklocn
			--AND delivery.itemno = lineitem.itemno
			AND delivery.ItemID = lineitem.ItemID																--IP - 01/08/11 - RI
		
			
			
			
			UPDATE branch
			SET hibuffno = hibuffno + 1,
			    hirefno = hirefno + 1
			FROM lineitem
			WHERE branchno = lineitem.stocklocn
			AND lineitem.acctno = @Acctno
			--AND itemno = @itemno
			AND ItemID = @itemID																				--IP - 01/08/11 - RI
			
			UPDATE agreement 
			SET agrmttotal = (SELECT SUM(ordval) FROM lineitem l
			                  WHERE l.acctno = agreement.acctno)
			WHERE agreement.acctno = @Acctno
			
			UPDATE acct  
			SET agrmttotal = (SELECT SUM(ordval) FROM lineitem l
			                  WHERE l.acctno = acct.acctno),
			    outstbal = (SELECT SUM(transvalue) FROM fintrans f
			                WHERE f.acctno = acct.acctno)
			WHERE acct.acctno = @Acctno
						 	
		 END
	END
	
	SET @return = 0
END