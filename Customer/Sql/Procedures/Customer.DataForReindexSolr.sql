IF OBJECT_ID('Customer.DataForReindexSolr') IS NOT NULL
	DROP PROCEDURE Customer.DataForReindexSolr
GO

CREATE PROCEDURE Customer.DataForReindexSolr 
	@CustomerId VarChar(12) =  NULL,
	@MaxId Int = 0
AS

	DECLARE 
		@TotalRows Int,
		@Max Int = 10000000,
		@MaxCustomerId Int,
		@MinId Int,
		@Today date = getdate(),
		@ActiveCashCustomerDays int = ISNULL((SELECT ValueInt FROM Config.Setting WHERE Namespace = 'Blue.Cosacs.SalesManagement' AND Id = 'HowManyDaysToBeAnInactiveCashCustomer'), 180),
		@ActiveCreditCustomerDays int = ISNULL((SELECT ValueInt FROM Config.Setting WHERE Namespace = 'Blue.Cosacs.SalesManagement' AND Id = 'HowManyDaysToBeAnInactiveCreditCustomer'), 180)

		SELECT @TotalRows = COUNT(ID), @MaxCustomerId = MAX(Id) FROM customer
	
	--the max id have to be ignored if we filter by customerid
	--but if the @MaxId is zero lets set it to the default value 10000
	IF @CustomerId IS NOT NULL
		SET @MaxId = @MaxCustomerId
	ELSE
		IF @MaxId = 0 
			SET @MaxId = @MaxCustomerId

	SET @MinId = CASE WHEN @CustomerId IS NULL THEN @MaxId - @Max ELSE 0 END
	
	SELECT  
		@TotalRows AS TotalRows,
		Cust.ID,
		cust.custid AS CustomerId, 
		cust.title as Title, 
		cust.firstname as FirstName, 
		cust.name AS LastName, 
		cust.alias as Alias, 
		cust.dateborn AS DOB,
		cth.telno AS HomePhoneNumber, 
		ctm.telno AS MobileNumber, 
		NULLIF(LTRIM(RTRIM(Cadd.Email)), '') AS Email,
		cadd.cusaddr1 AS HomeAddressLine1, 
		cadd.cusaddr2 AS HomeAddressLine2, 
		cadd.cusaddr3 AS City, 
		cadd.cuspocode AS PostCode,
		Convert(Bit,
					CASE WHEN rcreditacct.accttype IS NOT NULL
			THEN 1
			ELSE 0
		END ) as HasRCreditSource,
		rcreditacct.AccountOpenDate as RCreditSourceDate,
		Convert(Bit,
					CASE WHEN ocreditacct.accttype IS NOT NULL
			THEN 1
			ELSE 0
		END ) as HasOCreditSource,
		ocreditacct.AccountOpenDate as OCreditSourceDate,
		Convert(Bit,
					CASE WHEN cashacct.accttype IS NOT NULL
			THEN 1
			ELSE 0
		END ) as HasCashSource,
		cashacct.AccountOpenDate as CashSourceDate,
		Convert(Bit,
					CASE WHEN storecard.accttype IS NOT NULL
			THEN 1
			ELSE 0
		END ) as HasStoreCardSource,			
		storecard.AccountOpenDate as StoreCardSourceDate,
		Convert(Bit,
					CASE WHEN WarrantyType.CustomerId IS NOT NULL
			THEN 1
			ELSE 0
		END ) as HasWarrantySource,
		CASE 
			WHEN WarrantyType.CustomerId IS NOT NULL THEN WarrantyType.CreatedOn
			ELSE NULL
		END as WarrantySourceDate,
		Convert(Bit,
					CASE WHEN InstallationType.CustomerId IS NOT NULL
			THEN 1
			ELSE 0
		END ) as HasInstallationSource,
		CASE 
			WHEN InstallationType.CustomerId IS NOT NULL THEN InstallationType.CreatedOn
			ELSE NULL
		END as InstallationSourceDate,
		CONVERT(INT, CEILING(cust.AvailableSpend)) AS AvailableSpend,
		csp.CustomerBranch AS CustomerBranchNo,
		branch.branchname AS CustomerBranchName,
		csp.SalesPersonId as SalesPersonId, 
		usr.FullName AS SalesPerson, 
		LastBought.DateLastBought, 
		(CASE
			WHEN LastBought.DateLastBought is NULL THEN 'Never Bought anything'
			WHEN DATEDIFF(day, @Today, LastBought.DateLastBought) BETWEEN 0 AND 15 THEN 'Bought Within 15 Days'
			WHEN DATEDIFF(day, @Today, LastBought.DateLastBought) BETWEEN 15 AND 30 THEN 'Bought Within 30 Days'
			WHEN DATEDIFF(day, @Today, LastBought.DateLastBought) >=30 AND DATEDIFF(MONTH, @Today, LastBought.DateLastBought) <= 3 THEN 'Bought Within 3 Months'
			WHEN DATEDIFF(MONTH, @Today, LastBought.DateLastBought) BETWEEN 3 AND 6 THEN 'Bought Within 6 Months'
		ELSE 'Bought Over 6 Months'
		END) as DateLastBoughtRange,
		'Cosacs' AS CustomerSource,
		csp.DoNotCallAgain as DoNotCallAgain,
		Call.CalledAt as CalledAt,
		CONVERT(Bit, CASE
			WHEN ISNULL(PendingCalls.TotalRowCount, 0) = 0 THEN 0
			ELSE 1
		END) AS HasPendingCalls,
		CONVERT(Bit,
				CASE 
					WHEN (rcreditacct.accttype IS NULL OR ocreditacct.accttype IS NULL) 
						AND cashacct.accttype IS NOT NULL                     /*have no payments so lets just put a very old date*/
						AND DATEDIFF(day, @Today, ISNULL(cashacct.DateLastPaid, '19000101')) <= @ActiveCashCustomerDays 
					THEN 1
					ELSE 0
					END 
				) as IsActiveCashCustomer,
		CONVERT(Bit,
				CASE 
					WHEN (rcreditacct.accttype IS NOT NULL OR ocreditacct.accttype IS NOT NULL) 
						AND DATEDIFF(day, @Today, ISNULL(cashacct.DateLastPaid, '19000101')) <= @ActiveCreditCustomerDays 
					THEN 1
					ELSE 0
					END 
				) as IsActiveCreditCustomer,
		(SELECT STUFF
            ((
				SELECT DISTINCT ', ' + t.Name
				FROM 
					SalesManagement.Call call 
					INNER JOIN SalesManagement.CallType t ON call.CallTypeId = t.Id
                WHERE  call.CustomerId = cust.custid
                FOR XML PATH('')),1, 2, '') AS CSVColumn) as CustomerCallType,
		(CASE
			WHEN DATEPART(WEEK, @Today) - DATEPART(WEEK, cust.dateborn) = 0  THEN 'This Week'
			WHEN DATEPART(MONTH, @Today) - DATEPART(MONTH, cust.dateborn) = 0 THEN 'This Month'
			WHEN  DATEPART(MONTH, cust.dateborn) -1 = DATEPART(MONTH, @Today) THEN 'Next Month'
			ELSE 'Other'
		END) as HasBirthday, 
		CONVERT(Bit, CASE WHEN bml.Id IS NULL THEN 1 else 0 END) AS ReceiveEmails,
		ISNULL(cust.ResieveSms, 1) AS ResieveSms,
		CI.LastEmailSentOn,
		CI.LastSmsSentOn
	FROM            
		customer AS cust 
		LEFT JOIN
		(
			SELECT custid, NULLIF(ISNULL(NULLIF(LTRIM(RTRIM(CONVERT(VarChar, DialCode))), '') + '-', '') + LTRIM(RTRIM(telno)), '') AS telno
			FROM custtel
			WHERE tellocn = 'H' AND datediscon IS NULL
		) AS cth 
			ON cust.custid = cth.custid 
		LEFT JOIN
		(
			SELECT custid, NULLIF(LTRIM(RTRIM(telno)), '') AS telno
			FROM custtel
			WHERE tellocn = 'M' AND datediscon IS NULL 
		) AS ctm 
			ON cust.custid = ctm.custid 
		LEFT JOIN dbo.custaddress AS cadd 
			ON cust.custid = cadd.custid
			AND cadd.addtype = 'H'
			AND cadd.datemoved IS NULL
		LEFT JOIN Communication.BlackEmailList bml
			ON bml.Email =  LTRIM(RTRIM(Cadd.Email))
		LEFT JOIN 
		(
			SELECT custacct.custid,	acct.accttype, MAX(acct.dateacctopen) as AccountOpenDate, MAX(acct.datelastpaid) as DateLastPaid
			from custacct
				inner join acct
					on acct.acctno = custacct.acctno
					and acct.accttype in ('C')
					and custacct.hldorjnt = 'H'
			group by custacct.custid,acct.accttype
		) as cashacct
			on cashacct.custid = cust.custid
		LEFT JOIN 
		(
			select custacct.custid,	acct.accttype, max(acct.dateacctopen)  as AccountOpenDate
			from custacct
				inner join acct
					on acct.acctno = custacct.acctno
					and acct.accttype in ('R')
					and custacct.hldorjnt = 'H'
			group by custacct.custid,acct.accttype
		) as rcreditacct
			on rcreditacct.custid = cust.custid
		LEFT JOIN 
		(
			select custacct.custid,	acct.accttype, max(acct.dateacctopen)  as AccountOpenDate
			from 
				custacct
				inner join acct
					on acct.acctno = custacct.acctno
					and acct.accttype in ('O')
					and custacct.hldorjnt = 'H'
			group by custacct.custid,acct.accttype
		) as ocreditacct
			on ocreditacct.custid = cust.custid
		LEFT JOIN 
		(
			select custacct.custid, acct.accttype, max(acct.dateacctopen)  as AccountOpenDate
			from 
				custacct
				inner join acct
					on acct.acctno = custacct.acctno
					and acct.accttype in ('T')
					and custacct.hldorjnt = 'H'
			group by custacct.custid,acct.accttype
		) as storecard
			on storecard.custid = cust.custid
		LEFT JOIN 
		(
			SELECT 
				CustomerId,MAX(CreatedOn) AS CreatedOn
			FROM
				[Sales].[OrderCustomer] oc
			INNER JOIN  [Sales].[Order] o
				ON oc.OrderId = o.Id
				AND CustomerId IS NOT NULL				
			WHERE
				EXISTS
				(
					SELECT 1
					FROM Sales.OrderItem 
					WHERE OrderId = o.Id AND ItemTypeId = 3
				)
			GROUP BY CustomerId
		) AS InstallationType
		ON cust.custid = InstallationType.CustomerId
		LEFT JOIN
		(
			SELECT
				CustomerId,MAX(CreatedOn) AS CreatedOn
			FROM
				[Sales].[OrderCustomer] oc
			INNER JOIN  [Sales].[Order] o
				ON oc.OrderId = o.Id
				AND CustomerId IS NOT NULL
			WHERE
				EXISTS
				(
					SELECT 1
					FROM Sales.OrderItem 
					WHERE OrderId = o.Id AND ItemTypeId = 2
				)
			GROUP BY CustomerId
		) AS WarrantyType
		ON cust.custid = WarrantyType.CustomerId
		LEFT JOIN SalesManagement.CustomerSalesPerson csp
			ON cust.custid = csp.CustomerId
		OUTER APPLY 
		( 
			SELECT MAX(Call.CalledAt) as CalledAt From SalesManagement.Call Call WHERE cust.custid = Call.CustomerId
		) Call
		LEFT JOIN admin.[User] usr
			ON csp.SalesPersonId = usr.Id
		LEFT JOIN branch
			ON csp.CustomerBranch = branch.branchno
		LEFT JOIN 
		(
			SELECT 
				MAX(datechange) As DateLastBought,
				c.custid
			FROM
				LineitemAudit l
				INNER JOIN custacct c
					ON l.acctno = c.acctno
			WHERE 
				l.ValueAfter > l.ValueBefore
				and l.itemno != 'DT'
			GROUP BY 
				c.custid
		) LastBought
			ON cust.custid = LastBought.custid
		OUTER APPLY 
		( 
			SELECT COUNT(Call.Id) as TotalRowCount From SalesManagement.Call Call WHERE cust.custid = Call.CustomerId AND call.CallClosedReasonId IS NULL
		) PendingCalls
		LEFT JOIN  Communication.CustomerInteraction CI
			ON cust.custid = ci.CustomerId
	WHERE
		cust.custid = CASE 
						WHEN @CustomerId IS NULL THEN cust.custid
						ELSE @CustomerId 
						END
		AND cust.Id > @MinId
		AND cust.Id <= @MaxId
			Order by LastBought.DateLastBought desc
	option (OPTIMIZE FOR (@MinId = 0, @MaxId = 10000000))