
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM SYSOBJECTS 
           WHERE NAME = 'LoyaltyGRTFreeDelAdjust'
           AND xtype = 'P')
BEGIN 
DROP PROCEDURE LoyaltyGRTFreeDelAdjust
END
GO

CREATE PROCEDURE LoyaltyGRTFreeDelAdjust
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : LoyaltyGRTFreeDelAdjust.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : 
-- Author       : ?
-- Date         : ?

-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 27/07/11 ip  RI - System Integration
-- ================================================
@Acctno VARCHAR(12),
@user INT,
@return INT OUTPUT

AS
BEGIN

	DECLARE @itemno VARCHAR(20),
			@itemID INTEGER																				--IP - 27/07/11 - RI
	
	SET @itemno = (SELECT code FROM code  
						      WHERE reference = 4
							  AND category = 'HCI')
	
	SET @itemID = isnull((SELECT ID from StockInfo WHERE IUPC = @itemno AND repossesseditem = 0),0)		--IP - 27/07/11 - RI					  
							  
	IF (SELECT value FROM CountryMaintenance
	    WHERE codename = 'LoyaltyScheme') = 'True'
	AND EXISTS (SELECT * FROM lineitem
				WHERE acctno = @acctno
				--AND itemno = @itemno
				AND ItemID = @itemID																	--IP - 27/07/11 - RI															
				AND quantity > 0
				AND delqty > 0)
							  
							  
	BEGIN
		IF (SELECT SUM(ordval) FROM lineitem
		    WHERE lineitem.acctno = @acctno
		    --AND itemno != @itemno) < (SELECT value FROM CountryMaintenance
		    AND ItemID != @itemID) < (SELECT value FROM CountryMaintenance								--IP - 27/07/11 - RI
		                                                 WHERE codename = 'LoyaltyCashThreshold')
		 BEGIN
		 	UPDATE lineitem 
		 	SET qtydiff = 'N',
		 	    delqty = 0,
		 	    quantity = 0,
		 	    ordval = 0
		 	WHERE acctno = @Acctno
		 	--AND itemno = @itemno
		    AND ItemID = @itemID																		--IP - 27/07/11 - RI
			
			INSERT INTO delivery
				(origbr, acctno, agrmtno, datedel, delorcoll, itemno, stocklocn,
				quantity, retitemno, retstocklocn, retval, buffno, buffbranchno,
				datetrans, branchno, transrefno, transvalue, runno, contractno, ReplacementMarker,
				notifiedby, ftnotes, ItemID)															--IP - 27/07/11 - RI
				
			SELECT  lineitem.stocklocn, @acctno, 1, GETDATE(), 'C',@itemno,lineitem.stocklocn,
				      SUM(delivery.quantity) * -1 ,'' ,'','',hibuffno + 1,lineitem.stocklocn,
				      GETDATE(), lineitem.stocklocn,hirefno + 1, SUM(delivery.transvalue) * -1,0,'','',
				      @user,'HC', @itemID																--IP - 27/07/11 - RI 
		    FROM delivery 
			INNER JOIN lineitem ON delivery.acctno = lineitem.acctno
			INNER JOIN branch ON lineitem.stocklocn = branch.branchno
			WHERE  lineitem.acctno = @acctno
			--AND lineitem.itemno = delivery.itemno
			AND lineitem.ItemID = delivery.ItemID
			--AND lineitem.itemno = @itemno 
			AND lineitem.ItemID = @itemID																--IP - 27/07/11 - RI 
			AND lineitem.stocklocn = delivery.stocklocn
			GROUP BY lineitem.stocklocn,delivery.acctno, hibuffno,hirefno
			
			
			
			UPDATE branch
			SET hibuffno = hibuffno + 1,
			    hirefno = hirefno + 1
			FROM lineitem 
			WHERE branchno = stocklocn
			AND lineitem.acctno = @Acctno
			--AND lineitem.itemno = @itemno
			AND lineitem.ItemID = @itemID																--IP - 27/07/11 - RI
		
			
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