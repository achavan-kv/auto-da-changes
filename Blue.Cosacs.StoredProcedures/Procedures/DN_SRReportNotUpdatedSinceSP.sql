/****** Object:  StoredProcedure [dbo].[DN_SRReportNotUpdatedSinceSP]    Script Date: 10/23/2006 13:43:05 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[DN_SRReportNotUpdatedSinceSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DN_SRReportNotUpdatedSinceSP]
GO 
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Peter Chong
-- Create date: 22_Oct-2006
-- Description:	Not updated since service report 
-- Modified by: J.Hemans
-- Modified date: 03/01/2008
-- Modified Description: Code change required so that date comparisons work for @MinDateLogged = @MaxDateLogged UAT 340
-- =============================================
CREATE PROCEDURE DN_SRReportNotUpdatedSinceSP
-- =============================================
-- Author:		Peter Chong
-- Create date: 22-Oct-2006
-- Title:	DN_SRReportNotUpdatedSinceSP
--
--	This procedure will retieve the details for Not Updated Since.
-- 
-- Change Control
-----------------
-- 07/01/11 jec CR1030 - return Status and date change
-- 21/01/11 jec Restrict rows returned & 1900 dates as null 
-- =============================================
	-- Add the parameters for the function here
	@MinDateLogged datetime = null,
	@MaxDateLogged datetime = null,
	@MinSRNo	   int  = null,
	@MaxSRNo	   int  = null,
	@ViewTop	   BIT,			-- true = 500
	@NotUpdatedSinceDate datetime,
	@return		   int output	
AS
BEGIN
	SET NOCOUNT ON;
	
	declare @top INT
	if @ViewTop=1 set @top=500 else set @top = 9999
    -- Insert statements for procedure here
	SELECT top (@top) SR.ServiceRequestNo
		, convert(varchar(4), SR.ServiceBranchNo) + convert(varchar(16), SR.ServiceRequestNo) [ServiceRequestNoStr]
		, SR.Status
		, DateLogged
		, Convert(varchar(16), SA.TechnicianID) + ': ' + T.FirstName + ' ' + T.Lastname [TechnicianId]
		, case when SA.DateAllocated ='1900-01-01' then null else SA.DateAllocated end as 'Date Allocated'
		, case when SA.RepairDate ='1900-01-01' then null else SA.RepairDate end as 'RepairDate'
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
		ISNULL(DateClosed, '1900-01-01') = '1900-01-01' AND --Date closed is blank 
		DateLogged < @NotUpdatedSinceDate			AND
		SA.DateAllocated < @NotUpdatedSinceDate	AND
		(isnull(SR.DateReopened, '1900-01-01') = '1900-01-01' OR SR.DateReopened < @NotUpdatedSinceDate) AND
		(	SELECT ISNULL(Max(DateTrans), '1900-01-01') 
			FROM SR_ChargeAcct AC JOIN fintrans FT ON AC.AcctNo = FT.AcctNo
			WHERE AC.ServiceRequestNo = SR.ServiceRequestNo AND transTypeCode = 'PAY'
		) < @NotUpdatedSinceDate 

	SET @return = @@error
END
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End
