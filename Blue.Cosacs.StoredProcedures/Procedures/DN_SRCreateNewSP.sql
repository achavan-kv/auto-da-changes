SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

if exists (select * from dbo.sysobjects
where id = object_id('[dbo].[DN_SRCreateNewSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_SRCreateNewSP]
GO

CREATE PROCEDURE dbo.DN_SRCreateNewSP
-- =============================================
-- Author:		?
-- Create date: ?
-- Description:	Creates a new SR
-- Modified By:	Jez Hemans
-- Modified For:CR 949/958 Print Location, ActionRequired and Cash&Go service type added	
-- Modified By:	Ruth McQueeney
-- Modified For:Livewire 71338 changed setting ExtWarranty to Y	
-- Change Control
-- *****************
-- Date    Author    Change
-- 06/08/10  IP      UAT(1093) UAT5.2 - Purchase Date was not being set for a Stock Repair SR.
-- 29/06/11 jec		#3969 - CR1254 Service request product code shows IUPC 
-- 25/07/11  IP     #4387 - Ext Warranty was being set to 'N' incorrectly when creating a new SR against an item which was now in the Extended Warranty Period.
--					This was due to WarrantyBand.Warrantylength now holds months rather than previously holding years. Dateadd needed to change when updating the 
--					Ext Warranty column to use Months.
-- 11/01/12 jec #9415 SR shows No Warranty for Extended Warranty products#
-- 11/01/12 jec Fix 5.13 merge issue
-- 30/01/13 jec #10380 - LW75101 - Cash & Go SR _No invoice 
-- =============================================
 @ServiceBranchNo    SMALLINT,  
    @ServiceType        CHAR(1),  
    @CustId             VARCHAR(20),  
    @AccountNo          CHAR(12),  
    @InvoiceNo          INTEGER,  
    @User               INTEGER,  
    @StockLocn          SMALLINT,  
    @ProdCode           VARCHAR(18),
    @UnitPrice          MONEY,  
 @Description  VARCHAR(50),  
 @PurchaseDate  SMALLDATETIME,  
 @SerialNo   VARCHAR(30),  
 @PrintLocn   SMALLINT,
    @itemid				INT,					-- RI  
    @Return             INTEGER OUTPUT  
  
AS DECLARE  
    @ServiceRequestNo INTEGER,  
    @Sequence         INTEGER    
  
    SET NOCOUNT ON  
    SET @Return = 0  
    
  --  if @ServiceType !='N'
		--select @ProdCode=iupc from Stockinfo where ID=@itemid				-- RI 
		
    -- A sequence number is incremented each time the same product  
    -- item opens a new SR for the same customer or courts account  
   IF @serialno !='' -- 72107 SC 15/06/10
    BEGIN
		SELECT @Sequence = MAX(Sequence) + 1    
		FROM SR_ServiceRequest    
		WHERE CustId      = @CustId         
		 AND  AcctNo      = @AccountNo         
		 AND  InvoiceNo   = @InvoiceNo      
		 AND  StockLocn   = @StockLocn      
		 --AND  ProductCode = @ProdCode
		 AND  ((ItemID = @itemid and @ServiceType !='N')	-- RI check on Itemid if NOT NonCourts
				or (ProductCode = @ProdCode and @ServiceType ='N'))		-- RI check on product if NonCourts	     
		 AND  SerialNo    = @SerialNo  
	END
     
    SET @Sequence = ISNULL(@Sequence, 1)  
    IF (@Sequence < 1) SET @Sequence = 1  
  
    -- Create the new SR record  
    INSERT INTO SR_ServiceRequest  
       (ServiceBranchNo,  
        ServiceType,  
        Sequence,  
        CustId,  
        AcctNo,  
        InvoiceNo,  
        DateLogged,  
        LoggedBy,  
        StockLocn,  
        ProductCode,  
        UnitPrice,  
        PurchaseDate,  
        Status,  
        History,  
		SerialNo,  
		PrintLocn,
		ItemID		-- RI
		  )  
    VALUES  
       (@ServiceBranchNo,  
        @ServiceType,  
        @Sequence,  
        @CustId,  
        @AccountNo,  
        @InvoiceNo,  
        GETDATE(),  
        @User,  
        @StockLocn,  
        @ProdCode,  
        @UnitPrice,  
        '',  
        'N',  
        'N',  
		@SerialNo,  
		@PrintLocn,
		@itemid			-- RI
  )  
  
  
    SET @ServiceRequestNo = SCOPE_IDENTITY()  
  
    --UAT 367 Update previous SR's for the same product and set the LinkedSR field to the new SR  
    IF @sequence > 1  
    BEGIN  
  UPDATE SR_ServiceRequest  
        SET LinkedSR = @ServiceRequestNo  
        WHERE   CustId      = @CustId       
     AND  AcctNo      = @AccountNo       
     AND  InvoiceNo   = @InvoiceNo    
     AND  StockLocn   = @StockLocn    
     --AND  ProductCode = @ProdCode
     AND  ItemID		= @itemid				-- RI   
     AND     Sequence    < @sequence  
     AND     SerialNo    = @SerialNo  
     AND     ServiceType = 'C'  
 END     
  
    -- Add the first line of the item description  
    -- This will only be found for a Courts stock item  
    UPDATE SR_ServiceRequest  
    SET    Description = s.ItemDescr1  
    FROM   StockItem s  
    WHERE  SR_ServiceRequest.ServiceRequestNo = @ServiceRequestNo  
    AND    s.StockLocn = SR_ServiceRequest.StockLocn  
    --AND    s.ItemNo = SR_ServiceRequest.ProductCode
    AND    s.ID = SR_ServiceRequest.ItemID			-- RI 
  
 -- If Description is blank and the serviceType is non-Courts then use @Description parameter, i.e description manually input for a non-courts SR  
 IF (SELECT Description FROM SR_ServiceRequest WHERE ServiceRequestNo = @ServiceRequestNo) = ''   
 BEGIN  
 UPDATE SR_ServiceRequest  
    SET    Description = @Description  
 WHERE  ServiceRequestNo = @ServiceRequestNo AND (ServiceType ='N' OR ServiceType ='G')  
 END  
  
    -- Add the Purchase Date  
    -- This will only be found for a Courts Account with a delivery record  
    UPDATE SR_ServiceRequest  
    SET    PurchaseDate =  
        ISNULL((SELECT MAX(d.DateDel)  
                FROM   Delivery d  
                WHERE  d.AcctNo = SR_ServiceRequest.AcctNo  
                AND    d.AgrmtNo = SR_ServiceRequest.InvoiceNo  
                AND    d.StockLocn = SR_ServiceRequest.StockLocn  
                --AND    d.ItemNo = SR_ServiceRequest.ProductCode), CONVERT(DATETIME,'01 Jan 1900',106))
                AND    d.ItemId = SR_ServiceRequest.ItemID), CONVERT(DATETIME,'01 Jan 1900',106))  
   WHERE ServiceRequestNo = @ServiceRequestNo  
  
 -- If Purchase Date is blank and the serviceType is non-Courts/Cash and Go/ Stock Repair then use @PurchaseDate parameter, i.e purchase type manually input for a non-courts/Cash and Go/Stock Repair SR  
 IF (SELECT PurchaseDate FROM SR_ServiceRequest WHERE ServiceRequestNo = @ServiceRequestNo) = ''   
 BEGIN  
 UPDATE SR_ServiceRequest  
    SET    PurchaseDate = @PurchaseDate  
 WHERE  ServiceRequestNo = @ServiceRequestNo AND (ServiceType ='N' OR ServiceType ='G' OR ServiceType = 'S')  --IP - 06/08/10 - UAT(1093) UAT5.2 - included 'S' (Stock Repair)
 END  
  
    -- Find any linked warranties  
    -- This will only be found for a Courts Account with a delivery record  
    SELECT  
        l.AcctNo,  
        l.AgrmtNo,  
        l.ParentLocation,  
        --l.ParentItemNo,
        l.ParentItemId,				-- RI
        l.StockLocn AS WarrantyStockLocn,  
        --l.ItemNo    AS WarrantyItemNo,
        l.ItemId    AS WarrantyItemId,		-- RI
        l.ContractNo,  
        wb.WarrantyLength,  
        wb.RefCode,  
        sr.PurchaseDate AS DateDel  
    INTO #WarrantyList  
    FROM SR_ServiceRequest sr, LineItem l, WarrantyBand wb  
    WHERE  sr.ServiceRequestNo = @ServiceRequestNo  
    AND    l.AcctNo = sr.AcctNo  
    AND    l.AgrmtNo = sr.InvoiceNo  
    AND    l.ParentLocation = sr.StockLocn  
    --AND    l.ParentItemNo = sr.ProductCode
    AND    l.ParentItemId = sr.ItemID  
    AND    l.Quantity > 0          -- Warranty not collected  
    --AND    wb.WarItemNo = l.ItemNo
    AND    wb.ItemID = l.ItemId				-- RI
  
  
    -- Update with any warranty renewals (only for AgrmtNo = 1)  
    -- This will only be found for a Courts Account with a delivery record  
    UPDATE #WarrantyList  
    SET     WarrantyStockLocn = wr.StockLocn,  
            WarrantyItemId=wr.ItemId,		-- RI WarrantyItemNo = wr.ItemNo,  
            ContractNo = wr.ContractNo,  
            WarrantyLength = wb.WarrantyLength,  
            RefCode = wb.RefCode,  
            DateDel = ISNULL(wr.DateDelivered,'')  
    FROM    #WarrantyList wl, WarrantyRenewalPurchase wr, WarrantyBand wb, LineItem l  
    WHERE   wl.AgrmtNo = 1  
    AND     wr.StockItemAcctNo = wl.AcctNo  
    AND     wr.OriginalStockLocn = wl.WarrantyStockLocn  
    AND     wr.OriginalContractNo = wl.ContractNo  
    AND     wb.ItemId = wr.ItemId		-- RI	AND     wb.WarItemNo = wr.ItemNo  
    AND     l.AcctNo = wr.AcctNo  
    AND     l.AgrmtNo = 1  
    AND     l.StockLocn = wr.StockLocn  
    AND     l.ItemId = wr.ItemId		-- RI   AND l.ItemNo = wr.ItemNo        -- Renewal warranty line item  
    AND     l.ContractNo = wr.ContractNo  
    AND     l.Quantity > 0              -- Renewal warranty not collected  
  
  
    -- Tick Extended if within warranty period  
    -- This will only be found for a Courts Account with a delivery record  
    UPDATE SR_ServiceRequest  
    SET    ExtWarranty = 'Y',  
           RefCode = wl.RefCode,     -- 'ZZ' = Replacement Warranty  
           ContractNo = wl.ContractNo  
    FROM   #WarrantyList wl  
    WHERE  wl.AcctNo = SR_ServiceRequest.AcctNo  
    AND    wl.AgrmtNo = SR_ServiceRequest.InvoiceNo  
    AND    wl.ParentLocation = SR_ServiceRequest.StockLocn  
    --AND    wl.ParentItemNo = SR_ServiceRequest.ProductCode
    AND    wl.ParentItemId = SR_ServiceRequest.ItemID				-- RI  
    AND    (
   --DATEADD(year, (
     DATEADD(Month,  												--IP - 25/07/11 - RI - #4387
			--select isnull(convert(int,FirstYearWarPeriod), 1)
			ISNULL((select top 1 FirstYearWarPeriod						-- #9415 jec 1/01/12
			from warrantyband
			where ItemId = wl.WarrantyItemId		-- RI	waritemno = wl.WarrantyItemNo 
				and refcode =  Wl.refcode),12), wl.DateDel) < GETDATE()   	-- #9415 jec 1/01/12           --  Not FYW  
    AND       --RM - Should be AND not FYW and ExtWarranty not OR
    --DATEADD(Year, wl.WarrantyLength, wl.DateDel) >= GETDATE())    -- Ext Warranty  -- UAT 396 user requirement that contract no. is visible even if item is still under FYW  
     DATEADD(Month, wl.WarrantyLength, wl.DateDel) >= GETDATE())	--IP - 25/07/11 - RI - #4387   -- Ext Warranty  -- UAT 396 user requirement that contract no. is visible even if item is still under FYW	
    AND    NOT EXISTS (SELECT * FROM Delivery D JOIN   
      CollectionReason C ON  D.AcctNo = C.AcctNo AND   
       --D.ItemNo = C.ItemNo AND
       D.ItemId = C.ItemId AND				-- RI   
       D.StockLocn = C.StockLocn AND   
       C.CollectionReason = 'INW'   
      WHERE D.AcctNo = @AccountNo AND   
       --D.ItemNo = wl.WarrantyItemNo AND 
       D.ItemId = wl.WarrantyItemId AND			-- RI 
       D.ContractNo = Wl.ContractNo AND  
       D.FTNotes =  'CWRT' AND  
       D.DelorColl = 'C' AND   
       D.StockLocn = Wl.WarrantyStockLocn)  -- This is a check to see if there is an INW return   -- Added as part of UAT 383  
  
      
        
    -- Create the initial Allocation record  
    INSERT INTO SR_Allocation  
       (ServiceRequestNo,  
        DateAllocated,  
        Zone,  
        TechnicianId)  
    VALUES  
       (@ServiceRequestNo,  
        '',  
        '',  
        0)  
  
  
    -- Create the initial Resolution record  
    INSERT INTO SR_Resolution  
       (ServiceRequestNo,  
  Resolution,  
        ChargeTo)  
    VALUES  
       (@ServiceRequestNo,  
        '',  
        '')  
  
  
    -- Return this Service Request  
    SELECT  
  sr.ServiceBranchNo,  
        CONVERT(VARCHAR,sr.ServiceBranchNo) + CONVERT(VARCHAR,sr.ServiceRequestNo) AS ServiceRequestNoStr,  
        sr.ServiceRequestNo,  
        CONVERT(VARCHAR,sr.DateLogged) AS DateLoggedStr,  
        sr.DateLogged,  
        sr.Status,  
        sr.StockLocn,  
        sr.ActionRequired,  --CR 949/958  
        sr.ProductCode,  
        sr.UnitPrice,  
        sr.Sequence,  
        sr.Description,  
        ISNULL(s.ItemDescr2,'') AS ItemDescr2,  
        sr.ContractNo,  
        sr.ExtWarranty,  
        sr.RefCode,  
        sr.AcctNo,  
        sr.InvoiceNo,  
        sr.CustId,  
        sr.ServiceType,  
        sr.LoggedBy,  
        sr.DateReopened,  
        sr.PurchaseDate,  
        sr.ModelNo,  
        sr.SerialNo,  
        sr.ReceivedDate,  
        sr.ServiceEvaln,  
        sr.ServiceLocn,  
        sr.RepairEstimate,  
        sr.DeliveryDamage,  
        sr.GoodsOnLoan,  
        sr.DepositAmount,  
        sr.TransitNotes,  
        sr.Comments,  
        sr.DateCollected,  
        sr.History,  
        sr.LinkedSR,  
        sr.PrintLocn,  
        sr.LbrCostEstimate,									--CR 1024 (NM 29/04/2009)  
		sr.AdtnlLbrCostEstimate,							--CR 1024 (NM 29/04/2009)  
		sr.TransportCostEstimate,							--CR 1024 (NM 29/04/2009)  
		sr.TechnicianReport,								--CR 1024 (NM 29/04/2009) 
		CAST('N' AS CHAR(1)) AS Exchanged,					--IP - 18/08/11 - #4564 - UAT53 - Extra field required to mark item as exchanged
		CAST('N' AS CHAR(1)) AS Repossessed,				--IP - 18/08/11 - #4564 - UAT53 - Extra field required to mark item as repossessed
		CAST('N' AS CHAR(1)) AS Replaced,					--IP - 18/08/11 - #4564 - UAT53 - Extra field required to mark item as replaced 
		s.CostPrice,										--IP - 18/08/11 - #4564 - UAT53
		isnull(l.quantity,1) AS Quantity,			-- #10380 		--IP - 18/08/11 - #4564 - UAT53
		0 as ChargeAcctWrittenOff,							--IP - 29/09/11 - #8341 - UAT66
		CAST (0.00 AS MONEY) AS PreviousCostsEW,				--IP - 29/09/11 - #8341 - UAT66
		sr.ItemId				-- Merge issue Jec 11/01/12
    INTO #ProductList  
    FROM    SR_ServiceRequest sr
    LEFT JOIN LineItem l ON sr.acctno = l.acctno		--IP - 18/08/11 - #4564 - UAT53
    and sr.invoiceno=l.agrmtno		-- #10380 
    --AND sr.ProductCode = l.ItemNo						--IP - 18/08/11 - #4564 - UAT53
    and l.itemid=@itemid			-- #10380 
    AND sr.ItemId = l.ItemID						-- Merge issue Jec 11/01/12
    AND sr.StockLocn = l.StockLocn						--IP - 18/08/11 - #4564 - UAT53
    LEFT OUTER JOIN StockItem s  
    ON      s.StockLocn = sr.StockLocn  
    --AND     s.ItemNo = sr.ProductCode 
    AND     s.ItemId = sr.ItemId			-- RI
    WHERE   sr.ServiceRequestNo = @ServiceRequestNo  
  
  
	  --IP - 18/08/11 - #4564 - UAT53 - Below updates for Exchanged, Repossessed, Replaced taken from DN_SRGetCourtsAccountSP. Fields are required in Business Layer (BServiceRequest.CheckForWarranty)
	 -- If item has been repossessed then set Repossessed field to 'Y' (so that FYW can be removed in business layer)
      UPDATE #ProductList
      SET    Repossessed = 'Y'
      --WHERE  ProductCode IN 
      -- (SELECT itemno FROM Delivery d JOIN [#ProductList] p ON d.AcctNo = p.acctno AND d.itemno = p.ProductCode AND d.stocklocn = p.StockLocn
      WHERE  #ProductList.ItemId IN						-- Merge issue Jec 11/01/12
       (SELECT d.ItemId FROM Delivery d JOIN [#ProductList] p ON d.AcctNo = p.acctno AND d.ItemID = p.ItemId AND d.stocklocn = p.StockLocn
			WHERE d.acctno = @AccountNo AND delorcoll = 'R' and d.quantity < 0)	
      
    --If item has been collected or repossessed then ext warranty is to be removed
    --If item has been exchanged or replaced then the warranty remains (if inside warranty validity period)
      DECLARE @warrantyValidityPeriod INT
      SET @warrantyValidityPeriod = (SELECT value FROM CountryMaintenance WHERE codename = 'warrantyvalidity')
      
      UPDATE #ProductList
      SET    ExtWarranty = 'N'
      WHERE  ExtWarranty = 'Y' AND (Quantity = 0 OR Repossessed = 'Y')
      --AND ProductCode NOT IN 
      --(SELECT e.itemno FROM Exchange e JOIN [#ProductList] p ON e.AcctNo = p.acctno 
      --JOIN delivery d ON d.acctno = e.AcctNo AND d.itemno = e.WarrantyNo AND d.agrmtno = e.AgrmtNo
      AND #ProductList.ItemId NOT IN							-- Merge issue Jec 11/01/12
      (SELECT e.ItemID FROM Exchange e JOIN [#ProductList] p ON e.AcctNo = p.acctno 
			JOIN delivery d ON d.acctno = e.AcctNo AND d.ItemID = e.WarrantyID AND d.agrmtno = e.AgrmtNo  
      WHERE delorcoll = 'D' AND DATEADD(MONTH,@warrantyValidityPeriod,datedel) >= CONVERT(varchar,ExchangeDate,101) AND e.acctno = @AccountNo) 
      
      --UPDATE #ProductList
      --SET    ExtWarranty = 'N'
      --WHERE  ExtWarranty = 'Y' AND ProductCode NOT IN 
      --(SELECT d.itemno FROM delivery d JOIN [#ProductList] p ON d.acctno = p.acctno AND d.agrmtno = p.InvoiceNo AND d.itemno = p.ProductCode
      --JOIN lineitem l ON d.acctno = l.acctno AND d.itemno = l.parentitemno AND d.agrmtno = l.agrmtno AND d.stocklocn = l.stocklocn 
      --WHERE delorcoll = 'C' AND d.acctno = @AccountNo AND l.quantity <> 0 AND d.itemno NOT IN 
      --(SELECT e.itemno FROM Exchange e JOIN [#ProductList] p ON e.AcctNo = p.acctno 
      --JOIN delivery d ON d.acctno = e.AcctNo AND d.agrmtno = e.AgrmtNo WHERE e.acctno = @AccountNo)) AND ProductCode IN 
      --(SELECT d.itemno FROM delivery d JOIN [#ProductList] p ON d.acctno = p.acctno AND d.agrmtno = p.InvoiceNo AND d.itemno = p.ProductCode
      --WHERE delorcoll = 'C' AND d.acctno = @AccountNo AND p.Quantity <> 0)
      
      UPDATE #ProductList								-- Merge issue Jec 11/01/12
      SET    ExtWarranty = 'N'
      WHERE  ExtWarranty = 'Y' AND #ProductList.ItemId NOT IN 
      (SELECT d.ItemID FROM delivery d JOIN [#ProductList] p ON d.acctno = p.acctno AND d.agrmtno = p.InvoiceNo AND d.itemId = p.ItemId
			JOIN lineitem l ON d.acctno = l.acctno AND d.ItemID = l.ParentItemID AND d.agrmtno = l.agrmtno AND d.stocklocn = l.stocklocn 
      WHERE delorcoll = 'C' AND d.acctno = @AccountNo AND l.quantity <> 0 AND d.ItemID NOT IN 
		  (SELECT e.ItemID FROM Exchange e JOIN [#ProductList] p ON e.AcctNo = p.acctno 
				JOIN delivery d ON d.acctno = e.AcctNo AND d.agrmtno = e.AgrmtNo WHERE e.acctno = @AccountNo)) 
			AND #ProductList.ItemId IN 
				(SELECT d.ItemId FROM delivery d JOIN [#ProductList] p ON d.acctno = p.acctno AND d.agrmtno = p.InvoiceNo AND d.ItemID = p.ItemId
					WHERE delorcoll = 'C' AND d.acctno = @AccountNo AND p.Quantity <> 0)

      -- If item has been replaced then set Replaced field to 'Y' so that system can remove FYW (if warranty outside warranty validity period)
      --UPDATE #ProductList
      --SET    Replaced = 'Y'
      --WHERE  ProductCode NOT IN (SELECT d.itemno FROM delivery d JOIN [#ProductList] p ON d.acctno = p.acctno AND d.agrmtno = p.InvoiceNo AND d.itemno = p.ProductCode
      --JOIN lineitem l ON d.acctno = l.acctno AND d.itemno = l.parentitemno AND d.agrmtno = l.agrmtno AND d.stocklocn = l.stocklocn 
      --WHERE delorcoll = 'C' AND d.acctno = @AccountNo
      --AND d.itemno NOT IN 
      --(SELECT e.itemno FROM Exchange e JOIN [#ProductList] p ON e.AcctNo = p.acctno 
      --JOIN delivery d ON d.acctno = e.AcctNo AND d.agrmtno = e.AgrmtNo WHERE e.acctno = @AccountNo))
      --AND Quantity <> 0 AND ProductCode IN (SELECT d.itemno FROM delivery d JOIN [#ProductList] p ON d.acctno = p.acctno AND d.agrmtno = p.InvoiceNo AND d.itemno = p.ProductCode
      --WHERE delorcoll = 'C' AND d.acctno = @AccountNo AND p.Quantity <> 0) AND ExtWarranty = 'N'
      
      UPDATE #ProductList					-- Merge issue Jec 11/01/12
      SET    Replaced = 'Y'
      WHERE  #ProductList.ItemId NOT IN (SELECT d.ItemID FROM delivery d JOIN [#ProductList] p ON d.acctno = p.acctno AND d.agrmtno = p.InvoiceNo AND d.ItemID = p.ItemId
			JOIN lineitem l ON d.acctno = l.acctno AND d.ItemID = l.ParentItemID AND d.agrmtno = l.agrmtno AND d.stocklocn = l.stocklocn 
				WHERE delorcoll = 'C' AND d.acctno = @AccountNo
					AND d.ItemID NOT IN (SELECT e.ItemID FROM Exchange e JOIN [#ProductList] p ON e.AcctNo = p.acctno 
							JOIN delivery d ON d.acctno = e.AcctNo AND d.agrmtno = e.AgrmtNo WHERE e.acctno = @AccountNo))
					AND Quantity <> 0 
					AND #ProductList.ItemId IN (SELECT d.ItemID FROM delivery d JOIN [#ProductList] p ON d.acctno = p.acctno AND d.agrmtno = p.InvoiceNo AND d.ItemID = p.ItemId
						WHERE delorcoll = 'C' AND d.acctno = @AccountNo AND p.Quantity <> 0) AND ExtWarranty = 'N'
   -- -- If item has been exchanged and warranty outside warranty validity period then set exchanged field to 'Y' so that business layer can determine whether FYW exists for the item
	  --UPDATE #ProductList
   --   SET    Exchanged = 'Y'
   --   WHERE  ProductCode IN 
   --   (SELECT e.itemno FROM Exchange e JOIN [#ProductList] p ON e.AcctNo = p.acctno --JOIN warrantyband w ON e.WarrantyNo = w.waritemno 
   --   JOIN delivery d ON d.acctno = e.AcctNo AND d.itemno = e.WarrantyNo AND d.agrmtno = e.AgrmtNo 
   --   WHERE delorcoll = 'D' AND DATEADD(MONTH,@warrantyValidityPeriod,datedel) < CONVERT(varchar,ExchangeDate,101) AND e.acctno = @AccountNo) 
   
   -- If item has been exchanged and warranty outside warranty validity period then set exchanged field to 'Y' so that business layer can determine whether FYW exists for the item
	  UPDATE #ProductList				-- Merge issue Jec 11/01/12
      SET    Exchanged = 'Y'
      WHERE  #ProductList.ItemId IN 
		(SELECT e.ItemID FROM Exchange e JOIN [#ProductList] p ON e.AcctNo = p.acctno --JOIN warrantyband w ON e.WarrantyNo = w.waritemno 
				JOIN delivery d ON d.acctno = e.AcctNo AND d.ItemID = e.WarrantyID AND d.agrmtno = e.AgrmtNo 
				WHERE delorcoll = 'D' AND DATEADD(MONTH,@warrantyValidityPeriod,datedel) < CONVERT(varchar,ExchangeDate,101) AND e.acctno = @AccountNo) 
      
    -- If item has been exchanged and warranty inside warranty validity period then set exchanged field to 'E' so that business layer can determine whether FYW exists for the item
      --UPDATE #ProductList
      --SET    Exchanged = 'E'
      --WHERE  ProductCode IN 
      --(SELECT e.itemno FROM Exchange e JOIN [#ProductList] p ON e.AcctNo = p.acctno --JOIN warrantyband w ON e.WarrantyNo = w.waritemno 
      --JOIN delivery d ON d.acctno = e.AcctNo AND d.itemno = e.WarrantyNo AND d.agrmtno = e.AgrmtNo 
      --WHERE delorcoll = 'D' AND DATEADD(MONTH,@warrantyValidityPeriod,datedel) >= CONVERT(varchar,ExchangeDate,101) AND e.acctno = @AccountNo)
       
	 -- If item has been exchanged and warranty inside warranty validity period then set exchanged field to 'E' so that business layer can determine whether FYW exists for the item
	 UPDATE #ProductList				-- Merge issue Jec 11/01/12
      SET    Exchanged = 'E'
      WHERE  #ProductList.ItemId IN 
		(SELECT e.ItemID FROM Exchange e JOIN [#ProductList] p ON e.AcctNo = p.acctno --JOIN warrantyband w ON e.WarrantyNo = w.waritemno 
				JOIN delivery d ON d.acctno = e.AcctNo AND d.ItemID = e.WarrantyID AND d.agrmtno = e.AgrmtNo 
				WHERE delorcoll = 'D' AND DATEADD(MONTH,@warrantyValidityPeriod,datedel) >= CONVERT(varchar,ExchangeDate,101) AND e.acctno = @AccountNo) 
	 
    -- Return this new SR with the blank Allocation and Resolution  
    SELECT  * FROM #ProductList sr, SR_Allocation a, SR_Resolution r  
    WHERE   a.ServiceRequestNo = sr.ServiceRequestNo  
    AND     r.ServiceRequestNo = sr.ServiceRequestNo  
  
  
    SET @Return = @@error  
  
    SET NOCOUNT OFF  
    RETURN @Return  
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End
