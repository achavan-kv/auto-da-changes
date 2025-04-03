IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID('[dbo].[fn_CLAmortizationCalcDailyOutstandingBalance]')
			AND xtype IN (
				'FN'
				,'IF'
				,'TF'
				)
		)
	DROP FUNCTION [dbo].[fn_CLAmortizationCalcDailyOutstandingBalance]

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO
--=============================================================
-- Author : Ashwini Akula
-- Created date : 09/07/2019
--Description - Returns outstanding balance having components like principle, admin and int(penalty interest) for accounts
--=============================================================

CREATE FUNCTION [fn_CLAmortizationCalcDailyOutstandingBalance] (
	-- Add the parameters for the function here
	@AcctNo VARCHAR(12) 
	)
RETURNS MONEY
AS
BEGIN
	--Variable Declaration
	DECLARE @TotalPrinciple MONEY
		,@TotalAdminFee MONEY
		,@Int MONEY
		,@OutstandingBalance MONEY

	--Get SUM of all remaining principle into  @TotalPrinciple
	SELECT @TotalPrinciple = SUM(principal)
	FROM CLAmortizationPaymentHistory WITH (NOLOCK)
	WHERE acctno = @AcctNo

	-- Get sum of admin fee till current month into @totalServiceCharge.
	SELECT @TotalAdminFee = dbo.fn_CLAmortizationReturnAdminFee(@AcctNo)

	SELECT @Int = SUM(interest)
	FROM CLAmortizationPaymentHistory
	WHERE acctno = @AcctNo

	---Calculate Outstanding Balance as per below formula.
	SET @OutstandingBalance = @TotalPrinciple + @TotalAdminFee + @Int

	RETURN @OutstandingBalance
END
GO

