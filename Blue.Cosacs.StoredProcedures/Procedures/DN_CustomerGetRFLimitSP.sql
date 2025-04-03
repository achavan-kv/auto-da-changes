--!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
--
--NOTE: If this calculation changes then the SP dn_CustomerCalculateAvailableSpendAll will have to be changed as well
--
--!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

IF  EXISTS (SELECT 1 
	FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DN_CustomerGetRFLimitSP]') 
	AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DN_CustomerGetRFLimitSP]

GO

-- AA 26/05/2006 Excluding Add-to
CREATE PROCEDURE  [dbo].[DN_CustomerGetRFLimitSP]
/***********************************************************************************************************
-- ===============================================================================================
-- Version:		<004> 
-- ===============================================================================================

--
-- Project      : CoSACS .NET
-- File Name    : DN_CustomerGetRFLimitSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Description  : Procedure that returns the available limit for a customer
-- Author       : ?
-- Date         : ?
--
--  
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
  22/12/10   IP  Store Card Development - If the Customer has a Store Card Limit and a Store Card Available
				 then we need to re-calculate the Store Card Available based on changes to the RF Available.
  26/07/11   IP  RI - System Integration
  06/10/11   IP  #3915 - CR1232 - Cash Loan - Account Creation. Exclude RF accounts that have a Cash Loan pending
  13/10/11   jec CR1232 exclude CLD transtypecode from payments
************************************************************************************************************/
            @custid VARCHAR(20),
            @AcctList VARCHAR(400),
            @limit MONEY OUT,
            @available MONEY OUT,
            @return INT OUTPUT

