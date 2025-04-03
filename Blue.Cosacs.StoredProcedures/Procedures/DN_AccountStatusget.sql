

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[DN_AccountStatusGet]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DN_AccountStatusGet]
GO


--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_AccountStatusGet.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Returns application status and other account details to the Account Status screen.
-- Author       : Ilyas Parker
-- Date         : 16th May 2008
--
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 16/05/08  IP  Stored Proc creation
--------------------------------------------------------------------------------


CREATE PROCEDURE [dbo].[DN_AccountStatusGet]	
		@datefrom datetime,
		@dateto datetime,
		--@cancelledaccounts bit,
		@branchno int,
		@return int OUT


AS

DECLARE @status int

SET		@return = 0
SET		@status = 0

--Create the table to store the accounts returned.
CREATE TABLE #AccountStatus
(
	Acctno varchar(12),
	DateAcctOpen datetime,
	Custid varchar(20),
	Empeeno int,
	EmpeeName varchar(101),
	Status1 varchar(128),
	Status2 varchar(128),
	Status3 varchar(128)
)

CREATE TABLE #StatusA
(
	Acctno varchar(12),
	Status varchar(128)
)

--Create a temporary table that will store the status returned back from the 
--'DN_AccountApplicationStatusGet_AccountStatusScreen.sql' stored procedure
CREATE TABLE #Status
(
	StatusID int IDENTITY (1, 1) NOT NULL,
	Acctno varchar(12),
	Status varchar(128)
)



--Select the data into the temporary table.
BEGIN
	IF @Status = 0
	BEGIN
		INSERT INTO #AccountStatus
		(
			Acctno,
			DateAcctOpen, 
			Custid, 
			Empeeno, 
			EmpeeName,
			Status1,
			Status2,
			Status3
		)
		SELECT a.acctno, 
			a.dateacctopen, 
			ca.custid, 
			ag.empeenosale,
			u.fullname as EmployeeName,
			'',
			'',
			''
		FROM  acct a 
		INNER JOIN custacct ca	ON	  a.acctno = ca.acctno 
		INNER JOIN agreement ag ON	  a.acctno = ag.acctno 
		INNER JOIN Admin.[User] u ON	  ag.empeenosale = u.id
		WHERE a.dateacctopen between @datefrom AND @dateto
		AND	  ca.hldorjnt = 'H'
--		AND	  ((a.currstatus = 'S' and @cancelledaccounts=0 --include cancelled accounts
--				AND EXISTS(SELECT * FROM cancellation c WHERE C.acctno = a.acctno) 
--				OR (a.currstatus != 'S' ) )	--include non-settled
--			OR	  (a.currstatus != 'S' and @cancelledaccounts=1)  ) --excludes settled (and cancelled)

		AND   ((a.currstatus = 's' AND EXISTS(SELECT * FROM cancellation c WHERE C.acctno = a.acctno))
				OR (a.currstatus != 's'))

		AND	  substring(a.acctno, 0, 4) like CAST(@branchno as varchar(3))
		AND	  a.accttype in ('C', 'O', 'R')
		
		SET @status = @@error
	END

	IF @Status = 0
	BEGIN

		DECLARE @acctno varchar(12)
		DECLARE acctstatus_cursor CURSOR FOR		
				SELECT acctno
					FROM #AccountStatus
		
		OPEN acctstatus_cursor 

		FETCH NEXT FROM acctstatus_cursor
			INTO @acctno

		WHILE @@FETCH_STATUS = 0
		BEGIN
			TRUNCATE TABLE #StatusA
			TRUNCATE TABLE #Status
			EXECUTE DN_AccountApplicationStatusGet_AccountStatusScreen @acctno, 0
			
			--Set the identity insert to ON
			--SET IDENTITY_INSERT #Status ON

			INSERT INTO #Status 
			SELECT * FROM #StatusA
			
			UPDATE #AccountStatus
				SET Status1 = (SELECT Status FROM #Status s INNER JOIN #AccountStatus a
								ON a.acctno = s.acctno 
								AND s.StatusID = 1),
				    Status2 = (SELECT Status FROM #Status s INNER JOIN #AccountStatus a
								ON a.acctno = s.acctno 
								AND s.StatusID = 2),
				    Status3 = (SELECT Status FROM #Status s INNER JOIN #AccountStatus a
								ON a.acctno = s.acctno 
								AND s.StatusID = 3)
			FROM #Status s1, #AccountStatus a1
			WHERE s1.acctno = a1.acctno

			--SET IDENTITY_INSERT #Status OFF
		
			FETCH NEXT FROM acctstatus_cursor
			INTO @acctno
	
		END
		CLOSE acctstatus_cursor
		DEALLOCATE acctstatus_cursor
	END
	
	SELECT Acctno as AccountNumber,
		   convert(varchar, DateAcctOpen, 111) as DateOpened,
		   Custid as CustomerID,
		   Empeeno as Employee,
		   EmpeeName as EmployeeName,
		   Status1 as Status1,
		   Status2 as Status2,
		   Status3 as Status3
	FROM   #AccountStatus

END
