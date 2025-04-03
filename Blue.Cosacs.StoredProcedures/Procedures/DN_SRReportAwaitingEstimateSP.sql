
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[DN_SRReportAwaitingEstimateSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DN_SRReportAwaitingEstimateSP]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Peter Chong
-- Create date: 22-Oct-2006
-- Description:	Awaiting Estimate Service Report
-- Modified by: J.Hemans
-- Modified date: 03/01/2008
-- Modified Description: Code change required so that date comparisons work for @MinDateLogged = @MaxDateLogged UAT 340
-- =============================================
CREATE PROCEDURE DN_SRReportAwaitingEstimateSP
-- =============================================
-- Author:		Peter Chong
-- Create date: 22-Oct-2006
-- Title:	DN_SRReportAwaitingEstimateSP
--
--	This procedure will retieve the details for Awaitng Estimate.
-- 
-- Change Control
-----------------
-- 29/11/10 jec CR1030 - add new parameter - Days Outstanding
-- 30/11/10 jec CR1030 - return Technician name & Date allocated & add new parameter - technician
-- 07/01/11 jec CR1030 - return Status and date change
-- 21/01/11 jec Restrict rows returned & 1900 dates as null
-- 28/01/11 jec Include SR's logged today when @DaysOutstanding=0
-- 01/07/11 ip  CR1254 - Return CourtsCode/IUPC   
-- =============================================
	-- Add the parameters for the function here 
	@MinDateLogged datetime = null,
	@MaxDateLogged datetime = null,
	@MinSRNo	   int  = null,
	@MaxSRNo	   int  = null,
	@ViewTop	   BIT,			-- true = 500
	@TechnicianID		INT,	--CR1030
	@DaysOutstanding INT,	--CR1030	
	@Return		   int output	
AS
BEGIN
	SET NOCOUNT ON;
	declare @top INT
	if @ViewTop=1 set @top=500 else set @top = 9999
	if @DaysOutstanding=0 set @DaysOutstanding=-1		--jec  28/01/11

    -- Insert statements for procedure here
	SELECT top (@top) SR.ServiceRequestNo
		, convert(varchar(4), SR.ServiceBranchNo) + convert(varchar(16), SR.ServiceRequestNo) [ServiceRequestNoStr]
		,SR.Status
		, SR.DateLogged
		,isnull(si.iupc, SR.ProductCode) as ProductCode		--IP - 01/07/11 - CR1254
		,isnull(si.itemno,'') as CourtsCode				--IP - 01/07/11 - CR1254
		--, SR.ProductCode
		, SR.Description
		,ISNULL(CAST(t.TechnicianId as VARCHAR(3)) + ': ' + t.FirstName + ' ' + t.LastName,'Not Allocated') as 'Technician', 
		case when a.DateAllocated ='1900-01-01' then null else a.DateAllocated end as 'Date Allocated'
		, DATEDIFF(d,SR.DateLogged,getdate()) as 'Days Outstanding'
		, case when SR.DateChange ='1900-01-01' then null else SR.DateChange end as 'DateChange' 		
	FROM SR_ServiceRequest SR LEFT OUTER JOIN
		SR_Resolution SRE ON SR.ServiceRequestNo = SRE.ServiceRequestNo
		INNER JOIN SR_Allocation a on SR.ServiceRequestNo = a.ServiceRequestNo
		LEFT OUTER JOIN SR_Technician t on a.TechnicianId = t.TechnicianId
		LEFT OUTER JOIN StockInfo si on si.ID = SR.ItemID						--IP - 01/07/11 - CR1254
	WHERE
		(@MinDateLogged IS NULL OR CONVERT(DATETIME,CONVERT(VARCHAR,SR.DateLogged,103),103) >= @MinDateLogged) AND
		(@MaxDateLogged IS NULL OR CONVERT(DATETIME,CONVERT(VARCHAR,SR.DateLogged,103),103) <= @MaxDateLogged) AND
		(@MinSRNo IS NULL OR SR.ServiceRequestNo >= @MinSRNo) AND
		(@MaxSRNo IS NULL OR SR.ServiceRequestNo <= @MaxSRNo) AND
		ISNULL(SRE.DateClosed, '1900-01-01') = '1900-01-01' AND
		SR.Status = 'E' AND  SR.RepairEstimate = 0
		-- days outstanding > parameter days --CR1030 jec
		and DATEDIFF(d,SR.DateLogged,getdate())> @DaysOutstanding
		and (t.TechnicianId=@TechnicianID or @TechnicianID=0)		

	SET @return = @@error
END
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End
