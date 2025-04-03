IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[DN_SaveAmortizedCLScheduleSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DN_SaveAmortizedCLScheduleSP]
GO


CREATE PROC [dbo].[DN_SaveAmortizedCLScheduleSP]
-- ==================================================================================================================
-- Project      : CoSACS.NET
-- Produre Name : [dbo].[DN_SaveAmortizedCLScheduleSP]
-- Version        : 002
--
-- Change Control
-- --------------
-- Ver    Date        By                Description
-- ---    ----        --                -----------
-- 002  19/11/2020    Ritesh Joge       log #7426991 - Fixed showing incorrect order of sequencing
-- ==================================================================================================================
@acctno NVARCHAR(12)
,@instalment DECIMAL(15, 2)
,@term INT
,@scoringband VARCHAR(1)
,@amount DECIMAL(15, 2)
,@adminCharge DECIMAL(15, 2)
,@return INT OUTPUT
AS
	DECLARE @acctopen DATETIME
	DECLARE @termstype VARCHAR(4)
	DECLARE @servicechgpct FLOAT
	DECLARE @openingbal DECIMAL(15, 2)
	DECLARE @closingbal DECIMAL(15, 2)
	DECLARE @principalreduced DECIMAL(15, 2)
	DECLARE @servicechg DECIMAL(15, 2)
	DECLARE @counter INT
	DECLARE @instalduedate DATETIME
	DECLARE @dueday SMALLINT
	DECLARE @custid VARCHAR(20)
	DECLARE @adminfee DECIMAL(15, 2)
	DECLARE @adminchg DECIMAL(15, 2)
	DECLARE @admint DECIMAL(15, 2)
	
	SELECT @acctopen = dateacctopen
		,@termstype = termstype
	FROM acct
	WHERE acctno = @acctno --@openingbal=agrmttotal
		--seLect @openingbal=LoanAmount from cashloan where acctno=@acctno
	
	SET @openingbal = @amount
	
	SELECT TOP 1 @servicechgpct = intrate
	FROM intratehistory
	WHERE termstype = @termstype
		AND Band = @scoringband
	ORDER BY datechange DESC
	
	SELECT @custid = custid
	FROM custacct
	WHERE acctno = @acctno
	
	SELECT @admint = @adminCharge
	
	EXEC DN_AccountGetDueDay @custid
		,@dueday OUT
		,@return OUT
	
	SET @return = 0
	SET @counter = 1
	
	--set @acctopen = DateAdd(dd, @dueday-DatePart(dd, @acctopen), @acctopen)
	CREATE TABLE #temp (
		acctno CHAR(12)
		,instalduedate DATETIME
		,openingbal DECIMAL(15, 2)
		,instalment DECIMAL(15, 2)
		,principal DECIMAL(15, 2)
		,servicechg DECIMAL(15, 2)
		,closingbal DECIMAL(15, 2)
		,AdminFee DECIMAL(15, 2)
		)
	
	SET @servicechgpct = (@servicechgpct / 12) / 100
	SET @adminchg = @admint / @term
	
	WHILE (@openingbal > 0)
	BEGIN
		SET @instalduedate = convert(DATE, dateadd(month, @counter, @acctopen))
		SET @servicechg = @openingbal * @servicechgpct
	
		IF (
				@counter = @term
				AND @instalment > @openingbal
				)
		BEGIN
			SET @instalment = @openingbal + @servicechg
		END
	
		SET @principalreduced = @instalment - @servicechg
		SET @closingbal = @openingbal - @principalreduced
		SET @adminfee = @adminchg
	
		INSERT INTO #temp
		VALUES (
			@acctno
			,@instalduedate
			,@openingbal
			,@instalment
			,@principalreduced
			,@servicechg
			,@closingbal
			,@adminfee
			)
	
		SET @openingbal = @closingbal
		SET @counter = @counter + 1
	END
	
	IF EXISTS (
			SELECT *
			FROM CLAmortizationSchedule
			WHERE acctno = @acctno
			)
	BEGIN
		DELETE
		FROM CLAmortizationSchedule
		WHERE acctno = @acctno
	
		DELETE
		FROM CLAmortizationPaymentHistory
		WHERE acctno = @acctno
	END
	
	INSERT INTO CLAmortizationSchedule (
		acctno
		,instalduedate
		,openingbal
		,instalment
		,principal
		,servicechg
		,closingbal
		,AdminFee
		)
	SELECT *
	FROM #temp
	ORDER BY instalduedate ASC
	
	/*
		Author : Rahul D
		1. Changes to implement new rule for calculation of Outstanding Balance.
		2. Store the amortization schedule in table CLAmortizationPaymentHistory, 
		   only if CL_Amortized and CL_NewOutstandingCalculation is true in CountryMaintenance
	 */
	DECLARE @ValueCL_Amortized VARCHAR(MAX)
		,@ValueCL_NewOutstandingCalculation VARCHAR(MAX)
	
	SELECT @ValueCL_Amortized = VALUE
	FROM [dbo].[CountryMaintenance]
	WHERE CodeName = 'CL_Amortized'
	
	SELECT @ValueCL_NewOutstandingCalculation = VALUE
	FROM [dbo].[CountryMaintenance]
	WHERE CodeName = 'CL_NewOutstandingCalculation'
	
	IF (
			@ValueCL_Amortized = 'True'
			AND @ValueCL_NewOutstandingCalculation = 'True'
			)
	BEGIN
		INSERT INTO CLAmortizationPaymentHistory (
			acctno
			,instalduedate
			,openingbal
			,instalment
			,principal
			,servicechg
			,closingbal
			,adminfee
			,installmentNo
			)
		SELECT acctno
			,instalduedate
			,openingbal
			,instalment
			,principal
			,servicechg
			,closingbal
			,AdminFee
			,ROW_Number() OVER (
				ORDER BY instalduedate ASC
				) AS installmentNo
		FROM #temp
END


