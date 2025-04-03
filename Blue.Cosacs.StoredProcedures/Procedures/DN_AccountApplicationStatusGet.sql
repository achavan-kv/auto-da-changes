
IF  EXISTS (SELECT 1 
	FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DN_AccountApplicationStatusGet]') 
	AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DN_AccountApplicationStatusGet]

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[DN_AccountApplicationStatusGet]
-- ========================================================================
-- Version:		<002> 
-- ========================================================================
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_AcCOUNTApplicationStatusGet.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : AcCOUNT Application Status Get 
-- Author       : ??
-- Date         : ??
--
-- This procedure will retrieve the AcCOUNT Application Status.
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 02/03/10 jec CR1072 Malaysia v4 merge
-- 05/03/10 jec CR1072 Do not show SCH status if DHL delivery.
-- 09/03/10 jec CR1072 REJ - exclude DAD status if rejected, RFL exclude if DHL
-- 30/03/10 jec UAT40 correct DAD status
--				UAT43 Correct DEP status
-- 15/04/10 jec UAT81 correct SCH status
-- 19/04/10 jec UAT35 Correct DAD status
-- 20/04/10 jec UAT89 Correct DEP and DAD status when collection 
-- 21/04/10 jec UAT81 correct SCH status again
-- 21/05/10 jec UAT164 Application status with 1st inst required
-- 21/05/10 jec UAT204 Correct COL status
-- 25/05/10 ip  UAT224 Changed the DEP status
-- 21/09/10 ip  UAT5.2 Log - UAT1128 - Awaiting Cheque Clearance
-- 06/10/10 jec UAT1045 correct AWP status if third party warehouse
-- 25/02/11 jec CR1090 Instalment waiver
--              CR1225 Application Status ADI for instant credit    
-- 01/03/11 ip  #3242 - ADI incorrectly shown for acCOUNTs not qualified for Instant Credit. Added ISNULL check. 
-- 01/03/11 ip  #3242 - Moved SELECT for instalplan as previously values were not being SELECTed if instalpredel = 'N'
-- 04/03/11 ip  #3275 - INP status incorrectly displayed for acCOUNT qualified for first instalment waiver. Added check to ensure @InstalWaived = 0
-- 10/03/11 ip  #3297 - Do not display the 'PREV', 'SALY', and 'STOC' flags for Instant Credit acCOUNTs FROM category 'PH2'
-- 11/03/11 ip  #3308 - Removed change previously put in for #3275
-- 11/03/11 ip  #3319 - Set @deposit to instalment amount if > than agreement.deposit
-- 18/03/11 ip  #2578 - Show goods delievered for Cash Loan acCOUNTs
-- 13/10/11 jec CR1232 DAD status for cash loan
-- 15/05/12 ip  #9476 - LW74503 - Application Status should now reflect Goods Delivered for an acCOUNT with an ADDDR item
-- 18/07/12 ip  #10388 - Added Application Status Scheduled and Fail Delivery for Warehouse
-- 28/03/13 jec #12841 - schedule table no longer used -  
-- 16/05/13 ip  #13349 - No Order Lines not displayed on an acCOUNT due to DEL item of 0 value
-- 28/05/13 ip  #13658 - No Order Lines not displayed on an acCOUNT due to DT item of 0 value
-- 28/05/13 ip  #13658 - Check WHERE Lineitem.Quantity > 0 or Lineitem.DelQty > 0 for item COUNT.
-- 29/05/13 ip  #13658 - Only display No Order Lines if there are currently no items scheduled.
-- 17/10/13 ip  #15495 - Change Booking to Shipment
-- 22/01/14 ip  #17083 - Service Request CHARge acCOUNT table changed FROM SR_CHARgeAcct to ServiceCHARgeAcct
-- 26/06/14 ip  #18605 - CR15594 - Do not display status INP (Deposit/Instal-predel not paid) if Ready Assist acCOUNT
-- 30/07/20  Zensar Optimization changes : Changed Select * to Top 1 'a' in Exists and Non Exists Statement
--										   Unqualified Joins are changed to ANSI Joins.
-- ================================================
	-- Add the parameters for the stored procedure here
		 @acctno VARCHAR (12),
		 @return INTEGER output