AS

    DECLARE @acctno VARCHAR(12)
    DECLARE @agrmttotal MONEY
    DECLARE @serviceCharge MONEY
    DECLARE @deposit MONEY
    DECLARE @outstbal MONEY
    DECLARE @transvalue MONEY
    DECLARE @used MONEY
    DECLARE @payments MONEY
    DECLARE @nonstockvalue MONEY, @usedbyaccount MONEY
    DECLARE @ExcludeList VARCHAR(450)
 
    
    SET @return = 0               --initialise return code
    SET @available = 0            
    SET @limit = 0                
    SET @used = 0                 
    SET @serviceCharge = 0        
    SET @deposit = 0              
    SET @nonstockvalue = 0        

    IF (@AcctList != '')
        SET @ExcludeList = ' AND A.AcctNo NOT IN (' + @AcctList + ')'
    ELSE
        SET @ExcludeList = ''

     SELECT cl.AcctNo
     INTO #excludeAccounts
     FROM custacct ca
     INNER JOIN CashLoan cl
     ON ca.acctno = cl.AcctNo
     WHERE ca.custid = @custid
		AND ca.hldorjnt = 'H'
		AND cl.LoanStatus IN ('','P','C', 'R')

    EXECUTE  
       (' DECLARE RFaccts CURSOR FOR ' +  
        ' SELECT A.acctno, A.agrmttotal, A.outstbal, AG.servicechg, AG.deposit ' +   
        ' FROM   acct A INNER JOIN custacct CA ON A.acctno = CA.acctno ' +  
        '        INNER JOIN agreement AG ON A.acctno = AG.acctno ' +  
        '        INNER JOIN accttype AT ON A.accttype = AT.genaccttype ' +            -- added 27/11/03 JJ because H is actually O in acct table.  
        ' WHERE  AT.accttype NOT IN (''C'', ''S'') ' +  
        ' AND    CA.custid = ''' + @custid + '''' +  
        ' AND    CA.HldOrJnt = ''H'' ' +                                              -- omit settled accounts JEC 29/08/03  
        ' AND    currstatus <> ''S'' ' +                                              -- exclude cash Loan accounts that have not been disbursed           
        ' AND a.acctno not in(SELECT * from #excludeAccounts) ' +                     -- jec 04/11/11 IP - 06/10/11 -CR1232  
        @ExcludeList)  

    OPEN RFaccts
    FETCH NEXT FROM RFaccts 
    INTO @acctno, @agrmttotal, @outstbal, @serviceCharge, @deposit

    WHILE @@FETCH_STATUS = 0
    BEGIN

        SELECT    @payments = ISNULL(SUM(transvalue),0)
        FROM    fintrans
        WHERE    acctno = @acctno
        AND        transtypecode NOT IN ('GRT', 'DEL','ADD', 'CLD') 
        SELECT    @nonstockvalue = ISNULL (SUM(ordval), 0)
        FROM    lineitem l
		INNER JOIN stockitem s ON l.ItemID = s.ID									
        AND        l.stocklocn = s.stocklocn
        WHERE    acctno = @acctno
        AND        s.itemtype = 'N'
        AND        s.iupc NOT IN('DT', 'SD','ADDDR','ADDCR','LOAN')
        --AND        l.ItemID = s.ID									
        --AND        l.stocklocn = s.stocklocn
        AND        s.category NOT IN (SELECT code FROM code WHERE category = 'PCDIS') 
        

        IF(@payments + @deposit < 0) 
            SET @payments =@payments + @deposit -- reducing payments by deposit value
        ELSE /*has not yet paid the deposit */
			SET @payments = 0

		
            
         IF(@agrmttotal >0)
        BEGIN
			
				-- payments should be proportioned based on service charge and excluding non stocks
				SET    @payments = @payments*((@agrmttotal-@servicecharge- @nonstockvalue)/@agrmttotal) 
				--changes for amortization only loan amount to be consdiered 
				IF EXISTS (SELECT CodeName,value FROM countrymaintenance WHERE CodeName='CL_Amortized' AND value='true')
				BEGIN
				
						SET @usedbyaccount =@agrmttotal  + @payments 
				END 
				ELSE
				BEGIN
					SET @usedbyaccount =@agrmttotal - @serviceCharge - @deposit + @payments - @nonstockvalue
				END
				
			
	    IF @usedbyaccount >0 
	            
SET    @used = @used + @usedbyaccount
        END

        FETCH NEXT FROM RFaccts 
        INTO @acctno, @agrmttotal, @outstbal, @serviceCharge, @deposit
    END

    CLOSE RFaccts
    DEALLOCATE RFaccts

    SELECT    @limit = RFCreditLimit
    FROM    customer 
    WHERE    custid = @custid

    SET @available = @limit - @used
    
    UPDATE customer SET availablespend=@available
    WHERE    custid = @custid
    
    --IP - 22/12/10 - Store Card - If the Store Card Available is > RF Available
    --then set the Store Card Available = RF Available
    DECLARE @storeCardLimit MONEY,
			@storeCardAvailable MONEY,
            @totUsedAmt MONEY

    SET @totUsedAmt = 0
			
    SELECT	@storeCardLimit = ISNULL(c.StoreCardLimit,0)--,
			--@storeCardAvailable =    CASE WHEN c.AvailableSpend - C.StoreCardAvailable >0  THEN
			--	CASE WHEN  C.StoreCardAvailable >0 THEN  C.StoreCardAvailable ELSE 0 END ELSE  
			--	CASE WHEN  C.AvailableSpend >0 THEN c.AvailableSpend ELSE 0 END END 
	FROM	customer c
	WHERE custid = @custid

    SELECT 
        @totUsedAmt = ISNULL(SUM(transvalue ),0) 
    FROM 
        fintrans f 
    INNER JOIN 
        custacct ca ON f.acctno = ca.acctno
    INNER JOIN 
        Acct a ON a.acctno= ca.acctno
    WHERE 
        ca.custid = @custId AND a.accttype ='T'
        AND ca.hldorjnt = 'h'
        AND f.transtypecode IN ('PAY', 'REF', 'COR', 'SCT', 'STR')

    SET @storeCardAvailable = @storeCardLimit - ISNULL(@totUsedAmt, 0)
	
	IF(@storeCardLimit > 0 AND @storeCardAvailable > 0)
	BEGIN
		IF(@storeCardAvailable > @available)
		BEGIN
			UPDATE customer
			SET	   StoreCardAvailable = @available
			WHERE  custid = @custid
		END
		ELSE
		BEGIN
			--Need to update Store Card Available as the RF Available may have been brought back up to above the Store Card Available
			--meaning the Store Card Available would need to be recalculated.
			EXEC CustomerUpdateStoreCardAvailable @custid, @storeCardLimit OUT, @storeCardAvailable OUT, 0
		END
	END
	
    IF (@@ERROR != 0)
    BEGIN
        SET @return = @@ERROR
    END
   
GO


