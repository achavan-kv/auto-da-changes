-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
SELECT 
    csp.CustomerId
INTO #Customers 
FROM 
    SalesManagement.CustomerSalesPerson csp
    LEFT JOIN customer c
        ON csp.CustomerId = c.custid
    LEFT JOIN branch b
        ON csp.CustomerBranch = b.branchno
    LEFT JOIN Admin.[User] sp
        ON csp.SalesPersonId = sp.Id
    LEFT JOIN Admin.[User] tsp
        ON csp.TempSalesPersonId = tsp.Id
WHERE
    c.custid IS NULL
    OR b.branchno IS NULL
    OR sp.Id IS NULL
    OR (tsp.Id IS NULL AND csp.TempSalesPersonId IS NOT NULL)

DELETE SalesManagement.Call
WHERE Id in 
(
    SELECT 
        ca.Id
    FROM 
        SalesManagement.Call ca
        LEFT JOIN customer c
            ON ca.CustomerId = c.custid
        LEFT JOIN Admin.[User] u
            ON ca.SalesPersonId = u.id
        LEFT JOIN Admin.[User] called
            ON ca.SalesPersonId = called.id
        LEFT JOIN branch b
            ON ca.Branch = b.branchno
    WHERE
        c.custid IS NULL
        OR (u.id IS NULL AND ca.SalesPersonId IS NOT NULL)
        OR (called.id is null AND ca.CalledBy IS NOT NULL)
        OR (b.branchno is null AND ca.Branch IS NOT NULL)
    UNION ALL 
    SELECT 
        Id
    FROM 
        SalesManagement.Call c
        INNER JOIN #Customers cu
            ON c.CustomerId = cu.CustomerId
)
    
DELETE SalesManagement.CsrUnavailable
WHERE Id IN 
(
    SELECT 
        un.Id
    FROM 
        SalesManagement.CsrUnavailable un
        LEFT JOIN admin.[User] u
            ON un.SalesPersonId = u.Id
    WHERE
        u.Id IS NULL
)

DELETE SalesManagement.SalesPersonTargets
WHERE Id IN 
(
    SELECT 
        tsp.Id
    FROM 
        SalesManagement.SalesPersonTargets tsp
        LEFT JOIN admin.[User] u
            ON tsp.CreatedBy = u.Id
    WHERE
        u.Id IS NULL
)
    
DELETE SalesManagement.CustomerSalesPerson
WHERE CustomerId In 
(
    SELECT CustomerId FROM #Customers
)



  
DELETE SalesManagement.CustomerSalesPerson
WHERE CustomerId In 
(
    SELECT CustomerId
    FROM SalesManagement.CustomerSalesPerson a LEFT JOIN Admin.[User] u ON a.SalesPersonId = u.Id
    WHERE u.Id IS NULL
)

DROP TABLE #Customers