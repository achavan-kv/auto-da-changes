
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[DN_SRReportRepairExceededSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DN_SRReportRepairExceededSP]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE DN_SRReportRepairExceededSP
-- =============================================
-- Author:		John Croft	
-- Create date: January 2011
-- Title:	DN_SRReportRepairExceededSP
--
--	This procedure will retrieve the details for Previous Repair Total Exceeded report.
-- 
-- Change Control
-----------------
-- 19/01/11 jec CR1030 include Non Cosacs/Cash&Go SR's' & ServiceType parm.
-- 20/01/11 jec ServiceType parm should be serviceStatus
-- 21/01/11 jec Restrict rows returned   
-- 01/07/11 ip  CR1254 -RI - #3992 - Return CourtsCode/IUPC  
-- 02/03/12 ip  #9678 - LW74723 - Previous Repair % Parameter - Now parameter is used as a % of the Cost Price
-- =============================================
	-- Add the parameters for the function here 
	@MinDateLogged datetime = null,
	@MaxDateLogged datetime = null,
	@MinSRNo	   int  = null,
	@MaxSRNo	   int  = null,
	@ViewTop	   BIT,			-- true = 500			
	@ServiceStatus   VARCHAR(20),		
	--@TechnicianID		INT,	
	--@DaysOutstanding INT,		
	@Return		   int output	
