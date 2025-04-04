

GO

-- This SQL Region Drops Stored Procedure From Database If It Is Already Exists.
IF EXISTS(
			SELECT	1
			FROM	dbo.sysobjects
			WHERE	id = OBJECT_ID('[dbo].[SaveCustomerMMI]')
					AND OBJECTPROPERTY(id, 'IsProcedure') = 1
		  )
BEGIN
	DROP PROCEDURE [dbo].[SaveCustomerMMI]
END

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =======================================================================================
-- Project			: CoSaCS.NET
-- Procedure Name   : SaveCustomerMMI
-- Author			: Amit Vernekar
-- Create Date		: 10 July 2020
-- Description		: This procedure is used to calculate MMI value for customer 
--					  based on current score and Disposible Income percentage with respective MMI percentage matrix.

-- Change Control
-- --------------
-- Date			By			Description
-- ----			--			-----------
-- 
-- =======================================================================================
CREATE PROCEDURE [dbo].[SaveCustomerMMI]
@CustId VARCHAR(20),
@UserId INT,
@ReasonChanged VARCHAR(32),
@Return INT OUT
	
AS
BEGIN
	
	SET NOCOUNT ON;

	DECLARE @Score SMALLINT
			, @NetIncome FLOAT, @AdditionalIncome FLOAT, @UtilityCommitment FLOAT, @LoanCreCardCommitment FLOAT, @MiscLivExpCommitment FLOAT
			, @OtherPayments FLOAT, @AdditionalExpenditure1 FLOAT, @AdditionalExpenditure2 FLOAT, @Mortage FLOAT
			, @DisposibleIncome FLOAT = 0, @MMIPercentage FLOAT = 0, @MMI FLOAT = 0
			, @AcctNo VARCHAR(12) = '', @DateProp DATETIME, @IsSpouseWorking BIT = 0
			
	
	SET @Return = 0
	----------------------------------   Populate current score of customer
	SELECT  @Score = P.points
			, @NetIncome = CAST(ISNULL(mthlyincome, 0) AS FLOAT)
			, @AdditionalIncome = CAST(ISNULL(AddIncome, 0) AS FLOAT)
			, @UtilityCommitment = CAST(ISNULL(P.Commitments1, 0) AS FLOAT) 
			, @LoanCreCardCommitment = CAST(ISNULL(P.Commitments2, 0) AS FLOAT)
			, @MiscLivExpCommitment = CAST(ISNULL(P.Commitments3, 0) AS FLOAT) 
			, @OtherPayments = CAST(ISNULL(P.otherpmnts, 0) AS FLOAT)
			, @AdditionalExpenditure1 = CAST(ISNULL(P.additionalexpenditure1, 0) AS FLOAT)
			, @AdditionalExpenditure2 = CAST(ISNULL(P.additionalexpenditure2, 0) AS FLOAT)
			, @AcctNo = AcctNo
			, @DateProp = dateprop
			, @IsSpouseWorking = IsSpouseWorking
	FROM	proposal P WITH(NOLOCK)
	WHERE	P.custid = @CustId
			AND dateprop = (SELECT	MAX(MP.dateprop) 
							FROM	proposal MP WITH(NOLOCK)
							WHERE	MP.custid = P.custid)

	------------------------------------- Populate customer Disposible Income

	IF EXISTS (SELECT 1 FROM CustomerAdditionalDetails WITH(NOLOCK) WHERE CustId = @CustID AND DateFinancialUpdate IS NOT NULL)
	BEGIN
		
		SELECT	@NetIncome = CAST(ISNULL([mthlyincome], 0) AS FLOAT)
				, @AdditionalIncome = CAST(ISNULL([AdditionalIncome], 0) AS FLOAT)
				, @UtilityCommitment = CAST(ISNULL([Commitments1], 0) AS FLOAT)
				, @LoanCreCardCommitment = CAST(ISNULL([Commitments2], 0) AS FLOAT)
				, @MiscLivExpCommitment = CAST(ISNULL([Commitments3], 0) AS FLOAT)
				, @OtherPayments = CAST(ISNULL([OtherPayments], 0) AS FLOAT) 
				, @AdditionalExpenditure1 = CAST(ISNULL([AdditionalExpenditure1], 0) AS FLOAT)
				, @AdditionalExpenditure2 = CAST(ISNULL([AdditionalExpenditure2], 0) AS FLOAT)
				, @Mortage = CAST(ISNULL(MonthlyRent, 0) AS FLOAT)
		FROM	[CustomerAdditionalDetails] WITH (NOLOCK)
		WHERE	CustId = @CustId

	END	
	ELSE 
	BEGIN
		-- If Customer Additional Details are not present inside [CustomerAdditionalDetails] then 
		-- 1)	@NetIncome , @AdditionalIncome, @UtilityCommitment, @LoanCreCardCommitment, @MiscLivExpCommitment = [Commitments3]
		--		@OtherPayments, @AdditionalExpenditure1, @AdditionalExpenditure2 
		--      Need to pupulate from Proposal table which we already populated above.
		-- 2)   @Mortage - Need to pupulate from Proposal table which we already populated above.
		SELECT	@Mortage = CAST(ISNULL(CurrAD.mthlyrent, 0) AS FLOAT) 
		FROM	CustAddress CurrAD WITH (NOLOCK)                        
				LEFT OUTER JOIN Custaddress PrevAD WITH (NOLOCK)
				ON  CurrAD.Custid = PrevAD.CustID   
					AND PrevAD.datein = (	SELECT	MAX(datein) 
											FROM	Custaddress CA2 WITH (NOLOCK)
											WHERE	CA2.custid = CurrAD.custid  
													AND CA2.datein < CurrAD.datein  
													AND CA2.addtype = 'H')  
		WHERE	CurrAD.addtype = 'H' 
				AND CurrAD.custid = @CustID
				AND CurrAD.datein = (SELECT MAX(datein) 
									 FROM	Custaddress CA WITH (NOLOCK)
									 WHERE	CA.Custid = CurrAD.custid  
											AND CA.addtype = 'H')  

	END

	DECLARE @ApplyNewDispIncomeChanges BIT = 0

	SELECT	@ApplyNewDispIncomeChanges = ISNULL([Value], 0) 
	FROM	[dbo].[CountryMaintenance] WITH(NOLOCK)
	WHERE	CodeName ='ApplyNewDispIncomeChanges'

	-- Calulate Desposible Income 
	IF(@ApplyNewDispIncomeChanges = 0)
	BEGIN
	
		SET @DisposibleIncome =	  (@NetIncome + @AdditionalIncome)  -- Monthly Income
								- (	@UtilityCommitment + @LoanCreCardCommitment + @MiscLivExpCommitment 
									+ @OtherPayments + @AdditionalExpenditure1 + @AdditionalExpenditure2)  -- Monthly Commitments
								- (@Mortage) -- Monthly Rent/Mort

	END
	ELSE
	BEGIN

		DECLARE @SpouseWorking BIT = 0
		DECLARE @MonthlyIncome FLOAT, @SpouseRentFactor FLOAT = 0, @DependentSpendFactor FLOAT, @ApplicantSpendFactor FLOAT
		DECLARE @Maritalstat VARCHAR(5), @NoOffDependants INT , @DependentSpendReturn INT, @ApplicantSpendReturn INT

		SET @MonthlyIncome = (@NetIncome + @AdditionalIncome)

		SELECT	@Maritalstat = Maritalstat
				,@NoOffDependants = Dependants 
		FROM	Customer WITH(NOLOCK) 
		WHERE	custid = @CustId

		SELECT	@SpouseRentFactor = ISNULL([Value], 0) 
		FROM	[dbo].[CountryMaintenance] WITH(NOLOCK)
		WHERE	CodeName = 'SpouseRentFactor'

		IF(@Maritalstat = 'M' AND ISNULL(@IsSpouseWorking, 0) = 1)
		BEGIN
			SET @Mortage = ((@SpouseRentFactor/100) * @Mortage)
		END

		EXEC [dbo].[GetDependentSpendFactorByVal] @NoOffDependants, @DependentSpendFactor OUT, @DependentSpendReturn OUT
		DECLARE @MonthlyIncomeVarchar VARCHAR(20)
		SET @MonthlyIncomeVarchar = CAST(CAST(@MonthlyIncome AS DECIMAL(20)) AS VARCHAR(20))
		EXEC [dbo].[GetApplicantSpendFactorByVal] @MonthlyIncomeVarchar, @ApplicantSpendFactor OUT, @ApplicantSpendReturn OUT

		

		SET @DisposibleIncome = @MonthlyIncome - (	  @Mortage
													+ ( @NoOffDependants * (@DependentSpendFactor / 100) * @MonthlyIncome)
													+ (                    (@ApplicantSpendFactor / 100) * @MonthlyIncome)
												 )

	END


	SELECT	TOP 1 @MMI =  (ISNULL(M.MMIPercentage, 0) * @DisposibleIncome)/100
	FROM	MMIMatrix M WITH(NOLOCK)
	WHERE	@Score BETWEEN M.FromScore AND M.ToScore
	
	-- Check if customer record present inside CustomerMMI table
	-- If record present for customer then update new MMI value otherwise insert value of it
	IF EXISTS(SELECT 1 FROM CustomerMMI WHERE CustId = @CustId)
	BEGIN
		UPDATE	CustomerMmi
		SET		MmiLimit = CAST(@MMI AS MONEY)
		WHERE	CustId = @CustId
	END
	ELSE
	BEGIN
		INSERT INTO CustomerMmi (CustId, MmiLimit)
		VALUES(@CustId, CAST(@MMI AS MONEY))
	END
	

	INSERT INTO [dbo].[CustomerMmiHist] ([CustId], [AcctNo], [DateProp], [Points], [MmiLimit], [DateChange], [EmpNo], [ReasonChanged])
	VALUES (@CustId, @AcctNo, @DateProp, @Score, CAST(@MMI AS MONEY), GETDATE(), @UserId, @ReasonChanged)

END

