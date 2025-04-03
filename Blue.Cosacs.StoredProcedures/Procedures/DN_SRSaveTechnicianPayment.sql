
/****** Object:  StoredProcedure [dbo].[DN_SRSaveTechnicianPayment]    Script Date: 11/13/2006 13:41:33 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[DN_SRSaveTechnicianPayment]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DN_SRSaveTechnicianPayment]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Peter Chong
-- Create date: 13-Nov-2006
-- Description:	Saves details for a technician payment
-- =============================================
CREATE PROCEDURE DN_SRSaveTechnicianPayment
	-- Add the parameters for the stored procedure here
	@TechnicianId		int,
	@ServiceRequestNo	int,
	@DateClosed			datetime,
	@TotalCost			money,
	@Status				char(1),
	@Return				int output
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	

    
	-- Insert statements for procedure here
	UPDATE [SR_TechnicianPayment]
	SET 
       [TotalCost] = @TotalCost
      ,[Status] =	 @Status
      ,DateClosed = @DateClosed     --UAT 394 DateClosed may have changed
	WHERE 
		TechnicianId     =  @TechnicianId AND
		ServiceRequestNo =  @ServiceRequestNo 
--		AND
--		DateClosed		 =	@DateClosed 

	IF @@ROWCOUNT = 0 
	INSERT INTO [SR_TechnicianPayment]
           ([TechnicianId]
           ,[ServiceRequestNo]
           ,[DateClosed]
           ,[TotalCost]
           ,[Status])
     VALUES
           (@TechnicianId
           ,@ServiceRequestNo
           ,@DateClosed
           ,@TotalCost
           ,@Status)

	--IF @Status = 'P' --Create a transaction for payments
		
	SET @return = @@error
END
GO
