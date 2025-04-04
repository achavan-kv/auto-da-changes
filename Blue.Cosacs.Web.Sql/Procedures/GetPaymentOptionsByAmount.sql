IF EXISTS ( SELECT * FROM sysobjects WHERE NAME = 'GetPaymentOptionsByAmount' )
BEGIN
	DROP PROCEDURE [dbo].[GetPaymentOptionsByAmount]
END
GO

CREATE PROCEDURE [dbo].[GetPaymentOptionsByAmount] 
	  @CustId VARCHAR(20)
	, @LoanAmount DECIMAL(10,2)
	, @Message varchar(MAX) output
	, @Status varchar(5) output
AS
BEGIN
	--
	SET @Message = ''
	SET @Status = ''
	IF NOT EXISTS (select 1 from Customer where Custid=@CustId)
	BEGIN
		SET @Message = 'User not found'
		SET @Status = '404'
		RETURN		
	END
	ELSE IF NOT EXISTS (select 1 from custacct where custid=@CustId)
	BEGIN
		SET @Message = 'No accounts for user'
		SET @Status = '404'
		RETURN		
	END

	ELSE
	BEGIN
		DECLARE @ScoringBand varchar(4) = ''
		DECLARE @ServiceCharge DECIMAL(10,2) 
		DECLARE @INTEREST DECIMAL(10,2) 
		DECLARE @TENURELENGTH INT
		DECLARE @TERMSTYPE NVARCHAR(4)

		DECLARE @Branch varchar(5) 
		SELECT @Branch = origbr FROM country

		if exists (SELECT Top 1 termstype
		FROM    termstypetable where termstype='UN')
				 BEGIN

				SELECT Top 1 @TERMSTYPE = termstype
						FROM    termstypetable where termstype='UN'

				 END

			 ELSE 

				 Begin
						SELECT 	TOP 1
						 @TERMSTYPE = t.termstype
					FROM	termstype t 
						inner join termstypetable tt on t.termstype=tt.termstype,branch b 
					WHERE 	t.isactive = 1
						and b.branchno=@Branch and (b.storetype=tt.storetype or tt.storetype='A')
						AND		(t.delnonstocks = 0 ) 
					ORDER BY 	t.servpcent DESC, t.termstype 

				End
	

		--SELECT 	TOP 1
		--	 @TERMSTYPE = t.termstype
		--FROM	termstype t 
		--	inner join termstypetable tt on t.termstype=tt.termstype,branch b 
		--WHERE 	t.isactive = 1
		--	and b.branchno=@Branch and (b.storetype=tt.storetype or tt.storetype='A')
		--	AND		(t.delnonstocks = 0 ) 
		--ORDER BY 	t.servpcent DESC, t.termstype 

		--SELECT TOP 1 @ScoringBand = ScoringBand FROM proposal WHERE custid = @CustId order by dateprop desc 
		--SELECT TOP 1 @ServiceCharge = CONVERT(DECIMAL(10,2), ServiceCharge) FROM TermsTypeBand WHERE BAND = @ScoringBand
		select @ScoringBand = ScoringBand from customer where custid = @CustId 

			   SELECT top 1 @ServiceCharge=intrate
				FROM    intratehistory
				WHERE   termstype = @TERMSTYPE
				AND     band = @ScoringBand order by datefrom desc

		PRINT @ScoringBand
		PRINT @ServiceCharge

		DECLARE @PaymentOptionTable TABLE
		(
			numberOfInstallments nvarchar(10),
			interest nvarchar(200),
			interestRateAnnual nvarchar(10),
			effectiveTaxes nvarchar(10)
		)

		--SELECT  TOP 1 @TERMSTYPE = termstype	
		--FROM    TermsTypeLength
		--GROUP BY termstype
		--HAVING COUNT(termstype) > 2
		
		
		

		DECLARE @TermsTypeLengths table ( length INTEGER, termstype VARCHAR(500))

		DECLARE LengthCur CURSOR FOR
		SELECT length FROM TermsTypeLength WHERE termstype = @TERMSTYPE ORDER BY length ASC

		OPEN LengthCur 
		FETCH NEXT FROM LengthCur INTO @TENURELENGTH

		WHILE ( @@FETCH_STATUS = 0 )
			BEGIN
				/* 
					A = P(1 + rt)

					A = Total Accrued Amount (principal + interest)
					P = Principal Amount
					I = Interest Amount
					r = Rate of Interest per year in decimal; r = R/100
					R = Rate of Interest per year as a percent; R = r * 100
					t = Time Period involved in months or years

				*/

				--SET @INTEREST = CONVERT(DECIMAL(10,2), @LoanAmount) * 
				--				(1 + (CONVERT(DECIMAL(10,2), (@ServiceCharge/100)) * (CONVERT(DECIMAL(10,2), @TENURELENGTH) /12)))

			SET @INTEREST = CONVERT(DECIMAL(10,2), @LoanAmount) * CONVERT(DECIMAL(10,2), (@ServiceCharge/100)) * (CONVERT(DECIMAL(10,2), @TENURELENGTH) /12) 

				INSERT INTO @PaymentOptionTable(numberOfInstallments, interest, interestRateAnnual, effectiveTaxes) 
				SELECT
					CONVERT(VARCHAR(100), @TENURELENGTH) as numberOfInstallments, 
					CONVERT(VARCHAR(200), @INTEREST) as interest,  
					CONVERT(VARCHAR(100), @ServiceCharge) as interestRateAnnual,
					'0.0' as effectiveTaxes

				FETCH NEXT FROM LengthCur INTO @TENURELENGTH
			END

		CLOSE LengthCur 
		DEALLOCATE LengthCur 

		DECLARE @RowCount INT
		SELECT @RowCount = COUNT(1) FROM @PaymentOptionTable

		IF(@RowCount > 0)
		BEGIN
			SELECT numberOfInstallments, interest, interestRateAnnual, effectiveTaxes FROM @PaymentOptionTable

			SET @Message = 'Payment Option for given customer found'
			SET @Status = '200'
		END
		ELSE
		BEGIN			
			SET @Message = 'No details found for user'
			SET @Status = '404'
		END
	END
END