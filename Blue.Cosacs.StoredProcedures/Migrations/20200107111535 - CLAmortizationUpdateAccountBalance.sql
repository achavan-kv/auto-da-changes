GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = object_id(N'[dbo].CLAmortizationUpdateAccountBalance')
			AND OBJECTPROPERTY(id, N'IsProcedure') = 1
		)
	DROP PROCEDURE [dbo].CLAmortizationUpdateAccountBalance
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


----------------------------------------------------------------
-- ==============================================================
-- Author:		Ashish Padwal
-- Create date: 18-06-2019
-- Description:	This procedure will update payment information as per rules given in Amortization.
-- ==================================================================
CREATE PROCEDURE [dbo].[CLAmortizationUpdateAccountBalance] -- Add the parameters for the stored procedure here
	@AcctNo VARCHAR(12)
	,@Amount MONEY
	,@Return INT OUT
	,@IsGFT BIT = 0
AS
BEGIN
SET NOCOUNT ON
	DECLARE @Icount INT = 1
		,@TotalRecords INT
		,@Principal MONEY
		,@ServiceChg MONEY = 0
		,@AdminFee MONEY = 0
		,@Interest MONEY
		,@TotalDeductionSequence INT
		,@SequenceCount INT = 1
		,@SequenceName VARCHAR(20)
		,@InstallmentStartDate DATETIME
		,@InstallmentEndDate DATETIME
		,@Dueday INT
		,@Duedate DATETIME
		,@DailyIntrest MONEY
		,@SettlementFig MONEY
		,@CDAcctNo INT
		,@Instalduedate DATETIME
		,@instalmentamount MONEY
		,@payamount MONEY = @Amount
		,@outstandingBalance MONEY = 0
		,@instaldaydue DATE
		,@daynow DATE
		,@TotalServiceCharge MONEY
		,@ActualCharge MONEY
	DECLARE @HoBranchNo INT
			,@NextRefNo INT
			,@branchno INT
			,@datetrans DATETIME,
			@daysEarlyOrLate int

	--SET @Return = 0
	--SELECT * FROM CLAmortizationPaymentHistory
	--Get total number of records for selected account.
	SELECT @TotalRecords = MAX(installmentNo)
		,@InstallmentStartDate = MIN(instalduedate)
		,@InstallmentEndDate = MAX(instalduedate)
	FROM CLAmortizationPaymentHistory
	WHERE acctno = @AcctNo

	SELECT TOP 1 @instalmentamount = instalamount
	FROM instalplan
	WHERE acctno = @AcctNo

	---Get Start date as disbursement date.
	SET @InstallmentStartDate = dateadd(month, - 1, @InstallmentStartDate)

	----Current months DueDay Get it from Amortize schedule.
	SELECT @Dueday = dueday
	FROM instalplan
	WHERE acctno = @AcctNo

	----Current months duedate Get it from Amortize schedule.
	SELECT TOP 1 @Duedate = cast(instalduedate AS DATE)
	FROM [dbo].[CLAmortizationSchedule]
	WHERE acctno = @AcctNo
		AND cast(instalduedate AS DATE) > cast(getdate() AS DATE)
	ORDER BY instalduedate

	----------------------------------------Start of Early Settlement Calculation---------------------------------------------------------------------------------------------------
	--If payment amount greater than  then consider this as 'Early Settlement'
		EXEC [dbo].DN_GetEarlySettlementFigure @AcctNo
		,@SettlementFig OUT
		,@Return = @Return OUTPUT --SP returning new Oustanding balance

	--If current date is between installmenyt schedule then it is eligible for Daily intrest calculation.
	IF getdate() BETWEEN @InstallmentStartDate
			AND @InstallmentEndDate
	BEGIN
		SET @DailyIntrest = isnull(dbo.fn_CLAmortizationDailyInterest(@AcctNo), 0)

		IF round(cast(@Amount AS MONEY), 2) > = round(cast(@SettlementFig AS MONEY), 2) --If User is settling account Early
		BEGIN ------ We need to add daily intrest in fintrans table ----------------
			IF(@DailyIntrest>0)
			BEGIN
				SELECT @HoBranchNo = HoBranchNo
				FROM Country

				UPDATE Branch
				SET HiRefNo = HiRefNo + 1
				WHERE Branch.BranchNo = @HoBranchNo

				SELECT @NextRefNo = HiRefNo
				FROM Branch
				WHERE BranchNo = @HoBranchNo

				SET @branchno = LEFT(@AcctNo, 3)
				SET @datetrans = getdate()

				EXEC DN_FinTransWriteSP @origbr = @branchno
					,@branchno = @branchno
					,@AcctNo = @AcctNo
					,@transrefno = @NextRefNo
					,@datetrans = @datetrans
					,@transtypecode = 'INT'
					,@empeeno = - 88888
					,@transupdated = 'N'
					,@transprinted = 'N'
					,@transvalue = @DailyIntrest
					,@bankcode = ''
					,@bankacctno = ''
					,@chequeno = ''
					,@ftnotes = 'DNCH'
					,@paymethod = 0
					,@runno = 0
					,@source = 'COSACS'
					,@agrmtno = 1
					,@Return = @Return
			END --END OF @DailyIntrest>0
		END -- END OF SETTELEMENT CONDITION
		--ELSE
		--	BEGIN
				
		--	END
	END

	----------------------------------------Start of Early Settlement Calculation---------------------------------------------------------------------------------------------------
	------If payment amount greater than  then consider this as 'Early Settlement'
	--SELECT *
	--FROM CLAmortizationPaymentHistory
	--WHERE AcctNo = @AcctNo

	
