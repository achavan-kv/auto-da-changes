SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

--------------------------------------------------------------------------------------------------------------------------------
-- =============================================================================================================================
-- Author:		AShwini Akula
-- Create date: 30-07-2019
-- Description:	This procedure will Insert transValue and trans Type in Payment details table for CAShloan amortization for BC1 and BCA
-- =============================================================================================================================
IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = object_id(N'[dbo].[CLAmortizationInsertBrokerDataDetailsSP]')
			AND OBJECTPROPERTY(id, N'IsProcedure') = 1
		)
	DROP PROCEDURE [dbo].[CLAmortizationInsertBrokerDataDetailsSP]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET NOCOUNT OFF; 
Go
CREATE  PROCEDURE [dbo].[CLAmortizationInsertBrokerDataDetailsSP]
AS
BEGIN
	BEGIN TRAN

	DECLARE @LastSalesExRunDate DATETIME
		,@RawDecimalsSetting VARCHAR(1500)
		,@Decimals INT
	    ,@HoBranchNo INT
		,@NextRefNo INT
		,@NextRefNumber INT
		,@Nexthibuffno INT
		,@hibuffno INT
		,@branchno INT
		,@datetrans DATETIME

	SELECT @RawDecimalsSetting = [Value]
	FROM CountryMaintenance
	WHERE CodeName = 'decimalplaces'

	BEGIN TRY
		SET @Decimals = CAST(SUBSTRING(@RawDecimalsSetting, 2, LEN(@RawDecimalsSetting) - 1) AS INT)
	END TRY

	BEGIN CATCH
		SET @Decimals = 2
	END CATCH

	SELECT @LastSalesExRunDate = max(datefinish)
	FROM interfacecontrol
	WHERE interface = 'COS FACT'
		AND result = 'P'

	BEGIN TRY
		CREATE TABLE #AcctsForBCIBCA (
			AcctNo VARCHAR(15)
			,transvalue MONEY
			,transtypecode VARCHAR(5)
			,CreditDebitNo INT
			)

		CREATE CLUSTERED INDEX ix_insert_tmpAcctsForBCIBCA ON #AcctsForBCIBCA (
			AcctNo
			,transtypecode
			,CreditDebitNo
			)

		SELECT h.AcctNo AS AcctNo
			,servicechg
			,'BCI' AS code
			,A.termstype AS termstype
			,inspcent AS inspcent
			,ROUND((servicechg * inspcent * 0.01), 2) AS BCI
			,CASE 
				WHEN i.InsIncluded = 1
					THEN ROUND((servicechg - (servicechg * inspcent * 0.01)), 2)
				ELSE servicechg
				END AS BCS
			,h.AdminFee AS AdminFee
			,h.InstalDueDate AS InstalDueDate
		INTO #tempData
		FROM dbo.CLAmortizationSchedule h
		JOIN acct a ON a.AcctNo = h.AcctNo
			AND InstalDueDate BETWEEN DATEADD(DAY, - 10, GETDATE())
				AND GETDATE()
			AND servicechg <> 0
		JOIN CUSTACCT ca ON ca.AcctNo = a.AcctNo
		JOIN CUSTOMER c ON c.custid = ca.custid
		JOIN TERMSTYPEALLBANDS i ON A.TermsType = i.TermsType
			AND i.band = c.ScoringBand
		WHERE a.currstatus != 'S' --to stop pushing entries every due date if the account is settled possibly by Early settlement

		INSERT INTO #AcctsForBCIBCA
		SELECT AcctNo
			,BCS
			,'BCS'
			,dbo.fn_CLAGetTranstypeAcctNo('BCS', 1)
		FROM #tempData
		WHERE InstalDueDate BETWEEN @LastSalesExRunDate
				AND GETDATE()
			AND BCS <> 0
		
		UNION ALL
		
		SELECT AcctNo
			,AdminFee
			,'BCA'
			,dbo.fn_CLAGetTranstypeAcctNo('BCA', 1)
		FROM #tempData
		WHERE InstalDueDate BETWEEN @LastSalesExRunDate
				AND GETDATE()
			AND AdminFee <> 0
		
		UNION ALL
		
		SELECT AcctNo
			,BCI
			,'BCI'
			,dbo.fn_CLAGetTranstypeAcctNo('BCI', 1)
		FROM #tempData
		WHERE InstalDueDate BETWEEN @LastSalesExRunDate
				AND GETDATE()
			AND BCI <> 0
		
		UNION ALL
		
		SELECT AcctNo
			,BCS * - 1
			,'BCS'
			,dbo.fn_CLAGetTranstypeAcctNo('BCS', - 1)
		FROM #tempData
		WHERE InstalDueDate BETWEEN @LastSalesExRunDate
				AND GETDATE()
			AND BCS <> 0
		
		UNION ALL
		
		SELECT AcctNo
			,AdminFee * - 1
			,'BCA'
			,dbo.fn_CLAGetTranstypeAcctNo('BCA', - 1)
		FROM #tempData
		WHERE InstalDueDate BETWEEN @LastSalesExRunDate
				AND GETDATE()
			AND AdminFee <> 0
		
		UNION ALL
		
		SELECT AcctNo
			,BCI * - 1
			,'BCI'
			,dbo.fn_CLAGetTranstypeAcctNo('BCI', - 1)
		FROM #tempData
		WHERE InstalDueDate BETWEEN @LastSalesExRunDate
				AND GETDATE()
			AND BCI <> 0

		INSERT INTO dbo.CLANewPaymentDetails (
			AcctNo
			,transvalue
			,transtypecode
			,CreditDebitNo
			)
		SELECT AcctNo
			,transvalue
			,transtypecode
			,CreditDebitNo
		FROM #AcctsForBCIBCA
		ORDER BY AcctNo
			,transtypecode

		DROP TABLE #tempData

		DROP TABLE #AcctsForBCIBCA

		------Update entry in Delivery table based on due date of accounts----------------------
		DECLARE @iCount INT = 1
			,@TotalCount INT
			,@acctno VARCHAR(12)
			,@servicechg MONEY
			,@adminfee MONEY
			,@totalTransvalueDT MONEY
			,@totalTransvalueAdmin MONEY
			,@Return INT
			,@buffno INT
			,@transrefno INT
			,@stocklocn INT
			,@buffbranchno INT
			,@branchnumber INT
			,@transdate DATETIME
			,@HoBranchNum INT
			,@DTitemid VARCHAR(8)
			,@AdminItemid VARCHAR(8)

		IF (
				EXISTS (
					SELECT *
					FROM INFORMATION_SCHEMA.TABLES
					WHERE TABLE_NAME = '#tblupdateDT'
					)
				)
		BEGIN
			DROP TABLE #tblupdateDT
		END

		CREATE TABLE #tblupdateDT (
			ID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
			,acctno VARCHAR(12)
			,servicechg MONEY
			,adminfee MONEY
			)

		INSERT INTO #tblupdateDT
		SELECT DISTINCT CLAmortizationSchedule.acctno
			,CLAmortizationSchedule.servicechg
			,CLAmortizationSchedule.adminfee
		FROM [dbo].[CLAmortizationSchedule]
		INNER JOIN acct ON acct.acctno = CLAmortizationSchedule.acctno
		INNER JOIN cashloan cl on cl.acctno = CLAmortizationSchedule.acctno
		WHERE cast(instalduedate AS DATE) > cast(@LastSalesExRunDate AS DATE)  and cast(instalduedate AS DATE)<= cast(getdate() AS DATE) 
			AND acct.currstatus <> 'S'
			AND cl.Loanstatus='D'

		SELECT @TotalCount = Count(acctno)
		FROM #tblupdateDT

		WHILE (@iCount <= @TotalCount)
		BEGIN
			---------Fetch Acctno from temptable and update in Delivery Table--------------------
			PRINT @iCount

			SELECT @acctno = acctno
				,@servicechg = servicechg
				,@adminfee = adminfee
			FROM #tblupdateDT
			WHERE ID = @iCount

			SELECT @HoBranchNo = HoBranchNo
			FROM Country

			UPDATE Branch
			SET HiRefNo = HiRefNo + 1
			WHERE Branch.BranchNo = LEFT(@AcctNo, 3)

			SELECT @NextRefNo = HiRefNo
			FROM Branch
			WHERE BranchNo = LEFT(@AcctNo, 3)

			IF EXISTS (
					SELECT 'A'
					FROM delivery
					WHERE acctno = @acctno
						AND itemno = 'DT'
					)
			BEGIN
				IF NOT EXISTS (
						SELECT 'A'
						FROM delivery
						WHERE acctno = @acctno
							AND itemno = 'DT'
							AND cast(datetrans AS DATE) = cast(getdate() AS DATE)
						)
				BEGIN
					UPDATE delivery
					SET transvalue = transvalue + @servicechg
						,datetrans = getdate()
						,datedel = getdate()
					WHERE acctno = @acctno
						AND itemno = 'DT'
						AND cast(datetrans AS DATE) <> cast(getdate() AS DATE)

					----------------------Add record for DT in fintrans table------------------------------------------------------------------
					SET @branchno = LEFT(@acctno, 3)
					SET @datetrans = getdate()

					SELECT @acctno

					BEGIN
						INSERT INTO fintrans (
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
							)
						VALUES (
							@branchno
							,@branchno
							,@AcctNo
							,@NextRefNo
							,@datetrans
							,'DEL'
							,- 88888
							,'N'
							,'N'
							,@servicechg
							,''
							,''
							,''
							,'DNCH'
							,0
							,0
							,'COSACS'
							,1
							)
					END
				END
			END
			ELSE
			BEGIN
				----Insert new record for DT
				UPDATE Branch
				SET hibuffno = hibuffno + 1
				WHERE Branch.BranchNo = LEFT(@AcctNo, 3)

				SELECT @hibuffno = hibuffno
				FROM Branch
				WHERE BranchNo = LEFT(@AcctNo, 3)

				SET @transdate = getdate()

				SELECT @stocklocn = stocklocn
					,@buffbranchno = buffbranchno
					,@branchnumber = branchno
				FROM delivery
				WHERE acctno = @acctno
					AND itemno = 'LOAN'

				SELECT @DTitemid = id
				FROM stockinfo
				WHERE itemno = 'DT'

				SELECT @HoBranchNum = HoBranchNo
				FROM Country

				IF(@servicechg <> 0)
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
					@HoBranchNum
					,@acctno
					,1
					,getdate()
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
					,@NextRefNo
					,@servicechg
					,0
					,''
					,NULL
					,1000
					,'DND2'
					,NULL
					,NULL
					,0
					,@DTitemid
					,0
					,0
					,0
					)
			END

			UPDATE Branch
			SET HiRefNo = HiRefNo + 1
			WHERE Branch.BranchNo = LEFT(@AcctNo, 3)

			SELECT @NextRefNumber = HiRefNo
			FROM Branch
			WHERE BranchNo = LEFT(@AcctNo, 3)

			IF EXISTS (
					SELECT 'A'
					FROM delivery
					WHERE acctno = @acctno
						AND itemno = 'ADMIN'
					)
			BEGIN
				IF NOT EXISTS (
						SELECT 'A'
						FROM delivery
						WHERE acctno = @acctno
							AND itemno = 'ADMIN'
							AND cast(datetrans AS DATE) = cast(getdate() AS DATE)
						)
				BEGIN
					UPDATE delivery
					SET transvalue = transvalue + @adminfee
						,datetrans = getdate()
						,datedel = getdate()
					WHERE acctno = @acctno
						AND itemno = 'ADMIN'
						AND cast(datetrans AS DATE) <> cast(getdate() AS DATE)

					------Add entry of Admin in fintans table-----------------------------------------------------------
					SET @branchno = LEFT(@AcctNo, 3)
					SET @datetrans = getdate()

					BEGIN
						INSERT INTO fintrans (
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
							)
						VALUES (
							@branchno
							,@branchno
							,@AcctNo
							,@NextRefNumber
							,@datetrans
							,'DEL'
							,- 88888
							,'N'
							,'N'
							,@adminfee
							,''
							,''
							,''
							,'DNCH'
							,0
							,0
							,'COSACS'
							,1
							)
					END
				END
			END
			ELSE
			BEGIN
				----Insert new record for ADMIN
				UPDATE Branch
				SET hibuffno = hibuffno + 1
				WHERE Branch.BranchNo = LEFT(@AcctNo, 3)

				SELECT @Nexthibuffno = hibuffno
				FROM Branch
				WHERE BranchNo = LEFT(@AcctNo, 3)

				SET @transdate = getdate()

				SELECT @stocklocn = stocklocn
					,@buffbranchno = buffbranchno
					,@branchnumber = branchno
				FROM delivery
				WHERE acctno = @acctno
					AND itemno = 'LOAN'

				SELECT @AdminItemid = id
				FROM stockinfo
				WHERE itemno = 'ADMIN'

				SELECT @HoBranchNum = HoBranchNo
				FROM Country

				IF(@adminfee <> 0)
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
					@HoBranchNum
					,@acctno
					,1
					,getdate()
					,'D'
					,'ADMIN'
					,@stocklocn
					,1
					,''
					,0
					,0
					,@Nexthibuffno
					,@buffbranchno
					,@transdate
					,@branchnumber
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
					,@AdminItemid
					,0
					,0
					,0
					)
			END

			SET @iCount = @iCount + 1
		END

		COMMIT
	END TRY

	BEGIN CATCH
		IF @@TRANCOUNT > 0
		BEGIN
			ROLLBACK TRANSACTION;
		END

		DECLARE @ErrorMessage NVARCHAR(4000);
		DECLARE @ErrorSeverity INT;
		DECLARE @ErrorState INT;

		SELECT 
			@ErrorMessage = ERROR_MESSAGE(),
			@ErrorSeverity = ERROR_SEVERITY(),
			@ErrorState = ERROR_STATE();

		-- Use RAISERROR inside the CATCH block to return error
		-- information about the original error that caused
		-- execution to jump to the CATCH block.
		RAISERROR (@ErrorMessage, -- Message text.
				   @ErrorSeverity, -- Severity.
				   @ErrorState -- State.
				   );
	END CATCH
END