AS
BEGIN

	declare @top INT
	if @ViewTop=1 set @top=500 else set @top = 9999

	SET NOCOUNT ON;
	Declare @PreviousRepair MONEY
	
	select @PreviousRepair = value from CountryMaintenance where codename='PreviousRepair'
		
    -- Insert statements for procedure here
	SELECT 
        
        ISNULL(sr.ServiceBranchNo,0) AS ServiceBranchNo,
        ISNULL(CONVERT(VARCHAR,sr.ServiceBranchNo) + CONVERT(VARCHAR,sr.ServiceRequestNo),'') AS ServiceRequestNoStr,
        ISNULL(sr.ServiceRequestNo,0) AS ServiceRequestNo,
        sr.status as [Status],
        --sr.ProductCode AS ProductCode,
        isnull(si.iupc, sr.ProductCode) as ProductCode,		--IP - 01/07/11 - CR1254 - RI - #3992
        isnull(si.itemno,'') as CourtsCode,					--IP - 01/07/11 - CR1254 - RI - #3992	
        sr.UnitPrice AS UnitPrice,
        sr.Sequence,
        sr.AcctNo,
        ISNULL(sr.CustId,'') AS CustId,
        ISNULL(sr.ServiceType,'') AS ServiceType,
        ISNULL(sr.SerialNo,'') AS SerialNo,
		ISNULL(a.OutstBal,0) AS OutstBal,
		CAST (0.00 AS MONEY) AS PreviousCosts,
		ISNULL(sr.LinkedSR,0) AS LinkedSR,
		sp.CostPrice as CostPrice							--IP - 02/03/12 - #9678 - LW74723				
		
    INTO #ProductList
    FROM SR_ServiceRequest sr 
    --LEFT OUTER JOIN StockItem s on sr.ProductCode = s.ItemNo and sr.StockLocn = s.StockLocn    
    LEFT OUTER JOIN StockInfo si on si.ID = sr.ItemID				--IP - 01/07/11 - CR1254 - RI - #3992
    LEFT OUTER JOIN StockQuantity sq on si.ID = sq.ID and sr.stocklocn = sq.stocklocn --IP - 01/07/11 - CR1254 - RI - #3992
    LEFT OUTER JOIN StockPrice sp on si.ID = sp.ID and sp.branchno = sr.stocklocn	  --IP - 02/03/12 - #9678 - LW74723
    --LEFT OUTER JOIN Code c  on ISNULL(CAST(s.category AS VARCHAR(12)),'NON') = c.code and c.category in('PCE','PCF','PCW')
    LEFT OUTER JOIN Code c  on ISNULL(CAST(si.category AS VARCHAR(12)),'NON') = c.code and c.category in('PCE','PCF','PCW') --IP - 01/07/11 - CR1254 - RI - #3992
    LEFT OUTER JOIN SR_ChargeAcct ca ON sr.ServiceRequestNo = ca.ServiceRequestNo AND (ca.ChargeType = 'C' or ca.ChargeType = 'D')
	LEFT OUTER JOIN SR_Resolution r ON sr.ServiceRequestNo = r.ServiceRequestNo 
	LEFT OUTER JOIN Acct a ON a.AcctNo = ca.AcctNo 
	WHERE
		(@MinDateLogged IS NULL OR CONVERT(DATETIME,CONVERT(VARCHAR,SR.DateLogged,103),103) >= @MinDateLogged) AND
		(@MaxDateLogged IS NULL OR CONVERT(DATETIME,CONVERT(VARCHAR,SR.DateLogged,103),103) <= @MaxDateLogged) AND
		(@MinSRNo IS NULL OR SR.ServiceRequestNo >= @MinSRNo) AND
		(@MaxSRNo IS NULL OR SR.ServiceRequestNo <= @MaxSRNo)
		and (sr.Status=@ServiceStatus or @ServiceStatus='')
	and sr.ServiceRequestNo>0 
	--AND     ((sr.ServiceType in('C','G') AND s.ItemType = 'S') or (sr.ServiceType ='N')) 
	AND     ((sr.ServiceType in('C','G') AND si.ItemType = 'S') or (sr.ServiceType ='N'))  --IP - 01/07/11 - CR1254 - RI - #3992 
	AND (LEFT(r.ChargeTo,1) = ca.ChargeType OR (ca.ChargeType = 'C' AND r.ChargeTo <> 'DEL') OR (ca.ChargeType = 'D' and  r.ChargeTo <> 'CUS')
	OR ca.ChargeType IS NULL)
	
	-- Get any previous repair costs	
	SELECT SUM(TotalCost) AS PreviousCosts ,
		   --ProductCode,
		   isnull(SI.IUPC, S.ProductCode) as ProductCode,		--IP - 01/07/11 - CR1254 - RI - #3992 
		   isnull(SI.itemno,'') as CourtsCode,					--IP - 01/07/11 - CR1254 - RI - #3992 
		   SerialNo,
		   custid
	INTO #Previous FROM SR_Resolution R JOIN SR_ServiceRequest S ON R.ServiceRequestNo = S.ServiceRequestNo 
	LEFT JOIN StockInfo SI ON SI.ID = S.ItemID				--IP - 01/07/11 - CR1254 - RI - #3992 
	--GROUP BY ProductCode,CustId,SerialNo 
	GROUP BY isnull(SI.IUPC,S.ProductCode), isnull(SI.itemno,''),CustId,SerialNo --IP - 01/07/11 - CR1254 - RI - #3992 
	
	-- update table with repair costs
	UPDATE #ProductList
	SET		PreviousCosts = P.PreviousCosts
	FROM	#Previous P
	--WHERE	 #ProductList.ProductCode = P.ProductCode AND #ProductList.SerialNo = P.SerialNo
	WHERE	 #ProductList.ProductCode = P.ProductCode	
	AND		 #ProductList.CourtsCode = P.CourtsCode				--IP - 01/07/11 - CR1254 - RI - #3992 
	AND #ProductList.SerialNo = P.SerialNo
			AND #ProductList.Custid = P.Custid
					
	-- return data
	select top (@top) custid,ProductCode, CourtsCode,			--IP - 01/07/11 - CR1254 - RI - #3992 
	MAX(ServiceRequestNo) as ServiceRequestNo,
	MAX(ServiceRequestNoStr) as ServiceRequestNoStr,Status,MAX(Sequence) as NoOfRepairs,MAX(UnitPrice) as Allowable,
			MAX(PreviousCosts) as PreviousRepairs, MAX(unitPrice)-MAX(previousCosts) as BalanceAvailable
	from #ProductList
	--Where PreviousCosts>@PreviousRepair 	
	Where PreviousCosts> Round(CostPrice * (@PreviousRepair /100),2)	--IP - 02/03/12 - #9678 - LW74723
	--group by custid,productcode,Status
	group by custid,productcode,CourtsCode,Status				--IP - 01/07/11 - CR1254 - RI - #3992 
	order by MAX(ServiceRequestNoStr)
	
	SET @return = @@error
END
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End