Select 'AMount = ' SELECT @Amount,@SettlementFig

	IF round(cast(@Amount AS MONEY), 2) > = round(cast(@SettlementFig AS MONEY), 2)
	BEGIN
		PRINT 'yes in settlement'

		DECLARE @HoBranchNum INT

		SELECT @HoBranchNum = HoBranchNo
		FROM Country

		--Logic For inserting 'DT' and 'Admin' into Delivery and Fintrans for eartl settlement
		IF OBJECT_ID('tempdb..#tmpBus') IS NOT NULL
			DROP TABLE #tmpBus

		CREATE TABLE #tmpBus (
			ServiceCharge MONEY
			,AdminFee MONEY
			,TotalServiceCharge MONEY
			)

		INSERT INTO #tmpBus
		EXEC [dbo].[DN_GetEarlySettlementFigure] @acctno
			,0
			,0

		DECLARE @ServiceCharge MONEY
			,@NextRefNum INT
			,@hibuffno INT

		SELECT @ServiceCharge = ServiceCharge
		FROM #tmpBus

		DECLARE @buffno INT
			,@stocklocn INT
			,@buffbranchno INT
			,@branchnumber INT
			,@transdate DATETIME
			,@itemidd INT
			,@Chrgeapp MONEY

		SET @itemidd = (
				SELECT id
				FROM stockinfo
				WHERE itemno = 'DT'
				)
		SET @transdate = getdate()

		SELECT @stocklocn = stocklocn
			,@buffbranchno = buffbranchno
			,@branchnumber = branchno
		FROM delivery
		WHERE acctno = @acctno
			AND itemno = 'LOAN'

		UPDATE Branch
		SET HiRefNo = HiRefNo + 1
			,hibuffno = hibuffno + 1
		WHERE Branch.BranchNo = left(@acctno, 3)

		SELECT @NextRefNum = HiRefNo
			,@hibuffno = hibuffno
		FROM Branch
		WHERE BranchNo = left(@acctno, 3)

		SELECT @instaldaydue = CAST(instalduedate AS DATE)
		FROM [dbo].[CLAmortizationSchedule]
		WHERE acctno = @acctno
			AND (DATEPART(year, instalduedate)) = DATEPART(year, getdate())
			AND DATEPART(month, instalduedate) = DATEPART(month, getdate())

		SELECT @daynow = cast(getdate() AS DATE)

		SELECT @TotalServiceCharge = TotalServiceCharge
		FROM #tmpBus

		

		SELECT @Chrgeapp = isnull(Sum(transvalue), 0)
		FROM Delivery
		WHERE acctno = @acctno
			AND itemno IN ('DT')

		

		SELECT @Actualcharge = @Chrgeapp - @TotalServiceCharge

		
		IF (@Actualcharge > 0)
		BEGIN
			SET @Actualcharge = @Actualcharge
		END
		ELSE
		BEGIN
			SET @Actualcharge = @Actualcharge * - 1
		END

		IF EXISTS (
				SELECT 'DT'
				FROM Lineitem
				WHERE acctno = @acctno
					AND itemno IN ('DT')
				)
		BEGIN
			IF (@instaldaydue != @daynow)
			BEGIN
				IF EXISTS (
						SELECT 'DT'
						FROM Delivery
						WHERE acctno = @acctno
							AND itemno IN ('DT')
							AND @Actualcharge > 0
						)
				BEGIN
					UPDATE Delivery
					SET transvalue = isnull(@Actualcharge, 0) + isnull(transvalue, 0)
					WHERE acctno = @acctno
						AND itemno = 'DT'

					INSERT INTO Fintrans (
						origbr
						,branchno
						,acctno
						,transrefno
						,datetrans
						,transtypecode
						,empeeno
						,transupdated
						,transprinted
						,transvalue
						,bankcode
						,bankacctno
						,chequeno
						,ftnotes
						,paymethod
						,runno
						,source
						,agrmtno
						,ExportedToTallyman
						)
					VALUES (
						0
						,@branchnumber
						,@acctno
						,@NextRefNum
						,@transdate
						,'DEL'
						,0
						,'N'
						,'N'
						,@Actualcharge
						,''
						,''
						,''
						,'DND2'
						,''
						,0
						,'cosacs'
						,1
						,0
						)
				END --IF END
				ELSE IF (@Actualcharge > 0)
				BEGIN
					INSERT INTO Delivery (
						origbr
						,acctno
						,agrmtno
						,datedel
						,delorcoll
						,itemno
						,stocklocn
						,quantity
						,retitemno
						,retstocklocn
						,retval
						,buffno
						,buffbranchno
						,datetrans
						,branchno
						,transrefno
						,transvalue
						,runno
						,contractno
						,ReplacementMarker
						,NotifiedBy
						,ftnotes
						,InvoiceLineNo
						,ExtInvoice
						,ParentItemNo
						,ItemID
						,ParentItemID
						,RetItemID
						,BrokerExRunNo
						)
					VALUES (
						0
						,@acctno
						,1
						,@transdate
						,'D'
						,'DT'
						,@stocklocn
						,1
						,''
						,0
						,0
						,@hibuffno
						,@buffbranchno
						,@transdate
						,@branchnumber
						,@NextRefNum
						,@Actualcharge
						,0
						,''
						,NULL
						,1000
						,'DND2'
						,NULL
						,NULL
						,0
						,@itemidd
						,0
						,0
						,0
						)
				END --ELSE IF END
			END --IF END
			ELSE
			BEGIN
				IF EXISTS (
						SELECT 'DT'
						FROM Delivery
						WHERE acctno = @acctno
							AND itemno IN ('DT')
							AND @Actualcharge > 0
						)
				BEGIN
					UPDATE Delivery
					SET transvalue = isnull(@Actualcharge, 0) + isnull(transvalue, 0)
					WHERE acctno = @acctno
						AND itemno = 'DT'

					INSERT INTO Fintrans (
						origbr
						,branchno
						,acctno
						,transrefno
						,datetrans
						,transtypecode
						,empeeno
						,transupdated
						,transprinted
						,transvalue
						,bankcode
						,bankacctno
						,chequeno
						,ftnotes
						,paymethod
						,runno
						,source
						,agrmtno
						,ExportedToTallyman
						)
					VALUES (
						0
						,@branchnumber
						,@acctno
						,@NextRefNum
						,@transdate
						,'DEL'
						,- 88888
						,'N'
						,'N'
						,@Actualcharge
						,''
						,''
						,''
						,'DND2'
						,''
						,0
						,'cosacs'
						,1
						,0
						)
				END --IF END
				ELSE IF (@Actualcharge > 0)
				BEGIN
					INSERT INTO Delivery (
						origbr
						,acctno
						,agrmtno
						,datedel
						,delorcoll
						,itemno
						,stocklocn
						,quantity
						,retitemno
						,retstocklocn
						,retval
						,buffno
						,buffbranchno
						,datetrans
						,branchno
						,transrefno
						,transvalue
						,runno
						,contractno
						,ReplacementMarker
						,NotifiedBy
						,ftnotes
						,InvoiceLineNo
						,ExtInvoice
						,ParentItemNo
						,ItemID
						,ParentItemID
						,RetItemID
						,BrokerExRunNo
						)
					VALUES (
						0
						,@acctno
						,1
						,@transdate
						,'D'
						,'DT'
						,@stocklocn
						,1
						,''
						,0
						,0
						,@hibuffno
						,@buffbranchno
						,@transdate
						,@branchnumber
						,@NextRefNum
						,@Actualcharge
						,0
						,''
						,NULL
						,1000
						,'DND2'
						,NULL
						,NULL
						,0
						,@itemidd
						,0
						,0
						,0
						)
				END
			END
		END

		SELECT @adminfee = isnull(AdminFee, 0)
		FROM #tmpBus

		DECLARE @stocklocation INT
			,@buffbranchnum INT
			,@branchnumbers INT
			,@transdates DATETIME
			,@itemid VARCHAR(12)

		SET @transdates = getdate()

		SELECT @stocklocation = stocklocn
			,@buffbranchnum = buffbranchno
			,@branchnumbers = branchno
		FROM delivery
		WHERE acctno = @acctno
			AND itemno = 'LOAN'

		DECLARE @HoBranchNumber INT
			,@NextRefNumber INT
			,@hibuffnumber INT

		SELECT @HoBranchNumber = HoBranchNo
		FROM Country

		SET @itemid = (
				SELECT id
				FROM stockinfo
				WHERE itemno = 'ADMIN'
				)

		DECLARE @datefirst DATETIME

		SELECT @datefirst = DATEFIRST
		FROM instalplan
		WHERE acctno = @acctno

		DECLARE @ActualAdmin MONEY

		SELECT @ActualAdmin = isnull(Sum(transvalue), 0)
		FROM Delivery
		WHERE acctno = @acctno
			AND itemno IN ('ADMIN')

		
		IF EXISTS (
				SELECT 'A'
				FROM Lineitem
				WHERE acctno = @acctno
					AND itemno IN ('ADMIN')
				)
		BEGIN
			IF EXISTS (
					SELECT 'ADMIN'
					FROM Delivery
					WHERE acctno = @acctno
						AND itemno IN ('ADMIN')
						AND GETDATE() != @datefirst
						AND @ActualAdmin > 0
					)
				--BEGIN
			BEGIN
				
				UPDATE Branch
				SET HiRefNo = HiRefNo + 1
				WHERE Branch.BranchNo = @branchnumbers

				SELECT @NextRefNumber = HiRefNo
				FROM Branch
				WHERE BranchNo = @branchnumbers

				PRINT @NextRefNumber

				declare @RemainingAdminFee Money 
				set @RemainingAdminFee = isnull(@adminfee, 0) + (Select Sum(PrevAdminFee) from  CLAmortizationPaymentHistory WHERE acctno = @AcctNo)
				
				
				set @adminfee = @RemainingAdminFee - (Select sum(transvalue) from delivery WHERE acctno = @acctno
					AND itemno = 'ADMIN')

				UPDATE Delivery
				SET transvalue =@RemainingAdminFee WHERE acctno = @acctno AND itemno = 'ADMIN'
	
				INSERT INTO Fintrans (
					origbr
					,branchno
					,acctno
					,transrefno
					,datetrans
					,transtypecode
					,empeeno
					,transupdated
					,transprinted
					,transvalue
					,bankcode
					,bankacctno
					,chequeno
					,ftnotes
					,paymethod
					,runno
					,source
					,agrmtno
					,ExportedToTallyman
					)
				VALUES (
					0
					,@branchnumber
					,@acctno
					,@NextRefNumber
					,@transdate
					,'DEL'
					,- 8888
					,'N'
					,'N'
					,@adminfee
					,''
					,''
					,''
					,'DND2'
					,''
					,0
					,'COSACS'
					,1
					,0
					)
					
			END --IF END
					--END
			ELSE IF (@adminfee > 0)
			BEGIN
				UPDATE Branch
				SET HiRefNo = HiRefNo + 1
					,hibuffno = hibuffno + 1
				WHERE Branch.BranchNo = @branchnumbers

				SELECT @NextRefNumber = HiRefNo
					,@hibuffnumber = hibuffno
				FROM Branch
				WHERE BranchNo = @branchnumbers

			
				INSERT INTO Delivery (
					origbr
					,acctno
					,agrmtno
					,datedel
					,delorcoll
					,itemno
					,stocklocn
					,quantity
					,retitemno
					,retstocklocn
					,retval
					,buffno
					,buffbranchno
					,datetrans
					,branchno
					,transrefno
					,transvalue
					,runno
					,contractno
					,ReplacementMarker
					,NotifiedBy
					,ftnotes
					,InvoiceLineNo
					,ExtInvoice
					,ParentItemNo
					,ItemID
					,ParentItemID
					,RetItemID
					,BrokerExRunNo
					)
				VALUES (
					@buffbranchnum
					,@acctno
					,1
					,@transdates
					,'D'
					,'Admin'
					,@stocklocn
					,1
					,''
					,0
					,0
					,@hibuffnumber
					,@buffbranchnum
					,@transdates
					,@branchnumbers
					,@NextRefNumber
					,@adminfee
					,0
					,''
					,NULL
					,1000
					,'DND2'
					,NULL
					,NULL
					,0
					,@itemid
					,0
					,0
					,0
					)
			END --ELSE IF END
		END --lineitem
				----Then do not take ServiceChange and admin fee from next month.So mark it as zero.

		UPDATE CLAmortizationPaymentHistory
		SET servicechg = 0
			,adminfee = 0
		WHERE acctno = @AcctNo
			AND CAST(instalduedate AS DATE) >= @Duedate

		UPDATE CLAmortizationPaymentHistory
		SET Prevservicechg = Prevservicechg + ISNULL(servicechg, 0)
			,prevadminfee = prevadminfee + adminfee
			,prevPrincipal = prevPrincipal + Principal
			,Principal = 0
			,prevInterest = prevInterest + Interest
			,Interest = 0
			,servicechg = 0
			,adminfee = 0
		WHERE acctno = @AcctNo

		---------Update account detail after payment-------------------------------
		IF (
				(@Amount <> - 1)
				AND (@IsGFT = 0)
				)
		BEGIN
			UPDATE acct
			SET outstbal = 0
				,currstatus = 'S'
			WHERE acctno = @AcctNo
		END

		---After early settelement  do not deduct anything return from SP.
		RETURN;
	END -----------------------------------------Start of Daily Intrest Calculation-------------------------------------------------------------------------

	SELECT @TotalDeductionSequence = count(id)
	FROM CL_PaymentDeductionSeq

	--Loop through each row and Deduct amount .
	WHILE (@Icount <= @TotalRecords)
	BEGIN ----Fetch selected values in variables.
		SELECT @Principal = principal
			,@ServiceChg = servicechg
			,@AdminFee = adminfee
			,@Interest = interest
			,@Instalduedate = InstalDueDate
		FROM CLAmortizationPaymentHistory
		WHERE acctno = @AcctNo
			AND installmentNo = @Icount -----fetch sequence order to deduct amount----
		
		------------------------------------------------------------------------------
		-- Check INT for each installment
		------------------------------------------------------------------------------
		--SELECT MONTH('2017/05/25 09:08') AS Month;
		SET @daysEarlyOrLate = datediff(dd,@Instalduedate,getdate())
		SET @DailyIntrest = isnull(dbo.fn_CLAmortizationDailyInterest(@AcctNo), 0)
		IF(	@DailyIntrest<>0 AND 
			(
				((abs(@daysEarlyOrLate) between 1 AND 31)
					AND MONTH(@Instalduedate) IN (1,3,5,7,8,10,12))--Months of 31 days
				OR 
				((abs(@daysEarlyOrLate) between 1 AND 30) 
					AND MONTH(@Instalduedate) IN (4,6,9,11))
				OR
				((abs(@daysEarlyOrLate) between 1 AND 28) 
					AND MONTH(@Instalduedate) IN (2))
				)
			)
		BEGIN
			SELECT @HoBranchNo = HoBranchNo
			FROM Country

			UPDATE Branch
			SET HiRefNo = HiRefNo + 1
			WHERE Branch.BranchNo = @HoBranchNo

			SELECT @NextRefNo = HiRefNo
			FROM Branch
			WHERE BranchNo = @HoBranchNo

			SET @branchno = LEFT(@AcctNo, 3)
			SET @datetrans = getdate()

			EXEC DN_FinTransWriteSP @origbr = @branchno
				,@branchno = @branchno
				,@AcctNo = @AcctNo
				,@transrefno = @NextRefNo
				,@datetrans = @datetrans
				,@transtypecode = 'INT'
				,@empeeno = - 88888
				,@transupdated = 'N'
				,@transprinted = 'N'
				,@transvalue = @DailyIntrest
				,@bankcode = ''
				,@bankacctno = ''
				,@chequeno = ''
				,@ftnotes = 'DNCH'
				,@paymethod = 0
				,@runno = 0
				,@source = 'COSACS'
				,@agrmtno = 1
				,@Return = @Return
		END
		--****************************************************************************

		WHILE (@SequenceCount <= @TotalDeductionSequence)
		BEGIN --Fetch sequence name  as per sequence specified in table.
			SELECT @SequenceName = SequenceName
			FROM CL_PaymentDeductionSeq
			WHERE id = @SequenceCount

			IF (@Amount > 0)
			BEGIN
				----If SequenceName is Amortized Interest-----
				IF @SequenceName = 'Amortized Interest'
				BEGIN
					IF ((@Amount - @ServiceChg) >= 0)
					BEGIN
						--If Customer is paying month early then do not charge service charge for that month. --AND @instalmentamount > = @amount
						IF (NOT (cast(getdate() AS DATE) <= CAST((DATEADD(MONTH, - 1, @Instalduedate)) AS DATE)))
						BEGIN
							SET @Amount = @Amount - @ServiceChg --Calling SP to update deducted amount from CLAmortization history

							EXEC CLAmortizationUpdateDeductedBalance @AcctNo
								,@Icount
								,@ServiceChg
								,'ServiceCharge'

							UPDATE CLAmortizationPaymentHistory
							SET servicechg = 0
								,isPaid = 1
							WHERE acctno = @AcctNo
								AND installmentNo = @Icount
						END
						ELSE
						BEGIN --- Skip Service charge 
							--Calling SP to service charge as 0 because customer hasn't paid service charge.
							IF (@instalmentamount <= @amount) --
							BEGIN
								--print 'SKIPPING THE SERVICE CH'								
								--print 'DO not charge service charge'
								EXEC CLAmortizationUpdateDeductedBalance @AcctNo
									,@Icount
									,0
									,'ServiceCharge'

								UPDATE CLAmortizationPaymentHistory
								SET PrevServiceChg = @ServiceChg
									,servicechg = 0
									,isPaid = 1
								WHERE acctno = @AcctNo
									AND installmentNo = @Icount
							END
							ELSE
							BEGIN
								--print 'EARLY BUT NOT SKIP'
								SET @Amount = @Amount - @ServiceChg --Calling SP to update deducted amount from CLAmortization history

								EXEC CLAmortizationUpdateDeductedBalance @AcctNo
									,@Icount
									,@ServiceChg
									,'ServiceCharge'

								UPDATE CLAmortizationPaymentHistory
								SET servicechg = 0
									,isPaid = 1
								WHERE acctno = @AcctNo
									AND installmentNo = @Icount

								SET @ServiceChg = 0
							END
						END

						SET @ServiceChg = 0 --Update values in  CLAmortizationPaymentHistory table.
					END -- END of  IF((@Amount - @ServiceChg)> 0)
					ELSE
					BEGIN
						SET @ServiceChg = @ServiceChg - @Amount

						IF (NOT (cast(getdate() AS DATE) <= CAST((DATEADD(MONTH, - 1, @Instalduedate)) AS DATE)))
						BEGIN --Calling SP to update deducted amount from CLAmortization history
							EXEC CLAmortizationUpdateDeductedBalance @AcctNo
								,@Icount
								,@Amount
								,'ServiceCharge'

							SET @Amount = 0
						END
						ELSE
						BEGIN --Calling SP to update deducted amount from CLAmortization history
							EXEC CLAmortizationUpdateDeductedBalance @AcctNo
								,@Icount
								,0
								,'ServiceCharge'
						END --Update values in  CLAmortizationPaymentHistory table.

						UPDATE CLAmortizationPaymentHistory
						SET PrevServiceChg = ISNULL(PrevServiceChg, 0) + ISNULL(@Amount, 0)
							,servicechg = @ServiceChg
							,isPaid = 1
						WHERE acctno = @AcctNo
							AND installmentNo = @Icount

						SET @ServiceChg = 0
						SET @Icount = @TotalRecords + 1 -- To stop looping and calculations

						BREAK;
					END
				END -- END of IF @SequenceName = 'Amortized Interest' 

				---------------If SequenceName is Amortized Interest-----
				IF @SequenceName = 'Admin Fees'
				BEGIN
					IF ((@Amount - @AdminFee) > 0)
					BEGIN
						-- Calculating credit transaction value
						EXEC CLAmortizationUpdateDeductedBalance @AcctNo
							,@Icount
							,@AdminFee
							,'AdminFee'

						SET @Amount = @Amount - @AdminFee
						SET @AdminFee = 0.00

						--Update values in CLAmortizationPaymentHistory table.
						UPDATE CLAmortizationPaymentHistory
						SET adminfee = @AdminFee
							,isPaid = 1
						WHERE acctno = @AcctNo
							AND installmentNo = @Icount
					END
					ELSE
					BEGIN
						SET @AdminFee = @AdminFee - @Amount

						-- Calculating credit transaction value
						EXEC CLAmortizationUpdateDeductedBalance @AcctNo
							,@Icount
							,@Amount
							,'AdminFee'

						--Update values in CLAmortizationPaymentHistory table.
						UPDATE CLAmortizationPaymentHistory
						SET adminfee = @AdminFee
							,isPaid = 1
						WHERE acctno = @AcctNo
							AND installmentNo = @Icount

						SET @Amount = 0
						SET @Icount = @TotalRecords + 1 -- To stop looping and calculations.

						BREAK;
					END -- END of ELSE
				END ----If SequenceName is Amortized Interest-----

				IF @SequenceName = 'Principal'
				BEGIN
					IF ((@Amount - @Principal) > 0)
					BEGIN
						EXEC CLAmortizationUpdateDeductedBalance @AcctNo
							,@Icount
							,@Principal
							,'Principal'

						SET @Amount = @Amount - @Principal
						SET @Principal = 0 --Update values in CLAmortizationPaymentHistory table.

						UPDATE CLAmortizationPaymentHistory
						SET principal = @Principal
							,isPaid = 1
						WHERE acctno = @AcctNo
							AND installmentNo = @Icount
					END
					ELSE
					BEGIN
						EXEC CLAmortizationUpdateDeductedBalance @AcctNo
							,@Icount
							,@Amount
							,'Principal'

						SET @Principal = @Principal - @Amount --Update values in CLAmortizationPaymentHistory table.

						UPDATE CLAmortizationPaymentHistory
						SET principal = @Principal
							,isPaid = 1
						WHERE acctno = @AcctNo
							AND installmentNo = @Icount

						SET @Amount = 0
						SET @Icount = @TotalRecords + 1 -- To stop looping and calculations.

						BREAK;
					END --END of ELSE
				END ----If SequenceName is Amortized Interest-----

				IF @SequenceName = 'Penalty Int'
				BEGIN
					IF ((@Amount - @Interest) > 0)
					BEGIN
						--PRINT 'Penalty Int' -- Calculating credit transaction value
						EXEC CLAmortizationUpdateDeductedBalance @AcctNo
							,@Icount
							,@Interest
							,'Interest'

						SET @Amount = @Amount - @Interest
						SET @Interest = 0 --Update values in CLAmortizationPaymentHistory table.

						UPDATE CLAmortizationPaymentHistory
						SET interest = @Interest
							,isPaid = 1
						WHERE acctno = @AcctNo
							AND installmentNo = @Icount --PRINT @Interest
					END
					ELSE
					BEGIN
						SET @Amount = 0
						SET @Interest = @Interest - @Amount -- Calculating credit transaction value

						EXEC CLAmortizationUpdateDeductedBalance @AcctNo
							,@Icount
							,@Amount
							,'Interest'

						UPDATE CLAmortizationPaymentHistory
						SET interest = @Interest
							,isPaid = 1
						WHERE acctno = @AcctNo
							AND installmentNo = @Icount

						SET @Icount = @TotalRecords + 1 -- To stop looping and calculations

						BREAK;
					END -- END of ELSE
				END
			END -- END of IF (@Amount > 0)
					--Increament sequence counter by 1

			SET @SequenceCount = @SequenceCount + 1;
		END -- END of WHILE (@SequenceCount <= @TotalDeductionSequence)

		SET @SequenceCount = 1
		SET @Icount = @Icount + 1
	END -- END of WHILE (i <= @TotalRecords)


	---------Update account detail after payment-----------------------	--------
	IF (
			(@Amount <> - 1)
			AND (@IsGFT = 0)
			)
	BEGIN
		EXEC [CLAmortizationCalcDailyOutstandingBalance] @AcctNo
			,@outstandingBalance = @outstandingBalance OUTPUT

		UPDATE acct
		SET outstbal = @outstandingBalance
		WHERE acctno = @AcctNo
	END

	IF (@@error != 0)
		SET @Return = @@error
END
GO


