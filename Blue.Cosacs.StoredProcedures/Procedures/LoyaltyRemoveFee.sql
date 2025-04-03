
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM SYSOBJECTS 
           WHERE NAME = 'LoyaltyRemoveFee'
           AND xtype = 'P')
BEGIN 
DROP PROCEDURE LoyaltyRemoveFee
END
GO

CREATE PROCEDURE [dbo].[LoyaltyRemoveFee]
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : LoyaltyRemoveFee.sql
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
@custid VARCHAR(20),
@user INT 

AS
BEGIN

DECLARE @acctno VARCHAR(12),
        @branch VARCHAR(3),
        --@itemtoadd VARCHAR(10),
        @itemtoadd VARCHAR(18),							--IP - 27/07/11 - RI
        @linevalue MONEY, 
        @itemID INTEGER									--IP - 27/07/11 - RI
        
   
   
        
   --SELECT  @itemtoadd = itemno FROM delivery D
   SELECT  @itemtoadd = SI.iupc FROM delivery D	INNER JOIN stockinfo SI ON D.ItemID = SI.ID			--IP - 27/07/11 - RI
   INNER JOIN loyalty L ON D.acctno = L.loyaltyacct
   WHERE L.Custid = @Custid 
   AND D.Datedel = (SELECT MAX(datedel) FROM delivery D2
                    WHERE D2.acctno = D.acctno) 
                    

	SELECT @acctno = Loyalty.LoyaltyAcct
	FROM Loyalty
	WHERE custid = @custid

	
	SET @branch = SUBSTRING(@acctno,1,3)
	
	--SET @linevalue = (SELECT MAX(price) FROM lineitem
	--                  WHERE itemno = @itemtoadd
	--                  AND acctno = @acctno)
	
	
	SET @itemID = isnull((select ID from stockinfo where iupc = @itemtoadd and repossesseditem = 0),0)		--IP - 27/07/11 - RI
	
	SET @linevalue = (SELECT MAX(price) FROM lineitem												--IP - 27/07/11 - RI - replaces above code
	                  WHERE ItemID = @itemID
	                  AND acctno = @acctno)
	               
	
	IF (((SELECT SUM(transvalue) FROM fintrans
	     WHERE acctno = @acctno) > 0)  AND @itemtoadd = (SELECT code FROM code 
														WHERE category = 'HCI'
														AND reference = 3)
	   OR @itemtoadd =  (SELECT code FROM code 
	                      WHERE category = 'HCI'
	                      AND reference = 2))
	    
	BEGIN
	
		  
		INSERT INTO delivery
		(origbr, acctno, agrmtno, datedel, delorcoll, itemno, stocklocn,
		quantity, retitemno, retstocklocn, retval, buffno, buffbranchno,
		datetrans, branchno, transrefno, transvalue, runno, contractno, ReplacementMarker,
			notifiedby, ftnotes, ItemID)														--IP - 27/07/11 - RI
		SELECT	@branch ,@acctno, 1, getdate(), 'C', @itemtoadd,@branch , 
		-1, '', '', '', hibuffno + 1,@branch , 
		getdate(),@branch ,  hirefno + 1, @linevalue * -1, 0, '', '',
		@user, 'HC', @itemID																	--IP - 27/07/11 - RI
		FROM	Branch
		WHERE Branchno = @branch 


		--UPDATE lineitem  
		--SET quantity = quantity -1,  
		--	delqty = delqty -1,  
		--	ordval = ordval - @linevalue  
	 --  WHERE acctno = @acctno
	 --  AND itemno = @itemtoadd 
	 
		 UPDATE lineitem  
		 SET quantity = quantity -1,  
			 delqty = delqty -1,  
			 ordval = ordval - @linevalue  
		 WHERE acctno = @acctno
		 AND ItemID = @itemID																	--IP - 27/07/11 - RI - Replaces above code   
			
		UPDATE branch
		SET hibuffno = hibuffno + 1,
			hirefno = hirefno +1
		WHERE branchno = @branch
		
		UPDATE acct
		SET outstbal =  (SELECT SUM(transvalue) 
						 FROM fintrans 
						 WHERE fintrans.acctno = acct.acctno),
			agrmttotal = (SELECT SUM(ordval) FROM lineitem
						 WHERE lineitem.acctno = acct.acctno)			
		WHERE acctno = @acctno
			
		
		UPDATE agreement
		SET agrmttotal = (SELECT SUM(ordval) FROM lineitem
						  WHERE lineitem.acctno = agreement.acctno)
		WHERE acctno = @acctno
	
	END
	
	-- uat 139 --
	UPDATE acct	
	SET currstatus = 'S'  
	WHERE acctno = @acctno AND outstbal = 0
	
END
GO


