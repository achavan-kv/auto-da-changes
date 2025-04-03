SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

IF EXISTS (select * from sysobjects where name ='CSK_CalculateCollectionFEESP')
drop procedure CSK_CalculateCollectionFEESP
go

create  procedure [dbo].[CSK_CalculateCollectionFEESP]

/***********************************************************************************************************
--
-- Project      : CoSACS .NET
-- File Name    : CSK_CalculateCollectionFEESP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Description  : Procedure to calculate and return the collection fee for an account
-- Author       : Ilyas Parker
-- Date         : 28 September 2010
--
--  
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 28/09/10	 IP  Creation.

************************************************************************************************************/
--Parameters

@acctno VARCHAR(12),
@arrears MONEY,
@paymentAmt MONEY,
@collectionFEE MONEY OUT,
@return int out

as

set @return = 0

DECLARE 
	@decimalPlaces SMALLINT,
	@calculateFee BIT,
	@feeOnPaymentAmt BIT,
	@empeenoAllocated INT,
	@colltype CHAR(1),
	@status char(1),
    @rowcount INT,
    @collectionPercent FLOAT,
    @debitAccount SMALLINT,
    @wholeCollectionFEE MONEY,
    @totalForFEE MONEY,
    @minValue FLOAT,
    @maxValue FLOAT,
    @outstbal MONEY
    

SET @rowcount = 0
SET @collectionFEE = 0

SET @paymentAmt = ABS(@paymentAmt)

SELECT @decimalPlaces = ISNULL(SUBSTRING(decimalplaces,2,1),0) FROM country --Country decimal places for rounding
SELECT @feeOnPaymentAmt = VALUE FROM dbo.CountryMaintenance WHERE CodeName = 'arrearsonpay' --Fees calculated on payment amount (true/false)

SELECT @outstbal = outstbal FROM acct WHERE acctno = @acctno

--Only calculate collection fee where the tallman segment says the account is allocated to a Bailiff/Collector.
IF EXISTS(SELECT * FROM dbo.TM_Segments
				WHERE account_number = @acctno
				AND segment_name IN ('Bailiff Allocated (Auto)', 'Bailiff Allocated (Manual)',
									'Collector Allocated (Auto)','Collector Allocated (Manual)', 'Auto Allocation (Coll)'))
BEGIN
		SET @calculateFee = 1
END
ELSE
BEGIN
		SET @calculateFee = 0
END

--Select employee currently allocated to the account.
SELECT @empeenoAllocated = ISNULL(empeeno,0) FROM follupalloc
								   WHERE	isnull(datealloc, '1/1/1900') != '1/1/1900'
								   AND		isnull(datealloc, '1/1/1900') <= getdate()
								   AND		(isnull(datedealloc, '1/1/1900') = '1/1/1900'
								   OR		isnull(datedealloc, '1/1/1900') >= getdate())
								   AND		acctno = @acctno

