IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[DN_SRGetServiceRequestsForCustomerSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DN_SRGetServiceRequestsForCustomerSP]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Peter Chong
-- Create date: 20-Oct-2006
-- Description:	Returns the service requests for a customer
-- =============================================
CREATE PROCEDURE DN_SRGetServiceRequestsForCustomerSP
	@CustId varchar(20),
	@return int output
AS
BEGIN
	SET NOCOUNT ON;

  
	SELECT ServiceRequestNo,
		   convert(varchar(4), ServiceBranchNo ) + convert(varchar(16), ServiceRequestNo) [ServiceRequestNoStr]
	FROM SR_ServiceRequest	
	WHERE CustId = @CustId 
  ORDER BY ServiceRequestNo
  
	SET @return = @@error
END
GO
