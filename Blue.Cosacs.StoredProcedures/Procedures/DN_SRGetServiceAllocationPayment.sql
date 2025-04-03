/****** Object:  StoredProcedure [dbo].[DN_SRGetServiceAllocationPayment]    Script Date: 10/17/2006 13:39:23 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[DN_SRGetServiceAllocationPayment]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DN_SRGetServiceAllocationPayment]


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Peter Chong
-- Create date: 16-Oct-2006
-- Description:	Gets the service allocations for a technician with the labour payments owed to them
-- =============================================
CREATE PROCEDURE DN_SRGetServiceAllocationPayment
(	
	@TechnicianId int, 
	@return int out
)
AS
BEGIN
	
	SET NOCOUNT ON;

    SELECT dbo.fn_SRGetServiceRequestNo(S.[ServiceRequestNo]) [ServiceRequestNoStr]
      , S.Status
	  , SUM(ISNULL(P.TotalCost, 0)) LabourCost
	FROM SR_ServiceRequest S INNER JOIN 
		[SR_Allocation] A		 ON S.ServiceRequestNo = A.ServiceRequestNo LEFT OUTER JOIN
		[SR_TechnicianPayment] P ON P.TechnicianId = A.TechnicianId AND P.ServiceRequestNO = S.ServiceRequestNo AND P.Status IN ('', 'H')
	WHERE A.TechnicianId = @TechnicianId --AND Status IN ('A', 'E')
	
	GROUP BY  dbo.fn_SRGetServiceRequestNo(S.[ServiceRequestNo])
      , S.Status
	
	SET @return = @@error
END
GO


