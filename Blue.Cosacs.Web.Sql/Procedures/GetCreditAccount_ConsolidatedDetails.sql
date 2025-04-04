/*
--*********************************************************************** 
-- Script Name : GetCreditAccount_ConsolidatedDetails.sql 
-- Created For  : Unipay (T) 
-- Created By   : Zensar(Sagar Kute)
-- Created On   : 10/07/2018 
--***********************************************************************
-- Change Control 
-- -------------- 
-- Date(DD/MM/YYYY)		Changed By(FName LName)		Description 
-- ------------------------------------------------------------------------------------------------------- 
1. 11/03/2019			Zensar(Sagar Kute)			1. Corrected credit limit and credit available amount
													2. Added Settle Account details but excluded from monthly due.
													3. Changed logic for "TotalCreditDue" as per client request to (CreditLimit - CreditAvailable)
2. 
--*********************************************************************************************************

*/
IF EXISTS ( SELECT * FROM sysobjects WHERE NAME = 'GetCreditAccount_ConsolidatedDetails' )
BEGIN
	DROP PROCEDURE [dbo].[GetCreditAccount_ConsolidatedDetails]
END
GO

CREATE PROCEDURE [dbo].[GetCreditAccount_ConsolidatedDetails]
	@CustId VARCHAR(20),
	@Message VARCHAR(500) OUTPUT
