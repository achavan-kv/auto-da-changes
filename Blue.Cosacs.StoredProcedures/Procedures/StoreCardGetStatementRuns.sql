if exists (select * from dbo.sysobjects where id = object_id('StoreCardGetStatementRuns') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
DROP PROCEDURE StoreCardGetStatementRuns
GO

CREATE PROCEDURE StoreCardGetStatementRuns

AS


DECLARE @MaxAcctsPerBatch INT,
		@RunsTableCounter INT,
		@RunsTableRowCount INT,
		@AcctCount INT,
		@StartNum INT,
		@EndNum INT,
		@ResetCounter INT,
		@CurrentRunNo INT,
		@AcctDatePrinted DATETIME,
		@PrevAcctDatePrinted DATETIME

SELECT @MaxAcctsPerBatch = VALUE FROM dbo.CountryMaintenance WHERE CodeName = 'StoreCardBatchPrint'

CREATE TABLE #BatchStatements
(
	RunNo INT,
	RunDate DATETIME,
	DatePrinted DATETIME,
	BatchNo VARCHAR(100)
	
)

CREATE TABLE #Runs
(
	Id INT IDENTITY(1,1),
	RunNo INT,
	RunDate DATETIME,
	NoAccts INT
)

--Select top 5 runs into temporary table
INSERT INTO #Runs
SELECT TOP 5 InterfaceControl.RunNo, dbo.InterfaceControl.DateFinish,0
FROM dbo.InterfaceControl
WHERE Interface = 'STStatements'
AND result = 'P'
ORDER BY RunNo DESC

UPDATE #Runs
SET NoAccts = (SELECT COUNT(*)
				FROM dbo.StoreCardStatement s
				WHERE s.RunNo = #Runs.RunNo)

SET @RunsTableRowCount = (SELECT COUNT(*) FROM #Runs)


SET @RunsTableCounter = 1
SET @AcctCount = 1
SET @ResetCounter = 0
SET @CurrentRunNo = 0

DECLARE @NoAccts INT

--Loop through Runs table and insert records into #BatchStatements for each run
WHILE @RunsTableCounter <= @RunsTableRowCount
BEGIN
	SET @AcctCount = 1
	SET @NoAccts = (SELECT NoAccts FROM #Runs WHERE id = @RunsTableCounter) --Select number of accounts in this run
	SET @CurrentRunNo = (SELECT RunNo FROM #Runs WHERE id = @RunsTableCounter)
	
	WHILE @AcctCount <= @NoAccts
	BEGIN
		
		SET @AcctDatePrinted = (SELECT ManualDatePrinted FROM dbo.StoreCardStatement WHERE RunNo = @CurrentRunNo AND BatchNo = @AcctCount)
		
		--SELECT @AcctDatePrinted AS currentdateprinted, @PrevAcctDatePrinted AS previousdateprinted, @AcctCount AS AcctRow
		
		IF(ISNULL(@PrevAcctDatePrinted,'01-01-1900') != ISNULL(@AcctDatePrinted,'01-01-1900') AND @ResetCounter !=0)
		BEGIN
		
			SET @EndNum = @AcctCount - 1
			
			SET @ResetCounter = 0
				
			INSERT INTO #BatchStatements
			SELECT @CurrentRunNo, 
				   RunDate, 
				   CONVERT(datetime, SWITCHOFFSET(CONVERT(datetimeoffset, @PrevAcctDatePrinted), DATENAME(TzOffset, SYSDATETIMEOFFSET()))), 
				   CAST(@StartNum AS VARCHAR(10)) + ' - ' + CAST(@EndNum AS VARCHAR(10))
			FROM #Runs
			WHERE RunNo = @CurrentRunNo
	
		END
		
		IF(@ResetCounter = 0)
		BEGIN
			SET @StartNum = @AcctCount	
		END
		
		SET @ResetCounter += 1
		
		IF(@ResetCounter = @MaxAcctsPerBatch OR (@AcctCount = @NoAccts))
		BEGIN
			SET @EndNum = @AcctCount
			
			SET @ResetCounter = 0
			
			INSERT INTO #BatchStatements
			SELECT @CurrentRunNo, 
				   RunDate, 
				   CONVERT(datetime, SWITCHOFFSET(CONVERT(datetimeoffset, @AcctDatePrinted), DATENAME(TzOffset, SYSDATETIMEOFFSET()))) , 
				   CAST(@StartNum AS VARCHAR(10)) + ' - ' + CAST(@EndNum AS VARCHAR(10))
			FROM #Runs
			WHERE RunNo = @CurrentRunNo
			
		END
		
		SET @PrevAcctDatePrinted = @AcctDatePrinted
		SET @AcctCount +=1

	END
	
	SET @RunsTableCounter += 1

END

SELECT * FROM #BatchStatements
GO

