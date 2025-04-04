IF EXISTS ( SELECT * FROM sysobjects WHERE NAME = 'TPConfirmTransactionLineItem' )
BEGIN
	DROP PROCEDURE [dbo].[TPConfirmTransactionLineItem]
END
GO

CREATE PROCEDURE [dbo].[TPConfirmTransactionLineItem]
	@CustId As varchar(20),
	@loanAmount AS decimal(18,2),
	@numberOfInstallments As integer,
 	@storeId AS varchar(20),
	@acctno AS varchar(20),
	@BranchId AS integer
	--@Message varchar(MAX) output,
	--@Status varchar(5) output

AS
BEGIN

		Declare @itemno as varchar(20)
		Declare @stocklocn as integer
		Declare @ordval as integer
		Declare @ItemID as integer=0

		--Get Item No.
		select top 1 @itemno=SKU from merchandising.product where  
		PrimaryVendorid=(select ID As storeid from merchandising.supplier where Code =@storeId)

		select @ItemID=id from Stockinfo where itemno=@itemno

		----Update account table
		update acct set hasstocklineitems =1 where acctno=@acctno
		--Print @ItemID
		--Print 'hello1'

		----Create new line item
		INSERT INTO dbo.lineitem 
		(origbr ,acctno ,agrmtno ,itemno ,itemsupptext ,quantity ,delqty ,stocklocn ,price ,ordval ,datereqdel ,
		timereqdel ,dateplandel ,delnotebranch ,qtydiff ,itemtype ,notes ,taxamt ,isKit ,deliveryaddress ,parentitemno ,
		parentlocation ,contractno ,expectedreturndate ,deliveryprocess ,deliveryarea ,DeliveryPrinted ,assemblyrequired ,
		damaged ,OrderNo ,Orderlineno ,PrintOrder ,taxrate ,ParentItemID ,SalesBrnNo ,Express ,WarrantyGroupId,ItemID)
		VALUES 
		(1 ,@acctno ,1 ,@itemno ,''  ,@loanAmount,0 ,@BranchId ,1 ,@loanAmount ,Getdate() ,'' ,Getdate() ,
		@BranchId ,'' ,'S' ,'' ,0 ,0 ,'H' ,'' ,0 ,'' ,NULL ,'I' ,'' ,'N' ,'N' ,'N' ,NULL ,NULL ,0 ,0 , 0 ,@BranchId ,'N' ,NULL,
		@ItemID)
			

		--Start Find the interest
	--		BEGIN
	--	DECLARE @ScoringBand varchar(4) = ''
	--	DECLARE @ServiceCharge DECIMAL(10,2) 
	--	DECLARE @INTEREST DECIMAL(10,2) 
	--	DECLARE @TENURELENGTH INT
	--	DECLARE @TERMSTYPE NVARCHAR(4)
		DECLARE	@monthly money
		DECLARE @final money
		DECLARE @FinalAmount money
	--	DECLARE @deposit integer=0

	--	SELECT TOP 1 @ScoringBand = ScoringBand FROM proposal WHERE custid = @CustId order by dateprop desc
	--	SELECT TOP 1 @ServiceCharge = CONVERT(DECIMAL(10,2), ServiceCharge) FROM TermsTypeBand WHERE BAND = @ScoringBand

	--	if exists (SELECT Top 1 termstype
	--	FROM    termstypetable where termstype='UN')
	--			 BEGIN

	--			SELECT Top 1 @TERMSTYPE = termstype
	--					FROM    termstypetable where termstype='UN'

	--			 END

	--		 ELSE 

	--			 Begin
			
	--				SELECT  TOP 1 @TERMSTYPE = termstype
	--				FROM    TermsTypeLength 
	--				GROUP BY termstype
	--				HAVING COUNT(termstype) > 2

	--			End
	
	
	--	--SELECT Top 1 @TERMSTYPE = termstype
	--	--FROM    TermsTypeLength where termstype='UN'

	--	--SELECT  TOP 1 @TERMSTYPE = termstype
	--	--FROM    TermsTypeLength 
	--	--GROUP BY termstype
	--	--HAVING COUNT(termstype) > 2
		
	--	--DECLARE @TermsTypeLengths table ( length INTEGER, termstype VARCHAR(500))

	--	SELECT length FROM TermsTypeLength WHERE termstype = @TERMSTYPE ORDER BY length ASC
		
	--	--Print 'X-' + CONVERT(NVARCHAR(20), @LoanAmount)
	--	--Print 'Y-' + CONVERT(NVARCHAR(5), @ServiceCharge)
	--	--Print 'Z-' + CONVERT(NVARCHAR(5), @numberOfInstallments)

	--	SET @INTEREST = CONVERT(DECIMAL(10,2), @LoanAmount) * CONVERT(DECIMAL(10,2), (@ServiceCharge/100)) *
	--			 (CONVERT(DECIMAL(10,2), @numberOfInstallments) /12)

	--END

	----End Find the interest
	--	--PRINT 'INTEREST:- ' + CONVERT(NVARCHAR(15), @INTEREST)
		
		
	--	--PRINT 'FinalAmount:- ' +  CONVERT(NVARCHAR(5), @FinalAmount)
	--	--Print 'A-' + CONVERT(NVARCHAR(20), @LoanAmount)
	--	--Print @INTEREST
	--	--Print 'C-' + CONVERT(NVARCHAR(5), @deposit)
	--	--Print 'D-' + CONVERT(NVARCHAR(5), @numberOfInstallments)
	--	set  @monthly = (@LoanAmount + @INTEREST - @deposit) / @numberOfInstallments;
	--	--PRINT 'monthly:- ' +  CONVERT(NVARCHAR(15), @monthly)
	--	set	 @final = (@LoanAmount + @INTEREST - @deposit) - (@monthly * (@numberOfInstallments - 1)); 
	--	--PRINT 'final:- ' +  CONVERT(NVARCHAR(15), @final)
	--	Set @FinalAmount = @INTEREST + @loanAmount

		----------------------------------------------
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

			--Print '@LoanAmount'+ CONVERT(NVARCHAR(20), @LoanAmount)
			--Print '@ServiceCharge'+ CONVERT(NVARCHAR(20), @ServiceCharge)
			--Print '@TENURELENGTH'+ CONVERT(NVARCHAR(20), @TENURELENGTH)
			--Print '@ServiceCharge'+ CONVERT(NVARCHAR(20), @ServiceCharge)
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
		--SELECT numberOfInstallments, interest FROM @PaymentOptionTable  
		SELECT @numberOfInstallments=numberOfInstallments, @INTEREST=interest FROM @PaymentOptionTable where numberOfInstallments=@numberOfInstallments 
		Print '@numberOfInstallments '+ CONVERT(NVARCHAR(20), @numberOfInstallments)
		set  @monthly = (@LoanAmount + @INTEREST) / @numberOfInstallments;
		Print '@monthly '+ CONVERT(NVARCHAR(20), @monthly)
		set	 @final = (@LoanAmount + @INTEREST) - (@monthly * (@numberOfInstallments - 1))
		Print '@final'+ CONVERT(NVARCHAR(20), @final)
		Set @FinalAmount = @INTEREST + @loanAmount
		Print '@FinalAmount'+ CONVERT(NVARCHAR(20), @FinalAmount)
		----------------------------------------------

		--update instaal plan
		update instalplan set instalno=@numberOfInstallments, instalamount=@monthly,fininstalamt=@final,instaltot=@FinalAmount where acctno=@acctno

		--account auto aprrove
		update proposal set propresult='A',ProofId='E',ProofAddress='E',ProofIncome='E' where acctno=@acctno and custid=@CustId

		Update Proposalflag set datecleared=getutcdate() where acctno=@acctno and custid=@CustId and checktype='DC'

		Declare @AvSp As Money
		select @AvSp=AvailableSpend from customer where  custid=@CustId
			--SET @AvSp = @AvSp - (@INTEREST + @loanAmount )
			SET @AvSp = @AvSp - @loanAmount
		update customer set AvailableSpend = @AvSp where  custid=@CustId

		--update agreement set servicechg=@INTEREST where acctno=@acctno

		update acct set agrmttotal=@loanAmount where  acctno=@acctno

END
