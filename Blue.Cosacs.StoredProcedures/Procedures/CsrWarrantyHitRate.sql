If OBJECT_ID('CsrWarrantyHitRate') IS NOT NULL
	DROP PROCEDURE CsrWarrantyHitRate
GO

CREATE PROCEDURE CsrWarrantyHitRate
	@Today Date,
    @CalculatePerCsr Bit = 1
AS
	--------------------------------------------
	---   Get all data from delivery table   ---
	--------------------------------------------
    SET NOCOUNT ON
	
	DECLARE @threeMonthsAgo DATE = DATEADD(WEEK, -11, @Today)
	DECLARE @FirstWeek DATE 
	
	IF OBJECT_ID('tempdb..#d','U') IS NOT NULL 
		DROP TABLE #d
		
	--find the first day of that week
	select @FirstWeek = f.StartDate from FinancialWeeks f where @threeMonthsAgo BETWEEN f.StartDate AND f.EndDate


	SELECT 
		ItemId, 
		delorcoll, 
		d.acctno, 
		ParentItemID, 
		contractno, 
		CONVERT(DATE, d.datetrans) AS datetrans, 
		quantity, 
		d.agrmtno, 
		si.itemtype,
		f.Week, 
        CASE    
            WHEN @CalculatePerCsr = 1 THEN agr.empeenosale
            ELSE u.BranchNo
        END AS empeenosale
	INTO #d
	FROM 
		delivery d 
		INNER JOIN StockInfo si
			ON d.ItemID = si.Id
		INNER JOIN FinancialWeeks f
			ON CONVERT(DATE, d.datetrans) BETWEEN f.StartDate AND f.EndDate
        INNER JOIN agreement agr
            ON d.acctno = agr.acctno
            AND agr.agrmtno = d.agrmtno
        INNER JOIN Admin.[User] u
            ON agr.empeenosale = u.Id
	WHERE  
		CONVERT(DATE, d.datetrans) BETWEEN @FirstWeek AND @Today 
		AND d.itemno != 'STAX'

	------------------------------------------------------
	---   Get only the returned/cancelled warranties   ---
	------------------------------------------------------
	;WITH ReturnedWarranties(AccountNumber, ParentItemID, AgreementNumber) AS
	(
		SELECT d.acctno, d.ParentItemID, d.agrmtno
		FROM  
			#d d
			INNER JOIN Warranty.WarrantySale s
				ON d.acctno = s.CustomerAccount
				AND d.agrmtno = s.AgreementNumber
				AND d.contractno = s.WarrantyContractNo
				AND ISNULL(s.WarrantyType, '') != 'F' /*No point on free warranties*/
		WHERE  ((d.delorcoll = 'C' AND contractno != '') OR (d.delorcoll = 'R' AND d.quantity < 0  AND contractno != '')) AND d.itemtype != 's'
	              /*Calcel Itemns*/                             /*Repossesed Itemns*/
	),
	------------------------------
	---   Get the warranties   ---
	------------------------------
	Warranties(AccountNumber, ParentItemID, AgreementNumber, empeenosale, week) AS
	(
		SELECT d.acctno, d.ParentItemID, d.agrmtno, d.empeenosale, d.Week
		FROM  
			#d d  
			INNER JOIN Warranty.WarrantySale s 
				ON d.acctno = s.CustomerAccount
				AND d.agrmtno = s.AgreementNumber
				AND d.contractno = s.WarrantyContractNo
				AND ISNULL(s.WarrantyType, '') != 'F' /*No point on free warranties*/
		where  d.delorcoll = 'D' AND contractno != '' AND d.itemtype != 's' --only deliverys and non stock itens
	),
	----------------------------
	---   Get the products   ---
	----------------------------
	Products(ItemId, AccountNumber, ParentItemID, contractno, quantity, AgreementNumber, empeenosale, Week) AS
	(
		SELECT ItemId, d.acctno, ParentItemID, contractno, quantity, d.agrmtno, d.empeenosale, d.Week
		from  #d d
		where  d.delorcoll = 'D' AND contractno = '' AND d.itemtype = 's' --only deliverys and stock itens
	)
	SELECT 
		ROW_NUMBER() OVER (PARTITION BY WeeksUsers.Id ORDER BY WeeksUsers.StartDate) AS WeekNo, 
		@FirstWeek as FirstWeek,
        CASE 
            WHEN @CalculatePerCsr = 1 THEN WeeksUsers.Id
            ELSE WeeksUsers.BranchNo
        END AS csr,
        ISNULL(data.Total, 0.0) AS Total, 
		WeeksUsers.BranchNo AS BranchNo
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
	) WeeksUsers
	LEFT JOIN 
	(
		SELECT 
            CASE 
                WHEN @CalculatePerCsr = 1 THEN agr.empeenosale
                ELSE u.BranchNo
            END AS Entity,
			p.Week,	
			CONVERT(DECIMAL(18,2), 
			(
				CONVERT(DECIMAL(18,2), SUM(ISNULL(w.TotalRcordCount, 0)) - COUNT(r.ParentItemID))
				/ 
				CONVERT(DECIMAL(18,2), ISNULL(NULLIF(SUM(CASE WHEN ISNULL(c.warrantable, 0) = 0 THEN 0 ELSE p.quantity END), 0), 1))
			)  * 100) AS Total
		FROM
			Products p
		    LEFT JOIN 
		    (
			    SELECT 
				    COUNT(*) AS TotalRcordCount,
				    empeenosale, week
			    FROM Warranties d  
			    GROUP BY empeenosale, week
		    ) w
			    ON p.empeenosale = w.empeenosale
			    AND p.Week = w.week
			LEFT JOIN ReturnedWarranties r
				ON p.AccountNumber = r.AccountNumber
				AND p.ItemId = r.ParentItemID
				AND p.AgreementNumber = r.AgreementNumber	
			LEFT JOIN StockInfo c
				ON p.ItemID = c.Id
			INNER JOIN agreement agr
				ON p.AccountNumber = agr.acctno
				AND p.AgreementNumber = agr.agrmtno
			INNER JOIN acct a
				ON p.AccountNumber = a.acctno
            INNER JOIN Admin.[User] u 
                ON agr.empeenosale = u.id
		WHERE
			ISNULL(a.accttype, '') != ''
		GROUP BY
			CASE 
                WHEN @CalculatePerCsr = 1 THEN agr.empeenosale
                ELSE u.BranchNo
            END, p.Week
	) Data
		ON Data.Entity = WeeksUsers.Id
		AND Data.Week = WeeksUsers.Week
	ORDER BY 
		 WeeksUsers.Id,
		 WeekNo

	drop table #d