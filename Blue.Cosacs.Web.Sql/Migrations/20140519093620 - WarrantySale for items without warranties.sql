-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF EXISTS (Select 1 from sys.objects
		  where name = 'WarrantySaleGenerateView')
BEGIN
	DROP VIEW WarrantySaleGenerateView
END
GO          

-- Fix for the migration (failed on the one click deployment tool) - Create the required view, because it's used on the migration
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
						   custaddress.cusaddr1                           AS CustomerAddressLine1,
						   custaddress.cusaddr2                           AS CustomerAddressLine2,
						   custaddress.cusaddr3                           AS CustomerAddressLine3,
						   custaddress.cuspocode                          AS CustomerPostCode,
						   custaddress.Notes						      AS CustomerNotes,
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

				   
GO

-- This is the original migration...
IF NOT EXISTS(SELECT 1 FROM [$Migration] WHERE id=20140519093617)
BEGIN

if not exists(select * from Warranty.Warranty where Number ='Manufacturer')
begin
	insert into Warranty.Warranty (Number, Description, Length, TaxRate, Deleted, TypeCode)
	values('Manufacturer','Dummy Manufacturer Warranty',12,0,1,'F')

end


INSERT INTO  warranty.warrantysale
(InvoiceNumber, 
SaleBranch, 
SoldOn, 
SoldBy, 
CustomerAccount, 
CustomerId, 
CustomerTitle, 
CustomerFirstName, 
CustomerLastName, 
CustomerAddressLine1, 
CustomerAddressLine2, 
CustomerAddressLine3, 
CustomerPostcode, 
ItemId, 
ItemNumber, 
ItemUPC, 
ItemPrice, 
ItemDescription, 
ItemBrand, 
ItemModel, 
ItemSupplier, 
WarrantyContractNo, 
WarrantyId, 
WarrantyNumber, 
WarrantyLength, 
WarrantyTaxRate, 
WarrantyCostPrice, 
WarrantyRetailPrice, 
WarrantySalePrice, 
Status, 
ItemSerialNumber,
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

select g.invoiceNumber,g.SaleBranch,g.SoldOn,g.SoldBy,g.CustomerAccount,g.customerId,g.customerTitle,g.CustomerFirstName,g.CustomerLastName,
		g.CustomerAddressLine1,g.CustomerAddressLine2,g.CustomerAddressLine3,g.CustomerPostCode,g.ItemId,g.ItemNumber,g.ItemUPC,g.ItemPrice,
		g.[Description],g.itemBrand,g.Model,g.itemSupplier, 'Not available',w.id,w.Number,w.Length,w.TaxRate,0.00,0.00,0.00,
		
		'Active',null,d.stocklocn,g.CustomerNotes,g.ItemCostPrice,d.datedel,1,g.SoldById,g.Quantity,
		d.datedel,d.datedel,'F',d.agrmtno



 from delivery d inner join stockinfo s on d.itemno=s.itemno and s.itemtype='S'
	inner join WarrantySaleGenerateView g on d.acctno=g.CustomerAccount and d.agrmtno=g.AgreementNumber and d.itemid=g.ItemId and d.stocklocn=g.StockLocation,
	Warranty.Warranty w 
where d.agrmtno>0
and not exists(select * from warranty.warrantysale ws where d.acctno=ws.CustomerAccount and d.itemno=ws.ItemNumber and d.agrmtno=ws.AgreementNumber)
and not exists(select * from delivery c where d.acctno=c.acctno and d.itemid=c.itemid and d.stocklocn=c.stocklocn and c.delorcoll in('C','R') and d.agrmtno=c.agrmtno)
and dateadd(month,12,d.datedel)> getdate()
and w.Number = 'Manufacturer'


END
