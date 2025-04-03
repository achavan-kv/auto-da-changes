if exists (select * from dbo.sysobjects
           where id = object_id('[dbo].[CLAmortizationWriteOffAccountBalance]')
           and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[CLAmortizationWriteOffAccountBalance]
GO

CREATE PROCEDURE [dbo].[CLAmortizationWriteOffAccountBalance]  
	-- Add the parameters for the stored procedure here
		@acctno VARCHAR(12),
		@return INT OUT
AS
BEGIN
		--------------------------------------------Variable Declaration---------------------------------------------------------
		DECLARE @iCount int = 1 , @totalRecords int,
		@BCS money = 0,
		@BCA money = 0,
		@CLW money = 0,
		@BrokerAccountNo  int = 0

		--------Get Total number of Records needs to be process.
		SELECT @totalRecords = MAX(installmentNo) FROM CLAmortizationPaymentHistory WHERE acctno = @acctno

		
		WHILE(@icount <= @totalRecords)
		BEGIN
			SELECT @BCS = @BCS + ISNULL(Servicechg,0),
			@CLW = @CLW + ISNULL(principal,0)+ ISNULL(Servicechg,0) + ISNULL(Adminfee,0) + ISNULL(Interest,0),
			@BCA = @BCA + ISNULL(Adminfee,0)
			FROM  CLAmortizationPaymentHistory
			WHERE acctno = @acctno and installmentNo = @icount 

			UPDATE CLAmortizationPaymentHistory
			SET prevPrincipal = principal,prevServicechg = Servicechg,prevAdminfee = Adminfee,prevInterest =Interest,
			principal =0,Servicechg =0,Adminfee =0,Interest =0
			WHERE acctno = @acctno and installmentNo = @icount 
			--Increase counter by 1.
			SET @icount = @icount + 1
		END
		----------------------------------Insert these values in broker file----------------------------------------------
		----BCS
		SET @BrokerAccountNo = dbo.fn_CLAGetTranstypeAcctNo('BCS',1)
		EXEC CLAmortizationInsertNewPayMentDetails @acctno, @BCS, 'BCS',@BrokerAccountNo,0
		--BCA
		SET @BrokerAccountNo = dbo.fn_CLAGetTranstypeAcctNo('BCA',1)
		EXEC CLAmortizationInsertNewPayMentDetails @acctno, @BCA, 'BCA',@BrokerAccountNo,0
		--CLW
		--SET @BrokerAccountNo = dbo.fn_CLAGetTranstypeAcctNo('CLW',1)
		--EXEC CLAmortizationInsertNewPayMentDetails @acctno, @CLW, 'CLW',@BrokerAccountNo,0
		
		
END

