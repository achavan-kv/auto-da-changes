

IF EXISTS (
		SELECT *
		FROM sysobjects
		WHERE type = 'P'
			AND NAME = 'DN_GetEarlySettlementFigure'
		)
	DROP PROCEDURE dn_getearlysettlementfigure
GO
-- ========================================================================================================
-- Author:		Kedar Mulay
-- Create date: 18-06-2019
-- Description:	This procedure will Return Early settlement Amount as per rules given in Amortization.
-----------------------------------------------------------------------------------------------------------
CREATE PROC [dbo].[DN_GetEarlySettlementFigure] @acctno NVARCHAR(12)
	,@settlementFig MONEY OUTPUT
	,@return INT OUTPUT
AS

SET NOCOUNT ON

DECLARE @datenow DATETIME
DECLARE @nextinstaldate DATETIME
DECLARE @totalfees MONEY
DECLARE @custid VARCHAR(20)
DECLARE @termstype VARCHAR(2)
DECLARE @scoreband VARCHAR(4)
DECLARE @servicechgpct DECIMAL(15, 5)
DECLARE @openingbal MONEY
DECLARE @servicechg MONEY
DECLARE @datedel DATETIME
DECLARE @datefirstinstal DATETIME
DECLARE @admincharg MONEY
DECLARE @AmountPaid MONEY
DECLARE @amtl MONEY
DECLARE @Intdiff MONEY
DECLARE @payDate DATETIME
DECLARE @installDuedate DATETIME
DECLARE @noDaysEarly INT
DECLARE @noDaysInMonth INT
DECLARE @monthlyInt MONEY = 0
DECLARE @IntRelax MONEY = 0
DECLARE @DateDisbur DATETIME
DECLARE @Interst MONEY
DECLARE @instaldaydue DATE
DECLARE @daynow DATE
DECLARE @daily MONEY
DECLARE @daydiff int
DECLARE @perdaychg money


IF EXISTS (
		SELECT 'A'
		FROM acct
		WHERE acctno = @acctno
			AND isamortized = 1
			AND isamortizedoutstandingbal = 1
		) -- if no 1 
