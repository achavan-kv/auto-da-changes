
/****** Object:  StoredProcedure [dbo].[DN_CustomerAdditionalDetailsResidentialUpdate]    Script Date: 10/12/2006 09:11:12 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[DN_CustomerAdditionalDetailsResidentialUpdate]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DN_CustomerAdditionalDetailsResidentialUpdate]

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Peter Chong
-- Create date: 11-Oct-2006
-- Description:	Update financial details for the customer
-- =============================================
CREATE PROCEDURE DN_CustomerAdditionalDetailsResidentialUpdate
	-- Add the parameters for the stored procedure here
	@CustID varchar(20),
    @DateinPreviousAddress  datetime  = null,
	@DateinCurrentAddress   datetime = null,
	@ResidentialStatus		varchar(4) = null,
    @PrevResidentialStatus	varchar(4) = null,
	@PropertyType			char(4) = null ,
	@Mthlyrent				float = null,
	@return					int out
	
AS
BEGIN
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    
		
	--Update details for the previous address
	UPDATE	CustomerAdditionalDetails
	SET CustId = @CustID
		   , DateinPreviousAddress =  @DateinPreviousAddress
		   , [DateinCurrentAddress] = @DateinCurrentAddress
           , [ResidentialStatus] = @ResidentialStatus
           , [PrevResidentialStatus] = @PrevResidentialStatus
		   , [PropertyType] = @PropertyType
		   , [MonthlyRent] = @Mthlyrent
		   , [DateResidentialUpdate]  = getdate()
	WHERE CustId = @CustID 

	IF @@RowCount = 0 
	INSERT INTO [CustomerAdditionalDetails]
           ([Custid]
           ,[DateinPreviousAddress]
		   ,[DateinCurrentAddress]
           ,[ResidentialStatus]
           ,[PrevResidentialStatus]
		   ,[PropertyType]
		   ,[MonthlyRent]
		   ,[DateResidentialUpdate])
     VALUES
           (@CustID
		   , @DateinPreviousAddress
		   , @DateinCurrentAddress
           , @ResidentialStatus
           , @PrevResidentialStatus
		   , @PropertyType
		   , @Mthlyrent
		   , getdate()
          )
	
	SET @return = @@error	

END
GO
