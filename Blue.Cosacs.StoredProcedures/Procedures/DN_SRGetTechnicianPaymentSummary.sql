
/****** Object:  StoredProcedure [dbo].[DN_SRGetTechnicianPaymentSummary]    Script Date: 11/13/2006 11:52:36 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[DN_SRGetTechnicianPaymentSummary]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DN_SRGetTechnicianPaymentSummary]
GO

--exec DN_SRGetTechnicianPaymentSummary 5,0

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Jez Hemans
-- Create date: 15-07-2008
-- Description:	Gets a summary of charge-to costs
-- =============================================
CREATE PROCEDURE DN_SRGetTechnicianPaymentSummary
	-- Add the parameters for the stored procedure here
	@TechnicianId	int ,
	@return			int output
AS
BEGIN
	SET NOCOUNT ON;
		
	--UAT 485 Summary of totals required at foot of report
	SELECT SUM(ActualCost) AS Total,SUM(Internal) AS Internal,SUM(ExtWarranty) AS EW,SUM(Supplier) AS Supplier,	--CR1030 jec
	SUM(Deliverer) AS Deliverer, SUM(Customer) AS Customer 
	FROM SR_TechnicianPayment P INNER JOIN SR_ChargeTo sr ON P.ServiceRequestNo = sr.ServiceRequestNo
	WHERE TechnicianId = @TechnicianId AND SortOrder =5

	SET @return = @@error
END
GO
