IF OBJECT_ID('Customer.CustomerSearchView') IS NOT NULL
	DROP VIEW Customer.[CustomerSearchView]
GO

CREATE VIEW Customer.[CustomerSearchView]
AS
	WITH query 
	AS
	( 
		SELECT        
			CustomerId AS CustomerId, 
			Title, 
			FirstName,
			LastName AS LastName, 
			NULL as Alias,
			NULL AS DOB, 
			HomePhone AS HomePhoneNumber, 
			MobilePhone AS MobileNumber, 
			Email, 
			AddressLine1 AS HomeAddressLine1, 
			AddressLine2 AS HomeAddressLine2, 
			AddressLine3 AS City, 
			PostCode AS PostCode,
			IsSalesCustomer AS IsSalesCustomer,
			0 as HasRCreditSource,
			NULL as RCreditSourceDate,
			0 as HasOCreditSource,
			NULL as OCreditSourceDate,
			0 as HasCashSource,
			NULL as CashSourceDate,
			0 as HasStoreCardSource,
			NULL as StoreCardSourceDate,
			Convert(Bit,
						CASE WHEN WarrantyType.OrderId IS NOT NULL
				THEN 1
				ELSE 0
			END ) as HasWarrantySource,
			CASE WHEN WarrantyType.OrderId IS NOT NULL
				THEN o.CreatedOn
				ELSE NULL
			END as WarrantySourceDate,
			Convert(Bit,
						CASE WHEN InstallationType.OrderId IS NOT NULL
				THEN 1
				ELSE 0
			END ) as HasInstallationSource,
			CASE WHEN InstallationType.OrderId IS NOT NULL
				THEN o.CreatedOn
				ELSE NULL
			END as InstallationSourceDate,
			NULL AS AvailableSpend,
			NULL AS CustomerBranchNo,
			NULL AS CustomerBranchName,
			NULL AS SalesPersonId,
			NULL AS SalesPerson,
			NULL AS DateLastBought,
			'Sales' AS CustomerSource
		FROM 
			[Sales].[OrderCustomer] AS oc
			INNER JOIN [Sales].[Order] AS o
				ON orderid = id AND oc.IsSalesCustomer = 1
			LEFT JOIN
			(			
				SELECT
						Distinct(OrderId)
				FROM [Sales].[OrderItem]
				WHERE ItemTypeId = 2
			) AS WarrantyType
				ON o.id = WarrantyType.OrderId
			LEFT JOIN
			(			
				SELECT
						Distinct(OrderId)
				FROM [Sales].[OrderItem]
				WHERE ItemTypeId = 3
			) AS InstallationType
				ON o.id = InstallationType.OrderId
	)
	SELECT 
		ROW_NUMBER() OVER(ORDER BY CustomerId) as ID, *
	FROM query

GO