AS 
 SET NOCOUNT ON
	SET @return = 0

   	CREATE TABLE #applicationstatus (StatusCode VARCHAR (4), StatusDescription VARCHAR (200))

	DECLARE @accttype CHAR (1)
	DECLARE @propresult CHAR(1)
	DECLARE @currstatus CHAR(1)
	DECLARE @reflresult CHAR(1)
	DECLARE @instalpredel CHAR(1)
	DECLARE @holdprop CHAR(1)
	DECLARE @dateprop DATETIME
	DECLARE @delqty FLOAT
	DECLARE @quantity FLOAT
	DECLARE @canCOUNT INT
	DECLARE @lineCOUNT INT
	DECLARE @payCOUNT INT
	DECLARE @continue INT
	DECLARE @schedCOUNT INT
	DECLARE @delCOUNT INT
	DECLARE @agrmtno INT
	DECLARE @instamount MONEY
	DECLARE @amountpaid MONEY
	DECLARE @price MONEY
	DECLARE @deposit MONEY
	DECLARE @termstype VARCHAR(2)
	DECLARE @paymethod VARCHAR(1)
	DECLARE @custid VARCHAR(20)
	DECLARE @itemId INT
	DECLARE @cheqdays INT
	DECLARE @revCOUNT INT
	DECLARE @stocklocn INT
	DECLARE @isLoan BIT --IP - 22/08/08 - UAT(491)
	DECLARE @deltot FLOAT --IP - 25/07/08 - UAT(491)
	DECLARE @agreementtot FLOAT --IP - 25/07/08 - UAT(491)
	DECLARE @refused INT --IP - 18/02/10 - CR1072 - LW 71198 - General Fixes FROM 4.3 - Merge
	DECLARE @deliveryprocess CHAR(1)	--UAT81 
	DECLARE @cheqAmount MONEY	--IP - 21/09/10 - UAT5.2 Log - UAT(1128)
	DECLARE @InstalWaived INT	--jec 25/02/11
	DECLARE @InstantCredit VARCHAR(1) --jec 25/02/11
	DECLARE @LineSchedCOUNT INT -- #13658
	DECLARE @isReadyAssist BIT -- #18605 --CR19954

	SET @accttype = ''
	SET @termstype = ''
	SET @paymethod = ''
	SET @custid = ''
	SET @propresult = ''
	SET @currstatus = ''
	SET @reflresult = ''
	SET @instalpredel = ''
	SET @holdprop = ''
	SET @itemId = 0
	SET @canCOUNT = 0
	SET @instamount = 0
	SET @amountpaid = 0
	SET @lineCOUNT = 0
	SET @payCOUNT = 0
	SET @quantity = 0
	SET @continue = 0
	SET @schedCOUNT = 0
	SET @price = 0
	SET @delCOUNT = 0
	SET @deposit = 0
	SET @agrmtno = 0
	SET @cheqdays = 0
	SET @revCOUNT = 0
	SET	@stocklocn = 0
	SET @isLoan = 0 --IP - 22/08/08 - UAT(491)
	SET @deltot = ISNULL((SELECT SUM(transvalue) FROM delivery WHERE acctno =  @acctno), 0) --IP - 25/07/08 - UAT(491)
	SET @refused=0 --IP - 18/02/10 - CR1072 - LW 71198 - General Fixes FROM 4.3 - Merge
	SET @cheqAmount = 0	--IP - 21/09/10 - UAT5.2 Log - UAT(1128)
	SET @LineSchedCOUNT = 0 -- #13658
	SET @isReadyAssist = 0  -- #18605 --CR19954


   SET NOCOUNT ON	
	SELECT	@accttype = A.accttype,
			@currstatus = A.currstatus,
			@termstype = A.termstype,
			@paymethod = AG.paymethod,
			@deposit = AG.deposit,
			@holdprop =  AG.holdprop,
			@agreementtot = AG.agrmttotal, --IP - 25/07/08 - UAT(491)
			@isLoan = ISNULL(TT.isloan, 0), --IP - 22/08/08 - UAT(491)
			@custid = CA.custid
	FROM		acct A INNER JOIN
			agreement AG ON A.acctno = AG.acctno INNER JOIN
			custacct CA ON A.acctno = CA.acctno LEFT JOIN
			termstypetable TT on A.termstype = TT.termstype --IP - 22/08/08 - UAT(491)
	WHERE	A.acctno = @acctNo
	AND 		CA.hldorjnt = 'H'

	--#18605 - CR15594
	IF EXISTS(SELECT TOP 1 'a' FROM ReadyAssistDetails
				WHERE acctno = @acctno
				AND (Status IS NULL OR Status = 'Active'))
	BEGIN
		
		SET @isReadyAssist = 1

	END

	SELECT	TOP 1 @propresult = propresult,
		     	 @dateprop	  = dateprop	 	
	FROM		proposal
	WHERE	acctno = @acctno
	AND		custid = @custid
	ORDER BY 	dateprop DESC
	
	--IP - 18/02/10 - CR1072 - LW 71198 - General Fixes FROM 4.3 - Merge
	IF((@propresult = N'X' or @propresult=N'D') AND @acctType != N'S' AND @acctType != N'C')
	BEGIN
	        	INSERT INTO #applicationstatus  
	        	(
	            		StatusCode,
	            		StatusDescription
	        	)
	        	SELECT	code,
	               	codedescript
	        	FROM   	code
		WHERE  	code = 'CRF'
		AND category = 'APS'
		SET @refused=1
	END

