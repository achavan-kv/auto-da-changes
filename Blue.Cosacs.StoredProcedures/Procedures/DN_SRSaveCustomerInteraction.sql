/****** Object:  StoredProcedure [dbo].[DN_SRSaveCustomerInteraction]    Script Date: 10/20/2006 11:42:19 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[DN_SRSaveCustomerInteraction]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DN_SRSaveCustomerInteraction]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Peter Chong
-- Create date: 20-Oct-2006
-- Description:	Saves the customer interaction log
-- =============================================
CREATE PROCEDURE DN_SRSaveCustomerInteraction
(
	@CustId varchar(20),
	@Date datetime, 
	@Code varchar(12), 
	@EmployeeNo int, 
	@AccountNo  char(12), 
	@ServiceRequestNo int, 
	@Comments varchar(400),
	@return int output
)
AS
BEGIN

	SET NOCOUNT ON;
	
INSERT INTO [SR_CustomerInteraction]
           ([CustomerId]
           ,[Date]
           ,[Code]
           ,[EmpeeNo]
           ,[AcctNo]
           ,[ServiceRequestNo]
           ,[Comments])
     VALUES
           (@custId
           ,@Date
           ,@Code
           ,@EmployeeNo
           ,@AccountNo
           ,@ServiceRequestNo
           ,@Comments)
	
	SET @return = @@error
END
GO



