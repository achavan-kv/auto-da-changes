;WITH del (datedel, ParentItemID, ItemID, itemno, acctno, agrmtno, stocklocn, contractno)
AS (
    SELECT max(d.datedel), ParentItemID, ItemID, itemno, acctno, agrmtno, stocklocn, contractno
    FROM delivery d
    WHERE (
        -- only migrate to WarrantySale warranties for accounts where all sold warranties were later returned or repossessed
        SELECT sum(d2.quantity)
        FROM delivery d2
        INNER JOIN StockInfo i
            ON i.Id = d2.ItemID
            AND i.category IN (12, 82)
        WHERE d2.ItemID = d.ItemID
            --AND d2.ParentItemID = d.ParentItemID
            AND d2.acctno = d.acctno
            AND d2.agrmtno = d.agrmtno
            AND d2.stocklocn = d.stocklocn
            AND d2.contractno = d.contractno)=0
    AND delorcoll = 'D'
    GROUP BY ParentItemID, ItemID, itemno, acctno, agrmtno, stocklocn, contractno
)


-- create warranties to insert on WarrantySale
SELECT DISTINCT
    acct.acctno + ' ' + CONVERT(VARCHAR, agreement.agrmtno) AS InvoiceNumber,
    item.stocklocn                                          AS SaleBranch,
    agreement.dateagrmt                                     AS SoldOn,
    saleperson.fullname                                     AS SoldBy,
    acct.acctno                                             AS CustomerAccount,
    custacct.custid                                         AS customerId,
    customer.title                                          AS customerTitle,
    customer.firstname                                      AS CustomerFirstName,
    customer.name                                           AS CustomerLastName,
    custaddress.cusaddr1                                    AS CustomerAddressLine1,
    custaddress.cusaddr2                                    AS CustomerAddressLine2,
    custaddress.cusaddr3                                    AS CustomerAddressLine3,
    custaddress.cuspocode                                   AS CustomerPostCode,
    item.itemid                                             AS ItemId,
    Item.itemno                                             AS ItemNumber,
    i.iupc                                                  AS ItemUPC,
    i.unitpricecash                                         AS ItemPrice,
    i.itemdescr1                                            AS ItemDescription,
    i.brand                                                 AS ItemBrand,
                                                            -- ItemModel,
    i.supplier                                              AS itemSupplier,
    CASE
        WHEN gen.WarType='F' THEN  WarrantyTable.contractno + 'M' -- free
        WHEN gen.WarType='E' THEN  WarrantyTable.contractno       -- extended
    END                                                     AS WarrantyContractNo,
    -- this null WarrantyId will be fixed in the next migration
    NULL /*warrantyTable.ItemID*/                           AS WarrantyId,
    WarrantyTable.itemno                                    AS WarrantyNumber,
    CASE
        WHEN gen.WarType='F' THEN WarrantyTable.firstYearWarPeriod
        WHEN gen.WarType='E' THEN WarrantyTable.warrantylength
    END                                                     AS WarrantyLength,
    ISNULL(WarrantyTable.taxrate, 0)                        AS WarrantyTaxRate,
    ISNULL(WarrantyTable.costprice, 0)                      AS WarrantyCostPrice,
    ISNULL(WarrantyTable.unitpricecash, 0)                  AS WarrantyRetailPrice,
    ISNULL(WarrantyTable.price, 0)                          AS WarrantySalePrice,
    'Cancelled'                                                AS [Status],
                                                            -- ItemSerialNumber,
    item.stocklocn                                          AS StockLocation,
    custaddress.Notes                                       AS CustomerNotes,
    i.CostPrice                                             AS ItemCostPrice,
    del.datedel                                             AS ItemDeliveredOn,
    IDENTITY(int, 1, 1)                                     AS WarrantyGroupId,
    saleperson.Id                                           AS SoldById,
    item.quantity                                           AS ItemQuantity,
    del.datedel                                             AS EffectiveDate,
    del.datedel                                             AS WarrantyDeliveredOn,
    gen.WarType                                             AS WarrantyType,
    del.agrmtno                                             AS AgreementNumber
INTO #tempWarrantySale
FROM lineitem item
INNER JOIN acct
    ON acct.acctno = item.acctno
INNER JOIN agreement
    ON agreement.acctno = item.acctno
    AND agreement.agrmtno = item.agrmtno
INNER JOIN custacct
    ON item.acctno = custacct.acctno
    AND hldorjnt = 'H'
INNER JOIN customer
    ON customer.custid = custacct.custid
INNER JOIN admin.[user] saleperson
    ON saleperson.id = agreement.empeenosale
INNER JOIN del
    ON item.ItemID = del.ItemID
    --AND item.ParentItemID = del.ParentItemID
    AND item.acctno = del.acctno 
    AND item.agrmtno = del.agrmtno 
    AND item.stocklocn = del.stocklocn
    AND item.contractno = del.contractno