BEGIN
	SET @datenow = cast(Getdate() AS DATE)
	SET @return = 0

	SELECT @totalfees = Isnull(Sum(transvalue), 0)
	FROM fintrans
	WHERE transtypecode IN (
			'INT'
			,'FEE'
			)
		AND acctno = @acctno

	--Below code added after discussion as the penalty interest must be there in settlement 	
	SET @daily = dbo.fn_CLAmortizationDailyInterest(@acctno)

	IF (@daily > 0)
	BEGIN
		SET @daily = @daily
	END
	ELSE
	BEGIN
		SET @daily = 0
	END

	SET @totalfees = isnull(@totalfees, 0) + isnull(@daily, 0)

	SELECT @AmountPaid = Isnull(Sum(transvalue), 0)
	FROM fintrans
	WHERE transtypecode IN (
			'PAY'
			,'COR'
			,'XFR'
			,'JLX'
			,'SCX'
			,'REF'
			,'RET'
			,'DDE'
			,'DDN'
			,'DDR'
			,'OVE'
			,'DPY'
			,'ADX'
			,'SCT'
			,'STR'
			)
		AND acctno = @acctno

	SELECT @custid = custid
		,@termstype = termstype
		,@DateDisbur = DatePrinted
	FROM cashloan
	WHERE acctno = @acctno

	SELECT @scoreband = scoringband
	FROM customer
	WHERE custid = @custid

	SELECT @servicechgpct = intrate
	FROM intratehistory
	INNER JOIN acct ON acct.termstype = intratehistory.termstype
	WHERE acct.acctno = @acctno
		AND intratehistory.termstype = @termstype
		AND intratehistory.band = @scoreband

	SET @servicechgpct = @servicechgpct / 100

	SELECT @amtl = loanamount
	FROM cashloan
	WHERE acctno = @acctno

	IF EXISTS (
			SELECT acctno
			FROM acct
			WHERE acctno = @acctno
				AND isamortized = 1
			)
		IF EXISTS (
				SELECT *
				FROM cashloan
				WHERE acctno = @acctno
					AND loanstatus = 'D'
				)
			SELECT @openingbal = Sum(principal)
			FROM clamortizationschedule
			WHERE acctno = @acctno

	SET @openingbal = @amtl

	----------------------------------------Calculation For Service Charge----------------------------------------------------------------------- 
	DECLARE --@installno int, 
		@tp MONEY
		,@ta MONEY
		,@tint MONEY = 0
		,@nodaysinpaymonth INT
		,@asbal MONEY
		,@montlyaccumulate MONEY

	SET @payDate = cast(Getdate() AS DATE)

	--Step to find instalment no 
	SELECT @asbal = (
			SELECT Isnull(openingbal, 0.0)
			FROM clamortizationpaymenthistory
			WHERE acctno = @acctno
				AND installmentno = 1
			) - (
			SELECT Isnull(Sum(prevprincipal), 0.00)
			FROM clamortizationpaymenthistory
			WHERE acctno = @acctno
			)
			+
			(@totalfees)

	SELECT TOP 1 @installDuedate = cast(instalduedate AS DATE)
		,@monthlyInt = servicechg
	FROM clamortizationschedule
	WHERE acctno = @acctno
		AND Cast(instalduedate AS DATE) > Cast(@payDate AS DATE)
	ORDER BY instalduedate ASC

	IF (Cast(@installDuedate AS DATE) > Cast(@payDate AS DATE))
		-- Cust is paying in adv, we need to give int benefit 
	BEGIN
			SELECT @noDaysInMonth = Day(Eomonth(@installDuedate))

			SELECT @nodaysinpaymonth = Day(Eomonth(@payDate))

			SELECT @noDaysEarly = datediff(day, getdate(), @installDuedate)

			SELECT @tint = Isnull(Sum(servicechg), 0)
			FROM clamortizationpaymenthistory
			WHERE acctno = @acctno --and isPaid =0   
				AND Cast(instalduedate AS DATE) < Cast(@installDuedate AS DATE)
	END

	DECLARE @YEAR INT
		,@Totaldaysinyear INT
		,@a INT
		,@OTBal MONEY
		,@dailyinterest MONEY
		,@monthlyinterest MONEY

	SET @a = Datepart(year, Getdate())
	SET @YEAR = @a

	SELECT @Totaldaysinyear = Datediff(d, Cast(CONCAT (
					'01/01/'
					,@YEAR
					) AS DATETIME), Cast(CONCAT (
					'12/31/'
					,@YEAR
					) AS DATETIME) + 1)

	SET @dailyinterest = ((@asbal * @servicechgpct) * @noDaysEarly) / @Totaldaysinyear

	SET @monthlyinterest = ((@asbal * @servicechgpct) * @nodaysinpaymonth) / @Totaldaysinyear


	SELECT @instaldaydue = CAST(instalduedate AS DATE)
	FROM [dbo].[CLAmortizationSchedule]
	WHERE acctno = @acctno
		AND (DATEPART(year, instalduedate)) = DATEPART(year, getdate())
		AND DATEPART(month, instalduedate) = DATEPART(month, getdate())

	SELECT @daynow = cast(getdate() AS DATE)

	IF (cast(Getdate() AS DATE) = cast(@DateDisbur AS DATE))
	BEGIN
		SET @IntRelax = @monthlyinterest * - 1
	END
	ELSE IF (@instaldaydue = @daynow)
	BEGIN
		SET @IntRelax = @monthlyinterest * - 1
	END
	ELSE
	BEGIN
	
		SET @IntRelax = ((@monthlyinterest / @nodaysinpaymonth) * @noDaysEarly) * - 1

	END

	SET @tint = (Isnull(@tint, 0) + (Isnull(@monthlyinterest, 0) + Isnull(@IntRelax, 0)))
	SET @servicechg = @tint
	

	-------------------------------------------------------------------------------------------------------------------------------------------------- 
	IF exists(Select  'A' From clamortizationpaymenthistory where Servicechg > 0 and instalduedate<=getdate() and  acctno =@acctno)
	BEGIN
	SELECT @servicechg = (@tint) --, @tint 'total interest in sch' 
	END
	Else
	BEGIN
	SET @servicechg=0
	END
	-------------------------------------------------------------------------------------------------------------------------------------------------   
	--------------------------------------------------------Calculation For AdminCharge----------------------------------------------------------------------------------------------------- 
	IF EXISTS (
			SELECT 'A'
			FROM acct
			WHERE acctno = @acctno
				AND isamortizedoutstandingbal = 1
				AND isamortized = 1
			)
	BEGIN
		DECLARE @datefirst DATE

		SELECT @datefirst = cast(DATEFIRST AS DATE)
		FROM instalplan
		WHERE acctno = @acctno

		SELECT @datedel = cast(datedel AS DATE)
		FROM agreement
		WHERE acctno = @acctno

		IF (cast(Getdate() AS DATE) <= cast(@datefirst AS DATE))
			-- admin fee always be added till date of first installment  
		BEGIN
			SELECT @admincharg = adminfee
			FROM clamortizationpaymenthistory
			WHERE acctno = @acctno
				AND installmentno = 1
				-- AND ispaid = 0 
				-- PRINT @admincharg 
		END
		ELSE
		BEGIN --This will add admin fee on each due date  
			SELECT @admincharg = Isnull(Sum(adminfee), 0)
			FROM clamortizationpaymenthistory
			WHERE acctno = @acctno
				AND cast(instalduedate AS DATE) <= cast(Getdate() AS DATE)
		END
	END
	ELSE -- This will add admin fee on each due date as well 
	BEGIN
		SELECT @admincharg = Isnull(Sum(adminfee), 0)
		FROM clamortizationschedule
		WHERE acctno = @acctno
			AND cast(instalduedate AS DATE) <= cast(Getdate() AS DATE)
			--order by instalduedate 
	END


	IF ((Select Value from CountryMaintenance where CodeName='CL_ApplyFullAdmin')='true')
			BEGIN
				 SELECT @admincharg = Isnull(Sum(adminfee), 0) 
					FROM   clamortizationpaymenthistory--clamortizationschedule 
					WHERE  acctno = @acctno 
			END


	--IF (@totalfees > 0)
	--BEGIN
	--	SET @totalfees = @totalfees
	--END
	--ELSE
	--BEGIN
	--	SET @totalfees = 0
	--END

	------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ 
	SET @Interst = @monthlyinterest + @IntRelax

	DECLARE @adminchargess MONEY

	IF ((Select Value from CountryMaintenance where CodeName='CL_ApplyFullAdmin')='true')
	BEGIN
 		SELECT @adminchargess = Sum(adminfee)
			FROM clamortizationpaymenthistory 
			WHERE acctno = @acctno						
	END 
	ELSE IF (cast(Getdate() AS DATE) <= cast(@datefirst AS DATE))
	BEGIN
		SELECT @adminchargess = adminfee
		FROM clamortizationpaymenthistory
		WHERE acctno = @acctno
			AND installmentno = 1
			-- AND ispaid = 0 
	END
	ELSE
	BEGIN
		SELECT @adminchargess = Isnull((adminfee), 0)
		FROM clamortizationpaymenthistory
		WHERE acctno = @acctno
			AND cast(instalduedate AS DATE) = cast(Getdate() AS DATE)  AND ispaid = 0  order by instalduedate desc
	END
	IF exists(Select  'A' From clamortizationpaymenthistory where Servicechg > 0 and instalduedate<=getdate() and  acctno =@acctno)
	BEGIN
	SET @perdaychg=@monthlyinterest-((@monthlyinterest / @nodaysinpaymonth) * @noDaysEarly)
	 END
	 ELSE 
	 BEGIN
	 SET @perdaychg=0
	 END
	
	---------------------------------------------------------------------------------------------------------------------------------------------- 
	SET @settlementFig = Isnull(@asbal, 0.0) + Isnull(@servicechg, 0.00) + Isnull(@totalfees, 0.00) + Isnull(@admincharg, 0) --+@AmountPaid-- +@Intdiff 

	SELECT @perdaychg AS 'PerdaySc'
		,@adminchargess AS 'Admin Charge'
		,@servicechg AS 'Monthly SC'	
		-----------------------------------------Use below print statement for debug------------------------------------------------------------------ 
