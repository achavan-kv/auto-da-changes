/****** Object:  StoredProcedure [dbo].[DN_SRGetFoodLossSP]    Script Date: 10/19/2006 15:07:59 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[DN_SRGetFoodLossSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DN_SRGetFoodLossSP]

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Peter Chong
-- Create date: 19-Oct-2006
-- Description:	Gets the food loss items for a service request
-- =============================================
CREATE PROCEDURE DN_SRGetFoodLossSP
(	
	@ServiceRequestNo int,
	@return int output
)
AS
BEGIN
	SET NOCOUNT ON
	
	SELECT 	ServiceRequestNo
		, ItemDescription
		, ItemValue
	FROM SR_FoodLoss
	WHERE  ServiceRequestNo = @ServiceRequestNo 
	
	SET @return = @@error
END
GO
