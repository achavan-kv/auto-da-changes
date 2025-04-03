IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[DN_SRReportDateLoggedSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DN_SRReportDateLoggedSP]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Peter Chong
-- Create date: 22-Oct-2006
-- Description:	Service Requests by date logged
-- Modified by: J.Hemans
-- Modified date: 03/01/2008
-- Modified Description: Code change required so that date comparisons work for @MinDateLogged = @MaxDateLogged UAT 340
-- =============================================
CREATE PROCEDURE DN_SRReportDateLoggedSP
-- =============================================
-- Author:		Peter Chong
-- Create date: 22-Oct-2006
-- Title:	DN_SRReportDateLoggedSP
--
--	This procedure will retieve the details for Date Logged.
-- 
-- Change Control
-----------------
-- 29/11/10 jec CR1030 - add new parameter - Days Outstanding
-- 07/01/11 jec CR1030 - return Status and date change
-- 21/01/11 jec Restrict rows returned  & 1900 dates as null
--              Exclude Closed SR's
-- =============================================
	-- Add the parameters for the function here 
	@MinDateLogged datetime = null,
	@MaxDateLogged datetime = null,
	@MinSRNo	   int  = null,
	@MaxSRNo	   int  = null,
	@ViewTop	   BIT,			-- true = 500
	@DaysOutstanding INT,	--CR1030
	@return		   int output	
AS
BEGIN
	SET NOCOUNT ON;
	declare @top INT
	if @ViewTop=1 set @top=500 else set @top = 9999
	
	SELECT top (@top) SR.ServiceRequestNo
		, convert(varchar(4), SR.ServiceBranchNo) + convert(varchar(16), SR.ServiceRequestNo) [ServiceRequestNoStr]
		, SR.Status
		, DateLogged
		, convert(varchar(16), SA.TechnicianID) + ': ' + T.FirstName + ' ' + T.LastName [TechnicianId]
		, case when SA.DateAllocated ='1900-01-01' then null else SA.DateAllocated end as 'DateAllocated'
		, case when SA.RepairDate ='1900-01-01' then null else SA.RepairDate end as 'RepairDate'
		, DATEDIFF(d,SR.DateLogged,getdate()) as 'Days Outstanding'
		, case when SR.DateChange ='1900-01-01' then null else SR.DateChange end as 'DateChange'
	FROM SR_ServiceRequest SR INNER JOIN
	SR_Allocation SA ON SR.ServiceRequestNo = SA.ServiceRequestNo LEFT OUTER JOIN
	SR_Resolution SRE ON SR.ServiceRequestNo = SRE.ServiceRequestNo INNER JOIN
	SR_Technician T ON SA.TechnicianId = T.TechnicianId 
	WHERE
	(@MinDateLogged IS NULL OR CONVERT(DATETIME,CONVERT(VARCHAR,SR.DateLogged,103),103) >= @MinDateLogged) AND
	(@MaxDateLogged IS NULL OR CONVERT(DATETIME,CONVERT(VARCHAR,SR.DateLogged,103),103) <= @MaxDateLogged) AND
	(@MinSRNo IS NULL OR SR.ServiceRequestNo >= @MinSRNo) AND
	(@MaxSRNo IS NULL OR SR.ServiceRequestNo <= @MaxSRNo) AND
	ISNULL(DateClosed, '1900-01-01') = '1900-01-01'
	-- days outstanding > parameter days --CR1030 jec
	and DATEDIFF(d,SR.DateLogged,getdate())> @DaysOutstanding
	and SR.status!='C'		--21/01/11

	SET @return = @@error
END
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End