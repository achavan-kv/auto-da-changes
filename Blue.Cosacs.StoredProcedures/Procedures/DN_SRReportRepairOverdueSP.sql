IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[DN_SRReportRepairOverdueSP]') 
	AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DN_SRReportRepairOverdueSP]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE DN_SRReportRepairOverdueSP
-- =============================================
-- Author:		John Croft
-- Create date: 29-Nov-2010
-- Title:	DN_SRReportRepairOverdueSP
--
--	This procedure will retrieve details of SR's where the Repair is Overdue.
-- 
-- Change Control
-----------------
-- 07/01/11 jec CR1030 - return Status and date change
-- 21/01/11 jec Restrict rows returned & 1900 dates as null 
-- 01/07/11 ip  CR1254 -RI - #3992 - Return CourtsCode/IUPC  
-- =============================================
	-- Add the parameters for the function here 
	@MinDateLogged datetime = null,
	@MaxDateLogged datetime = null,
	@MinSRNo	   int  = null,
	@MaxSRNo	   int  = null,
	@ViewTop	   BIT,			-- true = 500
	@TechnicianId	INT,	--CR1030	
	@Return		   int output	
AS
BEGIN
	SET NOCOUNT ON;
	declare @top INT	
	if @ViewTop=1 set @top=500 else set @top = 9999

    -- Insert statements for procedure here
	SELECT  Distinct top (@top) SR.ServiceRequestNo
		, convert(varchar(4), SR.ServiceBranchNo) + convert(varchar(16), SR.ServiceRequestNo) [ServiceRequestNoStr]
		, SR.Status
		, SR.DateLogged,d.SlotDate as 'Repair Date',CAST(t.TechnicianId as VARCHAR(3)) + ': ' + t.FirstName + ' ' + t.LastName as 'Technician'
		--, SR.ProductCode 
		, isnull(SI.IUPC, SR.ProductCode) as ProductCode			--IP - 01/07/11 - CR1254 - RI - #3992
		, isnull(SI.itemno, '') as CourtsCode						--IP - 01/07/11 - CR1254 - RI - #3992
		, SR.Description
		, case when SR.DateChange ='1900-01-01' then null else SR.DateChange end as 'DateChange'			
	FROM SR_ServiceRequest SR LEFT OUTER JOIN
		SR_Resolution SRE ON SR.ServiceRequestNo = SRE.ServiceRequestNo
		INNER JOIN SR_TechnicianDiary d on SR.ServiceRequestNo = d.ServiceRequestNo
		INNER JOIN SR_Technician t on d.TechnicianId = t.TechnicianId
		LEFT OUTER JOIN StockInfo SI ON SI.ID = SR.ItemID		--IP - 01/07/11 - CR1254 - RI - #3992
	WHERE
		(@MinDateLogged IS NULL OR CONVERT(DATETIME,CONVERT(VARCHAR,SR.DateLogged,103),103) >= @MinDateLogged) 
		and (@MaxDateLogged IS NULL OR CONVERT(DATETIME,CONVERT(VARCHAR,SR.DateLogged,103),103) <= @MaxDateLogged) 
		and (@MinSRNo IS NULL OR SR.ServiceRequestNo >= @MinSRNo) 
		and (@MaxSRNo IS NULL OR SR.ServiceRequestNo <= @MaxSRNo) 
		and ISNULL(SRE.DateClosed, '1900-01-01') = '1900-01-01'
		and sr.status not in('C','R')		-- not closed/settled 
		and CONVERT(DATETIME,CONVERT(VARCHAR,GETDATE(),103),103)>d.SlotDate		-- past repair date
		and SRE.Resolution=''	-- resolution not set
		and (t.TechnicianId=@TechnicianID or @TechnicianID=0)		

	SET @return = @@error
END
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End
