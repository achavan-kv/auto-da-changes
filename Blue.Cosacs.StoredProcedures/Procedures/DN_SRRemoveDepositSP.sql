
/****** Object:  StoredProcedure [dbo].[DN_SRRemoveDepositSP]    Script Date: 11/03/2006 15:37:31 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[DN_SRRemoveDepositSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DN_SRRemoveDepositSP]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Peter Chong
-- Create date: 2006-Nov-03
-- Description:	Removes the deposit for a service request 
-- =============================================
CREATE PROCEDURE DN_SRRemoveDepositSP
	-- Add the parameters for the stored procedure here
	@ServiceRequestNo int,
	@return int output
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	UPDATE SR_ServiceRequest 
	SET DepositPaid = 'Y'
		,DepositAmount = 0
	WHERE 
		ServiceRequestNo = @ServiceRequestNo

	SET @return = @@error
END
GO
