IF EXISTS (Select 1 from sys.objects
		  where name = 'WarrantySaleGenerateView')
BEGIN
	DROP VIEW WarrantySaleGenerateView
END
GO          

CREATE VIEW WarrantySaleGenerateView
AS
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
						   branch.branchname							  AS SaleBranchName,
						   agreement.dateagrmt                            AS SoldOn,
						   saleperson.fullname                            AS SoldBy,
						   saleperson.Id                                  AS SoldById,
						   acct.acctno                                    AS CustomerAccount,
						   custacct.custid                                AS customerId,
						   customer.title                                 AS customerTitle,
						   customer.firstname                             AS CustomerFirstName,
						   customer.name                                  AS CustomerLastName,
						   isnull(custaddress.cusaddr1,'')                AS CustomerAddressLine1,
						   isnull(custaddress.cusaddr2,'')                AS CustomerAddressLine2,
						   isnull(custaddress.cusaddr3,'')                AS CustomerAddressLine3,
						   isnull(custaddress.cuspocode,'')               AS CustomerPostCode,
						   isnull(custaddress.Notes,'')				      AS CustomerNotes,
						   item.itemid                                    AS ItemId,
						   itemStock.CostPrice                            AS ItemCostPrice,
						   item.stocklocn                                 AS StockLocation,
						   Item.itemno                                    AS ItemNumber,
						   itemStock.iupc                                 AS ItemUPC,
						   --itemstock.unitpricecash                        AS ItemPrice,
						   item.price									  AS ItemPrice,			-- #18990
						   itemStock.itemdescr1                           AS [Description],
						   null											  AS Model,
						   itemstock.brand                                AS itemBrand,
						   itemstock.supplier                             AS itemSupplier,
						   agreement.agrmtno as AgreementNumber,
						   home.telno as homePhone, 
						   work.telno as workPhone, 
						   mobile.telno as mobilePhone, 
						   custaddress.Email as Email,
						   item.quantity as Quantity,
						   acct.accttype AS AccountType,
						   c.category AS Department
FROM   lineitem item
	   INNER JOIN branch ON item.stocklocn = branch.branchno
       INNER JOIN acct ON acct.acctno = item.acctno
       INNER JOIN agreement ON agreement.acctno = item.acctno
                  AND agreement.agrmtno = item.agrmtno
       INNER JOIN custacct ON item.acctno = custacct.acctno
                  AND hldorjnt = 'H'
       INNER JOIN customer ON customer.custid = custacct.custid
       INNER JOIN admin.[user] saleperson ON saleperson.id = agreement.empeenosale
       LEFT OUTER JOIN custaddress ON custaddress.custid = custacct.custid
                       AND custaddress.datemoved IS NULL
                       AND custaddress.addtype = 'H'
        INNER JOIN stockitem itemStock ON item.itemid = itemStock.id
                  AND itemStock.stocklocn = item.stocklocn
                  AND item.itemtype = 'S'
		LEFT JOIN Code c
			ON itemStock.category = CONVERT(Int, c.code)
			AND c.category IN ('PCE', 'PCF', 'PCO', 'PCW', 'PCDIS')
		LEFT JOIN custtel home on home.custid = customer.custid 
		                               AND home.datediscon is null 
									   AND home.tellocn = 'H'
		LEFT JOIN custtel work on work.custid = customer.custid
								  AND work.datediscon is null 
								  AND work.tellocn = 'W'
		LEFT JOIN custtel mobile on mobile.custid = customer.custid
								  AND mobile.datediscon is null 
								  AND mobile.tellocn = 'M'
WHERE  NOT EXISTS (SELECT 1
                   FROM   cancellation
                   WHERE  cancellation.acctno = acct.acctno)

go

