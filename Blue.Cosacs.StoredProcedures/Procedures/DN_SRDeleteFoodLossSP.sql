/****** Object:  StoredProcedure [dbo].[DN_SRDeleteFoodLossSP]    Script Date: 10/19/2006 15:18:31 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[DN_SRDeleteFoodLossSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DN_SRDeleteFoodLossSP]
GO 

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Peter Chong
-- Create date: 19-Oct-2006
-- Description:	Deletes all food loss items for a service request no
-- =============================================
CREATE PROCEDURE DN_SRDeleteFoodLossSP
(
	@ServiceRequestNo int, 
	@return int output
)
AS
BEGIN
	SET NOCOUNT ON;

	DELETE 
	FROM SR_FoodLoss 
	WHERE ServiceRequestNo = @ServiceRequestNo
	
	SET @return = @@error
END
GO
