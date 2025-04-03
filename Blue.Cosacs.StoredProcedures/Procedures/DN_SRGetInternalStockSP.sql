SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

if exists (select * from dbo.sysobjects
where id = object_id('[dbo].[DN_SRGetInternalStockSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_SRGetInternalStockSP]
GO

--exec DN_SRGetInternalStockSP '900','18236','COURTS',0

CREATE PROCEDURE dbo.DN_SRGetInternalStockSP
-- =============================================
-- Author:		????
-- Create date: ??
-- Title:	DN_SRGetInternalStockSP
--
--	This procedure will retrieve details of a Internal/NonStock SR
-- 
-- Change Control
-----------------
-- 10/11/10 jec CR1030 - return Retailer
-- 10/01/11 jec CR1030 - return ReAssign technician code & softscriptdate
-- 12/01/11 jec CR1030 - return ReAssignedBy 
-- 21/03/11 ip  #3346 - Return new column ChargeAcctWrittenOff to indicate a Service Requests Charge Account has been 
--									  written off
-- 29/09/11 ip #8341 -  UAT66 - Returning columns CostPrice and PreviousCostsEW required previously for UAT29
-- =============================================
	-- Add the parameters for the function here
    @ServiceBranchNo    SMALLINT,
    @ServiceUniqueId    INTEGER,
    @CustId             VARCHAR(20),
    @Return             INTEGER OUTPUT

AS
    SET NOCOUNT ON
    SET @Return = 0

    -- There could be a lot of stock repair Service Requests
    -- so must search on the SR number and the stock repair customer id.
    -- The products MUST be on the Courts Stock Item table.
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
        ISNULL(s.IUPC,sr.ProductCode) as ProductCode,			-- RI
        ISNULL(s.ItemNo,'') as CourtsCode,			-- RI
        sr.UnitPrice,
        1 AS Quantity,
        1 AS MultipleQuantity,
        sr.Sequence,
        ISNULL(s.ItemDescr1,'') AS Description,
        ISNULL(s.ItemDescr2,'') AS ItemDescr2,
		ISNULL(c.category,'') AS Category,
        sr.ContractNo,
        CONVERT(CHAR(12),'') AS DeliveryNoteNumber,
		CONVERT(CHAR(12),'') AS DeliveryDate,
        sr.ExtWarranty,
        sr.RefCode,
        CONVERT(CHAR(12),'') AS AcctNo,
        CONVERT(INTEGER,0) AS InvoiceNo,
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
        --sr.softscriptdate,			--CR1030 
        sr.DateCollected,
        CAST (0.00 AS MONEY) AS OutstBal,
		CAST (0.00 AS MONEY) AS PreviousCosts,
		CAST('N' AS CHAR(1)) AS Exchanged,       --UAT 381 Extra field required to mark item as exchanged
		CAST('N' AS CHAR(1)) AS Repossessed,   --UAT 380 Extra field required to mark item as repossessed
		CAST('N' AS CHAR(1)) AS Replaced,         --UAT 381 Extra field required to mark item as replaced
		ISNULL(sr.History,'N') AS History,       --UAT 367 New fields required for Customer Audit Details
		ISNULL(sr.LinkedSR,0) AS LinkedSR,
		ISNULL(sr.Updated,0) AS Updated,
		sr.PrintLocn,								-- CR 949/958
		sr.LbrCostEstimate, --CR 1024 (NM 29/04/2009)
		sr.AdtnlLbrCostEstimate, --CR 1024 (NM 29/04/2009)
		sr.TransportCostEstimate , --CR 1024 (NM 29/04/2009)
		sr.TechnicianReport,  --CR 1024 (NM 29/04/2009)
		ISNULL(sr.Retailer,'') as Retailer,			--CR1030 jec
		0 as ChargeAcctWrittenOff,	--IP - 21/03/11 - #3346
		sr.ItemId	,		-- RI
		ISNULL(s.CostPrice,0) as CostPrice,		--IP - 29/09/11 - #8341 - UAT66
		CAST (0.00 AS MONEY) AS PreviousCostsEW	--IP - 29/09/11 - #8341 - UAT66
		
    INTO #ProductList
	FROM StockItem s RIGHT OUTER JOIN SR_ServiceRequest sr    
	ON      s.StockLocn = sr.StockLocn
    --AND     s.ItemNo = sr.ProductCode
    AND     s.ItemId = sr.ItemId			-- RI 
	LEFT OUTER JOIN Code c
	ON CAST(s.category AS VARCHAR(12)) = c.code	
    WHERE   sr.ServiceRequestNo = @ServiceUniqueId
    AND     sr.ServiceBranchNo = @ServiceBranchNo
    AND     sr.CustId = @CustId
    AND     sr.ServiceType = 'S'    -- Internal Stock	
    AND		(c.category = 'PCE' OR c.category = 'PCF' OR c.category = 'PCW' OR c.category is null OR c.category = 'PCO' )   -- RI

	-- Include total of all previous costs for specific product

	SELECT SUM(TotalCost) AS PreviousCosts ,ProductCode,ItemId		-- RI
	INTO #Previous FROM SR_Resolution R JOIN SR_ServiceRequest S ON R.ServiceRequestNo = S.ServiceRequestNo 
	WHERE CustId IN (SELECT TOP 1 CustID FROM #ProductList) AND History = 'Y' 
	GROUP BY ProductCode,CustId,ItemId					-- RI

	UPDATE #ProductList
	SET		PreviousCosts = P.PreviousCosts
	FROM	#Previous P
	WHERE	 #ProductList.ItemId = P.ItemId				-- RI
	--WHERE	 #ProductList.ProductCode = P.ProductCode
	
	 --IP - 21/03/11 - #3346
     UPDATE #ProductList
     SET ChargeAcctWrittenOff = 1
     WHERE EXISTS(select * from SR_ChargeAcct sca
					inner join fintrans f on sca.acctno = f.acctno
					where f.transtypecode = 'WOS'
					and	sca.ServicerequestNo = #ProductList.ServiceRequestNo)
					

    -- Return the SR with the Allocation and Resolution
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
    LEFT JOIN StockItem SI ON sr.ItemId = SI.ItemId and sr.StockLocn = SI.StockLocn,	-- RI
    --LEFT JOIN StockItem SI ON sr.ProductCode = SI.ItemNo and sr.StockLocn = SI.StockLocn, --CR 1024 (NM 23/04/2009)
    SR_Allocation a, SR_Resolution r
    WHERE   a.ServiceRequestNo = sr.ServiceRequestNo
    AND     r.ServiceRequestNo = sr.ServiceRequestNo
    ORDER BY sr.ServiceRequestNo DESC, sr.ProductCode ASC, sr.StockLocn ASC


    -- Return the list of Parts for the SR with a non-courts Y/N flag
    SELECT  DISTINCT
            sr.ServiceRequestNo,
            sr.PartNo,
            CONVERT(VARCHAR, sr.Quantity) AS Quantity,
            CONVERT(VARCHAR, sr.UnitPrice) AS UnitPrice,
            CONVERT(VARCHAR, sr.Quantity * sr.UnitPrice) AS Total,
            sr.Description,
            CASE WHEN s.ItemNo IS NULL THEN "Y" ELSE "N" END AS NonCourts,
            sr.PartType,
			sr.StockLocn, --IP - 18/06/09 - UAT(687)
			sr.PartID	  --IP - 01/07/11 - CR1254 - RI - #3994
    FROM   SR_PartListResolved sr
    LEFT OUTER JOIN StockItem s	ON s.ItemId = sr.PartId			-- RI
    --ON     s.ItemNo = sr.PartNo
    WHERE  sr.ServiceRequestNo IN (SELECT p.ServiceRequestNo FROM #ProductList p)
    ORDER BY sr.ServiceRequestNo, sr.PartNo


    -- Return the Charge To Analysis for the SR (if saved)
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
