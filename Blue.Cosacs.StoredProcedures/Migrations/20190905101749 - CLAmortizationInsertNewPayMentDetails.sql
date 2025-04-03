IF EXISTS (SELECT * FROM SYS.PROCEDURES WHERE NAME = 'CLAmortizationInsertNewPayMentDetails')
	DROP PROC CLAmortizationInsertNewPayMentDetails
GO
--------------------------------------------------------------
-- =============================================
-- Author:		Rahul D
-- Create date: 20-06-2019
-- Description:	This procedure will Insert transValue and trans Type in Payment details table for Cashloan amortization
-- =============================================
CREATE PROCEDURE [dbo].[CLAmortizationInsertNewPayMentDetails] 
	-- Add the parameters for the stored procedure here
	@acctno VARCHAR(12),
	@amount Money,
	@transTypeCode VARCHAR(10),
	@CreditDebitNo INT,
	@return INT OUT
AS
BEGIN
	IF(@amount <> 0)
	BEGIN
		INSERT INTO CLANewPaymentDetails (acctno,transvalue,transtypeCode,CreditDebitNo)
		VALUES (@acctno,@amount,@transTypeCode,@CreditDebitNo)
	END
END