IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[DN_SRReportReassignTechnicianSP]') 
	AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DN_SRReportReassignTechnicianSP]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE DN_SRReportReassignTechnicianSP
-- =============================================
-- Author:		John Croft
-- Create date: 10-Jan-2011
-- Title:	DN_SRReportReassignTechnicianSP
--
--	This procedure will retrieve details of SR's where the technician has been reassigned.
-- 
-- Change Control
-----------------
-- 21/01/11 jec Restrict rows returned & 1900 dates as null 
-- 28/01/11 jec #2974 add ServiceStatus parameter 
-- =============================================
	-- Add the parameters for the function here 
	@MinDateLogged datetime = null,
	@MaxDateLogged datetime = null,
	@MinSRNo	   int  = null,
	@MaxSRNo	   int  = null,
	@ViewTop	   BIT,			-- true = 500
	@TechnicianID		INT,
	@ServiceStatus   VARCHAR(20),			
	@Return		   int output	
AS
BEGIN
	SET NOCOUNT ON;
	
	declare @top INT
	if @ViewTop=1 set @top=500 else set @top = 9999
    -- Insert statements for procedure here
-- Select Audit data	
	select aa.servicerequestno,aa.datechange,aa.technicianId,
		CAST(null as DATETIME) as DateReassign,CAST(null as INT) as ReassignId
	into #reassign
	from SR_AllocationAudit aa INNER JOIN SR_ServiceRequest SR on aa.ServiceRequestNo=SR.ServiceRequestNo
			INNER JOIN SR_Allocation a on SR.ServiceRequestNo = a.ServiceRequestNo
	where (@MinDateLogged IS NULL OR CONVERT(DATETIME,CONVERT(VARCHAR,SR.DateLogged,103),103) >= @MinDateLogged) 
			and (@MaxDateLogged IS NULL OR CONVERT(DATETIME,CONVERT(VARCHAR,SR.DateLogged,103),103) <= @MaxDateLogged) 
			and (@MinSRNo IS NULL OR SR.ServiceRequestNo >= @MinSRNo) 
			and (@MaxSRNo IS NULL OR SR.ServiceRequestNo <= @MaxSRNo)
		--and ISNULL(aa.ReassignCode,'')!='' 
		and aa.DateAllocated!='1900-01-01'
		and (aa.TechnicianId=@TechnicianID or @TechnicianID=0)
		and (sr.Status=@ServiceStatus or @ServiceStatus='')		-- #2974 jec
		
	-- update Reasssign info	
	UPDATE r set DateReassign=aa.datechange,ReassignId=aa.technicianId

	from SR_AllocationAudit aa, #reassign r where aa.ServiceRequestNo=r.servicerequestno
	and aa.datechange=(select MIN(datechange) from SR_AllocationAudit aa2 where aa2.datechange>r.datechange
	and ISNULL(aa2.ReassignCode,'')!='' and aa2.DateAllocated!='1900-01-01'
	and aa2.ServiceRequestNo=r.servicerequestno)

    -- Return Data
	SELECT top (@top) SR.ServiceRequestNo
		, convert(varchar(4), SR.ServiceBranchNo) + convert(varchar(16), SR.ServiceRequestNo) [ServiceRequestNoStr]
		, SR.Status
		, SR.DateLogged, a.DateAllocated
		,t1.FirstName + ' ' + t1.LastName as AllocatedTo		
		, r.DateReassign as DateReassigned
		,t2.FirstName + ' ' + t2.LastName as ReassignedTo
		,c.Codedescript as Reason
		,case when SRE.ReturnDate  ='1900-01-01' then null else SRE.ReturnDate  end as 'DateCompleted'				 
		,DATEDIFF(d,SR.DateLogged,case when SRE.ReturnDate='1900-01-01' then GETDATE() else 
					ISNULL(SRE.ReturnDate,GETDATE()) end) as 'Days Outstanding'		
		,cp.FullName as ReassignedBy		
	FROM SR_ServiceRequest SR LEFT OUTER JOIN
		SR_Resolution SRE ON SR.ServiceRequestNo = SRE.ServiceRequestNo
		INNER JOIN SR_Allocation a on SR.ServiceRequestNo = a.ServiceRequestNo
		INNER JOIN #reassign r on SR.ServiceRequestNo = r.ServiceRequestNo
		LEFT outer JOIN SR_AllocationAudit aa on aa.ServiceRequestNo = r.ServiceRequestNo and aa.DateChange=r.DateReassign
		LEFT OUTER JOIN SR_Technician t1 on r.technicianId=t1.technicianId
		LEFT OUTER JOIN SR_Technician t2 on r.ReassignId=t2.technicianId
		LEFT OUTER JOIN code c on category='SRREASON' and code=aa.ReassignCode
		LEFT OUTER JOIN Admin.[User] cp on cp.Id = aa.ReassignedBy
	WHERE r.DateReassign is not null			

	SET @return = @@error
END
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End
