-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

--************************************************Discounts**************************************************************************************

--------------------------------------------------Divisions---------------------------------------------------------------------------------------

UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU in ('DED', 'DEC', 'DEO', 'DECOM', 'DES', 'DEM', 'DWD', 'DWC', 'DWO', 'DWCOM', 'DWS', 'DWM', 'DFM', 'DFS', 'DFD', 'DFC', 'DFO',
                    'DFCOM', 'DS', '7DIS','CDS', 'TRADE1', 'TRADE2', 'TRADE3')
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 1 
    and mht.Code = 'R'

--------------------------------------------------Departments-------------------------------------------------------------------------------------

UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = '7DIS'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 2 
    and mht.Code = 'RR1'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU in ('DS', 'CDS', 'TRADE1', 'TRADE2', 'TRADE3')
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 2 
    and mht.Code = 'RR2'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU in ('DFM', 'DFS', 'DFD', 'DFC', 'DFO', 'DFCOM')
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 2 
    and mht.Code = 'RR3'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU in ('DED', 'DEC', 'DEO', 'DECOM', 'DES', 'DEM', 'DWD', 'DWC', 'DWO', 'DWCOM', 'DWS', 'DWM')
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 2 
    and mht.Code = 'RR4'

--------------------------------------------------Class-------------------------------------------------------------------------------------------

UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = '7DIS'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = '7DIS'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'DS'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'DS'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'CDS'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'CDS'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'TRADE1'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'TDE1'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'TRADE2'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'TDE2'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'TRADE3'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'TDE3'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'DFM'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'DFM'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'DFS'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'DFS'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'DFD'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'DFD'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'DFC'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'DFC'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'DFO'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'DFO'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'DFCOM'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'DFCOM'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'DED'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'DED'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'DEC'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'DEC'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'DEO'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'DEO'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'DECOM'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'DECOM'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'DES'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'DES'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'DEM'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'DEM'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'DWD'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'DWD'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'DWC'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'DWC'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'DWO'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'DWO'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'DWCOM'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'DWCOM'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'DWS'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'DWS'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'DWM'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'DWM'

--************************************************Cash Loan***************************************************************************************

--------------------------------------------------Division----------------------------------------------------------------------------------------

UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'LOAN'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 1 
    and mht.Code = 'T'

--------------------------------------------------Department--------------------------------------------------------------------------------------

UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'LOAN'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 2 
    and mht.Code = 'TT1'

--------------------------------------------------Class--------------------------------------------------------------------------------------

UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'LOAN'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'T1A'


--************************************************Installation/Assembly*************************************************************************

--------------------------------------------------Division--------------------------------------------------------------------------------------

UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU in ('AC12000', 'AC1824', 'AC9000', 'INST')
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 1 
    and mht.Code = 'U'

--------------------------------------------------Department------------------------------------------------------------------------------------

UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU in ('AC12000', 'AC1824', 'AC9000')
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 2 
    and mht.Code = 'UU2'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'INST'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 2 
    and mht.Code = 'UU1'

--------------------------------------------------Class-----------------------------------------------------------------------------------------

UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'AC12000'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'U2A'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'AC1824'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'U2B'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'AC9000'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'U2C'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'INST'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'U1A'


--************************************************Affinity**************************************************************************************

--------------------------------------------------Division----------------------------------------------------------------------------------------

UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU in ('A107', 'A168', 'A186', 'A187', 'A188', 'A189', 'A282', 'A285', 'A288', 'A291', 'A292', 'A293', 'A294', 'A295', 'A296', 'A297', 'A298', 'A299', 'A300',
                 'A301', 'A302','A304', 'A305', 'A312', 'A314', 'A316', 'A317', 'A318', 'A319', 'A321', 'A322', 'A323', 'A324', 'A325', 'A326', 'A327', 'A328', 'A331', 
                 'A332', 'A333', 'MSINS')
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 1 
    and mht.Code = 'W'

--------------------------------------------------Department-------------------------------------------------------------------------------------------------------

UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU in ('A107', 'A168', 'A186', 'A187', 'A188', 'A189', 'A282', 'A285', 'A288', 'A291','A292', 'A293', 'A294', 'A295', 'A296', 'A297', 'A298', 'A299', 'A300', 
                 'A301', 'A302', 'A304', 'A305', 'A312', 'A314', 'A316', 'A317', 'A318', 'A319', 'A321', 'A322', 'A323', 'A324', 'A325', 'A326', 'A327', 'A328', 'A331', 
                 'A332', 'A333', 'MSINS')
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 2 
    and mht.Code = 'WW2'

--------------------------------------------------Class---------------------------------------------------------------------------------------------------------

UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'A107'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'A107'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'A168'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'A168'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and si.category = 51
    and n.SKU = 'A186'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'A186'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'A187'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'A187'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'A188'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'A188'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'A189'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'A189'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'A282'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'A282'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'A285'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'A285'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'A288'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'A288'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'A291'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'A291'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'A292'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'A292'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'A293'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'A293'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'A294'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'A294'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'A295'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'A295'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'A296'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'A296'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'A297'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'A297'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'A298'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'A298'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'A299'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'A299'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'A300'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'A300'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'A301'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'A301'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'A302'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'A302'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'A304'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'A304'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'A305'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'A305'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'A312'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'A312'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'A314'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'A314'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'A316'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'A316'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'A317'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'A317'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'A318'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'A318'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'A319'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'A319'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'A321'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'A321'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'A322'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'A322'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'A323'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'A323'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'A324'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'A324'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'A325'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'A325'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'A326'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'A326'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'A327'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'A327'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'A327'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'A327'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'A328'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'A328'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'A331'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'A331'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'A332'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'A332'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'A333'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'A333'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'MSINS'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'MSI'


--***************************************************Ready Assist******************************************************

----------------------------------------------------Division-----------------------------------------------------------

UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU in ('READY2S', 'READY2', 'READY3S', 'READY3', 'READY1S', 'READ1S', 'READY1', 'READS1', 'READ2S')
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 1 
    and mht.Code = 'W'

----------------------------------------------------Department-----------------------------------------------------------

UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU in ('READY2S', 'READY2', 'READY3S', 'READY3', 'READY1S', 'READ1S', 'READY1', 'READS1', 'READ2S')
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 2 
    and mht.Code = 'WW1'


----------------------------------------------------Class-----------------------------------------------------------

UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU in ('READY2S', 'READ2S')
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'RY2S'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'READY2'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'RY2'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU in ('READY3S', 'READ3S')
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'RY3S'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'READY3'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'RY3'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU in ('READY1S', 'READ1S', 'READS1')
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'RY1S'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'READY1'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'RY1'


--********************************************Service Generic*******************************************************************

----------------------------------------------Division--------------------------------------------------------------------------

UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU in ('7C', '7L001', '7N')
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 1 
    and mht.Code = 'S'


----------------------------------------------Department--------------------------------------------------------------------------

UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU in ('7C', '7L001', '7N')
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 2 
    and mht.Code = 'SS1'


----------------------------------------------Class--------------------------------------------------------------------------

UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = '7C'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'S1A'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = '7L001'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'S1C'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = '7N'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'S2A'


--*********************************************************Charges*************************************************

-----------------------------------------------------------Division------------------------------------------------

UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU in ('ADDCR', 'ADDDR', 'DT', 'RB', 'GTTCPT', 'ADMIN', 'DEL', 'STAX')
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 1 
    and mht.Code = 'V'


-----------------------------------------------------------Department------------------------------------------------

UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'ADMIN'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 2 
    and mht.Code = 'VV1'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU in ('ADDCR', 'ADDDR', 'DT', 'RB')
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 2 
    and mht.Code = 'VV2'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'DEL'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 2 
    and mht.Code = 'VV3'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'GTTCPT'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 2 
    and mht.Code = 'VV5'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'STAX'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 2 
    and mht.Code = 'VV6'


-----------------------------------------------------------Class------------------------------------------------

UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'ADDCR'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'V2B'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'ADDDR'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'V2C'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'DT'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'V2D'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'RB'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'V2A'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'GTTCPT'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'V5A'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'ADMIN'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'V1A'
    

UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'DEL'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'V3A'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'STAX'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'V6A'


--************************************************Miscellaneous**********************************************************

--------------------------------------------------Division----------------------------------------------------------------

UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU in ('GIFT', 'GIFTWRAP', 'ABM', 'ACME', 'CINT', 'CITYC', 'GTTCELL', 'SOLUT', 'EASY', 'GOLD', 'INS', 'STARR')
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 1 
    and mht.Code = 'V'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU in ('GIFT', 'GIFTWRAP', 'ABM', 'ACME', 'CINT', 'CITYC', 'GTTCELL', 'SOLUT', 'EASY', 'GOLD', 'INS', 'STARR')
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 2 
    and mht.Code = 'VV4'
  
--------------------------------------------------Class----------------------------------------------------------------  

UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'GIFT'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'V4A'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'GIFTWRAP'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'V4C'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'ABM'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'V4D'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'ACME'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'V4E'
 
    
UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'CINT'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'V4F'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'CITYC'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'V4G'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'GTTCELL'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'V4H'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'SOLUT'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'V4I'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'EASY'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'V4J'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'GOLD'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'V4K'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'INS'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'V4L'


UPDATE
    NonStocks.NonStockHierarchy
SET
    LevelKey = mht.Id,
    LevelName = mht.Name
FROM
    NonStocks.NonStock n
INNER JOIN
    StockInfo si on n.SKU = si.SKU
    and n.SKU = 'STARR'
INNER JOIN
    NonStocks.NonStockHierarchy nh on nh.NonStockId = n.Id
INNER JOIN
    Merchandising.HierarchyLevel mhl on nh.[Level] = mhl.Id
INNER JOIN 
    Merchandising.HierarchyTag mht on mht.LevelId = mhl.Id
WHERE
    nh.[Level] = 3 
    and mht.Code = 'V4M'


























