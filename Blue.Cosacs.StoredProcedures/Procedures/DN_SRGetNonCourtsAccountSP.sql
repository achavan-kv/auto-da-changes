SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

if exists (select * from dbo.sysobjects
where id = object_id('[dbo].[DN_SRGetNonCourtsAccountSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_SRGetNonCourtsAccountSP]
GO

--exec DN_SRGetNonCourtsAccountSP '900','','TEST98',0

CREATE PROCEDURE dbo.DN_SRGetNonCourtsAccountSP
-- =============================================
-- Author:		????
-- Create date: ??
-- Title:	DN_SRGetNonCourtsAccountSP
--
--	This procedure will retrieve details of a Non Courts SR
-- 
-- Change Control
-----------------
-- 10/11/10 jec CR1030 - return Retailer 
-- 10/01/11 jec CR1030 - return ReAssign technician code
-- 12/01/11 jec CR1030 - return ReAssignedBy 
-- 09/02/11 ip - Sprint 5.10 - #2977 - Return new column ChargeAcctWrittenOff to indicate a Service Requests Charge Account has been 
--									  written off
-- 15/02/11 ip  -Sprint 5.10 - #2977 - Changed transtype from SDW to WOS
-- 23/06/11 jec #3969 - CR1254 Service request product code shows IUPC & return CourtsItemNo
-- 01/07/11 IP CR1254 - RI - #3994 - Return PartID when returning parts list
-- 10/01/12 IP #9407 - Merge issue fixed. ItemID was previously removed from select list.
-- 30/01/13 jec #10380 - LW75101 - Cash & Go SR _No invoice
-- =======================================================
	-- Add the parameters for the function here
    @ServiceBranchNo    SMALLINT,
    @ServiceUniqueId    INTEGER,
    @CustId             VARCHAR(20),
    @Return             INTEGER OUTPUT

AS
    SET NOCOUNT ON
    SET @Return = 0

    -- A NON Courts Account Service Request can loaded by either:
    -- . Service Request Number
    -- . Customer Id

    IF (@ServiceUniqueId > 0)
    BEGIN
        -- Find the customer from the Service Request Number
        SELECT @CustId = CustId
        FROM   SR_ServiceRequest
        WHERE  ServiceBranchNo = @ServiceBranchNo
        AND    ServiceRequestNo = @ServiceUniqueId
        AND    (ServiceType = 'N' OR ServiceType = 'G')   -- NON Courts Account or Cash and Go CR949/958	
    END


    -- List all existing Service Requests for this customer
    -- The products might NOT be on the Courts Stock Item table
    SELECT
        sr.ServiceBranchNo,
        CONVERT(VARCHAR,sr.ServiceBranchNo) + CONVERT(VARCHAR,sr.ServiceRequestNo) AS ServiceRequestNoStr,
        sr.ServiceRequestNo,
        CONVERT(VARCHAR,sr.DateLogged) AS DateLoggedStr,
        sr.DateLogged,
        sr.Status,
        sr.StockLocn,
        sr.ActionRequired,							-- CR 949/958
        --sr.ProductCode,
        ISNULL(s.IUPC,sr.ProductCode) AS ProductCode,				-- RI jec
        ISNULL(s.ItemNo,'') as CourtsCode,					-- RI jec 
        sr.UnitPrice,
        1 AS Quantity,
        1 AS MultipleQuantity,
        sr.Sequence,
        ISNULL(s.ItemDescr1, sr.Description) AS Description,
        ISNULL(s.ItemDescr2, '') AS ItemDescr2,
		ISNULL(c.category,'') AS Category,
        sr.ContractNo,
        CONVERT(CHAR(12),'') AS DeliveryNoteNumber,
		case when ServiceType = 'G' then sr.PurchaseDate else CONVERT(CHAR(12),'') end AS DeliveryDate,		-- #10380
        sr.ExtWarranty,
        sr.RefCode,
        CONVERT(CHAR(12),'') AS AcctNo,
        CASE WHEN SR.ServiceType = 'G' THEN sr.InvoiceNo ELSE CONVERT(INTEGER,0) END AS InvoiceNo, --IP - 14/11/08 - If the Service Request type is 'G' then return the 'Invoice Type'.
        sr.CustId,
        CASE WHEN @ServiceUniqueId > 0 AND sr.ServiceType = 'N' THEN 'NSR' ELSE ISNULL(sr.ServiceType,'') END AS ServiceType,
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
		--//CR1030 - needs to be included with Reports Release
		--sr.softscriptdate,
        sr.DateCollected,
        ISNULL(a.OutstBal,0) AS OutstBal,
		CAST (0.00 AS MONEY) AS PreviousCosts,
		CAST('N' AS CHAR(1)) AS Exchanged,       --UAT 381 Extra field required to mark item as exchanged
		CAST('N' AS CHAR(1)) AS Repossessed,     --UAT 380 Extra field required to mark item as repossessed
		CAST('N' AS CHAR(1)) AS Replaced,          --UAT 381 Extra field required to mark item as replaced
		ISNULL(sr.History,'N') AS History,       --UAT 367 New fields required for Customer Audit Details
		ISNULL(sr.LinkedSR,0) AS LinkedSR,
		ISNULL(sr.Updated,0) AS Updated,
		sr.PrintLocn,							-- CR 949/958
		sr.LbrCostEstimate, --CR 1024 (NM 29/04/2009)
		sr.AdtnlLbrCostEstimate, --CR 1024 (NM 29/04/2009)
		sr.TransportCostEstimate , --CR 1024 (NM 29/04/2009)
		sr.TechnicianReport,  --CR 1024 (NM 29/04/2009)
		ISNULL(sr.Retailer,'') as Retailer,			--CR1030 jec
		0 as ChargeAcctWrittenOff,					--IP - 09/02/11 - Sprint 5.10 - #2977
		sr.ItemId,									--IP - 10/01/12 - #9407 -- RI jec
		isnull(s.CostPrice,0) as CostPrice,			--IP - 17/08/11 - #4574 - UAT29
		CAST (0.00 AS MONEY) AS PreviousCostsEW		--IP - 16/09/11 - #8153 - UAT29 - Previous costs charged to Extended Warranty
		
    INTO #ProductList
    FROM StockItem s RIGHT OUTER JOIN SR_ServiceRequest sr    
	ON      s.StockLocn = sr.StockLocn
    --AND     s.ItemNo = sr.ProductCode
    and sr.ItemId=s.ItemId				-- RI
    ---------AND     case when sr.ServiceType!='N' then sr.ItemId=s.ItemId else  s.ItemNo = sr.ProductCode
	LEFT OUTER JOIN Code c
	ON CAST(s.category AS VARCHAR(12)) = c.code
	LEFT OUTER JOIN SR_ChargeAcct ca ON sr.ServiceRequestNo = ca.ServiceRequestNo AND ca.ChargeType = 'C'
	LEFT OUTER JOIN Acct a ON a.AcctNo = ca.AcctNo	
    WHERE   sr.CustId = @CustId
    AND     (sr.ServiceType = 'N' or sr.ServiceType = 'G')   -- NON Courts Account	or Cash and Go CR949/958	
    AND		(c.category = 'PCE' OR c.category = 'PCF' OR c.category = 'PCW' OR c.category is null)
    
	-- Include total of all previous costs for specific product

	SELECT SUM(TotalCost) AS PreviousCosts ,ProductCode 
	INTO #Previous FROM SR_Resolution R JOIN SR_ServiceRequest S ON R.ServiceRequestNo = S.ServiceRequestNo 
	WHERE CustId IN (SELECT TOP 1 CustID FROM #ProductList) AND History = 'Y' 
	GROUP BY ProductCode,CustId 
	
	--IP - 16/09/11 - #8153 - UAT29 - Select the previous repair totals charged to EW(Extended Warranty)
	SELECT SUM(TotalCost) AS PreviousCostsEW ,ProductCode ,SerialNo
	INTO #PreviousEW FROM SR_Resolution R JOIN SR_ServiceRequest S ON R.ServiceRequestNo = S.ServiceRequestNo 
	WHERE CustId = (SELECT TOP 1 CustID FROM #ProductList WHERE CustID <> '') 
	AND R.ChargeTo = 'EW'
	GROUP BY ProductCode,CustId,SerialNo 

	UPDATE #ProductList
	SET		PreviousCosts = P.PreviousCosts
	FROM	#Previous P
	WHERE	 #ProductList.ProductCode = P.ProductCode
	
	
	--IP - 16/09/11 - #8153 - UAT29 - Update the #ProductList table with the previous repair costs for EW (Extended Warranty)
	UPDATE #ProductList
	SET		PreviousCostsEW = P.PreviousCostsEW
	FROM	#PreviousEW P
	WHERE	 #ProductList.ProductCode = P.ProductCode AND #ProductList.SerialNo = P.SerialNo
	
	--IP - 09/02/11 - Sprint 5.10 - #2977
     UPDATE #ProductList
     SET ChargeAcctWrittenOff = 1
     WHERE EXISTS(select * from SR_ChargeAcct sca
					inner join fintrans f on sca.acctno = f.acctno
					where f.transtypecode = 'WOS' --IP - 15/02/11 - Change to WOS as SDW already used
					and	sca.ServicerequestNo = #ProductList.ServiceRequestNo)
					
	
	----Check for food loss	CR1030 jec
	--UPDATE #ProductList
	--	set FoodLoss='Y'
	--From SR_FoodLoss f
	--where #ProductList.ServiceRequestNo=f.ServiceRequestNo
	
    -- Return the list of SRs with the Allocation and Resolution
    SELECT  sr.*,
			a.DateAllocated,
			a.Zone,
			a.TechnicianId,
			a.PartsDate,
			a.RepairDate,
			a.IsAM,
			a.Instructions,
			ISNULL(a.ReAssignCode,'') AS ReAssignCode,		--CR1030 jec
			ISNULL(a.ReAssignedBy,'0')          AS ReAssignedBy,		--CR1030 jec
			r.DateClosed,
			r.Resolution,
			r.ResolutionChangedBy,
			r.ChargeTo,
			r.ChargeToChangedBy,
			r.ChargeToMake,
			r.ChargeToModel,
			r.HourlyRate,
			r.Hours,
			r.LabourCost,
			r.AdditionalCost,
			r.TotalCost,
			r.GoodsOnLoanCollected,
			r.Replacement,
			r.FoodLoss,
			r.SoftScript,
			r.Deliverer,
			r.Fault,
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
			r.TransportCost  --CR 1024 (NM 29/04/2009)	
    FROM #ProductList sr  
    --LEFT JOIN StockItem SI ON SI.ItemNo = sr.ProductCode and SI.StockLocn = sr.StockLocn, --CR 1024 (NM 23/04/2009)
    LEFT JOIN StockItem SI ON SI.ItemId = sr.ItemId and SI.StockLocn = sr.StockLocn,	-- RI 
    SR_Allocation a, SR_Resolution r
    WHERE   a.ServiceRequestNo = sr.ServiceRequestNo
    AND     r.ServiceRequestNo = sr.ServiceRequestNo
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
			sr.StockLocn, --IP - 18/06/09 - UAT(687)
			sr.PartID	--IP - 01/07/11 - CR1254 - RI - #3994
    FROM   SR_PartListResolved sr
    LEFT OUTER JOIN StockItem s
    --ON     s.ItemNo = sr.PartNo
    ON     s.ItemId = sr.PartId			-- RI
    WHERE  sr.ServiceRequestNo IN (SELECT p.ServiceRequestNo FROM #ProductList p)
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
    WHERE  sr.ServiceRequestNo IN (SELECT p.ServiceRequestNo FROM #ProductList p)
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
    
    SET @Return = @@error

    SET NOCOUNT OFF
    RETURN @Return
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End

