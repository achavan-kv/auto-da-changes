
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[DN_CustomerAdditionalDetailsFinancialGetSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DN_CustomerAdditionalDetailsFinancialGetSP]

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Peter Chong
-- Create date: 10-Oct-2006
-- Description:	Gets Customer residential and financial details from the additional details table if a record exists otherwise from the proposal
-- =============================================
CREATE PROCEDURE DN_CustomerAdditionalDetailsFinancialGetSP
	@CustId varchar(20),
	@return int out
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	IF EXISTS (SELECT * FROM CustomerAdditionalDetails WHERE CustId = @CustID AND DateFinancialUpdate IS NOT NULL)
		SELECT 
				CustId
			  , mthlyincome [MonthlyIncome]
			  , [AdditionalIncome]
			  , [Commitments1]
			  , [Commitments2]
			  , [Commitments3]
			  ,[OtherPayments] 
			  ,[AdditionalExpenditure1]
			  ,[AdditionalExpenditure2]
			  ,CCardNo1
			  ,CCardNo2
			  ,CCardNo3
			  ,CCardNo4
			  ,DueDayId 
			  , BankAccountName 
			  , PaymentMethod	
		FROM [CustomerAdditionalDetails]  
		WHERE CustId = @CustID 
		
	ELSE
		SELECT top 1  
				P.CustId
				,cast([mthlyincome] as money) [MonthlyIncome]
				, P.AddIncome  [AdditionalIncome]
				, P.Commitments1 
				, P.Commitments2 
				, P.Commitments3 
				, cast(P.otherpmnts as money)  [OtherPayments]
				, P.additionalexpenditure1
				, P.additionalexpenditure2
				, CCardNo1
				, CCardNo2
				, CCardNo3
			    , CCardNo4
				, null [DueDayId]		 --the following three fields are blank when they come from the proposal 
				, cast(null as varchar(40)) [BankAccountName]
				, cast(null as varchar(12)) [PaymentMethod]
			
		FROM Proposal P  
		WHERE P.CustID = @CustID 
		ORDER BY dateprop DESC

	SET @return = @@Error

END
GO



SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