END -- End of 1st if  
ELSE IF EXISTS (
		SELECT 'A'
		FROM acct
		WHERE acctno = @acctno
			AND isamortized = 1
			AND isamortizedoutstandingbal = 0
		) --if No 2 
BEGIN
	SET @datenow = cast(Getdate() AS DATE);
	SET @return = 0

	SELECT @totalfees = Isnull(Sum(transvalue), 0)
	FROM fintrans
	WHERE transtypecode IN (
			'INT'
			,'FEE'
			)
		AND acctno = @acctno

	SELECT @AmountPaid = Isnull(Sum(transvalue), 0)
	FROM fintrans
	WHERE transtypecode IN (
			'PAY'
			,'COR'
			,'XFR'
			,'JLX'
			,'SCX'
			,'REF'
			,'RET'
			,'DDE'
			,'DDN'
			,'DDR'
			,'OVE'
			,'DPY'
			,'ADX'
			,'SCT'
			,'STR'
			)
		AND acctno = @acctno

	SELECT @custid = custid
		,@termstype = termstype
		,@admincharg = admincharge
	FROM cashloan
	WHERE acctno = @acctno

	SELECT @scoreband = scoringband
	FROM customer
	WHERE custid = @custid

	SELECT TOP 1 @servicechgpct = intrate
	FROM intratehistory
	WHERE termstype = @termstype
		AND band = @scoreband
	ORDER BY datechange DESC

	SET @servicechgpct = @servicechgpct / 100

	SELECT @amtl = loanamount
	FROM cashloan
	WHERE acctno = @acctno

	IF EXISTS (
			SELECT *
			FROM cashloan
			WHERE acctno = @acctno
				AND loanstatus = 'D'
			) --if No 2.1 
	BEGIN
		SELECT @datedel = cast(datedel AS DATE)
		FROM delivery
		WHERE acctno = @acctno
			AND itemno = 'LOAN'

		SELECT @datefirstinstal = [datefirst]
		FROM instalplan
		WHERE acctno = @acctno

		IF (
				@datenow > @datedel
				AND @datenow < CONVERT(DATE, @datefirstinstal)
				) --if No 2.2 
		BEGIN
			SET @daydiff = Datediff(day, @datedel, @datenow)

			SELECT TOP 1 @openingbal = openingbal
			FROM clamortizationschedule
			WHERE acctno = @acctno

			SET @servicechg = (@servicechgpct / 365) * @daydiff * @openingbal
			SET @admincharg = (@admincharg / 365) * @daydiff
		END --end of if 2.2 
		ELSE -- else of if 2.2 
		BEGIN
			SET @daydiff = Datediff(day, @datedel, @datenow)

			IF (@daydiff > 0) -- if 2.2.1 
			BEGIN
				--select @openingbal=openingbal from CLAmortizationSchedule where acctno=@acctno and instalduedate<=@nextinstaldate
				SELECT @openingbal = @amtl --/365*@daydiff 

				SET @servicechg = (@servicechgpct / 365) * (@daydiff) * @openingbal
				SET @admincharg = (@admincharg / 365) * @daydiff
			END --end of if  2.2.1 
		END -- end of else 2.2 
	END -- END OF IF 2.1 

	SET @settlementFig = @openingbal + @servicechg + @totalfees + @admincharg + @AmountPaid
END --END of if 2
ELSE
BEGIN
	SET @settlementFig = 0
END

IF EXISTS (
		SELECT currstatus
		FROM acct
		WHERE acctno = @acctno
			AND currstatus = 'S'
		)
BEGIN
	SET @settlementFig = 0
END

IF (@@error != 0)
BEGIN
	SET @return = @@error
END
GO