IF OBJECT_ID('CsrCreditMix') IS NOT NULL
    DROP PROCEDURE CsrCreditMix
GO
CREATE PROCEDURE CsrCreditMix
    @Today Date
AS
    SET NOCOUNT ON

    --Set the tax rate based on the country parameter
    DECLARE @DefaultTaxRate DECIMAL(5, 2)
    SELECT @DefaultTaxRate = CONVERT(DECIMAL(18, 3), value) FROM dbo.CountryMaintenance WHERE CodeName = 'taxrate'
    DECLARE @threeMonthsAgo DATE = DATEADD(WEEK, -11, @Today)
    DECLARE @FirstWeek DATE 
        
    --find the first day of that week
    select @FirstWeek = f.StartDate from FinancialWeeks f where @threeMonthsAgo BETWEEN f.StartDate AND f.EndDate
    --find if country is tax inclusive
    DECLARE @IsTaxInclusive Bit = (SELECT CONVERT(BIT, CASE WHEN Value = 'E' THEN 0 ELSE 1 END) FROM dbo.CountryMaintenance WHERE CodeName = 'agrmttaxtype')

    IF OBJECT_ID('tempdb..#tmp') IS NOT NULL 
        DROP TABLE #tmp

    -----------------------------------------------------
    ---   Get all the data and put in a temp table.   ---
    ---   Also create ISExchange column and TempKey   ---
    -----------------------------------------------------
    SELECT
        a.LineItemAuditID,  
        CASE 
            WHEN a.source LIKE 'GRTExchange%' THEN 1
            ELSE 0
        END AS ISExchange,
        /*this temp key is based on the lineitem primary key*/
        a.acctno + '-' +  CONVERT(VarChar, a.agrmtno) + '-' +  CONVERT(VarChar, a.stocklocn) + '-' +  a.contractno + '-' +  CONVERT(VarChar, a.ItemID) + '-' +  CONVERT(VarChar, a.ParentItemID) AS TempKey,
        a.Empeenochange,
        a.ItemID,
        a.acctno,
        a.Datechange,
        a.stocklocn
    INTO #d
    FROM 
        LineitemAudit a
    WHERE
        a.acctno != ''
        AND a.ItemID NOT IN 
        (
            SELECT Id FROM StockInfo WHERE category in (SELECT code FROM code WHERE category = 'PCDIS') OR (itemno = 'STAX')
        )

    ---------------------------------------------------
    ---   Delete all data that has ISExchange = 1   ---
    ---------------------------------------------------

    --------------------------------------------------------------------------------------------------------------------------
    ---   How does it works?                                                                                               ---
    ---       LineItemAuditID - ISExchange - TempKey                                                                       ---
    ---       300               1            AAA                                                                           ---
    ---       299               0            ACT                                                                           ---
    ---       ...                                                                                                          ---
    ---       250               0            AAA                                                                           ---
    ---       ...                                                                                                          ---
    ---   So this query query will delete LineItemAuditID IN 300 & 250 because both belongs to a sale that was exchanged   ---
    --------------------------------------------------------------------------------------------------------------------------
    DELETE
        #d 
    where 
        LineItemAuditID IN 
        (
            SELECT LineItemAuditID FROM #d WHERE TempKey in 
            (
                SELECT DISTINCT TempKey FROM #d where ISExchange = 1
            )
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
                    SELECT Id, BranchNo FROM Admin.[User]-- where id = 100018
                ) usr
            WHERE 
                f.StartDate >= (select f.StartDate from FinancialWeeks f where (@threeMonthsAgo BETWEEN f.StartDate AND f.EndDate))
                AND f.EndDate <= (select f.EndDate from FinancialWeeks f where (@Today BETWEEN f.StartDate AND f.EndDate))
        ) WeeksUsers
        LEFT JOIN 
        (
            SELECT 
                i.Empeenochange AS SalesPersonId,
                f.Week,
                SUM(CASE    
                        WHEN a.accttype IN ('R', 'O') THEN sp.CashPrice / CASE WHEN @IsTaxInclusive = 1 THEN (1 + (ISNULL(NULLIF(tax.specialrate, 0), @DefaultTaxRate) / 100)) ELSE 1 END 
                        ELSE 0
                     END
                    ) 
                    /
                    ISNULL(NULLIF(SUM(sp.CashPrice / CASE WHEN @IsTaxInclusive = 1 THEN (1 + (ISNULL(NULLIF(tax.specialrate, 0), @DefaultTaxRate) / 100)) ELSE 1 END), 0), 1) AS Total
            FROM 

                #d i
                INNER JOIN FinancialWeeks f
                    ON CONVERT(DATE, i.Datechange) BETWEEN f.StartDate AND f.EndDate
                INNER JOIN StockPrice sp
                    ON sp.branchno = i.stocklocn
                    AND sp.ID = i.ItemID
                INNER JOIN acct a
                    ON i.acctno = a.acctno
                LEFT JOIN taxitemhist tax
                    ON tax.ItemID = i.ItemID
                    AND i.Datechange >= tax.datefrom
            WHERE
                CONVERT(DATE, i.Datechange) BETWEEN @FirstWeek AND @Today 
            GROUP BY 
                i.Empeenochange,
                f.Week
        ) Data
            ON Data.SalesPersonId = WeeksUsers.Id 
            AND Data.Week = WeeksUsers.Week
        ORDER BY 
             Csr,
             WeekNo

    DROP TABLE #d