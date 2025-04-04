
IF EXISTS (SELECT 'A' FROM sys.procedures WHERE name = 'CLAmortizationGFTTransactions')
	DROP PROC CLAmortizationGFTTransactions
GO
----------------------------------------------------------------------------
-- Script Name	:	CLAmortizationGFTTransactions.sql
-- Details		:	General Finance transaction for Cash loan Amortized account
-- Created by	:	Rahul Dubey, Zensar
-- Date			:	25/Oct/2019
-- Version		:	1.0		-- 25/10/2019	--	Created new
------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[CLAmortizationGFTTransactions] 
	-- Add the parameters for the stored procedure here 
	@acctNo varchar(12),
	@amount money,
	@creditDebit int,
	@transType varchar(5),
	@return int out

AS
BEGIN
	SET NOCOUNT ON
	--Variable Declaration
	DECLARE @maxInstallment int
	SET @return =0;
	------------------------------------------------------------------------
	-- Logic to process for Credit transaction of diffrent transtype codes
	------------------------------------------------------------------------
	IF(@creditDebit = -1)
	BEGIN
		-- Convert amount to absolute value
		IF(@amount<0)
			SET @amount = -1*@amount
		------------------------------------------------------------------------
		-- CLW (Cash Loan Write off)
		------------------------------------------------------------------------
		IF(@transtype = 'CLW')
		BEGIN
			EXEC CLAmortizationWriteOffAccountBalance @acctNo,@return
		END
		------------------------------------------------------------------------
		-- CLA (Cash Loan Admin Fee adjustment)
		------------------------------------------------------------------------
		ELSE IF(@transtype = 'CLA')
		BEGIN
			EXEC CLAmortizationUpdateAccountBalance @acctNo, @amount,@return, 1
		END
		------------------------------------------------------------------------
		-- INC (Cash Loan insurance claim)
		------------------------------------------------------------------------
		ELSE IF(@transtype = 'INC')
		BEGIN
			EXEC CLAmortizationUpdateAccountBalance @acctNo, @amount,@return, 1
		END
		------------------------------------------------------------------------
		-- CLP (Cash Loan penalty interest Reversal)
		------------------------------------------------------------------------
		ELSE IF(@transtype = 'CLP')
		BEGIN
			SELECT @maxInstallment = InstallmentNo FROM [dbo].[CLAmortizationPaymentHistory]
			WHERE [AcctNo] = @acctNo

			UPDATE [dbo].[CLAmortizationPaymentHistory]
			SET Interest = Interest - @amount
			WHERE [AcctNo] = @acctNo AND InstallmentNo = @maxInstallment
			 
		END
		------------------------------------------------------------------------
		-- CLL (Cash Loan Late Fee Reversal)
		------------------------------------------------------------------------
		ELSE IF(@transtype = 'CLL')
		BEGIN
			EXEC CLAmortizationUpdateAccountBalance @acctNo, @amount,@return, 1
		END
		------------------------------------------------------------------------
		-- SCC (Cash Loan Service Charge Correction)
		------------------------------------------------------------------------
		ELSE IF(@transtype = 'SCC')
		BEGIN
			EXEC CLAmortizationUpdateAccountBalance @acctNo, @amount,@return, 1
		END
	END
	------------------------------------------------------------------------
	-- Logic to process for Debit transaction of diffrent transtype codes
	------------------------------------------------------------------------
	ELSE IF(@creditDebit = 1)
	BEGIN
		------------------------------------------------------------------------
		-- CLW (Cash Loan Write off)
		------------------------------------------------------------------------
		--IF(@transtype = 'CLW')
		--BEGIN
		--	--Not Applicable for debit operation
		--END
		------------------------------------------------------------------------
		-- CLA (Cash Loan Admin Fee adjustment)
		------------------------------------------------------------------------
		IF(@transtype = 'CLA')
		BEGIN
			EXEC [dbo].[CLAmortizationReversePayment]  @acctNo, @amount,@return,1
		END
		------------------------------------------------------------------------
		-- INC (Cash Loan insurance claim)
		------------------------------------------------------------------------
		ELSE IF(@transtype = 'INC')
		BEGIN
			EXEC [dbo].[CLAmortizationReversePayment]  @acctNo, @amount,@return,1
		END
		------------------------------------------------------------------------
		-- CLP (Cash Loan penalty interest Reversal)
		------------------------------------------------------------------------
		ELSE IF(@transtype = 'CLP')
		BEGIN
			SELECT @maxInstallment = InstallmentNo FROM [dbo].[CLAmortizationPaymentHistory]
			WHERE [AcctNo] = @acctNo

			UPDATE [dbo].[CLAmortizationPaymentHistory]
			SET Interest = Interest + @amount
			WHERE [AcctNo] = @acctNo AND InstallmentNo = @maxInstallment
		END
		------------------------------------------------------------------------
		-- CLL (Cash Loan Late Fee Reversal)
		------------------------------------------------------------------------
		ELSE IF(@transtype = 'CLL')
		BEGIN
			EXEC [dbo].[CLAmortizationReversePayment]  @acctNo, @amount,@return,1
		END
		------------------------------------------------------------------------
		-- SCC (Cash Loan Service Charge Correction)
		------------------------------------------------------------------------
		ELSE IF(@transtype = 'SCC')
		BEGIN
			EXEC [dbo].[CLAmortizationReversePayment]  @acctNo, @amount,@return,1
		END
	END
	IF(@@ERROR<>0)
	BEGIN
		SET @return = @@ERROR
	END
END
