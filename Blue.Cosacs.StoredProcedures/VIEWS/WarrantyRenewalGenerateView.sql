
IF EXISTS (Select 1 from sys.objects
		  where name = 'WarrantyRenewalGenerateView')
BEGIN
	DROP VIEW WarrantyRenewalGenerateView
END
GO          

CREATE VIEW WarrantyRenewalGenerateView
AS
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
						   custaddress.cusaddr1                           AS CustomerAddressLine1,
						   custaddress.cusaddr2                           AS CustomerAddressLine2,
						   custaddress.cusaddr3                           AS CustomerAddressLine3,
						   custaddress.cuspocode                          AS CustomerPostCode,
						   custaddress.Notes						      AS CustomerNotes,
						   itemStock.ID                                   AS ItemId,
						   itemStock.CostPrice                            AS ItemCostPrice,
						   item.stocklocn                                 AS StockLocation,
						   itemStock.itemno                               AS ItemNumber,
						   itemStock.iupc                                 AS ItemUPC,
						   itemstock.unitpricecash                        AS ItemPrice,
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
						   CASE	
								WHEN acct.accttype = 'C' THEN 'BCW'
								ELSE 'BHW'
						   END AS AccountType,
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

        INNER JOIN stockitem itemStock ON item.parentitemid = itemStock.id
                  AND itemStock.stocklocn = item.parentlocation
                  AND item.itemtype = 'N'
		LEFT JOIN Code c
			ON itemStock.category = CONVERT(Int, c.code)
			AND c.category IN ('PCE', 'PCF')
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
