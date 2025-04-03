IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[DN_DisplayAmortizationScheduleSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DN_DisplayAmortizationScheduleSP]
GO


CREATE PROC [dbo].[DN_DisplayAmortizationScheduleSP]
-- ==================================================================================================================
-- Project      : CoSACS.NET
-- Produre Name : [dbo].[DN_DisplayAmortizationScheduleSP]
-- Version        : 004
--
-- Change Control
-- --------------
-- Ver    Date        By                Description
-- ---    ----        --                -----------
-- 002  19/11/2020    Ritesh Joge       log #7426991 - Fixed showing incorrect order of sequencing
-- 003  25/11/2020    Ritesh Joge       log #7229394 - Showing wrong entry of INT charges in CLA Schadule.
-- 004  25/11/2020    Ritesh Joge       Comment set LatePmtFee=0, PenaltyFee=0 first month in while loop. So that it shows penalty INT of first month
-- ==================================================================================================================
@acctno NVARCHAR(12)
,@return INT OUTPUT

AS
DECLARE @instalduedate DATETIME
DECLARE @nextinstaldate DATETIME
DECLARE @cumint DECIMAL(15, 2)
DECLARE @int DECIMAL(15, 2)
DECLARE @count INT
DECLARE @counter INT

SET @return = 0
SET @counter = 1

CREATE TABLE #temp (
	InstalNum INT identity(1, 1)
	,InstalDate DATETIME
	,OpeningBalance DECIMAL(15, 2)
	,InstalAmt DECIMAL(15, 2)
	,Principal DECIMAL(15, 2)
	,Interest DECIMAL(15, 2)
	,ClosingBalance DECIMAL(15, 2)
	,LatePmtFee DECIMAL(15, 2)
	,PenaltyFee DECIMAL(15, 2)
	,AdminFee DECIMAL(15, 2)
	,TotalInstalment DECIMAL(15, 2)
	)

INSERT INTO #temp (
	InstalDate
	,OpeningBalance
	,InstalAmt
	,Principal
	,Interest
	,ClosingBalance
	,AdminFee
	,TotalInstalment
	)
SELECT instalduedate
	,openingbal
	,instalment
	,principal
	,servicechg
	,closingbal
	,AdminFee
	,instalment + AdminFee
FROM CLAmortizationSchedule
WHERE acctno = @acctno
ORDER BY instalduedate ASC

SELECT @count = count(*)
FROM #temp

WHILE (@counter <= @count)
BEGIN
	--if(@counter=1)
	--begin
	--	update #temp set LatePmtFee=0, PenaltyFee=0 where InstalNum=@counter
	--end
	--else
	--begin
	SELECT @instalduedate = InstalDate
	FROM #temp
	WHERE InstalNum = @counter

	SELECT @nextinstaldate = InstalDate
	FROM #temp
	WHERE InstalNum = @counter + 1

	SELECT isnull(sum(transvalue), 0) AS transvalue
	INTO #LatePmtFeeTable
	FROM fintrans
	WHERE acctno = @acctno
		AND transtypecode = 'INT'
		AND datetrans >= @instalduedate
		AND datetrans < @nextinstaldate
		AND ftnotes != 'SCRD'
		AND transvalue > 0

	SELECT isnull(sum(transvalue), 0) AS transvalue
	INTO #PenaltyFeeTable
	FROM fintrans
	WHERE acctno = @acctno
		AND transtypecode = 'FEE'
		AND datetrans >= @instalduedate
		AND datetrans < @nextinstaldate

	UPDATE #temp
	SET LatePmtFee = isnull(transvalue, 0)
	FROM #LatePmtFeeTable
	WHERE InstalNum = @counter

	UPDATE #temp
	SET PenaltyFee = isnull(transvalue, 0)
	FROM #PenaltyFeeTable
	WHERE InstalNum = @counter

	IF OBJECT_ID('tempdb..#LatePmtFeeTable') IS NOT NULL
	BEGIN
		DROP TABLE #LatePmtFeeTable
	END

	IF OBJECT_ID('tempdb..#PenaltyFeeTable') IS NOT NULL
	BEGIN
		DROP TABLE #PenaltyFeeTable
	END

	--end	 
	SET @counter = @counter + 1
END

SELECT InstalNum AS 'PaymentNum'
	,InstalDate AS 'PaymentDate'
	,OpeningBalance AS 'BeginningBalance'
	,InstalAmt AS 'InstalAmt'
	,Principal AS 'Principal'
	,Interest AS 'AmortizedInterest'
	,ClosingBalance AS 'EndingBalance'
	,LatePmtFee AS 'LatePmtFee'
	,PenaltyFee AS 'PenaltyFee'
	,AdminFee AS 'AdminFee'
	,TotalInstalment AS 'TotalInstalment'
FROM #temp

IF (@@error != 0)
BEGIN
	SET @return = @@error
END
                
