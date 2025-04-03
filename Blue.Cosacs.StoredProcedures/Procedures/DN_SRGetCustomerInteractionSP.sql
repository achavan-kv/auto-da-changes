/****** Object:  StoredProcedure [dbo].[DN_SRGetCustomerInteractionSP]    Script Date: 10/20/2006 10:46:15 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[DN_SRGetCustomerInteractionSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DN_SRGetCustomerInteractionSP]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Peter Chong
-- Create date: 20-Oct-2006
-- Description:	Gets the interactions for a customer
-- Date      By  Description
-- ----      --  -----------
-- 17/07/12  IP  #10358 - changed to Left join onto code table
-- =============================================
CREATE PROCEDURE DN_SRGetCustomerInteractionSP 
(	
	@CustId varchar(20),
	@return int output
)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT  
       [Date]
      , C.CodeDescript [Description]
      , [EmpeeNo]
      , I.[AcctNo]
      , I.[ServiceRequestNo]
	  , dbo.fn_SRGetServiceRequestNo(I.[ServiceRequestNo])  [ServiceRequestNoStr]
      , I.[Comments]
	  , I.Code
	  
	--FROM [SR_CustomerInteraction] I JOIN 
	FROM [SR_CustomerInteraction] I LEFT JOIN 
		Code C ON C.Code = I.Code AND C.Category = 'SRCUSTINT' 
	WHERE CustomerId = @CustId
	ORDER BY [Date] DESC

	SET @return = @@error

END
GO

