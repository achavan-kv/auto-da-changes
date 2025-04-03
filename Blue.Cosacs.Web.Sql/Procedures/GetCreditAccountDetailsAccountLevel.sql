
IF EXISTS (SELECT * FROM sysobjects 
   WHERE NAME = 'GetCreditAccountDetailsAccountLevel'
   )
BEGIN
DROP PROCEDURE [dbo].[GetCreditAccountDetailsAccountLevel]
END
GO

CREATE PROCEDURE [dbo].[GetCreditAccountDetailsAccountLevel]
	@CustId VARCHAR(20) = N'',
	@PageNumber int,
    @PageSize int,
	@Message varchar(MAX) output
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
		DECLARE @totalCount int,
				@totalPages decimal(10,2),
				@numberOfElements int,
				@totalNoOfElements int


		SELECT	ISNULL(A.acctno, '') AS AccountNumber
				,sum(DL.transvalue) as TotalAmount,
				--(select b.branchname from Branch b where A.branchno= b.branchno ) as StoreName

				Case when '817'=substring(A.acctno,1,3) then 
				(SELECT	Top 1 MS.Name
				FROM lineitem l INNER JOIN StockInfo s ON l.ItemID = s.ID
				Inner join  merchandising.product MP on MP.SKU=s.itemno
				inner join merchandising.supplier MS on Ms.Id=MP.PrimaryVendorid
				WHERE	acctno=A.acctno) 
				else 				
				(select b.branchname from Branch b where A.branchno= b.branchno ) End as StoreName

				,DATEADD(ss,-(DATEDIFF(ss,GETUTCDATE(),GETDATE())), ISNULL(A.dateacctopen, '1/1/1970')) as 'DateOfTransaction'
				,'COSACS' As 'type'
		FROM   acct A 
			INNER JOIN custacct CA ON A.acctno = CA.acctno 
			INNER JOIN customer cust ON CA.custid = cust.custid
			INNER JOIN accttype AT ON A.accttype = AT.genaccttype 
			INNER JOIN [delivery] DL ON dl.acctno = CA.acctno
		WHERE AT.accttype NOT IN ('C', 'S') 
			AND CA.custid = @custid
			AND CA.HldOrJnt = 'H'  
			AND currstatus <> 'S' 
		Group by  cust.Name , cust.firstname,A.acctno,a.acctno,A.branchno,A.dateacctopen, AT.accttype
		ORDER BY A.dateacctopen desc

		OFFSET @pageSize * (@PageNumber - 1) ROWS
		FETCH NEXT @pageSize ROWS ONLY OPTION (RECOMPILE);

		WITH Account_CTE (totalElements)  
		AS  
		(  
			SELECT 1
			FROM acct A 
					INNER JOIN custacct CA ON A.acctno = CA.acctno 
					INNER JOIN customer cust ON CA.custid = cust.custid
					INNER JOIN accttype AT ON A.accttype = AT.genaccttype 
					INNER JOIN [delivery] DL ON dl.acctno = CA.acctno
			WHERE AT.accttype NOT IN ('C', 'S') 
				AND CA.custid = @custid
				AND CA.HldOrJnt = 'H'  
				AND currstatus <> 'S' 
				Group by  cust.Name, cust.firstname,A.acctno,a.acctno,A.branchno,A.dateacctopen,AT.accttype		
		)  

		SELECT @totalNoOfElements = Count(totalElements) FROM Account_CTE;

		SELECT 
			CASE WHEN @PageNumber = CEILING(Cast(Count(*) As decimal)/Cast(@pageSize As decimal)) THEN 1 ELSE 0 END As lastPage,
			@totalNoOfElements As totalElements,
			CAST( CEILING(Cast(@totalNoOfElements As decimal)/Cast(@pageSize As decimal)) as int) As 'totalPages',
			@pageSize As 'pageSize', 
			@PageNumber As 'pageNumber', 
			'date' As 'sort', 
			CASE WHEN @PageNumber = 1 THEN 1 ELSE 0 END As firstPage,
			CASE WHEN @PageNumber >= CEILING(Cast(@totalNoOfElements As decimal)/Cast(@pageSize As decimal)) 
				THEN CASE 
						WHEN @PageNumber = 1 OR @PageNumber <= (@totalNoOfElements/@pageSize) 
						THEN @totalNoOfElements-((@PageNumber-1)*(@pageSize)) 
						ELSE 0 
					 END
				ELSE @pageSize END AS 'numberOfElements'
		
	IF(@totalNoOfElements > 0 AND (@PageNumber = 1 OR @PageNumber <= (@totalNoOfElements/@pageSize)))
		SET @Message = 'Customer account details found.'
	ELSE	
		SET @Message = 'Customer account details not found.'

	END
END

GO

