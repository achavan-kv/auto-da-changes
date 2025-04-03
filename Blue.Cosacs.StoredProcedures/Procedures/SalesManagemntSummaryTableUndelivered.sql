IF OBJECT_ID('SalesManagemntSummaryTableUndelivered') IS NOT NULL
	DROP PROCEDURE SalesManagemntSummaryTableUndelivered
GO

CREATE PROCEDURE SalesManagemntSummaryTableUndelivered
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
			d.Empeenochange AS SalesPerson,
			SUM(CASE	
				WHEN CONVERT(Date, d.datetrans) = CONVERT(Date, @Today) THEN d.transvalue - t.amount
				ELSE 0
			END) AS Amount,
			'Undelivered' AS Area
		FROM 
			(
				SELECT 
					c.transvalue - ISNULL(de.transvalue, 0) AS transvalue,
					i.Empeenochange,
					i.ItemID,
					i.acctno,
					i.stocklocn,
					i.agrmtno,
					i.contractno,
					i.ParentItemID,
					c.datetrans
				FROM 
					delivery c
					INNER JOIN #csrs i
						on c.ItemID = i.ItemID
						and c.acctno = i.acctno
						and c.stocklocn = i.stocklocn
						and c.agrmtno = i.agrmtno
						and c.contractno = i.contractno
						and c.ParentItemID = i.ParentItemID
						--AND c.delorcoll = 'D'
					LEFT JOIN #delivery de
						ON de.acctno = c.acctno
						AND de.stocklocn = c.stocklocn
						AND de.agrmtno = c.agrmtno
						AND de.contractno = c.contractno
						AND de.ItemID = c.ItemID
						AND de.ParentItemID = c.ParentItemID
			) d
			INNER JOIN 
			(
				select 
					i.amount,
					i.ItemID,
					i.acctno,
					i.stocklocn,
					i.agrmtno,
					i.contractno,
					i.ParentItemID
				FROM 
					#csrs c
					INNER JOIN #items i
						ON c.ItemID = i.ItemID
						AND c.acctno = i.acctno
						AND c.stocklocn = i.stocklocn
						AND c.agrmtno = i.agrmtno
						AND c.contractno = i.contractno
						AND c.ParentItemID = i.ParentItemID
			) t
				ON  d.ItemID = t.ItemID
				AND d.acctno = t.acctno
				AND d.stocklocn = t.stocklocn
				AND d.agrmtno = t.agrmtno
				AND d.contractno = t.contractno
				AND d.ParentItemID = t.ParentItemID 
		GROUP BY 
			d.Empeenochange
	) Data
	INNER JOIN Admin.[User] u
		ON Data.SalesPerson = u.Id
	ORDER BY
		SalesPerson

    DROP TABLE #csrs
    DROP TABLE #items
    DROP TABLE #delivery