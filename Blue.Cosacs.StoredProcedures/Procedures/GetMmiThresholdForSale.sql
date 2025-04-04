
GO

-- This SQL Region Drops Stored Procedure From Database If It Is Already Exists.
IF EXISTS(
			SELECT	1
			FROM	dbo.sysobjects
			WHERE	id = OBJECT_ID('[dbo].[GetMmiThresholdForSale]')
					AND OBJECTPROPERTY(id, 'IsProcedure') = 1
		  )
BEGIN
	DROP PROCEDURE [dbo].[GetMmiThresholdForSale]
END

GO

-- This SQL Region Create Stored Procedure.
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =======================================================================================================
-- Project			: CoSaCS.NET
-- Procedure Name   : GetMmiThresholdForSale
-- Author			: Zensar (Amit)
-- Create Date		: 10 July 2020
-- Description		: This stored procedure is used to get MMI applicability and customer MMI details for selected sale options.

-- Change Control
-- --------------
-- Ver	Date(YYYY-MM-DD)	By					Description
-- ---	----------------	----------------	------------
-- 001	2020-07-10			Zensar (Amit)		Get MMI applicability and customer MMI details for selected sale options.
-- =======================================================================================================
CREATE PROCEDURE [dbo].[GetMmiThresholdForSale]
@CustId VARCHAR(20),
@AcctNo VARCHAR(20),
@TermsType VARCHAR(3),
@UserId INT,
@IsMmiAllowed BIT OUT,
@MmiLimit DECIMAL OUT,
@MmiThreshold DECIMAL OUT,
@Return INT OUT
	
AS
BEGIN
	
	SET NOCOUNT ON;
	
	DECLARE @IsMmiEnabledForCountry BIT = 0
	DECLARE @IsMmiEnabledForCustAssignedCode BIT = 0
	DECLARE @IsMmiEnabledForCustAssignedTermType BIT = 0
	DECLARE @TermTypeMmiThresholdPerc FLOAT = 0
	DECLARE @CustMmiLimit FLOAT = 0
	DECLARE @AcctOpenDate DATETIME
	DECLARE @MmiFeatureId INT
	DECLARE @MMIActivationDate DATETIME
	
	SET @IsMmiAllowed = 0
	SET	@MmiLimit = 0
	SET @MmiThreshold = 0
	SET @Return = 0 
	SET @AcctNo = REPLACE(@AcctNo, '-', '')

	-- MMI Feature Id Const - Do not change this 
	SET @MmiFeatureId = 1

	-- Check MMI applicable setting enabled for country
	IF  EXISTS (SELECT Value FROM [CountryMaintenance] WITH(NOLOCK) WHERE CodeName = 'EnableMMI')
	BEGIN
		SELECT	@IsMmiEnabledForCountry = CAST(ISNULL(value, 0) AS BIT) 
		FROM	CountryMaintenance WITH(NOLOCK)
		WHERE	CodeName = 'EnableMMI'	

		IF(@IsMmiEnabledForCountry = 1)
		BEGIN
			
			IF(@AcctNo = '000000000000')
			BEGIN
				SET @AcctOpenDate = GETDATE();
			END
			ELSE
			BEGIN
				SELECT	@AcctOpenDate = dateacctopen 
				FROM	[dbo].[Acct] AS A WITH(NOLOCK) 
				WHERE	A.acctno = @AcctNo
			END

			SELECT	@MMIActivationDate = ActivationDate
			FROM	[dbo].[FeatureConfiguration] AS FC WITH(NOLOCK)
			WHERE	Id = @MmiFeatureId

		END

	END

	IF(@IsMmiEnabledForCountry = 1 AND (@AcctOpenDate >= @MMIActivationDate) )
	BEGIN

		-- Check Term Type selected for sale is configured with MMI applicable setting and respective Threshold Percentage.
		SELECT	@IsMmiEnabledForCustAssignedTermType = ISNULL(TT.IsMmiActive, 0)
				, @TermTypeMmiThresholdPerc = ISNULL(TT.MmiThresholdPercentage, 0)
		FROM	[dbo].[TermsType] AS TT WITH(NOLOCK)
		WHERE	TT.TermsType = @TermsType

		IF(@IsMmiEnabledForCustAssignedTermType = 1)
		BEGIN

			-- If no code assigned to customer then MMI is applicable  to customer.
			IF NOT EXISTS(	SELECT	Custid
							FROM	[dbo].[custcatcode] AS CSC WITH(NOLOCK)
									INNER JOIN [dbo].[Code] AS C WITH(NOLOCK)
									ON	CSC.Code = C.Code
							WHERE	CSC.custid = @CustId
									AND CSC.datedeleted IS NULL
									AND	C.category in ('CC1', 'CC2')
						  )
			BEGIN
					SET @IsMmiEnabledForCustAssignedCode = 1
			END

			-- Check if any customer code configured for the customer is MMI applicable.
			IF( @IsMmiEnabledForCustAssignedCode = 0
				AND
				EXISTS(	    SELECT	IsMmiApplicable
							FROM	[dbo].[custcatcode] AS CSC WITH(NOLOCK)
									INNER JOIN [dbo].[Code] AS C WITH(NOLOCK)
									ON	CSC.Code = C.Code
									INNER JOIN [dbo].[CodeConfiguration] AS CO WITH(NOLOCK)
									ON	C.category = CO.Category
										AND C.code = CO.Code
							WHERE	CSC.custid = @CustId
									AND CSC.datedeleted IS NULL
									AND	C.category in ('CC1', 'CC2')
									AND ISNULL(CO.IsMmiApplicable, 0) = 1
					   )
			)
			BEGIN
					SET @IsMmiEnabledForCustAssignedCode = 1
			END


			IF(@IsMmiEnabledForCountry = 1 AND @IsMmiEnabledForCustAssignedCode = 1)
			BEGIN
				
				SET @IsMmiAllowed = 1

				-- Check if MmiLimit is calculated for customer
				-- "MmiLimit value not calculated for customer" and "Score calculated for customer" then calculated MmiLimit for customer.
				IF NOT EXISTS(SELECT C.MmiLimit FROM [dbo].[CustomerMmi] AS C WITH(NOLOCK) WHERE C.CustId = @CustId)
				BEGIN

					IF EXISTS(  SELECT  P.points
								FROM	proposal P WITH(NOLOCK)
								WHERE	P.custid = @CustId
										AND dateprop = (SELECT	MAX(MP.dateprop) 
														FROM	proposal MP WITH(NOLOCK)
														WHERE	MP.custid = P.custid)
								)
					BEGIN
						EXEC [dbo].[SaveCustomerMMI] @CustId, @UserId, 'Sale', @Return OUT
					END

				END

				-- Populate MMI limit and MMI Threshold Percentage value
				SELECT  @MmiLimit = ISNULL(C.MmiLimit,0)
						, @MmiThreshold = ((ISNULL(C.MmiLimit,0) * @TermTypeMmiThresholdPerc)/100)
				FROM	[dbo].[CustomerMmi] AS C WITH(NOLOCK)
				WHERE	C.CustId = @CustId

			END
		END
	END
END