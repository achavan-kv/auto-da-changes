-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF  NOT OBJECT_ID('[Service].[NewInternalServiceRequest]') IS NULL
	DROP PROCEDURE [Service].[NewInternalServiceRequest]
GO

CREATE PROCEDURE [Service].[NewInternalServiceRequest] 
	@Type varchar(4),
	@CustomerId varchar(50) = NULL,
	@CustomerAccount char(12) = NULL
AS
BEGIN
	SET NOCOUNT ON;

	IF OBJECT_ID('tempdb..#ServiceData','U') IS NOT NULL 
		DROP TABLE #ServiceData

	--------------------------------------------------------------------------------------------
	---   I need to create the table before the if statement, otherwise it throws an error   ---
	--------------------------------------------------------------------------------------------
	SELECT 
		c.CustomerAccount, c.CustomerAddressLine1, c.CustomerAddressLine2, c.CustomerAddressLine3, c.CustomerFirstName, c.CustomerId, c.CustomerLastName, c.CustomerPostcode, c.CustomerTitle, c.CustomerNotes, 
		c.ItemDescription, ISNULL(c.ItemPrice, 0) AS ItemPrice, ISNULL(c.ItemCostPrice, 0) AS ItemCostPrice, c.ItemDeliveredOn, ISNULL( c.ItemId, 0 )AS ItemId, c.ItemNumber, c.ItemUPC, c.StockLocation, brn.BranchNameLong,
		c.ItemSupplier, c.SoldOn, ISNULL(c.SoldById, 0) AS SoldById, c.SoldBy, c.ItemSerialNumber, c.WarrantyGroupId, c.WarrantyNumber, c.WarrantyType, ISNULL(c.WarrantyLength, 0) AS WarrantyLength,  c.WarrantyContractNo,
		c.ManufacturerWarrantyNumber, c.ManufacturerWarrantyContractNo, c.ManufacturerWarrantyLength, c.WarrantySaleId
	INTO #ServiceData
	FROM 
		Service.CustomerSearchView AS C 
		INNER JOIN Service.BranchLookup brn
		ON brn.branchno = c.StockLocation
	WHERE
		1 = 0 --to bring no data at all
	
	-----------------------------------------------------------------------------------------------
	---   Stores the data in a temp table. It is faster to filter by one condtion at the time   ---
	-----------------------------------------------------------------------------------------------
	IF @CustomerAccount IS NOT NULL
	BEGIN
		INSERT INTO #ServiceData
		SELECT 
			c.CustomerAccount, 
			c.CustomerAddressLine1, 
			c.CustomerAddressLine2, 
			c.CustomerAddressLine3, 
			c.CustomerFirstName, 
			c.CustomerId, 
			c.CustomerLastName, 
			c.CustomerPostcode, 
			c.CustomerTitle, 
			c.CustomerNotes, 
			CASE
				WHEN i.itemdescr1 IS NOT NULL THEN LTRIM(RTRIM(i.itemdescr1)) + LTRIM(RTRIM(i.itemdescr2))
				ELSE c.ItemDescription
			END AS ItemDescription,
			ISNULL(c.ItemPrice, 0) AS ItemPrice,
			ISNULL(c.ItemCostPrice, 0) AS ItemCostPrice,
			c.ItemDeliveredOn, 
			ISNULL( c.ItemId, 0 )AS ItemId, 
			c.ItemNumber, 
			c.ItemUPC,
			c.StockLocation,
			brn.BranchNameLong,
			c.ItemSupplier, 
			c.SoldOn,
			ISNULL(c.SoldById, 0) AS SoldById, 
			c.SoldBy,
			c.ItemSerialNumber, 
			c.WarrantyGroupId, 
			c.WarrantyNumber, 
			c.WarrantyType, 
			ISNULL(c.WarrantyLength, 0) AS WarrantyLength,  
			c.WarrantyContractNo,
			c.ManufacturerWarrantyNumber, 
			c.ManufacturerWarrantyContractNo,
			c.ManufacturerWarrantyLength, 
			c.WarrantySaleId
		FROM 
			Service.CustomerSearchView AS C 
			INNER JOIN Service.BranchLookup brn
				ON brn.branchno = c.StockLocation
			LEFT JOIN StockInfo i
				ON c.ItemNumber = i.itemno
		WHERE
			C.CustomerAccount = @CustomerAccount
			AND (@Type <> 'II' OR (@Type = 'II' AND c.Type IS NULL AND i.category = 93))
			AND (
					(
						(ISNULL( C.WarrantyContractNo, '') != '' and C.WarrantyContractNo != N'MAN001') or (ISNULL( C.ManufacturerWarrantyContractNo, '') != '' and C.ManufacturerWarrantyContractNo != N'MAN001')
						
					 ) OR  ISNULL( C.ItemSerialNumber, '' ) != ''
					
				) 
			AND NOT EXISTS
			(
				SELECT 1 -- DISTINCT SR.WarrantyContractNo
				FROM Service.Request SR 
				WHERE C.ItemId = SR.ItemId AND C.CustomerAccount = SR.Account AND C.WarrantyContractNo = SR.WarrantyContractNo AND SR.State != 'Closed'
			)
	END
	ELSE
	BEGIN
		INSERT INTO #ServiceData
		SELECT 
			c.CustomerAccount,
			c.CustomerAddressLine1, 
			c.CustomerAddressLine2, 
			c.CustomerAddressLine3, 
			c.CustomerFirstName, 
			c.CustomerId, 
			c.CustomerLastName, 
			c.CustomerPostcode, 
			c.CustomerTitle, 
			c.CustomerNotes, 
			CASE
				WHEN i.itemdescr1 IS NOT NULL THEN LTRIM(RTRIM(i.itemdescr1)) + LTRIM(RTRIM(i.itemdescr2))
				ELSE c.ItemDescription
			END AS ItemDescription,
			ISNULL(c.ItemPrice, 0) AS ItemPrice,
			ISNULL(c.ItemCostPrice, 0) AS ItemCostPrice,
			c.ItemDeliveredOn, 
			ISNULL(c.ItemId, 0) AS ItemId, 
			c.ItemNumber, 
			c.ItemUPC,
			c.StockLocation,
			brn.BranchNameLong,
			c.ItemSupplier, 
			c.SoldOn,
			ISNULL(c.SoldById, 0) AS SoldById, 
			c.SoldBy,
			c.ItemSerialNumber, 
			c.WarrantyGroupId, 
			c.WarrantyNumber, 
			c.WarrantyType, 
			ISNULL(c.WarrantyLength, 0) AS WarrantyLength,  
			c.WarrantyContractNo,
			c.ManufacturerWarrantyNumber, 
			c.ManufacturerWarrantyContractNo,
			c.ManufacturerWarrantyLength, 
			c.WarrantySaleId
		FROM 
			Service.CustomerSearchView AS C 
			INNER JOIN Service.BranchLookup brn
			ON brn.branchno = c.StockLocation
			LEFT JOIN StockInfo i
				ON c.ItemNumber = i.itemno
		WHERE
			c.CustomerId = @CustomerId
			AND (@Type <> 'II' OR (@Type = 'II' AND c.Type IS NULL))
			AND (
					(
						((ISNULL( C.WarrantyContractNo, '') != '' and C.WarrantyContractNo != N'MAN001') or (ISNULL( C.ManufacturerWarrantyContractNo, '') != '' and C.ManufacturerWarrantyContractNo != N'MAN001'))
						
					) OR  ISNULL( C.ItemSerialNumber, '' ) != ''
				) 
			AND NOT EXISTS
			(
				SELECT 1 -- DISTINCT SR.WarrantyContractNo
				FROM Service.Request SR 
				WHERE C.ItemId = SR.ItemId AND C.CustomerAccount = SR.Account AND C.WarrantyContractNo = SR.WarrantyContractNo AND SR.State != 'Closed'
			)
	END

	SELECT 
		c.CustomerAccount AS Account, 
		c.CustomerAddressLine1, 
		c.CustomerAddressLine2, 
		c.CustomerAddressLine3, 
		c.CustomerFirstName, 
		c.CustomerId, 
		c.CustomerLastName, 
		c.CustomerPostcode, 
		c.CustomerTitle, 
		c.CustomerNotes, 
		RTRIM(LTRIM(c.ItemDescription)) AS Item, 
		c.ItemPrice AS ItemAmount, 
		c.ItemCostPrice AS ItemCostPrice, 
		c.ItemDeliveredOn, 
		c.ItemId,
		c.ItemNumber, 
		c.ItemUPC AS Iupc, 
		c.StockLocation AS ItemStockLocation, 
		c.BranchNameLong AS ItemStockLocationName, 
		c.ItemSupplier, 
		c.SoldOn AS ItemSoldOn, 
		ISNULL(c.SoldById, 0) AS ItemSoldBy, 
		c.SoldBy AS ItemSoldByName, 
		c.ItemSerialNumber, 
		c.WarrantyGroupId, 
		c.WarrantyNumber, 
		c.WarrantyType, 
		ISNULL(c.WarrantyLength, 0) AS WarrantyLength,  
		c.WarrantyContractNo AS WarrantyContractNumber, 
		c.ManufacturerWarrantyNumber, 
		c.ManufacturerWarrantyContractNo AS ManufacturerWarrantyContractNumber, 
		c.ManufacturerWarrantyLength, 
		c.WarrantySaleId AS WarrantyContractId, 
		0 AS TotalRequests, 
		1 AS ItemCount
	FROM 
		#ServiceData C
	UNION 
	SELECT c.CustomerAccount AS Account, 
		c.CustomerAddressLine1, 
		c.CustomerAddressLine2, 
		c.CustomerAddressLine3, 
		c.CustomerFirstName, 
		c.CustomerId, 
		c.CustomerLastName, 
		c.CustomerPostcode, 
		c.CustomerTitle, 
		c.CustomerNotes, 
		CASE
			WHEN si.itemdescr1 IS NOT NULL THEN LTRIM(RTRIM(si.itemdescr1)) + LTRIM(RTRIM(si.itemdescr2))
			ELSE RTRIM(LTRIM(c.ItemDescription))
		END AS Item,
		ISNULL( c.ItemPrice, 0 )AS ItemAmount, 
		ISNULL( c.ItemCostPrice, 0 )AS ItemCostPrice, 
		c.ItemDeliveredOn, 
		ISNULL( c.ItemId, 0 )AS ItemId, 
		c.ItemNumber, 
		c.ItemUPC AS Iupc, 
		c.StockLocation AS ItemStockLocation, 
		brn.BranchNameLong AS ItemStockLocationName, 
		c.ItemSupplier, 
		c.SoldOn AS ItemSoldOn, 
		ISNULL( c.SoldById, 0 )AS ItemSoldBy, 
		c.SoldBy AS ItemSoldByName, 
		c.ItemSerialNumber, 
		NULL AS WarrantyGroupId, 
		c.WarrantyNumber, 
		c.WarrantyType, 
		ISNULL(c.WarrantyLength, 0) AS WarrantyLength,  
		c.WarrantyContractNo AS WarrantyContractNumber, 
		c.ManufacturerWarrantyNumber, 
		c.ManufacturerWarrantyContractNo AS ManufacturerWarrantyContractNumber, 
		c.ManufacturerWarrantyLength, 
		NULL AS WarrantyContractId, 
		I.TotalRequests, 
		I.ItemCount
	FROM 
		Service.CustomerSearchView AS c 
		INNER JOIN Service.BranchLookup brn
			ON brn.branchno = c.StockLocation
		INNER JOIN Service.ItemsWithoutWarrantyView I
			ON I.ItemId = C.ItemId AND I.StockLocation = C.StockLocation AND 
			I.CustomerId = C.CustomerId AND
			I.CustomerAccount = C.CustomerAccount
		LEFT JOIN StockInfo si
			ON c.ItemNumber = si.itemno
	WHERE
		(ISNULL( @CustomerId, '' ) = '' OR c.CustomerId = @CustomerId) 
		AND (ISNULL( @CustomerAccount, '' ) = '' OR C.CustomerAccount = @CustomerAccount) 
		AND (I.ItemCount > I.TotalRequests) 
		AND (@Type <> 'II' OR (@Type = 'II' AND c.Type IS NULL)) 
		AND (ISNULL( C.WarrantyContractNo, '' ) = '' OR C.WarrantyContractNo = N'MAN001' OR ISNULL( C.ItemSerialNumber, '' ) = '')
		AND NOT EXISTS(select 1 from #ServiceData sd
					where sd.CustomerAccount = c.CustomerAccount
					and sd.ItemId = c.ItemId
					and sd.StockLocation = c.StockLocation
					and sd.WarrantyGroupId = c.WarrantyGroupId)


	IF OBJECT_ID('tempdb..#ServiceData','U') IS NOT NULL 
		DROP TABLE #ServiceData

END
GO

