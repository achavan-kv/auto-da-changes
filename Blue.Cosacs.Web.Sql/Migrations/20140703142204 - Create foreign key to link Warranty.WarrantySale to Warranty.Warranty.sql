-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

UPDATE warranty.WarrantySale
SET WarrantyNumber = Data.WarrantyNumber
FROM
(
	SELECT 
		warranty.contractno,
		Warranty.itemid,
		warrantyStock.itemno AS WarrantyNumber,
		warrantystock.taxrate,
		warrantystock.costprice,
		warrantystock.unitpricecash,
		warranty.price,
		warranty.parentitemid,
		warranty.parentlocation,
		warranty.agrmtno,
		warranty.acctno,
		warrantyband.firstYearWarPeriod,
		warrantyband.warrantylength
	FROM   
		lineitem warranty
		INNER JOIN stockitem warrantyStock 
			ON warranty.itemid = warrantyStock.id
			AND warrantyStock.stocklocn = warranty.stocklocn
			AND warrantyStock.category IN ( '12','82' )
		LEFT OUTER JOIN warrantyband 
			ON warranty.itemid = warrantyband.itemid
) AS Data
WHERE
	Warranty.WarrantySale.CustomerAccount = Data.acctno
	AND Warranty.WarrantySale.AgreementNumber = Data.agrmtno
	AND Warranty.WarrantySale.WarrantyContractNo = Data.contractno
SELECT   				   
	item.stocklocn                                 AS SaleBranch,
	acct.acctno                                    AS CustomerAccount,
	custacct.custid                                AS customerId,
	customer.title                                 AS customerTitle,
	customer.firstname                             AS CustomerFirstName,
	customer.name                                  AS CustomerLastName,
	item.itemid                                    AS ItemId,
	itemStock.CostPrice                            AS ItemCostPrice,
	item.stocklocn                                 AS StockLocation,
	itemStock.itemno                               AS ItemNumber,
	itemStock.iupc                                 AS ItemUPC,
	itemstock.unitpricecash                        AS ItemPrice,
	itemStock.itemdescr1                           AS [Description],
	null											  AS Model,
	itemstock.brand                                AS itemBrand,
	itemstock.supplier                             AS itemSupplier,
	WarrantyTable.contractno + 'M'                 AS WarrantyContractNo,
	WarrantyTable.itemno                           AS WarrantyNumber,
	Isnull(WarrantyTable.firstYearWarPeriod, 12)   AS WarrantyLength,
	ISNULL(WarrantyTable.taxrate,0)                AS WarrantyTaxRate,
	1                                              AS WarrantyIsFree,
	ISNULL(WarrantyTable.costprice,0)              AS WarrantCostPrice,
	ISNULL(WarrantyTable.unitpricecash,0)          AS WarrantyRetailPrice,
	ISNULL(WarrantyTable.price,0)                  AS WarrantySalePrice,
	'Active'                                       AS Status,
	IDENTITY(int,1,1) as LineItemIdentifier,
	item.quantity as quantity,
	warrantyTable.ItemID as warrantyId,
	item.agrmtno AS AgreementNumber 
INTO #tempWarrantySale
FROM   
	lineitem item
    INNER JOIN acct 
		ON acct.acctno = item.acctno
    INNER JOIN custacct 
		ON item.acctno = custacct.acctno
		AND hldorjnt = 'H'
    INNER JOIN customer 
		ON customer.custid = custacct.custid
    LEFT OUTER JOIN 
	(
		SELECT 
			warranty.contractno,
            Warranty.itemid,
            warrantyStock.itemno,
            warrantystock.taxrate,
            warrantystock.costprice,
            warrantystock.unitpricecash,
            warranty.price,
            warranty.parentitemid,
            warranty.parentlocation,
            warranty.agrmtno,
            warranty.acctno,
			warrantyband.firstYearWarPeriod,
			warrantyband.warrantylength
		FROM   
			lineitem warranty
			INNER JOIN stockitem warrantyStock 
				ON warranty.itemid = warrantyStock.id
				AND warrantyStock.stocklocn = warranty.stocklocn
				AND warrantyStock.category IN ( '12','82' )
				AND warranty.itemid = warrantyStock.id
                LEFT OUTER JOIN warrantyband ON warranty.itemid = warrantyband.itemid
		WHERE warranty.quantity > 0
	)   AS WarrantyTable
		ON WarrantyTable.parentitemid = item.itemid
        AND WarrantyTable.parentlocation = item.stocklocn
        AND WarrantyTable.agrmtno = item.agrmtno
        AND WarrantyTable.acctno = item.acctno
	INNER JOIN stockitem itemStock 
		ON item.itemid = itemStock.id
        AND itemStock.stocklocn = item.stocklocn
        AND item.itemtype = 'S'

UPDATE ws
SET ws.ItemNumber = t.ItemNumber,
	ws.WarrantyNumber = t.WarrantyNumber
FROM Warranty.WarrantySale ws
INNER JOIN #tempWarrantySale t
	ON ws.CustomerAccount = t.CustomerAccount
	AND ws.ItemId = t.ItemId
	AND ws.WarrantyContractNo = t.WarrantyContractNo
	AND ws.StockLocation = t.StockLocation
	AND ws.AgreementNumber = t.AgreementNumber
	AND ws.SaleBranch = t.SaleBranch
	AND ws.CustomerId = t.customerId

UPDATE ws
SET ws.WarrantyId = w.Id
FROM 
	Warranty.WarrantySale ws
	INNER JOIN Warranty.Warranty w
		ON ws.WarrantyNumber = w.Number

INSERT INTO Warranty.Warranty
(Number, Description, Length, TaxRate, Deleted, TypeCode)
SELECT DISTINCT 
	s.WarrantyNumber,
	s.WarrantyNumber AS Description,
	24 AS Length,
	0.0 AS TaxRate,
	1 AS Deleted,
	'E' AS TypeCode
FROM 
	Warranty.WarrantySale s 
	LEFT JOIN Warranty.Warranty w 
		ON s.WarrantyId = w.Id
WHERE 
	w.id is null
	AND s.WarrantyNumber NOT IN
	(
		SELECT Number FROM Warranty.Warranty
	)

UPDATE ws
SET ws.WarrantyId = w.Id
FROM 
	Warranty.WarrantySale ws
	INNER JOIN Warranty.Warranty w
		ON ws.WarrantyNumber = w.Number

IF OBJECT_ID('[Warranty].[FK_WarrantySale_Warranty]') IS NULL
	ALTER TABLE Warranty.WarrantySale
	ADD CONSTRAINT FK_WarrantySale_Warranty
	FOREIGN KEY (WarrantyId)
	REFERENCES Warranty.Warranty(Id)
	ON UPDATE NO ACTION
	ON DELETE NO ACTION
