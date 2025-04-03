SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

-- Modified By:	Ruth McQueeney
-- Modified For:Livewire 71338 changed setting ExtWarranty to Y	


if exists (select * from dbo.sysobjects
where id = object_id('[dbo].[DN_SRGetCourtsAccountSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_SRGetCourtsAccountSP]
GO


CREATE PROCEDURE dbo.DN_SRGetCourtsAccountSP
-- =============================================
-- Author:		????
-- Create date: ??
-- Title:	DN_SRGetCourtsAccountSP
--
--	This procedure will retrieve details of a Courts SR
-- 
-- Change Control
-----------------
-- 10/11/10 jec CR1030 - return Retailer
-- 06/01/11 jec CR1030 - return ReAssign technician code
-- 12/01/11 jec CR1030 - return ReAssignedBy
-- 27/01/11 jec CR1030 - SR not returned when deliverer primary charge to = Deliverer and SR is not resolved.
--                     - Change in DN_SRLinkChargeToAccountSP also required.
-- 28/01/11 jec CR1030 - fix for above change
-- 31/01/11 ip/jc - Sprint 5.9 - #3063 - Change previously made for issue #3055 prevented rows from being returned.
-- 01/02/11 ip - Sprint 5.9 - #3068 - Prevent users from searching for other special accounts such as Service Internal, Service Stock,
--							  Service Warranty accounts.
-- 02/02/11 jec #3068 Do not allow search by supplier accounts
-- 09/02/11 ip - Sprint 5.10 - #2977 - Return new column ChargeAcctWrittenOff to indicate a Service Requests Charge Account has been 
--									  written off
-- 15/02/11 ip - Sprint 5.10 - #2977 - Changed transtype from SDW to WOS
-- 16/02/11 ip - Sprint 5.10 - #3191 - Removed products from the temporary table that have previously had SR's raised against them where there is no entry in delivery table
--							   for that product and stock location. Creating an SR against this item caused the purchase date = 01/01/1900
--							   later preventing the SR from being loaded due to a check on the purchase date.
-- 17/02/11 ip - Sprint 5.10 - #3191 - Check where ServiceType also in 'CSR'
-- 23/06/11 jec #3969 - CR1254 Service request product code shows IUPC & return CourtsItemNo
-- 01/07/11 IP CR1254 - RI - #3994 - Return PartID when returning parts list
-- 20/07/11 IP #4294 - Extended Warranty column displayed as 'N' for item with EW now out of the FYW period. WarrantyBand table WarrantyLength previously held years, now
--					   held in months.
-- 21/07/11 IP #4350 - Product rows not returned for a Courts Account due to an item in category PCO which was not being checked for.
-- 17/11/11 IP #8627 - LW74137 -  Error when opening SR for account. Joining on incorrect column. Now use WarrantyId rather than WarrantyNo since RI
-- 11/01/12 jec #9415 SR shows No Warranty for Extended Warranty products
-- =============================================
	-- Add the parameters for the function here
	@ServiceBranchNo	SMALLINT,
	@ServiceUniqueId	INTEGER,
	@AcctNo				CHAR(12),
	@InvoiceNo			INTEGER,
	@BranchNo			SMALLINT,
	@User				INT,
	@IsPaidAndTakenAcct	BIT OUTPUT, --IP - 31/07/09 - UAT(741) - added bool to check if the account being searched on is a Paid and Taken account.
    @Return             INTEGER OUTPUT

AS
	SET NOCOUNT ON
    SET @Return = 0

	-- A Courts Account Service Request can loaded by any of:
	-- . Service Request Number
	-- . Courts Account Number
	-- . Cash & Go Invoice Number

	--IP - 31/07/09 - UAT(741) - If a Paid and Taken account has been entered into the Courts search on the Service Request screen
	--then do not retrieve any details and return from the procedure.
	--If a search is made on an SR number then this procedure is executed from DN_SRGetSingleServiceRequestSP
	--If the SR searched on is an SR on a Paid and Taken item then we want to return the details.
	
	--IP - 01/02/2011 - Sprint 5.9 - #3068 - If any of the below special accounts are entered as the account number return 
	--from the procedure and display message to the user.
	declare @serviceStockAcct varchar(12),
			@serviceWarrantyAcct varchar(12),
			@serviceInternalAcct varchar(12),
			@installationsAcct varchar(12)
	
		select @serviceStockAcct = value from CountryMaintenance where codename = 'ServiceStockAccount'
		select @serviceWarrantyAcct = value from CountryMaintenance where codename = 'ServiceWarranty'
		select @serviceInternalAcct = value from CountryMaintenance where codename = 'ServiceInternal'
		select @installationsAcct = value from CountryMaintenance where codename = 'InstalChgAcct'
	
	IF (@AcctNo != '') AND (@AcctNo != '000000000000') AND (@InvoiceNo !>1)
		AND EXISTS(select * from agreement ag inner join custacct ca
				on ag.acctno = ca.acctno
				where ag.acctno = @AcctNo
				and ca.hldorjnt = 'H'
				and ca.custid like 'PAID%') 
				or @AcctNo in (@serviceStockAcct, @serviceWarrantyAcct, @serviceInternalAcct, @installationsAcct) --IP - 01/02/2011 - Sprint 5.9 - #3068 - Added check to ensure account is not one of the mentioned special accounts
				or exists (select * from code c where c.reference=@AcctNo and c.category='SRSUPPLIER') -- CR1030 #3068 jec 02/02/11 
				
	BEGIN
		SET @IsPaidAndTakenAcct = 1
		RETURN
	END
	ELSE
	BEGIN
		SET @IsPaidAndTakenAcct = 0	
	END
	
	IF (@ServiceUniqueId > 0)
	BEGIN
	    -- Find the account from the Service Request Number
		SELECT @AcctNo = AcctNo,
		       @InvoiceNo = InvoiceNo
        FROM   SR_ServiceRequest
        WHERE  ServiceBranchNo = @ServiceBranchNo
        AND    ServiceRequestNo = @ServiceUniqueId
        AND    ServiceType = 'C'    -- Courts Account
    END
	ELSE IF (@AcctNo != '') AND (@AcctNo != '000000000000')
	BEGIN
		--IP - 14/11/08 - If a Cash & Go sale was processed with a warranty 
		--and customer details were captured then another special account is created for the customer.
		--If the customer knows the special account number, they can enter this into the 'Courts Account'
		--field and search for the details which should be returned. Should not return details
		--if it is a PAID & TAKEN special account.
		IF EXISTS(select agrmtno from agreement ag inner join custacct ca
				on ag.acctno = ca.acctno
				where ag.acctno = @AcctNo
				and ca.hldorjnt = 'H'
				and ca.custid not like 'PAID%')
		BEGIN
			set @InvoiceNo = (select agrmtno from agreement ag inner join custacct ca
				on ag.acctno = ca.acctno
				where ag.acctno = @AcctNo
				and ca.hldorjnt = 'H')
		END
		ELSE
	    -- The invoice number must be one
	    SET @InvoiceNo = 1
	END
	ELSE IF (@InvoiceNo > 1)
	BEGIN
	    -- Load the account number from the invoice number
        SELECT @AcctNo = AcctNo
        FROM   Agreement
        WHERE  LEFT(AcctNo,3) = CONVERT(VARCHAR,@BranchNo)
        AND    AgrmtNo = @InvoiceNo
    END

	-- If account is currently locked by another user then return 0
	IF NOT EXISTS (SELECT * FROM AccountLocking WHERE lockedby <> @user AND acctno = @AcctNo AND lockcount > 0)
	BEGIN

    -- List all STOCK products on the account with their
    -- SR details if a previous SR exists
    SELECT
        -- The SR number and date logged needs to display as blank when there is no SR
        ISNULL(sr.ServiceBranchNo,0) AS ServiceBranchNo,
        ISNULL(CONVERT(VARCHAR,sr.ServiceBranchNo) + CONVERT(VARCHAR,sr.ServiceRequestNo),'') AS ServiceRequestNoStr,
        ISNULL(sr.ServiceRequestNo,0) AS ServiceRequestNo,
        ISNULL(CONVERT(VARCHAR,sr.DateLogged),'') AS DateLoggedStr,
        ISNULL(sr.DateLogged,'') AS DateLogged,
        LTRIM(ISNULL(sr.Status,'')) AS Status,     -- LTRIM because CHAR(1) can return single space
        l.StockLocn,
        --l.ItemNo AS ProductCode,
        s.IUPC AS ProductCode,				-- RI jec
        s.ItemNo as CourtsCode,			-- RI jec 
        l.Price AS UnitPrice,
        l.Quantity,
		l.Quantity AS MultipleQuantity,
        sr.Sequence,
        s.ItemDescr1 AS Description,
        s.ItemDescr2,
        ISNULL(c.category,'') AS Category,
        ISNULL(sr.ContractNo,'') AS ContractNo,
        CAST(0 AS INT) AS DeliveryNoteNumber,
        CAST('' AS DATETIME) AS DeliveryDate,
		--d.buffno AS DeliveryNoteNumber,          -- UAT 380/381 No longer joining on delivery because too many rows brought back for the same collected item
		--d.datedel AS DeliveryDate,
        ISNULL(sr.ExtWarranty,'') AS ExtWarranty,  --UAT 40 default ExtWarranty should be blank so that an ext warranty shows up on the screen when first loaded
        ISNULL(sr.RefCode,'') AS RefCode,
        l.AcctNo,
        l.AgrmtNo AS InvoiceNo,
        ISNULL(sr.CustId,'') AS CustId,
        CASE WHEN @ServiceUniqueId > 0 THEN 'CSR' ELSE ISNULL(sr.ServiceType,'') END AS ServiceType,
        ISNULL(sr.LoggedBy,0) AS LoggedBy,
        ISNULL(sr.DateReopened,'') AS DateReopened,
        ISNULL(sr.PurchaseDate,'') AS PurchaseDate,
        ISNULL(sr.ModelNo,'') AS ModelNo,
        ISNULL(sr.SerialNo,'') AS SerialNo,
        ISNULL(sr.ReceivedDate,'') AS ReceivedDate, 
        ISNULL(sr.ServiceEvaln,'') AS ServiceEvaln,
        ISNULL(sr.ServiceLocn,'') AS ServiceLocn,
        ISNULL(sr.ActionRequired,'') AS ActionRequired,				-- CR 949/958
        ISNULL(sr.RepairEstimate,0) AS RepairEstimate,
        ISNULL(sr.DeliveryDamage,'N') AS DeliveryDamage,
        ISNULL(sr.GoodsOnLoan,'N') AS GoodsOnLoan,
        ISNULL(sr.DepositAmount,-1) AS DepositAmount,
        ISNULL(sr.TransitNotes,'') AS TransitNotes,
        ISNULL(sr.Comments,'') AS Comments,
		--//CR1030 - needs to be included with Reports Release
		--ISNULL(sr.softscriptdate, '') AS SoftScriptDate,
        ISNULL(sr.DateCollected,'') AS DateCollected,
		ISNULL(a.OutstBal,0) AS OutstBal,
		CAST (0.00 AS MONEY) AS PreviousCosts,
		CAST('N' AS CHAR(1)) AS Exchanged,          --UAT 381 Extra field required to mark item as exchanged
		CAST('N' AS CHAR(1)) AS Repossessed,         --UAT 380 Extra field required to mark item as repossessed
		CAST('N' AS CHAR(1)) AS Replaced,          --UAT 381 Extra field required to mark item as replaced
		ISNULL(sr.History,'N') AS History,
		ISNULL(sr.LinkedSR,0) AS LinkedSR,
		ISNULL(sr.Updated,0) AS Updated,
		sr.PrintLocn,								-- CR 949/958
		ISNULL(sr.LbrCostEstimate,0) AS LbrCostEstimate, --CR 1024 (NM 29/04/2009)         --IP - 06/05/09 - UAT(655) - Added ISNULL --IP - 29/06/09 - Checked UAT(655) merged from 5.1
		ISNULL(sr.AdtnlLbrCostEstimate,0) AS AdtnlLbrCostEstimate, --CR 1024 (NM 29/04/2009)	 --IP - 06/05/09 - UAT(655) - Added ISNULL --IP - 29/06/09 - Checked UAT(655) merged from 5.1
		ISNULL(sr.TransportCostEstimate,0) AS TransportCostEstimate, --CR 1024 (NM 29/04/2009)	 --IP - 06/05/09 - UAT(655) - Added ISNULL --IP - 29/06/09 - Checked UAT(655) merged from 5.1
		sr.TechnicianReport,  --CR 1024 (NM 29/04/2009)
		ISNULL(sr.Retailer,'') as Retailer,			--CR1030 jec	
		0 as ChargeAcctWrittenOff,					--IP - 08/02/11 - Sprint 5.10 - #2977
		l.ItemId,				-- RI jec
		s.CostPrice,									--IP - 17/08/11 - #4574 - UAT29
		CAST (0.00 AS MONEY) AS PreviousCostsEW		--IP - 16/09/11 - #8153 - UAT29 - Previous costs charged to Extended Warranty
		
    INTO #ProductList
    FROM StockItem s, Code c, --Delivery d, 
    LineItem l
    LEFT OUTER JOIN SR_ServiceRequest sr
    ON      sr.AcctNo = l.AcctNo
    AND     sr.InvoiceNo = l.AgrmtNo
    AND     sr.StockLocn = l.StockLocn
    AND     sr.ItemId = l.ItemId			-- RI
    --AND     sr.ProductCode = l.ItemNo
    AND     sr.ServiceType = 'C'    -- Courts Account
    --AND     sr.History = 'N'
	LEFT OUTER JOIN SR_ChargeAcct ca ON sr.ServiceRequestNo = ca.ServiceRequestNo AND (ca.ChargeType = 'C' or ca.ChargeType = 'D')
	LEFT OUTER JOIN SR_Resolution r ON sr.ServiceRequestNo = r.ServiceRequestNo 
	LEFT OUTER JOIN Acct a ON a.AcctNo = ca.AcctNo 
    WHERE   l.AcctNo = @AcctNo
    AND     l.AgrmtNo = @InvoiceNo
    AND		CAST(s.category AS VARCHAR(12)) = c.code
    --AND		(c.category = 'PCE' OR c.category = 'PCF' OR c.category = 'PCW')
    AND		(c.category = 'PCE' OR c.category = 'PCF' OR c.category = 'PCW' OR c.category = 'PCO') --IP - 21/07/11 - RI - #4350
    AND     s.ItemType = 'S'        -- Stock Item
    -- AND     l.Quantity > 0          -- Product not collected -- UAT 380 If an item is collected/cancelled and it has an SR then it must be shown
    AND     s.StockLocn = l.StockLocn
    AND     s.Id = l.ItemId				-- RI jec 	
    --AND     s.ItemNo = l.ItemNo
--	AND		d.acctno = l.acctno         -- UAT 380/381 No longer joining on delivery because too many rows brought back for the same collected item
--	AND		d.stocklocn = l.stocklocn
--	AND		d.itemno = l.itemno
--	AND		d.AgrmtNo = l.AgrmtNo
	--UAT 312 Allows for more than one charge-to account existing for a SR as a result of the Charge To being changed 
	AND (LEFT(r.ChargeTo,1) = ca.ChargeType OR (ca.ChargeType = 'C' AND r.ChargeTo <> 'DEL') OR (ca.ChargeType = 'D' and  r.ChargeTo <> 'CUS')
	--OR (ca.ChargeType IS not NULL and r.dateclosed='1900-01-01')		-- jec 27/01/11 (may have implications elsewhere??) 28/01/11
	OR (r.ChargeTo = 'DEL' and not exists(select * from sr_chargeacct ca1 --IP/JC - 31/01/11 - Sprint 5.9 - #3063 - ChargeTo = Deliverer but chargeacct not created
											where ca1.servicerequestno = sr.servicerequestno
											and ca1.chargetype = 'D'))
	OR ca.ChargeType IS NULL)
	-- query needs to take account of returned items
	-- AND l.itemno IN (SELECT itemno FROM delivery WHERE   AcctNo = @AcctNo GROUP BY itemno HAVING SUM(quantity) <> 0)

     -- UAT 380 Do not show collected items unless they have an SR
     DELETE FROM [#ProductList] WHERE Quantity = 0 AND ServiceRequestNo = 0
     
     -- UAT 380 Do not show repossessed items unless they have an SR
		 --DELETE FROM [#ProductList] WHERE ProductCode IN (SELECT itemno FROM delivery d JOIN [#ProductList] p ON d.acctno = p.acctno AND d.itemno = p.ProductCode AND d.stocklocn = p.stocklocn
		 --WHERE   d.AcctNo = @AcctNo GROUP BY itemno,d.StockLocn HAVING SUM(d.quantity) = 0) AND ServiceRequestNo = 0 
		 --AND StockLocn IN (SELECT d.StockLocn FROM delivery d JOIN [#ProductList] p ON d.acctno = p.acctno AND d.itemno = p.ProductCode AND d.stocklocn = p.stocklocn
		 --WHERE   d.AcctNo = @AcctNo GROUP BY itemno,d.StockLocn HAVING SUM(d.quantity) = 0) 
     DELETE FROM [#ProductList] WHERE ItemId IN			-- RI jec
			(SELECT d.ItemId FROM delivery d JOIN [#ProductList] p ON d.acctno = p.acctno AND d.itemId = p.ItemId AND d.stocklocn = p.stocklocn
					WHERE   d.AcctNo = @AcctNo 
					GROUP BY d.ItemId,d.StockLocn HAVING SUM(d.quantity) = 0) 
			AND ServiceRequestNo = 0 
			AND StockLocn IN (SELECT d.StockLocn 
							FROM delivery d JOIN [#ProductList] p ON d.acctno = p.acctno AND d.ItemId = p.ItemId AND d.stocklocn = p.stocklocn
							WHERE   d.AcctNo = @AcctNo 
							GROUP BY d.ItemId,d.StockLocn HAVING SUM(d.quantity) = 0)
     
     --IP - 16/02/11 - Sprint 5.10 - #3191 - Remove items that are not in the delivery table as we do not want users to be able to create SR's
     --against these items.
     DELETE FROM [#ProductList]
     FROM [#ProductList] p
     INNER JOIN lineitem l on p.acctno = l.acctno
     --and p.productcode = l.itemno
     and p.ItemId = l.ItemId			-- RI jec
     and p.stocklocn = l.stocklocn
     AND NOT EXISTS(select 1 from delivery d
					 where d.acctno = p.acctno 
					 --and d.itemno = p.productcode
					 and d.ItemId = p.ItemId			-- RI jec
					 and d.stocklocn = p.stocklocn)
	 WHERE p.servicetype IN ('C', 'CSR') --IP - 17/02/11 - #3191 check where type = 'CSR'
     
    -- UAT 40 Ensure that only one row per delivered item is shown (where a collection has taken place)
    -- code removed

    
	-- For multiple items create one row for each to be displayed at the front end before service request has been created
	-- Once a service request has been created then this should be shown on one line with the remaining items on other lines

	-- Create table by copy (removed Create Table )
	Select top 1 * into #ProductList1 from #ProductList where ServiceBranchNo<0			-- RI jec	

	DECLARE @SR INT
	DECLARE @quantity INT
	DECLARE @quantityOrig INT
	DECLARE @prod VARCHAR(8)
	DECLARE @stockLocn INT 
	DECLARE @numSR INT
	DECLARE @x INT
	DECLARE @ItemId INT

	-- code removed

	BEGIN
	DECLARE SR_Quantities CURSOR
	--FOR SELECT DISTINCT ServiceRequestNo,ProductCode,StockLocn FROM #ProductList 
	FOR SELECT DISTINCT ServiceRequestNo,ItemId,StockLocn FROM #ProductList				-- RI jec
	WHERE Quantity > 1
	
	OPEN SR_Quantities

	FETCH NEXT FROM SR_Quantities
	INTO @SR,@ItemId,@stockLocn							-- RI jec

   WHILE @@FETCH_STATUS = 0
   BEGIN
		SET @quantityOrig = (SELECT DISTINCT Quantity FROM #ProductList WHERE ServiceRequestNo = @SR AND ItemId = @ItemId AND Quantity > 1 AND StockLocn = @stockLocn)		-- RI jec

		SET @numSR = (SELECT COUNT(*) FROM #ProductList WHERE ServiceRequestNo <> 0 AND ItemId = @ItemId AND StockLocn = @stockLocn)		-- RI jec

		IF @numSR > 0
		BEGIN
		SET @quantityOrig = @quantityOrig - @numSR
		END
		ELSE
		BEGIN
		SET @quantityOrig = @quantityOrig - 1
		END
		SET @quantity = @quantityOrig

		WHILE @quantity > 0
		BEGIN
		
		INSERT INTO #ProductList1 
		SELECT TOP 1 * FROM #ProductList WHERE ServiceRequestNo = @SR AND ItemId = @ItemId AND StockLocn = @stockLocn			-- RI jec

		UPDATE #ProductList1
		SET ServiceRequestNo =  0,Sequence = NULL,ServiceRequestNoStr = '',ServiceBranchNo = 0,Status = '',DateLoggedStr = '',
				CustId = '',SerialNo = '',ServiceEvaln = '', ServiceLocn = '',Comments = '',LoggedBy = 0

		SET @quantity = @quantity - 1 
		END
		

		INSERT INTO #ProductList
		SELECT * FROM #ProductList1 

		TRUNCATE TABLE #ProductList1

		UPDATE #ProductList
		SET Quantity = 1 
		WHERE ItemId = @ItemId AND StockLocn = @stockLocn		-- RI jec

       

	FETCH NEXT FROM SR_Quantities
	INTO @SR,@ItemId,@stockLocn				-- RI jec

	END
	CLOSE SR_Quantities
	DEALLOCATE SR_Quantities
	END

	DROP TABLE #ProductList1

    -- Get the Delivery Date of the STOCK item and use this as the Purchase Date
    -- This must not be updated if it is already populated on an existing SR
    UPDATE #ProductList
    SET    PurchaseDate =
        ISNULL((SELECT MAX(d.DateDel)
                FROM   Delivery d
                WHERE  d.AcctNo = #ProductList.AcctNo
                AND    d.AgrmtNo = #ProductList.InvoiceNo
                AND    d.StockLocn = #ProductList.StockLocn
                --AND    d.ItemNo = #ProductList.ProductCode),'')
                AND    d.ItemId = #ProductList.ItemId),'')			-- RI jec
    WHERE ISNULL(PurchaseDate,'') = ''

    -- Now get Delivery Date of each item
    UPDATE #ProductList
    SET    DeliveryDate =
        ISNULL((SELECT MAX(d.DateDel)
                FROM   Delivery d
                WHERE  d.AcctNo = #ProductList.AcctNo
                AND    d.AgrmtNo = #ProductList.InvoiceNo
                AND    d.StockLocn = #ProductList.StockLocn
                --AND    d.ItemNo = #ProductList.ProductCode
                AND    d.ItemId = #ProductList.ItemId			-- RI jec
                AND d.quantity > 0),'')
    WHERE ISNULL(DeliveryDate,'') = '' 
    
    -- Now get the Delivery Note Number
    UPDATE #ProductList
    SET    DeliveryNoteNumber =
        ISNULL((SELECT TOP 1 buffno
                FROM   Delivery d
                WHERE  d.AcctNo = #ProductList.AcctNo
                AND    d.AgrmtNo = #ProductList.InvoiceNo
                AND    d.StockLocn = #ProductList.StockLocn
                --AND    d.ItemNo = #ProductList.ProductCode
                AND    d.ItemId = #ProductList.ItemId			-- RI jec
                AND d.quantity > 0
                ORDER BY datedel desc),0)
    

    -- Find any linked warranties
    SELECT
        l.AcctNo,
        l.AgrmtNo,
        l.ParentLocation,
        l.ParentItemNo,
        l.StockLocn AS WarrantyStockLocn,
        l.ItemNo    AS WarrantyItemNo,
        l.ContractNo,
        wb.WarrantyLength,
        wb.RefCode,        	
        #ProductList.PurchaseDate AS WarrantyDateDel,
        l.ItemID AS WarrantyItemId,l.ParentItemID					-- RI jec
    INTO #WarrantyList
    FROM #ProductList, LineItem l, WarrantyBand wb
    WHERE  l.AcctNo = #ProductList.AcctNo
    AND    l.AgrmtNo = #ProductList.InvoiceNo
    AND    l.ParentLocation = #ProductList.StockLocn
    --AND    l.ParentItemNo = #ProductList.ProductCode
    AND    l.ParentItemID = #ProductList.ItemID				-- RI jec
    AND    l.Quantity > 0          -- Warranty not collected
    --AND    wb.WarItemNo = l.ItemNo
    AND    wb.ItemID = l.ItemID					-- RI jec
	
	
	  UPDATE #ProductList  
      SET    ExtWarranty = 'N'  
      WHERE  ExtWarranty = '' 


    -- Update with any warranty renewals (only for AgrmtNo = 1)
    UPDATE #WarrantyList
    SET     WarrantyStockLocn = wr.StockLocn,
            WarrantyItemNo = wr.ItemNo,
            ContractNo = wr.ContractNo,
            WarrantyLength = wb.WarrantyLength,
            RefCode = wb.RefCode,
            WarrantyDateDel = ISNULL(wr.DateDelivered,'')
    FROM    #WarrantyList wl, WarrantyRenewalPurchase wr, WarrantyBand wb, LineItem l
    WHERE   wl.AgrmtNo = 1
    AND     wr.StockItemAcctNo = wl.AcctNo
    AND     wr.OriginalStockLocn = wl.WarrantyStockLocn
    AND     wr.OriginalContractNo = wl.ContractNo
    --AND     wb.WarItemNo = wr.ItemNo
    AND     wb.ItemID = wr.ItemID			-- RI jec
    AND     l.AcctNo = wr.AcctNo
    AND     l.AgrmtNo = 1
    AND     l.StockLocn = wr.StockLocn
    --AND     l.ItemNo = wr.ItemNo        -- Renewal warranty line item
    AND     l.ItemID = wr.ItemID				-- RI jec
    AND     l.ContractNo = wr.ContractNo
    AND     l.Quantity > 0              -- Renewal warranty not collected
	
	

    -- Only tick Extended if this has not been calculated before (blank)  
    UPDATE #ProductList  
    SET    ExtWarranty = 'Y',  
           RefCode = wl.RefCode,     -- 'ZZ' = Replacement Warranty  
           ContractNo = wl.ContractNo  
    FROM   #WarrantyList wl  
    -- UAT 269 If ext warranty is always to be ticked if exists then the following will require commenting out:  
    -- ISNULL(#ProductList.ExtWarranty,'') = '' & DATEADD(Year, 1, #ProductList.PurchaseDate) < GETDATE()  
    WHERE  ISNULL(#ProductList.ExtWarranty,'') = 'N'  
    AND    wl.AcctNo = #ProductList.AcctNo  
    AND    wl.AgrmtNo = #ProductList.InvoiceNo  
    AND    wl.ParentLocation = #ProductList.StockLocn  
    --AND    wl.ParentItemNo = #ProductList.ProductCode
    AND    wl.ParentItemID = #ProductList.ItemID				-- RI jec
      AND    (
				( --RM CR1051 need to add the MAN warranty period not just one year
				--DATEADD(year, (
					DATEADD(month, 																				--IP - 20/07/11 - #4294						
						--select top 1  isnull(convert(int,FirstYearWarPeriod), 1)
						ISNULL((select top 1 FirstYearWarPeriod						-- #9415 jec 1/01/12
						from warrantyband b
						where refcode = (
											select top 1 refcode
											from stockitem s 
											--where s.itemno = wl.WarrantyItemno
											where s.id = wl.WarrantyItemId				-- RI jec
											and s.stocklocn = wl.parentlocation
										)
								),12), #ProductList.PurchaseDate					-- #9415 jec 1/01/12
						) < isnull(nullif(#ProductList.datelogged, '1900-01-01'), GETDATE())  -- Not FYW  
				
				and    
				--DATEADD(Year, wl.WarrantyLength, wl.WarrantyDateDel) >= isnull(nullif(#ProductList.datelogged, '1900-01-01'), GETDATE())				AND #productlist.[status] != 'C'
				DATEADD(Month, wl.WarrantyLength, wl.WarrantyDateDel) >= isnull(nullif(#ProductList.datelogged, '1900-01-01'), GETDATE())				AND #productlist.[status] != 'C'		--IP - 20/07/11 - #4294
				) 
			OR
				( --RM CR1051 need to add the MAN warranty period not just one year
				--DATEADD(year, (	
				DATEADD(month,  																											--IP - 20/07/11 - #4294																		
						--select top 1  isnull(convert(int,FirstYearWarPeriod), 1)
						ISNULL((select top 1 FirstYearWarPeriod						-- #9415 jec 1/01/12
						from warrantyband b
						where refcode = (
											select top 1 refcode
											from stockitem s 
											--where s.itemno = wl.WarrantyItemno
											where s.id = wl.WarrantyItemId				-- RI jec
											and s.stocklocn = wl.parentlocation
										)
								),12), #ProductList.PurchaseDate			-- #9415 jec 1/01/12
						) < GETDATE()  -- Not FYW  
				
				and    
				--DATEADD(Year, wl.WarrantyLength, wl.WarrantyDateDel) >=  GETDATE()	
				DATEADD(Month, wl.WarrantyLength, wl.WarrantyDateDel) >=  GETDATE()															--IP - 20/07/11 - #4294			
				AND #productlist.[status] = 'C'
				) 	
				
			)    -- Ext Warranty  -- UAT 396 user requirement that contract no. is visible even if item is still under FYW  
  AND    NOT EXISTS (SELECT * FROM Delivery D JOIN   
      CollectionReason C ON  D.AcctNo = C.AcctNo AND   
       --D.ItemNo = C.ItemNo AND
       D.ItemId = C.ItemId AND					-- RI jec
       D.StockLocn = C.StockLocn AND   
       C.CollectionReason = 'INW'   
      WHERE D.AcctNo = @AcctNo AND   
       --D.ItemNo = wl.WarrantyItemNo AND 
       D.ItemId = wl.WarrantyItemId AND				-- RI jec
       D.ContractNo = Wl.ContractNo AND  
       D.FTNotes =  'CWRT' AND  
       D.DelorColl = 'C' AND   
       D.StockLocn = Wl.WarrantyStockLocn)  -- This is a check to see if there is an INW return  
  
     --UAT 40 If ExtWarranty is not 'Y' then should be 'N'  
     -- UPDATE #ProductList
     -- SET    ExtWarranty = 'N'
    -- WHERE  ExtWarranty = ''
    
    -- UAT 380 If item has been repossessed then set Repossessed field to 'Y' (so that FYW can be removed in business layer)
      UPDATE #ProductList
      SET    Repossessed = 'Y'
      WHERE  itemid IN 
      --(SELECT itemno FROM Delivery d JOIN [#ProductList] p ON d.AcctNo = p.acctno AND d.retitemno = p.ProductCode AND d.stocklocn = p.StockLocn
      --WHERE d.acctno = @acctno AND delorcoll = 'R')
       (SELECT d.itemid FROM Delivery d JOIN [#ProductList] p ON d.AcctNo = p.acctno AND d.itemid = p.itemid AND d.stocklocn = p.StockLocn
      WHERE d.acctno = @acctno AND delorcoll = 'R' and d.quantity < 0)		--IP - 18/08/11 - #4564 - UAT53 - Replaces above.
      
    --UAT 380 If item has been collected or repossessed then ext warranty is to be removed
    --UAT 381 If item has been exchanged or replaced then the warranty remains (if inside warranty validity period)
      DECLARE @warrantyValidityPeriod INT
      SET @warrantyValidityPeriod = (SELECT value FROM CountryMaintenance WHERE codename = 'warrantyvalidity')
      
      UPDATE #ProductList
      SET    ExtWarranty = 'N'
      WHERE  ExtWarranty = 'Y' AND (Quantity = 0 OR Repossessed = 'Y')
      --AND ProductCode NOT IN 
      --(SELECT e.itemno FROM Exchange e JOIN [#ProductList] p ON e.AcctNo = p.acctno --JOIN warrantyband w ON e.WarrantyNo = w.waritemno 
      --JOIN delivery d ON d.acctno = e.AcctNo AND d.itemno = e.WarrantyNo AND d.agrmtno = e.AgrmtNo
      AND ItemId NOT IN				-- RI jec
      (SELECT e.itemId FROM Exchange e JOIN [#ProductList] p ON e.AcctNo = p.acctno --JOIN warrantyband w ON e.WarrantyNo = w.waritemno 
      JOIN delivery d ON d.acctno = e.AcctNo AND d.itemId = e.WarrantyId AND d.agrmtno = e.AgrmtNo  
      WHERE delorcoll = 'D' AND DATEADD(MONTH,@warrantyValidityPeriod,datedel) >= CONVERT(varchar,ExchangeDate,101) AND e.acctno = @acctno) 
      
      --UPDATE #ProductList
      --SET    ExtWarranty = 'N'
      --WHERE  ExtWarranty = 'Y' AND ProductCode NOT IN 
      --(SELECT d.itemno FROM delivery d JOIN [#ProductList] p ON d.acctno = p.acctno AND d.agrmtno = p.InvoiceNo AND d.itemno = p.ProductCode
      --JOIN lineitem l ON d.acctno = l.acctno AND d.itemno = l.parentitemno AND d.agrmtno = l.agrmtno AND d.stocklocn = l.stocklocn 
      --WHERE delorcoll = 'C' AND d.acctno = @acctno AND l.quantity <> 0 AND d.itemno NOT IN 
      --(SELECT e.itemno FROM Exchange e JOIN [#ProductList] p ON e.AcctNo = p.acctno 
      --JOIN delivery d ON d.acctno = e.AcctNo AND d.agrmtno = e.AgrmtNo WHERE e.acctno = @acctno)) AND ProductCode IN 
      --(SELECT d.itemno FROM delivery d JOIN [#ProductList] p ON d.acctno = p.acctno AND d.agrmtno = p.InvoiceNo AND d.itemno = p.ProductCode
      --WHERE delorcoll = 'C' AND d.acctno = @acctno AND p.Quantity <> 0)
      
	  UPDATE #ProductList
		SET    ExtWarranty = 'N'
      WHERE  ExtWarranty = 'Y' 
		AND ItemId NOT IN (SELECT d.ItemId		-- RI jec
				FROM delivery d JOIN [#ProductList] p ON d.acctno = p.acctno AND d.agrmtno = p.InvoiceNo AND d.ItemId = p.ItemId
				JOIN lineitem l ON d.acctno = l.acctno AND d.ItemId = l.ParentItemID AND d.agrmtno = l.agrmtno AND d.stocklocn = l.stocklocn 
				WHERE delorcoll = 'C' AND d.acctno = @acctno AND l.quantity <> 0 AND d.ItemId NOT IN 
					(SELECT e.ItemId FROM Exchange e JOIN [#ProductList] p ON e.AcctNo = p.acctno 
						JOIN delivery d ON d.acctno = e.AcctNo AND d.agrmtno = e.AgrmtNo 
							WHERE e.acctno = @acctno)) AND ItemId IN 
						(SELECT d.ItemId 
							FROM delivery d JOIN [#ProductList] p ON d.acctno = p.acctno AND d.agrmtno = p.InvoiceNo AND d.ItemId = p.ItemId
							WHERE delorcoll = 'C' AND d.acctno = @acctno AND p.Quantity <> 0)
      
      -- UAT 381 If item has been replaced then set Replaced field to 'Y' so that system can remove FYW (if warranty outside warranty validity period)
      --UPDATE #ProductList
      --SET    Replaced = 'Y'
      --WHERE  ProductCode NOT IN (SELECT d.itemno FROM delivery d JOIN [#ProductList] p ON d.acctno = p.acctno AND d.agrmtno = p.InvoiceNo AND d.itemno = p.ProductCode
      --JOIN lineitem l ON d.acctno = l.acctno AND d.itemno = l.parentitemno AND d.agrmtno = l.agrmtno AND d.stocklocn = l.stocklocn 
      --WHERE delorcoll = 'C' AND d.acctno = @acctno
      --AND d.itemno NOT IN 
      --(SELECT e.itemno FROM Exchange e JOIN [#ProductList] p ON e.AcctNo = p.acctno 
      --JOIN delivery d ON d.acctno = e.AcctNo AND d.agrmtno = e.AgrmtNo WHERE e.acctno = @acctno))
      --AND Quantity <> 0 AND ProductCode IN (SELECT d.itemno FROM delivery d JOIN [#ProductList] p ON d.acctno = p.acctno AND d.agrmtno = p.InvoiceNo AND d.itemno = p.ProductCode
      --WHERE delorcoll = 'C' AND d.acctno = @acctno AND p.Quantity <> 0) AND ExtWarranty = 'N'
      
      -- UAT 381 If item has been replaced then set Replaced field to 'Y' so that system can remove FYW (if warranty outside warranty validity period)
      UPDATE #ProductList
      SET    Replaced = 'Y'
      WHERE  ItemId NOT IN (SELECT d.ItemId			-- RI jec
				FROM delivery d JOIN [#ProductList] p ON d.acctno = p.acctno AND d.agrmtno = p.InvoiceNo AND d.ItemId = p.ItemId
				JOIN lineitem l ON d.acctno = l.acctno AND d.ItemId = l.ParentItemID AND d.agrmtno = l.agrmtno AND d.stocklocn = l.stocklocn 
				WHERE delorcoll = 'C' AND d.acctno = @acctno
					AND d.ItemId NOT IN 
					(SELECT e.ItemId FROM Exchange e JOIN [#ProductList] p ON e.AcctNo = p.acctno 
						JOIN delivery d ON d.acctno = e.AcctNo AND d.agrmtno = e.AgrmtNo WHERE e.acctno = @acctno))
						AND Quantity <> 0 AND ItemId IN 
							(SELECT d.ItemId FROM delivery d JOIN [#ProductList] p ON d.acctno = p.acctno AND d.agrmtno = p.InvoiceNo AND d.ItemId = p.ItemId
							WHERE delorcoll = 'C' AND d.acctno = @acctno AND p.Quantity <> 0) AND ExtWarranty = 'N'
      
    -- UAT 381 If item has been exchanged and warranty outside warranty validity period then set exchanged field to 'Y' so that business layer can determine whether FYW exists for the item
	  UPDATE #ProductList
      SET    Exchanged = 'Y'
      WHERE  ItemId IN			-- RI jec
      (SELECT e.ItemId FROM Exchange e JOIN [#ProductList] p ON e.AcctNo = p.acctno			-- RI jec --JOIN warrantyband w ON e.WarrantyNo = w.waritemno 
      JOIN delivery d ON d.acctno = e.AcctNo AND d.ItemId = e.WarrantyId AND d.agrmtno = e.AgrmtNo		-- RI jec
      WHERE delorcoll = 'D' AND DATEADD(MONTH,@warrantyValidityPeriod,datedel) < CONVERT(varchar,ExchangeDate,101) AND e.acctno = @acctno) 
      
    -- UAT 381 If item has been exchanged and warranty inside warranty validity period then set exchanged field to 'E' so that business layer can determine whether FYW exists for the item
      UPDATE #ProductList
      SET    Exchanged = 'E'
      WHERE  ItemId IN				-- RI jec
      (SELECT e.ItemId FROM Exchange e JOIN [#ProductList] p ON e.AcctNo = p.acctno			-- RI jec --JOIN warrantyband w ON e.WarrantyNo = w.waritemno 
      --JOIN delivery d ON d.acctno = e.AcctNo AND d.ItemId = e.WarrantyNo AND d.agrmtno = e.AgrmtNo		-- RI jec
      JOIN delivery d ON d.acctno = e.AcctNo AND d.ItemId = e.WarrantyID AND d.agrmtno = e.AgrmtNo		--IP - #8627 - LW74137 -- RI jec
      WHERE delorcoll = 'D' AND DATEADD(MONTH,@warrantyValidityPeriod,datedel) >= CONVERT(varchar,ExchangeDate,101) AND e.acctno = @acctno) 
	-- Include total of all previous costs for specific product

	SELECT SUM(TotalCost) AS PreviousCosts ,ItemId ,SerialNo			-- RI jec
	INTO #Previous FROM SR_Resolution R JOIN SR_ServiceRequest S ON R.ServiceRequestNo = S.ServiceRequestNo 
	WHERE CustId = (SELECT TOP 1 CustID FROM #ProductList WHERE CustID <> '') 

--AND History = 'Y' 
	GROUP BY ItemId,CustId,SerialNo 
	
	--IP - 16/09/11 - #8153 - UAT29 - Select the previous repair totals charged to EW(Extended Warranty)
	SELECT SUM(TotalCost) AS PreviousCostsEW ,itemid ,SerialNo
	INTO #PreviousEW FROM SR_Resolution R JOIN SR_ServiceRequest S ON R.ServiceRequestNo = S.ServiceRequestNo 
	WHERE CustId = (SELECT TOP 1 CustID FROM #ProductList WHERE CustID <> '') 
	AND R.ChargeTo = 'EW'
	GROUP BY itemid,CustId,SerialNo 

	UPDATE #ProductList
	SET		PreviousCosts = P.PreviousCosts
	FROM	#Previous P
	WHERE	 #ProductList.itemid = P.itemid AND #ProductList.SerialNo = P.SerialNo
	
	--IP - 16/09/11 - #8153 - UAT29 - Update the #ProductList table with the previous repair costs for EW (Extended Warranty)
	UPDATE #ProductList
	SET		PreviousCostsEW = P.PreviousCostsEW
	FROM	#PreviousEW P
	WHERE	 #ProductList.itemid = P.itemid AND #ProductList.SerialNo = P.SerialNo

	-- Determine if Instant Replacement is valid
	IF EXISTS (SELECT RefCode FROM #WarrantyList WHERE RefCode IN ('ZZ') )
	BEGIN
		UPDATE #ProductList
		SET RefCode = wl.RefCode,     -- 'ZZ' = Replacement Warranty
           ContractNo = wl.ContractNo
		FROM   #WarrantyList wl
		WHERE  wl.AcctNo = #ProductList.AcctNo
		AND    wl.AgrmtNo = #ProductList.InvoiceNo
		AND    wl.ParentLocation = #ProductList.StockLocn
		AND    wl.ParentItemId = #ProductList.ItemId					-- RI jec
		--AND    DATEADD(Year, wl.WarrantyLength, wl.WarrantyDateDel) >= #ProductList.datelogged       
		AND    DATEADD(Month, wl.WarrantyLength, wl.WarrantyDateDel) >= #ProductList.datelogged					--IP - 20/07/11 - #4294
	END
	
	-- If searching on a service request number and that SR has been overwritten (i.e.History = 'Y') 
	-- then change the ServiceType to 'C' (Courts)
	
	IF NOT EXISTS (SELECT * FROM #ProductList WHERE ServiceRequestNo = @ServiceUniqueId)
	BEGIN
		UPDATE #ProductList
		SET ServiceType = 'C'
	END
	
	 --IP - 08/02/11 - Sprint 5.10 - #2977
     UPDATE #ProductList
     SET ChargeAcctWrittenOff = 1
     WHERE EXISTS(select * from SR_ChargeAcct sca
					inner join fintrans f on sca.acctno = f.acctno
					where f.transtypecode = 'WOS'	--IP - 15/02/11 - Sprint 5.10 - #2977 changed from SDW to WOS
					and	sca.ServicerequestNo = #ProductList.ServiceRequestNo)
					

	----Check for food loss	CR1030 jec
	--UPDATE #ProductList
	--	set FoodLoss='Y'
	--From SR_FoodLoss f
	--where #ProductList.ServiceRequestNo=f.ServiceRequestNo
	
    -- Return the list of products with the Allocation and Resolution
    SELECT  sr.*,
			ISNULL(a.DateAllocated,'')          AS DateAllocated,
			ISNULL(a.Zone,'')                   AS Zone,
			ISNULL(a.TechnicianId,0)            AS TechnicianId,
			ISNULL(a.PartsDate,'')              AS PartsDate,
			ISNULL(a.RepairDate,'')             AS RepairDate,
			ISNULL(a.IsAM,'')                   AS IsAM,
			ISNULL(a.Instructions,'')           AS Instructions,
			ISNULL(a.ReAssignCode,'')           AS ReAssignCode,		--CR1030 jec
			ISNULL(a.ReAssignedBy,'0')          AS ReAssignedBy,		--CR1030 jec
			ISNULL(r.DateClosed,'')             AS DateClosed,
			ISNULL(r.Resolution,'')             AS Resolution,
			ISNULL(r.ResolutionChangedBy,0)     AS ResolutionChangedBy,
			ISNULL(r.ChargeTo,'')               AS ChargeTo,
			ISNULL(r.ChargeToChangedBy,0)       AS ChargeToChangedBy,
			ISNULL(r.ChargeToMake,'')           AS ChargeToMake,
			ISNULL(r.ChargeToModel,'')          AS ChargeToModel,
			ISNULL(r.HourlyRate,0)              AS HourlyRate,
			ISNULL(r.Hours,0)                   AS Hours,
			ISNULL(r.LabourCost,0)              AS LabourCost,
			ISNULL(r.AdditionalCost,0)          AS AdditionalCost,
			ISNULL(r.TotalCost,0)               AS TotalCost,
			ISNULL(r.GoodsOnLoanCollected,'N')  AS GoodsOnLoanCollected,
			ISNULL(r.Replacement,'N')           AS Replacement,
			ISNULL(r.FoodLoss,'N')              AS FoodLoss,
			ISNULL(r.SoftScript,'N')            AS SoftScript,
			ISNULL(r.Deliverer,'')              AS Deliverer,
			ISNULL(r.Fault,'')					AS Fault,
			r.ReturnDate,								-- CR 949/958
			r.FailureReason,							-- CR 949/958
			r.Delivered,								-- CR 949/958
			r.CustomerCollected,						-- CR 949/958
			r.RepairedHome,								-- CR 949/958
			Case  --CR 1024 (NM 23/04/2009)	
				When SI.supplierCode is NOT NULL and SI.supplierCode != '' Then SI.supplierCode
				When SI.supplier is NOT NULL and SI.supplier != '' Then SI.supplier				 
				Else ''
			End as SupplierCode,
			ISNULL(r.TransportCost,0)  AS TransportCost --CR 1024 (NM 29/04/2009)	
    FROM #ProductList sr
    LEFT OUTER JOIN SR_Allocation a
    ON      a.ServiceRequestNo = sr.ServiceRequestNo
    LEFT OUTER JOIN SR_Resolution r
    ON      r.ServiceRequestNo = sr.ServiceRequestNo
    LEFT JOIN StockItem SI							--CR 1024 (NM 23/04/2009)
    ON sr.ItemId = SI.ItemID and sr.StockLocn = SI.StockLocn 					-- RI jec
    WHERE   ISNULL(sr.PurchaseDate,'') > CONVERT(DATETIME,'01 Jan 1900',106)
    ORDER BY sr.ServiceRequestNo DESC, sr.ProductCode ASC, sr.StockLocn ASC


    -- Return the list of Parts for each SR with a non-courts Y/N flag
    SELECT  DISTINCT
            sr.ServiceRequestNo,
            sr.PartNo,
            CONVERT(VARCHAR, sr.Quantity) AS Quantity,
            CONVERT(VARCHAR, sr.UnitPrice) AS UnitPrice,
            CONVERT(VARCHAR, sr.Quantity * sr.UnitPrice) AS Total,
            sr.Description,
            CASE WHEN s.ItemNo IS NULL THEN 'Y' ELSE 'N' END AS NonCourts,
            sr.PartType,
			sr.StockLocn, --IP - 18/06/09 - UAT(687) - return the stock location of the part.
			sr.PartID	  --IP - 01/07/11 - CR1254 - RI - #3994
    FROM   SR_PartListResolved sr
    LEFT OUTER JOIN StockItem s
    ON     s.ItemId = sr.PartId					-- RI jec
    WHERE  sr.ServiceRequestNo IN (SELECT ServiceRequestNo FROM #ProductList p
                                   WHERE  ISNULL(p.PurchaseDate,'') > CONVERT(DATETIME,'01 Jan 1900',106))
    ORDER BY sr.ServiceRequestNo, sr.PartNo


    -- Return the Charge To Analysis for each SR (if saved)
    SELECT  sr.ServiceRequestNo,
            sr.SortOrder,
            CAST ('' AS VARCHAR(15)) AS Title,
            sr.ActualCost,
            sr.Supplier,
            sr.Internal,
            sr.ExtWarranty,
            sr.Customer,
            sr.Deliverer
    INTO #ChargeTo
    FROM    SR_ChargeTo sr
    WHERE  sr.ServiceRequestNo IN (SELECT ServiceRequestNo FROM #ProductList p
                                   WHERE  ISNULL(p.PurchaseDate,'') > CONVERT(DATETIME,'01 Jan 1900',106))
    ORDER BY sr.ServiceRequestNo, sr.SortOrder

	UPDATE #ChargeTo
    SET Title = 'Parts - Courts' WHERE SortOrder = 1
    UPDATE #ChargeTo
    SET Title = 'Parts - Other' WHERE SortOrder = 2
    UPDATE #ChargeTo
    SET Title = 'Parts - Total' WHERE SortOrder = 3
    UPDATE #ChargeTo
    SET Title = 'Total Labour' WHERE SortOrder = 4
    UPDATE #ChargeTo
    SET Title = 'Total Cost' WHERE SortOrder = 5

    SELECT * FROM #ChargeTo
    
    -- UAT 383 Bring back data from CollectionReason if there is an INW return on an open SR
    
    SELECT sr.acctno,sr.ProductCode FROM Delivery D JOIN CollectionReason C 
    ON  D.AcctNo = C.AcctNo AND D.ItemId = C.ItemId AND D.StockLocn = C.StockLocn AND C.CollectionReason = 'INW'		-- RI jec
    JOIN #WarrantyList wl ON D.ItemId = wl.WarrantyItemId AND D.ContractNo = Wl.ContractNo AND D.StockLocn = Wl.WarrantyStockLocn		-- RI jec
    JOIN #ProductList sr ON sr.Acctno = D.AcctNo
	WHERE D.AcctNo = @AcctNo AND 	  
		  D.FTNotes =  'CWRT' AND
		  D.DelorColl = 'C'   AND
		  sr.ServiceRequestNo <> 0	AND
		  sr.Status <> 'C' AND 
		  sr.ExtWarranty = 'Y'	
    
	END

	ELSE

	BEGIN
	SELECT 0 AS Locked,@AcctNo AS AcctNo
	END 

    SET @Return = @@error

	SET NOCOUNT OFF
	RETURN @Return
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO


-- End End End End End End End End End End End End End End End End End End End End End End End End End End