IF (@refused=0) --IP - 18/02/10 - CR1072 - LW 71198 - General Fixes FROM 4.3 - Merge
BEGIN
--Check if Details Required

	--IP - 01/03/11 - #3242 --IP - 10/03/11 - #3297 - Moved to here FROM below
	SELECT TOP 1	@instamount = instalamount, 
						@InstalWaived = InstalmentWaived,@InstantCredit	= InstantCredit		-- jec 25/02/11
		FROM 		INSTALPLAN
		WHERE 	acctno = @acctno
		ORDER BY 	datefirst DESC

	BEGIN
	        	INSERT INTO #applicationstatus  
	        	(
	            		StatusCode,
	            		StatusDescription
	        	)
	        	SELECT	code,
	               	codedescript
	        	FROM   	code 
	        	INNER JOIN  proposalflag ON code.code =proposalflag.checktype
		WHERE  	proposalflag.acctno = @acctno
        AND datecleared IS NULL                  
		AND	((category LIKE 'PH%'     
        AND @InstantCredit = 'N')
        OR category = 'PH1')
        	--IP - 10/03/11 - #3297 
	END
	--Check if Details Required
	IF(@currstatus = '0')
	BEGIN
	        	INSERT INTO #applicationstatus  
	        	(
	            		StatusCode,
	            		StatusDescription
	        	)
	        	SELECT	code,
	               	codedescript
	        	FROM   	code
		WHERE  	code = 'REQ'
		AND category = 'APS'
	END

	--Check if Awaiting Sanction
	IF(@propresult = '' AND @acctType != 'S' AND @acctType != 'C')
	BEGIN
	        	INSERT INTO #applicationstatus  
	        	(
	            		StatusCode,
	            		StatusDescription
	        	)
	        	SELECT	code,
	               	codedescript
	        	FROM   	code
		WHERE  	code = 'SAN'
		AND category = 'APS'
	END

	--Check if Awaiting Referal
	IF(@propresult = 'R' AND @acctType != 'S' AND @acctType != 'C')
	BEGIN
	        	INSERT INTO #applicationstatus  
	        	(
	            		StatusCode,
	            		StatusDescription
	        	)
	        	SELECT	code,
	               	codedescript
	        	FROM   	code
		WHERE  	code = 'REF'
		AND category = 'APS'
	END


	IF(@propresult = 'D' AND @acctType != 'S' AND @acctType != 'C')
	BEGIN
		   	INSERT INTO #applicationstatus  
	        	(
	            		StatusCode,
	            		StatusDescription
	        	)
	        	SELECT	code,
	               	codedescript
	        	FROM   	code
		WHERE  	code = 'CRF'
		AND category = 'APS'
	END

	SELECT	@instalpredel = instalpredel
	FROM 		termstype t
	INNER JOIN country c ON t.countrycode  = c.countrycode
	WHERE 	--t.countrycode  = c.countrycode AND 		
	termstype    = @termstype
	
	IF(@instalpredel = 'Y')
	BEGIN
		

		IF @instamount > 0 
		--		and @instamount>@deposit)	-- UAT164 greater of deposit or instalment
				and @InstalWaived=0			-- CR1090 instalment not waived	
		AND @instamount > @deposit
			--SET @deposit = @instamount + @deposit 
			SET @deposit = @instamount --IP - 11/03/11 - #3319
	END

	SELECT	@amountpaid = ISNULL(SUM(transvalue),0)
	FROM 		fintrans
	WHERE	acctno = @acctno 
	AND	(transtypecode = 'PAY' 	OR
		 transtypecode = 'COR' OR
		 transtypecode = 'REF' OR
		 transtypecode = 'RET' OR
		 transtypecode = 'SCX' OR
		 transtypecode = 'REB' OR
		 (transtypecode = 'XFR'))

	SELECT @cheqdays = cheqdays
	FROM country

	--Checking to see if a cheque payment has been made in the last 5 days
	SELECT	@payCOUNT = COUNT(*) 
	FROM 		fintrans
	WHERE	acctno = @acctno 
	AND 		datetrans + @cheqdays > GETDATE()
	AND		(paymethod%10) = 2 
	
	--IP - 21/09/10 - UAT5.2 Log - UAT(1128) - SELECT amount paid using cheque
	SET @cheqAmount = (SELECT SUM(transvalue) FROM fintrans
						WHERE acctno = @acctno
						AND datetrans + @cheqdays > GETDATE()
						AND	(paymethod%10) = 2 
						)

	--Check if Deposit/Instal pre-del not paid
	--IF(0 - @amountpaid < @deposit AND @acctType != 'S' AND @acctType != 'C' AND @InstalWaived = 0) --IP - 04/03/11 - #3275 - check acCOUNT is not qualified for first instalment waiver
	IF(0 - @amountpaid < @deposit AND @acctType != 'S' AND @acctType != 'C' AND @isReadyAssist = 0) --#18605 - CR15594 --IP - 11/03/11 - #3308 
	BEGIN
		INSERT INTO #applicationstatus  
	        	(
	            		StatusCode,
	            		StatusDescription
	        	)
	        	SELECT	code,
	               	codedescript
	        	FROM   	code
		WHERE  	code = 'INP'
		AND category = 'APS'
	END

	--Check if Awaiting Cheque Clearance
	--IF(@amountpaid >= @deposit AND @currstatus = 'U' AND @acctType != 'S' AND @acctType != 'C' AND @payCOUNT > 0)
	IF(ABS(@amountpaid) >= @deposit AND (ABS(@amountpaid - @cheqAmount)!> @deposit AND ABS(@amountpaid - @cheqAmount) <> @deposit)	--IP - 21/09/10 - UAT5.2 Log - UAT(1128)
		AND @acctType != 'S' AND @acctType != 'C' AND @payCOUNT > 0)
	BEGIN
	        	INSERT INTO #applicationstatus  
	        	(
	            		StatusCode,
	            		StatusDescription
	        	)
	        	SELECT	code,
	               	codedescript
	        	FROM   	code
		WHERE  	code = 'CHQ'
		AND category = 'APS'
	END

	--Check if Awaiting D.A
	--IF(@holdprop = 'Y' AND @acctType != 'S' AND NOT (@isLoan = 1 AND @agreementtot = @deltot AND @deltot > 0)) --IP - 22/08/08 - UAT(491) & UAT(512)
	IF(@holdprop = 'Y' AND @acctType != 'S' AND @agreementtot != @deltot  AND NOT (@isLoan = 1 AND @agreementtot = @deltot AND @deltot > 0))--IP - 15/05/12 - #9476 - LW74503 --IP - 22/08/08 - UAT(491) & UAT(512)
	BEGIN
		IF ISNULL(@InstantCredit,'N')!='Y' --IP - 01/03/11 - #3242 - Added ISNULL check
		BEGIN			
	        	INSERT INTO #applicationstatus  
	        	(
	            		StatusCode,
	            		StatusDescription
	        	)
	        	SELECT	code,
	               	codedescript
	        	FROM   	code
		WHERE  	code = 'ADA'
		AND category = 'APS'		
		END
		ELSE
		-- CR1225 Application Status ADI for instant credit
		BEGIN
			INSERT INTO #applicationstatus (StatusCode,StatusDescription)
	        	SELECT	code,codedescript
	        	FROM   	code
				WHERE  	code = 'ADI' AND category = 'APS'
		END
	END

	--Check if Lineitems exist
        	SELECT	@lineCOUNT = COUNT(*)
       	 FROM 		LINEITEM L
		 INNER JOIN STOCKITEM S ON L.ItemId    = S.ItemId
	        	AND    		L.StockLocn = S.StockLocn
        	WHERE 	L.AcctNo    = @acctno
        	--AND    		L.ItemId    = S.ItemId
        	--AND    		L.StockLocn = S.StockLocn
        	AND    		iskit = 0
			-- #12863 exclude cancelled/collected lineitems & Tax
			--and		s.iupc !='STAX'	
			AND		s.iupc NOT IN ('STAX', 'DEL', 'DT')	 --#13658	--#13349	
			--and		(l.quantity!=0 or l.delqty!=0)
			AND		(l.quantity > 0 OR l.delqty > 0)	 --#13658


		--Check if there are any items currently Scheduled  -- #13658
		SELECT @LineSchedCOUNT = COUNT(*)
		FROM Lineitem l INNER JOIN LineItemBookingSchedule ls ON l.id = ls.LineItemID
		WHERE ls.Quantity != 0 AND l.delqty!=0			-- #15000
		AND l.acctno =  @acctno

	IF(@lineCOUNT = 0 AND @LineSchedCOUNT = 0)			-- #13658
	BEGIN
	        	INSERT INTO #applicationstatus  
	        	(
	            		StatusCode,
	            		StatusDescription
	        	)
	        	SELECT	code,
	               	codedescript
	        	FROM   	code
		WHERE  	code = 'NOL'
		AND category = 'APS'
	END

	
	
	-- Delivery Authorise for Cash Loan -- jec CR1232
	IF (@holdprop='N' AND @isLoan=1 AND @agreementtot!=@deltot)
	BEGIN
		INSERT INTO #applicationstatus (StatusCode, StatusDescription)
        SELECT	code,codedescript
       	FROM   	code
		WHERE  	code = 'DAD'
		AND category = 'APS'
	END
	
	
	
	--IP - 25/05/10 - UAT(224) UAT5.2.1.0 Log
	IF EXISTS(SELECT TOP 1 'a' FROM scheduleaudit sa WHERE sa.acctno = @acctno AND sa.delorcoll = 'D'
			AND NOT EXISTS (SELECT TOP 1 'a' FROM scheduleremoval sr
								WHERE sr.acctno = sa.acctno
								AND sr.buffno = sa.buffno)
			AND EXISTS(SELECT TOP 1 'a' FROM schedule WHERE acctno = @acctno))
		INSERT INTO #applicationstatus
		values(
		'DEP',
		'Partial Delivery'
		)
	
			
	
	
	--Using a cursor to loop through Lineitems to see if any items are Awaiting Delivery Scheduling	
	DECLARE items_cursor CURSOR FOR 
	SELECT L.quantity, L.delqty, L.price, L.agrmtno, L.itemId, L.stocklocn, L.deliveryprocess
        	FROM 	LINEITEM L
			INNER JOIN STOCKITEM S ON L.ItemId    = S.ItemId
	        	AND    	L.StockLocn = S.StockLocn
        	WHERE 	L.AcctNo    = @acctno
        	--AND    	L.ItemId    = S.ItemId
        	--AND    	L.StockLocn = S.StockLocn
        	AND    	iskit = 0 
			AND CONVERT(VARCHAR,s.category) NOT IN (SELECT code FROM dbo.code WHERE category = 'WAR')
			--and s.itemtype !='N'
			AND (s.itemtype !='N'
					OR(s.itemtype = 'N' AND s.iupc = 'ADDDR'))								--IP - 15/05/12 - #9476 - LW74503
        	AND (L.quantity>0 OR L.delqty>0)  --#13658 - Re-instated --UAT204 removed       --UAT81 21/04/10
			--and (l.quantity!=0 or l.delqty!=0)			-- jec 16/04/13 only SELECT valid items
	
	OPEN items_cursor
	
	FETCH NEXT FROM items_cursor 
	INTO @quantity, @delqty, @price, @agrmtno, @itemId, @stocklocn,@deliveryprocess	--UAT81 
	
	WHILE @@FETCH_STATUS = 0 AND @continue = 0
	BEGIN
		-- JC/IP 4.3 merge 02/03/10
		--SELECT	@schedCOUNT = ISNULL(COUNT(*),0)
  --     	 FROM 		SCHEDULE S 
  --     	 WHERE 	S.acctno    = @acctno 
		SELECT	@schedCOUNT = ISNULL(COUNT(*),0)
       		FROM LineItem l INNER JOIN LineItemBookingSchedule lb ON l.id = lb.LineItemId	-- #12841
					WHERE	l.acctno = @acctno
					AND		l.agrmtno = @agrmtno
					AND		l.itemId = @itemId					
       	 
	
		
		--IF EXISTS (	SELECT	1
		--			FROM	schedule
		--			WHERE	acctno = @acctno
		--			AND		agrmtno = @agrmtno
		--			AND		itemId = @itemId
		--			AND		stocklocn = @stocklocn
		--			AND		delorcoll = 'C')
		IF EXISTS (	SELECT	1
					FROM LineItem l INNER JOIN LineItemBookingSchedule lb ON l.id = lb.LineItemId	-- #12841
					WHERE	l.acctno = @acctno
					AND		l.agrmtno = @agrmtno
					AND		l.itemId = @itemId
					AND lb.Quantity!=0
					AND		delorcoll = 'C')
		BEGIN
			INSERT INTO #applicationstatus  
			(
				StatusCode,
				StatusDescription
			)
		    SELECT	code,
		            codedescript
		    FROM   	code
			WHERE  	code = 'COL'
			AND category = 'APS'
			SET @continue = 1;
		END
		
		FETCH NEXT FROM items_cursor 

		INTO @quantity, @delqty, @price, @agrmtno, @itemId, @stocklocn,@deliveryprocess	--UAT81 
	END
   	CLOSE items_cursor
   	
   	--Using a cursor to check if any warranty renewals are awaiting delivery	
	DECLARE renewal_cursor CURSOR FOR 
	SELECT L.quantity, L.delqty, L.price, L.agrmtno, L.itemId
        	FROM 	LINEITEM L
			INNER JOIN STOCKITEM S ON L.ItemId    = S.ItemId
	        	AND    	L.StockLocn = S.StockLocn
			INNER JOIN WARRANTYRENEWALPURCHASE W ON L.AcctNo    = W.acctno
        		AND    	L.ItemId    = W.ItemID
        		AND    	L.StockLocn = W.StockLocn
        	WHERE 	L.AcctNo    = @acctno
        	--AND    	L.AcctNo    = W.acctno
        	--AND    	L.ItemId    = W.ItemID
        	--AND    	L.StockLocn = W.StockLocn
        	--AND    	L.ItemId    = S.ItemId
        	--AND    	L.StockLocn = S.StockLocn
        	AND    	S.IUPC != 'DT'
	
	OPEN renewal_cursor
	
	FETCH NEXT FROM renewal_cursor 
	INTO @quantity, @delqty, @price, @agrmtno, @itemId
	
	WHILE @@FETCH_STATUS = 0 AND @continue = 0
	BEGIN
		IF(@quantity > @delqty AND @price != 0)
		BEGIN
		        	INSERT INTO #applicationstatus  
		        	(
		            		StatusCode,
		            		StatusDescription
		       	)
		        	SELECT	code,
		               	codedescript
		       	 FROM   	code
			WHERE  	code = 'WRD'
			AND category = 'APS'
			SET @continue = 1;
		END
		
		FETCH NEXT FROM renewal_cursor 

		INTO @quantity, @delqty, @price, @agrmtno, @itemId
	END
   	CLOSE renewal_cursor
   	--DEALLOCATE renewal_cursor


	--Using a cursor to loop through Lineitems to see if Goods Delivered
	SET @continue = 0
	OPEN items_cursor
	
	FETCH NEXT FROM items_cursor 
	INTO @quantity, @delqty, @price, @agrmtno, @itemId, @stocklocn,@deliveryprocess	--UAT81 
	
	WHILE @@FETCH_STATUS = 0 AND @continue = 0
	BEGIN
		SELECT	@delCOUNT = COUNT(*)
		FROM 		DELIVERY
		WHERE 	itemId = @itemId
		AND		acctno = @acctno
		AND		agrmtno = @agrmtno

		IF(@delCOUNT > 0)
		BEGIN
		       	 INSERT INTO #applicationstatus  
		        	(
		            		StatusCode,
		            		StatusDescription
		       	)
		        	SELECT	code,
		               	codedescript
		       	 FROM   	code
			WHERE  	code = 'DEL'
			AND category = 'APS'
			SET @continue = 1
		END
		FETCH NEXT FROM items_cursor 
		INTO @quantity, @delqty, @price, @agrmtno, @itemId, @stocklocn,@deliveryprocess	--UAT81 
	END
   	CLOSE items_cursor
   	DEALLOCATE items_cursor
   	
   	OPEN renewal_cursor
	FETCH NEXT FROM renewal_cursor 
	INTO @quantity, @delqty, @price, @agrmtno, @itemId
	
	WHILE @@FETCH_STATUS = 0 AND @continue = 0
	BEGIN
		SELECT	@delCOUNT = COUNT(*)
		FROM 		DELIVERY
		WHERE 	itemId = @itemId
		AND		acctno = @acctno
		AND		agrmtno = @agrmtno

		IF(@delCOUNT > 0)
		BEGIN
			DELETE FROM #applicationstatus WHERE StatusCode = 'WRD'
		    
		    INSERT INTO #applicationstatus  
		    (
				StatusCode,
		        StatusDescription
		    )
		    SELECT	code,
		            codedescript
		    FROM   	code
			WHERE  	code = 'DEL'
			AND category = 'APS'
			SET @continue = 1
		END
		FETCH NEXT FROM renewal_cursor 
		INTO @quantity, @delqty, @price, @agrmtno, @itemId
	END
   	CLOSE renewal_cursor
   	DEALLOCATE renewal_cursor
   	
   	--IP - 18/03/11 - #2578 - Show goods delievered for Cash Loan acCOUNTs
   	IF(@isLoan = 1 AND @agreementtot = @deltot AND @deltot > 0)
   	BEGIN
   		 INSERT INTO #applicationstatus  
		    (
				StatusCode,
		        StatusDescription
		    )
		    SELECT	code,
		            codedescript
		    FROM   	code
			WHERE  	code = 'DEL'
			AND category = 'APS'
   	END

   --SELECT	@schedCOUNT = ISNULL(COUNT(*),0)
   --    	 FROM 		SCHEDULE S ,LINEITEM L
   --    	 WHERE 	L.acctno    = @acctno 
   --      AND L.ITEMID = S.ITEMID AND L.STOCKLOCN = S.STOCKLOCN and l.acctno = s.acctno
   --      and  picklistnumber =0
   --      and s.delorcoll='D'		-- UAT89 jec
   SELECT	@schedCOUNT = ISNULL(COUNT(*),0)						-- #12841
		FROM LineItem l INNER JOIN LineItemBookingSchedule lb on l.id = lb.LineItemId	-- #12841
					WHERE	l.acctno = @acctno
					AND		l.agrmtno = @agrmtno
					AND		l.itemId = @itemId
					AND		lb.Quantity!=0      	
         AND lb.delorcoll='D'
         
	--Check if Deliveries Scheduled
	IF(@schedCOUNT > 0)
	BEGIN
		IF NOT EXISTS(SELECT TOP 1 'a' FROM #applicationstatus WHERE StatusCode = 'DAD')
	        	INSERT INTO #applicationstatus  
	       	 (
	            		StatusCode,
	           		 StatusDescription
	       	 )
	        	SELECT	code,
	               	codedescript
	       	 FROM   	code
		WHERE  	code = 'DAD'
		AND category = 'APS'
	END


	--IP - 28/04/09 - CR929 & 974 - Application Status 'RFL' (Removed FROM Load).
	--When a Delivery Note has been removed FROM a load.

	SELECT	 @schedCOUNT = ISNULL(COUNT(*),0)
	FROM	 lineitem l INNER JOIN schedule s
	ON		 l.acctno = s.acctno
	AND		 l.itemId = s.itemId
	AND		 l.stocklocn = s.stocklocn
	INNER JOIN scheduleremoval sr
	ON		 s.acctno = sr.acctno
	AND		 s.itemId = sr.itemId
	AND		 s.stocklocn = sr.stocklocn
	WHERE	 l.deliveryprocess = 'S'
	AND		 s.picklistnumber = 0
	AND		 s.loadno = 0
	AND		 l.acctno = @acctno
	AND ISNULL(s.vanno,'') !='DHL'		-- Malaysia 3PL - jec 09/03/10 not if 3PL
		
	IF(@schedCOUNT > 0)
	BEGIN	
		INSERT INTO #applicationstatus  
	    (
			StatusCode,
	        StatusDescription
	    )
		SELECT	code,
	           	codedescript
	    FROM   	code
		WHERE  	code = 'RFL'
		AND category = 'APS'
	END

	--IP - 28/04/09 - CR929 & 974 - Application Status 'DNR' (Delivery Note Rescheduled/Reloaded).
	--When a Delivery Note previously removed has been rescheduled/reloaded.

	SELECT	 @schedCOUNT = ISNULL(COUNT(*),0)
	FROM	 lineitem l INNER JOIN schedule s
	ON		 l.acctno = s.acctno
	AND		 l.itemId = s.itemId
	AND		 l.stocklocn = s.stocklocn
	INNER JOIN scheduleremoval sr
	ON		 s.acctno = sr.acctno
	AND		 s.itemId = sr.itemId
	AND		 s.stocklocn = sr.stocklocn
	WHERE	 l.deliveryprocess = 'S'
	AND		 s.loadno != 0
	AND		 l.acctno = @acctno
		
	IF(@schedCOUNT > 0)
	BEGIN	
		INSERT INTO #applicationstatus  
	    (
			StatusCode,
	        StatusDescription
	    )
		SELECT	code,
	           	codedescript
	    FROM   	code
		WHERE  	code = 'DNR'
		AND category = 'APS'
	END
	
		--IP - 18/07/12 - #10388
	IF EXISTS(SELECT TOP 1 'a' FROM lineitem l INNER JOIN lineitembooking lb ON l.id = lb.LineItemID
				INNER JOIN lineitembookingschedule ls on lb.ID = ls.BookingId
				WHERE l.acctno = @acctno
				AND ls.quantity !=0 AND l.delqty!=0)		-- #15000
	BEGIN
		INSERT INTO #applicationstatus
		(
			StatusCode,
			StatusDescription
		)
		SELECT	code,
	           	codedescript
	    FROM   	code
		WHERE  	code = 'SCD'
		AND category = 'APS'
	END
	
	--IP - 18/07/12 - #10388
	IF EXISTS(SELECT TOP 1 'a' FROM lineitem l INNER JOIN lineitembooking lb ON l.id = lb.LineItemID
				INNER JOIN lineitembookingfailures lf ON lf.OriginalBookingID = lb.ID
				WHERE l.acctno = @acctno
				AND lf.Actioned IS NULL)
	BEGIN
		INSERT INTO #applicationstatus
		(
			StatusCode,
			StatusDescription
		)
		SELECT	code,
	           	codedescript
	    FROM   	code
		WHERE  	code = 'FLD'
		AND category = 'APS'
	END
	
END -- if credit not refused
	SELECT	@revCOUNT = COUNT(*)
	FROM	reverse_cancellation
	WHERE	acctno = @acctno
	
	IF(@revCOUNT > 0)
	BEGIN
		INSERT INTO #applicationstatus  
	        	(
	            		StatusCode,
	            		StatusDescription
	        	)
	        	SELECT	code,
	             		codedescript
	        	FROM   	code
	       	WHERE  	code = 'ROP'
	       	AND category = 'APS'
	END




	SELECT	@canCOUNT = ISNULL( COUNT(*),0)
	FROM		CANCELLATION
	WHERE	acctno = @acctno
		
	--Check if acCOUNT cancelled
	IF(@canCOUNT > 0 AND @revCOUNT=0)
	BEGIN

       DELETE FROM #applicationstatus
		INSERT INTO #applicationstatus  
	        	(
	            		StatusCode,
	            		StatusDescription
	        	)
	        	SELECT	code,
	             		codedescript
	        	FROM   	code
	       	WHERE  	code = 'CAN'
	       	AND category = 'APS'
	END
 
	IF @accttype ='C' 
	BEGIN
		IF EXISTS (SELECT TOP 1 'a'  FROM ServiceCHARgeAcct WHERE AcctNo = @acctno)  --#17083
		BEGIN
		
		 DELETE FROM #applicationstatus
		IF @currstatus !='S'
			INSERT INTO #applicationstatus  
	        		(          		StatusCode,
	            			StatusDescription     	)
			VALUES ( 'SRC',
					'Service Request CHARge')
		ELSE 
			INSERT INTO #applicationstatus  
	        		(          		StatusCode,
	            			StatusDescription     	)
			VALUES ( 'SRS',
					'Service Request Settled')
		END 
	END
 

  
	IF EXISTS (SELECT TOP 1 'a'  FROM countryMaintenance
			   WHERE CodeName = 'LoyaltyScheme'
			   AND value = 'TRUE') AND EXISTS (SELECT TOP 1 'a'  FROM Loyalty
			                                   WHERE Loyalty.LoyaltyAcct = @acctno)
     BEGIN
	 	IF (SELECT SUM(transvalue) FROM fintrans
	 	    WHERE fintrans.acctno = @acctno) > 0
	 	    BEGIN
			 	SELECT 'PAY' AS StatusCode,'Home Club Payment Required' AS StatusDescription
			 END
			 ELSE
			 BEGIN
			 	IF EXISTS (SELECT TOP 1 'a'  FROM loyalty
							WHERE LoyaltyAcct = @acctno
							AND statusacct = 1)
	              BEGIN
					SELECT 'HCM' AS StatusCode,'Home Club Member' AS StatusDescription 
				  END  
		       ELSE
		          BEGIN
					IF EXISTS (SELECT TOP 1 'a'  FROM loyalty
							   WHERE LoyaltyAcct = @acctno
							   AND statusacct = 2
							   AND Enddate = (SELECT MAX(enddate) FROM Loyalty
							                  WHERE LoyaltyAcct = @acctno))
						BEGIN
							SELECT 'CHC' AS StatusCode,'Home Club Membership Cancelled' AS StatusDescription	
						END
						ELSE
						BEGIN
							SELECT 'NHC' AS StatusCode,'No Home Club Membership' AS StatusDescription 
						END
					END
			 END
	 END

	 INSERT INTO #applicationstatus
	 SELECT 'PSN', 'Shipment: ' 
		+ CASE 
		WHEN Exception  LIKE '%pk_booking%' THEN 'Primary Key'
		WHEN Exception  LIKE '%solr%' THEN 'Solr Connection Error - Retry'
		WHEN Exception  LIKE '%deadlock%' THEN 'Deadlock - Retry'
		WHEN Exception  LIKE '%timeout%' THEN 'Timeout - Retry'
		ELSE SUBSTRING(Exception, 1, 190)
		END
	 FROM [hub].[QueueMessage] QM
     INNER JOIN [Hub].[Message] M ON QM.MessageId = M.Id
	 WHERE CorrelationId = @acctno AND 
			[state] = 'P' AND
			queueid = 1

	 
	  INSERT INTO #applicationstatus
	 SELECT 'PSN', 'Delivery: ' 
			+ CASE 
				WHEN Exception LIKE '%LineitemBookingSchedule do not match%' THEN 'Mismatch between booking and delivery'
				WHEN Exception  LIKE '%exceeds agreement total%' THEN SUBSTRING(Exception, 1, 95)
				WHEN Exception  LIKE '%deadlock%' THEN 'Deadlock - Retry'
				WHEN Exception  LIKE '%timeout%'THEN 'Timeout - Retry'
			ELSE SUBSTRING(Exception, 1, 190)
			END
		 FROM [hub].[QueueMessage] QM
         INNER JOIN [Hub].[Message] M ON QM.MessageId = M.Id
		 WHERE CorrelationId = @acctno AND 
				[state] = 'P' AND
				queueid = 3

	INSERT INTO #applicationstatus
	SELECT 'PSN',  CASE WHEN queueid = 2 THEN 'Cancel Shipment FROM CoSACS: ' 
				ELSE 'Cancel Shipment FROM Logistics: 'END 
			+  SUBSTRING(Exception, 1, 160)
		 FROM [hub].[QueueMessage] QM
         INNER JOIN [Hub].[Message] M ON QM.MessageId = M.Id
		 WHERE CorrelationId = @acctno AND 
				[state] = 'P' AND
				queueid IN (2,4)


	 
	 SELECT * FROM #applicationstatus

 
	

	--Drop temp table
	DROP TABLE #applicationstatus

	SET @return = @@ERROR
	
GO


