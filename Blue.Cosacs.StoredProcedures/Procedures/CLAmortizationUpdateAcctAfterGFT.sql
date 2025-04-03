if exists (select * from dbo.sysobjects
           where id = object_id('[dbo].[CLAmortizationUpdateAcctAfterGFT]')
           and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[CLAmortizationUpdateAcctAfterGFT]
GO

CREATE PROCEDURE CLAmortizationUpdateAcctAfterGFT 
	-- Add the parameters for the stored procedure here
	@acctno varchar(12),
	@outstandingBalance MONEY OUTPUT,
	@bdCharges MONEY OUTPUT,
	@return int out
AS
BEGIN
		SET @return = 0
		SELECT @bdCharges = sum(adminfee)+sum(Servicechg)+sum(Interest) FROM CLAmortizationPaymentHistory
		WHERE acctno = @acctno

		EXEC @outstandingBalance = [fn_CLAmortizationCalcDailyOutstandingBalance] @acctno
		
END
GO


