delete from [Warranty].[WarrantyContact]
GO

delete from  warranty.warrantysale
GO

ALTER TABLE Warranty.WarrantySale
ALTER Column LineitemIdentifier int
GO

WITH del (datedel,Itemid,acctno,agrmtno,stocklocn,contractno)
AS
(
	select max(datedel),Itemid,acctno,agrmtno,stocklocn,contractno
	from delivery 
	where (Select sum(quantity) 
			from delivery d2 
			where d2.ItemID = delivery.ItemID 
				AND d2.acctno = delivery.acctno 
				AND d2.agrmtno = delivery.agrmtno 
				AND d2.stocklocn = delivery.stocklocn
				AND d2.contractno = delivery.contractno) > 0
	AND delorcoll = 'D'
	group by Itemid,acctno,agrmtno,stocklocn,contractno 	
)
SELECT acct.acctno + ' ' + CONVERT(VARCHAR, agreement.agrmtno)            AS invoiceNumber,
						   item.stocklocn                                 AS SaleBranch,
						   agreement.dateagrmt                            AS SoldOn,
						   saleperson.fullname                            AS SoldBy,
						   acct.acctno                                    AS CustomerAccount,
						   custacct.custid                                AS customerId,
						   customer.title                                 AS customerTitle,
						   customer.firstname                             AS CustomerFirstName,
						   customer.name                                  AS CustomerLastName,
						   custaddress.cusaddr1                           AS CustomerAddressLine1,
						   custaddress.cusaddr2                           AS CustomerAddressLine2,
						   custaddress.cusaddr3                           AS CustomerAddressLine3,
						   custaddress.cuspocode                          AS CustomerPostCode,
						   custaddress.Notes						      AS CustomerNotes,
						   item.itemid                                    AS ItemId,
						   itemStock.CostPrice                            AS ItemCostPrice,
						   item.stocklocn                                 AS StockLocation,
						   del.datedel                                    AS ItemDeliveredOn,
						   Item.itemno                                    AS ItemNumber,
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
	   warrantyTable.ItemID as warrantyId 
INTO #tempWarrantySale
FROM   lineitem item
       INNER JOIN acct ON acct.acctno = item.acctno
       INNER JOIN agreement ON agreement.acctno = item.acctno
                  AND agreement.agrmtno = item.agrmtno
       INNER JOIN custacct ON item.acctno = custacct.acctno
                  AND hldorjnt = 'H'
       INNER JOIN customer ON customer.custid = custacct.custid
       INNER JOIN admin.[user] saleperson ON saleperson.id = agreement.empeenosale
	   INNER JOIN del on item.ItemID = del.ItemID 
	                          AND item.acctno = del.acctno 
							  AND item.agrmtno = del.agrmtno 
							  AND item.stocklocn = del.stocklocn
							  AND item.contractno = del.contractno
       LEFT OUTER JOIN custaddress ON custaddress.custid = custacct.custid
                       AND custaddress.datemoved IS NULL
                       AND custaddress.addtype = 'H'
       LEFT OUTER JOIN (SELECT warranty.contractno,
                               Warranty.itemid,
                               Warranty.itemno,
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
                          FROM   lineitem warranty
						  INNER JOIN del on warranty.ItemID = del.ItemID 
											  AND warranty.acctno = del.acctno 
											  AND warranty.agrmtno = del.agrmtno 
											  AND warranty.stocklocn = del.stocklocn
											  AND warranty.contractno = del.contractno
							INNER JOIN stockitem warrantyStock ON warranty.itemid = warrantyStock.id
										AND warrantyStock.stocklocn = warranty.stocklocn
										AND warrantyStock.category IN ( '12','82' )
										AND warranty.itemid = warrantyStock.id
                            INNER JOIN stockprice warrantyPrice ON warranty.ItemID = warrantyPrice.ID			
							   AND warrantyPrice.branchno = warranty.stocklocn									
                            LEFT OUTER JOIN warrantyband ON warranty.itemid = warrantyband.itemid
							   AND warrantyband.refcode = warrantyPrice.Refcode									
							   where warranty.quantity > 0)   AS WarrantyTable
                    ON WarrantyTable.parentitemid = item.itemid
                       AND WarrantyTable.parentlocation = item.stocklocn
                       AND WarrantyTable.agrmtno = item.agrmtno
                       AND WarrantyTable.acctno = item.acctno
       INNER JOIN stockitem itemStock ON item.itemid = itemStock.id
                  AND itemStock.stocklocn = item.stocklocn
                  AND item.itemtype = 'S'
WHERE  NOT EXISTS (SELECT 1
                   FROM   cancellation
                   WHERE  cancellation.acctno = acct.acctno)
        AND ( WarrantyTable.itemid IS NOT NULL AND Dateadd(month, isnull(WarrantyTable.warrantylength,12), agreement.dateagrmt) > Getdate()	
              OR ( WarrantyTable.itemid IS NULL AND Dateadd(month, 15, agreement.dateagrmt) > Getdate() ) )						
	
