-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
DECLARE @Date Date	

/*I am doing the MIN because there may be multiples upgrades version on the same database*/
SELECT 
	@Date = MIN(d.upgrade_date)  
FROM 
	dbversion d
WHERE 
	version = 'Netv8_0_0_24074'

/*Some countries may not have that version*/
SET @Date = ISNULL(@date, GETDATE())

DELETE Financial.WarrantyMessage WHERE MessageId = -1

INSERT INTO Financial.WarrantyMessage
	(ContractNumber, DeliveredOn, AccountType, Department, SalePrice, CostPrice, BranchNo, WarrantyNo, WarrantyLength, MessageId)
SELECT 
	s.WarrantyContractNo, 
	s.ItemDeliveredOn, 
	a.accttype, 
	c.category, 
	s.WarrantySalePrice,
	s.WarrantyCostPrice,
	LEFT(s.CustomerAccount, 3) AS Branch,
	W.Number,
	s.WarrantyLength, 
	-1 /*MessageId for m,igrated warranties*/
FROM 
	Warranty.WarrantySale s 
	INNER JOIN acct a
		ON s.CustomerAccount = a.acctno
	INNER JOIN StockInfo st
		ON s.ItemId = st.Id
	INNER JOIN Code c
		ON st.category = CONVERT(Int, c.code)
		AND c.category IN ('PCE', 'PCF', 'PCO', 'PCW', 'PCDIS')
	INNER JOIN Warranty.Warranty w
		ON s.WarrantyId = w.Id
WHERE 
	s.WarrantyId IS NOT NULL
	AND DATEADD(Month, s.WarrantyLength, s.EffectiveDate) >= @Date