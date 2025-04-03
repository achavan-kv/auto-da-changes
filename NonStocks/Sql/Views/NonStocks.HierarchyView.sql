IF EXISTS (SELECT 'a' FROM sys.views WHERE object_id = OBJECT_ID(N'[NonStocks].[HierarchyView]'))
    DROP VIEW [NonStocks].[HierarchyView]
GO

CREATE VIEW [NonStocks].[HierarchyView]
AS
    SELECT 
        n.id AS NonStockId,
        hl.LevelId,
        hl.[Level],
        hl.LevelName,
        n.[Type] AS NonStockType,
        n.SKU,
        n.Active,
        n.TaxRate
    FROM NonStocks.NonStock n
    LEFT OUTER JOIN (SELECT l.Id AS LevelId,
                            l.Name AS [Level],
                            h.LevelName,
                            h.NonStockId 
                     FROM Merchandising.HierarchyLevel l
                     INNER JOIN NonStocks.NonStockHierarchy h
                        ON h.[Level] = l.Id
                    ) AS hl
    ON hl.NonStockId = n.id
GO