declare @maxId int

SELECT @maxId = max(LineItemIdentifier) 
from #tempWarrantySale

INSERT INTO  warranty.warrantysale
(
InvoiceNumber, 
SaleBranch,
SoldOn, SoldBy,
CustomerAccount,
CustomerId, CustomerTitle,
CustomerFirstName,
CustomerLastName,
CustomerAddressLine1,
CustomerAddressLine2,
CustomerAddressLine3,
CustomerPostcode,
CustomerNotes,
ItemId, 
ItemNumber,
ItemUPC,
ItemPrice,
ItemDescription,
ItemBrand,
ItemModel,
ItemSupplier,
WarrantyContractNo,
WarrantyNumber,
WarrantyLength,
WarrantyTaxRate,
WarrantyIsFree,
WarrantyCostPrice,
WarrantyRetailPrice, 
WarrantySalePrice, 
Status, 
StockLocation, 
ItemCostPrice, 
ItemDeliveredOn,
LineItemIdentifier      )
 
SELECT  invoiceNumber,
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
		 CustomerNotes,
		 ItemId,
		 ItemNumber,
		 ItemUPC,
		 ItemPrice,
		  [Description],
		   itemBrand,
		   Model,
		    itemSupplier, 
			WarrantyContractNo,
		 WarrantyNumber,
		 WarrantyLength,
		 WarrantyTaxRate,
		 WarrantyIsFree,
		 WarrantCostPrice,
		 WarrantyRetailPrice,
		 WarrantySalePrice,
		 Status,
		 StockLocation,
		  ItemCostPrice,
		 ItemDeliveredOn,
	     LineItemIdentifier 
FROM #tempWarrantySale;



WITH quant (quantity,invoiceNumber,StockLocation,ItemId)
AS
(
	select count(*) as quantity, invoiceNumber,StockLocation,ItemId
	from #tempWarrantySale
	group by invoiceNumber,StockLocation,ItemId
)
INSERT INTO  warranty.warrantysale
(
InvoiceNumber, 
SaleBranch,
SoldOn, SoldBy,
CustomerAccount,
CustomerId, CustomerTitle,
CustomerFirstName,
CustomerLastName,
CustomerAddressLine1,
CustomerAddressLine2,
CustomerAddressLine3,
CustomerPostcode,
CustomerNotes,
ItemId, 
ItemNumber,
ItemUPC,
ItemPrice,
ItemDescription,
ItemBrand,
ItemModel,
ItemSupplier,
WarrantyContractNo,
WarrantyNumber,
WarrantyLength,
WarrantyTaxRate,
WarrantyIsFree,
WarrantyCostPrice,
WarrantyRetailPrice, 
WarrantySalePrice, 
Status, 
StockLocation, 
ItemCostPrice, 
ItemDeliveredOn,
LineItemIdentifier      )
 
SELECT	 t.invoiceNumber,
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
		CustomerNotes,
		t.ItemId,
		ItemNumber,
		ItemUPC,
		ItemPrice,
		[Description],
		itemBrand,
		Model,
		itemSupplier, 
		WarrantyContractNo,
		WarrantyNumber,
		ISNULL(wb.warrantylength,12),
		WarrantyTaxRate,
		CASE WHEN warrantyContractNo IS NULL THEN 1 ELSE 0 END as WarrantyIsFree,
		WarrantCostPrice,
		WarrantyRetailPrice,
		WarrantySalePrice,
		Status,
		t.StockLocation,
		ItemCostPrice,
		ItemDeliveredOn,
		CASE WHEN warrantyContractNo IS NULL THEN ROW_NUMBER() over(order by LineItemIdentifier) + @MaxId
											ELSE LineItemIdentifier END as LineItemIdentifier
from #tempWarrantySale t
left join quant q on q.invoiceNumber = t.invoiceNumber 
				AND q.ItemId = t.ItemId
				AND q.Stocklocation = t.stocklocation
cross join stockinfo
left join stockprice sp on t.warrantyId = sp.ID and sp.branchno = t.StockLocation											
left join warrantyband wb on wb.ItemID = t.warrantyId 
and wb.refcode = sp.Refcode																									
where stockinfo.id <= t.quantity -  CASE WHEN warrantyContractNo IS NULL THEN 0 ELSE isnull(q.quantity,0) -1 END


if NOT exists (select 1 from HiLo
           where Sequence = 'WarrantySaleLineItemIdentifer')
BEGIN
	insert into hilo
	select 'WarrantySaleLineItemIdentifer', max(LineItemIdentifier),100
	FROM warranty.Warrantysale
END
ELSE
BEGIN 
	UPDATE hilo
	set NextHi = (select max(LineItemIdentifier) From warranty.WarrantySale	)
	where hilo.Sequence = 'WarrantySaleLineItemIdentifer'
END


drop table #tempWarrantySale
go
