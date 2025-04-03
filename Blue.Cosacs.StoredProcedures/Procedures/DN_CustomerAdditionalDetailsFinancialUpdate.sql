
/****** Object:  StoredProcedure [dbo].[DN_CustomerAdditionalDetailsFinancialUpdate]    Script Date: 10/12/2006 09:11:59 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[DN_CustomerAdditionalDetailsFinancialUpdate]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DN_CustomerAdditionalDetailsFinancialUpdate]

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Peter Chong
-- Create date: 11-Oct-2006
-- Description:	Update financial details for the customer
-- =============================================
CREATE PROCEDURE DN_CustomerAdditionalDetailsFinancialUpdate
	-- Add the parameters for the stored procedure here
	   @CustID					varchar(20),
	   @mthlyincome				money = null,
	   @AdditionalIncome		money = null,
	   @Commitments1			money = null,
	   @Commitments2			money = null,
	   @Commitments3			money = null,
	   @OtherPayments			money = null,
	   @AdditionalExpenditure1  money = null,
	   @AdditionalExpenditure2  money = null,
	   @CCardNo1				char(4) = '',
	   @CCardNo2				char(4) = '',
	   @CCardNo3				char(4) = '',
	   @CCardNo4				char(4) = '',
	   @DueDayId				int,  
	   @BankAccountName			varchar(40), 
	   @PaymentMethod			varchar(12), 
	   @return	int				output
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Update the additional details
	UPDATE	CustomerAdditionalDetails
	SET		
			mthlyincome	= @mthlyincome
			, AdditionalIncome = @AdditionalIncome
			, Commitments1 = @Commitments1
			, Commitments2 = @Commitments2
			, Commitments3 = @Commitments3
			, OtherPayments = @OtherPayments
			, AdditionalExpenditure1 = @AdditionalExpenditure1
			, AdditionalExpenditure2 = @AdditionalExpenditure2
			, CCardNo1 = @CCardNo1
		    , CCardNo2 = @CCardNo2				
		    , CCardNo3 = @CCardNo3				
		    , CCardNo4 = @CCardNo4	
		    , DueDayId = @DueDayId				  
			, BankAccountName = @BankAccountName		
			, PaymentMethod	= 	@PaymentMethod	 
			, DateFinancialUpdate = getdate()			
	   
	WHERE CustID = @CustID
	
	--Peform an insert if update fails
	IF @@RowCount = 0 
	INSERT INTO [CustomerAdditionalDetails]
           ( [Custid]
           , [mthlyincome]
           , [AdditionalIncome]
           , [Commitments1]
           , [Commitments2]
           , [Commitments3]
           , [OtherPayments]
           , [AdditionalExpenditure1]
           , [AdditionalExpenditure2]
		   , CCardNo1
		   , CCardNo2
		   , CCardNo3
		   , CCardNo4
			, DueDayId 
			, BankAccountName 
			, PaymentMethod	
		   , DateFinancialUpdate)
     VALUES
           (   @CustID				
			  , @mthlyincome				
			  , @AdditionalIncome		
			  , @Commitments1			
	          , @Commitments2			
	          , @Commitments3			
	          , @OtherPayments			
	          , @AdditionalExpenditure1  
	          , @AdditionalExpenditure2  
			  , @CCardNo1
			  , @CCardNo2				
		      , @CCardNo3				
		      , @CCardNo4
			  , @DueDayId				  
			  , @BankAccountName		
			  , @PaymentMethod	 
			  , getdate()	
			)		
	SET @return = @@error
END
GO
