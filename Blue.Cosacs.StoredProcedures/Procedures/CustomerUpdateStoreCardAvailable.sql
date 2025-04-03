SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects
           where id = object_id('[dbo].[CustomerUpdateStoreCardAvailable]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[CustomerUpdateStoreCardAvailable]
GO

CREATE PROCEDURE  CustomerUpdateStoreCardAvailable
/***********************************************************************************************************
--
-- Project      : CoSACS .NET
-- File Name    : CustomerUpdateStoreCardAvailable.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Description  : Procedure that updates and returns the Store Card Available limit for a Customer
-- Date      By  Description
-- ----      --  -----------
-- 16/12/10  IP  
-- 19/01/11  IP  Store Card Sprint4 - #2715 - Calculation was previously only picking up Cash accounts that belonged
--				 to the Customer that has the Store Card. Changed to select all Cash where a payment has been made
--				 using the Store Card.

************************************************************************************************************/
            @custID varchar(20),
            @storeCardLimit money OUT,
            @storeCardAvailable money OUT,
            @return int OUTPUT
           
  
AS
 
 DECLARE @acctno varchar(12)
 DECLARE @accttype char(1)
 DECLARE @agrmtno int
 DECLARE @agrmttotal float
 DECLARE @totAgrmttotal float
 DECLARE @storeCardAcct varchar(12)
 DECLARE @storeCardNo bigint
 DECLARE @nonstockvalue float
 DECLARE @totNonStockVal float
 DECLARE @storeCardUsedAmt money
 DECLARE @totUsedAmt money
 DECLARE @totPaymentsOnStoreCard money
 DECLARE @totPaymentsToReduceUsed money
 DECLARE @rfAvailable money 
 DECLARE @rfLimit money
 
	SET	@return = 0
	SET @storeCardAvailable = 0            
    SET @storeCardLimit = 0   
    SET	@agrmttotal = 0  
    SET @totAgrmttotal = 0             
    SET @nonstockvalue = 0
    SET @totNonStockVal= 0
    SET @storeCardUsedAmt = 0
	SET @totUsedAmt = 0    
   	SET @totPaymentsOnStoreCard = 0
   	SET @totPaymentsToReduceUsed = 0
   	
	


    SELECT    @storeCardLimit = StoreCardLimit,
			  @rfAvailable = Case when AvailableSpend >0 then AvailableSpend  else 0 end
    FROM	  customer 
    WHERE     custid = @custID
	
	SELECT @totUsedAmt = ISNULL(SUM(transvalue ),0) FROM fintrans f 
	JOIN custacct ca ON f.acctno = ca.acctno
	join Acct a on a.acctno= ca.acctno
	WHERE ca.custid = @custId and a.accttype ='T'
	AND ca.hldorjnt = 'h' 
    AND f.transtypecode in ('PAY', 'REF', 'COR', 'SCT', 'STR')
--	AND f.transtypecode NOT IN ('INT','STI') --not now excluding interest charges as keeping it simple.
	
	SET @storeCardAvailable = @storeCardLimit - ISNULL(@totUsedAmt,0)

	IF @storeCardAvailable > @rfAvailable
		SET @storeCardAvailable = @rfAvailable
	
   	IF @storeCardAvailable <0 
   		SET @storeCardAvailable=0 
	----Select the Customers Store Card Account number
	--select @storeCardAcct = MAX(a.acctno)
	--						from acct a inner join custacct ca on a.acctno = ca.acctno
	--						where ca.custid = @custID
	--						and ca.hldorjnt = 'H'
	--						and a.accttype = 'T'
	
	----Select the Store Card number tied to the Store Card account.
	--select @storeCardNo = cardnumber from storecard where acctno = 	@storeCardAcct
      
 --  DECLARE Accts CURSOR FOR  
   
 --  ----Select all cash accounts for the customer --IP - 19/01/11 - This now replaced with the code below.
 --  --SELECT	a.acctno, ag.agrmtno, a.accttype, a.agrmttotal
 --  --FROM		acct a INNER JOIN custacct ca ON a.acctno = ca.acctno
	--		--INNER JOIN agreement ag on a.acctno = ag.acctno
 --  --WHERE	a.accttype = 'C'
 --  --AND		ca.custid = @custID
 --  --AND		ca.hldorjnt = 'H'
 --  --AND		a.currstatus!='S'
   
 --  --Select any Cash accounts which has had a payment made using the customers store card --IP - 19/01/11 - #2715
 --  SELECT	a.acctno, ag.agrmtno, a.accttype, a.agrmttotal
 --  FROM		agreement ag INNER JOIN acct a ON a.acctno = ag.acctno
	--		INNER JOIN finxfr f on f.acctnoxfr = a.acctno
 --  WHERE	a.accttype = 'C'
 --  AND		a.currstatus!='s'
 --  AND		f.storecardno = @storeCardNo
 --  UNION
 --  --Select any Cash & Go sales processed using Store Card as payment
 --  SELECT	ag.acctno, ag.agrmtno, a.accttype, ag.agrmttotal 
 --  FROM		agreement ag INNER JOIN acct a ON a.acctno = ag.acctno
	--		INNER JOIN finxfr f on f.acctnoxfr = ag.acctno
 --  WHERE	f.storecardno = @storeCardNo
 --  AND		f.agrmtno = ag.agrmtno     
 --  AND      f.agrmtno > 1
 --  AND		a.accttype = 'S'
   
 -- --Select the sum of any payments made against the Store Card account which we need to work out how much to increase the Store Card Available by.
 --  SELECT @totPaymentsOnStoreCard = isnull(SUM(f.transvalue),0)
 --  FROM	  fintrans f
 --  WHERE  f.acctno = @storeCardAcct
 --  AND f.transtypecode in ('PAY', 'REF','COR')
   
 --  SET @totPaymentsOnStoreCard = abs(@totPaymentsOnStoreCard)
   
 --  OPEN Accts FETCH NEXT FROM Accts
 --  INTO @acctno, @agrmtno, @accttype, @agrmttotal
   
 --  WHILE @@fetch_status = 0
 --  BEGIN
	
	--   SET @totAgrmttotal = @totAgrmttotal + @agrmttotal -- Will use this later to work out the portion of the total payments which we need to increase the Store Card Available Spend
	  
	--   --Select the sum of payments made on Cash accounts/Special accounts (for Cash & Go) using Store Card as payment
	  
	--   SELECT @storeCardUsedAmt = isnull(SUM(transvalue),0)
	--   FROM   fintrans f 
	--   WHERE  f.acctno = @acctno
	--   AND	  f.agrmtno = @agrmtno
	--   AND    f.transtypecode = 'SCT'

	--   --Select the value of the non stocks on the account
	--   SELECT  @nonstockvalue = isnull (SUM(ordval), 0)
 --      FROM    lineitem l, stockitem s
 --      WHERE   acctno = @acctno
 --      AND	   agrmtno = @agrmtno	
 --      AND        s.itemtype = 'N'
 --      AND        l.itemno NOT IN('DT', 'SD','ADDDR','ADDCR','LOAN', 'STAX')  --IP - 19/01/11 - Also exclude tax
 --      AND        l.itemno = s.itemno
 --      AND        l.stocklocn = s.stocklocn
 --      AND        s.category NOT IN (select code from code where category = 'PCDIS') -- Remove hardcoded discounts & warranties
       
 --      SET @totNonStockVal = @totNonStockVal + @nonstockvalue -- Will use this later to work out the portion of the total payments which will increase the Available Spend
	   
	--   IF(@agrmttotal >0)
	--   BEGIN
	--		SET    @storeCardUsedAmt = abs(@storeCardUsedAmt *((@agrmttotal- @nonstockvalue)/@agrmttotal)) --Amount used from store card should not include non-stocks should be for stocks
			
	--		IF(@storeCardUsedAmt > 0)
	--		begin
	--			SET @totUsedAmt = @totUsedAmt + @storeCardUsedAmt
	--		end
	--   END
		
	--	FETCH NEXT FROM Accts 
 --       INTO @acctno, @agrmtno, @accttype, @agrmttotal	 
 --  END
   
 --  CLOSE Accts
 --  DEALLOCATE Accts
 
 --  --Now need to increase the available spend by any payments that may have been made against the Store Card account itself. Reduce the @used amount
 -- IF(@totPaymentsOnStoreCard > 0)
 -- BEGIN
		
	--	SET @totPaymentsToReduceUsed = @totPaymentsOnStoreCard * ((@totAgrmttotal - @totNonStockVal)/ @totAgrmttotal)
	--	SET @totUsedAmt = @totUsedAmt - @totPaymentsToReduceUsed
		
 -- END
   

 --   SELECT    @storeCardLimit = StoreCardLimit,
	--		  @rfAvailable = AvailableSpend
 --   FROM	  customer 
 --   WHERE     custid = @custID

	--SET @storeCardAvailable = @storeCardLimit - @totUsedAmt
     
 --   --If the Store Card Available is > RF Available then set the Store Card Available to RF Available.
 --   IF  @storeCardAvailable >=  @rfAvailable
 --   BEGIN
	--    SET @storeCardAvailable = @rfAvailable
	--END

	--IF @storecardavailable <0
	--   set @storecardavailable = 0


	UPDATE customer 
    SET	   StoreCardAvailable=Round(@storeCardAvailable,2)
    WHERE  custid = @custID --AND  @StoreCardAvailable <=availablespend
   


IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END 
GO  

