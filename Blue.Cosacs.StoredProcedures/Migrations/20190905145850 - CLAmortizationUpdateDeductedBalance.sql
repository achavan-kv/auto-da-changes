IF EXISTS (
		SELECT *
		FROM sys.procedures
		WHERE name = 'CLAmortizationUpdateDeductedBalance'
		)
	DROP PROCEDURE CLAmortizationUpdateDeductedBalance
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET NOCOUNT OFF; 
Go
/*
	Script Name		:	CLAmortizationUpdateDeductedBalance
	Script Details	:	The procedure will updated the deducted component in repective column
	Author			:	Rahul D, Zensar
	Date			:	7/Jul/2019
*/
CREATE PROCEDURE CLAmortizationUpdateDeductedBalance @AcctNo VARCHAR(12)
	,@InstallNo INT
	,@Amount MONEY
	,@ComponentToUpdate VARCHAR(50)
AS
BEGIN
	IF (@ComponentToUpdate = 'Principal')
	BEGIN
		UPDATE [CLAmortizationPaymentHistory]
		SET PrevPrincipal = ISNULL(PrevPrincipal, 0) + @Amount
		WHERE [acctno] = @AcctNo
			AND [installmentNo] = @InstallNo
	END
	ELSE IF (@ComponentToUpdate = 'ServiceCharge')
	BEGIN
		UPDATE [CLAmortizationPaymentHistory]
		SET PrevServicechg = IsNull(PrevServicechg, 0) + @Amount
		WHERE [acctno] = @AcctNo
			AND [installmentNo] = @InstallNo
	END
	ELSE IF (@ComponentToUpdate = 'AdminFee')
	BEGIN
		UPDATE [CLAmortizationPaymentHistory]
		SET PrevAdminfee = IsNull(PrevAdminfee, 0) + @Amount
		WHERE [acctno] = @AcctNo
			AND [installmentNo] = @InstallNo
	END
	ELSE IF (@ComponentToUpdate = 'Interest')
	BEGIN
		UPDATE [CLAmortizationPaymentHistory]
		SET PrevInterest = IsNull(PrevInterest, 0) + @Amount
		WHERE [acctno] = @AcctNo
			AND [installmentNo] = @InstallNo
	END
END