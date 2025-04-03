GO
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].CLAmortizationReversePayment') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].CLAmortizationReversePayment
GO
SET QUOTED_IDENTIFIER ON
GO
-- ===============================================================================================
-- Author:		Ashish Padwal
-- Create date: 
-- Description:	This procedure will reverse payment by given amount.
-- ================================================================================================
CREATE PROCEDURE [dbo].[CLAmortizationReversePayment] 
	-- Add the parameters for the stored procedure here
	@acctno VARCHAR(12),
	@amount Money = -1,
	@return INT OUT,
	@IsGFT BIT = 0
AS
BEGIN
	SET @return = 0
	--BEGIN TRAN
	-----------------Variable Declaration---------------------------------------
	Declare @Maxinstallment int = 0 ,@totalDeductionSequence int,@SequenceCount int = 1,@SequenceName varchar(20),
	 @prevPrincipal money,@prevServicechg money,@prevAdminfee money,@prevInterest money
	Declare @outstandingBalance money = 0,@arrears money=0
	exec [CLAmortizationCalcDailyOutstandingBalance] @acctno,@outstandingBalance =@outstandingBalance OUTPUT
	select @arrears = arrears from acct where acctno = @acctno
	IF NOT (@outstandingBalance = 0 and @arrears < 0)
	BEGIN
	--SELECT * from CLAmortizationPaymentHistory where acctno = @acctno
		--------------Fetch max installment number which we need to reverse.-------------------------------
		SELECT @Maxinstallment= max(installmentNo) from CLAmortizationPaymentHistory where acctno = @acctno and ispaid = 1
		SELECT @totalDeductionSequence= count(id),@SequenceCount = max(id) from CL_PaymentDeductionSeq 
		--------------Loop through table in reverse order and deduct amount  from CLAmortizationPaymentHistory table.------------------------
		WHILE (@Maxinstallment > 0)
		BEGIN
		SELECT @prevPrincipal=prevPrincipal,@prevServicechg=prevServicechg,@prevAdminfee=prevAdminfee,@prevInterest=prevInterest
		FROM CLAmortizationPaymentHistory WHERE  acctno = @acctno and installmentNo = @Maxinstallment

			SELECT @SequenceCount = max(id) from CL_PaymentDeductionSeq 
			WHILE(@SequenceCount >= 1)
			BEGIN
			------Fetch sequence name  as per sequence specified in table.
				SELECT @SequenceName = SequenceName FROM CL_PaymentDeductionSeq WHERE id = @SequenceCount
				IF (@amount > 0)
				BEGIN
					 IF @SequenceName = 'Amortized Interest' 
					 BEGIN
							 IF((@amount - @prevServicechg)>= 0)
							 BEGIN
								--print 'Deduct Amortized Interest:'+ Convert(varchar(10), @amount) + ' For instalment:' + Convert(varchar(10),@Maxinstallment)
								SET @amount = @amount-@prevServicechg
								UPDATE CLAmortizationPaymentHistory
								SET servicechg = servicechg+@prevServicechg,prevServicechg = 0
								WHERE acctno = @acctno and installmentNo = @Maxinstallment
							 END --END of ((@amount - @servicechg)>= 0)
							 ELSE
							 BEGIN
								UPDATE CLAmortizationPaymentHistory
								SET servicechg = servicechg+@amount,prevServicechg = @prevServicechg-@amount
								WHERE acctno = @acctno and installmentNo = @Maxinstallment
								--SET amount is 0
								SET @amount =0
							 END
					 END--END of @SequenceName = 'Amortized Interest' 
					 IF @SequenceName = 'Admin Fees' 
					 BEGIN
							 IF((@amount - @prevAdminfee)>= 0)
							 BEGIN
							 --print 'Deduct Admin Fees:'+ Convert(varchar(10), @amount) + 'For instalment:' + Convert(varchar(10),@Maxinstallment)
								SET @amount = @amount-@prevAdminfee
								UPDATE CLAmortizationPaymentHistory
								SET adminfee = adminfee+@prevadminfee,prevadminfee = 0
								WHERE acctno = @acctno and installmentNo = @Maxinstallment
							 END
							 ELSE
							 BEGIN 
								UPDATE CLAmortizationPaymentHistory
								SET adminfee = adminfee+@amount,prevadminfee = @prevadminfee-@amount
								WHERE acctno = @acctno and installmentNo = @Maxinstallment
								SET @amount =0
							 END
					 END
					 IF @SequenceName = 'Principal' 
					 BEGIN
						 IF((@amount - @prevPrincipal)>= 0)
							 BEGIN
							 --print 'Deduct Principal:'+ Convert(varchar(10), @amount) + ' For instalment:' + Convert(varchar(10),@Maxinstallment)
								SET @amount = @amount-@prevPrincipal
								UPDATE CLAmortizationPaymentHistory
								SET Principal = Principal+@prevPrincipal,prevPrincipal = 0
								WHERE acctno = @acctno and installmentNo = @Maxinstallment
							 END
						ELSE
							BEGIN
							  --print'Inside this'
								UPDATE CLAmortizationPaymentHistory
								SET Principal = Principal+@amount,prevPrincipal = @prevPrincipal-@amount
								WHERE acctno = @acctno and installmentNo = @Maxinstallment
								SET @amount =0
							END
					 END
					 IF @SequenceName = 'Penalty Int' 
					 BEGIN
						 IF((@amount - @prevInterest)>= 0)
							 BEGIN
								 --print 'Penalty Int:'+ Convert(varchar(10), @amount) + ' For instalment:' + Convert(varchar(10),@Maxinstallment)
								SET @amount = @amount-@prevInterest
								UPDATE CLAmortizationPaymentHistory
								SET Interest = Interest+@prevInterest,prevInterest = 0
								WHERE acctno = @acctno and installmentNo = @Maxinstallment
							 END
						ELSE
							BEGIN
								UPDATE CLAmortizationPaymentHistory
								SET Interest = Interest+@amount,prevInterest = @prevInterest-@amount
								WHERE acctno = @acctno and installmentNo = @Maxinstallment
								SET @amount =0
							END
					 END
				END---END of IF(@amount > 0) 
			------Deduct as per reverse sequence------------------
				--print 'Remaining Amount:'+  Convert(varchar(10), @amount)
			SET @SequenceCount = @SequenceCount -1
			--- AFter all values deducted SET ispaid to 0.
			 IF (@SequenceCount = 0 and @amount > 0)
			 BEGIN
				UPDATE CLAmortizationPaymentHistory
				SET isPaid = 0
				WHERE acctno = @acctno and installmentNo = @Maxinstallment
			 END
			END---END of (@SequenceCount >= 1)
			------Deduct amount in reverse order------------------
			SET @Maxinstallment = @Maxinstallment -1
		END--End of While (@Maxinstallment > 0)
	
			---------Update account detail after payment-------------------------------
		IF(@amount<> -1 and @IsGFT=0)
		BEGIN
		
			exec [CLAmortizationCalcDailyOutstandingBalance] @acctno,@outstandingBalance =@outstandingBalance OUTPUT
			UPDATE acct
			SET outstbal = @outstandingBalance
			WHERE acctno = @acctno
		END
	END
END--End of main BEGIN
