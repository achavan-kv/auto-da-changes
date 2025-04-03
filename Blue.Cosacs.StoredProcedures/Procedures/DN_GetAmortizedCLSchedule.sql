IF EXISTS (
		SELECT 1
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID('[dbo].[DN_GetAmortizedCLSchedule]')
			AND OBJECTPROPERTY(id, 'IsProcedure') = 1
		)
	DROP PROCEDURE [dbo].[DN_GetAmortizedCLSchedule]
GO

CREATE PROCEDURE [dbo].[DN_GetAmortizedCLSchedule]
-- ==================================================================================================================
-- Project: CoSACS.NET
-- Produre Name : [dbo].[DN_GetAmortizedCLSchedule]
-- Version: 002
--
-- Change Control
-- --------------
-- Ver    Date        By                Description
-- ---    ----        --                -----------
-- 002  19/11/2020    Ritesh Joge       log #7426991 - Fixed showing incorrect order of sequencing
-- ==================================================================================================================
@acctno NVARCHAR(12)
,@return INT OUTPUT
AS

		DECLARE @instalduedate DATETIME
		DECLARE @cumint DECIMAL(15, 2)
		DECLARE @int DECIMAL(15, 2)
		DECLARE @count INT
		DECLARE @counter INT
		
		SET @return = 0
		SET @cumint = 0
		SET @counter = 1
		
		CREATE TABLE #temp (
			InstalNum INT identity(1, 1)
			,InstalDate DATETIME
			,OpeningBalance DECIMAL(15, 2)
			,InstalAmt DECIMAL(15, 2)
			,Principal DECIMAL(15, 2)
			,Interest DECIMAL(15, 2)
			,ClosingBalance DECIMAL(15, 2)
			,CumInterest DECIMAL(15, 2)
			)
		
		INSERT INTO #temp (
			InstalDate
			,OpeningBalance
			,InstalAmt
			,Principal
			,Interest
			,ClosingBalance
			)
		SELECT instalduedate
			,openingbal
			,instalment
			,principal
			,servicechg
			,closingbal
		FROM CLAmortizationSchedule
		WHERE acctno = @acctno
		ORDER BY instalduedate ASC
		
		SELECT @count = count(*)
		FROM #temp
		
		WHILE (@counter <= @count)
		BEGIN
			SELECT @int = Interest
			FROM #temp
			WHERE InstalNum = @counter
		
			SET @cumint = @cumint + @int
		
			UPDATE #temp
			SET CumInterest = @cumint
			WHERE InstalNum = @counter
		
			SET @counter = @counter + 1
		END
		
		SELECT *
		FROM #temp
		
		IF OBJECT_ID('tempdb..#temp') IS NOT NULL
		BEGIN
			DROP TABLE #temp
		END
		
		IF (@@error != 0)
		BEGIN
			SET @return = @@error
		END