AS
BEGIN
	SET @Message = ''

	IF NOT EXISTS (select 1 from Customer where Custid=@CustId)
	BEGIN
		SET @Message = 'User not found'
		RETURN		
	END
	ELSE IF NOT EXISTS (select 1 from custacct where custid=@CustId)
	BEGIN
		SET @Message = 'No accounts for user'
		RETURN		
	END
	ELSE IF EXISTS (SELECT 1 FROM customer cust INNER JOIN custacct ca ON ca.CustId = cust.CustId WHERE RFCreditLimit = 0.00 AND cust.CustId=@CustId)
	BEGIN
		SET @Message = 'No transactions for user'
		RETURN		
	END
	ELSE
	BEGIN
		
		DECLARE @TempTable table
		(
			[date] DATETIME
			,invoiceNumber CHAR(12)
			,amount MONEY
			,[status] CHAR(50)
			,totalRemainingAmount MONEY
			,[days] INT
		)

		INSERT INTO @TempTable
		(
			[date]
			,invoiceNumber
			,amount
			,[status]
			,totalRemainingAmount
			,[days]
		)
		SELECT DISTINCT
			CASE 
				WHEN A.arrears > 0 OR (DATEDIFF(MINUTE, GETUTCDATE(), datenextdue) >= 0 AND (DATEDIFF(MINUTE, GETUTCDATE(), IP1.datelast) >= 0 AND MONTH(GETUTCDATE()) <= MONTH(datenextdue) AND YEAR(GETUTCDATE()) <= YEAR(datenextdue)))
					THEN datenextdue 
				WHEN MONTH(GETUTCDATE()) = MONTH(IP1.datelast) AND YEAR(GETUTCDATE()) = YEAR(IP1.datelast) AND DATEDIFF(DAY, GETUTCDATE(), IP1.datelast) >= 0
					THEN IP1.datelast
				WHEN  IP1.dueday >= DATEPART(DAY,GETUTCDATE()) AND DATEDIFF(MINUTE, DATEADD(MONTH,1,GETUTCDATE()), [datefirst]) >= 0
					THEN CONVERT(VARCHAR,DATEPART(YYYY,datenextdue)) + '-' + CONVERT(VARCHAR,DATEPART(MM,GETUTCDATE())) + '-' + CONVERT(VARCHAR,CASE WHEN ISNULL(dueday,0) > 0 THEN dueday ELSE DATEPART(DD,datenextdue) END) + ' ' + CONVERT(VARCHAR,DATEPART(HOUR, datenextdue)) + ':' + CONVERT(VARCHAR,DATEPART(MINUTE, datenextdue)) + ':' + CONVERT(VARCHAR,DATEPART(SECOND, datenextdue)) + ':' + CONVERT(VARCHAR,DATEPART(MILLISECOND, datenextdue)) 
				WHEN  IP1.dueday < DATEPART(DAY,GETUTCDATE())
					THEN CONVERT(VARCHAR,DATEPART(YYYY,datenextdue)) + '-' + CONVERT(VARCHAR,DATEPART(MM,DATEADD(MONTH, ABS(DATEDIFF(MONTH, GETUTCDATE(), datenextdue)) + 1, datenextdue))) + '-' + CONVERT(VARCHAR,CASE WHEN ISNULL(dueday,0) > 0 THEN dueday ELSE DATEPART(DD,datenextdue) END) + ' ' + CONVERT(VARCHAR,DATEPART(HOUR, datenextdue)) + ':' + CONVERT(VARCHAR,DATEPART(MINUTE, datenextdue)) + ':' + CONVERT(VARCHAR,DATEPART(SECOND, datenextdue)) + ':' + CONVERT(VARCHAR,DATEPART(MILLISECOND, datenextdue)) 
				ELSE DATEADD(MONTH, ABS(DATEDIFF(MONTH, GETUTCDATE(), datenextdue)) + 1, datenextdue) 
			END AS 'date'  
			,A.acctno AS 'invoiceNumber' 
			,CASE 
				WHEN MONTH(GETUTCDATE()) = MONTH(IP1.datelast) AND YEAR(GETUTCDATE()) = YEAR(IP1.datelast) 
				THEN ISNULL(IP1.fininstalamt,0) + ISNULL(A.arrears,0)
				ELSE ISNULL(IP1.instalamount,0) + ISNULL(A.arrears,0)
			END AS 'amount'
			,CASE	
					--WHEN ((CAST(GETUTCDATE() AS DATE) = CAST(AG.datenextdue AS DATE)) AND FT.transtypecode = 'PAY') THEN 'UP_TO_DATE' 			
					WHEN A.arrears <= 0 OR (CAST(GETUTCDATE() AS DATE) <= CAST(AG.datenextdue AS DATE)) THEN 'EXPIRES_IN' --+ CASE WHEN DATEDIFF(day, CAST(GETUTCDATE() AS DATE), CAST(IP1.datefirst AS DATE)) > 0 THEN CONVERT(varchar(5), DATEDIFF(day, CAST(GETUTCDATE() AS DATE), CAST(IP1.datefirst AS DATE)))  ELSE CONVERT(VARCHAR(5), DATEDIFF(day, CAST(IP1.datefirst AS DATE), CAST(GETUTCDATE() AS DATE))) END
					WHEN A.arrears > 0 THEN 'EXPIRED' 
			END 'status'
			, A.outstbal AS 'totalRemainingAmount'
			, CASE
				WHEN A.arrears > 0 AND (CAST(GETUTCDATE() AS DATE) > CAST(datenextdue AS DATE)) 
					THEN DATEDIFF(DAY, datenextdue, GETUTCDATE())
				WHEN MONTH(GETUTCDATE()) = MONTH(IP1.datelast) AND YEAR(GETUTCDATE()) = YEAR(IP1.datelast) AND DAY(GETUTCDATE()) < DAY(IP1.datelast) 
					THEN DATEDIFF(DAY, GETUTCDATE(), IP1.datelast)
				WHEN CAST(GETUTCDATE() AS DATE) < (CASE 
														WHEN DATEDIFF(MINUTE, GETUTCDATE(), datenextdue) >= 0 
														THEN datenextdue 
														ELSE CASE 
																WHEN IP1.dueday >= DATEPART(DAY,GETUTCDATE())  AND DATEDIFF(MINUTE, DATEADD(MONTH,1,GETUTCDATE()), [datefirst]) >= 0
																	THEN CONVERT(VARCHAR,DATEPART(YYYY,datenextdue)) + '-' + CONVERT(VARCHAR,DATEPART(MM,GETUTCDATE())) + '-' + CONVERT(VARCHAR,CASE WHEN ISNULL(dueday,0) > 0 THEN dueday ELSE DATEPART(DD,datenextdue) END) + ' ' + CONVERT(VARCHAR,DATEPART(HOUR, datenextdue)) + ':' + CONVERT(VARCHAR,DATEPART(MINUTE, datenextdue)) + ':' + CONVERT(VARCHAR,DATEPART(SECOND, datenextdue)) + ':' + CONVERT(VARCHAR,DATEPART(MILLISECOND, datenextdue)) 
																WHEN  IP1.dueday < DATEPART(DAY,GETUTCDATE())
																	THEN CONVERT(VARCHAR,DATEPART(YYYY,datenextdue)) + '-' + CONVERT(VARCHAR,DATEPART(MM,DATEADD(MONTH, ABS(DATEDIFF(MONTH, GETUTCDATE(), datenextdue)) + 1, datenextdue))) + '-' + CONVERT(VARCHAR,CASE WHEN ISNULL(dueday,0) > 0 THEN dueday ELSE DATEPART(DD,datenextdue) END) + ' ' + CONVERT(VARCHAR,DATEPART(HOUR, datenextdue)) + ':' + CONVERT(VARCHAR,DATEPART(MINUTE, datenextdue)) + ':' + CONVERT(VARCHAR,DATEPART(SECOND, datenextdue)) + ':' + CONVERT(VARCHAR,DATEPART(MILLISECOND, datenextdue)) 
																ELSE DATEADD(MONTH, ABS(DATEDIFF(MONTH, GETUTCDATE(), datenextdue) + 1),datenextdue) 
															END 
														END)
					THEN CONVERT(INT, DATEDIFF(day, CAST(GETUTCDATE() AS DATE), (CASE 
																					WHEN DATEDIFF(MINUTE, GETUTCDATE(), datenextdue) >= 0 
																						THEN datenextdue 
																					ELSE CASE 
																							WHEN IP1.dueday >= DATEPART(DAY,GETUTCDATE())  AND DATEDIFF(MINUTE, DATEADD(MONTH,1,GETUTCDATE()), [datefirst]) >= 0
																								THEN CONVERT(VARCHAR,DATEPART(YYYY,datenextdue)) + '-' + CONVERT(VARCHAR,DATEPART(MM,GETUTCDATE())) + '-' + CONVERT(VARCHAR,CASE WHEN ISNULL(dueday,0) > 0 THEN dueday ELSE DATEPART(DD,datenextdue) END) + ' ' + CONVERT(VARCHAR,DATEPART(HOUR, datenextdue)) + ':' + CONVERT(VARCHAR,DATEPART(MINUTE, datenextdue)) + ':' + CONVERT(VARCHAR,DATEPART(SECOND, datenextdue)) + ':' + CONVERT(VARCHAR,DATEPART(MILLISECOND, datenextdue)) 
																							WHEN  IP1.dueday < DATEPART(DAY,GETUTCDATE())
																								THEN CONVERT(VARCHAR,DATEPART(YYYY,datenextdue)) + '-' + CONVERT(VARCHAR,DATEPART(MM,DATEADD(MONTH, ABS(DATEDIFF(MONTH, GETUTCDATE(), datenextdue)) + 1, datenextdue))) + '-' + CONVERT(VARCHAR,CASE WHEN ISNULL(dueday,0) > 0 THEN dueday ELSE DATEPART(DD,datenextdue) END) + ' ' + CONVERT(VARCHAR,DATEPART(HOUR, datenextdue)) + ':' + CONVERT(VARCHAR,DATEPART(MINUTE, datenextdue)) + ':' + CONVERT(VARCHAR,DATEPART(SECOND, datenextdue)) + ':' + CONVERT(VARCHAR,DATEPART(MILLISECOND, datenextdue)) 
																							ELSE DATEADD(MONTH, ABS(DATEDIFF(MONTH, GETUTCDATE(), datenextdue) + 1),datenextdue) 
																						END
																					END)
												)
								)  
				ELSE CONVERT(INT, DATEDIFF(day, (CASE 
													WHEN DATEDIFF(MINUTE, GETUTCDATE(), datenextdue) >= 0  
														THEN datenextdue  
													ELSE CASE 
															WHEN IP1.dueday >= DATEPART(DAY,GETUTCDATE())  AND DATEDIFF(MINUTE, DATEADD(MONTH,1,GETUTCDATE()), [datefirst]) >= 0
																THEN CONVERT(VARCHAR,DATEPART(YYYY,datenextdue)) + '-' + CONVERT(VARCHAR,DATEPART(MM,GETUTCDATE())) + '-' + CONVERT(VARCHAR,CASE WHEN ISNULL(dueday,0) > 0 THEN dueday ELSE DATEPART(DD,datenextdue) END) + ' ' + CONVERT(VARCHAR,DATEPART(HOUR, datenextdue)) + ':' + CONVERT(VARCHAR,DATEPART(MINUTE, datenextdue)) + ':' + CONVERT(VARCHAR,DATEPART(SECOND, datenextdue)) + ':' + CONVERT(VARCHAR,DATEPART(MILLISECOND, datenextdue)) 
															WHEN  IP1.dueday < DATEPART(DAY,GETUTCDATE())
																THEN CONVERT(VARCHAR,DATEPART(YYYY,datenextdue)) + '-' + CONVERT(VARCHAR,DATEPART(MM,DATEADD(MONTH, ABS(DATEDIFF(MONTH, GETUTCDATE(), datenextdue)) + 1, datenextdue))) + '-' + CONVERT(VARCHAR,CASE WHEN ISNULL(dueday,0) > 0 THEN dueday ELSE DATEPART(DD,datenextdue) END) + ' ' + CONVERT(VARCHAR,DATEPART(HOUR, datenextdue)) + ':' + CONVERT(VARCHAR,DATEPART(MINUTE, datenextdue)) + ':' + CONVERT(VARCHAR,DATEPART(SECOND, datenextdue)) + ':' + CONVERT(VARCHAR,DATEPART(MILLISECOND, datenextdue)) 
															ELSE DATEADD(MONTH, ABS(DATEDIFF(MONTH, GETUTCDATE(), datenextdue) + 1),datenextdue) 
														END
													END)
											, CAST(GETUTCDATE() AS DATE)
										)
							) 
			END AS 'days'
		FROM instalplan IP1
			INNER JOIN acct A ON A.acctno = IP1.acctno 
			INNER JOIN custacct CA ON CA.acctno = IP1.acctno 
			INNER JOIN accttype AT ON A.accttype = AT.genaccttype 
			INNER JOIN agreement AG ON A.acctno = AG.acctno 
			INNER JOIN delivery DL ON DL.acctno = A.acctno 
			INNER JOIN fintrans FT ON FT.acctno = A.acctno 
		WHERE 
			AT.accttype NOT IN ('C', 'S','SRC') 
			AND CA.HldOrJnt = 'H'  
			AND currstatus <> 'S'
			AND A.outstbal > 0
			AND CA.custid = @CustId
			AND CONVERT(DATE, IP1.datelast) >= CONVERT(DATE, GETUTCDATE())
		ORDER BY (
					CASE 
						WHEN A.arrears > 0 OR (DATEDIFF(MINUTE, GETUTCDATE(), datenextdue) >= 0 AND (DATEDIFF(MINUTE, GETUTCDATE(), IP1.datelast) >= 0 AND MONTH(GETUTCDATE()) <= MONTH(datenextdue) AND YEAR(GETUTCDATE()) <= YEAR(datenextdue)))
							THEN datenextdue 
						WHEN MONTH(GETUTCDATE()) = MONTH(IP1.datelast) AND YEAR(GETUTCDATE()) = YEAR(IP1.datelast) AND DATEDIFF(DAY, GETUTCDATE(), IP1.datelast) >= 0
							THEN IP1.datelast
						WHEN  IP1.dueday >= DATEPART(DAY,GETUTCDATE()) AND DATEDIFF(MINUTE, DATEADD(MONTH,1,GETUTCDATE()), [datefirst]) >= 0
							THEN CONVERT(VARCHAR,DATEPART(YYYY,datenextdue)) + '-' + CONVERT(VARCHAR,DATEPART(MM,GETUTCDATE())) + '-' + CONVERT(VARCHAR,CASE WHEN ISNULL(dueday,0) > 0 THEN dueday ELSE DATEPART(DD,datenextdue) END) + ' ' + CONVERT(VARCHAR,DATEPART(HOUR, datenextdue)) + ':' + CONVERT(VARCHAR,DATEPART(MINUTE, datenextdue)) + ':' + CONVERT(VARCHAR,DATEPART(SECOND, datenextdue)) + ':' + CONVERT(VARCHAR,DATEPART(MILLISECOND, datenextdue)) 
						WHEN  IP1.dueday < DATEPART(DAY,GETUTCDATE())
							THEN CONVERT(VARCHAR,DATEPART(YYYY,datenextdue)) + '-' + CONVERT(VARCHAR,DATEPART(MM,DATEADD(MONTH, ABS(DATEDIFF(MONTH, GETUTCDATE(), datenextdue)) + 1, datenextdue))) + '-' + CONVERT(VARCHAR,CASE WHEN ISNULL(dueday,0) > 0 THEN dueday ELSE DATEPART(DD,datenextdue) END) + ' ' + CONVERT(VARCHAR,DATEPART(HOUR, datenextdue)) + ':' + CONVERT(VARCHAR,DATEPART(MINUTE, datenextdue)) + ':' + CONVERT(VARCHAR,DATEPART(SECOND, datenextdue)) + ':' + CONVERT(VARCHAR,DATEPART(MILLISECOND, datenextdue)) 
						ELSE DATEADD(MONTH, ABS(DATEDIFF(MONTH, GETUTCDATE(), datenextdue)) + 1, datenextdue) 
					END
				) ASC

		SELECT DISTINCT CA.custid As CustId,
			(ISNULL(cust.RFCreditLimit, 0.00) ) - (ISNULL(CAST(ROUND(cust.AvailableSpend, 2) AS NUMERIC(10,2)) , 0.00)) AS 'TotalCreditDue'
			--,ISNULL(@ListOfDueDate,'') as DueDate
			,(ISNULL(cust.RFCreditLimit, 0.00) ) AS CreditLimit
			,(ISNULL(CAST(ROUND(cust.AvailableSpend, 2) AS NUMERIC(10,2)) , 0.00)) AS CreditAvailable
			,'' as 'monthlyPaymentCapacity'
			,( 
				SELECT  sum(
							CASE 
								WHEN MONTH(GETUTCDATE()) = MONTH(IP.datelast) AND YEAR(GETUTCDATE()) = YEAR(IP.datelast) 
								THEN ISNULL(IP.fininstalamt,0) + ISNULL(A2.arrears,0)
								ELSE ISNULL(IP.instalamount,0) + ISNULL(A2.arrears,0)
							END
						)
				FROM instalplan IP 
						INNER JOIN acct A2 ON A2.acctno = IP.acctno 
						INNER JOIN custacct CA2 ON A2.acctno = CA2.acctno 
				WHERE A2.currstatus <> 'S' AND CA2.custid = @custid AND IP.datelast >=  DATEADD(month, 0, CAST(CURRENT_TIMESTAMP AS DATE))
			)as 'MonthlyDue'
			, (
				select  sum(
							CASE 
								WHEN MONTH(GETUTCDATE()) = MONTH(IP.datelast) AND YEAR(GETUTCDATE()) = YEAR(IP.datelast) 
								THEN ISNULL(IP.fininstalamt,0) + ISNULL(A.arrears,0)
								ELSE ISNULL(IP.instalamount,0) + ISNULL(A.arrears,0)
							END
							) 
				from instalplan IP
					INNER JOIN acct A ON A.acctno = IP.acctno 
					INNER JOIN custacct AC ON IP.acctno = AC.acctno
					INNER JOIN agreement AG ON AC.acctno = AG.acctno 
				where AC.custid = @custid
					AND CAST(CURRENT_TIMESTAMP AS DATE) BETWEEN CAST(A.dateacctopen AS DATE) AND  CAST(IP.datelast AS DATE) 	
			) as 'AmountDueNow'
		
		FROM   acct A 
			INNER JOIN custacct CA ON A.acctno = CA.acctno 
			INNER JOIN customer cust ON CA.custid = cust.custid
			INNER JOIN agreement AG ON A.acctno = AG.acctno 
			INNER JOIN accttype AT ON A.accttype = AT.genaccttype 
			--INNER JOIN instalplan IP ON A.acctno = IP.acctno 
		WHERE AT.accttype NOT IN ('C', 'S','SRC') 
			AND CA.custid = @custid
			AND CA.HldOrJnt = 'H'  
		Group by cust.AvailableSpend,cust.RFCreditLimit,CA.custid, A.acctno	
		
		SELECT  TOP 3
			ISNULL(DATEADD(ss, -(DATEDIFF(ss,GETUTCDATE(),GETDATE())),[date]), '') AS 'date'
			,invoiceNumber
			,amount
			,[status]
			,totalRemainingAmount
			,[days] 
		FROM @TempTable

			 
		SET @Message = 'Customer credit details found.'
	END
END

