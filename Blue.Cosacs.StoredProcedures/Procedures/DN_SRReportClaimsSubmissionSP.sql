/****** Object:  StoredProcedure [dbo].[DN_SRReportClaimsSubmissionSP]    Script Date: 10/25/2006 09:40:05 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[DN_SRReportClaimsSubmissionSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DN_SRReportClaimsSubmissionSP]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Peter Chong
-- Create date: 25-Oct-200
-- Description:	Claims Submission Report
-- Modified by: J.Hemans
-- Modified date: 03/01/2008
-- Modified Description: Code change required so that date comparisons work for @MinDateLogged = @MaxDateLogged UAT 340
-- Modified by: J.Hemans
-- Modified date: 07/10/2008
-- Modified Description: CR 949/958 Delivery Date added to report
-- 10/02/2009 jec 70815 Missing Name/address only show total cost & labour cost on one row.
-- Modified by: J.Hemans
-- Modified date: 26/08/2008
-- Modified Description: UAT 531 Duplicate rows being returned if more than one slot allocated for a SR
-- =============================================
CREATE PROCEDURE DN_SRReportClaimsSubmissionSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_SRReportClaimsSubmissionSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Claims Submission Report 
-- Author       : Peter Chong
-- Date         : 25-Oct-2007
--
-- This procedure will retrieve details for the Claims Submission Report.
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 09/11/10 jec CR1030 Check for "EW"
-- 22/07/11 jec CR1254 RI Changes
-- ================================================
	-- Add the parameters for the stored procedure here
	@MinDateLogged datetime = null, 
	@MaxDateLogged datetime = null,
	@MinSRNo int = null,
	@MaxSRNo int = null,
	@fault	CHAR(4),   -- UAT 453
	@return int = 0 OUTPUT
AS
BEGIN
	SET NOCOUNT ON;

    -- UAT 345 Country Code is also required for these reports
    DECLARE @countryCode CHAR(1)
    SET @countryCode = (SELECT TOP 1 countrycode FROM country)
    
    IF @fault = 'ALL'
    BEGIN
	SELECT DISTINCT @countryCode AS Country,SR.ServiceRequestNo 
	, convert(varchar(4), SR.ServiceBranchNo) + convert(varchar(16), SR.ServiceRequestNo) [ServiceRequestNoStr] 
	, SR.Status
	, Case when isnull(CurrStatus, '') = 'S' then 'Y' else 'N' end [Paid]
	, SR.ModelNo 
	, SR.SerialNo
	, case when SR.PurchaseDate = '1900-01-01' then null else SR.PurchaseDate end [PurchaseDate]
	, R.DateClosed [DateResolved]
	, PL.PartNo
	, PL.Quantity
	, PL.UnitPrice [Cost]
	, R.TotalCost	
	, R.LabourCost
	, C.FirstName + ' ' + C.LastName [CustomerName]
	, C.Address1 + ' ' + C.Address2 + ' ' + C.Address3 + ' ' + C.AddressPC [CustomerAddress]
	, SR.AcctNo
	, SI.itemdescr1  [PartDescription]
	, SR.ReceivedDate
	, SR.DateLogged
	, R.Resolution
	, CASE WHEN DateDiff(d, purchasedate, getdate()) <(	
			select convert(int,reference) * 365
			from code 
			where code.category='WRF' 
				and code =  sr.RefCode
	) THEN 'Y' ELSE 'N' END [FYWtic] --if purchase date < 1 year old (bSevicerequest)
	
	, SR.ExtWarranty 
	, SI.unitpricecash UnitPrice --Stock item table for courts only.
	, TD.SlotDate [RepairDate]
	--RM add change from 5.1
	, REPLACE (REPLACE(SR.Comments, CHAR(13) + CHAR(10), ''), CHAR(9),'') as Comments --IP 01/04/09 - Correcting error adding column name
	, dateadd(d, 6, SR.DateLogged) [DatePromised]
	, SR.DateCollected
	, SR.PurchaseDate AS 'Delivery Date'
	, SR.TechnicianReport --// CR 1024 (NM 30/04/2009)
into #ClaimsReportA			-- 70815
FROM dbo.SR_ServiceRequest SR LEFT OUTER JOIN 
	dbo.SR_Resolution R			ON SR.ServiceRequestNo = R.ServiceRequestNo				LEFT OUTER JOIN  
	dbo.SR_PartListResolved PL	ON SR.ServiceRequestNo = PL.ServiceRequestNo			LEFT OUTER JOIN 
	dbo.SR_Customer C			ON C.ServiceRequestNo = SR.ServiceRequestNo				LEFT OUTER JOIN 
	--StockItem SI				ON PL.PartNo = SI.Itemno AND SI.Stocklocn = SR.Stocklocn 	LEFT OUTER JOIN
	StockItem SI				ON PL.PartId = SI.ItemId AND SI.Stocklocn = SR.Stocklocn			-- RI
	LEFT OUTER JOIN dbo.SR_TechnicianDiary TD	ON  TD.ServiceRequestNo = SR.ServiceRequestNo		LEFT OUTER JOIN
	sr_chargeacct CA			ON CA.ChargeType = 'S' AND CA.ServiceRequestNo = SR.ServiceRequestNo LEFT OUTER JOIN
	Acct A						ON A.Acctno = CA.AcctNO  
	--dbo.SR_ChargeTo	CT			ON CT.ServiceRequestNo = SR.ServiceRequestNo
	
WHERE 
	SR.Status IN ('R', 'C') AND
	(@MinDateLogged IS NULL OR CONVERT(DATETIME,CONVERT(VARCHAR,SR.DateLogged,103),103) >= @MinDateLogged) AND
	(@MaxDateLogged IS NULL OR CONVERT(DATETIME,CONVERT(VARCHAR,SR.DateLogged,103),103) <= @MaxDateLogged) AND
	(@MinSRNo IS NULL OR SR.ServiceRequestNo >= @MinSRNo) AND
	(@MaxSRNo IS NULL OR SR.ServiceRequestNo <= @MaxSRNo) AND 
	-- UAT 346 OR clause needs to be in brackets
	(isnull(R.ChargeTo, '') = 'SUP' OR --isnull(CT.Supplier, 0) <> 0  
	EXISTS (SELECT * FROM SR_ChargeTo WHERE ServiceRequestNo = SR.ServiceRequestNo AND ISNULL(Supplier, 0) <> 0 )
	-- UAT 385 report to include AIG charges as well
	OR isnull(R.ChargeTo, '') = 'EW'	--CR1030
	OR isnull(R.ChargeTo, '') = 'AIG' OR EXISTS (SELECT * FROM SR_ChargeTo WHERE ServiceRequestNo = SR.ServiceRequestNo AND ISNULL(ExtWarranty, 0) <> 0))
	
		-- Update name and address if null (i.e. not in SR_Customer)	-- 70815 jec 10/02/09
	update #claimsreportA
	set customername= firstname+' '+name 
	from #claimsreportA r INNER JOIN SR_ServiceRequest sr on r.ServiceRequestNo=sr.ServiceRequestNo
		INNER JOIN customer c on sr.custid=c.custid 
	where r.customerName is null

	update #claimsreportA
	set customerAddress= cusaddr1+' '+cusaddr2+' '+cusaddr3
	from #claimsreportA r INNER JOIN SR_ServiceRequest sr on r.ServiceRequestNo=sr.ServiceRequestNo
		INNER JOIN custAddress a on sr.custid=a.custid and addtype='H'
	where r.customerAddress is null
	
	-- remove duplicate Totalcost and labour cost 
	select servicerequestNo,totalcost,labourcost,max(partno) as partno
	into #CostsA
	from #claimsreportA
	group by servicerequestNo,totalcost,labourcost

	update #claimsreportA set totalcost=0,labourcost=0
	update #claimsreportA
			set totalcost=c.totalcost,labourcost=c.labourcost
	from #claimsreportA r INNER JOIN #CostsA c on r.servicerequestNo=c.servicerequestNo and isnull(r.partno,'99')=isnull(c.partno,'99')

	
	-- Return data
	select * from #claimsreportA order by ServiceRequestNo
	
	END
	ELSE
	BEGIN
	SELECT DISTINCT @countryCode AS Country,SR.ServiceRequestNo 
	, convert(varchar(4), SR.ServiceBranchNo) + convert(varchar(16), SR.ServiceRequestNo) [ServiceRequestNoStr] 
	, SR.Status
	, Case when isnull(CurrStatus, '') = 'S' then 'Y' else 'N' end [Paid]
	, SR.ModelNo 
	, SR.SerialNo
	, case when SR.PurchaseDate = '1900-01-01' then null else SR.PurchaseDate end [PurchaseDate]
	, R.DateClosed [DateResolved]
	, PL.PartNo
	, PL.Quantity
	, PL.UnitPrice [Cost]
	, R.TotalCost	
	, R.LabourCost
	, C.FirstName + ' ' + C.LastName [CustomerName]
	, C.Address1 + ' ' + C.Address2 + ' ' + C.Address3 + ' ' + C.AddressPC [CustomerAddress]
	, SR.AcctNo
	, SI.itemdescr1  [PartDescription]
	, SR.ReceivedDate
	, SR.DateLogged
	, R.Resolution
	, CASE WHEN DateDiff(y, purchasedate, getdate()) <(	
			select top 1  isnull(convert(int,FirstYearWarPeriod), 1)
			from warrantyband b
			where refcode = (select top 1 refcode
			from stockitem s where s.itemno = sr.productcode
			and s.stocklocn = sr.stocklocn))
	 THEN 'Y' ELSE 'N' END [FYWtic] --if purchase date < length of man war period(bSevicerequest)
	
	, SR.ExtWarranty 
	, SI.unitpricecash UnitPrice --Stock item table for courts only.
	, TD.SlotDate [RepairDate]
	--RM add change from 5.1
	, REPLACE (REPLACE(SR.Comments, CHAR(13) + CHAR(10), ''), CHAR(9),'') as Comments --IP 01/04/09 - Correcting error adding column name
	, dateadd(d, 6, SR.DateLogged) [DatePromised]
	, SR.DateCollected
	, SR.PurchaseDate AS 'Delivery Date'
	, SR.TechnicianReport --// CR 1024 (NM 30/04/2009)
into #ClaimsReportB			-- 70815
FROM dbo.SR_ServiceRequest SR LEFT OUTER JOIN 
	dbo.SR_Resolution R			ON SR.ServiceRequestNo = R.ServiceRequestNo				LEFT OUTER JOIN  
	dbo.SR_PartListResolved PL	ON SR.ServiceRequestNo = PL.ServiceRequestNo			LEFT OUTER JOIN 
	dbo.SR_Customer C			ON C.ServiceRequestNo = SR.ServiceRequestNo				LEFT OUTER JOIN 
	--StockItem SI				ON PL.PartNo = SI.Itemno AND SI.Stocklocn = SR.Stocklocn 	LEFT OUTER JOIN
	StockItem SI				ON PL.PartId = SI.ItemId AND SI.Stocklocn = SR.Stocklocn			-- RI
	LEFT OUTER JOIN dbo.SR_TechnicianDiary TD	ON  TD.ServiceRequestNo = SR.ServiceRequestNo		LEFT OUTER JOIN
	sr_chargeacct CA			ON CA.ChargeType = 'S' AND CA.ServiceRequestNo = SR.ServiceRequestNo LEFT OUTER JOIN
	Acct A						ON A.Acctno = CA.AcctNO  
	--dbo.SR_ChargeTo	CT			ON CT.ServiceRequestNo = SR.ServiceRequestNo
	
WHERE 
	SR.Status IN ('R', 'C') AND
	(@MinDateLogged IS NULL OR CONVERT(DATETIME,CONVERT(VARCHAR,SR.DateLogged,103),103) >= @MinDateLogged) AND
	(@MaxDateLogged IS NULL OR CONVERT(DATETIME,CONVERT(VARCHAR,SR.DateLogged,103),103) <= @MaxDateLogged) AND
	(@MinSRNo IS NULL OR SR.ServiceRequestNo >= @MinSRNo) AND
	(@MaxSRNo IS NULL OR SR.ServiceRequestNo <= @MaxSRNo) AND 
	-- UAT 346 OR clause needs to be in brackets
	(isnull(R.ChargeTo, '') = 'SUP' OR --isnull(CT.Supplier, 0) <> 0  
	EXISTS (SELECT * FROM SR_ChargeTo WHERE ServiceRequestNo = SR.ServiceRequestNo AND ISNULL(Supplier, 0) <> 0 )
	-- UAT 385 report to include AIG charges as well
	OR isnull(R.ChargeTo, '') = 'EW'	--CR1030
	OR isnull(R.ChargeTo, '') = 'AIG' OR EXISTS (SELECT * FROM SR_ChargeTo WHERE ServiceRequestNo = SR.ServiceRequestNo AND ISNULL(ExtWarranty, 0) <> 0))
	AND R.Fault = @fault
	
		-- Update name and address if null (i.e. not in SR_Customer)	-- 70815 jec 10/02/09
	update #claimsreportB
	set customername= firstname+' '+name 
	from #claimsreportB r INNER JOIN SR_ServiceRequest sr on r.ServiceRequestNo=sr.ServiceRequestNo
		INNER JOIN customer c on sr.custid=c.custid 
	where r.customerName is null

	update #claimsreportB
	set customerAddress= cusaddr1+' '+cusaddr2+' '+cusaddr3
	from #claimsreportB r INNER JOIN SR_ServiceRequest sr on r.ServiceRequestNo=sr.ServiceRequestNo
		INNER JOIN custAddress a on sr.custid=a.custid and addtype='H'
	where r.customerAddress is null
	
	-- remove duplicate Totalcost and labour cost 
	select servicerequestNo,totalcost,labourcost,max(partno) as partno
	into #CostsB
	from #claimsreportB
	group by servicerequestNo,totalcost,labourcost

	update #claimsreportB set totalcost=0,labourcost=0
	update #claimsreportB
			set totalcost=c.totalcost,labourcost=c.labourcost
	from #claimsreportB r INNER JOIN #CostsB c on r.servicerequestNo=c.servicerequestNo and isnull(r.partno,'99')=isnull(c.partno,'99')

	
	-- Return data
	select * from #claimsreportB order by ServiceRequestNo	
	
	END
	
	
SET @Return = @@error
END
GO


