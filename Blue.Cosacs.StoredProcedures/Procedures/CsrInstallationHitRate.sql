If OBJECT_ID('CsrInstallationHitRate') IS NOT NULL
	DROP PROCEDURE CsrInstallationHitRate
GO

CREATE PROCEDURE CsrInstallationHitRate
	@Today Date,
    @CalculatePerCsr Bit = 1
AS
	SET NOCOUNT ON 

	DECLARE @threeMonthsAgo DATE = DATEADD(WEEK, -11, @Today)
	DECLARE @FirstWeek DATE 
	
	IF OBJECT_ID('tempdb..#d','U') IS NOT NULL 
		DROP TABLE #d
		
	--find the first day of that week
	select @FirstWeek = f.StartDate from FinancialWeeks f where @threeMonthsAgo BETWEEN f.StartDate AND f.EndDate

    IF OBJECT_ID('tempdb..#Deliveries') IS NOT NULL
		DROP TABLE #Deliveries

	SELECT 
		d.ItemID AS itemId,
		d.branchno AS Branch,
		d.ParentItemID,
		d.quantity AS Quantity,
		f.week, 
        CASE    
            WHEN @CalculatePerCsr = 1 THEN agr.empeenosale
            ELSE u.BranchNo
        END AS SalesPerson
	INTO #Deliveries
	FROM 
		delivery d
		INNER JOIN agreement ag
			ON d.acctno = ag.acctno
			AND d.agrmtno = ag.agrmtno
		INNER JOIN FinancialWeeks f
			ON CONVERT(DATE, d.datedel) BETWEEN f.StartDate AND f.EndDate
        INNER JOIN agreement agr
            ON d.acctno = agr.acctno
            AND agr.agrmtno = d.agrmtno
        INNER JOIN Admin.[User] u
            ON agr.empeenosale = u.Id
	WHERE
		d.quantity > 0
        AND CONVERT(DATE, d.datedel) BETWEEN @FirstWeek AND @Today 

	--Get all sales and applay all possible filters
	;WITH Installations(ItemId, ProductGroup, Category, Class, SubClass) AS
	(
		SELECT 
			s.id,
			NULLIF(sa.ProductGroup, 'Any') AS ProductGroup,
			NULLIF(sa.Category, 0) AS Category,
			NULLIF(sa.Class, 'Any') AS Class,
			NULLIF(sa.SubClass , 'Any') AS SubClass
		FROM 
			StockInfoAssociated sa
			INNER JOIN StockInfo s
				ON s.Id = sa.AssocItemId
				AND s.category = 93
	),
	--Get Products that are installations
	--joining StockInfo with Installations we know what products can have installations attached
	ProductWithInstallation(id) AS
	(
		SELECT DISTINCT
			s.Id
		FROM 
			StockInfo s
			INNER JOIN ProductHeirarchy PH
				ON CONVERT(VARCHAR(32), s.category) = ph.primaryCode 
				AND ph.catalogType = '02'
			INNER JOIN  Installations a
				ON PH.ParentCode = ISNULL(a.ProductGroup, PH.ParentCode)
				AND CONVERT(VARCHAR, s.category) = ISNULL(a.category, s.category)
				AND ISNULL(s.Class, '') = COALESCE(a.Class, s.class, '')
				AND ISNULL(s.SubClass, '') = COALESCE(a.SubClass, s.SubClass, '')
			INNER JOIN Code c
				ON c.code = s.category
				AND c.category IN ('PCE', 'PCF', 'PCO', 'PCW')
	),
	--Get products with installations sold
	ProductWithInstallationSold(SalesPerson, SalesCount, Week) AS
	(
		SELECT 
			d.SalesPerson,
			SUM(d.Quantity) AS SalesCount,
			d.week
		FROM 
			ProductWithInstallation s
			INNER JOIN #Deliveries d
				ON s.Id = d.ItemId
		WHERE
			ParentItemId = 0
		GROUP BY
			d.SalesPerson, d.Week
	),
	--get installations sold
	InstallationSold(SalesPerson, InstallationCount, week) AS
	(
		SELECT 
			d.SalesPerson,
			SUM(d.Quantity) InstallationCount, d.Week
		FROM 
			(SELECT ItemId FROM Installations GROUP BY ItemId) AS inst
			INNER JOIN #Deliveries d
				ON inst.ItemId = d.ItemId
			INNER JOIN StockInfo s
				ON d.ParentItemId = s.Id
		GROUP BY
			d.SalesPerson, d.Week
	),
	--this is just to SUM the columns that i will need to calculate the hit rate
	TotalSales(SalesPerson, SalesCount, InstallationsCount, Week) AS 
	(
		SELECT
			pis.SalesPerson,
			SUM(pis.SalesCount) AS SalesCount,
			SUM(iss.InstallationCount) AS  InstallationsCount,
			pis.Week
		FROM 
			ProductWithInstallationSold pis
			LEFT JOIN InstallationSold iss
				ON pis.SalesPerson = iss.SalesPerson
                AND pis.Week = iss.week
		GROUP BY
			pis.SalesPerson, pis.Week
	)
	SELECT 
		ROW_NUMBER() OVER (PARTITION BY WeeksUsers.Id ORDER BY WeeksUsers.StartDate) AS WeekNo, 
		@FirstWeek as FirstWeek,
		WeeksUsers.Id AS Csr,
		ISNULL(Data.Total, 0.0) AS Total,
		WeeksUsers.BranchNo
	FROM 
	(
		---------------------------------------------------
		---   Here is created all weeks for all users   ---
		---------------------------------------------------
		SELECT 
			usr.Id, f.Week, usr.BranchNo, f.StartDate
		FROM 
			FinancialWeeks f
			CROSS JOIN 
			(
				SELECT DISTINCT
                    CASE 
                        WHEN @CalculatePerCsr = 1 THEN Id
                        ELSE BranchNo
                     END AS id, BranchNo FROM Admin.[User]
			) usr
		WHERE 
			f.StartDate >= (select f.StartDate from FinancialWeeks f where (@threeMonthsAgo BETWEEN f.StartDate AND f.EndDate))
			AND f.EndDate <= (select f.EndDate from FinancialWeeks f where (@Today BETWEEN f.StartDate AND f.EndDate))
	)WeeksUsers
	LEFT JOIN 
	(
		SELECT
			SalesPerson,
			CONVERT(DECIMAL(10, 3), 
				CONVERT(DECIMAL(10, 3), ISNULL(InstallationsCount, 0)) 
				/                           --The NULLIF is to avoid divide by 0 exception
				ISNULL(CONVERT(DECIMAL(10, 3), NULLIF(SalesCount, 0)), 1) * 100.0
			) AS Total,
			Week
		FROM 
			TotalSales 
	) Data
		ON Data.SalesPerson = WeeksUsers.Id 
		AND Data.Week = WeeksUsers.Week
	ORDER BY 
		WeeksUsers.Id,
		WeekNo

	DROP TABLE #Deliveries