INNER JOIN (
    SELECT
        warrantyLine.contractno,
        warrantyLine.itemid,
        warrantyLine.itemno,
        warrantystock.taxrate,
        warrantystock.costprice,
        warrantystock.unitpricecash,
        warrantyLine.price,
        warrantyLine.parentitemid,
        warrantyLine.parentlocation,
        warrantyLine.stocklocn,
        warrantyLine.agrmtno,
        warrantyLine.acctno,
        warrantyband.firstYearWarPeriod,
        warrantyband.warrantylength
    FROM lineitem warrantyLine
    INNER JOIN del
        ON warrantyLine.ItemID = del.ItemID
        --AND warrantyLine.ParentItemID = del.ParentItemID
        AND warrantyLine.acctno = del.acctno
        AND warrantyLine.agrmtno = del.agrmtno
        AND warrantyLine.stocklocn = del.stocklocn
        AND warrantyLine.contractno = del.contractno
    INNER JOIN stockitem warrantyStock
        ON warrantyStock.id = warrantyLine.itemid
        AND warrantyStock.stocklocn = warrantyLine.stocklocn
        AND warrantyStock.category IN ('12','82')
    INNER JOIN warrantyband
        ON warrantyLine.itemid = warrantyband.itemid
) AS WarrantyTable
    ON WarrantyTable.ItemID = item.itemid
    --AND WarrantyTable.ParentItemID = item.ParentItemID
    AND WarrantyTable.stocklocn = item.stocklocn
    AND WarrantyTable.agrmtno = item.agrmtno
    AND WarrantyTable.acctno = item.acctno
    AND WarrantyTable.contractno = item.contractno
LEFT JOIN custaddress
    ON custaddress.custid = custacct.custid
    AND custaddress.datemoved IS NULL
    AND custaddress.addtype = 'H'
LEFT JOIN stockitem i ON item.itemid = i.id
    AND i.stocklocn = item.stocklocn
CROSS JOIN ( --warranty type generator
    --duplicate each line, to create Free/Extended warranty pairs
    SELECT 'F' as WarType
    union
    SELECT 'E' as WarType
) gen
LEFT JOIN Warranty.WarrantySale warSale
    ON del.acctno=warSale.CustomerAccount
    AND del.contractno=warSale.WarrantyContractNo
WHERE warSale.CustomerAccount IS NULL -- eliminate records already on Warranty.WarrantySale


INSERT INTO Warranty.WarrantySale (
    InvoiceNumber,
    SaleBranch,
    SoldOn,
    SoldBy,
    CustomerAccount,
    customerId,
    customerTitle,
    CustomerFirstName,
    CustomerLastName,
    CustomerAddressLine1,
    CustomerAddressLine2,
    CustomerAddressLine3,
    CustomerPostCode,
    ItemId,
    ItemNumber,
    ItemUPC,
    ItemPrice,
    ItemDescription,
    ItemBrand,
    itemSupplier,
    WarrantyContractNo,
    WarrantyId,
    WarrantyNumber,
    WarrantyLength,
    WarrantyTaxRate,
    WarrantyCostPrice,
    WarrantyRetailPrice,
    WarrantySalePrice,
    [Status],
    StockLocation,
    CustomerNotes,
    ItemCostPrice,
    ItemDeliveredOn,
    WarrantyGroupId,
    SoldById,
    ItemQuantity,
    EffectiveDate,
    WarrantyDeliveredOn,
    WarrantyType,
    AgreementNumber
)
SELECT
    t.InvoiceNumber,
    SaleBranch,
    SoldOn,
    SoldBy,
    CustomerAccount,
    customerId,
    customerTitle,
    CustomerFirstName,
    CustomerLastName,
    CustomerAddressLine1,
    CustomerAddressLine2,
    CustomerAddressLine3,
    CustomerPostCode,
    t.ItemId,
    ItemNumber,
    ItemUPC,
    ItemPrice,
    ItemDescription,
    ItemBrand,
    itemSupplier,
    WarrantyContractNo,
    WarrantyId,
    WarrantyNumber,
    t.WarrantyLength,
    WarrantyTaxRate,
    WarrantyCostPrice,
    WarrantyRetailPrice,
    WarrantySalePrice,
    [Status],
    t.StockLocation,
    CustomerNotes,
    ItemCostPrice,
    ItemDeliveredOn,
    WarrantyGroupId,
    SoldById,
    ItemQuantity,
    EffectiveDate,
    WarrantyDeliveredOn,
    WarrantyType,
    AgreementNumber
FROM #tempWarrantySale t

IF EXISTS (select 1 from HiLo where Sequence = 'WarrantySaleLineItemIdentifer')
BEGIN
    UPDATE hilo
    SET NextHi = (
        SELECT max(sub.NextHiVal)
        FROM (
            select max(WarrantyGroupId) as NextHiVal
            from Warranty.WarrantySale
            UNION
            select NextHi as NextHiVal
            from HiLo
            where HiLo.Sequence = 'WarrantySaleLineItemIdentifer'
        ) sub
    )
    WHERE hilo.Sequence = 'WarrantySaleLineItemIdentifer'
END

drop table #tempWarrantySale
go
