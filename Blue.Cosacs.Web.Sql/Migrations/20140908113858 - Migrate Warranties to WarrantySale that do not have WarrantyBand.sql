-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

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
						   item.itemid                                    AS ItemId,
						   Item.itemno                                    AS ItemNumber,
						   itemStock.iupc                                 AS ItemUPC,
						   itemstock.unitpricecash                        AS ItemPrice,
						   itemStock.itemdescr1                           AS [Description],
						   itemstock.brand                                AS itemBrand,
						   null											  AS Model,
						   itemstock.supplier                             AS itemSupplier,
						   WarrantyTable.contractno + 'M'                 AS WarrantyContractNo,
						   ww.id										  AS warrantyId, 
						   WarrantyTable.itemno                           AS WarrantyNumber,
						   Isnull(WarrantyTable.firstYearWarPeriod, 12)   AS WarrantyLength,
						   ISNULL(WarrantyTable.taxrate,0)                AS WarrantyTaxRate,
						   ISNULL(WarrantyTable.costprice,0)              AS WarrantCostPrice,
						   ISNULL(WarrantyTable.unitpricecash,0)          AS WarrantyRetailPrice,
						   ISNULL(WarrantyTable.price,0)                  AS WarrantySalePrice,
						   'Active'                                       AS Status,
						   null											  AS ItemSerialNumber,
						   item.stocklocn                                 AS StockLocation,
						   custaddress.Notes						      AS CustomerNotes,
						   itemStock.CostPrice                            AS ItemCostPrice,
						   del.datedel                                    AS ItemDeliveredOn,
						   IDENTITY(int,1,1)							  AS WarrantyGroupId,
						   saleperson.Id							      AS SoldById,
						   Item.quantity								  AS ItemQuantity, 
						   del.datedel									  AS EffectiveDate,
						   WarrantyTable.datedel						  AS WarrantyDeliveredOn,
						   'F'                                            AS WarrantyType,
						   agreement.agrmtno							  AS AgreementNumber
						  
	
INTO	
	#tempWarrantySale
FROM   
	lineitem item
INNER JOIN 
	acct ON acct.acctno = item.acctno
INNER JOIN 
	agreement ON agreement.acctno = item.acctno
    AND agreement.agrmtno = item.agrmtno
INNER JOIN 
	custacct ON item.acctno = custacct.acctno
    AND hldorjnt = 'H'
INNER JOIN 
	customer ON customer.custid = custacct.custid
INNER JOIN 
	admin.[user] saleperson ON saleperson.id = agreement.empeenosale
INNER JOIN 
	del on item.ItemID = del.ItemID 
	AND item.acctno = del.acctno 
	AND item.agrmtno = del.agrmtno 
	AND item.stocklocn = del.stocklocn
	AND item.contractno = del.contractno
LEFT OUTER JOIN 
	custaddress ON custaddress.custid = custacct.custid
    AND custaddress.datemoved IS NULL
    AND custaddress.addtype = 'H'
LEFT OUTER JOIN 
	(SELECT warranty.contractno,
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
							   12 AS firstYearWarPeriod,
							   cast(RIGHT(Warranty.itemno, 1) as int) * 12 AS warrantylength,
							   del.datedel
                          FROM   
								lineitem warranty
						  INNER JOIN 
								del on warranty.ItemID = del.ItemID 
											  AND warranty.acctno = del.acctno 
											  AND warranty.agrmtno = del.agrmtno 
											  AND warranty.stocklocn = del.stocklocn
											  AND warranty.contractno = del.contractno
							INNER JOIN 
								stockitem warrantyStock ON warranty.itemid = warrantyStock.id
										AND warrantyStock.stocklocn = warranty.stocklocn
										AND warrantyStock.category IN ( '12','82' )
										AND warranty.itemid = warrantyStock.id
                            INNER JOIN 
								stockprice warrantyPrice ON warranty.ItemID = warrantyPrice.ID			
							    AND warrantyPrice.branchno = warranty.stocklocn																
						 where warranty.quantity > 0
								AND warranty.ordval > 0
								AND NOT EXISTS(select * from warrantyband wb
												where wb.ItemID = warranty.ItemID))   AS WarrantyTable
                    ON WarrantyTable.parentitemid = item.itemid
                       AND WarrantyTable.parentlocation = item.stocklocn
                       AND WarrantyTable.agrmtno = item.agrmtno
                       AND WarrantyTable.acctno = item.acctno
INNER JOIN 
	stockitem itemStock ON item.itemid = itemStock.id
    AND itemStock.stocklocn = item.stocklocn
    AND item.itemtype = 'S'
INNER JOIN 
	warranty.warranty ww on ww.Number = WarrantyTable.itemno
WHERE  NOT EXISTS (SELECT 1
                   FROM   cancellation
                   WHERE  cancellation.acctno = acct.acctno)
        AND WarrantyTable.itemid IS NOT NULL AND Dateadd(month, WarrantyTable.warrantylength, agreement.dateagrmt) > Getdate()	
AND NOT EXISTS(select * from warranty.warrantysale ws
					where ws.CustomerAccount = item.acctno
					and ws.AgreementNumber = item.agrmtno
					and ws.ItemId = item.ItemID
					and ws.StockLocation = item.stocklocn)	
		

--Select the missing free warranties (that did not have warrantyband) into warranty.warranty
select distinct WarrantyNumber + 'M' as FreeWarrantyNumber, WarrantyNumber as ExtendedWarrantyNumber, 'Free ' + w.[Description] as FreeWarrantyDescription
into #freewarranties
from #tempWarrantySale t
inner join warranty.warranty w on t.WarrantyNumber = w.Number

insert into warranty.warranty
select f.FreeWarrantyNumber, f.FreeWarrantyDescription, 12, 0, 0, 'F'
from #freewarranties f


--First insert the extended
insert into warranty.warrantysale
select 
		t.invoiceNumber,
		t.SaleBranch,
		t.SoldOn,
		t.SoldBy,
		t.CustomerAccount,
		t.customerId,
		t.customerTitle,
		t.CustomerFirstName,
		t.CustomerLastName,
		t.CustomerAddressLine1,
		t.CustomerAddressLine2,
		t.CustomerAddressLine3,
		t.CustomerPostCode,
		t.ItemId,
		t.ItemNumber,
		t.ItemUPC,
		t.ItemPrice,
		t.[Description],
		t.itemBrand,
		t.Model,
		t.itemSupplier,
		left(t.WarrantyContractNo, len(t.WarrantyContractNo) - 1),
		t.warrantyId, 
		t.WarrantyNumber,
		(cast(RIGHT(t.WarrantyNumber, 1) as int) * 12) - 12,
		t.WarrantyTaxRate,
		t.WarrantCostPrice,
		t.WarrantyRetailPrice,
		t.WarrantySalePrice,
		t.[Status],
		t.ItemSerialNumber,
		t.StockLocation,
		t.CustomerNotes,
		t.ItemCostPrice,
		t.ItemDeliveredOn,
		t.WarrantyGroupId,
		t.SoldById,
		t.ItemQuantity, 
		Dateadd(month, 12, t.EffectiveDate),
		t.WarrantyDeliveredOn,
		'E',
		t.AgreementNumber
from #tempWarrantySale t

--Finally insert the free warranties
update #tempwarrantysale
set WarrantyNumber = wf.Number,
WarrantyId = wf.Id
from #freewarranties f
inner join warranty.warranty wf on f.FreeWarrantyNumber = wf.Number
where f.ExtendedWarrantyNumber = #tempwarrantysale.WarrantyNumber

insert into warranty.warrantysale
select * from #tempWarrantySale;