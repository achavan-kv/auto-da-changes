IF EXISTS (SELECT * FROM sysobjects 
   WHERE NAME = 'GetCustomerCreditSummaryDetails'
   )
BEGIN
DROP PROCEDURE [dbo].[GetCustomerCreditSummaryDetails]
END
GO

CREATE PROCEDURE [dbo].[GetCustomerCreditSummaryDetails]
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
		DECLARE @ListOfDueDate NVARCHAR(MAX) = '';
		DECLARE @ListOfDueDate1 NVARCHAR(MAX) = '';
		SET @ListOfDueDate = (SELECT (
										STUFF (
													(
														SELECT distinct ',' + CONVERT(NVARCHAR,DATEADD(ss,-(DATEDIFF(ss,GETUTCDATE(),GETDATE())), ISNULL(IP1.datefirst, '1/1/1970')),20) 
														FROM instalplan IP1
															INNER JOIN acct A ON A.acctno = IP1.acctno 
															INNER JOIN custacct CA ON CA.acctno = IP1.acctno 
															INNER JOIN accttype AT ON A.accttype = AT.genaccttype 
															INNER JOIN agreement AG ON A.acctno = AG.acctno 
														WHERE 
															AT.accttype NOT IN ('C', 'S') 
															AND CA.HldOrJnt = 'H'  
															AND currstatus <> 'S'
															AND CA.custid = @CustId
															AND AG.datenextdue >=  DATEADD(month, 0, CAST(CURRENT_TIMESTAMP AS DATE))
															And AG.datenextdue <=  DATEADD(s,-1,DATEADD(mm, DATEDIFF(m,0,DATEADD(month, 0, CAST(CURRENT_TIMESTAMP AS DATE)))+1,0))
															And (Convert(varchar,IP1.datefirst,103)<>'01/01/1900' and Convert(varchar,IP1.datelast,103)<>'01/01/1900') 
														FOR XML PATH ('')
													)
												,1,1,'')
										)
								)

		DECLARE @TempTable table
		(
			CustId NVARCHAR(20)
			,CustomerName NVARCHAR(90)
			,TotalCreditDue MONEY
			,DueDate NVARCHAR(MAX)
			,CreditLimit MONEY
			,CreditAvailable MONEY
			,MonthlyDue MONEY
			,AmountDueNow MONEY
		)

		INSERT @TempTable
		(
			CustId 
			,CustomerName 
			,TotalCreditDue 
			,DueDate 
			,CreditLimit 
			,CreditAvailable 
			,MonthlyDue 
			,AmountDueNow 
		)
		SELECT CA.custid As CustId,
			cust.Name +' '+ cust.firstname As CustomerName ,		
			sum(ISNULL(A.outstbal, 0.00)) AS 'TotalCreditDue',
			@ListOfDueDate as DueDate,
			(ISNULL(cust.RFCreditLimit, 0.00) )AS CreditLimit,
			(ISNULL(cust.AvailableSpend, 0.00) ) AS CreditAvailable,
			sum(IP.instalamount) as 'MonthlyDue',
			(
				select  isnull(sum(IP.instalamount),0)  + isnull(sum(transvalue),0) 
				from fintrans f 
					INNER JOIN custacct AC ON f.acctno = AC.acctno 
				where AC.custid = @custid and transtypecode in ('INT', 'FEE', 'ADM') 
			) as 'AmountDueNow'
		FROM   acct A 
			INNER JOIN custacct CA ON A.acctno = CA.acctno 
			INNER JOIN customer cust ON CA.custid = cust.custid
			INNER JOIN agreement AG ON A.acctno = AG.acctno 
			INNER JOIN accttype AT ON A.accttype = AT.genaccttype 
			INNER JOIN instalplan IP ON A.acctno = IP.acctno  
		WHERE AT.accttype NOT IN ('C', 'S') 
			AND CA.custid = @custid
			AND CA.HldOrJnt = 'H'  
			AND A.currstatus <> 'S'
		Group by cust.AvailableSpend,cust.RFCreditLimit,CA.custid,cust.Name +' '+ cust.firstname
		
		SELECT * FROM @TempTable

		IF EXISTS(SELECT 1 FROM @TempTable)				 
		BEGIN
			SET @Message = 'Customer credit details found.'
		END
		ELSE
			SET @Message = 'Customer credit details not found.'
			
	END
END


