IF OBJECT_ID('SalesManagemntSummaryTableAverageTermLength') IS NOT NULL
	DROP PROCEDURE SalesManagemntSummaryTableAverageTermLength 
GO

CREATE PROCEDURE SalesManagemntSummaryTableAverageTermLength
	@Today			Date
AS 
	SET NOCOUNT ON

	DECLARE @Week DATE = (SELECT DATEADD(DAY, -DATEPART(WEEKDAY, @Today), @Today))

    IF OBJECT_ID('tempdb..#items') IS NOT NULL 
        DROP TABLE #items

    IF OBJECT_ID('tempdb..#csrs') IS NOT NULL 
        DROP TABLE #csrs

    IF OBJECT_ID('tempdb..#delivery') IS NOT NULL
        DROP TABLE #delivery

    SELECT 
        l.ItemID,
        l.ParentItemID,
        l.stocklocn,
        l.acctno,
        l.agrmtno,
        l.contractno,
        sum(l.ValueAfter - l.ValueBefore) AS amount
    INTO #items
    FROM 
        lineitemaudit l 
        INNER JOIN acct a
			ON l.acctno = a.acctno
			AND a.accttype IN ('C', 'R', 'O')
    WHERE
        l.acctno!= ''
		AND CONVERT(DATE, l.Datechange) = @Today
        AND l.itemno != 'STAX'
        AND l.ValueAfter - l.ValueBefore > 0
    GROUP BY 
        l.ItemID,
        l.ParentItemID,
        l.stocklocn,
        l.acctno,
        l.agrmtno,
        l.contractno

    SELECT 
        l.ItemID,
        l.ParentItemID,
        l.stocklocn,
        l.acctno,
        l.agrmtno,
        l.contractno,
        l.Empeenochange,
        l.ValueAfter - l.ValueBefore as Amount,
        l.Datechange
    INTO #csrs
    FROM 
        LineitemAudit l
        INNER JOIN acct a
			ON l.acctno = a.acctno
			AND a.accttype IN ('C', 'R', 'O')
    WHERE
        l.ValueAfter - l.ValueBefore > 0
        AND l.acctno!= ''
		AND CONVERT(DATE, l.Datechange) = @Today
        AND l.itemno != 'STAX'

    /* in case that there is more than one reposetion we need to take inconsideration only the last one*/
    SELECT 
        d.acctno, 
        d.stocklocn, 
        d.agrmtno, 
        d.contractno, 
        d.ItemID, 
        d.ParentItemID, 
        d.transvalue
    INTO #delivery
    FROM
        delivery d
        INNER JOIN 
        (
            SELECT acctno, stocklocn, agrmtno, contractno, ItemID, ParentItemID, MAX(d.datedel) AS datedel
            FROM delivery d
            WHERE CONVERT(DATE, d.datedel) >= @Today AND (d.delorcoll = 'R' AND d.quantity < 0 OR d.delorcoll = 'C')
            GROUP BY acctno, stocklocn, agrmtno, contractno, ItemID, ParentItemID
        ) md
            ON d.acctno = md.acctno
            AND d.stocklocn = md.stocklocn
            AND d.agrmtno = md.agrmtno
            AND d.contractno = md.contractno
            AND d.ItemID = md.ItemID
            AND d.ParentItemID = md.ParentItemID
            AND d.datedel = md.datedel
            AND (d.delorcoll = 'R' AND d.quantity < 0  OR d.delorcoll = 'C')

	SELECT 
		Data.SalesPerson,
		u.BranchNo,
		Data.Amount,
		Data.Area
	FROM 
	(

		SELECT 
			i.empeenochange AS SalesPerson,
			SUM(CASE	
					WHEN CONVERT(Date, a.dateagrmt) = CONVERT(Date, @Today) THEN i.instalno
					ELSE 0
			END)
			/
			ISNULL(NULLIF(SUM(CASE
							WHEN CONVERT(Date, a.dateagrmt) = CONVERT(Date, @Today) THEN 1
							ELSE 0
						  END), 0), 1)										AS Amount,
			'Average Term Length'                                           AS Area
		FROM
			instalplan i
			INNER JOIN agreement a
				ON i.acctno = a.acctno
			INNER JOIN acct ac
				ON i.acctno = ac.acctno
				AND ac.accttype IN ('C', 'R', 'O')
		WHERE 
			i.acctno!= ''
			AND CONVERT(DATE, a.dateagrmt) = @Today
		GROUP BY 
			i.empeenochange
	) Data
	INNER JOIN Admin.[User] u
		ON Data.SalesPerson = u.Id
	ORDER BY
		SalesPerson

    DROP TABLE #csrs
    DROP TABLE #items
    DROP TABLE #delivery