--Only calculate collection fee if account is allocated to a bailiff/collector and arrears > 0
IF(@calculateFee = 1 AND @empeenoAllocated!=0 AND @arrears > 0 AND @outstbal > 0)
BEGIN				

	--IF(@paymentAmt = 0)
		SET @colltype = 'W'

	DECLARE @CollectionRates TABLE 
	(
		CollectionPercent FLOAT,
		MinValue FLOAT,
		MaxValue FLOAT,
		DebitAccount SMALLINT,
		CollType CHAR(1)
	) 
	
	 -- The account status to be used can be at the time of allocation
    -- or the current status

    IF NOT EXISTS (SELECT Value FROM CountryMaintenance
                   WHERE  CodeName = 'CurStatusCommission'
                   AND    Value    = 'True')
    BEGIN
        -- Get the status at the time of allocation
        SELECT  TOP 1
                @status = statuscode
        FROM    status
        WHERE   acctno = @acctno
        AND     datestatchge <=
            (SELECT  max(datealloc)
             FROM    follupalloc
             WHERE   isnull(datealloc, '1/1/1900') != '1/1/1900'
             AND     isnull(datealloc, '1/1/1900') <= getdate()
             AND     (isnull(datedealloc, '1/1/1900') = '1/1/1900' OR isnull(datedealloc, '1/1/1900') >= getdate())
             AND     acctno = @acctno
             AND     empeeno = @empeenoAllocated)
        ORDER BY    datestatchge desc

        SET @rowcount = @@ROWCOUNT
    END


    if @rowcount = 0
        -- Get the current status
		select @status = currstatus from acct where acctno = @acctno

	--Select the collection rates to use to calculate collection fee on whole payment
	INSERT INTO @CollectionRates
	SELECT collectionpercent, minvalue, maxvalue,  isnull(debitaccount, 0),'W'
	FROM    bailcommnbas
    WHERE   empeeno = @empeenoAllocated
    AND     statuscode = @status
    AND     collecttype = @colltype

	SELECT @collectionPercent = CollectionPercent FROM @CollectionRates WHERE CollType = 'W'
	SELECT @debitAccount = DebitAccount FROM @CollectionRates WHERE CollType = 'W'
	

	IF(@debitAccount = 1)
	BEGIN
	
		SET @wholeCollectionFEE = (@arrears/100) * @collectionPercent
		SET @collectionFEE = ROUND(@wholeCollectionFEE,  @decimalPlaces)
		
	END
	
		
	--Then this is a part payment, therefore calculate collection fee
	IF(@paymentAmt > 0 AND @paymentAmt < @arrears + @wholeCollectionFEE)
	BEGIN
		SET @colltype = 'P'
		INSERT INTO @CollectionRates
		SELECT collectionpercent, minvalue, maxvalue,  isnull(debitaccount, 0),'P'
		FROM    bailcommnbas
		WHERE   empeeno = @empeenoAllocated
		AND     statuscode = @status
		AND     collecttype = @colltype
		
		SELECT @collectionPercent = CollectionPercent FROM @CollectionRates WHERE CollType = 'P'
		SELECT @debitAccount = DebitAccount FROM @CollectionRates WHERE CollType = 'P'
		
		IF(@collectionPercent  > 0 AND @debitAccount = 1)
		BEGIN
			IF(@feeOnPaymentAmt='True')
			BEGIN
				SET @arrears = @arrears/100 * (100 + @collectionPercent)
				SET @arrears = ROUND(@arrears, @decimalPlaces)
				SET @totalForFEE = CASE WHEN @paymentAmt > 0 AND @paymentAmt < @arrears THEN @paymentAmt ELSE @arrears END
				SET @collectionFEE = (@totalForFEE / (100 + @collectionPercent)) * @collectionPercent
			END
			ELSE
			BEGIN
				SET @totalForFEE = CASE WHEN @paymentAmt > 0 AND @paymentAmt < @arrears THEN @paymentAmt ELSE @arrears END
				SET @collectionFEE = (@totalForFEE / 100) * @collectionPercent
			END
		
			--Ensure the collection fee is within the min and max values of the selected rate.
			SELECT @minValue = MinValue FROM @CollectionRates WHERE @colltype = 'P'
			SELECT @maxvalue = MaxValue FROM @CollectionRates WHERE @colltype = 'P'
			
			SET @collectionFEE = CASE WHEN @collectionFEE < @minValue THEN @minValue 
								 WHEN @collectionFEE > @maxValue THEN @maxValue
								 ELSE @collectionFEE END
								 
			SET @collectionFEE = ROUND(@collectionFEE, @decimalPlaces)
		END
	END
	

END
ELSE
BEGIN
	SET @collectionFEE = ROUND(@collectionFEE, @decimalPlaces)
END

 
IF (@@error != 0)
    BEGIN
        SET @return = @@error
    END

GO

