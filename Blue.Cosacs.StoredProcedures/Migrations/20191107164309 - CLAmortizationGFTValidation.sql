
IF EXISTS (SELECT 'A' FROM sys.procedures where name = 'CLAmortizationGFTValidation')
	DROP PROC CLAmortizationGFTValidation

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Sachin Wandhare
-- Create date: 25-Oct-2019
-- Description:	Cash Loan Amortization GFT Validation
-- =============================================
CREATE PROCEDURE dbo.CLAmortizationGFTValidation
	@AcctNo			VARCHAR(12),
	@TransTypeCode  VARCHAR(3),
	@CreditDebit	INT,
	@Amount			MONEY OUT,
	@Return			INT OUT 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	SET @Return = 0
	--Variable Declaration
	DECLARE	@SettlementFig money,
		    @ClpCreditAmount money,
		    @ReturnParam int

	-------------------
    IF EXISTS (SELECT 'A' 
				FROM ACCT 
				WHERE AcctNo = @AcctNo AND
					IsAmortizedOutStandingBal = 1 AND
					isAmortized =1) --checking weather the account is IsAmortizedOutStandingBaland and isAmortized, or not
	BEGIN
		--if account is IsAmortizedOutStandingBaland and isAmortized, go for validations
		-------------------------------------------------------------------
		-- DEBIT OPERATION FOR GFT
		------------------------------------------------------------------
		IF (@CreditDebit = 1) --if selected operation is debit
		BEGIN
			
			IF (@TransTypeCode = 'CLW') -- debit operation for Cash loan BD write off
			BEGIN
				SELECT @Amount = SUM(principal) + SUM(servicechg) + SUM(adminfee) + SUM(interest)  
				FROM CLAmortizationPaymentHistory WHERE acctno = @AcctNo
			END
			IF (@TransTypeCode = 'CLA') -- debit operation for Cash loan admin fee
			BEGIN
				--This maximum amount that can be posted for admin (a debit entry) is the difference 
				--between the full admin as per the schedule & what is already earned
				SELECT @Amount = (SELECT SUM(AdminFee) FROM CLAmortizationSchedule where AcctNo = @AcctNo) - SUM(PrevAdminfee)
					FROM CLAmortizationPaymentHistory WHERE acctno = @AcctNo
			END
			IF (@TransTypeCode = 'INC') -- debit operation for Cash loans Insurance claim 
			BEGIN
				SELECT @Amount = ABS(ISNULL(SUM(TransValue),0)) FROM fintrans WHERE acctno = @AcctNo AND TransTypeCode = 'INC'
			END
			IF (@TransTypeCode = 'CLP') -- debit operation Cash loan penalty interest
			BEGIN
				SELECT @Amount = ABS(ISNULL(SUM(prevInterest),0)) FROM CLAmortizationPaymentHistory WHERE acctno = @AcctNo
				
				SELECT @ClpCreditAmount = ABS(ISNULL(SUM(TransValue),0)) 
										FROM fintrans 
										WHERE acctno = @AcctNo AND TransTypeCode = 'CLP' and TransValue <0
				IF(@ClpCreditAmount <>0)
					SET @Amount = @Amount+@ClpCreditAmount
			END
			IF (@TransTypeCode = 'CLL') -- debit operation for Cash loan late fee & reversal
			BEGIN
				SELECT @Amount = ABS(ISNULL(SUM(transvalue),0))
				FROM fintrans WHERE acctno = @AcctNo AND TransTypeCode= 'CLL'
				SET @Amount= -1 * @Amount;
			END
			IF (@TransTypeCode = 'SCC') -- debit operation for Cash loan service charge correction
			BEGIN
				--This maximum amount that can be posted for admin (a debit entry) is the difference 
				--between the full Service charge as per the schedule & what is already earned
				SELECT @Amount = (SELECT SUM(ServiceChg) FROM CLAmortizationSchedule where AcctNo = @AcctNo) - SUM(PrevServiceChg)
					FROM CLAmortizationPaymentHistory WHERE acctno = @AcctNo
			END
		END
		-------------------------------------------------------------------
		-- CREDIT OPERATION FOR GFT
		------------------------------------------------------------------
		ELSE IF (@CreditDebit = -1) --if selected operation is Credit
		BEGIN
			SET @Amount = ABS(@Amount)
			
			IF (@TransTypeCode = 'CLW') -- Credit operation for Cash loan BD write off
			BEGIN
				
				EXEC [dbo].[DN_GetEarlySettlementFigure] @AcctNo, @SettlementFig OUTPUT, @ReturnParam OUTPUT
				SELECT @Amount = @SettlementFig
			END
			IF (@TransTypeCode = 'CLA') -- Credit operation for Cash loan admin fee
			BEGIN
				SELECT @Amount = SUM(PrevAdminfee) FROM CLAmortizationPaymentHistory WHERE acctno = @AcctNo
			END
			IF (@TransTypeCode = 'INC') -- Credit operation for Cash loans Insurance claim 
			BEGIN
				EXEC [dbo].[DN_GetEarlySettlementFigure] @AcctNo, @SettlementFig OUTPUT, @ReturnParam OUTPUT
				SELECT @Amount = @SettlementFig
			END
			IF (@TransTypeCode = 'CLP') -- Credit operation Cash loan penalty interest
			BEGIN
				SELECT @Amount = ABS(SUM(interest)) FROM CLAmortizationPaymentHistory WHERE acctno = @AcctNo
			END
			IF (@TransTypeCode = 'CLL') -- Credit operation for Cash loan late fee & reversal
			BEGIN
				SELECT @Amount = ABS(ISNULL(SUM(transvalue),0))
				FROM fintrans WHERE acctno = @AcctNo AND TransTypeCode= 'FEE'
			END
			IF (@TransTypeCode = 'SCC') -- Credit operation for Cash loan service charge correction
			BEGIN
				SELECT @Amount = SUM(PrevServiceChg) FROM CLAmortizationPaymentHistory WHERE acctno = @AcctNo
				SET @Return =0
			END
		END
	END
	ELSE
	BEGIN
		--if account is not an IsAmortizedOutStandingBaland and isAmortized, go for validations
		SET @Return = -1
		
	END
END
GO
