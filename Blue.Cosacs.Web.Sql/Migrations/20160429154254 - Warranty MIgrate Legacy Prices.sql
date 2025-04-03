-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

SELECT TOP 0 *
INTO #legacyWarrantyData
FROM Warranty.WarrantyPrice


INSERT INTO #legacyWarrantyData
SELECT w.Id, 
       NULL,
       a.BranchNo,
       a.CostPrice,
       a.CreditPrice,
       CAST(a.DateChange AS DATE),
       NULL,
       NULL,
       NULL,
       NULL,
       NULL,
       NULL,
       NULL
FROM StockPriceAudit a
INNER JOIN Warranty.Warranty w
ON a.Itemno = w.Number
WHERE a.DateChange != '1900-01-01 00:00:00.000'
    AND a.DateChange < (SELECT MIN(EffectiveDate) FROM Warranty.WarrantyPrice)

;WITH warrantyStuff(WarrantyId, BranchNumber, CostPrice, RetailPrice, minEffectiveDate)
AS 
(
    SELECT WarrantyId, BranchNumber, CostPrice, RetailPrice, MIN(EffectiveDate)
    FROM #legacyWarrantyData
    GROUP BY WarrantyId, BranchNumber, CostPrice, RetailPrice
)

DELETE l
FROM #legacyWarrantyData l
INNER JOIN warrantyStuff w
ON l.WarrantyId = w.WarrantyId
    AND l.BranchNumber = w.BranchNumber
    AND l.CostPrice = w.CostPrice
    AND l.RetailPrice = w.RetailPrice
    AND l.EffectiveDate > w.minEffectiveDate

SELECT WarrantyId,
       CAST(NULL AS INT) AS BranchNo,
       CostPrice,
       RetailPrice,
       EffectiveDate,
       COUNT(*) AS Num
INTO #cleanUp
FROM #legacyWarrantyData
GROUP BY WarrantyId,
         CostPrice,
         RetailPrice,
         EffectiveDate
HAVING COUNT(*) > 5

UPDATE c
SET c.BranchNo = (SELECT MIN(BranchNumber) 
                  FROM #legacyWarrantyData
                  WHERE c.WarrantyId = WarrantyId
                    AND c.CostPrice = CostPrice
                    AND c.EffectiveDate = EffectiveDate
                    AND c.RetailPrice = RetailPrice)
FROM #cleanUp c

DELETE l
FROM #legacyWarrantyData l
INNER JOIN #cleanUp c
ON l.WarrantyId = c.WarrantyId
    AND l.CostPrice = c.CostPrice
    AND l.EffectiveDate = c.EffectiveDate
    AND l.RetailPrice = c.RetailPrice
    AND l.BranchNumber != c.BranchNo

UPDATE l
SET l.BranchNumber = NULL
FROM #legacyWarrantyData l
INNER JOIN #cleanUp c
ON l.WarrantyId = c.WarrantyId
    AND l.CostPrice = c.CostPrice
    AND l.EffectiveDate = c.EffectiveDate
    AND l.RetailPrice = c.RetailPrice
    AND l.BranchNumber = c.BranchNo

DELETE l
FROM #legacyWarrantyData l
WHERE EXISTS (SELECT 'a' FROM #legacyWarrantyData l2
              WHERE l.WarrantyId = l2.WarrantyId
                AND l.CostPrice = l2.CostPrice
                AND l.RetailPrice = l2.RetailPrice
                AND l.BranchNumber IS NOT NULL
                AND l2.BranchNumber IS NULL
                AND l.EffectiveDate > l2.EffectiveDate)

INSERT INTO Warranty.WarrantyPrice
SELECT WarrantyId,
       BranchType,
       BranchNumber,
       CostPrice,
       RetailPrice,
       EffectiveDate,
       CostPriceChange,
       CostPricePercentChange,
       RetailPriceChange,
       RetailPricePercentChange,
       TaxInclusivePriceChange,
       TaxInclusivePricePercentChange,
       BulkEditId
FROM #legacyWarrantyData l
WHERE NOT EXISTS (SELECT 'a' FROM Warranty.WarrantyPrice w
                  WHERE l.WarrantyId = w.WarrantyId
                    AND l.EffectiveDate = w.EffectiveDate
                    AND ISNULL(l.BranchNumber, 0) = ISNULL(w.BranchNumber, 0)
                    AND ISNULL(l.BranchType, 'a') = ISNULL(w.BranchType, 'a'))