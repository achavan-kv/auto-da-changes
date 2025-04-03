-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

DELETE FROM [Sales].[OrderCustomer]
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Sales].[OrderLayaway]') AND type in (N'U'))
	DELETE FROM [Sales].[OrderLayaway]
GO

--Delete from sales.discountlimit
DELETE FROM Sales.DiscountLimit
GO

--Delete from Sales.OrderCreditCharges
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Sales].[OrderCreditCharges]') AND type in (N'U'))
	DELETE FROM [Sales].[OrderCreditCharges]
GO

--Delete from Sales.DeliveryDetail
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Sales].[DeliveryDetail]') AND type in (N'U'))
	DELETE FROM [Sales].[DeliveryDetail]
GO

--Delete from Sales.Customer
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Sales].[Customer]') AND type in (N'U'))
DELETE FROM [Sales].[Customer]
GO

-- Delete all records
DELETE FROM [Sales].[Order]
GO

DECLARE @MaxId int

SELECT @MaxId = NextHi
FROM HiLo
WHERE Sequence = 'CashAndGoAgrmtNo'

-- Clean POS agreement
DELETE FROM agreement
WHERE agrmtno >= @MaxId AND 
	acctno IN (SELECT DISTINCT c.acctno
				FROM CustAcct c
				WHERE c.custid LIKE '%PAID & TAKEN%')

-- Clean POS lineitem
DELETE FROM lineitem
WHERE agrmtno >= @MaxId AND 
	acctno IN (SELECT DISTINCT c.acctno
				FROM CustAcct c
				WHERE c.custid LIKE '%PAID & TAKEN%')

-- Clean POS delivery
DELETE FROM delivery
WHERE agrmtno >= @MaxId AND 
	acctno IN (SELECT DISTINCT c.acctno
				FROM CustAcct c
				WHERE c.custid LIKE '%PAID & TAKEN%')

-- Clean POS facttrans
DELETE FROM facttrans
WHERE agrmtno >= @MaxId AND 
	acctno IN (SELECT DISTINCT c.acctno
				FROM CustAcct c
				WHERE c.custid LIKE '%PAID & TAKEN%')

-- Clean POS fintrans
DELETE FROM fintrans
WHERE agrmtno >= @MaxId AND 
	acctno IN (SELECT DISTINCT c.acctno
				FROM CustAcct c
				WHERE c.custid LIKE '%PAID & TAKEN%')

-- Clean POS warranty.WarrantySale 
DELETE FROM warranty.WarrantySale 
WHERE AgreementNumber >= @MaxId AND 
	CustomerAccount IN (SELECT DISTINCT c.acctno
				FROM CustAcct c
				WHERE c.custid LIKE '%PAID & TAKEN%')

-- Clean POS warranty.WarrantyPotentialSale
DELETE FROM warranty.WarrantyPotentialSale
WHERE AgreementNumber >= @MaxId AND 
	CustomerAccount IN (SELECT DISTINCT c.acctno
				FROM CustAcct c
				WHERE c.custid LIKE '%PAID & TAKEN%')

-- Set current ID to "1"
DBCC CHECKIDENT ('[Sales].[Order]', RESEED, @MaxId)
DBCC CHECKIDENT ('[Sales].[OrderItem]', RESEED, 0)
DBCC CHECKIDENT ('[Sales].[OrderPayment]', RESEED, 0)
GO