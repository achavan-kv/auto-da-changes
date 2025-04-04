if exists (select * from dbo.sysobjects where id = object_id('[dbo].[VE_CustomerSearch]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[VE_CustomerSearch]
END
GO


Create PROCEDURE [dbo].[VE_CustomerSearch]
	@CustId varchar(20) = NULL,
	@FirstName varchar(30) = NULL,
	@LastName varchar(60) = NULL,
	@PhoneNumber VARCHAR(20) = NULL,
	@PostalCode VARCHAR(10) = NULL
AS
BEGIN
	IF(@CustId = '')
		SET @CustId = NULL
	IF(@FirstName = '')
		SET @FirstName = NULL
	IF(@LastName = '')
		SET @LastName = NULL
	IF(@PhoneNumber = '')
		SET @PhoneNumber = NULL
	IF(@PostalCode = '')
		SET @PostalCode = NULL

	IF(@PhoneNumber IS NOT NULL)
		SET @PhoneNumber = REPLACE(@PhoneNumber,'-','')

	DECLARE @CustomerLinkTemp TABLE
	(
		CustomerId VARCHAR(20)
		,IdType VARCHAR(4)
		,IdNumber VARCHAR(30)
		,Title VARCHAR(25)
		,FirstName VARCHAR(30)
		,LastName VARCHAR(60)
		,DOB VARCHAR(10)
		,BranchNo SMALLINT
	)
	INSERT INTO @CustomerLinkTemp
	(
		CustomerId
		,IdType
		,IdNumber
		,Title
		,FirstName
		,LastName
		,DOB
		,BranchNo
	)
	SELECT DISTINCT 
			C.custid AS CustomerId
			, C.IdType AS IdType
			, C.IdNumber AS IdNumber
			, C.title AS Title
			, C.firstname AS FirstName
			, C.name AS LastName
			, CONVERT(VARCHAR(10), C.dateborn, 101) AS DOB
			, C.branchnohdle AS BranchNo
	FROM   	customer C
			LEFT OUTER JOIN custaddress ad ON C.custid = ad.custid 
			LEFT OUTER JOIN custtel t ON C.custid = t.custid  AND t.datediscon IS NULL	 
	WHERE   (C.custid = @CustId OR @CustId IS NULL) 
			AND 
			(C.firstname LIKE @FirstName + '%' OR @FirstName IS NULL)
			AND
			(C.name LIKE @LastName + '%' OR @LastName IS NULL)
			AND
			(replace(telno,'-','') LIKE @PhoneNumber + '%' OR @PhoneNumber IS NULL)
			AND
			(cuspocode LIKE @PostalCode + '%' OR @PostalCode IS NULL)
	
	SELECT * FROM @CustomerLinkTemp
	SELECT 
			ca.custid AS CustomerId
			, addtype AS AddressType
			, cusaddr1 AS Address1
			, cusaddr2 AS Address2
			, cusaddr3 AS Address3
			, cuspocode as PostalCode
			, deliveryarea as DeliveryArea
			, Email AS Email
			, CONVERT(VARCHAR(10), datein, 101) AS DateIn
			, Notes AS Notes 
	FROM	custaddress ca 
				INNER JOIN @CustomerLinkTemp clt ON clt.CustomerId = ca.custid
	SELECT 
			ct.custid AS CustomerId
			, CASE WHEN telno = '' THEN NULL ELSE 
			
			LTRIM(RTRIM(DialCode)) + LTRIM(RTRIM(replace(telno,'-',''))) END AS ContactNumber
			, tellocn AS ContactLocation
			, CASE WHEN extnno = '' THEN NULL ELSE extnno END AS Ext
	FROM	custtel ct 
				INNER JOIN @CustomerLinkTemp clt ON clt.CustomerId = ct.custid
	WHERE ct.datediscon IS NULL AND ISNULL(telno, '') <> ''
		 
END
