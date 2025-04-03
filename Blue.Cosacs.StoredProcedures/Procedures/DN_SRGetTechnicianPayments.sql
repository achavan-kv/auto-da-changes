
/****** Object:  StoredProcedure [dbo].[DN_SRGetTechnicianPayments]    Script Date: 11/13/2006 11:52:36 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[DN_SRGetTechnicianPayments]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DN_SRGetTechnicianPayments]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Peter Chong
-- Create date: 13-11-2006
-- Description:	Gets payments owed to technicians
-- =============================================
CREATE PROCEDURE DN_SRGetTechnicianPayments
	-- Add the parameters for the stored procedure here
	@TechnicianId	int ,
	@return			int output
AS
BEGIN
	SET NOCOUNT ON;

    -- Insert statements for procedure here
    -- UAT 485 Non courts parts and labour costs required for technician report
	SELECT convert(varchar(4), SR.ServiceBranchNo) + convert(varchar(16), SR.ServiceRequestNo) [ServiceRequestNoStr]
	    , ActualCost [Non Courts Parts]
		, LabourCost [Labour]		
		, P.TotalCost 
		, P.DateClosed
		, P.Status
	FROM   SR_TechnicianPayment P JOIN 
		SR_ServiceRequest SR ON P.ServiceRequestNo = SR.ServiceRequestNo
		INNER JOIN SR_Resolution R ON SR.ServiceRequestNo = R.ServiceRequestNo
		INNER JOIN SR_ChargeTo C ON R.ServiceRequestNo = C.ServiceRequestNo
	WHERE
		TechnicianId = @TechnicianId
		AND SortOrder = 2
	ORDER BY P.DateClosed

	SET @return = @@error
END
GO
